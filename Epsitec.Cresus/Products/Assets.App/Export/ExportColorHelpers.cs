//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
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
					return Res.Strings.Enum.ExportColor.Transparent.ToString ();

				case ExportColor.White:
					return Res.Strings.Enum.ExportColor.White.ToString ();

				case ExportColor.LightGrey:
					return Res.Strings.Enum.ExportColor.LightGrey.ToString ();

				case ExportColor.Grey:
					return Res.Strings.Enum.ExportColor.Grey.ToString ();

				case ExportColor.DarkGrey:
					return Res.Strings.Enum.ExportColor.DarkGrey.ToString ();

				case ExportColor.Black:
					return Res.Strings.Enum.ExportColor.Black.ToString ();

				case ExportColor.LightRed:
					return Res.Strings.Enum.ExportColor.LightRed.ToString ();

				case ExportColor.LightGreen:
					return Res.Strings.Enum.ExportColor.LightGreen.ToString ();

				case ExportColor.LightBlue:
					return Res.Strings.Enum.ExportColor.LightBlue.ToString ();

				case ExportColor.LightYellow:
					return Res.Strings.Enum.ExportColor.LightYellow.ToString ();

				case ExportColor.LightPurple:
					return Res.Strings.Enum.ExportColor.LightPurple.ToString ();

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