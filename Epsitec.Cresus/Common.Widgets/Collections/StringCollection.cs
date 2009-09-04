//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Collections
{
	public class StringCollection : System.Collections.IList, System.IDisposable
	{
		public StringCollection(IStringCollectionHost host)
		{
			this.host  = host;
			this.list  = new List<object> ();
			this.names = new List<string> ();
		}
		
		
		public bool								AcceptsRichText
		{
			get
			{
				return this.acceptsRichText;
			}
			set
			{
				this.acceptsRichText = value;
			}
		}
		
		public string							this[int index]
		{
			get
			{
				if (index == -1)
				{
					return null;
				}
				else
				{
					if (this.converter != null)
					{
						return this.converter (this.list[index]);
					}
					else
					{
						return this.list[index].ToString ();
					}
				}
			}
		}
		
		public object[]							Values
		{
			get
			{
				return this.list.ToArray ();
			}
		}
		
		public string[]							Names
		{
			get
			{
				string[] names = new string[this.names.Count];
				this.names.CopyTo (names);
				return names;
			}
		}

		public ConverterCallback				Converter
		{
			get
			{
				return this.converter;
			}
			set
			{
				this.converter = value;
			}
		}
		
		
		public int  Add(string name, object value)
		{
			int index0 = this.list.Count;
			int index1 = this.names.Count;
			
			this.list.Add (value);
			this.names.Add (name);
			
			System.Diagnostics.Debug.Assert (index0 == index1);
			
			this.HandleInsert (value);
			this.HandleChange ();
			
			return index0;
		}

		public void AddRange(System.Collections.Generic.IEnumerable<string> values)
		{
			foreach (var value in values)
			{
				this.Add (value);
			}
		}
		
		
		public int FindNameIndex(string name)
		{
			return this.names.IndexOf (name);
		}
		
		public void SetName(int index, string name)
		{
			this.names[index] = name;
		}

		public string GetName(int index)
		{
			return this.names[index] as string;
		}

		public int FindIndex(System.Predicate<object> match)
		{
			return this.list.FindIndex (match);
		}
		
		public int FindExactMatch(string find)
		{
			if ((find != null) &&
				(find.Length > 0))
			{
				find = find.ToUpper ();
				
				for (int i = 0; i < this.list.Count; i++)
				{
					string text = this[i];
					
					if (this.acceptsRichText)
					{
						text = TextLayout.ConvertToSimpleText (text);
					}
					
					text = text.ToUpper ();
					
					if (text == find)
					{
						return i;
					}
				}
			}
			
			return -1;
		}
		
		public int FindStartMatch(string find)
		{
			return this.FindStartMatch (find, 0);
		}
		
		public int FindStartMatch(string find, int startAt)
		{
			find = find.ToUpper ();
			
			for (int i = startAt; i < this.list.Count; i++)
			{
				string text = this[i];
					
				if (this.acceptsRichText)
				{
					text = TextLayout.ConvertToSimpleText (text);
				}
					
				text = text.ToUpper ();
				
				if (text.StartsWith (find))
				{
					return i;
				}
			}
			
			return -1;
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
				this.names.Clear ();
				this.list = null;
				this.names = null;
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
				this.list[index] = value;
			}
		}

		public void RemoveAt(int index)
		{
			object item = this.list[index];
			this.HandleRemove (item);
			this.list.RemoveAt (index);
			this.names.RemoveAt (index);
			this.HandleChange ();
		}

		public void Insert(int index, object value)
		{
			this.list.Insert (index, value);
			this.names.Insert (index, null);
			this.HandleInsert (value);
			this.HandleChange ();
		}

		public void Remove(object value)
		{
			int index = this.list.IndexOf (value);
			if (index >= 0)
			{
				this.HandleRemove (value);
				this.list.RemoveAt (index);
				this.names.RemoveAt (index);
				this.HandleChange ();
			}
		}

		public bool Contains(object value)
		{
			return this.list.Contains (value);
		}

		public void Clear()
		{
			foreach (object item in this.list)
			{
				this.HandleRemove (item);
			}
			this.list.Clear ();
			this.names.Clear ();
			this.HandleChange ();
		}

		public int IndexOf(object value)
		{
			return this.list.IndexOf (value);
		}

		public int Add(object value)
		{
			int index0 = this.list.Count;
			int index1 = this.names.Count;

			this.list.Add (value);
			this.names.Add (null);
			
			System.Diagnostics.Debug.Assert (index0 == index1);
			
			this.HandleInsert (value);
			this.HandleChange ();
			
			return index0;
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
			System.Collections.IList list = this.list;
			list.CopyTo (array, index);
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
		public System.Collections.IEnumerator GetEnumerator()
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
				this.host.NotifyStringCollectionChanged ();
			}
		}

		public delegate string ConverterCallback(object value);
		
		private IStringCollectionHost			host;
		private List<object>					list;
		private List<string>					names;
		private bool							acceptsRichText;
		private ConverterCallback				converter;
	}
}
