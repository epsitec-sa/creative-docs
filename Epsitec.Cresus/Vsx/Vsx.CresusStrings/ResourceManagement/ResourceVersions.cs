using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Epsitec.Cresus.ResourceManagement
{
	public class ResourceVersions : IEnumerable<ResourceVersion>
	{
		public ResourceVersions(XElement element)
		{
			this.element = element;
			this.items = new Lazy<IEnumerable<ResourceVersion>>(() => this.element.Elements ("Version").Select (e => new ResourceVersion (e)).ToList());
		}

		#region IEnumerable<ModuleVersion> Members

		public IEnumerator<ResourceVersion> GetEnumerator()
		{
			return this.items.Value.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}

		#endregion

		private readonly XElement element;
		private readonly Lazy<IEnumerable<ResourceVersion>> items;
	}
}
