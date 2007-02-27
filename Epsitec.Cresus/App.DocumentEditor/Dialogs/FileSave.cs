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
			this.CreateAll ("FileSave", new Size (720, 480), Res.Strings.Dialog.Save.TitleDoc, 20);
		}

		public override void PersistWindowBounds()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("FileSave");
		}
	}
}
