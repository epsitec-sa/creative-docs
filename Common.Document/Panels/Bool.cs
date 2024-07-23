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

namespace Epsitec.Common.Document.Panels
{
    /// <summary>
    /// La classe Bool permet de choisir une valeur booléenne.
    /// </summary>
    public class Bool : Abstract
    {
        public Bool(Document document)
            : base(document)
        {
            this.grid = new RadioIconGrid(this);
            this.grid.SelectionChanged += HandleTypeChanged;
            this.grid.TabIndex = 0;
            this.grid.TabNavigationMode = TabNavigationMode.ActivateOnTab;

            this.AddRadioIcon(false);
            this.AddRadioIcon(true);
        }

        protected void AddRadioIcon(bool type)
        {
            this.grid.AddRadioIcon(
                Misc.Icon(Properties.Bool.GetIconText(type)),
                Properties.Bool.GetName(type),
                type ? 1 : 0,
                false
            );
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.grid.SelectionChanged -= HandleTypeChanged;
                this.grid = null;
            }

            base.Dispose(disposing);
        }

        protected override void PropertyToWidgets()
        {
            //	Propriété -> widgets.
            base.PropertyToWidgets();

            Properties.Bool p = this.property as Properties.Bool;
            if (p == null)
                return;

            this.ignoreChanged = true;
            this.grid.SelectedValue = p.BoolValue ? 1 : 0;
            this.ignoreChanged = false;
        }

        protected override void WidgetsToProperty()
        {
            //	Widgets -> propriété.
            Properties.Bool p = this.property as Properties.Bool;
            if (p == null)
                return;

            p.BoolValue = (this.grid.SelectedValue == 1);
        }

        protected override void UpdateClientGeometry()
        {
            //	Met à jour la géométrie.
            base.UpdateClientGeometry();

            if (this.grid == null)
                return;

            Rectangle rect = this.UsefulZone;
            rect.Inflate(1);
            this.grid.SetManualBounds(rect);
        }

        private void HandleTypeChanged(object sender)
        {
            //	Le type a été changé.
            if (this.ignoreChanged)
                return;
            this.OnChanged();
        }

        protected RadioIconGrid grid;
    }
}
