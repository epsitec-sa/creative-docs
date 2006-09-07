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
	/// Dialogue pour créer un nouveau document.
	/// </summary>
	public class FileNew : AbstractFile
	{
		public FileNew(DocumentEditor editor) : base(editor)
		{
			this.fileExtension = ".crmod";
			this.isModel = true;
			this.isNavigationEnabled = true;
			this.isMultipleSelection = false;
			this.isNewEmtpyDocument = true;
			this.isSave = false;
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.CreateAll("FileNew", new Size(400, 379), Res.Strings.Dialog.New.Title, 50);
			}

			this.UpdateAll(0, false);  // sélectionne 'Document vide'
			this.window.ShowDialog();  // montre le dialogue modal...
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("FileNew");
		}
	}
}
