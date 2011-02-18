namespace Epsitec.Common.Printing
{
	/// <summary>
	/// Summary description for PaperSource.
	/// </summary>
	public class PaperSource
	{
		internal PaperSource(System.Drawing.Printing.PaperSource ps)
		{
			this.ps = ps;
		}
		
		
		public PaperSourceKind					Kind
		{
			get
			{
				switch (this.ps.Kind)
				{
					case System.Drawing.Printing.PaperSourceKind.AutomaticFeed:	return PaperSourceKind.AutomaticFeed;
					case System.Drawing.Printing.PaperSourceKind.Cassette:		return PaperSourceKind.PaperCassette;
					case System.Drawing.Printing.PaperSourceKind.Custom:		return PaperSourceKind.Custom;
					case System.Drawing.Printing.PaperSourceKind.Envelope:		return PaperSourceKind.Envelope;
					case System.Drawing.Printing.PaperSourceKind.FormSource:	return PaperSourceKind.DefaultInputBin;
					case System.Drawing.Printing.PaperSourceKind.LargeCapacity:	return PaperSourceKind.LargeCapacityBin;
					case System.Drawing.Printing.PaperSourceKind.LargeFormat:	return PaperSourceKind.LargeFormatPaper;
					case System.Drawing.Printing.PaperSourceKind.Lower:			return PaperSourceKind.LowerBin;
					case System.Drawing.Printing.PaperSourceKind.Manual:		return PaperSourceKind.ManualPaperFeed;
					case System.Drawing.Printing.PaperSourceKind.ManualFeed:	return PaperSourceKind.ManuelEnvelopeFeed;
					case System.Drawing.Printing.PaperSourceKind.Middle:		return PaperSourceKind.MiddleBin;
					case System.Drawing.Printing.PaperSourceKind.SmallFormat:	return PaperSourceKind.SmallFormatPaper;
					case System.Drawing.Printing.PaperSourceKind.TractorFeed:	return PaperSourceKind.TractorFeed;
					case System.Drawing.Printing.PaperSourceKind.Upper:			return PaperSourceKind.UpperBin;
				}
				
				return PaperSourceKind.Other;
			}
		}
		
		public string							Name
		{
			get
			{
				return this.ps.SourceName;
			}
		}
		
		
		internal System.Drawing.Printing.PaperSource GetPaperSource()
		{
			return this.ps;
		}
		
		
		System.Drawing.Printing.PaperSource		ps;
	}
}
