using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Epsitec.Data.Platform.Directories.Helpers
{
	static class DirectoriesValueHelper
	{
		public static string TryGetFromXElement(string AttributeName,XElement Element)
		{
			return Element.Attribute (AttributeName) != null ? Element.Attribute (AttributeName).Value : "";
		}
	}
}
