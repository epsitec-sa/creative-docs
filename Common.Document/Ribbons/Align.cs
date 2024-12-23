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
    /// La classe Align permet de choisir les commandes d'alignement de la sélection.
    /// </summary>
    public class Align : Abstract
    {
        public Align(DocumentType type, InstallType install, DebugMode debugMode)
            : base(type, install, debugMode)
        {
            this.Title = Res.Strings.Action.AlignMain;
            this.PreferredWidth =
                8
                + 22 * 3
                + this.separatorWidth
                + 22 * 4
                + this.separatorWidth
                + 22
                + this.separatorWidth
                + 22;

            this.buttonAlignLeft = this.CreateIconButton("AlignLeft");
            this.buttonAlignCenterX = this.CreateIconButton("AlignCenterX");
            this.buttonAlignRight = this.CreateIconButton("AlignRight");
            this.buttonAlignTop = this.CreateIconButton("AlignTop");
            this.buttonAlignCenterY = this.CreateIconButton("AlignCenterY");
            this.buttonAlignBottom = this.CreateIconButton("AlignBottom");

            this.separator1 = new IconSeparator(this);

            this.buttonShareSpaceX = this.CreateIconButton("ShareSpaceX");
            this.buttonShareLeft = this.CreateIconButton("ShareLeft");
            this.buttonShareCenterX = this.CreateIconButton("ShareCenterX");
            this.buttonShareRight = this.CreateIconButton("ShareRight");
            this.buttonShareSpaceY = this.CreateIconButton("ShareSpaceY");
            this.buttonShareTop = this.CreateIconButton("ShareTop");
            this.buttonShareCenterY = this.CreateIconButton("ShareCenterY");
            this.buttonShareBottom = this.CreateIconButton("ShareBottom");

            this.separator2 = new IconSeparator(this);

            this.buttonAdjustWidth = this.CreateIconButton("AdjustWidth");
            this.buttonAdjustHeight = this.CreateIconButton("AdjustHeight");

            this.separator3 = new IconSeparator(this);

            this.buttonAlignGrid = this.CreateIconButton("AlignGrid");

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

            if (this.buttonAlignLeft == null)
                return;

            double dx = this.buttonAlignLeft.PreferredWidth;
            double dy = this.buttonAlignLeft.PreferredHeight;

            Rectangle rect = this.UsefulZone;
            rect.Left += dx * 3;
            rect.Width = this.separatorWidth;
            this.separator1.SetManualBounds(rect);

            rect = this.UsefulZone;
            rect.Left += dx * 3 + this.separatorWidth + dx * 4;
            rect.Width = this.separatorWidth;
            this.separator2.SetManualBounds(rect);

            rect = this.UsefulZone;
            rect.Left += dx * 3 + this.separatorWidth + dx * 4 + this.separatorWidth + dx;
            rect.Width = this.separatorWidth;
            this.separator3.SetManualBounds(rect);

            rect = this.UsefulZone;
            rect.Width = dx;
            rect.Height = dy;
            rect.Offset(0, dy + 5);
            this.buttonAlignLeft.SetManualBounds(rect);
            rect.Offset(dx, 0);
            this.buttonAlignCenterX.SetManualBounds(rect);
            rect.Offset(dx, 0);
            this.buttonAlignRight.SetManualBounds(rect);
            rect.Offset(dx + this.separatorWidth, 0);
            this.buttonShareLeft.SetManualBounds(rect);
            rect.Offset(dx, 0);
            this.buttonShareCenterX.SetManualBounds(rect);
            rect.Offset(dx, 0);
            this.buttonShareSpaceX.SetManualBounds(rect);
            rect.Offset(dx, 0);
            this.buttonShareRight.SetManualBounds(rect);
            rect.Offset(dx + this.separatorWidth, 0);
            this.buttonAdjustWidth.SetManualBounds(rect);
            rect.Offset(dx + this.separatorWidth, 0);
            this.buttonAlignGrid.SetManualBounds(rect);

            rect = this.UsefulZone;
            rect.Width = dx;
            rect.Height = dy;
            this.buttonAlignTop.SetManualBounds(rect);
            rect.Offset(dx, 0);
            this.buttonAlignCenterY.SetManualBounds(rect);
            rect.Offset(dx, 0);
            this.buttonAlignBottom.SetManualBounds(rect);
            rect.Offset(dx + this.separatorWidth, 0);
            this.buttonShareTop.SetManualBounds(rect);
            rect.Offset(dx, 0);
            this.buttonShareCenterY.SetManualBounds(rect);
            rect.Offset(dx, 0);
            this.buttonShareSpaceY.SetManualBounds(rect);
            rect.Offset(dx, 0);
            this.buttonShareBottom.SetManualBounds(rect);
            rect.Offset(dx + this.separatorWidth, 0);
            this.buttonAdjustHeight.SetManualBounds(rect);
        }

        protected IconButton buttonAlignLeft;
        protected IconButton buttonAlignCenterX;
        protected IconButton buttonAlignRight;
        protected IconButton buttonAlignTop;
        protected IconButton buttonAlignCenterY;
        protected IconButton buttonAlignBottom;
        protected IconSeparator separator1;
        protected IconButton buttonShareLeft;
        protected IconButton buttonShareCenterX;
        protected IconButton buttonShareSpaceX;
        protected IconButton buttonShareRight;
        protected IconButton buttonShareTop;
        protected IconButton buttonShareCenterY;
        protected IconButton buttonShareSpaceY;
        protected IconButton buttonShareBottom;
        protected IconSeparator separator2;
        protected IconButton buttonAdjustWidth;
        protected IconButton buttonAdjustHeight;
        protected IconSeparator separator3;
        protected IconButton buttonAlignGrid;
    }
}
