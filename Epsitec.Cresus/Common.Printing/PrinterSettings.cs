//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 21/03/2004

namespace Epsitec.Common.Printing
{
	/// <summary>
	/// Summary description for PrinterSettings.
	/// </summary>
	public class PrinterSettings
	{
		internal PrinterSettings(System.Drawing.Printing.PrinterSettings ps)
		{
			this.ps = ps;
		}
		
		
		public bool								CanDuplex
		{
			get
			{
				return this.ps.CanDuplex;
			}
		}
		
		public DuplexMode						Duplex
		{
			get
			{
				switch (this.ps.Duplex)
				{
					case System.Drawing.Printing.Duplex.Default:	return DuplexMode.Default;
					case System.Drawing.Printing.Duplex.Horizontal:	return DuplexMode.Horizontal;
					case System.Drawing.Printing.Duplex.Simplex:	return DuplexMode.Simplex;
					case System.Drawing.Printing.Duplex.Vertical:	return DuplexMode.Vertical;
				}
				
				return DuplexMode.Default;
			}
			set
			{
				switch (value)
				{
					case DuplexMode.Default:	this.ps.Duplex = System.Drawing.Printing.Duplex.Default;	break;
					case DuplexMode.Horizontal:	this.ps.Duplex = System.Drawing.Printing.Duplex.Horizontal;	break;
					case DuplexMode.Simplex:	this.ps.Duplex = System.Drawing.Printing.Duplex.Simplex;	break;
					case DuplexMode.Vertical:	this.ps.Duplex = System.Drawing.Printing.Duplex.Vertical;	break;
				}
			}
		}
		
		
		public bool								Collate
		{
			get
			{
				return this.ps.Collate;
			}
			set
			{
				this.ps.Collate = value;
			}
		}
		
		public int								Copies
		{
			get
			{
				return this.ps.Copies;
			}
			set
			{
				if ((value > 1000) ||
					(value < 1))
				{
					throw new System.ArgumentOutOfRangeException ("value", value, "Copies count is invalid.");
				}
				
				this.ps.Copies = (short) value;
			}
		}
		
		public int								MaximumCopies
		{
			get
			{
				return this.ps.MaximumCopies;
			}
		}
		
		
		public int								FromPage
		{
			get
			{
				return this.ps.FromPage;
			}
			set
			{
				this.ps.FromPage = value;
			}
		}
		
		public int								ToPage
		{
			get
			{
				return this.ps.ToPage;
			}
			set
			{
				this.ps.ToPage = value;
			}
		}
		
		public PrintRange						PrintRange
		{
			get
			{
				switch (this.ps.PrintRange)
				{
					//	TODO: case...
				}
			}
		}
		
		public int								MaximumPage
		{
			get
			{
				return this.ps.MaximumPage;
			}
			set
			{
				this.ps.MaximumPage = value;
			}
		}
		
		public int								MinimumPage
		{
			get
			{
				return this.ps.MinimumPage;
			}
			set
			{
				this.ps.MinimumPage = value;
			}
		}
		
		
		public PageSettings						DefaultPageSettings
		{
			get
			{
				return new PageSettings (this.ps.DefaultPageSettings);
			}
		}
		
		public bool								IsDefaultPrinter
		{
			get
			{
				return this.ps.IsDefaultPrinter;
			}
		}
		
		public bool								IsValid
		{
			get
			{
				return this.ps.IsValid;
			}
		}
		
		public int								LandscapeAngle
		{
			get
			{
				return this.ps.LandscapeAngle;
			}
		}
		
		
		public PaperSize[]						PaperSizes
		{
			get
			{
				System.Drawing.Printing.PrinterSettings.PaperSizeCollection list = this.ps.PaperSizes;
				PaperSize[] values = new PaperSize[list.Count];
				
				for (int i = 0; i < list.Count; i++)
				{
					values[i] = new PaperSize (list[i]);
				}
				
				return values;
			}
		}
		
		public PaperSource[]					PaperSources
		{
			get
			{
				System.Drawing.Printing.PrinterSettings.PaperSourceCollection list = this.ps.PaperSources;
				PaperSource[] values = new PaperSource[list.Count];
				
				for (int i = 0; i < list.Count; i++)
				{
					values[i] = new PaperSource (list[i]);
				}
				
				return values;
			}
		}
		
		public PrinterResolution[]				PrinterResolutions
		{
			get
			{
				System.Drawing.Printing.PrinterSettings.PrinterResolutionCollection list = this.ps.PrinterResolutions;
				PrinterResolution[] values = new PrinterResolution[list.Count];
				
				for (int i = 0; i < list.Count; i++)
				{
					values[i] = new PrinterResolution (list[i]);
				}
				
				return values;
			}
		}
		
		
		public string							PrinterName
		{
			get
			{
				return this.ps.PrinterName;
			}
		}
		
		public bool								PrintToFile
		{
			get
			{
				return this.ps.PrintToFile;
			}
		}
		
		public bool								SupportsColor
		{
			get
			{
				return this.ps.SupportsColor;
			}
		}
		
		
		public static string[]					InstalledPrinters
		{
			get
			{
				System.Drawing.Printing.PrinterSettings.StringCollection list = System.Drawing.Printing.PrinterSettings.InstalledPrinters;
				string[] values = new string[list.Count];
				
				for (int i = 0; i < list.Count; i++)
				{
					values[i] = list[i];
				}
				
				return values;
			}
		}
		
		
		public static PrinterSettings FindPrinter(string name)
		{
			System.Drawing.Printing.PrinterSettings	ps = new System.Drawing.Printing.PrinterSettings ();
			
			ps.PrinterName = name;
			
			if (ps.IsValid)
			{
				return new PrinterSettings (ps);
			}
			
			return null;
		}
		
		
		System.Drawing.Printing.PrinterSettings	ps;
	}
}
