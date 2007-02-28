using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Dialogs
{
	using GlobalSettings=Common.Document.Settings.GlobalSettings;

	public abstract class AbstractFile : Epsitec.Common.Dialogs.AbstractFileDialog
	{
		public AbstractFile(Document document)
		{
			this.document = document;
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
			settings.Add (".jpg", "Toto");
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
			//	Donne les frontières de l'application.
			Window window = this.editor.Window;

			if ( window == null )
			{
				return this.globalSettings.MainWindowBounds;
			}
			else
			{
				return new Rectangle(window.WindowLocation, window.WindowSize);
			}
		}
		
		public override void PersistWindowBounds()
		{
			//	Sauve la fenêtre.
			if ((this.window == null) ||
				(this.globalSettings == null))
			{
				return;
			}

			this.globalSettings.SetWindowBounds (this.window.Name, this.window.WindowLocation, this.window.ClientSize);
		}
		
		protected Document document;
		protected GlobalSettings globalSettings;
	}
}
