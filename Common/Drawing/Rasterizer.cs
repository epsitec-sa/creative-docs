//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
    /// <summary>
    /// The <c>Rasterizer</c> transforms paths and glyphs into coverage information
    /// in AGG. This coverage information is then used by one of the renderers to
    /// produce actual pixels.
    /// </summary>
    public class Rasterizer : AbstractRasterizer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rasterizer"/> class.
        /// </summary>
        public Rasterizer()
        {
            this.rasterizer = new AntigrainCPP.Rasterizer();
        }

        /// <summary>
        /// Sets the clip box for this rasterizer, specified in destination
        /// pixel coordinates (without any transform matrix).
        /// </summary>
        /// <param name="x1">The leftmost pixel included in the clip box.</param>
        /// <param name="y1">The bottommost pixel included in the clip box.</param>
        /// <param name="x2">The rightmost pixel included in the clip box.</param>
        /// <param name="y2">The topmost pixel included in the clip box.</param>
        public override void SetClipBox(double x1, double y1, double x2, double y2)
        {
            this.rasterizer.SetClipBox(x1, y1, x2, y2);
        }

        /// <summary>
        /// Resets the clip box to an infinite surface.
        /// </summary>
        public override void ResetClipBox()
        {
            this.rasterizer.ResetClipBox();
        }

        /// <summary>
        /// Adds the surface; this will generate coverage information for the
        /// surface specified by the path.
        /// </summary>
        /// <param name="path">The path.</param>
        public override void AddSurface(Path path)
        {
            if (path != null)
            {
                /*
                this.rasterizer.AddPath(path.Handle, path.ContainsCurves);
                */
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Adds the outline; this will generate coverage information for the
        /// outline specified by the path, using default join and cap styles.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="width">The outline width.</param>
        public override void AddOutline(Path path, double width)
        {
            if (path != null)
            {
                /*
                this.rasterizer.AddPathStroke1(
                    path.Handle,
                    width,
                    path.ContainsCurves
                );
                */
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Adds the outline; this will generate coverage information for the
        /// outline specified by the path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="width">The outline width.</param>
        /// <param name="cap">The line cap.</param>
        /// <param name="join">The line join.</param>
        /// <param name="miterLimit">The miter limit.</param>
        public override void AddOutline(
            Path path,
            double width,
            CapStyle cap,
            JoinStyle join,
            double miterLimit
        )
        {
            if (path != null)
            {
                /*
                this.rasterizer.AddPathStroke2(
                    path.Handle,
                    width,
                    (int)cap,
                    (int)join,
                    miterLimit,
                    path.ContainsCurves
                );
                */
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Adds the glyph; this will generate coverage information for the
        /// specified glyph in the given font.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="glyph">The glyph index.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="scale">The font scale (or font point size).</param>
        public override void AddGlyph(Font font, int glyph, double x, double y, double scale)
        {
            /*

            if (font.IsSynthetic)
            {
                Transform ft = font.SyntheticTransform;

                ft = new Transform(ft.XX, ft.XY, ft.YX, ft.YY, x, y);

                switch (font.SyntheticFontMode)
                {
                    case SyntheticFontMode.Oblique:
                        ft = ft.MultiplyBy(this.transform);
                        this.rasterizer.SetTransform(
                            ft.XX,
                            ft.XY,
                            ft.YX,
                            ft.YY,
                            ft.TX,
                            ft.TY
                        );
                        this.rasterizer.AddGlyph(font.Handle, glyph, 0, 0, scale);
                        this.rasterizer.SetTransform(
                            this.transform.XX,
                            this.transform.XY,
                            this.transform.YX,
                            this.transform.YY,
                            this.transform.TX,
                            this.transform.TY
                        );
                        return;

                    default:
                        break;
                }
            }

            this.rasterizer.AddGlyph(font.Handle, glyph, x, y, scale);
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Adds the glyph; this will generate coverage information for the
        /// specified glyph in the given font.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="glyph">The glyph index.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="scale">The font scale (or font point size).</param>
        /// <param name="sx">The horizontal glyph stretch ratio.</param>
        /// <param name="sy">The vertical glyph stretch ratio.</param>
        public override void AddGlyph(
            Font font,
            int glyph,
            double x,
            double y,
            double scale,
            double sx,
            double sy
        )
        {
            /*
            if ((sx == 1.0) && (sy == 1.0))
            {
                this.AddGlyph(font, glyph, x, y, scale);
            }
            else
            {
                Transform ft = font.SyntheticTransform;

                ft = new Transform(ft.XX, ft.XY, ft.YX, ft.YY, x, y);

                ft = ft.MultiplyBy(this.transform);
                ft = ft.MultiplyByPostfix(Transform.CreateScaleTransform(sx, sy));

                this.rasterizer.SetTransform(
                    ft.XX,
                    ft.XY,
                    ft.YX,
                    ft.YY,
                    ft.TX,
                    ft.TY
                );
                this.rasterizer.AddGlyph(font.Handle, glyph, 0, 0, scale);
                this.rasterizer.SetTransform(
                    this.transform.XX,
                    this.transform.XY,
                    this.transform.YX,
                    this.transform.YY,
                    this.transform.TX,
                    this.transform.TY
                );
            }
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Renders the pixels based on the accumulated coverage information,
        /// using the specified solid renderer.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        public override void Render(Renderers.Solid renderer)
        {
            /*
            this.rasterizer.RenderSolid(renderer.Handle);
            this.rasterizer.Clear();
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Renders the pixels based on the accumulated coverage information,
        /// using the specified image renderer.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        public override void Render(Renderers.Image renderer)
        {
            /*
            this.rasterizer.RenderImage(renderer.Handle);
            this.rasterizer.Clear();
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Renders the pixels based on the accumulated coverage information,
        /// using the specified gradient renderer.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        public override void Render(Renderers.Gradient renderer)
        {
            /*
            this.rasterizer.RenderGradient(renderer.Handle);
            this.rasterizer.Clear();
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Tests if the specified point has associated coverage information.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>
        /// 	<c>true</c> if the specified point has associated coverage
        /// information; otherwise, <c>false</c>.
        /// </returns>
        public override bool HitTest(double x, double y)
        {
            int xx = (int)(x + 0.5);
            int yy = (int)(y + 0.5);

            return this.rasterizer.HitTest(xx, yy);
        }

        /// <summary>
        /// Adds the glyphs specified by their indices. The offset is specified
        /// in an unscaled coordinate system and the transform matrix will be
        /// applied afterwards.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="glyphs">The glyph indices.</param>
        /// <param name="x">The array of unscaled x offsets.</param>
        /// <param name="xx">The xx element of the transform matrix.</param>
        /// <param name="xy">The xy element of the transform matrix.</param>
        /// <param name="yx">The yx element of the transform matrix.</param>
        /// <param name="yy">The yy element of the transform matrix.</param>
        /// <param name="tx">The tx element of the transform matrix.</param>
        /// <param name="ty">The ty element of the transform matrix.</param>
        protected override void AddPlainGlyphs(
            Font font,
            ushort[] glyphs,
            double[] x,
            double xx,
            double xy,
            double yx,
            double yy,
            double tx,
            double ty
        )
        {
            /*
            if ((glyphs == null) || (glyphs.Length == 0))
            {
                return;
            }

            Transform transform = new Transform(xx, xy, yx, yy, tx, ty);

            transform = transform.MultiplyBy(this.transform);
            this.rasterizer.SetTransform(
                transform.XX,
                transform.XY,
                transform.YX,
                transform.YY,
                transform.TX,
                transform.TY
            );
            this.rasterizer.AddGlyphs(font.Handle, 1.0, glyphs, x, null, null);
            this.rasterizer.SetTransform(
                this.transform.XX,
                this.transform.XY,
                this.transform.YX,
                this.transform.YY,
                this.transform.TX,
                this.transform.TY
            );
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Adds the glyphs specified by their indices.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="scale">The font scale.</param>
        /// <param name="glyphs">The glyph indices.</param>
        /// <param name="x">The x coordinates.</param>
        /// <param name="y">The y coordinates.</param>
        /// <param name="sx">The horizontal glyph stretch ratio or <c>null</c>.</param>
        protected override void AddPlainGlyphs(
            Font font,
            double scale,
            ushort[] glyphs,
            double[] x,
            double[] y,
            double[] sx
        )
        {
            /*
            this.rasterizer.AddGlyphs(font.Handle, scale, glyphs, x, y, sx);
            */
            throw new System.NotImplementedException();
        }

        protected override void SyncFillMode()
        {
            this.rasterizer.FillingRule((int)this.fillMode);
        }

        protected override void SyncGamma()
        {
            this.rasterizer.Gamma(this.gamma);
        }

        protected override void SyncTransform()
        {
            this.rasterizer.SetTransform(
                this.transform.XX,
                this.transform.XY,
                this.transform.YX,
                this.transform.YY,
                this.transform.TX,
                this.transform.TY
            );
        }

        private readonly AntigrainCPP.Rasterizer rasterizer;
    }
}
