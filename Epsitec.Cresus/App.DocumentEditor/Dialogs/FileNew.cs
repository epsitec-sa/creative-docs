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
			this.FileExtension = ".crmod";
			this.isModel = true;
			this.enableNavigation = true;
			this.enableMultipleSelection = false;
		}

		protected override Epsitec.Common.Dialogs.FileDialogType FileDialogType
		{
			get
			{
				return Epsitec.Common.Dialogs.FileDialogType.New;
			}
		}

		protected override void CreateWindow()
		{
			//	Crée la fenêtre du dialogue.
			this.CreateUserInterface ("FileNew", new Size (720, 480), Res.Strings.Dialog.New.Title, 50, this.editor.Window);
		}

		public override void PersistWindowBounds()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("FileNew");
		}
	}
}
