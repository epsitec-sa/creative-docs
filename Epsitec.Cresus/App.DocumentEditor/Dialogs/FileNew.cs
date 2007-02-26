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
	/// Dialogue pour cr�er un nouveau document.
	/// </summary>
	public class FileNew : AbstractFileDialog
	{
		public FileNew(DocumentEditor editor) : base(editor)
		{
			this.FileExtension = ".crmod";
			this.isModel = true;
			this.enableNavigation = true;
			this.enableMultipleSelection = false;
			this.isNewEmtpyDocument = true;
			this.isSave = false;
		}

		public override void Show()
		{
			//	Cr�e et montre la fen�tre du dialogue.
			if ( this.window == null )
			{
				this.CreateAll ("FileNew", new Size (720, 480), Res.Strings.Dialog.New.Title, 50);
			}

			this.UpdateAll(0, false);  // s�lectionne 'Document vide'
			this.window.ShowDialog();  // montre le dialogue modal...
		}

		public override void Save()
		{
			//	Enregistre la position de la fen�tre du dialogue.
			this.WindowSave("FileNew");
		}
	}
}
