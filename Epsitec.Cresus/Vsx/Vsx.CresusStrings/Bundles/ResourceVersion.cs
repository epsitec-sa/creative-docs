using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Epsitec.Cresus.Strings.Bundles
{
	public class ResourceVersion
	{
		public ResourceVersion(XElement element)
		{
			this.id = element.Attribute ("id").GetString ();
			this.build = element.Attribute ("build").GetString ();
			this.date = element.Attribute ("date").GetString ();
		}

		private readonly string id;
		private readonly string build;
		private readonly string date;
	}
}
