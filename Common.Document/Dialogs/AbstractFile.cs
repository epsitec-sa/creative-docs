/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document.Dialogs
{
    using GlobalSettings = Common.Document.Settings.GlobalSettings;

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
            get { return this.globalSettings; }
        }

        protected override void FavoritesAddApplicationFolders() { }

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
