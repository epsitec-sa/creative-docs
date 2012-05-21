//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Statut : OK/PA, 21/03/2004

namespace Epsitec.Common.Printing
{
	/// <summary>
	/// L'énumération PaperKind indique la nature d'un papier.
	/// </summary>
	public enum PaperKind
	{
		A2,
		A3, A3Extra,
		A4, A4Extra, A4Plus, A4Small,
		A5, A5Extra,
		A6,
		
		B4Envelope, B5Envelope, B6Envelope,
		C3Envelope, C4Envelope, C5Envelope, C65Envelope, C6Envelope,
		
		Other,
		
		Custom
	}
}
