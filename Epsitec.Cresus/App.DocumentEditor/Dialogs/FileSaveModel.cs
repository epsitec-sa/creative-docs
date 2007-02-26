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
	public class FileSaveModel : AbstractFileSave
	{
		public FileSaveModel(DocumentEditor editor) : base(editor)
		{
			this.FileExtension = ".crmod";
			this.isModel = true;
			this.enableNavigation = true;
			this.enableMultipleSelection = false;
			this.displayNewEmtpyDocument = false;
			this.isSave = true;
		}

		public override void Show()
		{
			//	Cr�e et montre la fen�tre du dialogue.
			if ( this.window == null )
			{
				this.CreateAll ("FileSaveModel", new Size (720, 480), Res.Strings.Dialog.Save.TitleMod, 50);
			}

			this.UpdateAll(-1, true);
			this.window.ShowDialog();  // montre le dialogue modal...
		}

		public override void Save()
		{
			//	Enregistre la position de la fen�tre du dialogue.
			this.WindowSave("FileSaveModel");
		}
	}
}
