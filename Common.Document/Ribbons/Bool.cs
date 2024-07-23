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
    /// La classe Bool permet de choisir les opérations booléennes.
    /// </summary>
    public class Bool : Abstract
    {
        public Bool(DocumentType type, InstallType install, DebugMode debugMode)
            : base(type, install, debugMode)
        {
            this.Title = Res.Strings.Action.BooleanMain;
            this.PreferredWidth = 8 + 22 * 3;

            this.buttonBooleanOr = this.CreateIconButton("BooleanOr");
            this.buttonBooleanAnd = this.CreateIconButton("BooleanAnd");
            this.buttonBooleanXor = this.CreateIconButton("BooleanXor");
            this.buttonBooleanFrontMinus = this.CreateIconButton("BooleanFrontMinus");
            this.buttonBooleanBackMinus = this.CreateIconButton("BooleanBackMinus");

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

            if (this.buttonBooleanOr == null)
                return;

            double dx = this.buttonBooleanOr.PreferredWidth;
            double dy = this.buttonBooleanOr.PreferredHeight;

            Rectangle rect = this.UsefulZone;
            rect.Width = dx;
            rect.Height = dy;
            rect.Offset(0, dy + 5);
            this.buttonBooleanOr.SetManualBounds(rect);
            rect.Offset(dx, 0);
            this.buttonBooleanAnd.SetManualBounds(rect);
            rect.Offset(dx, 0);
            this.buttonBooleanXor.SetManualBounds(rect);

            rect = this.UsefulZone;
            rect.Width = dx;
            rect.Height = dy;
            this.buttonBooleanFrontMinus.SetManualBounds(rect);
            rect.Offset(dx, 0);
            this.buttonBooleanBackMinus.SetManualBounds(rect);
        }

        protected IconButton buttonBooleanOr;
        protected IconButton buttonBooleanAnd;
        protected IconButton buttonBooleanXor;
        protected IconButton buttonBooleanFrontMinus;
        protected IconButton buttonBooleanBackMinus;
    }
}
