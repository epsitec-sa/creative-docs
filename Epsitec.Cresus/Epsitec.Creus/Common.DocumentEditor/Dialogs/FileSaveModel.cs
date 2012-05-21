using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;
using Epsitec.Common.IO;
using System.IO;

namespace Epsitec.Common.DocumentEditor.Dialogs
{
	/// <summary>
	/// Dialogue pour ouvrir un document existant.
	/// </summary>
	public class FileSaveModel : AbstractFileSave
	{
		public FileSaveModel(DocumentEditor editor) : base(editor)
		{
			if (editor.DocumentType == DocumentType.Pictogram)
			{
				this.FileExtension = ".iconmod";
			}
			else
			{
				this.FileExtension = ".crmod";
			}

			this.enableNavigation = true;
			this.enableMultipleSelection = false;
		}

		protected override Epsitec.Common.Dialogs.FileDialogType FileDialogType
		{
			get
			{
				return Epsitec.Common.Dialogs.FileDialogType.Save;
			}
		}

		protected override void CreateWindow()
		{
			//	Crée la fenêtre du dialogue.
			this.CreateUserInterface("FileSaveModel", new Size(720, 480), Res.Strings.Dialog.Save.TitleMod, 50, this.editor.Window);
		}
	}
}
