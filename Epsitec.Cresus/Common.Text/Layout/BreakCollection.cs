//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Layout
{
	/// <summary>
	/// Summary description for BreakCollection.
	/// </summary>
	public class BreakCollection : System.Collections.IList
	{
		public BreakCollection()
		{
			this.breaks      = new Layout.Break[4];
			this.break_count = 0;
		}
		
		
		public void Add(Layout.Break info)
		{
			if (this.break_count == this.breaks.Length)
			{
				Layout.Break[] old_breaks = this.breaks;
				Layout.Break[] new_breaks = new Layout.Break[this.break_count+8];
				
				System.Array.Copy (old_breaks, 0, new_breaks, 0, this.break_count);
				
				this.breaks = new_breaks;
			}
			
			this.breaks[this.break_count] = info;
			this.break_count++;
		}
		
		public void Clear()
		{
			this.break_count = 0;
		}
		
		
		public Layout.Break						this[int index]
		{
			get
			{
				if (index < this.break_count)
				{
					return this.breaks[index];
				}
				
				throw new System.ArgumentOutOfRangeException ("index", index, "Index out of range.");
			}
		}
		
		
		#region ICollection Members
		public bool								IsSynchronized
		{
			get
			{
				return this.breaks.IsSynchronized;
			}
		}
		
		public object							SyncRoot
		{
			get
			{
				return this.breaks.SyncRoot;
			}
		}
		
		public int								Count
		{
			get
			{
				return this.break_count;
			}
		}
		
		
		public void CopyTo(System.Array array, int index)
		{
			System.Array.Copy (this.breaks, 0, array, index, this.break_count);
		}
		#endregion
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			return new Enumerator (this.breaks, this.break_count);
		}
		#endregion

		#region IList Members
		public bool								IsReadOnly
		{
			get
			{
				return true;
			}
		}
		
		public bool								IsFixedSize
		{
			get
			{
				return true;
			}
		}
		
		object									System.Collections.IList.this[int index]
		{
			get
			{
				if (index < this.break_count)
				{
					return this.breaks[index];
				}
				
				throw new System.ArgumentOutOfRangeException ("index", index, "Index out of range.");
			}
			set
			{
				throw new System.InvalidOperationException ();
			}
		}
		
		
		public int IndexOf(object value)
		{
			for (int i = 0; i < this.break_count; i++)
			{
				if (this.breaks[i] == value)
				{
					return i;
				}
			}
			
			return -1;
		}
		
		public bool Contains(object value)
		{
			for (int i = 0; i < this.break_count; i++)
			{
				if (this.breaks[i] == value)
				{
					return true;
				}
			}
			
			return false;
		}
		
		
		void System.Collections.IList.RemoveAt(int index)
		{
			throw new System.InvalidOperationException ();
		}
		
		void System.Collections.IList.Insert(int index, object value)
		{
			throw new System.InvalidOperationException ();
		}
		
		void System.Collections.IList.Remove(object value)
		{
			throw new System.InvalidOperationException ();
		}
		
		void System.Collections.IList.Clear()
		{
			throw new System.InvalidOperationException ();
		}
		
		int System.Collections.IList.Add(object value)
		{
			throw new System.InvalidOperationException ();
		}
		#endregion
		
		#region Enumerator Class
		private class Enumerator : System.Collections.IEnumerator
		{
			public Enumerator(Break[] breaks, int count)
			{
				this.breaks = breaks;
				this.count  = count;
				this.index  = -1;
			}
			
			#region IEnumerator Members
			public object						Current
			{
				get
				{
					return this.breaks[this.index];
				}
			}
			
			
			public void Reset()
			{
				this.index = -1;
			}
			
			public bool MoveNext()
			{
				this.index++;
				return this.index < this.count;
			}
			#endregion
			
			private Break[]						breaks;
			private int							count;
			private int							index;
		}

		#endregion
		
		private Break[]							breaks;
		private int								break_count;
	}
}
