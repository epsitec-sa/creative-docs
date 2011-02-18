//	Copyright © 2004-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Collections
{
	/// <summary>
	/// The <c>StringCollection</c> class stores a collection of key/value pairs,
	/// where the values are converted to strings on demand. This is used by some
	/// <see cref="Widget"/> classes which have an <c>Items</c> property (see also
	/// <see cref="IStringCollectionHost"/>).
	/// </summary>
	public class StringCollection : System.Collections.IList, System.IDisposable
	{
		public StringCollection(IStringCollectionHost host)
		{
			this.host   = host;
			this.values = new List<object> ();
			this.keys   = new List<string> ();
		}


		public bool								AcceptsRichText
		{
			get;
			set;
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
					return this.ConvertValueToString (this.values[index]);
				}
			}
		}
		
		public object[]							Values
		{
			get
			{
				return this.values.ToArray ();
			}
		}
		
		public string[]							Keys
		{
			get
			{
				return this.keys.ToArray ();
			}
		}

		/// <summary>
		/// Gets or sets the value converter, which is used to convert the <c>object</c>
		/// values to <c>string</c> values. If no converter is specified, the conversion
		/// will be done by <see cref="ToString"/>.
		/// </summary>
		/// <value>The value converter.</value>
		public System.Func<object, string>		ValueConverter
		{
			get;
			set;
		}
		
		
		public int Add(string key, object value)
		{
			int index0 = this.values.Count;
			int index1 = this.keys.Count;
			
			System.Diagnostics.Debug.Assert (index0 == index1);
			
			this.values.Add (value);
			this.keys.Add (key);
			
			this.HandleInsert (value);
			this.HandleChange ();
			
			return index0;
		}

		public void AddRange(IEnumerable<object> values)
		{
			if (values == null)
            {
				return;
            }

			foreach (var value in values)
			{
				this.Add (value);
			}
		}

		public void AddRange(IEnumerable<string> values)
		{
			if (values == null)
            {
				return;
            }
			
			foreach (var value in values)
			{
				this.Add (value);
			}
		}

		public void AddRange(string[] values)
		{
			this.AddRange ((IEnumerable<string>) values);
		}
		
		public void SetKey(int index, string key)
		{
			this.keys[index] = key;
		}

		public string GetKey(int index)
		{
			return this.keys[index];
		}

		public object GetValue(int index)
		{
			return this.values[index];
		}

		public T GetValue<T>(int index)
			where T : class
		{
			return this.GetValue (index) as T;
		}

		
		public int FindIndexByKey(string key)
		{
			return this.keys.IndexOf (key);
		}

		public int FindIndexByValue(object value)
		{
			return this.FindIndexByValue (item => item == value);
		}
		
		public int FindIndexByValue<T>(System.Predicate<T> match)
			where T : class
		{
			return this.values.FindIndex (x => match (x as T));
		}
		
		public int FindIndexByValue(System.Predicate<object> match)
		{
			return this.values.FindIndex (match);
		}
		
		public int FindIndexByValueExactMatch(string find, int startAt = 0)
		{
			if (string.IsNullOrEmpty (find))
            {
				return -1;
            }

			return this.FindIndexByValue (find, startAt, (x, y) => x == y);
		}
		
		public int FindIndexByValueStartMatch(string find, int startAt = 0)
		{
			return this.FindIndexByValue (find ?? "", startAt, (x, y) => x.StartsWith (find));
		}

		public int FindIndexByValue(string find, int startAt, System.Func<string, string, bool> match)
		{
			if (match == null)
			{
				throw new System.ArgumentNullException ("No comparator specified");
			}
			if (find == null)
			{
				throw new System.ArgumentNullException ("No search criteria specified");
			}

			find = find.ToUpperInvariant ();

			for (int i = startAt; i < this.values.Count; i++)
			{
				string text = this[i];

				if (this.AcceptsRichText)
				{
					text = TextLayout.ConvertToSimpleText (text);
				}

				text = text.ToUpperInvariant ();

				if (match (text, find))
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

		#region IDisposable Members

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.values.Clear ();
				this.keys.Clear ();
			}
		}

		#endregion

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
				return this.values[index];
			}
			set
			{
				this.values[index] = value;
			}
		}

		public void RemoveAt(int index)
		{
			object item = this.values[index];
			this.HandleRemove (item);
			this.values.RemoveAt (index);
			this.keys.RemoveAt (index);
			this.HandleChange ();
		}

		public void Insert(int index, object value)
		{
			this.values.Insert (index, value);
			this.keys.Insert (index, null);
			this.HandleInsert (value);
			this.HandleChange ();
		}

		public void Remove(object value)
		{
			int index = this.values.IndexOf (value);
			if (index >= 0)
			{
				this.HandleRemove (value);
				this.values.RemoveAt (index);
				this.keys.RemoveAt (index);
				this.HandleChange ();
			}
		}

		public bool Contains(object value)
		{
			return this.values.Contains (value);
		}

		public void Clear()
		{
			foreach (object item in this.values)
			{
				this.HandleRemove (item);
			}
			this.values.Clear ();
			this.keys.Clear ();
			this.HandleChange ();
		}

		public int IndexOf(object value)
		{
			return this.values.IndexOf (value);
		}

		public int Add(object value)
		{
			return this.Add (null, value);
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
				return this.values.Count;
			}
		}

		public void CopyTo(System.Array array, int index)
		{
			System.Collections.IList list = this.values;
			list.CopyTo (array, index);
		}

		public object SyncRoot
		{
			get
			{
				return this.values;
			}
		}

		#endregion
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			return this.values.GetEnumerator ();
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


		private string ConvertValueToString(object value)
		{
			if (this.ValueConverter != null)
			{
				return this.ValueConverter (value);
			}
			else if (value == null)
			{
				return null;
			}
			else
			{
				return value.ToString ();
			}
		}

		private readonly IStringCollectionHost	host;
		private readonly List<string>			keys;
		private readonly List<object>			values;
	}
}
