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


namespace Epsitec.Common.Drawing.Renderers
{
    public sealed class Solid : IRenderer, System.IDisposable
    {
        public Solid(AntigrainSharp.Renderer.Solid solidRenderer)
        {
            this.solidRenderer = solidRenderer;
            this.AlphaMutiplier = 1.0;
        }

        ~Solid()
        {
            this.Dispose();
        }

        public Color Color
        {
            get { return this.color; }
            set
            {
                if (this.color != value)
                {
                    this.color = value;
                    this.SetColor(value);
                }
            }
        }

        public double AlphaMutiplier { get; set; }

        public void Clear(Color color)
        {
            if (color.IsValid)
            {
                this.ClearAlphaRgb(color.A, color.R, color.G, color.B);
            }
        }

        public void Clear(double r, double g, double b)
        {
            this.ClearAlphaRgb(1, r, g, b);
        }

        public void ClearAlphaRgb(double a, double r, double g, double b)
        {
            this.solidRenderer.Clear(r, g, b, a * this.AlphaMutiplier);
        }

        public void Clear4Colors(
            int x,
            int y,
            int dx,
            int dy,
            Color c1,
            Color c2,
            Color c3,
            Color c4
        )
        {
            this.solidRenderer.Fill4Colors(
                x,
                y,
                dx,
                dy,
                c1.R,
                c1.G,
                c1.B,
                c2.R,
                c2.G,
                c2.B,
                c3.R,
                c3.G,
                c3.B,
                c4.R,
                c4.G,
                c4.B
            );
        }

        public void SetColor(Color color)
        {
            if (color.IsEmpty)
            {
                this.SetColorAlphaRgb(0, 0, 0, 0);
            }
            else
            {
                this.SetColorAlphaRgb(color.A, color.R, color.G, color.B);
            }
        }

        public void SetColor(double r, double g, double b)
        {
            this.SetColorAlphaRgb(1, r, g, b);
        }

        public void SetColorAlphaRgb(double a, double r, double g, double b)
        {
            this.solidRenderer.Color(r, g, b, a * this.AlphaMutiplier);
        }

        public void SetAlphaMask(DrawingBitmap mask, MaskComponent component)
        {
            this.DestroyAlphaMask();
            System.Console.WriteLine($"SetAlphaMask {mask}");
            this.alphaMask = mask;
            this.solidRenderer.SetAlphaMask(
                this.alphaMask.buffer,
                (AntigrainSharp.Renderer.MaskComponent)component
            );
        }

        private void DestroyAlphaMask()
        {
            if (this.alphaMask != null)
            {
                System.Console.WriteLine($"DestroyAlphaMask {this.alphaMask}");
                this.alphaMask.Dispose();
                this.alphaMask = null;
            }
        }

        #region IDisposable Members
        public void Dispose()
        {
            this.DestroyAlphaMask();
            System.GC.SuppressFinalize(this);
        }
        #endregion

        private Color color;
        internal readonly AntigrainSharp.Renderer.Solid solidRenderer;
        private DrawingBitmap alphaMask;
    }
}
