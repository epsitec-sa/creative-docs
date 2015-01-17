using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Epsitec.Cresus.ResourceManagement
{
	public abstract class ResourceElement : ResourceNode
	{
		public ResourceElement(XElement element)
		{
			element.ThrowIfNull ();
			this.element = element;
		}

		public XElement Element
		{
			get
			{
				return this.element;
			}
		}

		private readonly XElement element;
	}
}
