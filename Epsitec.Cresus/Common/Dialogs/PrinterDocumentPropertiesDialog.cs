//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// La classe PrinterDocumentProperties présente le dialogue pour choisir
	/// les préférences de l'imprimante (ce sont les réglages avancés propres
	/// à l'imprimante et généralement pas accessibles via programmation).
	/// </summary>
	public class PrinterDocumentPropertiesDialog : PrintDialog
	{
		public PrinterDocumentPropertiesDialog()
		{
		}
		
		public override void OpenDialog()
		{
			System.Windows.Forms.IWin32Window owner  = this.owner == null ? null : this.owner.PlatformWindowObject;
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
		[System.Runtime.InteropServices.DllImport("winspool.drv", CharSet=System.Runtime.InteropServices.CharSet.Unicode)] static extern int DocumentProperties(System.IntPtr windowHandle, System.IntPtr printeHandle, string deviceName, System.IntPtr devModeOut, System.IntPtr devModeIn, int mode);
		[System.Runtime.InteropServices.DllImport("winspool.drv", CharSet=System.Runtime.InteropServices.CharSet.Unicode, SetLastError=true)] static extern bool OpenPrinter(string printerName, out System.IntPtr printerHandle, System.IntPtr defaultPtr);
		[System.Runtime.InteropServices.DllImport("winspool.drv", CharSet=System.Runtime.InteropServices.CharSet.Unicode, SetLastError=true)] static extern bool ClosePrinter(System.IntPtr printerHandle);
		
		[System.Runtime.InteropServices.DllImport("kernel32.dll")] static extern System.IntPtr GlobalFree(System.IntPtr handle);
		[System.Runtime.InteropServices.DllImport("kernel32.dll")] static extern System.IntPtr GlobalLock(System.IntPtr handle);
		[System.Runtime.InteropServices.DllImport("kernel32.dll")] static extern bool GlobalUnlock(System.IntPtr handle);
		#endregion
		
		private System.Windows.Forms.DialogResult InternalShowDialog(System.Windows.Forms.IWin32Window owner)
		{
			//	Pour pouvoir travailler, nous avons besoins des pointeurs vers les structures
			//	internes DEVMODE; .NET permet heureusement d'obtenir un HDEVMODE et c'est facile
			//	ensuite d'obtenir la mémoire en faisant un "GlobalLock".
			
			System.IntPtr devModeHandle  = this.document.PrinterSettings.GetDevMode ();

			if (devModeHandle == System.IntPtr.Zero)
			{
				return System.Windows.Forms.DialogResult.Abort;
			}
			
			System.IntPtr devModePtr     = PrinterDocumentPropertiesDialog.GlobalLock (devModeHandle);
			System.IntPtr printerHandle   = System.IntPtr.Zero;
			System.IntPtr printerDefaults = System.IntPtr.Zero;
			
			try
			{
				string printerName = this.dialog.PrinterSettings.PrinterName;
				string deviceName  = printerName;
				
				//	Obtient un handle de l'imprimante et affiche le dialogue des réglages
				//	avancés (via winspool.drv), puis copie les réglages du DEVMODE vers
				//	les classes de support .NET :
				
				if (PrinterDocumentPropertiesDialog.OpenPrinter (printerName, out printerHandle, printerDefaults))
				{
					int result = PrinterDocumentPropertiesDialog.DocumentProperties (owner == null ? System.IntPtr.Zero : owner.Handle, printerHandle, deviceName, devModePtr, devModePtr, 0x0e);
					this.document.PrinterSettings.SetDevMode (devModeHandle);
					PrinterDocumentPropertiesDialog.ClosePrinter (printerHandle);
					return (System.Windows.Forms.DialogResult) result;
				}
			}
			finally
			{
				//	Dans tous les cas, ne pas oublier de libérer la mémoire globale associée
				//	au HDEVMODE :

				PrinterDocumentPropertiesDialog.GlobalUnlock (devModeHandle);
				PrinterDocumentPropertiesDialog.GlobalFree (devModeHandle);
			}
			
			return System.Windows.Forms.DialogResult.Abort;
		}
	}
}
