//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.Export;

namespace Epsitec.Cresus.Assets.Export.Helpers
{
	public static class ExportFontHelpers
	{
		public static string GetFontName(this ExportFont font)
		{
			switch (font)
			{
				case ExportFont.Arial:
					return "Arial";

				case ExportFont.Times:
					return "Times New Roman";

				case ExportFont.Courier:
					return "Courier New";

				default:
					return null;
			}
		}
	}
}
