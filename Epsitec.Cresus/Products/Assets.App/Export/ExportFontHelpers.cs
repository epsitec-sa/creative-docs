//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.Export;

namespace Epsitec.Cresus.Assets.App.Export
{
	public static class ExportFontHelpers
	{
		public static string Labels
		{
			//	Retourne le texte pour peupler le ComboStackedController.
			get
			{
				return string.Join ("<br/>", ExportFontHelpers.Fonts.Select (x => ExportFontHelpers.GetFontName (x)));
			}
		}

		public static ExportFont IntToFont(int? value)
		{
			if (value.HasValue)
			{
				var e = ExportFontHelpers.Fonts.ToArray ();

				if (value >= 0 && value < e.Length)
				{
					return e[value.Value];
				}
			}

			return ExportFont.Unknown;
		}

		public static int? FontToInt(ExportFont font)
		{
			int value = 0;

			foreach (var e in ExportFontHelpers.Fonts)
			{
				if (e == font)
				{
					return value;
				}

				value++;
			}

			return null;
		}


		private static string GetFontName(ExportFont font)
		{
			switch (font)
			{
				case ExportFont.Arial:
					return "Arial";

				case ExportFont.Times:
					return "Times";

				case ExportFont.Courier:
					return "Courrier";

				default:
					return null;
			}
		}

		private static IEnumerable<ExportFont> Fonts
		{
			//	Retourne la liste des polices supportées, dans l'ordre où elles apparaissent
			//	dans la UI.
			get
			{
				yield return ExportFont.Arial;
				yield return ExportFont.Times;
				yield return ExportFont.Courier;
			}
		}
	}
}