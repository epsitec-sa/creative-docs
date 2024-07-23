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

namespace Epsitec.Common.Document.Ribbons
{
    /// <summary>
    /// La classe Replace gère les commandes chercher/remplacer.
    /// </summary>
    public class Replace : Abstract
    {
        public Replace(DocumentType type, InstallType install, DebugMode debugMode)
            : base(type, install, debugMode)
        {
            this.Title = Res.Strings.Action.ReplaceMain;
            this.PreferredWidth = 8 + 22 * 1.5 * 2;

            this.buttonReplace = this.CreateIconButton("Replace", "Large");

            //			this.UpdateClientGeometry();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { }

            base.Dispose(disposing);
        }

        protected override void UpdateClientGeometry()
        {
            //	Met à jour la géométrie.
            base.UpdateClientGeometry();

            if (this.buttonReplace == null)
                return;

            double dx = this.buttonReplace.PreferredWidth;
            double dy = this.buttonReplace.PreferredHeight;

            Rectangle rect = this.UsefulZone;
            rect.Width = dx * 1.5;
            rect.Height = dy * 1.5;
            rect.Offset(0, dy * 0.5);
            this.buttonReplace.SetManualBounds(rect);
        }

        protected IconButton buttonReplace;
    }
}
