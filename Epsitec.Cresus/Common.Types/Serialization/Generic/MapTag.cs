//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Serialization.Generic
{
	public class MapTag<T> where T : class
	{
		public MapTag()
		{
		}

		public IEnumerable<T>					RecordedValues
		{
			get
			{
				return this.valueToTagLookup.Keys;
			}
		}
		public IEnumerable<string>				RecordedTags
		{
			get
			{
				return this.tagToValueLookup.Keys;
			}
		}
		
		public int								ValueCount
		{
			get
			{
				return this.valueToTagLookup.Count;
			}
		}
		public int								TagCount
		{
			get
			{
				return this.tagToValueLookup.Count;
			}
		}

		public void Record(string tag, T value)
		{
			if (this.tagToValueLookup.ContainsKey (tag))
			{
				throw new System.ArgumentException (string.Format ("Duplicate tag '{0}' in MapTag", tag));
			}
			
			//	TODO: consider allowing a single value to have multiple tags
			
			if (this.valueToTagLookup.ContainsKey (value))
			{
				throw new System.ArgumentException (string.Format ("Same value for tag '{0}' and '{1}'", tag, this.valueToTagLookup[value]));
			}
			
			this.tagToValueLookup[tag] = value;
			this.valueToTagLookup[value] = tag;
		}

		public bool IsValueDefined(T value)
		{
			return this.valueToTagLookup.ContainsKey (value);
		}
		public bool IsTagDefined(string tag)
		{
			return this.tagToValueLookup.ContainsKey (tag);
		}

		public T GetValue(string tag)
		{
			T value;
			
			if (this.tagToValueLookup.TryGetValue (tag, out value))
			{
				return value;
			}
			
			return null;
		}
		public string GetTag(T value)
		{
			string tag;

			if (this.valueToTagLookup.TryGetValue (value, out tag))
			{
				return tag;
			}
			
			return null;
		}
		
		private Dictionary<string, T> tagToValueLookup = new Dictionary<string, T> ();
		private Dictionary<T, string> valueToTagLookup = new Dictionary<T, string> ();
	}
}
