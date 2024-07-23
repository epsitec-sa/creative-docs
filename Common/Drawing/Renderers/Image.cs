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
    public sealed class Image : IRenderer
    {
        public Image(Graphics graphics, AntigrainSharp.Renderer.Image imageRenderer)
        {
            this.graphics = graphics;
            this.imageRenderer = imageRenderer;
        }

        public Transform Transform
        {
            get { return this.transform; }
            set
            {
                //	Note: on recalcule la transformation à tous les coups, parce que l'appelant peut être
                //	Graphics.UpdateTransform...

                this.transform = value;
                this.internalTransform = value.MultiplyBy(this.graphics.Transform);

                Transform inverse = Transform.Inverse(this.internalTransform);

                this.imageRenderer.Matrix(
                    inverse.XX,
                    inverse.XY,
                    inverse.YX,
                    inverse.YY,
                    inverse.TX,
                    inverse.TY
                );
            }
        }

        public void AttachBitmap(Bitmap bitmap)
        {
            this.imageRenderer.AttachSource(
                bitmap.GetPixelBuffer(),
                (int)bitmap.Width,
                (int)bitmap.Height,
                bitmap.Stride // negative stride value flips the image
            );
        }

        public void SetAlphaMask(DrawingBitmap pixmap, MaskComponent component)
        {
            /*
            this.imageRenderer.SetAlphaMask(
                (pixmap == null) ? System.IntPtr.Zero : pixmap.Handle,
                (AntigrainSharp.Renderer.MaskComponent)component
            );
            */
            throw new System.NotImplementedException();
        }

        public void SelectAdvancedFilter(ImageFilteringMode mode, double radius)
        {
            this.imageRenderer.SetStretchMode((int)mode, radius);
        }

        readonly Graphics graphics;
        internal readonly AntigrainSharp.Renderer.Image imageRenderer;

        private Transform transform = Transform.Identity;
        private Transform internalTransform = Transform.Identity;
    }
}
