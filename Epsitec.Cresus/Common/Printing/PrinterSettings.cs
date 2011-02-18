//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

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
					case System.Drawing.Printing.PrintRange.AllPages:	return PrintRange.AllPages;
					case System.Drawing.Printing.PrintRange.Selection:	return PrintRange.SelectedPages;
					case System.Drawing.Printing.PrintRange.SomePages:	return PrintRange.FromPageToPage;
				}
				
				return PrintRange.AllPages;
			}
			set
			{
				switch (value)
				{
					case PrintRange.AllPages:		this.ps.PrintRange = System.Drawing.Printing.PrintRange.AllPages;	break;
					case PrintRange.SelectedPages:	this.ps.PrintRange = System.Drawing.Printing.PrintRange.Selection;	break;
					case PrintRange.FromPageToPage:	this.ps.PrintRange = System.Drawing.Printing.PrintRange.SomePages;	break;
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
		
		
		public string							OutputFileName
		{
			get
			{
				System.Type type = typeof (System.Drawing.Printing.PrinterSettings);
				System.Reflection.FieldInfo info = type.GetField ("outputPort", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				return info.GetValue (this.ps) as string;
			}
			set
			{
				System.Type type = typeof (System.Drawing.Printing.PrinterSettings);
				System.Reflection.FieldInfo info = type.GetField ("outputPort", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				info.SetValue (this.ps, value);
			}
		}
		
		public string							OutputPort
		{
			get
			{
				System.Type                    type = typeof (System.Drawing.Printing.PrinterSettings);
				System.Reflection.PropertyInfo info = type.GetProperty ("OutputPort", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				return info.GetValue (this.ps, null) as string;
			}
			set
			{
				System.Type                    type = typeof (System.Drawing.Printing.PrinterSettings);
				System.Reflection.PropertyInfo info = type.GetProperty ("OutputPort", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				info.SetValue (this.ps, value, null);
			}
		}
		
		public string							DriverName
		{
			get
			{
				System.Type                    type = typeof (System.Drawing.Printing.PrinterSettings);
				System.Reflection.PropertyInfo info = type.GetProperty ("DriverName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				return info.GetValue (this.ps, null) as string;
			}
			set
			{
				System.Type                    type = typeof (System.Drawing.Printing.PrinterSettings);
				System.Reflection.PropertyInfo info = type.GetProperty ("DriverName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				info.SetValue (this.ps, value, null);
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
				int n = 0;
				
				for (int i = 0; i < list.Count; i++)
				{
					if ((list[i].X > 0) && (list[i].Y > 0))
					{
						n++;
					}
				}
				
				PrinterResolution[] values = new PrinterResolution[n];
				
				n = 0;
				
				for (int i = 0; i < list.Count; i++)
				{
					if ((list[i].X > 0) && (list[i].Y > 0))
					{
						values[n] = new PrinterResolution (list[i]);
						n++;
					}
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
			set
			{
				this.ps.PrintToFile = value;
			}
		}
		
		public bool								SupportsColor
		{
			get
			{
				return this.ps.SupportsColor;
			}
		}
		
		
		public object							Object
		{
			get
			{
				return this.ps;
			}
		}
		
		
		public System.IntPtr GetDevMode()
		{
			try
			{
				return this.ps.GetHdevmode ();
			}
			catch
			{
				return System.IntPtr.Zero;
			}
		}
		
		public bool SetDevMode(System.IntPtr devMode)
		{
			try
			{
				this.ps.SetHdevmode (devMode);
				return true;
			}
			catch
			{
				return false;
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
