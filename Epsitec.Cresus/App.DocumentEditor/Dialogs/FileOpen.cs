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
				this.window.Text = Res.Strings.Dialog.New.Title;
				this.window.Owner = this.editor.Window;
				this.window.Icon = Bitmap.FromManifestResource("Epsitec.App.DocumentEditor.Images.Application.icon", this.GetType().Assembly);
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.MinSize = new Size(300, 200);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				this.CreateResizer();

				Widget header = new Widget(this.window.Root);
				header.PreferredHeight = 20;
				header.Margins = new Margins(0, 0, 0, 8);
				header.Dock = DockStyle.Top;

				StaticText label = new StaticText(header);
				label.Text = "Document";
				label.PreferredWidth = 70;
				label.Dock = DockStyle.Left;

				this.fieldFilename = new TextField(header);
				this.fieldFilename.Dock = DockStyle.Fill;
				
				this.CreateTable();
				this.CreateFooter();
			}

			this.selectedFilename = null;
			this.UpdateTable(-1);

			this.fieldFilename.Text = "";
			this.fieldFilename.Focus();

			this.window.ShowDialog();
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("FileOpen");
		}


		protected override string FilenameFilter
		{
			get
			{
				return "*.crdoc";
			}
		}

		protected override string SelectedFilename
		{
			get
			{
				string path = System.IO.Path.GetDirectoryName(this.globalSettings.NewDocument);
				string filename = string.Concat(path, "\\", this.fieldFilename.Text, ".crdoc");
				return filename;
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
				this.fieldFilename.Text = this.files[sel].ShortFilename;
			}
		}


		protected TextField						fieldFilename;
	}
}
