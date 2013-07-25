using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Epsitec.Cresus.Strings.Bundles
{
	public class ResourceVersions : IEnumerable<ResourceVersion>
	{
		public ResourceVersions(XElement element)
		{
			this.items = element.Elements ("Version").Select (e => new ResourceVersion (e)).ToList();
		}

		#region IEnumerable<ModuleVersion> Members

		public IEnumerator<ResourceVersion> GetEnumerator()
		{
			return this.items.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.items.GetEnumerator ();
		}

		#endregion

		private readonly List<ResourceVersion> items;
	}
}
