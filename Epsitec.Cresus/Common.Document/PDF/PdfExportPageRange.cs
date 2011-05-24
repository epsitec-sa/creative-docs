using System.Collections.Generic;

namespace Epsitec.Common.Document.PDF
{
	class PdfExportPageRange
	{
		public PdfExportPageRange(int max)
		{
			this.Max = max;
			this.From = 1;
			this.To = max;
		}
		public void Constrain()
		{
			this.From = System.Math.Min (this.From, this.Max);
			this.To = System.Math.Min (this.To, this.Max);
			if (this.From > this.To)
				Misc.Swap (ref this.From, ref this.To);
		}
		public IEnumerable<int> GetPrintablePageList(Modifier modifier)
		{
			List<int> pageList = new List<int> ();
			if (this.JustOneMaster)
				pageList.Add (this.From);
			else
				for (int page = this.From; page <= this.To; page++)
				{
					int rank = modifier.PrintablePageRank (page - 1);
					if (rank != -1)
						pageList.Add (rank);
				}
			return pageList;
		}
		public int Max;
		public int From;
		public int To;
		public bool JustOneMaster;
		public int Total
		{
			get
			{
				return this.To - this.From + 1;
			}
		}
	}
}
