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

		public void Record(string tag, T value)
		{
			if (this.tagToValueLookup.ContainsKey (tag))
			{
				throw new System.ArgumentException (string.Format ("Duplicate tag '{0}' in MapTag", tag));
			}
			if (this.valueToTagLookup.ContainsKey (value))
			{
				throw new System.ArgumentException (string.Format ("Duplicate value for tag '{0}' and '{1}'", tag, this.valueToTagLookup[value]));
			}
			
			this.tagToValueLookup[tag] = value;
			this.valueToTagLookup[value] = tag;
		}

		private Dictionary<string, T> tagToValueLookup = new Dictionary<string, T> ();
		private Dictionary<T, string> valueToTagLookup = new Dictionary<T, string> ();
	}
}
