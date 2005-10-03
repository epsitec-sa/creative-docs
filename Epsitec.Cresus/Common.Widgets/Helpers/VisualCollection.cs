//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Helpers
{
	public class VisualCollection : System.Collections.IList, System.IDisposable
	{
		public VisualCollection(Visual host)
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
		
		public bool								AutoEmbedding
		{
			get
			{
				return this.auto_embedding;
			}
			set
			{
				this.auto_embedding = value;
			}
		}
		
		
		public void AddRange(Visual[] widgets)
		{
			this.list.AddRange (widgets);
			
			foreach (Visual widget in widgets)
			{
				this.HandleInsert (widget);
			}
		}
		
		public void Dispose()
		{
			System.Diagnostics.Debug.Assert (this.list.Count == 0);
			
			this.host = null;
			this.list = null;
		}
		
		
		
		
		#region IList Members
		public bool IsReadOnly
		{
			get
			{
				return false;
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
				this.list[index] = value;
			}
		}

		public void RemoveAt(int index)
		{
			Visual item = this.list[index] as Visual;
			this.HandleRemove (item);
			this.list.RemoveAt (index);
			this.HandlePostRemove (item);
		}

		public void Insert(int index, object value)
		{
			this.list.Insert (index, value);
			this.HandleInsert (value as Visual);
		}

		public void Remove(object value)
		{
			this.HandleRemove (value as Visual);
			this.list.Remove (value);
			this.HandlePostRemove (value as Visual);
		}

		public bool Contains(object value)
		{
			return this.list.Contains (value);
		}

		public void Clear()
		{
			Visual[] widgets = new Visual[this.list.Count];
			this.list.CopyTo (widgets, 0);
			
			for (int i = 0; i < widgets.Length; i++)
			{
				this.HandleRemove (widgets[i]);
			}
			
			this.list.Clear ();
			
			for (int i = 0; i < widgets.Length; i++)
			{
				this.HandlePostRemove (widgets[i]);
			}
		}

		public int IndexOf(object value)
		{
			return this.list.IndexOf (value);
		}

		public int Add(object value)
		{
			if (value is Visual)
			{
				int index = this.list.Add (value);
				this.HandleInsert (value as Visual);
				return index;
			}
			
			throw new System.ArgumentException ("Expecting Visual, got " + value.GetType ().Name);
		}

		public bool IsFixedSize
		{
			get
			{
				return this.list.IsFixedSize;
			}
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
		
		protected void HandleInsert(Visual item)
		{
			if (this.auto_embedding)
			{
				Visual embedder = this.host as Visual;
				
				System.Diagnostics.Debug.Assert (embedder != null);
//?				System.Diagnostics.Debug.Assert (item.Parent == null);
				
//?				item.SetEmbedder (embedder);
			}
			
			this.RenumberItems ();
//?			this.host.NotifyInsertion (item);
		}
		
		protected void HandleRemove(Visual item)
		{
//?			this.host.NotifyRemoval (item);
			this.RenumberItems ();
		}
		
		protected void HandlePostRemove(Visual item)
		{
//?			item.Index = -1;
//?			this.host.NotifyPostRemoval (item);
		}
		
		protected void RenumberItems()
		{
			for (int i = 0; i < this.list.Count; i++)
			{
				Visual item = this.list[i] as Visual;
//?				item.Index = i;
			}
		}
		
		
		private System.Collections.ArrayList	list;
		private Visual							host;
		private bool							auto_embedding;
	}
}
