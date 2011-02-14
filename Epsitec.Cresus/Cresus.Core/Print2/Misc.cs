//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Print2
{
	public static class Misc
	{
		public static string DocumentOptionToString(DocumentOption option)
		{
			return option.ToString ();
		}

		public static DocumentOption StringToDocumentOption(string name)
		{
			DocumentOption option;

			if (System.Enum.TryParse (name, out option))
			{
				return option;
			}
			else
			{
				return DocumentOption.None;
			}
		}


		public static string PageTypeToString(PageType type)
		{
			return type.ToString ();
		}

		public static PageType StringToPageType(string name)
		{
			PageType type;

			if (System.Enum.TryParse (name, out type))
			{
				return type;
			}
			else
			{
				return PageType.Unknown;
			}
		}
	}
}
