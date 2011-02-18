//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Printing
{
	/// <summary>
	/// La classe PrintDocument représente les réglages d'un "job" d'impression.
	/// </summary>
	public class PrintDocument
	{
		public PrintDocument()
		{
			this.pd = new System.Drawing.Printing.PrintDocument ();
			
			this.pd.BeginPrint        += this.HandleBeginPrint;
			this.pd.EndPrint          += this.HandleEndPrint;
			this.pd.PrintPage         += this.HandlePrintPage;
			this.pd.QueryPageSettings += this.HandleQueryPageSettings;
			
			this.pd.OriginAtMargins = true;
		}
		
		
		public string							DocumentName
		{
			get
			{
				return this.pd.DocumentName;
			}
			set
			{
				this.pd.DocumentName = value;
			}
		}
		
		public PrinterSettings					PrinterSettings
		{
			get
			{
				return new PrinterSettings (this.pd.PrinterSettings);
			}
		}
		
		
		public Printing.PageSettings			DefaultPageSettings
		{
			get
			{
				return new Printing.PageSettings (this.pd.DefaultPageSettings);
			}
		}
		
		public bool								OriginAtMargins
		{
			get
			{
				return this.pd.OriginAtMargins;
			}
			set
			{
				this.pd.OriginAtMargins = value;
			}
		}
		
		
		public object							Object
		{
			get
			{
				return this.pd;
			}
		}
		
		
		public bool SelectPrinter(string name)
		{
			PrinterSettings ps = PrinterSettings.FindPrinter (name);
			
			if (ps != null)
			{
				this.pd.PrinterSettings = ps.Object as System.Drawing.Printing.PrinterSettings;
				this.OnPrinterChanged ();
				return true;
			}
			
			return false;
		}
		
		public bool Print(IPrintEngine engine)
		{
			if (engine == null)
			{
				return false;
			}
			
			bool ok = false;
			
			try
			{
				this.engine = engine;
				this.status = PrintEngineStatus.MorePages;
			
				this.pd.Print ();
			
				ok = (this.status == PrintEngineStatus.FinishJob);
			}
			catch (System.ComponentModel.Win32Exception)
			{
				ok = false;
			}
			finally
			{
				this.engine = null;
			}
			
			return ok;
		}
		
		
		protected virtual void OnPrinterChanged()
		{
			if (this.PrinterChanged != null)
			{
				this.PrinterChanged (this);
			}
		}
		
		
		private void HandleBeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			this.engine.StartingPrintJob ();
		}
		
		private void HandleEndPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			this.engine.FinishingPrintJob ();
		}
		
		private void HandlePrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			System.Drawing.Graphics graphics = e.Graphics;
			PageSettings            settings = new PageSettings (e.PageSettings);
			PrintPort               port     = new PrintPort (graphics, settings, e);
			PrintEngineStatus       status   = this.engine.PrintPage (port);
			
			switch (status)
			{
				case PrintEngineStatus.CancelJob:
					e.Cancel = true;
					break;
				
				case PrintEngineStatus.FinishJob:
					e.HasMorePages = false;
					break;
				
				case PrintEngineStatus.MorePages:
					e.HasMorePages = true;
					break;
			}
			
			this.status = status;
		}
		
		private void HandleQueryPageSettings(object sender, System.Drawing.Printing.QueryPageSettingsEventArgs e)
		{
			PageSettings settings = new PageSettings (e.PageSettings);
			this.engine.PrepareNewPage (settings);
		}
		
		
		public event Support.EventHandler		PrinterChanged;
		
		
		System.Drawing.Printing.PrintDocument	pd;
		private IPrintEngine					engine;
		private PrintEngineStatus				status;
	}
}
