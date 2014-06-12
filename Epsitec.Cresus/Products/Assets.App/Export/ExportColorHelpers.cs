//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.Export;

namespace Epsitec.Cresus.Assets.App.Export
{
	public static class ExportColorHelpers
	{
		public static string Labels
		{
			//	Retourne le texte pour peupler le ComboStackedController.
			get
			{
				return string.Join ("<br/>", ExportColorHelpers.Colors.Select (x => ExportColorHelpers.GetStyleName (x)));
			}
		}

		public static ExportColor IntToColor(int? value)
		{
			if (value.HasValue)
			{
				var e = ExportColorHelpers.Colors.ToArray ();

				if (value >= 0 && value < e.Length)
				{
					return e[value.Value];
				}
			}

			return ExportColor.Unknown;
		}

		public static int? ColorToInt(ExportColor color)
		{
			int value = 0;

			foreach (var e in ExportColorHelpers.Colors)
			{
				if (e == color)
				{
					return value;
				}

				value++;
			}

			return null;
		}


		private static string GetStyleName(ExportColor color)
		{
			switch (color)
			{
				case ExportColor.Transparent:
					return "Transparent";

				case ExportColor.White:
					return "Blanc";

				case ExportColor.LightGrey:
					return "Gris clair";

				case ExportColor.Grey:
					return "Gris moyen";

				case ExportColor.DarkGrey:
					return "Gris foncé";

				case ExportColor.Black:
					return "Noir";

				case ExportColor.LightRed:
					return "Rose";

				case ExportColor.LightGreen:
					return "Vert clair";

				case ExportColor.LightBlue:
					return "Bleu clair";

				case ExportColor.LightYellow:
					return "Jaune clair";

				case ExportColor.LightPurple:
					return "Lilas";

				default:
					return null;
			}
		}

		private static IEnumerable<ExportColor> Colors
		{
			//	Retourne la liste des couleurs supportées, dans l'ordre où elles apparaissent
			//	dans la UI.
			get
			{
				yield return ExportColor.Transparent;
				yield return ExportColor.White;
				yield return ExportColor.LightGrey;
				yield return ExportColor.Grey;
				yield return ExportColor.DarkGrey;
				yield return ExportColor.Black;
				yield return ExportColor.LightRed;
				yield return ExportColor.LightGreen;
				yield return ExportColor.LightBlue;
				yield return ExportColor.LightYellow;
				yield return ExportColor.LightPurple;
			}
		}
	}
}