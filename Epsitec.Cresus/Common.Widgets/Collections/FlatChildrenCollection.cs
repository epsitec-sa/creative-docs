//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Collections
{
	/// <summary>
	/// La classe FlatChildrenCollection stocke un ensemble de widgets de mani�re
	/// ordonn�e.
	/// </summary>
	public class FlatChildrenCollection : IList<Visual>, ICollection<Types.DependencyObject>
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

		#region Shapshot Class
		class Snapshot
		{
			private Snapshot()
			{
				this.snapshot   = new Types.DependencyObjectTreeSnapshot ();
				this.visuals    = new List<Visual> ();
				this.properties = new List<Types.DependencyProperty> ();
			}

			public void NotifyChanges()
			{
				//	Maintenant que tous les visuals ont leur parent d�finitif, il
				//	faut tous les passer en revue pour d�terminer les changements
				//	de propri�t�s h�rit�es.
				
				foreach (Visual visual in this.visuals)
				{
					IList<Types.DependencyProperty> properties = Types.DependencyObjectTree.FindInheritedProperties (visual);

					foreach (Types.DependencyProperty property in properties)
					{
						//	S'il y a tout � coup de nouvelles propri�t�s h�rit�es, il
						//	faut les inclure dans le snapshot de d�part, sous leur forme
						//	"undefined" :
						
						if (this.properties.Contains (property) == false)
						{
							this.snapshot.RecordUndefinedTree (visual, property);
						}
					}
				}

				//	Passe en revue tous les changements.

				Types.DependencyObjectTreeSnapshot.ChangeRecord[] records = this.snapshot.GetChanges ();
				for (int i = 0; i < records.Length; i++)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("{0}: {1}.{2} changed from {3} to {4}", i, (records[i].Object as Visual).Name, records[i].Property.Name, records[i].OldValue, records[i].NewValue));
					
					object oldValue = records[i].OldValue;
					object newValue = records[i].NewValue;

					if (oldValue == newValue)
					{
						//	Nothing changed, skip.
					}
					else if ((oldValue == null) ||
						/**/ (!oldValue.Equals (newValue)))
					{
						records[i].Object.InvalidateProperty (records[i].Property, oldValue, newValue);
					}
				}
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
					
					IList<Types.DependencyProperty> properties = Types.DependencyObjectTree.FindInheritedProperties (visual);

					foreach (Types.DependencyProperty property in properties)
					{
						if (this.properties.Contains (property) == false)
						{
							this.properties.Add (property);
						}
					}

					this.snapshot.Record (visual, properties);
					this.snapshot.RecordSubtree (visual, properties);
				}
			}

			Types.DependencyObjectTreeSnapshot	snapshot;
			List<Visual>						visuals;
			List<Types.DependencyProperty>		properties;
		}
		#endregion

		private const string					NullVisualMessage = "Visual children may not be null";
		private const string					NotTwiceMessage = "Visual may not be inserted twice";
		
		private Visual							host;
		private List<Visual>					visuals = new List<Visual> ();
	}
}
