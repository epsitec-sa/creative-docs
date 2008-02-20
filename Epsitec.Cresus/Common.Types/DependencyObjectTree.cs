//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.DependencyObjectTree))]

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

		public static DependencyObjectTreeSnapshot CreateInheritedPropertyTreeSnapshot(DependencyObject root)
		{
			IList<DependencyProperty> properties = DependencyObjectTree.FindInheritedProperties (root);
			return DependencyObjectTree.CreatePropertyTreeSnapshot (root, properties);
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
		public static DependencyObjectTreeSnapshot CreatePropertyTreeSnapshot(DependencyObject root, IList<DependencyProperty> properties)
		{
			DependencyObjectTreeSnapshot snapshot = new DependencyObjectTreeSnapshot ();

			snapshot.Record (root, properties);
			snapshot.RecordSubtree (root, properties);

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

		public static DependencyObject FindFirst(DependencyObject root, string name)
		{
			//	Do a breadth-first search for an item named 'name' in the tree
			//	starting at 'root' (the search includes the root).
			
			return DependencyObjectTree.FindFirst (root, name, FindMode.SearchAll);
		}
		public static DependencyObject FindFirst(DependencyObject root, System.Text.RegularExpressions.Regex regex)
		{
			if (root == null)
			{
				return null;
			}

			//	Breadth first search for the named item.

			List<DependencyObject> roots = new List<DependencyObject> ();

			roots.Add (root);

			while (roots.Count > 0)
			{
				for (int i = 0; i < roots.Count; i++)
				{
					DependencyObject item = roots[i];
					string itemName = DependencyObjectTree.GetName (item);

					if ((itemName != null) &&
						(regex.IsMatch (itemName)))
					{
						return item;
					}
				}

				int n = roots.Count;

				for (int i = 0; i < n; i++)
				{
					DependencyObject item = roots[i];

					if (DependencyObjectTree.GetHasChildren (item))
					{
						roots.AddRange (DependencyObjectTree.GetChildren (item));
					}
				}

				roots.RemoveRange (0, n);
			}

			return null;
		}
		
		public static DependencyObject[] FindAll(DependencyObject root, string name)
		{
			//	Do a breadth-first search for all items named 'name' in the tree
			//	starting at 'root' (the search includes the root). If there are
			//	several matches, they will be returned in order (starting from
			//	the root, then level 1, then level 2, etc.)
			
			if (root == null)
			{
				return new DependencyObject[0];
			}

			List<DependencyObject> roots = new List<DependencyObject> ();
			List<DependencyObject> result = new List<DependencyObject> ();

			roots.Add (root);

			while (roots.Count > 0)
			{
				for (int i = 0; i < roots.Count; i++)
				{
					DependencyObject item = roots[i];

					if ((name == "*") ||
						(DependencyObjectTree.GetName (item) == name))
					{
						result.Add (item);
					}
				}

				int n = roots.Count;

				for (int i = 0; i < n; i++)
				{
					DependencyObject item = roots[i];

					if (DependencyObjectTree.GetHasChildren (item))
					{
						roots.AddRange (DependencyObjectTree.GetChildren (item));
					}
				}

				roots.RemoveRange (0, n);
			}

			return result.ToArray ();
		}
		public static DependencyObject[] FindAll(DependencyObject root, System.Text.RegularExpressions.Regex regex)
		{
			if (root == null)
			{
				return new DependencyObject[0];
			}

			//	Breadth first search for the named item.

			List<DependencyObject> roots = new List<DependencyObject> ();
			List<DependencyObject> result = new List<DependencyObject> ();

			roots.Add (root);

			while (roots.Count > 0)
			{
				for (int i = 0; i < roots.Count; i++)
				{
					DependencyObject item = roots[i];
					string itemName = DependencyObjectTree.GetName (item);

					if ((itemName != null) &&
						(regex.IsMatch (itemName)))
					{
						result.Add (item);
					}
				}

				int n = roots.Count;

				for (int i = 0; i < n; i++)
				{
					DependencyObject item = roots[i];

					if (DependencyObjectTree.GetHasChildren (item))
					{
						roots.AddRange (DependencyObjectTree.GetChildren (item));
					}
				}

				roots.RemoveRange (0, n);
			}

			return result.ToArray ();
		}
		
		public static DependencyObject FindChild(DependencyObject root, params string[] path)
		{
			//	Find the item by looking first for an item which matches the first
			//	element of the path, then a child which matches the second element,
			//	etc.
			//
			//	Named items which don't match the path are not further analysed.
			//	A tree [R] --> [A] --> [B] --> [C] in which a path of "A"/"C" is
			//	being searched won't return [C], as the search will abort when it
			//	encounters [B]. However, [R] --> [A] --> [] --> [C] will be OK,
			//	as items with no names (null) will be skipped.
			
			return DependencyObjectTree.FindChild (root, path, 0);
		}
		public static DependencyObject FindChild(DependencyObject root, string[] path, int start)
		{
			if (root == null)
			{
				return null;
			}
			if (start == path.Length)
			{
				return root;
			}

			DependencyObject item = DependencyObjectTree.FindFirst (root, path[start], FindMode.SearchMatchingChildrenOnly);

			if (item != null)
			{
				return DependencyObjectTree.FindChild (item, path, start+1);
			}
			
			return null;
		}

		public static DependencyObject FindParentDefiningProperty(DependencyObject item, DependencyProperty property)
		{
			if (item == null)
			{
				throw new System.ArgumentNullException ();
			}
			
			item = DependencyObjectTree.GetParent (item);
			
			while (item != null)
			{
				if (item.GetLocalValue (property) != UndefinedValue.Value)
				{
					break;
				}
				
				item = DependencyObjectTree.GetParent (item);
			}
			
			return item;
		}
		public static IList<DependencyProperty> FindInheritedProperties(DependencyObject item)
		{
			List<DependencyProperty> list = new List<DependencyProperty> ();
			DependencyObjectTree.FindInheritedProperties (item, list);
			return list;
		}
		
		public static DependencyObject GetParent(DependencyObject o)
		{
			if (o == null)
			{
				throw new System.ArgumentNullException ();
			}
			else
			{
				return o.GetValue (DependencyObjectTree.ParentProperty) as DependencyObject;
			}
		}
		public static string GetName(DependencyObject o)
		{
			if (o == null)
			{
				throw new System.ArgumentNullException ();
			}
			else
			{
				return o.GetValue (DependencyObjectTree.NameProperty) as string;
			}
		}
		
		public static ICollection<DependencyObject> GetChildren(DependencyObject o)
		{
			if (o == null)
			{
				throw new System.ArgumentNullException ();
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
				throw new System.ArgumentNullException ();
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

		#region FindMode Enumeration
		private enum FindMode
		{
			SearchAll,
			SearchMatchingChildrenOnly
		}
		#endregion

		#region Private Methods
		
		private static DependencyObject FindFirst(DependencyObject root, string name, FindMode mode)
		{
			if (root == null)
			{
				return null;
			}

			//	Breadth first search for the named item. Depending on the search mode,
			//	either consider the root as a candidate or only process its children.

			List<DependencyObject> roots = new List<DependencyObject> ();

			roots.Add (root);

			bool findAll = (mode == FindMode.SearchAll);
			bool skipRoot = (mode == FindMode.SearchMatchingChildrenOnly);

			while (roots.Count > 0)
			{
				if (skipRoot == false)
				{
					for (int i = 0; i < roots.Count; i++)
					{
						DependencyObject item = roots[i];

						if (DependencyObjectTree.GetName (item) == name)
						{
							return item;
						}
					}
				}

				int n = roots.Count;

				for (int i = 0; i < n; i++)
				{
					DependencyObject item = roots[i];

					//	If we are skipping the root, always analyse its children,
					//	regardless of the root's name.
					//	If SearchMatchingChildrenOnly has been specified, then we
					//	will not process the children if items which have a name.

					if ((findAll) || (skipRoot) ||
						(DependencyObjectTree.GetName (item) == null))
					{
						if (DependencyObjectTree.GetHasChildren (item))
						{
							roots.AddRange (DependencyObjectTree.GetChildren (item));
						}
					}
				}

				skipRoot = false;

				roots.RemoveRange (0, n);
			}

			return null;
		}
		
		private static void FindInheritedProperties(DependencyObject item, List<DependencyProperty> list)
		{
			while (item != null)
			{
				foreach (DependencyProperty property in item.DefinedProperties)
				{
					if (property.GetMetadata (item).InheritsValue)
					{
						if (list.Contains (property) == false)
						{
							list.Add (property);
						}
					}
				}

				item = DependencyObjectTree.GetParent (item);
			}
		}
		
		#endregion

		public static readonly DependencyProperty ParentProperty = DependencyProperty.RegisterReadOnly ("Parent", typeof (DependencyObject), typeof (DependencyObjectTree));
		public static readonly DependencyProperty ChildrenProperty = DependencyProperty.RegisterReadOnly ("Children", typeof (ICollection<DependencyObject>), typeof (DependencyObjectTree));
		public static readonly DependencyProperty HasChildrenProperty = DependencyProperty.RegisterReadOnly ("HasChildren", typeof (bool), typeof (DependencyObjectTree));
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register ("Name", typeof (string), typeof (DependencyObjectTree));
	}
}
