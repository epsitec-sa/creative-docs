//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Statut : OK/PA, 21/03/2004

namespace Epsitec.Common.Printing
{
	/// <summary>
	/// La classe PaperSize indique les dimensions d'un papier, ainsi que sa
	/// nature.
	/// </summary>
	public class PaperSize
	{
		internal PaperSize(System.Drawing.Printing.PaperSize ps)
		{
			this.ps = ps;
		}
		
		
		public PaperSize(string name, double width, double height)
		{
			int dx = (int) System.Math.Floor (width / PaperSize.Millimeters);
			int dy = (int) System.Math.Floor (height / PaperSize.Millimeters);
			
			this.ps = new System.Drawing.Printing.PaperSize (name, dx, dy);
		}
		
		public PaperSize(string name, Drawing.Size size) : this (name, size.Width, size.Height)
		{
		}
		
		
		public double							Width
		{
			get
			{
				return this.ps.Width * PaperSize.Millimeters;
			}
		}
		
		public double							Height
		{
			get
			{
				return this.ps.Height * PaperSize.Millimeters;
			}
		}
		
		public Drawing.Size						Size
		{
			get
			{
				return new Drawing.Size (this.Width, this.Height);
			}
		}
		
		public string							Name
		{
			get 
			{
				return this.ps.PaperName;
			}
		}
		
