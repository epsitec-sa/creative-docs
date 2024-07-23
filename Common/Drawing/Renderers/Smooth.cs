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
    public sealed class Smooth : IRenderer, System.IDisposable
    {
        public Smooth(Graphics graphics, AntigrainSharp.Renderer.Smooth smoothRenderer)
        {
            this.graphics = graphics;
            this.smoothRenderer = smoothRenderer;
        }

        public DrawingBitmap Pixmap
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
            set { this.smoothRenderer.Color(value.R, value.G, value.B, value.A); }
        }

        public void SetAlphaMask(DrawingBitmap pixmap, MaskComponent component)
        {
            /*
            this.smoothRenderer.SetAlphaMask(
                (pixmap == null) ? System.IntPtr.Zero : pixmap.Handle,
                (AntigrainSharp.Renderer.MaskComponent)component
            );
            */
            throw new System.NotImplementedException();
        }

        public void SetParameters(double r1, double r2)
        {
            if ((this.r1 != r1) || (this.r2 != r2))
            {
                this.r1 = r1;
                this.r2 = r2;
            }
        }

        public void AddPath(Drawing.Path path)
        {
            /*
            this.SetTransform(this.graphics.Transform);

            this.smoothRenderer.Setup(
                this.r1,
                this.r2,
                this.transform.XX,
                this.transform.XY,
                this.transform.YX,
                this.transform.YY,
                this.transform.TX,
                this.transform.TY
            );
            this.smoothRenderer.AddPath(path.Handle);
            */
            throw new System.NotImplementedException();
        }

        #region IDisposable Members
        public void Dispose()
        {
            this.Detach();
        }
        #endregion


        private void SetTransform(Transform value)
        {
            this.transform = value;
            this.smoothRenderer.Setup(
                this.r1,
                this.r2,
                this.transform.XX,
                this.transform.XY,
                this.transform.YX,
                this.transform.YY,
                this.transform.TX,
                this.transform.TY
            );
        }

        private void Attach(DrawingBitmap pixmap)
        {
            this.Detach();
            this.pixmap = pixmap;
        }

        private void Detach()
        {
            if (this.pixmap != null)
            {
                this.pixmap = null;
                this.transform = Transform.Identity;
            }
        }

        readonly Graphics graphics;
        private readonly AntigrainSharp.Renderer.Smooth smoothRenderer;
        private DrawingBitmap pixmap;
        private Transform transform = Transform.Identity;
        private double r1;
        private double r2;
    }
}
