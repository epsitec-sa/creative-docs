//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The DependencyObjectTree provides a few basic properties used when navigating
	/// through object hierarchies.
	/// </summary>
	public sealed class DependencyObjectTree : DependencyObject
	{
		private DependencyObjectTree()
		{
		}

		public static DependencyObjectTreeSnapshot CreatePropertyTreeSnapshot(DependencyObject root, DependencyProperty property)
		{
			DependencyObjectTreeSnapshot snapshot = new DependencyObjectTreeSnapshot ();

			snapshot.Record (root, property);
			snapshot.RecordSubtree (root, property);

			return snapshot;
		}
		public static DependencyObjectTreeSnapshot CreatePropertyTreeSnapshot(DependencyObject root, DependencyProperty property1, DependencyProperty property2)
		{
			DependencyObjectTreeSnapshot snapshot = new DependencyObjectTreeSnapshot ();

			snapshot.Record (root, property1, property2);
			snapshot.RecordSubtree (root, property1, property2);

			return snapshot;
		}
		public static DependencyObjectTreeSnapshot CreatePropertySubtreeSnapshot(DependencyObject root, DependencyProperty property)
		{
			DependencyObjectTreeSnapshot snapshot = new DependencyObjectTreeSnapshot ();

			snapshot.RecordSubtree (root, property);

			return snapshot;
		}
		public static DependencyObjectTreeSnapshot CreatePropertySubtreeSnapshot(DependencyObject root, DependencyProperty property1, DependencyProperty property2)
		{
			DependencyObjectTreeSnapshot snapshot = new DependencyObjectTreeSnapshot ();

			snapshot.RecordSubtree (root, property1, property2);

			return snapshot;
		}
		
		public static DependencyObject GetParent(DependencyObject o)
		{
			if (o == null)
			{
				return null;
			}
			else
			{
				return o.GetValue (DependencyObjectTree.ParentProperty) as DependencyObject;
			}
		}
		
		public static ICollection<DependencyObject> GetChildren(DependencyObject o)
		{
			if (o == null)
			{
				return null;
			}
			else
			{
				return o.GetValue (DependencyObjectTree.ChildrenProperty) as ICollection<DependencyObject>;
			}
		}
		public static bool GetHasChildren(DependencyObject o)
		{
			//	Find out whether the specified DependencyObject contains any children; this
			//	can be derived from its HasChildren property. But maybe it has no
			//	such property at all, in which case we must return false too.
			
			if (o == null)
			{
				return false;
			}
			else
			{
				object value = o.GetValue (DependencyObjectTree.HasChildrenProperty);
				
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

		public static DependencyProperty ParentProperty = DependencyProperty.RegisterReadOnly ("Parent", typeof (DependencyObject), typeof (DependencyObjectTree));
		public static DependencyProperty ChildrenProperty = DependencyProperty.RegisterReadOnly ("Children", typeof (ICollection<DependencyObject>), typeof (DependencyObjectTree));
		public static DependencyProperty HasChildrenProperty = DependencyProperty.RegisterReadOnly ("HasChildren", typeof (bool), typeof (DependencyObjectTree));
	}
}