		public PaperKind						Kind
		{
			get
			{
				switch (this.ps.Kind)
				{
					case System.Drawing.Printing.PaperKind.A2:										return PaperKind.A2;
					case System.Drawing.Printing.PaperKind.A3:										return PaperKind.A3;
					case System.Drawing.Printing.PaperKind.A3Extra:									return PaperKind.A3Extra;
					case System.Drawing.Printing.PaperKind.A3ExtraTransverse:						return PaperKind.A3Extra;
					case System.Drawing.Printing.PaperKind.A3Rotated:								return PaperKind.A3;
					case System.Drawing.Printing.PaperKind.A3Transverse:							return PaperKind.A3;
					case System.Drawing.Printing.PaperKind.A4:										return PaperKind.A4;
					case System.Drawing.Printing.PaperKind.A4Extra:									return PaperKind.A4Extra;
					case System.Drawing.Printing.PaperKind.A4Plus:									return PaperKind.A4Plus;
					case System.Drawing.Printing.PaperKind.A4Rotated:								return PaperKind.A4;
					case System.Drawing.Printing.PaperKind.A4Small:									return PaperKind.A4Small;
					case System.Drawing.Printing.PaperKind.A4Transverse:							return PaperKind.A4;
					case System.Drawing.Printing.PaperKind.A5:										return PaperKind.A5;
					case System.Drawing.Printing.PaperKind.A5Extra:									return PaperKind.A5Extra;
					case System.Drawing.Printing.PaperKind.A5Rotated:								return PaperKind.A5;
					case System.Drawing.Printing.PaperKind.A5Transverse:							return PaperKind.A5;
					case System.Drawing.Printing.PaperKind.A6:										return PaperKind.A6;
					case System.Drawing.Printing.PaperKind.A6Rotated:								return PaperKind.A6;
					case System.Drawing.Printing.PaperKind.APlus:									return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.B4:										return PaperKind.B4Envelope;
					case System.Drawing.Printing.PaperKind.B4Envelope:								return PaperKind.B4Envelope;
					case System.Drawing.Printing.PaperKind.B4JisRotated:							return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.B5:										return PaperKind.B5Envelope;
					case System.Drawing.Printing.PaperKind.B5Envelope:								return PaperKind.B5Envelope;
					case System.Drawing.Printing.PaperKind.B5Extra:									return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.B5JisRotated:							return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.B5Transverse:							return PaperKind.B5Envelope;
					case System.Drawing.Printing.PaperKind.B6Envelope:								return PaperKind.B6Envelope;
					case System.Drawing.Printing.PaperKind.B6Jis:									return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.B6JisRotated:							return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.BPlus:									return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.C3Envelope:								return PaperKind.C3Envelope;
					case System.Drawing.Printing.PaperKind.C4Envelope:								return PaperKind.C4Envelope;
					case System.Drawing.Printing.PaperKind.C5Envelope:								return PaperKind.C5Envelope;
					case System.Drawing.Printing.PaperKind.C65Envelope:								return PaperKind.C65Envelope;
					case System.Drawing.Printing.PaperKind.C6Envelope:								return PaperKind.C6Envelope;
					case System.Drawing.Printing.PaperKind.CSheet:									return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Custom:									return PaperKind.Custom;
					case System.Drawing.Printing.PaperKind.DLEnvelope:								return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.DSheet:									return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.ESheet:									return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Executive:								return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Folio:									return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.GermanLegalFanfold:						return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.GermanStandardFanfold:					return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.InviteEnvelope:							return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.IsoB4:									return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.ItalyEnvelope:							return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.JapaneseDoublePostcard:					return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.JapaneseDoublePostcardRotated:			return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.JapaneseEnvelopeChouNumber3:				return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.JapaneseEnvelopeChouNumber3Rotated:		return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.JapaneseEnvelopeChouNumber4:				return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.JapaneseEnvelopeChouNumber4Rotated:		return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.JapaneseEnvelopeKakuNumber2:				return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.JapaneseEnvelopeKakuNumber2Rotated:		return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.JapaneseEnvelopeKakuNumber3:				return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.JapaneseEnvelopeKakuNumber3Rotated:		return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.JapaneseEnvelopeYouNumber4:				return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.JapaneseEnvelopeYouNumber4Rotated:		return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.JapanesePostcard:						return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.JapanesePostcardRotated:					return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Ledger:									return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Legal:									return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.LegalExtra:								return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Letter:									return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.LetterExtra:								return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.LetterExtraTransverse:					return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.LetterPlus:								return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.LetterRotated:							return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.LetterSmall:								return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.LetterTransverse:						return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.MonarchEnvelope:							return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Note:									return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Number10Envelope:						return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Number11Envelope:						return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Number12Envelope:						return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Number14Envelope:						return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Number9Envelope:							return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.PersonalEnvelope:						return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Prc16K:									return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Prc16KRotated:							return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Prc32K:									return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Prc32KBig:								return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Prc32KBigRotated:						return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Prc32KRotated:							return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.PrcEnvelopeNumber1:						return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.PrcEnvelopeNumber10:						return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.PrcEnvelopeNumber10Rotated:				return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.PrcEnvelopeNumber1Rotated:				return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.PrcEnvelopeNumber2:						return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.PrcEnvelopeNumber2Rotated:				return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.PrcEnvelopeNumber3:						return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.PrcEnvelopeNumber3Rotated:				return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.PrcEnvelopeNumber4:						return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.PrcEnvelopeNumber4Rotated:				return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.PrcEnvelopeNumber5:						return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.PrcEnvelopeNumber5Rotated:				return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.PrcEnvelopeNumber6:						return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.PrcEnvelopeNumber6Rotated:				return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.PrcEnvelopeNumber7:						return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.PrcEnvelopeNumber7Rotated:				return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.PrcEnvelopeNumber8:						return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.PrcEnvelopeNumber8Rotated:				return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.PrcEnvelopeNumber9:						return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.PrcEnvelopeNumber9Rotated:				return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Quarto:									return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Standard10x11:							return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Standard10x14:							return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Standard11x17:							return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Standard12x11:							return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Standard15x11:							return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Standard9x11:							return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Statement:								return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.Tabloid:									return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.TabloidExtra:							return PaperKind.Other;
					case System.Drawing.Printing.PaperKind.USStandardFanfold:						return PaperKind.Other;
				}
				
				return PaperKind.Other;
			}
		}
		
		
		internal System.Drawing.Printing.PaperSize GetPaperSize()
		{
			return this.ps;
		}
		
		
		private const double					Millimeters = 25.4 / 100;
		System.Drawing.Printing.PaperSize		ps;
	}
}
