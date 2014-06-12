//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.Export;

namespace Epsitec.Cresus.Assets.App.Export
{
	public static class PdfStyleHelpers
	{
		public static int? StyleToInt(PdfStyle style)
		{
			int value = 0;

			foreach (var e in PdfPredefinedStyleHelpers.Predefined)
			{
				if (style == PdfStyle.Factory (e))
				{
					return value;
				}

				value++;
			}

			return null;
		}

		public static string GetDescription(PdfStyle style)
		{
			var predefined = PdfStyleHelpers.StyleToPredefined (style);
			var desc = PdfPredefinedStyleHelpers.GetPredefinedName (predefined);

			if (string.IsNullOrEmpty (desc))
			{
				return "Sur mesure";
			}
			else
			{
				return desc;
			}
		}

		private static PdfPredefinedStyle StyleToPredefined(PdfStyle style)
		{
			int value = 0;

			foreach (var e in PdfPredefinedStyleHelpers.Predefined)
			{
				if (style == PdfStyle.Factory (e))
				{
					return e;
				}

				value++;
			}

			return PdfPredefinedStyle.Unknown;
		}
	}
}