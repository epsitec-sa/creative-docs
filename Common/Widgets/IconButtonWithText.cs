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


namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// La classe IconButtonWithText permet de dessiner de petits pictogrammes avec un texte
    /// court dessous, en particulier pour remplir une ToolBar.
    /// </summary>
    public class IconButtonWithText : IconButton
    {
        public IconButtonWithText()
        {
            this.textLayout = new TextLayout();
            this.textLayout.DefaultFontSize = 9.0;
            this.textLayout.Alignment = Drawing.ContentAlignment.MiddleCenter;
            this.textLayout.BreakMode =
                Drawing.TextBreakMode.Ellipsis
                | Drawing.TextBreakMode.Split
                | Drawing.TextBreakMode.SingleLine;

            this.ButtonStyle = ButtonStyle.ToolItem;
        }

        public IconButtonWithText(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public IconButtonWithText(Command command)
            : this()
        {
            this.CommandObject = command;
        }

        public IconButtonWithText(Command command, Drawing.Size size, DockStyle dock)
            : this(command)
        {
            this.PreferredSize = size;
            this.PreferredIconSize = size;
            this.Dock = dock;
        }

        public IconButtonWithText(string icon)
            : this()
        {
            this.IconUri = icon;
        }

        public IconButtonWithText(string command, string icon)
            : this(icon)
        {
            this.CommandObject = Command.Get(command);
        }

        public IconButtonWithText(string command, string icon, string name)
            : this(command, icon)
        {
            this.Name = name;
        }

        public int MaxAdditionnalWidth
        {
            //	Largeur maximale autorisée, si le texte est trop grand.
            //	C'est un peu du bricolage, dans la mesure où cette propriété doit être appelée
            //	après avoir donné la commande (CommandObject, qui détermine le texte à afficher)
            //	et après avoir donné la taille préférencielle (PreferredSize).
            get { return this.maxAdditionnalWidth; }
            set
            {
                if (this.maxAdditionnalWidth != value)
                {
                    this.maxAdditionnalWidth = value;
                    this.ComputeWidth();
                    this.Invalidate();
                }
            }
        }

        private void ComputeWidth()
        {
            if (this.CommandObject != null)
            {
                this.textLayout.Text = this.CommandObject.Caption.DefaultLabel;
                double hopeWidth = System.Math.Floor(this.textLayout.GetSingleLineSize().Width + 4);

                if (
                    hopeWidth > this.PreferredSize.Width
                    && hopeWidth <= this.PreferredSize.Width + this.maxAdditionnalWidth
                )
                {
                    this.PreferredSize = new Drawing.Size(hopeWidth, this.PreferredSize.Height);
                }
            }
        }

        protected override double GetBaseLineVerticalOffset()
        {
            //	Remonte l'icône pour faire de la place au texte dessous.
            return 8.0; // TODO: Généraliser le calcul de la géométrie !
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

            //	Dessine le texte sous l'icône.
            // TODO: Il est ridicule de réinitialiser le TextLayout à chaque fois, mais je ne sais pas comment faire autrement.
            var textRect = new Drawing.Rectangle(rect.Left, rect.Bottom, rect.Width, 20); // TODO: Généraliser le calcul de la géométrie !
            this.textLayout.LayoutSize = textRect.Size;
            this.textLayout.Text = this.CommandObject.Caption.DefaultLabel;
            adorner.PaintGeneralTextLayout(
                graphics,
                textRect,
                textRect.BottomLeft,
                this.textLayout,
                state,
                PaintTextStyle.StaticText,
                TextFieldDisplayMode.Default,
                Drawing.Color.Empty
            );
        }

        private TextLayout textLayout;
        private int maxAdditionnalWidth;
    }
}
