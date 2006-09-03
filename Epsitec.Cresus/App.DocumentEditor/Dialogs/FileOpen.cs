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

				//	Chamin d'accès.
				Widget header1 = new Widget(this.window.Root);
				header1.PreferredHeight = 20;
				header1.Margins = new Margins(0, 0, 0, 4);
				header1.Dock = DockStyle.Top;

				StaticText label = new StaticText(header1);
				label.Text = "Chemin";
				label.PreferredWidth = 70;
				label.Dock = DockStyle.Left;

				this.fieldPath = new TextField(header1);
				this.fieldPath.IsReadOnly = true;
				this.fieldPath.Dock = DockStyle.Fill;

				IconButton buttonParent = new IconButton(header1);
				buttonParent.IconName = Misc.Icon("ParentDirectory");
				buttonParent.Dock = DockStyle.Right;
				buttonParent.Margins = new Margins(5, 0, 0, 0);
				buttonParent.Clicked += new MessageEventHandler(this.HandleButtonParentClicked);
				ToolTip.Default.SetToolTip(buttonParent, "Dossier parent");

				//	Nom du fichier.
				Widget header2 = new Widget(this.window.Root);
				header2.PreferredHeight = 20;
				header2.Margins = new Margins(0, 0, 0, 8);
				header2.Dock = DockStyle.Top;

				label = new StaticText(header2);
				label.Text = Res.Strings.Dialog.Open.LabelDoc;
				label.PreferredWidth = 70;
				label.Dock = DockStyle.Left;

				this.fieldFilename = new TextField(header2);
				this.fieldFilename.Dock = DockStyle.Fill;
				this.fieldFilename.Margins = new Margins(0, 27, 0, 0);
				

				this.CreateTable();
				this.CreateFooter();
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


		private void HandleButtonParentClicked(object sender, MessageEventArgs e)
		{
			int index = this.initialDirectory.LastIndexOf("\\");
			if (index != -1)
			{
				this.InitialDirectory = this.initialDirectory.Substring(0, index);
				this.UpdateTable(-1);
			}
		}

		protected override void HandleTableFinalSelectionChanged(object sender)
		{
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


		protected TextField						fieldPath;
		protected TextField						fieldFilename;
	}
}
