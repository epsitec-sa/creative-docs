//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 21/03/2004

namespace Epsitec.Common.Printing
{
	/// <summary>
	/// L'énumération PaperSourceKind indique les diverses sources possibles pour
	/// l'alimentation papier.
	/// </summary>
	public enum PaperSourceKind
	{
		PaperCassette,
		
		Envelope,
		LargeFormatPaper,
		SmallFormatPaper,
		
		DefaultInputBin,
		LargeCapacityBin,
		LowerBin,
		MiddleBin,
		UpperBin,
		
		AutomaticFeed,
		ManualPaperFeed,
		ManuelEnvelopeFeed,
		TractorFeed,
		
		Custom,
		
		Other
	}
}
