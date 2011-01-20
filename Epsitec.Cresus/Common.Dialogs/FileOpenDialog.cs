//	Copyright © 2004-2009, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Reflection;
using System.Runtime.InteropServices;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// La classe FileOpenDialog présente le dialogue pour ouvrir un fichier.
	/// </summary>
	public class FileOpenDialog : Helpers.IFilterCollectionHost
	{
		public FileOpenDialog()
		{
			this.dialog = new System.Windows.Forms.OpenFileDialog ();
			this.filters = new Helpers.FilterCollection (this);

			this.dialog.AutoUpgradeEnabled = true;
			this.dialog.DereferenceLinks = true;
			this.dialog.AddExtension = true;
			this.dialog.CheckFileExists = true;
			this.dialog.CheckPathExists = true;
			this.dialog.DereferenceLinks = true;
			this.dialog.RestoreDirectory = true;
			this.dialog.ShowHelp = false;
			this.dialog.ShowReadOnly = false;
			this.dialog.ValidateNames = true;
			
			this.dialog.FileOk +=
				(sender, e) =>
				{
					string name = this.FileName;
					string path = System.IO.Path.GetDirectoryName (name);
					string ext  = System.IO.Path.GetExtension (name);

					if ((this.dialog.AddExtension) &&
						(this.filters.FindExtension (ext) == null))
					{
						if (this.dialog.ShowHelp)
						{
							//	See FileSaveDialog.
							var type = typeof (System.Windows.Forms.FileDialog);
							var info = type.GetField ("dialogHWnd", BindingFlags.NonPublic | BindingFlags.Instance);
							var fileDialogHandle = (System.IntPtr) info.GetValue (dialog);

							var fixedName = System.IO.Path.Combine (path, System.IO.Path.GetFileNameWithoutExtension (name) + "." + this.DefaultExt);

							FileOpenDialog.SetFileName (fileDialogHandle, fixedName);
							e.Cancel = true;
						}
						if (ext.ToLowerInvariant () == ".url")
                        {
							e.Cancel = true;
                        }
					}
				};
		}

#if true
		[DllImport ("User32")]
		private static extern int SetDlgItemText(System.IntPtr hwnd, int id, string title);

		private const int FileTitleCntrlID = 0x47c;

		private static void SetFileName(System.IntPtr hdlg, string name)
		{
			FileOpenDialog.SetDlgItemText (hdlg, FileTitleCntrlID, name);
		}
#endif
		
		
		#region IDialog Members
		public void OpenDialog()
		{
			this.dialog.Filter = this.Filters.FileDialogFilter;
			this.dialog.FilterIndex = this.filterIndex + 1;
			
			System.Windows.Forms.DialogResult result = this.dialog.ShowDialog (this.owner == null ? null : this.owner.PlatformWindowObject);
			
			switch (result)
			{
				case System.Windows.Forms.DialogResult.OK:
				case System.Windows.Forms.DialogResult.Yes:
					this.result       = DialogResult.Accept;
					this.filterIndex = this.dialog.FilterIndex - 1;
					break;
				
				default:
					this.result = DialogResult.Cancel;
					break;
			}
		}
		
		public Common.Widgets.Window	OwnerWindow
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
		
		public string							DefaultExt
		{
			get { return this.dialog.DefaultExt; }
			set { this.dialog.DefaultExt = value; }
		}
		
		public string							FileName
		{
			get { return this.dialog.FileName; }
			set { this.dialog.FileName = value; }
		}
		
		public string[]							FileNames
		{
			get { return this.dialog.FileNames; }
		}
		
		public Helpers.FilterCollection			Filters
		{
			get { return this.filters; }
		}
		
		public int								FilterIndex
		{
			get { return this.filterIndex; }
			set { this.filterIndex = value; }
		}
		
		public string							InitialDirectory
		{
			get { return this.dialog.InitialDirectory; }
			set { this.dialog.InitialDirectory = value; }
		}
		
		public bool								AcceptMultipleSelection
		{
			get { return this.dialog.Multiselect; }
			set { this.dialog.Multiselect = value; }
		}
		
		public string							Title
		{
			get { return this.dialog.Title; }
			set { this.dialog.Title = value; }
		}
		
		public DialogResult						Result
		{
			get
			{
				return this.result;
			}
		}
		
		
		#region IFilterCollectionHost Members
		public void FilterCollectionChanged()
		{
			//	Rien de spécial à faire...
		}
		#endregion
		
		
		Common.Widgets.Window					owner;
		System.Windows.Forms.OpenFileDialog		dialog;
		Helpers.FilterCollection				filters;
		int										filterIndex;
		private DialogResult					result = DialogResult.None;
	}
}
