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
	public class FileSave : AbstractFileSave
	{
		public FileSave(DocumentEditor editor) : base(editor)
		{
			this.FileExtension = ".crdoc";
			this.isModel = false;
			this.enableNavigation = true;
			this.enableMultipleSelection = false;
			this.isNewEmtpyDocument = false;
			this.isSave = true;
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.CreateAll ("FileSave", new Size (720, 480), Res.Strings.Dialog.Save.TitleDoc, 20);
			}

			this.UpdateAll(-1, true);
			this.window.ShowDialog();  // montre le dialogue modal...
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("FileSave");
		}
	}
}
