//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Runtime.CompilerServices;

namespace Epsitec.Common.Drawing.Renderers
{
    public sealed class Image : IRenderer, System.IDisposable
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

        public Pixmap Pixmap
        {
            set
            {
                if (this.pixmap != value)
                {
                    if (value == null)
                    {
                        this.BitmapImage = null;
                        this.Detach();
                        this.transform = Transform.Identity;
                    }
                    else
                    {
                        this.Attach(value);
                    }
                }
            }
        }

        public Drawing.Image BitmapImage
        {
            get { return this.image; }
            set
            {
                // bl-net8-cross
                //if (this.image == value)
                //{
                //    return;
                //}
                //if (this.bitmap != null)
                //{
                //    if (this.bitmapNeedsUnlock)
                //    {
                //        this.bitmap.UnlockBits();
                //    }

                //    this.bitmap = null;
                //    this.bitmapNeedsUnlock = false;

                //    this.imageRenderer.Source2(System.IntPtr.Zero, 0, 0, 0);
                //}

                //this.image = value;

                //if (this.image == null)
                //{
                //    return;
                //}

                //this.bitmap = this.image.BitmapImage;
                //this.bitmapNeedsUnlock = !this.bitmap.IsLocked;

                //int width = this.bitmap.PixelWidth;
                //int height = this.bitmap.PixelHeight;

                //if (this.bitmapNeedsUnlock)
                //{
                //    this.bitmap.LockBits();
                //}

                //this.imageRenderer.Source2(
                //    this.bitmap.Scan0,
                //    width,
                //    height,
                //    -this.bitmap.Stride
                //);
            }
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

        #region IDisposable Members
        public void Dispose()
        {
            this.Detach();
            this.BitmapImage = null;
        }
        #endregion

        private void Attach(Pixmap pixmap)
        {
            this.Detach();

            this.transform = Transform.Identity;
            this.pixmap = pixmap;
        }

        private void Detach()
        {
            if (this.pixmap != null)
            {
                this.pixmap = null;
            }
        }

        readonly Graphics graphics;
        internal readonly AntigrainCPP.Renderer.Image imageRenderer;
        private Pixmap pixmap;
        private Drawing.Image image;
        private Drawing.Bitmap bitmap;
        private bool bitmapNeedsUnlock;

        private Transform transform = Transform.Identity;
        private Transform internalTransform = Transform.Identity;
    }
}
