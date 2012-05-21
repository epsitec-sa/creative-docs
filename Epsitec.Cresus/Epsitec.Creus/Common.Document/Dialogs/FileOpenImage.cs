using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using System.IO;

namespace Epsitec.Common.Document.Dialogs
{
	/// <summary>
	/// Dialogue pour importer une image bitmap.
	/// </summary>
	public class FileOpenImage : AbstractFile
	{
		public FileOpenImage(Document document, Window ownerWindow) : base(document, ownerWindow)
		{
			//	Il faut mettre en premier les extensions qu'on souhaite voir.
			this.FileFilterPattern = "*.tif|*.jpg|*.gif|*.png|*.bmp|*.wmf|*.emf|*.tiff|*.jpeg";
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
		
		protected override string ActionButtonName
		{
			get
			{
				return Res.Strings.Dialog.OpenImage.ActionButtonName;
			}
		}

		protected override void FavoritesAddApplicationFolders()
		{
			this.AddFavorite(FolderId.MyPictures);
		}

		protected override void CreateWindow()
		{
			//	Crée la fenêtre du dialogue.
			this.CreateUserInterface("FileOpenImage", new Size(720, 480), Res.Strings.Dialog.OpenImage.Title, 20, this.ownerWindow);
		}
	}
}
