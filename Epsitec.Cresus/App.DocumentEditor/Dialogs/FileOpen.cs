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
			this.FileExtension = ".crdoc";
			this.isModel = false;
			this.isNavigationEnabled = true;
			this.isMultipleSelection = true;
			this.isNewEmtpyDocument = false;
			this.isSave = false;
		}

		public override void Show()
		{
			//	Cr�e et montre la fen�tre du dialogue.
			if ( this.window == null )
			{
				this.CreateAll ("FileOpen", new Size (720, 480), Res.Strings.Dialog.Open.TitleDoc, 20);
			}

			this.UpdateAll(-1, true);
			this.window.ShowDialog();  // montre le dialogue modal...
		}

		public override void Save()
		{
			//	Enregistre la position de la fen�tre du dialogue.
			this.WindowSave("FileOpen");
		}
	}
}
