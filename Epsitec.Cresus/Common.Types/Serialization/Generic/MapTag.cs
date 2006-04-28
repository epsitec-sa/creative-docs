//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Serialization.Generic
{
	/// <summary>
	/// The MapTag class is used to record relations between values of type T
	/// and tags, which are just unique names.
	/// </summary>
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
		public int								UsedValueCount
		{
			get
			{
				return this.valueCounters.Count;
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

		public void IncrementUseValue(T value)
		{
			int count;

			if (this.valueToTagLookup.ContainsKey (value) == false)
			{
				throw new System.ArgumentException ("Value is not known");
			}
			
			if (this.valueCounters.TryGetValue (value, out count))
			{
				this.valueCounters[value] = count+1;
			}
			else
			{
				this.valueCounters[value] = 1;
			}
		}
		public void ClearUseCount()
		{
			this.valueCounters.Clear ();
		}

		public int GetValueUseCount(T value)
		{
			int count;
			
			if (this.valueCounters.TryGetValue (value, out count))
			{
				return count;
			}
			else
			{
				return 0;
			}
		}
		
		public IEnumerable<T> GetUsedValues()
		{
			foreach (T value in this.valueCounters.Keys)
			{
				yield return value;
			}
		}
		public IEnumerable<T> GetUnusedValues()
		{
			foreach (T value in this.valueToTagLookup.Keys)
			{
				if (this.valueCounters.ContainsKey (value))
				{
					continue;
				}
				yield return value;
			}
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
		private Dictionary<T, int> valueCounters = new Dictionary<T, int> ();
	}
}
