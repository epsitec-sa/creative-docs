using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Dialogs
{
	using GlobalSettings=Common.Document.Settings.GlobalSettings;

	public abstract class AbstractFile : Epsitec.Common.Dialogs.AbstractFileDialog
	{
		public AbstractFile(Document document, Window ownerWindow)
		{
			this.document = document;
			this.ownerWindow = ownerWindow;
			this.globalSettings = document.GlobalSettings;
		}

		protected override Epsitec.Common.Dialogs.IFavoritesSettings FavoritesSettings
		{
			get
			{
				return this.globalSettings;
			}
		}

		protected override Rectangle GetPersistedWindowBounds(string name)
		{
			Point location;
			Size  size;

			if (this.globalSettings.GetWindowBounds (name, out location, out size))
			{
				return new Rectangle (location, size);
			}
			else
			{
				return base.GetPersistedWindowBounds (name);
			}
		}

		protected override void CreateFileExtensionDescriptions(Epsitec.Common.Dialogs.IFileExtensionDescription settings)
		{
			settings.Add (".bmp", "Image JPG");
			settings.Add (".tif", "Image TIFF");
			settings.Add (".tiff", "Image TIFF");
			settings.Add (".jpg", "Image JPEG");
			settings.Add (".jpeg", "Image JPEG");
			settings.Add (".gif", "Image GIF");
			settings.Add (".png", "Image PNG");
			settings.Add (".wmf", "Image WMF");
			settings.Add (".emf", "Image EMF");
		}

		protected override void FavoritesAddApplicationFolders()
		{
		}

		protected override string RedirectPath(string path)
		{
			Document.RedirectPath (ref path);
			return path;
		}

		protected override Rectangle GetOwnerBounds()
		{
			//	Donne les fronti�res de l'application.
			if ( this.ownerWindow == null )
			{
				return this.globalSettings.MainWindowBounds;
			}
			else
			{
				return new Rectangle(this.ownerWindow.WindowLocation, this.ownerWindow.WindowSize);
			}
		}
		
		public override void PersistWindowBounds()
		{
			//	Sauve la fen�tre.
			if ((this.window == null) ||
				(this.globalSettings == null))
			{
				return;
			}

			this.globalSettings.SetWindowBounds (this.window.Name, this.window.WindowLocation, this.window.ClientSize);
		}
		
		protected Document document;
		protected Window ownerWindow;
		protected GlobalSettings globalSettings;
	}
}
