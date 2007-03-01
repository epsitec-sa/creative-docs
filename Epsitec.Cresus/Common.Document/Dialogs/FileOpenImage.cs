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
			this.FileFilterPattern = "*.bmp|*.tif|*.tiff|*.jpg|*.jpeg|*.gif|*.png|*.wmf|*.emf";
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
		
		protected override string FileTypeLabel
		{
			get
			{
				return Res.Strings.Dialog.OpenImage.FileLabel;
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
			//	Cr�e la fen�tre du dialogue.
			this.CreateUserInterface("FileOpenImage", new Size(720, 480), Res.Strings.Dialog.OpenImage.Title, 20, this.ownerWindow);
		}
	}
}
