//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The ObjectTree provides a few basic properties used when navigating
	/// through object hierarchies.
	/// </summary>
	public sealed class ObjectTree : Object
	{
		private ObjectTree()
		{
		}

		public static ObjectTreeSnapshot CreatePropertyTreeSnapshot(Object root, Property property)
		{
			ObjectTreeSnapshot snapshot = new ObjectTreeSnapshot ();

			snapshot.Record (root, property);
			snapshot.RecordSubtree (root, property);

			return snapshot;
		}
		public static ObjectTreeSnapshot CreatePropertyTreeSnapshot(Object root, Property property1, Property property2)
		{
			ObjectTreeSnapshot snapshot = new ObjectTreeSnapshot ();

			snapshot.Record (root, property1, property2);
			snapshot.RecordSubtree (root, property1, property2);

			return snapshot;
		}
		public static ObjectTreeSnapshot CreatePropertySubtreeSnapshot(Object root, Property property)
		{
			ObjectTreeSnapshot snapshot = new ObjectTreeSnapshot ();

			snapshot.RecordSubtree (root, property);

			return snapshot;
		}
		public static ObjectTreeSnapshot CreatePropertySubtreeSnapshot(Object root, Property property1, Property property2)
		{
			ObjectTreeSnapshot snapshot = new ObjectTreeSnapshot ();

			snapshot.RecordSubtree (root, property1, property2);

			return snapshot;
		}
		
		public static Object GetParent(Object o)
		{
			if (o == null)
			{
				return null;
			}
			else
			{
				return o.GetValue (ObjectTree.ParentProperty) as Object;
			}
		}
		
		public static ICollection<Object> GetChildren(Object o)
		{
			if (o == null)
			{
				return null;
			}
			else
			{
				return o.GetValue (ObjectTree.ChildrenProperty) as ICollection<Object>;
			}
		}
		public static bool GetHasChildren(Object o)
		{
			//	Find out whether the specified Object contains any children; this
			//	can be derived from its HasChildren property. But maybe it has no
			//	such property at all, in which case we must return false too.
			
			if (o == null)
			{
				return false;
			}
			else
			{
				object value = o.GetValue (ObjectTree.HasChildrenProperty);
				
				if (value is bool)
				{
					return (bool) value;
				}
				else
				{
					return false;
				}
			}
		}

		public static Property ParentProperty = Property.RegisterReadOnly ("Parent", typeof (Object), typeof (ObjectTree));
		public static Property ChildrenProperty = Property.RegisterReadOnly ("Children", typeof (ICollection<Object>), typeof (ObjectTree));
		public static Property HasChildrenProperty = Property.RegisterReadOnly ("HasChildren", typeof (bool), typeof (ObjectTree));
	}
}
