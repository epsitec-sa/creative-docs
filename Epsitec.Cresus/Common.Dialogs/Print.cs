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
			this.document = new System.Drawing.Printing.PrintDocument ();
			
			this.dialog.AllowPrintToFile = true;
			this.dialog.AllowSelection   = true;
			this.dialog.AllowSomePages   = true;
			this.dialog.PrintToFile      = false;
			this.dialog.ShowHelp         = false;
			this.dialog.ShowNetwork      = true;
			this.dialog.Document         = this.document;
		}
		
		public void Show()
		{
			this.dialog.ShowDialog ();
		}
		
		
		System.Windows.Forms.PrintDialog		dialog;
		System.Drawing.Printing.PrintDocument	document;
	}
}
