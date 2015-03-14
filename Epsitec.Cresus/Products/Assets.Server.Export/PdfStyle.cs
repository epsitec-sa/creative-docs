﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Helpers;

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

		public PdfStyle(System.Xml.XmlReader reader)
		{
			this.LabelColor      = reader.ReadTypeAttribute<ExportColor> (X.Attr.LabelColor);
			this.EvenColor       = reader.ReadTypeAttribute<ExportColor> (X.Attr.EvenColor);
			this.OddColor        = reader.ReadTypeAttribute<ExportColor> (X.Attr.OddColor);
			this.BorderColor     = reader.ReadTypeAttribute<ExportColor> (X.Attr.BorderColor);
			this.BorderThickness = (double) reader.ReadDecimalAttribute (X.Attr.BorderThickness);

			reader.Read ();
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
			return !(s1 == s2);
		}

		public override bool Equals(object obj)
		{
			if (obj is PdfStyle)
			{
				return this.Equals ((PdfStyle) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.LabelColor.GetHashCode ()
				 ^ this.EvenColor.GetHashCode ()
				 ^ this.OddColor.GetHashCode ()
				 ^ this.BorderColor.GetHashCode ()
				 ^ this.BorderThickness.GetHashCode ();
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
				case PdfPredefinedStyle.Frameless:
					return new PdfStyle (ExportColor.Grey, ExportColor.Transparent, ExportColor.Transparent, ExportColor.Black, 0.0);

				case PdfPredefinedStyle.LightFrame:
					return new PdfStyle (ExportColor.Grey, ExportColor.Transparent, ExportColor.Transparent, ExportColor.Grey, 0.1);

				case PdfPredefinedStyle.BoldFrame:
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


		public void Serialize(System.Xml.XmlWriter writer, string name)
		{
			writer.WriteStartElement (name);

			writer.WriteTypeAttribute    (X.Attr.LabelColor,      this.LabelColor);
			writer.WriteTypeAttribute    (X.Attr.EvenColor,       this.EvenColor);
			writer.WriteTypeAttribute    (X.Attr.OddColor,        this.OddColor);
			writer.WriteTypeAttribute    (X.Attr.BorderColor,     this.BorderColor);
			writer.WriteDecimalAttribute (X.Attr.BorderThickness, (decimal) this.BorderThickness);

			writer.WriteEndElement ();
		}


		public readonly ExportColor				LabelColor;
		public readonly ExportColor				EvenColor;
		public readonly ExportColor				OddColor;
		public readonly ExportColor				BorderColor;
		public readonly double					BorderThickness;
	}
}