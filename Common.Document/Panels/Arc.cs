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
    /// La classe Arc permet de choisir un mode d'arc de cercle ou d'ellipse.
    /// </summary>
    public class Arc : Abstract
    {
        public Arc(Document document)
            : base(document)
        {
            this.grid = new RadioIconGrid(this);
            this.grid.SelectionChanged += HandleTypeChanged;
            this.grid.TabIndex = 1;
            this.grid.TabNavigationMode = TabNavigationMode.ActivateOnTab;

            this.AddRadioIcon(Properties.ArcType.Full);
            this.AddRadioIcon(Properties.ArcType.Open);
            this.AddRadioIcon(Properties.ArcType.Close);
            this.AddRadioIcon(Properties.ArcType.Pie);

            this.fieldStarting = new Widgets.TextFieldLabel(
                this,
                Widgets.TextFieldLabel.Type.TextFieldReal
            );
            this.fieldStarting.LabelShortText = Res.Strings.Panel.Arc.Short.Initial;
            this.fieldStarting.LabelLongText = Res.Strings.Panel.Arc.Long.Initial;
            this.document.Modifier.AdaptTextFieldRealAngle(this.fieldStarting.TextFieldReal);
            this.fieldStarting.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
            this.fieldStarting.TabIndex = 2;
            this.fieldStarting.TabNavigationMode = TabNavigationMode.ActivateOnTab;
            ToolTip.Default.SetToolTip(this.fieldStarting, Res.Strings.Panel.Arc.Tooltip.Initial);

            this.fieldEnding = new Widgets.TextFieldLabel(
                this,
                Widgets.TextFieldLabel.Type.TextFieldReal
            );
            this.fieldEnding.LabelShortText = Res.Strings.Panel.Arc.Short.Final;
            this.fieldEnding.LabelLongText = Res.Strings.Panel.Arc.Long.Final;
            this.document.Modifier.AdaptTextFieldRealAngle(this.fieldEnding.TextFieldReal);
            this.fieldEnding.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
            this.fieldEnding.TabIndex = 3;
            this.fieldEnding.TabNavigationMode = TabNavigationMode.ActivateOnTab;
            ToolTip.Default.SetToolTip(this.fieldEnding, Res.Strings.Panel.Arc.Tooltip.Final);

            this.isNormalAndExtended = true;
        }

        protected void AddRadioIcon(Properties.ArcType type)
        {
            this.grid.AddRadioIcon(
                Misc.Icon(Properties.Arc.GetIconText(type)),
                Properties.Arc.GetName(type),
                (int)type,
                false
            );
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.grid.SelectionChanged -= HandleTypeChanged;
                this.fieldStarting.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
                this.fieldEnding.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;

                this.grid = null;
                this.fieldStarting = null;
                this.fieldEnding = null;
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
                        h += 80;
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

            Properties.Arc p = this.property as Properties.Arc;
            if (p == null)
                return;

            this.ignoreChanged = true;

            this.SelectButtonType = p.ArcType;
            this.fieldStarting.TextFieldReal.InternalValue = (decimal)p.StartingAngle;
            this.fieldEnding.TextFieldReal.InternalValue = (decimal)p.EndingAngle;

            this.EnableWidgets();
            this.ignoreChanged = false;
        }

        protected override void WidgetsToProperty()
        {
            //	Widgets -> propriété.
            Properties.Arc p = this.property as Properties.Arc;
            if (p == null)
                return;

            p.ArcType = this.SelectButtonType;
            p.StartingAngle = (double)this.fieldStarting.TextFieldReal.InternalValue;
            p.EndingAngle = (double)this.fieldEnding.TextFieldReal.InternalValue;
        }

        protected Properties.ArcType SelectButtonType
        {
            get { return (Properties.ArcType)this.grid.SelectedValue; }
            set { this.grid.SelectedValue = (int)value; }
        }

        protected void EnableWidgets()
        {
            //	Grise les widgets nécessaires.
            bool enable = (this.SelectButtonType != Properties.ArcType.Full);

            this.fieldStarting.Enable = (this.isExtendedSize && enable);
            this.fieldEnding.Enable = (this.isExtendedSize && enable);

            this.fieldStarting.Visibility = (this.isExtendedSize);
            this.fieldEnding.Visibility = (this.isExtendedSize);
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
                r.Offset(0, -25);
                r.Left = rect.Left;
                r.Right = rect.Right;
                this.fieldStarting.SetManualBounds(r);
                r.Offset(0, -25);
                this.fieldEnding.SetManualBounds(r);
            }
            else
            {
                double w = Widgets.TextFieldLabel.ShortWidth + 10;

                r.Offset(0, -25);
                r.Left = rect.Right - w - w;
                r.Width = w;
                this.fieldStarting.SetManualBounds(r);
                r.Left = r.Right;
                r.Width = w;
                this.fieldEnding.SetManualBounds(r);
            }
        }

        private void HandleFieldChanged(object sender)
        {
            //	Un champ a été changé.
            if (this.ignoreChanged)
                return;
            this.OnChanged();
        }

        private void HandleTypeChanged(object sender)
        {
            //	Une valeur a été changée.
            if (this.ignoreChanged)
                return;
            this.SelectButtonType = (Properties.ArcType)this.grid.SelectedValue;
            this.EnableWidgets();
            this.OnChanged();
        }

        protected RadioIconGrid grid;
        protected Widgets.TextFieldLabel fieldStarting;
        protected Widgets.TextFieldLabel fieldEnding;
    }
}
