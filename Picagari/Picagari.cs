﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Picagari.Attributes;
using Picagari.Exceptions;
using Picagari.ScopeObjects;
using PostConstruct = Picagari.PostConstructContainer.PostConstruct;

namespace Picagari
{
    /// <summary>
    /// The Picagari class handles injecting all marked fields and properties
    /// of the target class.
    /// </summary>
    public static class Picagari
    {
        private static readonly Dictionary<Type, object> _applicationScopedObjects = new Dictionary<Type, object>();
        private static readonly Dictionary<Type, MethodInfo> _knownProducers = new Dictionary<Type, MethodInfo>();
        private static readonly List<Type> _knownTypes = new List<Type>();

        private static readonly Dictionary<RequestScopedKey, Dictionary<Type, Object>> _requestScopedObjects =
        new Dictionary<RequestScopedKey, Dictionary<Type, object>>();

        private static readonly Dictionary<SessionScopedKey, Dictionary<Type, Object>> _sessionScopedObjects =
        new Dictionary<SessionScopedKey, Dictionary<Type, object>>();

        /// <summary>
        /// Start recursively constructing and injecting members in this object.
        /// </summary>
        /// <param name="obj">The object in which to begin injecting marked members.</param>
        /// <returns>Return the object after injection is compled. Useful for getting a reference to manually constructed objects that get bootstrapped.</returns>
        /// <exception cref="PicagariException">Throws when requirements for injection are not satisfied.</exception>
        public static object Start( object obj )
        {
            var postConstructContainer = new PostConstructContainer();
            //scanHierarchy( obj.GetType() );
            performFullAssemblyScan();
            injectMembers( obj, getInjectableMembers( obj ), new List<Type>(), postConstructContainer );
            postConstructContainer.InvokePostConstruct();
            return obj;
        }

        public static void EndRequest( RequestScopedKey requestKey )
        {
            _requestScopedObjects.Remove( requestKey );
        }

        public static void EndSession( SessionScopedKey sessionKey )
        {
            _sessionScopedObjects.Remove( sessionKey );
        }

        private static IEnumerable<MemberInfo> getInjectableMembers( object obj )
        {
            return obj.GetType()
                      .GetMembers( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic )
                      .Where( m => getAttribute<InjectAttribute>( m ) != null );
        }

        private static void injectMembers( object parent, IEnumerable<MemberInfo> objs, List<Type> parentTypesList, PostConstructContainer postConstructContainer )
        {
            MethodInfo requestKeyProducer = null;
            MethodInfo sessionKeyProducer = null;
            SessionScopedKey sessionScopedKey = null;
            RequestScopedKey requestScopedKey = null;

            foreach ( var member in objs )
            {
                parentTypesList.Add( parent.GetType() );

                var type = getMemberType( member );
                object value = null;
                var injectionPoint = new InjectionPoint( parent, member, getAttribute<InjectAttribute>( member ) );
                SessionScopedAttribute sessionScopedAttribute;
                RequestScopedAttribute requestScopedAttribute;

                if ( parentTypesList.Contains( type ) )
                {
                    throw new PicagariException( PicagariException.InjectWillCauseInfiniteRecursion, new[] {member, parent} );
                }

                if ( isApplicationScopedTypeAvailable( parent, type, member ) )
                {
                    continue;
                }

                prepareScopeKey( type, out requestScopedAttribute, ref requestKeyProducer, ref requestScopedKey, injectionPoint );

                if ( isRequestScopedTypeAvailable( parent, type, member, requestScopedKey ) )
                {
                    continue;
                }

                prepareScopeKey( type, out sessionScopedAttribute, ref sessionKeyProducer, ref sessionScopedKey, injectionPoint );

                if ( isSessionScopedTypeAvailable( parent, type, member, sessionScopedKey ) )
                {
                    continue;
                }

                if ( !_knownTypes.Contains( type ) )
                {
                    scanHierarchy( type );
                }

                if ( _knownProducers.ContainsKey( type ) )
                {
                    var producer = _knownProducers[ type ];
                    value = producer.Invoke( null, new[] {injectionPoint} );
                    setMemberValue( parent, member, value );
                }

                if ( value == null )
                {
                    value = getInjectedValue( parent, member, ref type );
                }

                if ( getAttribute<ApplicationScopedAttribute>( type ) != null )
                {
                    _applicationScopedObjects[ type ] = value;
                }

                //# If this fires, this means it's the first object for this scope to be inserted.
                if ( requestScopedAttribute != null && requestScopedKey != null )
                {
                    _requestScopedObjects[ requestScopedKey ] = new Dictionary<Type, object> {{type, value}};
                }

                //# If this fires, this means it's the first object for this scope to be inserted.
                if ( sessionScopedAttribute != null && sessionScopedKey != null )
                {
                    _sessionScopedObjects[ sessionScopedKey ] = new Dictionary<Type, object> {{type, value}};
                }

                //# Recurse
                injectMembers( value, getInjectableMembers( value ), parentTypesList, postConstructContainer );
                setPostConstructDelegates( type, value, postConstructContainer );
                parentTypesList.Clear();
            }
        }

