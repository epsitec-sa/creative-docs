//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.Export;

namespace Epsitec.Cresus.Assets.App.Export
{
	public static class PdfStyleHelpers
	{
		public static string Labels
		{
			//	Retourne le texte pour peupler le ComboStackedController.
			get
			{
				return string.Join ("<br/>", PdfStyleHelpers.Styles.Select (x => PdfStyleHelpers.GetStyleName (x)));
			}
		}

		public static PdfStyle IntToStyle(int? value)
		{
			if (value.HasValue)
			{
				var e = PdfStyleHelpers.Styles.ToArray ();

				if (value >= 0 && value < e.Length)
				{
					return e[value.Value];
				}
			}

			return PdfStyle.Unknown;
		}

		public static int? StyleToInt(PdfStyle style)
		{
			int value = 0;

			foreach (var e in PdfStyleHelpers.Styles)
			{
				if (e == style)
				{
					return value;
				}

				value++;
			}

			return null;
		}


		private static string GetStyleName(PdfStyle style)
		{
			switch (style)
			{
				case PdfStyle.Default:
					return "Normal";

				case PdfStyle.Light:
					return "Sans cadres";

				case PdfStyle.Bold:
					return "Cadres gras";

				case PdfStyle.GreyEvenOdd:
					return "Lignes impaires grises";

				case PdfStyle.BlueEvenOdd:
					return "Lignes impaires bleues";

				case PdfStyle.YellowEvenOdd:
					return "Lignes impaires jaunes";

				case PdfStyle.RedEvenOdd:
					return "Lignes impaires roses";

				case PdfStyle.GreenEvenOdd:
					return "Lignes impaires vertes";

				case PdfStyle.Colored:
					return "Lignes colorées";

				default:
					return null;
			}
		}

		private static IEnumerable<PdfStyle> Styles
		{
			//	Retourne la liste des styles supportés, dans l'ordre où ils apparaissent
			//	dans la UI.
			get
			{
				yield return PdfStyle.Default;
				yield return PdfStyle.Light;
				yield return PdfStyle.Bold;
				yield return PdfStyle.GreyEvenOdd;
				yield return PdfStyle.BlueEvenOdd;
				yield return PdfStyle.YellowEvenOdd;
				yield return PdfStyle.RedEvenOdd;
				yield return PdfStyle.GreenEvenOdd;
				yield return PdfStyle.Colored;
			}
		}
	}
}