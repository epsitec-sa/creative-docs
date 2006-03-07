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

		public static DependencyObject FindFirst(DependencyObject root, string name)
		{
			return DependencyObjectTree.FindFirst (root, name, true, false);
		}
		private static DependencyObject FindFirst(DependencyObject root, string name, bool all, bool skipRoot)
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
				if (!skipRoot)
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

					if ((all) || (skipRoot) ||
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

			DependencyObject item = DependencyObjectTree.FindFirst (root, path[start], false, true);

			if (item != null)
			{
				return DependencyObjectTree.FindChild (item, path, start+1);
			}
			
			return null;
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

		public static DependencyProperty ParentProperty = DependencyProperty.RegisterReadOnly ("Parent", typeof (DependencyObject), typeof (DependencyObjectTree));
		public static DependencyProperty ChildrenProperty = DependencyProperty.RegisterReadOnly ("Children", typeof (ICollection<DependencyObject>), typeof (DependencyObjectTree));
		public static DependencyProperty HasChildrenProperty = DependencyProperty.RegisterReadOnly ("HasChildren", typeof (bool), typeof (DependencyObjectTree));
		public static DependencyProperty NameProperty = DependencyProperty.Register ("Name", typeof (string), typeof (DependencyObjectTree));
	}
}
