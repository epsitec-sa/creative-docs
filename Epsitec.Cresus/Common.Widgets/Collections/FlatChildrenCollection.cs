//	Copyright � 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Collections
{
	/// <summary>
	/// La classe FlatChildrenCollection stocke un ensemble de widgets de mani�re
	/// ordonn�e.
	/// </summary>
	public class FlatChildrenCollection : IList<Visual>, ICollection<Types.DependencyObject>, System.Collections.ICollection
	{
		internal FlatChildrenCollection(Visual host)
		{
			this.host = host;
		}

		public Widget[]							Widgets
		{
			get
			{
				Visual[] visuals = this.visuals.ToArray ();
				Widget[] widgets = new Widget[visuals.Length];
				visuals.CopyTo (widgets, 0);
				return widgets;
			}
		}
		
		public int								AnchorLayoutCount
		{
			get
			{
				this.VerifyLayoutStatistics ();
				return this.anchorLayoutCount;
			}
		}
		public int								DockLayoutCount
		{
			get
			{
				this.VerifyLayoutStatistics ();
				return this.dockLayoutCount;
			}
		}
		public int								StackLayoutCount
		{
			get
			{
				this.VerifyLayoutStatistics ();
				return this.stackLayoutCount;
			}
		}
		public int								GridLayoutCount
		{
			get
			{
				this.VerifyLayoutStatistics ();
				return this.gridLayoutCount;
			}
		}

		public Visual FindNext(Visual find)
		{
			int index = this.visuals.IndexOf (find);

			if ((index < 0) ||
				(index > this.visuals.Count-2))
			{
				return null;
			}
			else
			{
				return this.visuals[index+1];
			}
		}
		public Visual FindPrevious(Visual find)
		{
			int index = this.visuals.IndexOf (find);

			if (index < 1)
			{
				return null;
			}
			else
			{
				return this.visuals[index-1];
			}
		}

		public int ZOrderOf(Visual visual)
		{
			int index = this.IndexOf (visual);

			if (index < 0)
			{
				return -1;
			}
			
			return this.Count - index - 1;
		}
		public void ChangeZOrder(Visual visual, int z)
		{
			if (this.Contains (visual) == false)
			{
				throw new System.ArgumentException ("Cannot change Z order of visual; it does not belong to this children collection");
			}

			z = System.Math.Max (0, z);
			z = System.Math.Min (z, this.Count-1);

			int newIndex = this.Count - z - 1;
			int oldIndex = this.IndexOf (visual);
			
			if (oldIndex < newIndex)
			{
				this.visuals.RemoveAt (oldIndex);
				this.visuals.Insert (newIndex, visual);
				
				Visual parent = this.host;

				if (parent != null)
				{
					Layouts.LayoutContext.AddToMeasureQueue (parent);
					Layouts.LayoutContext.AddToArrangeQueue (parent);
				}
			}
			else if (oldIndex > newIndex)
			{
				this.visuals.RemoveAt (oldIndex);
				this.visuals.Insert (newIndex, visual);

				Visual parent = this.host;

				if (parent != null)
				{
					Layouts.LayoutContext.AddToMeasureQueue (parent);
					Layouts.LayoutContext.AddToArrangeQueue (parent);
				}
			}
		}
		
		public void AddRange(IEnumerable<Visual> collection)
		{
			if (collection != null)
			{
				Snapshot snapshot = Snapshot.RecordTree (collection);
				
				this.visuals.AddRange (collection);

				foreach (Visual item in collection)
				{
					this.AttachVisual (item);
				}

				this.NotifyChanges (snapshot);
			}
		}

		internal void RefreshLayoutStatistics()
		{
			this.dockLayoutCount = 0;
			this.anchorLayoutCount = 0;
			this.stackLayoutCount = 0;
			this.gridLayoutCount = 0;

			foreach (Visual visual in this.visuals)
			{
				this.UpdateLayoutStatistics (visual, 1);
			}
		}
		
		internal void UpdateLayoutStatistics(Visual visual, DockStyle dockOld, DockStyle dockNew, AnchorStyles anchorOld, AnchorStyles anchorNew)
		{
			this.UpdateLayoutStatistics (visual, dockOld, anchorOld, -1);
			this.UpdateLayoutStatistics (visual, dockNew, anchorNew, 1);
		}
		
		private void NotifyChanges(Snapshot snapshot)
		{
			snapshot.NotifyChanges ();
		}

		private void AttachVisual(Visual visual)
		{
			//	Attache le visual � son nouveau parent; il est au pr�alable d�tach�
			//	de son ancien parent. Cette m�thode ne s'occupe pas de la question
			//	des propri�t�s h�rit�es.

			System.Diagnostics.Debug.Assert (visual != null);
			System.Diagnostics.Debug.Assert (this.visuals.Contains (visual));
			
			Visual parent = visual.Parent;

			if (parent == null)
			{
				//	Le visual n'a pas de parent, ce qui simplifie la gestion. Il
				//	suffit de lui en attribuer un :

				visual.SetParentVisual (this.host);
				visual.InheritedPropertyCache.InheritValuesFromParent (visual, this.host);

				this.UpdateLayoutStatistics (visual, 1);
			}
			else if (this.host == parent)
			{
				//	Le visual a d�j� le m�me parent; il n'y a donc rien � faire
				//	au niveau des liens.
			}
			else
			{
				//	Le visual est encore attach� � un parent. Il faut commencer par
				//	le d�tacher de son ancien parent, puis notifier l'ancien parent
				//	du changement :

				FlatChildrenCollection others = parent.Children;

				others.visuals.Remove (visual);
				others.DetachVisual (visual);

				System.Diagnostics.Debug.Assert (visual.Parent == null);
				
				visual.SetParentVisual (this.host);
				visual.InheritedPropertyCache.InheritValuesFromParent (visual, this.host);
				this.UpdateLayoutStatistics (visual, 1);
			}
			
			System.Diagnostics.Debug.Assert (visual.Parent == this.host);
			
			this.NotifyChanged ();
		}
		private void DetachVisual(Visual visual)
		{
			//	D�tache le visual de son parent.
			
			System.Diagnostics.Debug.Assert (visual != null);
			System.Diagnostics.Debug.Assert (this.visuals.Contains (visual) == false);
			System.Diagnostics.Debug.Assert (this.host == visual.Parent);

			this.UpdateLayoutStatistics (visual, -1);
			
			visual.SetParentVisual (null);
			visual.InheritedPropertyCache.ClearAllValues (visual);

			System.Diagnostics.Debug.Assert (visual.Parent == null);
			
			this.NotifyChanged ();
		}

		[System.Diagnostics.Conditional ("DEBUG")]
		private void VerifyLayoutStatistics()
		{
			int dock = 0;
			int anchor = 0;
			int stack = 0;
			int grid = 0;
			
			foreach (Visual visual in this.visuals)
			{
				switch (Layouts.LayoutEngine.GetLayoutMode (visual))
				{
					case Epsitec.Common.Widgets.Layouts.LayoutMode.Docked:
						dock++;
						break;

					case Epsitec.Common.Widgets.Layouts.LayoutMode.Anchored:
						anchor++;
						break;

					case Epsitec.Common.Widgets.Layouts.LayoutMode.Stacked:
						stack++;
						break;
					
					case Epsitec.Common.Widgets.Layouts.LayoutMode.Grid:
						grid++;
						break;
				}
			}

#if true
			System.Diagnostics.Debug.Assert (dock == this.dockLayoutCount);
			System.Diagnostics.Debug.Assert (anchor == this.anchorLayoutCount);
			System.Diagnostics.Debug.Assert (stack == this.stackLayoutCount);
			System.Diagnostics.Debug.Assert (grid == this.gridLayoutCount);
#endif
		}

		private void UpdateLayoutStatistics(Visual visual, int increment)
		{
			switch (Layouts.LayoutEngine.GetLayoutMode (visual))
			{
				case Epsitec.Common.Widgets.Layouts.LayoutMode.Docked:
					this.dockLayoutCount += increment;
					break;
				
				case Epsitec.Common.Widgets.Layouts.LayoutMode.Anchored:
					this.anchorLayoutCount += increment;
					break;

				case Epsitec.Common.Widgets.Layouts.LayoutMode.Stacked:
					this.stackLayoutCount += increment;
					break;

				case Epsitec.Common.Widgets.Layouts.LayoutMode.Grid:
					this.gridLayoutCount += increment;
					break;
			}
		}
		private void UpdateLayoutStatistics(Visual visual, DockStyle dock, AnchorStyles anchor, int increment)
		{
			switch (Layouts.LayoutEngine.GetLayoutMode (visual, dock, anchor))
			{
				case Epsitec.Common.Widgets.Layouts.LayoutMode.Docked:
					this.dockLayoutCount += increment;
					break;

				case Epsitec.Common.Widgets.Layouts.LayoutMode.Anchored:
					this.anchorLayoutCount += increment;
					break;

				case Epsitec.Common.Widgets.Layouts.LayoutMode.Stacked:
					this.stackLayoutCount += increment;
					break;

				case Epsitec.Common.Widgets.Layouts.LayoutMode.Grid:
					this.gridLayoutCount += increment;
					break;
			}
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
				//	If the caller tries to add a visual to the same parent, we
				//	simply accept it; don't do anything here...
				
				return;
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
			if (this.visuals.Count > 0)
			{
				Visual[] copy = this.visuals.ToArray ();
				Snapshot snapshot = Snapshot.RecordTree (copy);
				
				this.visuals.Clear ();
				
				foreach (Visual visual in copy)
				{
					this.DetachVisual (visual);
				}
				
				this.NotifyChanges (snapshot);
			}
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

		#region ICollection<Types.DependencyObject> Members

		public void Add(Types.DependencyObject item)
		{
			this.Add (item as Visual);
		}

		public bool Contains(Types.DependencyObject item)
		{
			return this.Contains (item as Visual);
		}

		public void CopyTo(Types.DependencyObject[] array, int index)
		{
			Visual[] temp = this.visuals.ToArray ();
			temp.CopyTo (array, index);
		}

		public bool Remove(Types.DependencyObject item)
		{
			return this.Remove (item as Visual);
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

		#region IEnumerable<Types.DependencyObject> Members

		IEnumerator<Types.DependencyObject> IEnumerable<Types.DependencyObject>.GetEnumerator()
		{
			foreach (Visual item in this.visuals)
			{
				yield return item;
			}
		}

		#endregion

		#region ICollection Members

		void System.Collections.ICollection.CopyTo(System.Array array, int index)
		{
			System.Collections.ICollection collection = this.visuals;
			collection.CopyTo (array, index);
		}

		int System.Collections.ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		bool System.Collections.ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object System.Collections.ICollection.SyncRoot
		{
			get
			{
				return this.visuals;
			}
		}

		#endregion

		#region Shapshot Class
		class Snapshot
		{
			private Snapshot()
			{
				this.snapshot   = new Types.DependencyObjectTreeSnapshot ();
				this.visuals    = new List<Visual> ();
			}

			public void NotifyChanges()
			{
				//	Maintenant que tous les visuals ont leur parent d�finitif, il
				//	faut tous les passer en revue pour d�terminer les changements
				//	de propri�t�s h�rit�es.
				
				foreach (Visual visual in this.visuals)
				{
					visual.InheritedPropertyCache.NotifyChanges (visual);
				}

				this.snapshot.InvalidateDifferentProperties ();
			}

			public static Snapshot RecordTree(Visual visual)
			{
				Snapshot snapshot = new Snapshot ();
				snapshot.Add (visual);
				return snapshot;
			}
			public static Snapshot RecordTree(params Visual[] visuals)
			{
				IEnumerable<Visual> collection = visuals;
				return Snapshot.RecordTree (collection);
			}
			public static Snapshot RecordTree(IEnumerable<Visual> collection)
			{
				Snapshot snapshot = new Snapshot ();
				foreach (Visual item in collection)
				{
					snapshot.Add (item);
				}
				return snapshot;
			}
			
			private void Add(Visual visual)
			{
				if (this.visuals.Contains (visual))
				{
					//	Rien � faire, car ce visual est d�j� connu et a d�j� �t� analys�.
				}
				else
				{
					this.visuals.Add (visual);
					
					this.snapshot.Record (visual, Visual.ParentProperty);
				}
			}

			Types.DependencyObjectTreeSnapshot	snapshot;
			List<Visual>						visuals;
		}
		#endregion

		private const string					NullVisualMessage = "Visual children may not be null";
		private const string					NotTwiceMessage = "Visual may not be inserted twice";
		
		private Visual							host;
		private List<Visual>					visuals = new List<Visual> ();
		
		private int								dockLayoutCount;
		private int								anchorLayoutCount;
		private int								stackLayoutCount;
		private int								gridLayoutCount;
	}
}