        private static void prepareScopeKey<TA, TK>( Type type, out TA attribute, ref MethodInfo producer, ref TK key, InjectionPoint injectionPoint ) where TA : Attribute
        where TK : ScopeKey
        {
            attribute = getAttribute<TA>( type );
            if ( attribute == null )
            {
                return;
            }
            //# We can safely break out of this early if there is a scope key (no need to check for producer and produce a key)
            if ( key != null )
            {
                return;
            }

            if ( producer == null )
            {
                _knownProducers.TryGetValue( typeof ( TK ), out producer );
            }

            if ( producer == null )
            {
                return;
            }

            if ( key == null )
            {
                key = (TK) producer.Invoke( null, new[] {injectionPoint} );
            }
        }

        private static bool isSessionScopedTypeAvailable( object parent, Type type, MemberInfo member, SessionScopedKey sessionScopedKey )
        {
            //# We should refactor above and below this code into separate methods. Top for preparing reusables, below for assigning a reusable
            if ( sessionScopedKey == null || !_sessionScopedObjects.ContainsKey( sessionScopedKey ) )
            {
                return false;
            }
            if ( !_sessionScopedObjects[ sessionScopedKey ].ContainsKey( type ) )
            {
                return false;
            }

            setMemberValue( parent, member, _sessionScopedObjects[ sessionScopedKey ][ type ] );

            return true;
        }

        private static bool isRequestScopedTypeAvailable( object parent, Type type, MemberInfo member, RequestScopedKey requestScopedKey )
        {
            if ( requestScopedKey == null || !_requestScopedObjects.ContainsKey( requestScopedKey ) )
            {
                return false;
            }

            if ( !_requestScopedObjects[ requestScopedKey ].ContainsKey( type ) )
            {
                return false;
            }

            setMemberValue( parent, member, _requestScopedObjects[ requestScopedKey ][ type ] );

            return true;
        }

        private static bool isApplicationScopedTypeAvailable( object parent, Type type, MemberInfo member )
        {
            if ( !_applicationScopedObjects.ContainsKey( type ) )
            {
                return false;
            }
            setMemberValue( parent, member, _applicationScopedObjects[ type ] );
            return true;
        }

        private static void setInjectedType( ref Type type, InjectAttribute injectAttribute )
        {
            var initialType = type;
            var allImplementations = _knownTypes.Where( t => t != initialType && initialType.IsAssignableFrom( t ) );

            if ( allImplementations.Count() > 1 )
            {
                if ( injectAttribute.AlternateType != null )
                {
                    type = injectAttribute.AlternateType;
                }
                else
                {
                    var defaultImplementation = allImplementations.FirstOrDefault( t => getAttribute<DefaultAttribute>( t ) != null );

                    if ( defaultImplementation == null )
                    {
                        throw new PicagariException( PicagariException.NoDefaultNoProducer, new[] {type} );
                    }

                    type = defaultImplementation;
                }
            }
        }

        private static object getInjectedValue( object parent, MemberInfo member, ref Type type )
        {
            if ( type.IsInterface || type.IsAbstract )
            {
                var injectAttribute = getAttribute<InjectAttribute>( member );
                setInjectedType( ref type, injectAttribute );
            }

            object value = null;

            try
            {
                value = Activator.CreateInstance( type );
            }
            catch ( MissingMethodException e )
            {
                if ( e.Message.ToLower().Contains( "interface" ) )
                {
                    throw new PicagariException( PicagariException.CannotConstructInterfaceByItself, new[] {type}, e );
                }
                if ( e.Message.ToLower().Contains( "constructor" ) )
                {
                    throw new PicagariException( PicagariException.NeedsParameterlessConstructor, new[] {type}, e );
                }
            }

