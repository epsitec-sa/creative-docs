using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec.Cresus.ResourceManagement
{
	public class ResourceStack : IResourceTable
	{
		public ResourceStack(IEnumerable<IResourceTable> tables)
		{
			foreach (var table in tables)
			{
				this.stack.Push (table);
			}
		}

		#region IResourceTable Members

		public System.Globalization.CultureInfo Culture
		{
			get
			{
				throw new NotImplementedException ();
			}
		}

		#endregion

		#region IReadOnlyDictionary<string,ResourceItem> Members

		public bool ContainsKey(string key)
		{
			throw new NotImplementedException ();
		}

		public IEnumerable<string> Keys
		{
			get
			{
				throw new NotImplementedException ();
			}
		}

		public bool TryGetValue(string key, out ResourceItem value)
		{
			throw new NotImplementedException ();
		}

		public IEnumerable<ResourceItem> Values
		{
			get
			{
				throw new NotImplementedException ();
			}
		}

		public ResourceItem this[string key]
		{
			get
			{
				throw new NotImplementedException ();
			}
		}

		#endregion

		#region IReadOnlyCollection<KeyValuePair<string,ResourceItem>> Members

		public int Count
		{
			get
			{
				throw new NotImplementedException ();
			}
		}

		#endregion

		#region IEnumerable<KeyValuePair<string,ResourceItem>> Members

		public IEnumerator<KeyValuePair<string, ResourceItem>> GetEnumerator()
		{
			throw new NotImplementedException ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException ();
		}

		#endregion

		private readonly Stack<IResourceTable> stack = new Stack<IResourceTable> ();
	}
}
