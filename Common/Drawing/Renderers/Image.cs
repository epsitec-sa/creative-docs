//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


namespace Epsitec.Common.Drawing.Renderers
{
    public sealed class Image : IRenderer
    {
        public Image(Graphics graphics, AntigrainCPP.Renderer.Image imageRenderer)
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
                -bitmap.Stride // negative stride value flips the image
            );
        }

        public void SetAlphaMask(Pixmap pixmap, MaskComponent component)
        {
            /*
            this.imageRenderer.SetAlphaMask(
                (pixmap == null) ? System.IntPtr.Zero : pixmap.Handle,
                (AntigrainCPP.Renderer.MaskComponent)component
            );
            */
            throw new System.NotImplementedException();
        }

        public void SelectAdvancedFilter(ImageFilteringMode mode, double radius)
        {
            this.imageRenderer.SetStretchMode((int)mode, radius);
        }

        readonly Graphics graphics;
        internal readonly AntigrainCPP.Renderer.Image imageRenderer;

        private Transform transform = Transform.Identity;
        private Transform internalTransform = Transform.Identity;
    }
}
