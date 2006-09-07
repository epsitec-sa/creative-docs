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
	public class FileOpenModel : AbstractFile
	{
		public FileOpenModel(DocumentEditor editor) : base(editor)
		{
			this.fileExtension = ".crmod";
			this.isModel = true;
			this.isNavigationEnabled = true;
			this.isMultipleSelection = true;
			this.isNewEmtpyDocument = false;
			this.isSave = false;
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.CreateAll("FileOpenModel", new Size(400, 379), Res.Strings.Dialog.Open.TitleMod, 50);
			}

			this.UpdateAll(-1, true);
			this.window.ShowDialog();  // montre le dialogue modal...
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("FileOpenModel");
		}
	}
}
