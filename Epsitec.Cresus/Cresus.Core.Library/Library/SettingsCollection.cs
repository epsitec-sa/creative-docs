//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>SettingsCollection</c> class implements something which looks like
	/// a dictionary of key/values tuples, stored as <see cref="SettingsTuple"/>s. The
	/// order of the tuples will be preserved.
	/// </summary>
	public sealed class SettingsCollection : IEnumerable<SettingsTuple>
	{
		public SettingsCollection()
		{
			this.dict = new Dictionary<string, string> ();
			this.tuples = new List<SettingsTuple> ();
		}

		public SettingsCollection(IEnumerable<SettingsTuple> tuples)
			: this ()
		{
			foreach (var tuple in tuples)
			{
				this[tuple.Key] = tuple.Value;
			}
		}

		/// <summary>
		/// Gets or sets the key/value tuple. If the key matches an element already
		/// known in the collection, then it will simply be replaced. Otherwise, a
		/// new key/value tuple will be added at the end of the collection.
		/// </summary>
		public string this[string key]
		{
			get
			{
				string value;

				if (this.dict.TryGetValue (key, out value))
				{
					return value;
				}
				else
				{
					return null;
				}
			}
			set
			{
				System.Diagnostics.Debug.Assert (this.dict.Count == this.tuples.Count);
				
				if (value == null)
				{
					if (this.dict.Remove (key))
					{
						this.tuples.RemoveAll (x => x.Key == key);
					}
				}
				else if (this.dict.ContainsKey (key))
				{
					this.dict[key] = value;
					this.tuples[this.tuples.FindIndex (x => x.Key == key)] = new SettingsTuple (key, value);
				}
				else
				{
					this.dict[key] = value;
					this.tuples.Add (new SettingsTuple (key, value));
				}
				
				System.Diagnostics.Debug.Assert (this.dict.Count == this.tuples.Count);
			}
		}

		public int Count
		{
			get
			{
				return this.dict.Count;
			}
		}

		public void Clear()
		{
			this.dict.Clear ();
			this.tuples.Clear ();
		}

		public bool AddRange(IEnumerable<SettingsTuple> collection)
		{
			int changes = 0;

			foreach (var item in collection)
			{
				if (this[item.Key] != item.Value)
				{
					this[item.Key] = item.Value;
					changes++;
				}
			}

			return changes > 0;
		}

		public bool Remove(string key)
		{
			bool changed = false;

			System.Diagnostics.Debug.Assert (this.dict.Count == this.tuples.Count);
			
			if (this.dict.Remove (key))
			{
				this.tuples.RemoveAt (this.tuples.FindIndex (x => x.Key == key));
				changed = true;
			}
			
			System.Diagnostics.Debug.Assert (this.dict.Count == this.tuples.Count);

			return changed;
		}

		public bool RemoveAllStartingWith(string prefix)
		{
			var items = from setting in this.tuples
						let key = setting.Key
						where key.StartsWith (prefix)
						select key;

			var remove = items.ToArray ();

			if (remove.Length > 0)
			{
				remove.ForEach (key => this.Remove (key));
				return true;
			}
			else
			{
				return false;
			}
		}

        public XElement Save(string xmlNodeName)
		{
			var nodes = this.tuples.Select (x =>
				new XElement (Xml.Tuple,
					new XAttribute (Xml.Key, x.Key),
					new XAttribute (Xml.Value, x.Value)));

			return new XElement (xmlNodeName, nodes);
		}

		public void Restore(XElement xml)
		{
			this.Clear ();

			if (xml == null)
			{
				return;
			}

			foreach (XElement node in xml.Elements ())
			{
				System.Diagnostics.Debug.Assert (node.Name == Xml.Tuple);

				string key   = (string) node.Attribute (Xml.Key);
				string value = (string) node.Attribute (Xml.Value);

				this[key] = value;
			}
		}

		#region IEnumerable<SettingTuple> Members

		public IEnumerator<SettingsTuple> GetEnumerator()
		{
			return this.tuples.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.tuples.GetEnumerator ();
		}

		#endregion

		#region Xml Constants

		private static class Xml
		{
			public const string					Tuple	= "tuple";
			public const string					Key		= "k";
			public const string					Value	= "v";
		}

		#endregion

		private readonly Dictionary<string, string> dict;
		private readonly List<SettingsTuple> tuples;
	}
}
