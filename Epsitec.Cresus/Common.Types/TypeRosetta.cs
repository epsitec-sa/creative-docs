//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
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
					namedType = new AutomaticNamedType (systemType);
				}
			}

			System.Diagnostics.Debug.Assert (namedType != null);

			return namedType;
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

			StructuredRecord structuredRecord = value as StructuredRecord;

			if (structuredRecord != null)
			{
				return structuredRecord.StructuredType;
			}

			if (dependencyObject != null)
			{
				return dependencyObject.ObjectType;
			}

			return value.GetType ();
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

			public string Caption
			{
				get
				{
					return null;
				}
			}

			public string Description
			{
				get
				{
					return null;
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
			obj.ClearValueBase (TypeRosetta.TypeObjectProperty);
		}
		
		public static readonly DependencyProperty TypeObjectProperty = DependencyProperty.RegisterAttached ("TypeObject", typeof (object), typeof (TypeRosetta.Properties));
	}
}
