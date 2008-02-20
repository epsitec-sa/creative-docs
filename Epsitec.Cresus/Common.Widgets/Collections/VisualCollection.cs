//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Collections
{
	public sealed class VisualCollection : System.Collections.IList
	{
		public VisualCollection(IVisualCollectionHost host)
		{
			this.host = host;
			this.list = new System.Collections.ArrayList ();
		}
		
		
		public Visual							this[int index]
		{
			get
			{
				return this.list[index] as Visual;
			}
		}
		public Visual							this[string name]
		{
			get
			{
				foreach (Visual item in this.list)
				{
					if (item.Name == name)
					{
						return item;
					}
				}
				
				return null;
			}
		}
		
		
		public int Add(Visual value)
		{
			if (value == null)
			{
				throw new System.ArgumentNullException ();
			}
			
			this.NotifyBeforeVisualInsertion (value);
			int index = this.list.Add (value);
			this.NotifyAfterVisualInsertion (value);
			this.NotifyContentsChanged ();
			return index;
		}
		public void AddRange(System.Collections.ICollection visuals)
		{
			foreach (Visual visual in visuals)
			{
				this.NotifyBeforeVisualInsertion (visual);
			}
			
			this.list.AddRange (visuals);
			
			foreach (Visual visual in visuals)
			{
				this.NotifyAfterVisualInsertion (visual);
			}
			
			this.NotifyContentsChanged ();
		}
		public bool Remove(Visual value)
		{
			if (this.list.Contains (value))
			{
				this.NotifyBeforeVisualRemoval (value);
				this.list.Remove (value);
				this.NotifyAfterVisualRemoval (value);
				this.NotifyContentsChanged ();
				return true;
			}
			else
			{
				return false;
			}
		}
		
		public bool Contains(Visual value)
		{
			return this.list.Contains (value);
		}
		
		public Visual[] ToArray()
		{
			return (Visual[]) this.list.ToArray (typeof (Visual));
		}
		
		
		#region IList Members
		public bool								IsReadOnly
		{
			get
			{
				return false;
			}
		}
		
		public bool								IsFixedSize
		{
			get
			{
				return this.list.IsFixedSize;
			}
		}
		
		object System.Collections.IList.this[int index]
		{
			get
			{
				return this.list[index];
			}
			set
			{
				throw new System.InvalidOperationException ("Indexed set prohibited");
			}
		}
		
		public void RemoveAt(int index)
		{
			Visual item = this[index];
			
			this.NotifyBeforeVisualRemoval (item);
			this.list.RemoveAt (index);
			this.NotifyAfterVisualRemoval (item);
			this.NotifyContentsChanged ();
		}

		public void Insert(int index, object value)
		{
			this.NotifyBeforeVisualInsertion (value as Visual);
			this.list.Insert (index, value);
			this.NotifyAfterVisualInsertion (value as Visual);
			this.NotifyContentsChanged ();
		}

		void System.Collections.IList.Remove(object value)
		{
			this.Remove (value as Visual);
		}

		bool System.Collections.IList.Contains(object value)
		{
			return this.Contains (value as Visual);
		}

		public void Clear()
		{
			if (this.list.Count > 0)
			{
				Visual[] visuals = this.ToArray ();
				
				for (int i = 0; i < visuals.Length; i++)
				{
					this.NotifyBeforeVisualRemoval (visuals[i]);
				}
				
				this.list.Clear ();
				
				for (int i = 0; i < visuals.Length; i++)
				{
					this.NotifyAfterVisualRemoval (visuals[i]);
				}
				
				this.NotifyContentsChanged ();
			}
		}

		int System.Collections.IList.IndexOf(object value)
		{
			return this.list.IndexOf (value);
		}
		
		int System.Collections.IList.Add(object value)
		{
			return this.Add (value as Visual);
		}
		#endregion
		
		#region ICollection Members
		public bool IsSynchronized
		{
			get
			{
				return this.list.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public void CopyTo(System.Array array, int index)
		{
			this.list.CopyTo (array, index);
		}

		public object SyncRoot
		{
			get
			{
				return this.list.SyncRoot;
			}
		}

		#endregion
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}
		#endregion
		
		private void NotifyBeforeVisualInsertion(Visual item)
		{
			this.host.NotifyVisualCollectionBeforeInsertion (this, item);
		}
		private void NotifyAfterVisualInsertion(Visual item)
		{
			this.host.NotifyVisualCollectionAfterInsertion (this, item);
		}
		
		private void NotifyBeforeVisualRemoval(Visual item)
		{
			this.host.NotifyVisualCollectionBeforeRemoval (this, item);
		}
		private void NotifyAfterVisualRemoval(Visual item)
		{
			this.host.NotifyVisualCollectionAfterRemoval (this, item);
		}
		
		private void NotifyContentsChanged()
		{
			this.host.NotifyVisualCollectionChanged (this);
		}
		
		private IVisualCollectionHost			host;
		private System.Collections.ArrayList	list;
	}
}
