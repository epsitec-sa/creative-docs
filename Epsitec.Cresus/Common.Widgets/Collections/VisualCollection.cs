//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			
			int index = this.list.Add (value);
			this.NotifyVisualInsertion (value);
			this.NotifyContentsChanged ();
			return index;
		}
		
		public void AddRange(System.Collections.ICollection widgets)
		{
			this.list.AddRange (widgets);
			
			foreach (Visual widget in widgets)
			{
				this.NotifyVisualInsertion (widget);
			}
			
			this.NotifyContentsChanged ();
		}
		
		public void Remove(Visual value)
		{
			if (this.list.Contains (value))
			{
				this.list.Remove (value);
				this.NotifyVisualRemoval (value);
				this.NotifyContentsChanged ();
			}
		}
		
		public bool Contains(Visual value)
		{
			return this.list.Contains (value);
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
			
			this.list.RemoveAt (index);
			this.NotifyVisualRemoval (item);
			this.NotifyContentsChanged ();
		}

		public void Insert(int index, object value)
		{
			this.list.Insert (index, value);
			this.NotifyVisualInsertion (value as Visual);
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
				Visual[] widgets = (Visual[]) this.list.ToArray (typeof (Visual));
				
				this.list.Clear ();
				
				for (int i = 0; i < widgets.Length; i++)
				{
					this.NotifyVisualRemoval (widgets[i]);
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
		
		private void NotifyVisualInsertion(Visual item)
		{
			this.host.NotifyVisualCollectionInsertion (this, item);
		}
		
		private void NotifyVisualRemoval(Visual item)
		{
			this.host.NotifyVisualCollectionRemoval (this, item);
		}
		
		private void NotifyContentsChanged()
		{
			this.host.NotifyVisualCollectionChanged (this);
		}
		
		
		private IVisualCollectionHost			host;
		private System.Collections.ArrayList	list;
	}
}
