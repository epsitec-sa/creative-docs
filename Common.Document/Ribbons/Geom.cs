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
    /// La classe Geom permet de modifier la géométrie de la sélection.
    /// </summary>
    public class Geom : Abstract
    {
        public Geom(DocumentType type, InstallType install, DebugMode debugMode)
            : base(type, install, debugMode)
        {
            this.Title = Res.Strings.Action.GeometryMain;
            this.PreferredWidth = 8 + 22 * 4;

            this.buttonCombine = this.CreateIconButton("Combine");
            this.buttonUncombine = this.CreateIconButton("Uncombine");
            this.buttonReset = this.CreateIconButton("Reset");
            this.buttonToBezier = this.CreateIconButton("ToBezier");
            this.buttonToPoly = this.CreateIconButton("ToPoly");
            this.buttonToTextBox2 = this.CreateIconButton("ToTextBox2");
            this.buttonFragment = this.CreateIconButton("Fragment");

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

            if (this.buttonCombine == null)
                return;

            double dx = this.buttonCombine.PreferredWidth;
            double dy = this.buttonCombine.PreferredHeight;

            Rectangle rect = this.UsefulZone;
            rect.Width = dx;
            rect.Height = dy;
            rect.Offset(0, dy + 5);
            this.buttonCombine.SetManualBounds(rect);
            rect.Offset(dx, 0);
            this.buttonUncombine.SetManualBounds(rect);
            rect.Offset(dx * 2, 0);
            this.buttonReset.SetManualBounds(rect);

            rect = this.UsefulZone;
            rect.Width = dx;
            rect.Height = dy;
            this.buttonToBezier.SetManualBounds(rect);
            rect.Offset(dx, 0);
            this.buttonToPoly.SetManualBounds(rect);
            rect.Offset(dx, 0);
            this.buttonToTextBox2.SetManualBounds(rect);
            rect.Offset(dx, 0);
            this.buttonFragment.SetManualBounds(rect);
        }

        protected IconButton buttonCombine;
        protected IconButton buttonUncombine;
        protected IconButton buttonReset;
        protected IconButton buttonToBezier;
        protected IconButton buttonToPoly;
        protected IconButton buttonToTextBox2;
        protected IconButton buttonFragment;
    }
}