            if ( value == null )
            {
                throw new PicagariException( PicagariException.UnknownInjectionError, new[] {type} );
            }

            setMemberValue( parent, member, value );
            return value;
        }

        private static object getMemberValue( object parent, MemberInfo member )
        {
            switch ( member.MemberType )
            {
                case MemberTypes.Field:
                    return ( (FieldInfo) member ).GetValue( parent );
                case MemberTypes.Property:
                    return ( (PropertyInfo) member ).GetValue( parent );
            }

            return null;
        }

        private static void setMemberValue( object parent, MemberInfo member, object value )
        {
            switch ( member.MemberType )
            {
                case MemberTypes.Field:
                    ( (FieldInfo) member ).SetValue( parent, value );
                    break;
                case MemberTypes.Property:
                    ( (PropertyInfo) member ).SetValue( parent, value );
                    break;
            }
        }

        private static Type getMemberType( MemberInfo member )
        {
            switch ( member.MemberType )
            {
                case MemberTypes.Field:
                    return ( (FieldInfo) member ).FieldType;
                case MemberTypes.Property:
                    return ( (PropertyInfo) member ).PropertyType;
            }

            return null;
        }

        private static void performFullAssemblyScan()
        {
            var assemblies = AppDomain.CurrentDomain
                                      .GetAssemblies()
                                      .Where( a =>
                                              !a.FullName.StartsWith( "System." ) &&
                                              !a.FullName.StartsWith( "System," ) &&
                                              !a.FullName.StartsWith( "mscorlib," ) );
            foreach ( var assembly in assemblies )
            {
                var newTypes = assembly.GetTypes().Where( t => !_knownTypes.Contains( t ) ).ToList();
                var producers = newTypes.SelectMany( t => t.GetMethods().Where( m => getAttribute<ProducesAttribute>( m ) != null ) );

                _knownTypes.AddRange( newTypes );

                foreach ( var producer in producers )
                {
                    var productType = getAttribute<ProducesAttribute>( producer ).QualifiedType ?? producer.ReturnType;

                    if ( _knownProducers.ContainsKey( productType ) )
                    {
                        var known = _knownProducers[ productType ];
                        throw new PicagariException( PicagariException.TooManyProducersForType, new object[] {productType, known.ReflectedType, known.Name} );
                    }

                    _knownProducers.Add( productType, producer );
                }
            }
        }

        private static void scanHierarchy( Type type )
        {
            Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            var newTypes = Assembly.GetAssembly( type )
                                   .GetTypes()
                                   .Where( t => !_knownTypes.Contains( t ) )
                                   .ToList();
            var producers = newTypes.SelectMany( t => t.GetMethods()
                                                       .Where( n => getAttribute<ProducesAttribute>( n ) != null ) );

            _knownTypes.AddRange( newTypes );
            producers.ToList().ForEach( delegate( MethodInfo m )
            {
                var productType = getAttribute<ProducesAttribute>( m ).QualifiedType ?? m.ReturnType;
                try
                {
                    _knownProducers.Add( productType, m );
                }
                catch ( ArgumentException e )
                {
                    var known = _knownProducers[ productType ];
                    throw new PicagariException( PicagariException.TooManyProducersForType, new object[] {productType, known.ReflectedType, known.Name}, e );
                }
            } );
        }

        private static void setPostConstructDelegates( Type type, object value, PostConstructContainer postConstructContainer )
        {
            var postConstructMethod = type.GetMethods( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic )
                                          .FirstOrDefault( m => getAttribute<PostConstructAttribute>( m ) != null );
            if ( postConstructMethod == null )
            {
                return;
            }

            try
            {
                var postConstructDelegate = (PostConstruct) postConstructMethod.CreateDelegate( typeof ( PostConstruct ), value );
                postConstructContainer.AddDelegateToPostConstruct( postConstructDelegate );
            }
            catch ( Exception e )
            {
                throw new PicagariException( PicagariException.CannotUseMethodAsPostConstructDelegate, new object[] {type, postConstructMethod}, e );
            }
        }

        private static T getAttribute<T>( MemberInfo member ) where T : Attribute
        {
            return (T) member.GetCustomAttribute( typeof ( T ), false );
        }
    }
}
