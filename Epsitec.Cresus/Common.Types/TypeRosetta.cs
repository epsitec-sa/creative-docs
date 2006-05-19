//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public static class TypeRosetta
	{
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
				return structuredRecord.StructuredRecordType;
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

		public static object GetTypeObject(DependencyObject obj)
		{
			return obj.GetValue (TypeRosetta.TypeObjectProperty);
		}

		public static void SetTypeObject(DependencyObject obj, object value)
		{
			obj.SetValue (TypeRosetta.TypeObjectProperty, value);
		}

		public static void ClearTypeObject(DependencyObject obj)
		{
			obj.ClearValueBase (TypeRosetta.TypeObjectProperty);
		}
		
		public static readonly DependencyProperty TypeObjectProperty = DependencyProperty.RegisterAttached ("TypeObject", typeof (object), typeof (TypeRosetta.Properties));
	}
}
