using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using System.IO;

namespace Epsitec.Common.Document.Dialogs
{
	/// <summary>
	/// Dialogue pour ouvrir un document existant.
	/// </summary>
	public class FileOpenImage : AbstractFile
	{
		public FileOpenImage(Document document, Window ownerWindow) : base(document, ownerWindow)
		{
			this.FileExtension = ".jpg";
			this.enableNavigation = true;
			this.enableMultipleSelection = false;
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
			this.CreateUserInterface ("FileOpenImage", new Size (720, 480), "Toto", 20, this.ownerWindow);
		}
	}
}
