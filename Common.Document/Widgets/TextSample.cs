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

namespace Epsitec.Common.Document.Widgets
{
    /// <summary>
    /// La classe TextSample est un widget affichant un échantillon d'une propriété de texte.
    /// </summary>
    public class TextSample : AbstractSample
    {
        public TextSample()
            : base() { }

        public TextSample(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public Text.TextStyle TextStyle
        {
            //	Style représenté.
            get { return this.textStyle; }
            set
            {
                if (this.textStyle != value)
                {
                    this.textStyle = value;
                    this.Invalidate();
                }
            }
        }

        protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
        {
            //	Dessine l'échantillon.
            if (this.document == null)
                return;

            IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
            Drawing.Rectangle rect = this.Client.Bounds;

            if (
                this.textStyle == null
                || !this.document.Wrappers.IsStyleAsDefaultParent(this.textStyle)
            )
            {
                rect.Deflate(0.5);
                Color color = Drawing.Color.FromAlphaColor(0.3, adorner.ColorBorder);

                graphics.AddLine(rect.BottomLeft, rect.TopRight); // dessine une croix
                graphics.RenderSolid(color);

                graphics.AddLine(rect.TopLeft, rect.BottomRight);
                graphics.RenderSolid(color);
            }
            else
            {
                this.PaintSample(graphics, rect);
            }
        }

        protected void PaintSample(Graphics graphics, Drawing.Rectangle rect)
        {
            if (rect.IsEmpty)
                return;

            Drawing.Rectangle iClip = graphics.SaveClippingRectangle();
            graphics.SetClippingRectangle(this.MapClientToRoot(rect));

            double h = rect.Height;
            rect.Deflate(rect.Height * 0.05);
            rect.Bottom -= rect.Height * 10; // hauteur presque infinie

            double scale = 1.0 / 7.0;
            Transform initial = graphics.Transform;
            graphics.ScaleTransform(scale, scale, 0.0, 0.0);
            rect.Scale(1.0 / scale);
            h *= 1.0 / scale;

            Document document = this.document.DocumentForSamples;
            using (document.Modifier.DisableOpletQueue())
            {
                if (this.textStyle.TextStyleClass == Common.Text.TextStyleClass.Paragraph)
                {
                    Objects.TextBox2 obj = this.document.ObjectForSamplesParagraph;
                    obj.RectangleToSample(rect);
                    obj.SampleDefineStyle(this.textStyle);

                    Shape[] shapes = obj.ShapesBuild(graphics, null, false);

                    Drawer drawer = new Drawer(document);
                    drawer.DrawShapes(graphics, null, obj, Drawer.DrawShapesMode.OnlyText, shapes);
                }

                if (this.textStyle.TextStyleClass == Common.Text.TextStyleClass.Text)
                {
                    Point p1 = rect.TopLeft;
                    Point p2 = rect.TopRight;
                    p1.Y -= h * 0.7;
                    p2.Y -= h * 0.7;

                    double r = 12 * Modifier.FontSizeScale;
                    graphics.LineWidth = 1.0;
                    graphics.AddLine(p1.X - 10, p1.Y, p2.X + 10, p2.Y);
                    graphics.AddLine(p1.X - 10, p1.Y + r, p2.X + 10, p2.Y + r);
                    graphics.RenderSolid(Color.FromRgb(1, 0, 0)); // rouge

                    Objects.TextLine2 obj = this.document.ObjectForSamplesCharacter;
                    obj.RectangleToSample(p1, p2);
                    obj.SampleDefineStyle(this.textStyle);

                    Shape[] shapes = obj.ShapesBuild(graphics, null, false);

                    Drawer drawer = new Drawer(document);
                    drawer.DrawShapes(graphics, null, obj, Drawer.DrawShapesMode.All, shapes);
                }

                graphics.Transform = initial;
                graphics.RestoreClippingRectangle(iClip);
            }
        }

        protected Text.TextStyle textStyle;
    }
}
