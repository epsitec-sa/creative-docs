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
	public class FileOpenModel : AbstractFile
	{
		public FileOpenModel(DocumentEditor editor) : base(editor)
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
			this.enableMultipleSelection = true;
		}

		protected override Epsitec.Common.Dialogs.FileDialogType FileDialogType
		{
			get
			{
				return Epsitec.Common.Dialogs.FileDialogType.Open;
			}
		}

		protected override void CreateWindow()
		{
			//	Crée la fenêtre du dialogue.
			this.CreateUserInterface("FileOpenModel", new Size(720, 480), Res.Strings.Dialog.Open.TitleMod, 50, this.editor.Window);
		}
	}
}
