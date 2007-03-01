using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using System.IO;

namespace Epsitec.Common.Document.Dialogs
{
	/// <summary>
	/// Dialogue pour exporter une image bitmap.
	/// </summary>
	public class FileSaveImage : AbstractFile
	{
		public FileSaveImage(Document document, Window ownerWindow) : base(document, ownerWindow)
		{
			this.FileFilterPattern = "*.bmp|*.tif|*.tiff|*.jpg|*.jpeg|*.gif|*.png|*.wmf|*.emf";
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
				return Res.Strings.Dialog.SaveImage.ActionButtonName;
			}
		}

		protected override void FavoritesAddApplicationFolders()
		{
			this.AddFavorite(FolderId.MyPictures);
		}

		protected override void CreateWindow()
		{
			//	Crée la fenêtre du dialogue.
			this.CreateUserInterface("FileSaveImage", new Size(720, 480), Res.Strings.Dialog.SaveImage.Title, 20, this.ownerWindow);
		}
	}
}
