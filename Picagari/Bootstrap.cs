using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Picagari.Attributes;
using Picagari.Exceptions;
using PostConstruct = Picagari.PostConstructContainer.PostConstruct;

namespace Picagari
{
	/// <summary>
	/// The Bootstrap class handles injecting all marked fields and properties
	/// of the target class.
	/// </summary>
	public static class Bootstrap
	{
		private static readonly Dictionary<Type, object> _applicationScopedObjects = new Dictionary<Type, object>();
		private static readonly Dictionary<Type, MethodInfo> _knownProducers = new Dictionary<Type, MethodInfo>();
		private static readonly List<Type> _knownTypes = new List<Type>();

		/// <summary>
		/// Start recursively constructing and injecting members in this object.
		/// </summary>
		/// <param name="obj">The object in which to begin injecting marked members.</param>
		/// <returns>Return the object after injection is compled. Useful for getting a reference to manually constructed objects that get bootstrapped.</returns>
		/// <exception cref="PicagariException">Throws when requirements for injection are not satisfied.</exception>
		public static object Start( object obj )
		{
			var postConstructContainer = new PostConstructContainer();
			scanHierarchy( obj.GetType() );
			injectMembers( obj, getInjectableMembers( obj ), new List<Type>(), postConstructContainer );
			postConstructContainer.InvokePostConstruct();
			return obj;
		}

		private static IEnumerable<MemberInfo> getInjectableMembers( object obj )
		{
			return obj.GetType()
					  .GetMembers( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic )
					  .Where( m => getAttribute<InjectAttribute>( m ) != null );
		}

		private static void injectMembers( object parent, IEnumerable<MemberInfo> objs, List<Type> parentTypesList, PostConstructContainer postConstructContainer )
		{
			foreach ( var member in objs )
			{
				parentTypesList.Add( parent.GetType() );
				var type = getMemberType( member );
				object value = null;

				if ( parentTypesList.Contains( type ) )
				{
					throw new PicagariException( PicagariException.InjectWillCauseInfiniteRecursion, new[] {member, parent} );
				}

				if ( _applicationScopedObjects.ContainsKey( type ) )
				{
					setMemberValue( parent, member, _applicationScopedObjects[ type ] );
					continue;
				}

				if ( !_knownTypes.Contains( type ) )
				{
					scanHierarchy( type );
				}

				if ( _knownProducers.ContainsKey( type ) )
				{
					var producer = _knownProducers[ type ];
					var injectionPoint = new InjectionPoint(
					parent,
					member,
					getAttribute<InjectAttribute>( member )
					);
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

				//# Recurse
				injectMembers( value, getInjectableMembers( value ), parentTypesList, postConstructContainer );
				setPostConstructDelegates( type, value, postConstructContainer );
				parentTypesList.Clear();
			}
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

		private static void scanHierarchy( Type type )
		{
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
