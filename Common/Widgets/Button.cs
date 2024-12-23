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


using Epsitec.Common.Types;

[assembly: Epsitec.Common.Types.DependencyClass(typeof(Epsitec.Common.Widgets.Button))]

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// La class Button représente un bouton standard.
    /// </summary>
    public class Button : AbstractButton
    {
        public Button() { }

        public Button(string text)
        {
            this.Text = text;
        }

        public Button(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public Button(Command command)
            : this()
        {
            this.CommandObject = command;
        }

        static Button()
        {
            double height = Widget.DefaultFontHeight + 10;
            Visual.PreferredHeightProperty.OverrideMetadataDefaultValue<Button>(height);
        }

        public virtual ButtonStyle ButtonStyle
        {
            get { return (ButtonStyle)this.GetValue(Button.ButtonStyleProperty); }
            set { this.SetValue(Button.ButtonStyleProperty, value); }
        }

        public double InnerZoom
        {
            get { return this.innerZoom; }
            set
            {
                if (this.innerZoom != value)
                {
                    if (value < 0.01)
                    {
                        value = 1.0;
                    }

                    this.innerZoom = value;
                    this.Invalidate();
                }
            }
        }

        public override Drawing.Margins GetShapeMargins()
        {
            if (this.ButtonStyle == ButtonStyle.ActivableIcon)
            {
                if ((this.PaintState & WidgetPaintState.ThreeState) == 0)
                {
                    return Widgets.Adorners.Factory.Active.GeometryToolShapeMargins;
                }
                else
                {
                    return Widgets.Adorners.Factory.Active.GeometryThreeStateShapeMargins;
                }
            }
            else
            {
                return base.GetShapeMargins();
            }
        }

        protected override void DefineIconFromCaption(string icon)
        {
            base.DefineIconFromCaption(icon);

            if ((string.IsNullOrEmpty(this.Text)) && (!string.IsNullOrEmpty(icon)))
            {
                this.Text = string.Concat(@"<img src=""", icon, @"""/>");
            }
        }

        protected override bool ProcessShortcut(Shortcut shortcut)
        {
            IFeel feel = Feel.Factory.Active;

            if (
                (this.ButtonStyle == ButtonStyle.DefaultAccept)
                || (this.ButtonStyle == ButtonStyle.DefaultAcceptAndCancel)
            )
            {
                if (feel.AcceptShortcut == shortcut)
                {
                    this.OnShortcutPressed();
                    return true;
                }
            }

            if (
                (this.ButtonStyle == ButtonStyle.DefaultCancel)
                || (this.ButtonStyle == ButtonStyle.DefaultAcceptAndCancel)
            )
            {
                if (feel.CancelShortcut == shortcut)
                {
                    this.OnShortcutPressed();
                    return true;
                }
            }

            return base.ProcessShortcut(shortcut);
        }

        protected override double GetBaseLineVerticalOffset()
        {
            //	Le texte dans les boutons standards doit être remonté d'un pixel
            //	pour paraître centré, mais surtout pas dans les IconButtons !

            switch (this.ButtonStyle)
            {
                case ButtonStyle.Normal:
                case ButtonStyle.DefaultAccept:
                case ButtonStyle.DefaultCancel:
                case ButtonStyle.DefaultAcceptAndCancel:
                    return 1.0;

                default:
                    return 0.0;
            }
        }

        protected override Epsitec.Common.Drawing.Size GetTextLayoutSize()
        {
            var padding = this.Padding + this.GetInternalPadding();
            var size = base.GetTextLayoutSize() - padding.Size;

            switch (this.ButtonStyle)
            {
                case ButtonStyle.Normal:
                case ButtonStyle.DefaultAccept:
                case ButtonStyle.DefaultAcceptAndCancel:
                case ButtonStyle.DefaultCancel:
                    size.Width = System.Math.Max(0, size.Width - 8);
                    size.Height = System.Math.Max(0, size.Height - 8);
                    break;
            }

            return size;
        }

        protected override Epsitec.Common.Drawing.Point GetTextLayoutOffset()
        {
            var padding = this.Padding + this.GetInternalPadding();
            var offset =
                base.GetTextLayoutOffset() + new Drawing.Point(padding.Left, padding.Bottom);

            switch (this.ButtonStyle)
            {
                case ButtonStyle.Normal:
                case ButtonStyle.DefaultAccept:
                case ButtonStyle.DefaultAcceptAndCancel:
                case ButtonStyle.DefaultCancel:
                    return offset + new Drawing.Point(4, 4);
            }

            return offset;
        }

        protected override void PaintBackgroundImplementation(
            Drawing.Graphics graphics,
            Drawing.Rectangle clipRect
        )
        {
            //	Dessine le bouton.
            var adorner = Widgets.Adorners.Factory.Active;

            var rect = this.Client.Bounds;
            var state = Widget.ConstrainPaintState(this.GetPaintState());
            var pos = this.GetTextLayoutOffset();

            if (this.BackColor.IsTransparent)
            {
                //	Ne peint pas le fond du bouton si celui-ci a un fond explicitement défini
                //	comme "transparent".
            }
            else
            {
                //	Ne reproduit pas l'état sélectionné si on peint nous-même le fond du bouton.

                state &= ~WidgetPaintState.Selected;
                adorner.PaintButtonBackground(
                    graphics,
                    rect,
                    state,
                    Direction.Down,
                    this.ButtonStyle
                );
            }

            pos.Y += this.GetBaseLineVerticalOffset();

            if (this.innerZoom != 1.0)
            {
                Drawing.Transform transform = graphics.Transform;
                graphics.ScaleTransform(
                    this.innerZoom,
                    this.innerZoom,
                    this.Client.Size.Width / 2,
                    this.Client.Size.Height / 2
                );
                adorner.PaintButtonTextLayout(
                    graphics,
                    pos,
                    this.TextLayout,
                    state,
                    this.ButtonStyle
                );
                graphics.Transform = transform;
            }
            else
            {
                adorner.PaintButtonTextLayout(
                    graphics,
                    pos,
                    this.TextLayout,
                    state,
                    this.ButtonStyle
                );
            }
        }

        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register(
            "ButtonStyle",
            typeof(ButtonStyle),
            typeof(Button),
            new Helpers.VisualPropertyMetadata(
                ButtonStyle.Normal,
                Helpers.VisualPropertyMetadataOptions.AffectsDisplay
            )
        );

        protected double innerZoom = 1.0;
    }
}
