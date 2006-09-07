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
			this.isModel = true;
			this.isNavigationEnabled = true;
			this.isMultipleSelection = false;
			this.isNewEmtpyDocument = false;
			this.isSave = true;
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.CreateAll("FileSaveModel", new Size(510, 379), Res.Strings.Dialog.Save.TitleMod, 50);
			}

			this.UpdateAll(-1, true);
			this.window.ShowDialog();  // montre le dialogue modal...
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("FileSaveModel");
		}
	}
}
