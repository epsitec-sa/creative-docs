//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 20/03/2004

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// La classe Print présente le dialogue pour imprimer un document.
	/// </summary>
	public class Print
	{
		public Print()
		{
			this.dialog   = new System.Windows.Forms.PrintDialog ();
			
			this.dialog.AllowPrintToFile = true;
			this.dialog.AllowSelection   = true;
			this.dialog.AllowSomePages   = true;
			this.dialog.PrintToFile      = false;
			this.dialog.ShowHelp         = false;
			this.dialog.ShowNetwork      = true;
			
			this.Document = new Printing.PrintDocument ();
		}
		
		
		public bool								AllowPrintToFile
		{
			get
			{
				return this.dialog.AllowPrintToFile;
			}
			set
			{
				this.dialog.AllowPrintToFile = value;
			}
		}
		
		public bool								AllowFromPageToPage
		{
			get
			{
				return this.dialog.AllowSomePages;
			}
			set
			{
				this.dialog.AllowSomePages = value;
			}
		}
		
		public bool								AllowSelectedPages
		{
			get
			{
				return this.dialog.AllowSelection;
			}
			set
			{
				this.dialog.AllowSelection = value;
			}
		}
		
		
		public bool								PrintToFile
		{
			get
			{
				return this.dialog.PrintToFile;
			}
			set
			{
				this.dialog.PrintToFile = value;
			}
		}
		
		
		public Printing.PrintDocument			Document
		{
			get
			{
				return this.document;
			}
			set
			{
				if (this.document != value)
				{
					if (this.document != null)
					{
						this.Detach (this.document);
					}
					
					this.document = value;
					
					if (this.document != null)
					{
						this.Attach (this.document);
					}
				}
			}
		}
		
		
		#region IDialog Members
		public void Show()
		{
			this.dialog.ShowDialog (this.owner == null ? null : this.owner.PlatformWindowObject as System.Windows.Forms.IWin32Window);
		}
		
		public Common.Widgets.Window	Owner
		{
			get
			{
				return this.owner;
			}
			set
			{
				this.owner = value;
			}
		}
		#endregion
		
		protected virtual void Attach(Printing.PrintDocument document)
		{
			this.dialog.Document        = document.Object as System.Drawing.Printing.PrintDocument;
			this.dialog.PrinterSettings = document.PrinterSettings.Object as System.Drawing.Printing.PrinterSettings;
			
			document.PrinterChanged += new Support.EventHandler (this.HandleDocumentPrinterChanged);
		}
		
		protected virtual void Detach(Printing.PrintDocument document)
		{
			this.dialog.Document = null;
			this.dialog.PrinterSettings = null;
			
			document.PrinterChanged -= new Support.EventHandler (this.HandleDocumentPrinterChanged);
		}
		
		
		private void HandleDocumentPrinterChanged(object sender)
		{
			this.dialog.PrinterSettings = this.document.PrinterSettings.Object as System.Drawing.Printing.PrinterSettings;
		}
		
		
		Common.Widgets.Window					owner;
		System.Windows.Forms.PrintDialog		dialog;
		private Printing.PrintDocument			document;
	}
}
