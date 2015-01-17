using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec.Cresus.ResourceManagement
{
	public class MultiCultureResourceItem : IReadOnlyDictionary<CultureInfo, ResourceItem>
	{
		internal MultiCultureResourceItem(string symbolName, Dictionary<CultureInfo, ResourceItem> cultureMap)
		{
			this.symbolName = symbolName;
			this.CultureMap = cultureMap;
		}

		public string SymbolName
		{
			get
			{
				return this.symbolName;
			}
		}

		#region IReadOnlyDictionary<CultureInfo,ResourceItem> Members

		public bool ContainsKey(CultureInfo key)
		{
			return this.CultureMap.ContainsKey (key);
		}

		public IEnumerable<CultureInfo> Keys
		{
			get
			{
				return this.CultureMap.Keys;
			}
		}

		public bool TryGetValue(CultureInfo key, out ResourceItem value)
		{
			return this.CultureMap.TryGetValue (key, out value);
		}

		public IEnumerable<ResourceItem> Values
		{
			get
			{
				return this.CultureMap.Values;
			}
		}

		public ResourceItem this[CultureInfo key]
		{
			get
			{
				return this.CultureMap[key];
			}
		}

		#endregion

		#region IReadOnlyCollection<KeyValuePair<CultureInfo,ResourceItem>> Members

		public int Count
		{
			get
			{
				return this.CultureMap.Count;
			}
		}

		#endregion

		#region IEnumerable<KeyValuePair<CultureInfo,ResourceItem>> Members

		public IEnumerator<KeyValuePair<CultureInfo, ResourceItem>> GetEnumerator()
		{
			return this.CultureMap.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}

		#endregion

		private readonly string symbolName;
		internal readonly Dictionary<CultureInfo,ResourceItem> CultureMap;
	}
}
