//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 21/03/2004

namespace Epsitec.Common.Printing
{
	/// <summary>
	/// La classe PageSettings décrit une page (dimensions imprimables, marges, taille
	/// et type de papier, orientation, source du papier, etc.)
	/// </summary>
	public class PageSettings
	{
		internal PageSettings(System.Drawing.Printing.PageSettings ps)
		{
			this.ps = ps;
		}
		
		
		public Drawing.Rectangle				Bounds
		{
			get
			{
				return Drawing.Rectangle.Empty;
			}
		}
		
		public Drawing.Margins					Margins
		{
			get
			{
				double left   = this.ps.Margins.Left * PageSettings.Millimeters;
				double right  = this.ps.Margins.Right * PageSettings.Millimeters;
				double top    = this.Margins.Top * PageSettings.Millimeters;
				double bottom = this.Margins.Bottom * PageSettings.Millimeters;
				
				return new Drawing.Margins (left, right, top, bottom);
			}
		}
		
		public PaperSize						PaperSize
		{
			get
			{
				return new PaperSize (this.ps.PaperSize);
			}
			set
			{
				this.ps.PaperSize = value.GetPaperSize ();
			}
		}
		
		public bool								Landscape
		{
			get { return this.ps.Landscape; }
			set { this.ps.Landscape = value; }
		}
		
		public PaperSource						PaperSource
		{
			get
			{
				return new PaperSource (this.ps.PaperSource);
			}
			set
			{
				this.ps.PaperSource = value.GetPaperSource ();
			}
		}
		
		public PrinterResolution				PrinterResolution
		{
			get
			{
				return new PrinterResolution (this.ps.PrinterResolution);
			}
		}
		
		public PrinterSettings					PrinterSettings
		{
			get
			{
				return new PrinterSettings (this.ps.PrinterSettings);
			}
		}
		
		
		private const double					Millimeters = 25.4 / 100;
		System.Drawing.Printing.PageSettings	ps;
	}
}
