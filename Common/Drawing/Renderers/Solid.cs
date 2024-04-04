//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing.Renderers
{
    public sealed class Solid : IRenderer, System.IDisposable
    {
        public Solid()
        {
            this.AlphaMutiplier = 1.0;
        }

        public Pixmap Pixmap
        {
            set
            {
                if (this.pixmap != value)
                {
                    if (value == null)
                    {
                        this.Detach();
                    }
                    else
                    {
                        this.Attach(value);
                    }
                }
            }
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
            /*
            AntigrainCPP.Renderer.Special.Fill4Colors(
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
            */
            throw new System.NotImplementedException();
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

        public void SetAlphaMask(Pixmap pixmap, MaskComponent component)
        {
            /*
            this.solidRenderer.SetAlphaMask(
                (pixmap == null) ? System.IntPtr.Zero : pixmap.Handle,
                (AntigrainCPP.Renderer.MaskComponent)component
            );
            */
            throw new System.NotImplementedException();
        }

        #region IDisposable Members
        public void Dispose()
        {
            this.Detach();
        }
        #endregion

        private void Attach(Pixmap pixmap)
        {
            this.Detach();

            this.pixmap = pixmap;
            this.color = new Color();
        }

        private void Detach()
        {
            if (this.pixmap != null)
            {
                this.pixmap = null;
            }
        }

        private Color color;
        private readonly AntigrainCPP.Renderer.Solid solidRenderer;
        private Pixmap pixmap;
    }
}
