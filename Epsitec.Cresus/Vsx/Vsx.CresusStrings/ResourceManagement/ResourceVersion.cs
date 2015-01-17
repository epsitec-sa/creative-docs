using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Epsitec.Cresus.ResourceManagement
{
	public class ResourceVersion
	{
		public ResourceVersion(XElement element)
		{
			this.element = element;
		}

		public string Id
		{
			get
			{
				return this.element.Attribute ("id").GetString ();
			}
		}

		public string Build
		{
			get
			{
				return this.element.Attribute ("build").GetString ();
			}
		}

		public string Date
		{
			get
			{
				return this.element.Attribute ("date").GetString ();
			}
		}

		private readonly XElement element;
	}
}
