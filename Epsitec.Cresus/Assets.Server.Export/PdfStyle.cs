//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.Export
{
	public struct PdfStyle : System.IEquatable<PdfStyle>
	{
		public PdfStyle(ExportColor labelColor, ExportColor evenColor, ExportColor oddColor, ExportColor borderColor, double borderThickness)
		{
			this.LabelColor      = labelColor;
			this.EvenColor       = evenColor;
			this.OddColor        = oddColor;
			this.BorderColor     = borderColor;
			this.BorderThickness = borderThickness;
		}


		public static bool operator==(PdfStyle s1, PdfStyle s2)
		{
			return s1.LabelColor      == s2.LabelColor
				&& s1.EvenColor       == s2.EvenColor
				&& s1.OddColor        == s2.OddColor
				&& s1.BorderColor     == s2.BorderColor
				&& s1.BorderThickness == s2.BorderThickness;
		}

		public static bool operator!=(PdfStyle s1, PdfStyle s2)
		{
			return s1.LabelColor      != s2.LabelColor
				|| s1.EvenColor       != s2.EvenColor
				|| s1.OddColor        != s2.OddColor
				|| s1.BorderColor     != s2.BorderColor
				|| s1.BorderThickness != s2.BorderThickness;
		}


		#region IEquatable<PdfStyle> Members
		public bool Equals(PdfStyle other)
		{
			return this == other;
		}
		#endregion


		public static PdfStyle Factory(PdfPredefinedStyle predefined)
		{
			switch (predefined)
			{
				case PdfPredefinedStyle.Light:
					return new PdfStyle (ExportColor.Grey, ExportColor.Transparent, ExportColor.Transparent, ExportColor.Black, 0.0);

				case PdfPredefinedStyle.Discreet:
					return new PdfStyle (ExportColor.Grey, ExportColor.Transparent, ExportColor.Transparent, ExportColor.Grey, 0.1);

				case PdfPredefinedStyle.Bold:
					return new PdfStyle (ExportColor.Grey, ExportColor.Transparent, ExportColor.Transparent, ExportColor.Black, 0.5);

				case PdfPredefinedStyle.GreyEvenOdd:
					return new PdfStyle (ExportColor.Grey, ExportColor.Transparent, ExportColor.LightGrey, ExportColor.White, 0.2);

				case PdfPredefinedStyle.BlueEvenOdd:
					return new PdfStyle (ExportColor.Grey, ExportColor.Transparent, ExportColor.LightBlue, ExportColor.White, 0.2);

				case PdfPredefinedStyle.YellowEvenOdd:
					return new PdfStyle (ExportColor.Grey, ExportColor.Transparent, ExportColor.LightYellow, ExportColor.White, 0.2);

				case PdfPredefinedStyle.RedEvenOdd:
					return new PdfStyle (ExportColor.Grey, ExportColor.Transparent, ExportColor.LightRed, ExportColor.White, 0.2);

				case PdfPredefinedStyle.GreenEvenOdd:
					return new PdfStyle (ExportColor.Grey, ExportColor.Transparent, ExportColor.LightGreen, ExportColor.White, 0.2);

				case PdfPredefinedStyle.Colored:
					return new PdfStyle (ExportColor.Transparent, ExportColor.LightBlue, ExportColor.LightYellow, ExportColor.White, 0.2);

				case PdfPredefinedStyle.Contrast:
					return new PdfStyle (ExportColor.Grey, ExportColor.Transparent, ExportColor.DarkGrey, ExportColor.Black, 0.1);

				case PdfPredefinedStyle.Kitch:
					return new PdfStyle (ExportColor.LightYellow, ExportColor.LightGreen, ExportColor.LightPurple, ExportColor.Black, 0.1);

				default:
					return new PdfStyle (ExportColor.Grey, ExportColor.Transparent, ExportColor.Transparent, ExportColor.Black, 0.1);
			}
		}


		public readonly ExportColor				LabelColor;
		public readonly ExportColor				EvenColor;
		public readonly ExportColor				OddColor;
		public readonly ExportColor				BorderColor;
		public readonly double					BorderThickness;
	}
}