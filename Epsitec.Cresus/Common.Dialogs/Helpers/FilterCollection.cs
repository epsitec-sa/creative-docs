//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Dialogs.Helpers
{
	/// <summary>
	/// The <c>FilterCollection</c> class implements a list of <see cref="FilterItem"/>
	/// elements.
	/// </summary>
	public class FilterCollection : System.Collections.IList, System.IDisposable, IEnumerable<FilterItem>
	{
		public FilterCollection(IFilterCollectionHost host)
		{
			this.host  = host;
			this.list  = new List<FilterItem> ();
		}
		
		
		public FilterItem				this[int index]
		{
			get
			{
				if (index == -1) return null;
				return this.list[index];
			}
		}
		
		public string					FileDialogFilter
		{
			get
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				for (int i = 0; i < this.list.Count; i++)
				{
					if (i > 0)
					{
						buffer.Append ("|");
					}
					
					buffer.Append (this[i].FileDialogFilter);
				}
				
				return buffer.ToString ();
			}
		}
		
		public int Add(FilterItem item)
		{
			int index = this.list.Count;
			this.list.Add (item);
			this.HandleInsert (item);
			this.HandleChange ();
			return index;
		}
		
		public int Add(string name, string caption, string filter)
		{
			return this.Add (new FilterItem (name, caption, filter));
		}


		/// <summary>
		/// Finds the <see cref="FilterItem"/> with the specified extension.
		/// </summary>
		/// <param name="extension">The extension (with or without a leading dot).</param>
		/// <returns></returns>
		public FilterItem FindExtension(string extension)
		{
			if (string.IsNullOrEmpty (extension))
            {
				return null;
            }
			if (extension[0] != '.')
			{
				extension = "." + extension;
			}

			foreach (var item in this.list)
			{
				if (item.Filter.EndsWith (extension, System.StringComparison.InvariantCultureIgnoreCase))
				{
					return item;
				}
			}

			return null;
		}
		
		
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.list.Clear ();
				this.list = null;
			}
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
				this.list[index] = (FilterItem) value;
			}
		}

		public void RemoveAt(int index)
		{
			object item = this.list[index];
			this.HandleRemove (item);
			this.list.RemoveAt (index);
			this.HandleChange ();
		}

		public void Insert(int index, object value)
		{
			this.list.Insert (index, (FilterItem) value);
			this.HandleInsert (value);
			this.HandleChange ();
		}

		public void Remove(object value)
		{
			int index = this.list.IndexOf ((FilterItem) value);
			if (index >= 0)
			{
				this.HandleRemove (value);
				this.list.RemoveAt (index);
				this.HandleChange ();
			}
		}

		public bool Contains(object value)
		{
			return this.list.Contains ((FilterItem) value);
		}

		public void Clear()
		{
			foreach (object item in this.list)
			{
				this.HandleRemove (item);
			}
			this.list.Clear ();
			this.HandleChange ();
		}

		public int IndexOf(object value)
		{
			return this.list.IndexOf ((FilterItem) value);
		}

		public int Add(object value)
		{
			return this.Add ((FilterItem) value);
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}
		#endregion
		
		#region ICollection Members
		public bool IsSynchronized
		{
			get
			{
				return false;
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
			System.Collections.ICollection collection = this.list;
			collection.CopyTo (array, index);
		}

		public object SyncRoot
		{
			get
			{
				return this.list;
			}
		}

		#endregion
		
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}
		#endregion

		#region IEnumerable<FilterItem> Members

		public IEnumerator<FilterItem> GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}

		#endregion
		
		protected virtual void HandleInsert(object item)
		{
		}
		
		protected virtual void HandleRemove(object item)
		{
		}
		
		protected virtual void HandleChange()
		{
			if (this.host != null)
			{
				this.host.FilterCollectionChanged ();
			}
		}
		
		
		private IFilterCollectionHost			host;
		private List<FilterItem>				list;
	}
}
