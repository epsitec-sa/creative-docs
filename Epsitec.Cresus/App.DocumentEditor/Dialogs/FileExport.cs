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

		protected override string FileTypeLabel
		{
			get
			{
				return "Fichier";
			}
		}

		protected override string ActionButtonName
		{
			get
			{
				return "Exporter";
			}
		}

		protected override void CreateWindow()
		{
			//	Crée la fenêtre du dialogue.
			this.CreateUserInterface("FileExport", new Size(720, 480), "Fichier à exporter", 20, this.editor.Window);
		}
	}
}
