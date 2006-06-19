//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>TypeRosetta</c> class is the Rosetta Stone for everything which
	/// pertains to types. It is used to map between different type description
	/// mechanisms and well-known types, such as <see cref="T:System.Type"/>,
	/// <see cref="T:INamedType"/> or <see cref="T:IStructuredType"/>.
	/// </summary>
	public static class TypeRosetta
	{
		/// <summary>
		/// Gets the system type from type object.
		/// </summary>
		/// <param name="typeObject">The type object.</param>
		/// <returns>The system type.</returns>
		public static System.Type GetSystemTypeFromTypeObject(object typeObject)
		{
			System.Type sytemType = typeObject as System.Type;

			if (sytemType != null)
			{
				return sytemType;
			}

			INamedType namedType = typeObject as INamedType;

			if (namedType != null)
			{
				return namedType.SystemType;
			}

			DependencyProperty dependencyProperty = typeObject as DependencyProperty;

			if (dependencyProperty != null)
			{
				return dependencyProperty.PropertyType;
			}

			DependencyObjectType dependencyObjectType = typeObject as DependencyObjectType;

			if (dependencyObjectType != null)
			{
				return dependencyObjectType.SystemType;
			}
			
			//	The type object does not map to any known type description object.
			
			return null;
		}

		/// <summary>
		/// Gets the named type from type object.
		/// </summary>
		/// <param name="typeObject">The type object.</param>
		/// <returns>The named type.</returns>
		public static INamedType GetNamedTypeFromTypeObject(object typeObject)
		{
			INamedType namedType = typeObject as INamedType;

			if (namedType == null)
			{
				System.Type systemType = TypeRosetta.GetSystemTypeFromTypeObject (typeObject);

				if (systemType == null)
				{
					//	No underlying System.Type exists for the specified type object.
					//	This is not a valid type object.

					throw new Exceptions.InvalidTypeObjectException (typeObject);
				}
				else
				{
					if (systemType.IsEnum)
					{
						namedType = new EnumType (systemType);
					}
					else
					{
						namedType = new AutomaticNamedType (systemType);
					}
				}
			}

			System.Diagnostics.Debug.Assert (namedType != null);

			return namedType;
		}

		/// <summary>
		/// Gets the structured type from type object. The <see cref="T:IStructuredType"/> can
		/// then be used to query the type about its fields.
		/// </summary>
		/// <param name="type">The type object to convert.</param>
		/// <returns>The structured type compatible with the type object, or <c>null</c> if it
		/// canoot be derived from the type object.</returns>
		public static IStructuredType GetStructuredTypeFromTypeObject(object type)
		{
			IStructuredType structuredType = type as IStructuredType;

			if (structuredType != null)
			{
				return structuredType;
			}
			
			IStructuredTypeProvider structuredTypeProvider = type as IStructuredTypeProvider;

			if (structuredTypeProvider != null)
			{
				return structuredTypeProvider.GetStructuredType ();
			}

			return null;
		}
		
		/// <summary>
		/// Gets the type object from a value.
		/// </summary>
		/// <param name="value">The value to derive a type from.</param>
		/// <returns>The type object.</returns>
		public static object GetTypeObjectFromValue(object value)
		{
			if (value == null)
			{
				return null;
			}

			DependencyObject dependencyObject = value as DependencyObject;

			if (dependencyObject != null)
			{
				object type = TypeRosetta.GetTypeObject (dependencyObject);

				if (type != null)
				{
					return type;
				}
			}
			
			IStructuredTypeProvider structuredTypeProvider = value as IStructuredTypeProvider;

			if (structuredTypeProvider != null)
			{
				return structuredTypeProvider.GetStructuredType ();
			}
			
			if (dependencyObject != null)
			{
				return dependencyObject.ObjectType;
			}

			IStructuredData structuredData = value as IStructuredData;

			if (structuredData != null)
			{
				return new DynamicStructuredType (structuredData);
			}

			return value.GetType ();
		}

		/// <summary>
		/// Verifies the validity of the value (proper type and valid with respect
		/// to <see cref="T:IDataConstraint"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="type">The type object (may not be <c>null</c>).</param>
		/// <returns><c>true</c> if the value is compatible with the specified type, <c>false</c> otherwise.</returns>
		public static bool IsValidValue(object value, object typeObject)
		{
			if (typeObject == null)
			{
				throw new System.ArgumentNullException ("Null type specified");
			}
			
			INamedType      targetType       = TypeRosetta.GetNamedTypeFromTypeObject (typeObject);
			System.Type     targetSysType    = TypeRosetta.GetSystemTypeFromTypeObject (typeObject);
			IDataConstraint targetConstraint = targetType as IDataConstraint;

			System.Diagnostics.Debug.Assert (targetType != null);

			//	A DependencyProperty implements IDataConstraint too, so there is
			//	no need to check explicitely for dependency properties :
			
			if (targetConstraint != null)
			{
				return targetConstraint.IsValidValue (value);
			}

			if (value == null)
			{
				//	Only reference types can be set to null.

				//	TODO: check for nullable types too ?

				return targetSysType.IsClass;
			}
			else
			{
				return targetSysType.IsAssignableFrom (value.GetType ());
			}
		}
		


		#region AutomaticNamedType Class

		private class AutomaticNamedType : INamedType
		{
			public AutomaticNamedType(System.Type type)
			{
				this.type = type;
			}
			
			#region INamedType Members

			public System.Type SystemType
			{
				get
				{
					return this.type;
				}
			}

			#endregion

			#region INameCaption Members

			public long CaptionId
			{
				get
				{
					return -1;
				}
			}

			#endregion

			#region IName Members

			public string Name
			{
				get
				{
					return this.type.FullName;
				}
			}

			#endregion

			private System.Type type;
		}
		
		#endregion

		#region Tools Class

		private class Properties : DependencyObject
		{
		}

		#endregion

		/// <summary>
		/// Gets the attached type object from the dependency object.
		/// </summary>
		/// <param name="obj">The dependency object.</param>
		/// <returns>The attached type object or <c>null</c> if none can be found.</returns>
		public static object GetTypeObject(DependencyObject obj)
		{
			return obj.GetValue (TypeRosetta.TypeObjectProperty);
		}

		/// <summary>
		/// Sets the attached type object to the dependency object.
		/// </summary>
		/// <param name="obj">The dependency object.</param>
		/// <param name="value">The type object to attach.</param>
		public static void SetTypeObject(DependencyObject obj, object value)
		{
			obj.SetValue (TypeRosetta.TypeObjectProperty, value);
		}

		/// <summary>
		/// Clears the attached type object from the dependency object.
		/// </summary>
		/// <param name="obj">The dependency object.</param>
		public static void ClearTypeObject(DependencyObject obj)
		{
			obj.ClearValue (TypeRosetta.TypeObjectProperty);
		}

		/// <summary>
		/// Verifies if the type implements the specified interface.
		/// </summary>
		/// <param name="systemType">Type to check.</param>
		/// <param name="interfaceType">Type of the interface to find.</param>
		/// <returns><c>true</c> if the interface is found; otherwise, <c>false</c>.</returns>
		public static bool DoesTypeImplementInterface(System.Type systemType, System.Type interfaceType)
		{
			if (systemType == interfaceType)
			{
				return true;
			}
			else
			{
				foreach (System.Type type in systemType.GetInterfaces ())
				{
					if (type == interfaceType)
					{
						return true;
					}
				}
			}

			return false;
		}
		
		
		public static readonly DependencyProperty TypeObjectProperty = DependencyProperty.RegisterAttached ("TypeObject", typeof (object), typeof (TypeRosetta.Properties));
	}
}
