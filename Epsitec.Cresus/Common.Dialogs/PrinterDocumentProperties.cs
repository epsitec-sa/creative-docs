//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// La classe PrinterDocumentProperties présente le dialogue pour choisir
	/// les préférences de l'imprimante (ce sont les réglages avancés propres
	/// à l'imprimante et généralement pas accessibles via programmation).
	/// </summary>
	public class PrinterDocumentProperties : Print
	{
		public PrinterDocumentProperties()
		{
		}
		
		public override void OpenDialog()
		{
			System.Windows.Forms.IWin32Window owner  = this.owner == null ? null : this.owner.PlatformWindowObject as System.Windows.Forms.IWin32Window;
			System.Windows.Forms.DialogResult result = this.InternalShowDialog (owner);
			
			switch (result)
			{
				case System.Windows.Forms.DialogResult.OK:
				case System.Windows.Forms.DialogResult.Yes:
					this.result = DialogResult.Accept;
					break;
				
				default:
					this.result = DialogResult.Cancel;
					break;
			}
		}
		
		
		#region Native Win32 Imports
		[System.Runtime.InteropServices.DllImport("winspool.drv", CharSet=System.Runtime.InteropServices.CharSet.Unicode)] static extern int DocumentProperties(System.IntPtr window_handle, System.IntPtr printer_handle, string device_name, System.IntPtr dev_mode_out, System.IntPtr dev_mode_in, int mode);
		[System.Runtime.InteropServices.DllImport("winspool.drv", CharSet=System.Runtime.InteropServices.CharSet.Unicode, SetLastError=true)] static extern bool OpenPrinter(string printer_name, out System.IntPtr printer_handle, System.IntPtr default_ptr);
		[System.Runtime.InteropServices.DllImport("winspool.drv", CharSet=System.Runtime.InteropServices.CharSet.Unicode, SetLastError=true)] static extern bool ClosePrinter(System.IntPtr printer_handle);
		
		[System.Runtime.InteropServices.DllImport("kernel32.dll")] static extern System.IntPtr GlobalFree(System.IntPtr handle);
		[System.Runtime.InteropServices.DllImport("kernel32.dll")] static extern System.IntPtr GlobalLock(System.IntPtr handle);
		[System.Runtime.InteropServices.DllImport("kernel32.dll")] static extern bool GlobalUnlock(System.IntPtr handle);
		#endregion
		
		private System.Windows.Forms.DialogResult InternalShowDialog(System.Windows.Forms.IWin32Window owner)
		{
			//	Pour pouvoir travailler, nous avons besoins des pointeurs vers les structures
			//	internes DEVMODE; .NET permet heureusement d'obtenir un HDEVMODE et c'est facile
			//	ensuite d'obtenir la mémoire en faisant un "GlobalLock".
			
			System.IntPtr dev_mode_handle  = this.document.PrinterSettings.GetDevMode ();
			System.IntPtr dev_mode_ptr     = PrinterDocumentProperties.GlobalLock (dev_mode_handle);
			System.IntPtr printer_handle   = System.IntPtr.Zero;
			System.IntPtr printer_defaults = System.IntPtr.Zero;
			
			try
			{
				string printer_name = this.dialog.PrinterSettings.PrinterName;
				string device_name  = printer_name;
				
				//	Obtient un handle de l'imprimante et affiche le dialogue des réglages
				//	avancés (via winspool.drv), puis copie les réglages du DEVMODE vers
				//	les classes de support .NET :
				
				if (PrinterDocumentProperties.OpenPrinter (printer_name, out printer_handle, printer_defaults))
				{
					int result = PrinterDocumentProperties.DocumentProperties (owner == null ? System.IntPtr.Zero : owner.Handle, printer_handle, device_name, dev_mode_ptr, dev_mode_ptr, 0x0e);
					this.document.PrinterSettings.SetDevMode (dev_mode_handle);
					PrinterDocumentProperties.ClosePrinter (printer_handle);
					return (System.Windows.Forms.DialogResult) result;
				}
			}
			finally
			{
				//	Dans tous les cas, ne pas oublier de libérer la mémoire globale associée
				//	au HDEVMODE :

				PrinterDocumentProperties.GlobalUnlock (dev_mode_handle);
				PrinterDocumentProperties.GlobalFree (dev_mode_handle);
			}
			
			return System.Windows.Forms.DialogResult.Abort;
		}
	}
}
