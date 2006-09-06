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
	public class FileSaveModel : AbstractFile
	{
		public FileSaveModel(DocumentEditor editor) : base(editor)
		{
			this.fileExtension = ".crmod";
			this.isNavigationEnabled = true;
			this.isMultipleSelection = false;
			this.isNewEmtpyDocument = false;
			this.isSave = true;
		}

		public override void Show()
		{
			//	Cr�e et montre la fen�tre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeSecondaryWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("FileSaveModel", 400, 379, true);
				this.window.Text = Res.Strings.Dialog.Save.TitleMod;
				this.window.Owner = this.editor.Window;
				this.window.Icon = Bitmap.FromManifestResource("Epsitec.App.DocumentEditor.Images.Application.icon", this.GetType().Assembly);
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.MinSize = new Size(300, 200);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				this.CreateResizer();
				this.CreateAccess();
				this.CreateTable(50);
				this.CreateRename();
				this.CreateFooter();
				this.CreateFilename();
			}

			this.selectedFilename = null;
			this.selectedFilenames = null;
			this.UpdateTable(-1);
			this.UpdateInitialDirectory();
			this.UpdateInitialFilename();

			this.fieldFilename.SelectAll();
			this.fieldFilename.Focus();  // focus pour frapper le nom du fichier � ouvrir

			this.window.ShowDialog();
		}

		public override void Save()
		{
			//	Enregistre la position de la fen�tre du dialogue.
			this.WindowSave("FileSaveModel");
		}
	}
}
