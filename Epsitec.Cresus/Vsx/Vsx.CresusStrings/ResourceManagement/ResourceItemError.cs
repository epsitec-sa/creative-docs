using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Epsitec.Cresus.ResourceManagement
{
	[Flags]
	public enum ResourceItemErrors
	{
		NoId				= 0x01,
		NoName				= 0x02,
		IdMismatch			= 0x04,
		NameMismatch		= 0x08,
		NoNeutralResource	= 0x10,
		UndefinedResource	= 0x20,
	}

	public class ResourceItemError : ResourceItem
	{
		public ResourceItemError(ResourceItemErrors reasons, string id, string name, XElement element, ResourceBundle sourceBundle, ResourceItem neutralItem)
			: base (id, name, element, sourceBundle, neutralItem)
		{
			this.reasons = reasons;
		}

		public ResourceItemErrors Reasons
		{
			get
			{
				return this.reasons;
			}
		}

		private readonly ResourceItemErrors reasons;
	}
}
