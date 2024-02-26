using Epsitec.Common.Document;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.DocumentEditor.Dialogs
{
    using Document = Common.Document.Document;
    using GlobalSettings = Common.Document.Settings.GlobalSettings;

    public abstract class AbstractFile : Epsitec.Common.Dialogs.AbstractFileDialog
    {
        public AbstractFile(DocumentEditor editor)
        {
            this.editor = editor;
            this.globalSettings = editor.GlobalSettings;
        }

        protected override Epsitec.Common.Dialogs.IFavoritesSettings FavoritesSettings
        {
            get { return this.globalSettings; }
        }

        protected override void FavoritesAddApplicationFolders()
        {
            if (this.fileDialogType != Epsitec.Common.Dialogs.FileDialogType.Save)
            {
                this.AddFavorite(
                    Document.OriginalSamplesDisplayName,
                    Misc.Icon("FileTypeOriginalSamples"),
                    Document.OriginalSamplesPath
                );
            }

            this.AddFavorite(
                Document.MySamplesDisplayName,
                Misc.Icon("FileTypeMySamples"),
                Document.MySamplesPath
            );
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

        protected DocumentEditor editor;
        protected GlobalSettings globalSettings;
    }
}
