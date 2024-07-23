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
    /// La classe Insert permet de choisir un élément à insérer dans le texte.
    /// </summary>
    public class Insert : Abstract
    {
        public Insert(DocumentType type, InstallType install, DebugMode debugMode)
            : base(type, install, debugMode)
        {
            this.Title = Res.Strings.Action.Text.Insert;
            this.PreferredWidth = 8 + 22 * 2;

            this.buttonNewFrame = this.CreateIconButton("TextInsertNewFrame");
            this.buttonNewPage = this.CreateIconButton("TextInsertNewPage");
            this.buttonQuad = this.CreateIconButton("TextInsertQuad");
            this.buttonGlyphs = this.CreateIconButton("Glyphs");

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

            if (this.buttonGlyphs == null)
                return;

            double dx = this.buttonGlyphs.PreferredWidth;
            double dy = this.buttonGlyphs.PreferredHeight;

            Rectangle rect = this.UsefulZone;

            rect = this.UsefulZone;
            rect.Width = dx;
            rect.Height = dy;
            rect.Offset(0, dy + 5);
            this.buttonNewFrame.SetManualBounds(rect);
            rect.Offset(20, 0);
            this.buttonNewPage.SetManualBounds(rect);

            rect = this.UsefulZone;
            rect.Width = dx;
            rect.Height = dy;
            this.buttonQuad.SetManualBounds(rect);
            rect.Offset(20, 0);
            this.buttonGlyphs.SetManualBounds(rect);
        }

        protected IconButton buttonNewFrame;
        protected IconButton buttonNewPage;
        protected IconButton buttonQuad;
        protected IconButton buttonGlyphs;
    }
}
