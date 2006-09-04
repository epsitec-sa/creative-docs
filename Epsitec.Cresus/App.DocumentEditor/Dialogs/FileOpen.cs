using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;
using Epsitec.Common.IO;
using System.IO;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	/// <summary>
	/// Dialogue pour ouvrir un document existant.
	/// </summary>
	public class FileOpen : AbstractFile
	{
		public FileOpen(DocumentEditor editor) : base(editor)
		{
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeSecondaryWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("FileOpen", 400, 300, true);
				this.window.Text = Res.Strings.Dialog.Open.TitleDoc;
				this.window.Owner = this.editor.Window;
				this.window.Icon = Bitmap.FromManifestResource("Epsitec.App.DocumentEditor.Images.Application.icon", this.GetType().Assembly);
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.MinSize = new Size(300, 200);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				this.CreateResizer();

				//	Chemin d'accès.
				Widget access = new Widget(this.window.Root);
				access.PreferredHeight = 20;
				access.Margins = new Margins(0, 0, 0, 8);
				access.Dock = DockStyle.Top;

				StaticText label = new StaticText(access);
				label.Text = Res.Strings.Dialog.Open.LabelPath;
				label.PreferredWidth = 80;
				label.Dock = DockStyle.Left;

				this.fieldPath = new TextFieldCombo(access);
				this.fieldPath.IsReadOnly = true;
				this.fieldPath.Dock = DockStyle.Fill;
				this.fieldPath.ComboOpening += new EventHandler<CancelEventArgs>(this.HandleFieldPathComboOpening);

				IconButton buttonNew = new IconButton(access);
				buttonNew.IconName = Misc.Icon("NewDirectory");
				buttonNew.Dock = DockStyle.Right;
				buttonNew.Margins = new Margins(0, 0, 0, 0);
				buttonNew.Clicked += new MessageEventHandler(this.HandleButtonNewClicked);
				ToolTip.Default.SetToolTip(buttonNew, "Nouveau dossier");

				IconButton buttonParent = new IconButton(access);
				buttonParent.IconName = Misc.Icon("ParentDirectory");
				buttonParent.Dock = DockStyle.Right;
				buttonParent.Margins = new Margins(5, 0, 0, 0);
				buttonParent.Clicked += new MessageEventHandler(this.HandleButtonParentClicked);
				ToolTip.Default.SetToolTip(buttonParent, Res.Strings.Dialog.Open.ParentDirectory);

				//	Liste centrale principale.
				this.CreateTable();

				//	Boutons en bas.
				this.CreateFooter();

				//	Nom du fichier.
				Widget file = new Widget(this.window.Root);
				file.PreferredHeight = 20;
				file.Margins = new Margins(0, 0, 8, 0);
				file.Dock = DockStyle.Bottom;

				label = new StaticText(file);
				label.Text = Res.Strings.Dialog.Open.LabelDoc;
				label.PreferredWidth = 80;
				label.Dock = DockStyle.Left;

				this.fieldFilename = new TextField(file);
				this.fieldFilename.Dock = DockStyle.Fill;
			}

			this.selectedFilename = null;
			this.UpdateTable(-1);

			this.fieldPath.Text = this.initialDirectory;

			this.fieldFilename.Text = "";
			this.fieldFilename.Focus();

			this.window.ShowDialog();
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("FileOpen");
		}


		public override string InitialDirectory
		{
			get
			{
				return this.initialDirectory;
			}
			set
			{
				if (this.initialDirectory != value)
				{
					this.initialDirectory = value;

					if (this.fieldPath != null)
					{
						this.fieldPath.Text = this.initialDirectory;
					}
				}
			}
		}

		protected override bool IsNavigationEnabled
		{
			get
			{
				return true;
			}
		}

		protected override string Extension
		{
			get
			{
				return ".crdoc";
			}
		}

		protected override string SelectedFilename
		{
			get
			{
				string path = System.IO.Path.GetDirectoryName(this.globalSettings.NewDocument);
				string filename = string.Concat(path, "\\", this.fieldFilename.Text, this.Extension);
				return filename;
			}
		}


		protected void ParentDirectory()
		{
			//	Remonte dans le dossier parent.
			int index = this.initialDirectory.LastIndexOf("\\");
			if (index != -1)
			{
				this.InitialDirectory = this.initialDirectory.Substring(0, index);
				this.UpdateTable(-1);
			}
		}

		protected void NewDirectory()
		{
			//	Crée un nouveau dossier vide.
			string newDir = this.NewDirectoryName;
			if (newDir == null)
			{
				return;
			}

			try
			{
				System.IO.Directory.CreateDirectory(newDir);
			}
			catch
			{
				return;
			}

			this.UpdateTable(-1);

			for (int i=0; i<this.files.Count; i++)
			{
				Item item = this.files[i];
				if (item.Filename == newDir)
				{
					this.table.SelectRow(i, true);
					this.table.ShowSelect();
					break;
				}
			}
		}

		protected string NewDirectoryName
		{
			//	Retourne le nom à utiliser pour le nouveau dossier à créer.
			get
			{
				for (int i=1; i<100; i++)
				{
					string newDir = string.Concat(this.initialDirectory, "\\Nouveau dossier");
					if (i > 1)
					{
						newDir = string.Concat(newDir, " (", i.ToString(), ")");
					}

					bool exist = false;
					foreach (Item item in this.files)
					{
						if (item.IsDirectory && item.Filename == newDir)
						{
							exist = true;
							break;
						}
					}

					if (!exist)
					{
						return newDir;
					}
				}

				return null;
			}
		}


		private void HandleFieldPathComboOpening(object sender, CancelEventArgs e)
		{
			//	Le menu pour le chamin d'accès va être ouvert.
			string[] drivers = System.IO.Directory.GetLogicalDrives();

			this.fieldPath.Items.Clear();

			foreach (string driver in drivers)
			{
				this.fieldPath.Items.Add(driver);
			}
		}

		private void HandleButtonParentClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton 'dossier parent' a été cliqué.
			this.ParentDirectory();
		}

		private void HandleButtonNewClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton 'nouveau dossier' a été cliqué.
			this.NewDirectory();
		}

		protected override void HandleTableFinalSelectionChanged(object sender)
		{
			//	Sélection changée dans la liste.
			int sel = this.table.SelectedRow;
			if (sel == -1)
			{
				this.fieldFilename.Text = "";
			}
			else
			{
				if (this.files[sel].IsDirectory)
				{
					this.fieldFilename.Text = "";
				}
				else
				{
					this.fieldFilename.Text = this.files[sel].ShortFilename;
				}
			}
		}


		protected TextFieldCombo				fieldPath;
		protected TextField						fieldFilename;
	}
}
