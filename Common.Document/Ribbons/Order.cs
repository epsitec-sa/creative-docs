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
    /// La classe Order permet de choisir l'ordre de la sélection.
    /// </summary>
    public class Order : Abstract
    {
        public Order(DocumentType type, InstallType install, DebugMode debugMode)
            : base(type, install, debugMode)
        {
            this.Title = Res.Strings.Action.OrderMain;
            this.PreferredWidth = 8 + 22 * 1.5 * 2 + 4 + 22 * 1;

            this.buttonUpAll = this.CreateIconButton("OrderUpAll", "Large");
            this.buttonDownAll = this.CreateIconButton("OrderDownAll", "Large");
            this.buttonUpOne = this.CreateIconButton("OrderUpOne");
            this.buttonDownOne = this.CreateIconButton("OrderDownOne");

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

            if (this.buttonDownAll == null)
                return;

            double dx = this.buttonDownAll.PreferredWidth;
            double dy = this.buttonDownAll.PreferredHeight;

            Rectangle rect = this.UsefulZone;
            rect.Width = dx * 1.5;
            rect.Height = dy * 1.5;
            rect.Offset(0, dy * 0.5 + 5);
            this.buttonUpAll.SetManualBounds(rect);
            rect.Offset(dx * 1.5, -dy * 0.5 - 5);
            this.buttonDownAll.SetManualBounds(rect);

            rect = this.UsefulZone;
            rect.Width = dx;
            rect.Height = dy;
            rect.Offset(dx * 1.5 * 2 + 4, dy + 5);
            this.buttonUpOne.SetManualBounds(rect);
            rect.Offset(0, -dy - 5);
            this.buttonDownOne.SetManualBounds(rect);
        }

        protected IconButton buttonDownAll;
        protected IconButton buttonDownOne;
        protected IconButton buttonUpOne;
        protected IconButton buttonUpAll;
    }
}
