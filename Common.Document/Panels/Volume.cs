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
    /// La classe Volume permet de choisir un objet volume 3d.
    /// </summary>
    public class Volume : Abstract
    {
        public Volume(Document document)
            : base(document)
        {
            this.grid = new RadioIconGrid(this);
            this.grid.SelectionChanged += HandleTypeChanged;
            this.grid.TabIndex = 0;
            this.grid.TabNavigationMode = TabNavigationMode.ActivateOnTab;

            this.AddRadioIcon(Properties.VolumeType.BoxClose);
            this.AddRadioIcon(Properties.VolumeType.BoxOpen);
            this.AddRadioIcon(Properties.VolumeType.Pyramid);
            this.AddRadioIcon(Properties.VolumeType.Cylinder);

            this.fieldRapport = new Widgets.TextFieldLabel(
                this,
                Widgets.TextFieldLabel.Type.TextFieldReal
            );
            this.fieldRapport.LabelShortText = Res.Strings.Panel.Volume.Short.Rapport;
            this.fieldRapport.LabelLongText = Res.Strings.Panel.Volume.Long.Rapport;
            this.document.Modifier.AdaptTextFieldRealPercent(this.fieldRapport.TextFieldReal);
            this.fieldRapport.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
            this.fieldRapport.TabIndex = 2;
            this.fieldRapport.TabNavigationMode = TabNavigationMode.ActivateOnTab;
            ToolTip.Default.SetToolTip(this.fieldRapport, Res.Strings.Panel.Volume.Tooltip.Rapport);

            this.fieldLeft = new Widgets.TextFieldLabel(
                this,
                Widgets.TextFieldLabel.Type.TextFieldReal
            );
            this.fieldLeft.LabelShortText = Res.Strings.Panel.Volume.Short.Left;
            this.fieldLeft.LabelLongText = Res.Strings.Panel.Volume.Long.Left;
            this.document.Modifier.AdaptTextFieldRealAngle(this.fieldLeft.TextFieldReal);
            this.fieldLeft.TextFieldReal.InternalMaxValue = 90.0M;
            this.fieldLeft.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
            this.fieldLeft.TabIndex = 3;
            this.fieldLeft.TabNavigationMode = TabNavigationMode.ActivateOnTab;
            ToolTip.Default.SetToolTip(this.fieldLeft, Res.Strings.Panel.Volume.Tooltip.Left);

            this.fieldRight = new Widgets.TextFieldLabel(
                this,
                Widgets.TextFieldLabel.Type.TextFieldReal
            );
            this.fieldRight.LabelShortText = Res.Strings.Panel.Volume.Short.Right;
            this.fieldRight.LabelLongText = Res.Strings.Panel.Volume.Long.Right;
            this.document.Modifier.AdaptTextFieldRealAngle(this.fieldRight.TextFieldReal);
            this.fieldRight.TextFieldReal.InternalMaxValue = 90.0M;
            this.fieldRight.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
            this.fieldRight.TabIndex = 4;
            this.fieldRight.TabNavigationMode = TabNavigationMode.ActivateOnTab;
            ToolTip.Default.SetToolTip(this.fieldRight, Res.Strings.Panel.Volume.Tooltip.Right);

            this.isNormalAndExtended = true;
        }

        protected void AddRadioIcon(Properties.VolumeType type)
        {
            this.grid.AddRadioIcon(
                Misc.Icon(Properties.Volume.GetIconText(type)),
                Properties.Volume.GetName(type),
                (int)type,
                false
            );
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.grid.SelectionChanged -= HandleTypeChanged;
                this.fieldRapport.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
                this.fieldLeft.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
                this.fieldRight.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;

                this.grid = null;
                this.fieldRapport = null;
                this.fieldLeft = null;
                this.fieldRight = null;
            }

            base.Dispose(disposing);
        }

        public override double DefaultHeight
        {
            //	Retourne la hauteur standard.
            get
            {
                double h = this.LabelHeight;

                if (this.isExtendedSize) // panneau étendu ?
                {
                    if (this.IsLabelProperties) // étendu/détails ?
                    {
                        h += 105;
                    }
                    else // étendu/compact ?
                    {
                        h += 55;
                    }
                }
                else // panneau réduit ?
                {
                    h += 30;
                }

                return h;
            }
        }

        protected override void PropertyToWidgets()
        {
            //	Propriété -> widgets.
            base.PropertyToWidgets();

            Properties.Volume p = this.property as Properties.Volume;
            if (p == null)
                return;

            this.ignoreChanged = true;

            this.grid.SelectedValue = (int)p.VolumeType;
            this.fieldRapport.TextFieldReal.InternalValue = (decimal)p.Rapport;
            this.fieldLeft.TextFieldReal.InternalValue = (decimal)p.AngleLeft;
            this.fieldRight.TextFieldReal.InternalValue = (decimal)p.AngleRight;

            this.EnableWidgets();
            this.ignoreChanged = false;
        }

        protected override void WidgetsToProperty()
        {
            //	Widgets -> propriété.
            Properties.Volume p = this.property as Properties.Volume;
            if (p == null)
                return;

            p.VolumeType = (Properties.VolumeType)this.grid.SelectedValue;
            p.Rapport = (double)this.fieldRapport.TextFieldReal.InternalValue;
            p.AngleLeft = (double)this.fieldLeft.TextFieldReal.InternalValue;
            p.AngleRight = (double)this.fieldRight.TextFieldReal.InternalValue;
        }

        protected void EnableWidgets()
        {
            //	Grise les widgets nécessaires.
        }

        protected override void UpdateClientGeometry()
        {
            //	Met à jour la géométrie.
            base.UpdateClientGeometry();

            if (this.grid == null)
                return;

            this.EnableWidgets();

            Rectangle rect = this.UsefulZone;

            Rectangle r = rect;
            r.Bottom = r.Top - 20;
            r.Inflate(1);
            this.grid.SetManualBounds(r);

            if (this.isExtendedSize && this.IsLabelProperties)
            {
                r.Top = rect.Top - 25;
                r.Bottom = r.Top - 20;
                r.Left = rect.Left;
                r.Right = rect.Right;
                this.fieldRapport.SetManualBounds(r);

                r.Top = r.Bottom - 5;
                r.Bottom = r.Top - 20;
                r.Left = rect.Left;
                r.Right = rect.Right;
                this.fieldLeft.SetManualBounds(r);

                r.Top = r.Bottom - 5;
                r.Bottom = r.Top - 20;
                r.Left = rect.Left;
                r.Right = rect.Right;
                this.fieldRight.SetManualBounds(r);
            }
            else
            {
                r.Top = rect.Top - 25;
                r.Bottom = r.Top - 20;
                r.Left = rect.Left;
                r.Width = Widgets.TextFieldLabel.ShortWidth;
                this.fieldRapport.SetManualBounds(r);

                r.Left = r.Right;
                r.Width = Widgets.TextFieldLabel.ShortWidth;
                this.fieldLeft.SetManualBounds(r);

                r.Left = r.Right;
                r.Width = Widgets.TextFieldLabel.ShortWidth;
                this.fieldRight.SetManualBounds(r);
            }
        }

        private void HandleTypeChanged(object sender)
        {
            //	Le type a été changé.
            if (this.ignoreChanged)
                return;
            this.EnableWidgets();
            this.OnChanged();
        }

        private void HandleFieldChanged(object sender)
        {
            //	Un champ a été changé.
            if (this.ignoreChanged)
                return;
            this.OnChanged();
        }

        protected RadioIconGrid grid;
        protected Widgets.TextFieldLabel fieldRapport;
        protected Widgets.TextFieldLabel fieldLeft;
        protected Widgets.TextFieldLabel fieldRight;
    }
}
