using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Picagari.Attributes;
using Picagari.Exceptions;

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
			scanHierarchy( obj.GetType() );
			injectMembers( obj, getInjectableMembers( obj ), new List<Type>() );
			return obj;
		}

		private static IEnumerable<MemberInfo> getInjectableMembers( object obj )
		{
			return obj.GetType()
					  .GetMembers( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic )
					  .Where( m => getAttribute<InjectAttribute>( m ) != null );
		}

		private static void injectMembers( object parent, IEnumerable<MemberInfo> objs, List<Type> parentTypesList )
		{
			foreach ( var member in objs )
			{
				parentTypesList.Add( parent.GetType() );
				var type = getMemberType( member );
				object value = null;

				if ( parentTypesList.Contains( type ) )
				{
					throw new PicagariException( "Injecting the member " + member + " inside " + parent + " would cause infinite recursion." );
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
				injectMembers( value, getInjectableMembers( value ), parentTypesList );
				doPostConstruct( type, value );
				parentTypesList.Clear();
			}
		}

		private static void doPostConstruct( Type type, object value )
		{
			var postConstructMethod = type.GetMethods( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic )
										  .FirstOrDefault( m => getAttribute<PostConstructAttribute>( m ) != null );

			if ( postConstructMethod != null )
			{
				//TODO Handle invocation exception.
				postConstructMethod.Invoke( value, null );
			}
		}

		private static T getAttribute<T>( MemberInfo member ) where T : Attribute
		{
			return (T) member.GetCustomAttribute( typeof ( T ), false );
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
					throw new PicagariException( "You should create a producer for, or have at least one class that implements " + type + ".", e );
				}
				if ( e.Message.ToLower().Contains( "constructor" ) )
				{
					throw new PicagariException( "NetCDI requires a parameterless constructor in order to inject " + type + ".", e );
				}
			}

			if ( value == null )
			{
				throw new PicagariException( "NetCDI somehow came up injecting a null value for " + type + ". Is there a producer for this type?" );
			}

			setMemberValue( parent, member, value );
			return value;
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
						throw new PicagariException( "More than one implementation exists for " + type + ", and no default has been specified for injection." );
					}

					type = defaultImplementation;
				}
			}
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
					throw new PicagariException( "There can only be one producer for " + productType + ", and there is already one at " + known.ReflectedType + "." + known.Name, e );
				}
			} );
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
	}
}
