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
    /// La classe Debug donne accès aux commandes provisoires de debug.
    /// </summary>
    public class Debug : Abstract
    {
        public Debug(DocumentType type, InstallType install, DebugMode debugMode)
            : base(type, install, debugMode)
        {
            this.Title = "Debug";
            this.PreferredWidth = 8 + 40;

            this.buttonOthers = this.CreateMenuButton(
                "",
                "Debug menu...",
                this.HandleOthersPressed
            );

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

            if (this.buttonOthers == null)
                return;

            double dx = this.buttonOthers.PreferredWidth;
            double dy = this.buttonOthers.PreferredHeight;

            Rectangle rect = this.UsefulZone;
            rect.Width = dx;
            rect.Height = dy;
            rect.Offset(0, dy);
            this.buttonOthers.SetManualBounds(rect);
        }

        private void HandleOthersPressed(object sender, MessageEventArgs e)
        {
            //	Bouton pour ouvrir le menu des autres opérations.
            GlyphButton button = sender as GlyphButton;
            if (button == null)
                return;
            VMenu menu = this.BuildOthersMenu();
            if (menu == null)
                return;
            menu.Host = this;
            menu.MinWidth = button.ActualWidth;
            TextFieldCombo.AdjustComboSize(button, menu, false);
            menu.ShowAsComboList(button, Point.Zero, button);
        }

        protected VMenu BuildOthersMenu()
        {
            //	Construit le sous-menu des autres opérations.
            VMenu menu = new VMenu();

#if false
			this.MenuAdd(menu, "", "ResDesignerBuild", "Ressources Designer (build)", "");
			this.MenuAdd(menu, "", "ResDesignerTranslate", "Ressources Designer (translate)", "");
			this.MenuAdd(menu, "", "", "", "");
#endif
            this.MenuAdd(menu, "y/n", "DebugBboxThin", "Show BBoxThin", "");
            this.MenuAdd(menu, "y/n", "DebugBboxGeom", "Show BBoxGeom", "");
            this.MenuAdd(menu, "y/n", "DebugBboxFull", "Show BBoxFull", "");
            this.MenuAdd(menu, "", "", "", "");
            this.MenuAdd(menu, "", "DebugDirty", "Make dirty", "F12");
            this.MenuAdd(menu, "", "", "", "");
            this.MenuAdd(menu, "", "ForceSaveAll", "Save and overwrite all", "");
            this.MenuAdd(menu, "", "OverwriteAll", "Open, overwrite and close all", "");

            menu.AdjustSize();
            return menu;
        }

        protected GlyphButton buttonOthers;
    }
}
