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
			this.document       = document;
			this.ownerWindow    = ownerWindow;
			this.globalSettings = document.GlobalSettings;
		}

		protected override Epsitec.Common.Dialogs.IFavoritesSettings FavoritesSettings
		{
			get
			{
				return this.globalSettings;
			}
		}

		protected override void FavoritesAddApplicationFolders()
		{
		}

		protected override string RedirectPath(string path)
		{
			Document.RedirectPath(ref path);
			return path;
		}

		protected override Rectangle GetOwnerBounds()
		{
			//	Donne les frontières de l'application.
			if (this.ownerWindow == null)
			{
				return this.globalSettings.MainWindowBounds;
			}
			else
			{
				return new Rectangle(this.ownerWindow.WindowLocation, this.ownerWindow.WindowSize);
			}
		}
		
		protected Document document;
		protected Window ownerWindow;
		protected GlobalSettings globalSettings;
	}
}
