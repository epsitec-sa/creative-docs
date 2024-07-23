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

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// La classe Balloon est un widget affichant une "bulle" style bd, avec la queue au milieu en bas.
    /// </summary>
    public class Balloon : Widget
    {
        public Balloon()
        {
            this.distance = 10;
            this.margin = 3;
            this.hot = 0;
            this.awayMargin = 5;

            this.Padding = new Drawing.Margins(
                this.margin,
                this.margin,
                this.margin,
                this.distance + this.margin
            );

            this.backgroundColor = Color.FromName("Info");
            this.frameColor = Color.FromBrightness(0);
        }

        public Balloon(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public double Distance
        {
            //	Hauteur de la queue.
            get { return this.distance; }
            set
            {
                if (this.distance != value)
                {
                    this.distance = value;
                    this.Invalidate();
                }
            }
        }

        public double Margin
        {
            //	Marge autour des boutons contenus.
            get { return this.margin; }
            set
            {
                if (this.margin != value)
                {
                    this.margin = value;
                    this.Padding = new Drawing.Margins(
                        this.margin,
                        this.margin,
                        this.margin,
                        this.distance + this.margin
                    );
                    this.Invalidate();
                }
            }
        }

        public double Hot
        {
            //	Position horizontale du point chaud (queue), habituellement au milieu de la largeur.
            get { return this.hot; }
            set { this.hot = value; }
        }

        public double AwayMargin
        {
            //	Distance d'extinction lorsque la souris s'éloigne.
            get { return this.awayMargin; }
            set { this.awayMargin = value; }
        }

        protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
        {
            //	Dessine la "bulle" de style bd.
            Rectangle rect = this.Client.Bounds;
            rect.Deflate(0.5);

            Path path;

            if (this.hot.IsSafeNaN())
            {
                rect.Bottom += this.distance;
                path = Path.FromRectangle(rect);
            }
            else
            {
                double m = this.Client.Bounds.Left + this.hot;
                double h = this.distance;
                double w = this.distance * 0.3;

                path = new Path();
                path.MoveTo(m, rect.Bottom);
                path.LineTo(m - w, rect.Bottom + h);
                path.LineTo(rect.Left, rect.Bottom + h);
                path.LineTo(rect.Left, rect.Top);
                path.LineTo(rect.Right, rect.Top);
                path.LineTo(rect.Right, rect.Bottom + h);
                path.LineTo(m + w, rect.Bottom + h);
                path.Close();
            }

            graphics.Rasterizer.AddSurface(path);
            graphics.RenderSolid(this.backgroundColor);

            graphics.Rasterizer.AddOutline(path);
            graphics.RenderSolid(this.frameColor);
        }

        public bool IsAway(Point mouse)
        {
            //	Indique si la souris est trop loin de la mini-palette.
            Point p1 = this.MapClientToScreen(new Point(0, 0));
            Point p2 = this.MapClientToScreen(new Point(this.ActualWidth, this.ActualHeight));
            Rectangle rect = new Rectangle(p1, p2);

            double dx = System.Math.Abs(mouse.X - rect.Center.X);
            double dy = System.Math.Abs(mouse.Y - rect.Center.Y);

            if (dx > dy * (rect.Width / rect.Height))
            {
                return (dx - rect.Width / 2 > this.awayMargin);
            }
            else
            {
                return (dy - rect.Height / 2 > this.awayMargin);
            }
        }

        protected double distance;
        protected double margin;
        protected double hot;
        protected double awayMargin;
        protected Color backgroundColor;
        protected Color frameColor;
    }
}
