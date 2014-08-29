//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
using Epsitec.Cresus.Assets.Server.Export;

namespace Epsitec.Cresus.Assets.App.Export
{
	public static class PdfPredefinedStyleHelpers
	{
		public static string Labels
		{
			//	Retourne le texte pour peupler le ComboStackedController.
			get
			{
				return string.Join ("<br/>", PdfPredefinedStyleHelpers.Predefined.Select (x => PdfPredefinedStyleHelpers.GetPredefinedName (x)));
			}
		}

		public static PdfPredefinedStyle IntToPredefined(int? value)
		{
			if (value.HasValue)
			{
				var e = PdfPredefinedStyleHelpers.Predefined.ToArray ();

				if (value >= 0 && value < e.Length)
				{
					return e[value.Value];
				}
			}

			return PdfPredefinedStyle.Unknown;
		}

		public static int? PredefinedToInt(PdfPredefinedStyle predefined)
		{
			int value = 0;

			foreach (var e in PdfPredefinedStyleHelpers.Predefined)
			{
				if (e == predefined)
				{
					return value;
				}

				value++;
			}

			return null;
		}


		public static string GetPredefinedName(PdfPredefinedStyle predefined)
		{
			return EnumKeyValues.GetEnumKeyValue (predefined).Values.Last ().ToString ();
		}

		public static IEnumerable<PdfPredefinedStyle> Predefined
		{
			//	Retourne la liste des styles supportés, dans l'ordre où ils apparaissent
			//	dans la UI.
			get
			{
				yield return PdfPredefinedStyle.Frameless;
				yield return PdfPredefinedStyle.LightFrame;
				yield return PdfPredefinedStyle.StandardFrame;
				yield return PdfPredefinedStyle.BoldFrame;
				yield return PdfPredefinedStyle.Contrast;
				yield return PdfPredefinedStyle.GreyEvenOdd;
				yield return PdfPredefinedStyle.BlueEvenOdd;
				yield return PdfPredefinedStyle.YellowEvenOdd;
				yield return PdfPredefinedStyle.RedEvenOdd;
				yield return PdfPredefinedStyle.GreenEvenOdd;
				yield return PdfPredefinedStyle.Colored;
				yield return PdfPredefinedStyle.Kitch;
			}
		}
	}
}