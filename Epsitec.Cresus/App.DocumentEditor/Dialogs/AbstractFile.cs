using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	using GlobalSettings=Common.Document.Settings.GlobalSettings;

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

		protected override bool UpdateWindowBounds(string name, ref Epsitec.Common.Drawing.Point location, ref Epsitec.Common.Drawing.Size size)
		{
			return this.globalSettings.GetWindowBounds (name, ref location, ref size);
		}

		protected override Window GetWindowOwner()
		{
			return this.editor.Window;
		}

		protected override void CreateFileExtensionDescriptions(Epsitec.Common.Dialogs.IFileExtensionDescription settings)
		{
			settings.Add (".crdoc", Res.Strings.Dialog.File.Document);
			settings.Add (".crmod", Res.Strings.Dialog.File.Model);
		}

		protected override void FavoritesAddApplicationFolders()
		{
			if (!this.isSave)
			{
				this.FavoritesAdd (Document.OriginalSamplesDisplayName, Misc.Icon ("FileTypeOriginalSamples"), Document.OriginalSamplesPath);
			}

			this.FavoritesAdd (Document.MySamplesDisplayName, Misc.Icon ("FileTypeMySamples"), Document.MySamplesPath);
		}

		protected override string RedirectPath(string path)
		{
			Document.RedirectPath (ref path);
			return path;
		}

		protected override Rectangle GetCurrentBounds()
		{
			//	Donne les frontières de l'application.
			Window window = this.GetWindowOwner ();

			if ( window == null )
			{
				return this.globalSettings.MainWindowBounds;
			}
			else
			{
				return new Rectangle(window.WindowLocation, window.WindowSize);
			}
		}
		
		protected void WindowSave(string name)
		{
			//	Sauve la fenêtre.
			if (this.window == null)
			{
				return;
			}

			this.globalSettings.SetWindowBounds (name, this.window.WindowLocation, this.window.ClientSize);
		}
		
		protected DocumentEditor editor;
		protected GlobalSettings globalSettings;
	}
}
