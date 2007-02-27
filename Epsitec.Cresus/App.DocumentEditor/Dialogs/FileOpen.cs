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
	public class FileOpen : AbstractFile
	{
		public FileOpen(DocumentEditor editor) : base(editor)
		{
			this.FileExtension = ".crdoc";
			this.isModel = false;
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
			this.CreateAll ("FileOpen", new Size (720, 480), Res.Strings.Dialog.Open.TitleDoc, 20);
		}

		public override void PersistWindowBounds()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("FileOpen");
		}
	}
}
