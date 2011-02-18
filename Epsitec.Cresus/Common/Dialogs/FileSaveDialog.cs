//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Reflection;
using System.Runtime.InteropServices;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// La classe FileSaveDialog présente le dialogue pour enregistrer un fichier.
	/// </summary>
	public class FileSaveDialog : Helpers.IFilterCollectionHost, IDialog
	{
		public FileSaveDialog()
		{
			this.dialog = new System.Windows.Forms.SaveFileDialog ();
			this.filters = new Helpers.FilterCollection (this);
			
			this.dialog.AddExtension = true;
			this.dialog.AutoUpgradeEnabled = true;
			this.dialog.CheckFileExists = false;
			this.dialog.CheckPathExists = true;
			this.dialog.CreatePrompt = false;
			this.dialog.DereferenceLinks = true;
			this.dialog.OverwritePrompt = false;
			this.dialog.RestoreDirectory = true;
			this.dialog.ShowHelp = false;
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
							//	Works only for dialogs which are XP compatible; to force that, use the
							//	ShowHelp = true setting.
							//	http://stackoverflow.com/questions/1599511/how-to-replace-filename-in-savefiledialog-fileok-event-handler

							var type = typeof (System.Windows.Forms.FileDialog);
							var info = type.GetField ("dialogHWnd", BindingFlags.NonPublic | BindingFlags.Instance);
							var fileDialogHandle = (System.IntPtr) info.GetValue (dialog);

							var fixedName = System.IO.Path.Combine (path, System.IO.Path.GetFileNameWithoutExtension (name) + "." + this.DefaultExt);

							FileSaveDialog.SetFileName (fileDialogHandle, fixedName);
							e.Cancel = true;
						}
						if (ext.ToLowerInvariant () == ".url")
						{
							e.Cancel = true;
						}
					}

					if (this.PromptForOverwriting)
					{
						if (System.IO.File.Exists (name))
                        {
							var title   = Res.Strings.Dialog.File.Message.SaveTitle.ToSimpleText ();
							var message = string.Format (Res.Strings.Dialog.File.Message.PromptForOverwriting.ToSimpleText (), System.IO.Path.GetFileName (name));
							var result  = System.Windows.Forms.MessageBox.Show (this.OwnerWindow == null ? null : this.OwnerWindow.PlatformWindowObject, message, title, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Exclamation, System.Windows.Forms.MessageBoxDefaultButton.Button2);
							
							if (result != System.Windows.Forms.DialogResult.Yes)
							{
								e.Cancel = true;
							}
                        }
					}
					if (this.PromptForCreation)
                    {
						if (!System.IO.File.Exists (name))
						{
							var title   = Res.Strings.Dialog.File.Message.SaveTitle.ToSimpleText ();
							var message = string.Format (Res.Strings.Dialog.File.Message.PromptForCreation.ToSimpleText (), System.IO.Path.GetFileName (name));
							var result  = System.Windows.Forms.MessageBox.Show (this.OwnerWindow == null ? null : this.OwnerWindow.PlatformWindowObject, message, title, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Exclamation, System.Windows.Forms.MessageBoxDefaultButton.Button1);

							if (result != System.Windows.Forms.DialogResult.Yes)
							{
								e.Cancel = true;
							}
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
			FileSaveDialog.SetDlgItemText (hdlg, FileTitleCntrlID, name);
		}
#endif
		
		
		public string							DefaultExt
		{
			get
			{
				return this.dialog.DefaultExt;
			}
			set
			{
				this.dialog.DefaultExt = value;
			}
		}

		public string							FileName
		{
			get
			{
				return this.FileNames[0];
			}
			set
			{
				this.dialog.FileName = value;
			}
		}
		
		public string[]							FileNames
		{
			get
			{
				return this.GetFixedFileNames ();
			}
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
		
		public bool								CheckFileExists
		{
			get { return this.dialog.CheckFileExists; }
			set { this.dialog.CheckFileExists = value; }
		}
		
		public bool								PromptForCreation
		{
			get;
			set;
		}
		
		public bool								PromptForOverwriting
		{
			get;
			set;
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
					this.result      = DialogResult.Accept;
					this.filterIndex = this.dialog.FilterIndex - 1;
					break;
				
				default:
					this.result = DialogResult.Cancel;
					break;
			}
		}

		private string[] GetFixedFileNames()
		{
			if (string.IsNullOrEmpty (this.DefaultExt))
            {
				return this.dialog.FileNames;
            }

			var names = this.dialog.FileNames;

			if (this.dialog.AddExtension)
			{
				for (int i = 0; i < names.Length; i++)
				{
					names[i] = this.FixFileName (names[i]);
				}
			}

			return names;
		}

		private string FixFileName(string name)
		{
			var fileExt  = System.IO.Path.GetExtension (name).ToLowerInvariant ();
			var forceExt = this.GetDefaultExtension ();

			if ((forceExt != null) &&
				(forceExt != fileExt))
			{
				return string.Concat (name, forceExt);
			}
			else
			{
				return name;
			}
		}

		private string GetDefaultExtension()
		{
			var ext = this.DefaultExt;
			
			if (string.IsNullOrEmpty (ext))
            {
				return null;
            }

			if (ext[0] != '.')
			{
				return string.Concat (".", ext.ToLowerInvariant ());
			}
			else
			{
				return ext.ToLowerInvariant ();
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
		
		#region IFilterCollectionHost Members
		public void FilterCollectionChanged()
		{
			//	Rien de spécial à faire...
		}
		#endregion
		
		
		Common.Widgets.Window					owner;
		System.Windows.Forms.SaveFileDialog 	dialog;
		Helpers.FilterCollection				filters;
		int										filterIndex;
		private DialogResult					result = DialogResult.None;
	}
}
