using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.Common.DocumentEditor.Dialogs
{
	using GlobalSettings=Common.Document.Settings.GlobalSettings;
	using Document=Common.Document.Document;

	public abstract class AbstractFile : Epsitec.Common.Dialogs.AbstractFileDialog
	{
		public AbstractFile(DocumentEditor editor)
		{
			this.editor = editor;
			this.globalSettings = editor.GlobalSettings;
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

			if (this.globalSettings.GetWindowBounds(name, out location, out size))
			{
				return new Rectangle(location, size);
			}
			else
			{
				return base.GetPersistedWindowBounds(name);
			}
		}

		protected override void CreateFileExtensionDescriptions(Epsitec.Common.Dialogs.IFileExtensionDescription settings)
		{
			settings.Add(".crdoc", Res.Strings.Dialog.File.Document);
			settings.Add(".crmod", Res.Strings.Dialog.File.Model);
		}

		protected override void FavoritesAddApplicationFolders()
		{
			if (this.FileDialogType != Epsitec.Common.Dialogs.FileDialogType.Save)
			{
				this.AddFavorite(Document.OriginalSamplesDisplayName, Misc.Icon("FileTypeOriginalSamples"), Document.OriginalSamplesPath);
			}

			this.AddFavorite(Document.MySamplesDisplayName, Misc.Icon("FileTypeMySamples"), Document.MySamplesPath);
		}

		protected override string RedirectPath(string path)
		{
			Document.RedirectPath(ref path);
			return path;
		}

		protected override Rectangle GetOwnerBounds()
		{
			//	Donne les frontières de l'application.
			Window window = this.editor.Window;

			if (window == null)
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
			if (this.window == null || this.globalSettings == null)
			{
				return;
			}

			this.globalSettings.SetWindowBounds(this.window.Name, this.window.WindowLocation, this.window.ClientSize);
		}
		
		protected DocumentEditor				editor;
		protected GlobalSettings				globalSettings;
	}
}
