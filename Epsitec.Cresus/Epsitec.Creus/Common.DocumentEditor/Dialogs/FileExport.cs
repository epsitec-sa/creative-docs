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
	/// Dialogue pour exporter un fichier.
	/// </summary>
	public class FileExport : AbstractFileSave
	{
		public FileExport(DocumentEditor editor) : base(editor)
		{
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

		protected override string ActionButtonName
		{
			get
			{
				return Res.Strings.Dialog.Export.Button.OK;
			}
		}

		protected override void CreateWindow()
		{
			//	Crée la fenêtre du dialogue.
			this.CreateUserInterface("FileExport", new Size(720, 480), null, 20, this.editor.Window);
		}
	}
}
