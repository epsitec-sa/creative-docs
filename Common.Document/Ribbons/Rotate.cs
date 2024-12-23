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
    /// La classe Rotate permet de tourner la sélection.
    /// </summary>
    public class Rotate : Abstract
    {
        public Rotate(DocumentType type, InstallType install, DebugMode debugMode)
            : base(type, install, debugMode)
        {
            this.Title = Res.Strings.Action.RotateMain;
            this.PreferredWidth = 8 + 22 * 2 + this.separatorWidth + 50;

            this.buttonRotate90 = this.CreateIconButton("Rotate90");
            this.buttonRotate270 = this.CreateIconButton("Rotate270");
            this.buttonRotate180 = this.CreateIconButton("Rotate180");
            this.separator = new IconSeparator(this);
            this.buttonRotateFreeCCW = this.CreateIconButton("RotateFreeCCW");
            this.buttonRotateFreeCW = this.CreateIconButton("RotateFreeCW");
            this.fieldRotate = this.CreateFieldRot(Res.Strings.Action.RotateValue);

            //			this.UpdateClientGeometry();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { }

            base.Dispose(disposing);
        }

        public override void SetDocument(Settings.GlobalSettings gs, Document document)
        {
            base.SetDocument(gs, document);

            this.AdaptFieldRot(this.fieldRotate);
        }

        protected override void UpdateClientGeometry()
        {
            //	Met à jour la géométrie.
            base.UpdateClientGeometry();

            if (this.buttonRotate90 == null)
                return;

            double dx = this.buttonRotate90.PreferredWidth;
            double dy = this.buttonRotate90.PreferredHeight;

            Rectangle rect = this.UsefulZone;
            rect.Left += dx * 2;
            rect.Width = this.separatorWidth;
            this.separator.SetManualBounds(rect);

            rect = this.UsefulZone;
            rect.Width = dx;
            rect.Height = dy;
            rect.Offset(0, dy + 5);
            this.buttonRotate90.SetManualBounds(rect);
            rect.Offset(dx, 0);
            this.buttonRotate270.SetManualBounds(rect);
            rect.Offset(dx + this.separatorWidth, 0);
            rect.Width = 25;
            this.buttonRotateFreeCCW.SetManualBounds(rect);
            rect.Offset(25, 0);
            this.buttonRotateFreeCW.SetManualBounds(rect);

            rect = this.UsefulZone;
            rect.Width = dx;
            rect.Height = dy;
            rect.Offset(dx * 0.5, 0);
            this.buttonRotate180.SetManualBounds(rect);
            rect.Offset(dx * 1.5 + this.separatorWidth, 0);
            rect.Width = 50;
            this.fieldRotate.SetManualBounds(rect);
        }

        protected TextFieldReal CreateFieldRot(string tooltip)
        {
            //	Crée un champ éditable pour une rotation.
            TextFieldReal field = new TextFieldReal(this);
            field.PreferredWidth = 50;
            field.TabIndex = this.tabIndex++;
            field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
            field.ValueChanged += this.HandleFieldValueChanged;
            ToolTip.Default.SetToolTip(field, tooltip);
            return field;
        }

        protected void AdaptFieldRot(TextFieldReal field)
        {
            //	Adapte un champ éditable pour une rotation.
            if (this.document == null)
            {
                field.Enable = false;
            }
            else
            {
                field.Enable = true;

                this.ignoreChange = true;
                this.document.Modifier.AdaptTextFieldRealAngle(field);
                field.InternalValue = (decimal)this.document.Modifier.RotateAngle;
                this.ignoreChange = false;
            }
        }

        private void HandleFieldValueChanged(object sender)
        {
            if (this.ignoreChange)
                return;
            TextFieldReal field = sender as TextFieldReal;
            if (field == this.fieldRotate)
            {
                this.document.Modifier.RotateAngle = (double)field.InternalValue;
            }
        }

        protected IconButton buttonRotate90;
        protected IconButton buttonRotate180;
        protected IconButton buttonRotate270;
        protected IconSeparator separator;
        protected IconButton buttonRotateFreeCCW;
        protected IconButton buttonRotateFreeCW;
        protected TextFieldReal fieldRotate;
    }
}
