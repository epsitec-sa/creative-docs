//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Collections
{
	/// <summary>
	/// La classe FlatChildrenCollection stocke un ensemble de widgets de manière
	/// ordonnée.
	/// </summary>
	public class FlatChildrenCollection : IList<Visual>
	{
		internal FlatChildrenCollection(Visual host)
		{
			this.host = host;
		}

		public Widget[] Widgets
		{
			get
			{
				Visual[] visuals = this.visuals.ToArray ();
				Widget[] widgets = new Widget[visuals.Length];
				visuals.CopyTo (widgets, 0);
				return widgets;
			}
		}
		
		class Snapshot
		{
			private Snapshot()
			{
			}

			public void NotifyChanges()
			{
			}
			
			public static Snapshot RecordTree(params Visual[] visuals)
			{
				Snapshot snapshot = new Snapshot ();
				return snapshot;
			}
		}
		
		private void NotifyChanges(Snapshot snapshot)
		{
			snapshot.NotifyChanges ();
		}

		private void AttachVisual(Visual visual)
		{
			//	Attache le visual à son nouveau parent; il est au préalable détaché
			//	de son ancien parent. Cette méthode ne s'occupe pas de la question
			//	des propriétés héritées.

			System.Diagnostics.Debug.Assert (visual != null);
			System.Diagnostics.Debug.Assert (this.visuals.Contains (visual));
			
			Visual parent = visual.Parent;

			if (parent == null)
			{
				//	Le visual n'a pas de parent, ce qui simplifie la gestion. Il
				//	suffit de lui en attribuer un :

				visual.SetParentVisual (this.host);
			}
			else if (this.host == parent)
			{
				//	Le visual a déjà le même parent; il n'y a donc rien à faire
				//	au niveau des liens.
			}
			else
			{
				//	Le visual est encore attaché à un parent. Il faut commencer par
				//	le détacher de son ancien parent, puis notifier l'ancien parent
				//	du changement :

				FlatChildrenCollection others = parent.Children;

				others.visuals.Remove (visual);
				others.DetachVisual (visual);

				System.Diagnostics.Debug.Assert (visual.Parent == null);
				
				visual.SetParentVisual (this.host);
			}
			
			System.Diagnostics.Debug.Assert (visual.Parent == this.host);
			
			this.NotifyChanged ();
		}
		private void DetachVisual(Visual visual)
		{
			//	Détache le visual de son parent.
			
			System.Diagnostics.Debug.Assert (visual != null);
			System.Diagnostics.Debug.Assert (this.visuals.Contains (visual) == false);
			System.Diagnostics.Debug.Assert (this.host == visual.Parent);
			
			visual.SetParentVisual (null);

			System.Diagnostics.Debug.Assert (visual.Parent == null);
			
			this.NotifyChanged ();
		}

		private void NotifyChanged()
		{
			this.host.NotifyChildrenChanged ();
		}
		
#region IList<Visual> Members

		public Visual							this[int index]
		{
			get
			{
				return this.visuals[index];
			}
			set
			{
				if (value == null)
				{
					throw new System.ArgumentNullException (FlatChildrenCollection.NullVisualMessage);
				}

				Visual oldValue = this.visuals[index];
				Visual newValue = value;
				
				if (oldValue != newValue)
				{
					if (value.Parent == this.host)
					{
						throw new System.InvalidOperationException (FlatChildrenCollection.NotTwiceMessage);
					}
					
					Snapshot snapshot = Snapshot.RecordTree (oldValue, newValue);

					this.visuals[index] = null;
					this.DetachVisual (oldValue);
					this.visuals[index] = value;
					this.AttachVisual (newValue);

					this.NotifyChanges (snapshot);
				}
			}
		}

		public int IndexOf(Visual item)
		{
			return this.visuals.IndexOf (item);
		}

		

		public void Insert(int index, Visual item)
		{
			if (item == null)
			{
				throw new System.ArgumentNullException (FlatChildrenCollection.NullVisualMessage);
			}
			if (item.Parent == this.host)
			{
				throw new System.InvalidOperationException (FlatChildrenCollection.NotTwiceMessage);
			}
			
			Snapshot snapshot = Snapshot.RecordTree (item);

			this.visuals.Insert (index, item);
			this.AttachVisual (item);

			this.NotifyChanges (snapshot);
		}

		public void RemoveAt(int index)
		{
			Visual item = this.visuals[index];

			Snapshot snapshot = Snapshot.RecordTree (item);

			this.visuals.RemoveAt (index);
			this.DetachVisual (item);

			this.NotifyChanges (snapshot);
		}

		#endregion

		#region ICollection<Visual> Members

		public int								Count
		{
			get
			{
				return this.visuals.Count;
			}
		}
		public bool								IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public void Add(Visual item)
		{
			if (item == null)
			{
				throw new System.ArgumentNullException (FlatChildrenCollection.NullVisualMessage);
			}
			if (item.Parent == this.host)
			{
				throw new System.InvalidOperationException (FlatChildrenCollection.NotTwiceMessage);
			}

			Snapshot snapshot = Snapshot.RecordTree (item);

			this.visuals.Add (item);
			this.AttachVisual (item);
			
			this.NotifyChanges (snapshot);
		}

		public bool Remove(Visual item)
		{
			if (item == null)
			{
				throw new System.ArgumentNullException (FlatChildrenCollection.NullVisualMessage);
			}

			Snapshot snapshot = Snapshot.RecordTree (item);

			if (this.visuals.Remove (item))
			{
				this.DetachVisual (item);
				this.NotifyChanges (snapshot);
				
				return true;
			}
			else
			{
				return false;
			}
		}

		public void Clear()
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		public bool Contains(Visual item)
		{
			return this.visuals.Contains (item);
		}

		public void CopyTo(Visual[] array, int index)
		{
			this.visuals.CopyTo (array, index);
		}

		#endregion

		#region IEnumerable<Visual> Members

		public IEnumerator<Visual> GetEnumerator()
		{
			return this.visuals.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}
		
		#endregion

		private const string					NullVisualMessage = "Visual children may not be null";
		private const string					NotTwiceMessage = "Visual may not be inserted twice";
		
		private Visual							host;
		private List<Visual>					visuals = new List<Visual> ();

		internal Visual FindNext(Visual find)
		{
			int index = this.visuals.IndexOf (find);
			
			if ((index < 0) ||
				(index >= this.visuals.Count))
			{
				return null;
			}
			else
			{
				return this.visuals[index+1];
			}
		}

		internal Visual FindPrevious(Visual find)
		{
			int index = this.visuals.IndexOf (find);

			if ((index < 1) ||
				(index > this.visuals.Count))
			{
				return null;
			}
			else
			{
				return this.visuals[index-1];
			}
		}
	}
}
