//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// The <c>AbstractRasterizer</c> is the base class used by all rasterizer
	/// implementations (currently, there is just one such implementation).
	/// See the <see cref="Rasterizer"/> class for details.
	/// </summary>
	public abstract class AbstractRasterizer : System.IDisposable
	{
		/// <summary>
		/// Gets or sets the surface fill mode.
		/// </summary>
		/// <value>The fill mode.</value>
		public FillMode							FillMode
		{
			get
			{
				return this.fillMode;
			}
			set
			{
				if (this.fillMode != value)
				{
					this.fillMode = value;
					this.SyncFillMode ();
				}
			}
		}

		/// <summary>
		/// Gets or sets the gamma used by the anti-aliasing algorithm.
		/// The gamma defines how luminosity is perceived by the user
		/// in order to produce a visually uniform distribution of
		/// intensity in the gray levels.
		/// </summary>
		/// <value>The gamma.</value>
		public double							Gamma
		{
			get
			{
				return this.gamma;
			}
			set
			{
				if (this.gamma != value)
				{
					this.gamma = value;
					this.SyncGamma ();
				}
			}
		}

		/// <summary>
		/// Gets or sets the transform associated with this rasterizer.
		/// </summary>
		/// <value>The transform.</value>
		public Transform						Transform
		{
			get
			{
				return this.transform;
			}
			set
			{
				if (value == null)
				{
					throw new System.NullReferenceException ("Rasterizer.Transform");
				}
				
				if (this.transform != value)
				{
					this.transform = value;
					this.SyncTransform ();
				}
			}
		}


		/// <summary>
		/// Sets the clip box for this rasterizer, specified in destination
		/// pixel coordinates (without any transform matrix). 
		/// </summary>
		/// <param name="x1">The leftmost pixel included in the clip box.</param>
		/// <param name="y1">The bottommost pixel included in the clip box.</param>
		/// <param name="x2">The rightmost pixel included in the clip box.</param>
		/// <param name="y2">The topmost pixel included in the clip box.</param>
		public abstract void SetClipBox(double x1, double y1, double x2, double y2);

		/// <summary>
		/// Resets the clip box to an infinite surface.
		/// </summary>
		public abstract void ResetClipBox();

		/// <summary>
		/// Adds the surface; this will generate coverage information for the
		/// surface specified by the path.
		/// </summary>
		/// <param name="path">The path.</param>
		public abstract void AddSurface(Path path);

		/// <summary>
		/// Adds the outline; this will generate coverage information for the
		/// outline specified by the path, using default join and cap styles.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="width">The outline width.</param>
		public abstract void AddOutline(Path path, double width);

		/// <summary>
		/// Adds the outline; this will generate coverage information for the
		/// outline specified by the path, using a one pixel wide line.
		/// </summary>
		/// <param name="path">The path.</param>
		public void AddOutline(Path path)
		{
			this.AddOutline (path, 1);
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
		public abstract void AddOutline(Path path, double width, CapStyle cap, JoinStyle join, double miterLimit);

		/// <summary>
		/// Adds the outline; this will generate coverage information for the
		/// outline specified by the path. A default miter limit of <c>4</c>
		/// will be used.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="width">The outline width.</param>
		/// <param name="cap">The line cap.</param>
		/// <param name="join">The line join.</param>
		public void AddOutline(Path path, double width, CapStyle cap, JoinStyle join)
		{
			this.AddOutline (path, width, cap, join, 4.0);
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
		public abstract void AddGlyph(Font font, int glyph, double x, double y, double scale);

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
		public abstract void AddGlyph(Font font, int glyph, double x, double y, double scale, double sx, double sy);

		/// <summary>
		/// Adds the glyphs; this will call <see cref="AddGlyph"/> repeatedly for
		/// every glyph specified in the array.
		/// </summary>
		/// <param name="font">The font.</param>
		/// <param name="scale">The font scale.</param>
		/// <param name="glyphs">The glyph indices.</param>
		/// <param name="x">The x coordinates.</param>
		/// <param name="y">The y coordinates.</param>
		/// <param name="sx">The horizontal glyph stretch ratios or <c>null</c>.</param>
		public void AddGlyphs(Font font, double scale, ushort[] glyphs, double[] x, double[] y, double[] sx)
		{
			if (font.IsSynthetic)
			{
				//	When using a synthetic font, we don't use the highly optimized
				//	AddPlainGlyphs method, but position every glyph individually.
				
				//	A synthetic font is one with a skewed transform (e.g. used to
				//	produce an oblique).
				
				if (sx == null)
				{
					for (int i = 0; i < glyphs.Length; i++)
					{
						this.AddGlyph (font, glyphs[i], x[i], y[i], scale);
					}
				}
				else
				{
					for (int i = 0; i < glyphs.Length; i++)
					{
						this.AddGlyph (font, glyphs[i], x[i], y[i], scale, sx[i], 1.0);
					}
				}
			}
			else
			{
				this.AddPlainGlyphs (font, scale, glyphs, x, y, sx);
			}
		}


		/// <summary>
		/// Adds the text; this will call <see cref="AddGlyph"/> repeatedly
		/// for every character of the text.
		/// </summary>
		/// <param name="font">The font.</param>
		/// <param name="text">The text.</param>
		/// <param name="x">The x coordinate of the first character.</param>
		/// <param name="y">The y coordinate of the first character.</param>
		/// <param name="scale">The font scale (or font point size).</param>
		/// <returns>The advance width of the text.</returns>
		public double AddText(Font font, string text, double x, double y, double scale)
		{
			if (font.IsSynthetic)
			{
				Transform ft = font.SyntheticTransform;

				ft = new Transform (ft.XX * scale, ft.XY * scale, ft.YX * scale, ft.YY * scale, x, y);

				switch (font.SyntheticFontMode)
				{
					case SyntheticFontMode.Oblique:
						return this.AddPlainText (font, text, ft.XX, ft.XY, ft.YX, ft.YY, ft.TX, ft.TY);

					default:
						break;
				}
			}

			return this.AddPlainText (font, text, scale, 0, 0, scale, x, y) * scale;
		}

		
		/// <summary>
		/// Renders the pixels based on the accumulated coverage information,
		/// using the specified solid renderer.
		/// </summary>
		/// <param name="renderer">The renderer.</param>
		public abstract void Render(Renderers.Solid renderer);

		/// <summary>
		/// Renders the pixels based on the accumulated coverage information,
		/// using the specified image renderer.
		/// </summary>
		/// <param name="renderer">The renderer.</param>
		public abstract void Render(Renderers.Image renderer);

		/// <summary>
		/// Renders the pixels based on the accumulated coverage information,
		/// using the specified gradient renderer.
		/// </summary>
		/// <param name="renderer">The renderer.</param>
		public abstract void Render(Renderers.Gradient renderer);

		/// <summary>
		/// Tests if the specified point has associated coverage information.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <returns><c>true</c> if the specified point has associated coverage
		/// information; otherwise, <c>false</c>.</returns>
		public abstract bool HitTest(double x, double y);

		
		/// <summary>
		/// Adds the glyphs specified by their indices.
		/// </summary>
		/// <param name="font">The font.</param>
		/// <param name="scale">The font scale.</param>
		/// <param name="glyphs">The glyph indices.</param>
		/// <param name="x">The x coordinates.</param>
		/// <param name="y">The y coordinates.</param>
		/// <param name="sx">The horizontal glyph stretch ratio or <c>null</c>.</param>
		protected abstract void AddPlainGlyphs(Font font, double scale, ushort[] glyphs, double[] x, double[] y, double[] sx);

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
		protected abstract void AddPlainGlyphs(Font font, ushort[] glyphs, double[] x, double xx, double xy, double yx, double yy, double tx, double ty);

		/// <summary>
		/// Adds the text; this will call <see cref="AddPlainGlyph"/> repeatedly
		/// for every character of the text.
		/// </summary>
		/// <param name="font">The font.</param>
		/// <param name="text">The text.</param>
		/// <param name="xx">The xx element of the transform matrix.</param>
		/// <param name="xy">The xy element of the transform matrix.</param>
		/// <param name="yx">The yx element of the transform matrix.</param>
		/// <param name="yy">The yy element of the transform matrix.</param>
		/// <param name="tx">The tx element of the transform matrix.</param>
		/// <param name="ty">The ty element of the transform matrix.</param>
		/// <returns>The unscaled text advance width.</returns>
		protected double AddPlainText(Font font, string text, double xx, double xy, double yx, double yy, double tx, double ty)
		{
			ushort[] glyphs  = font.OpenTypeFont.GenerateGlyphs (text);
			double[] x       = new double[glyphs.Length];
			double   advance = font.OpenTypeFont.GetPositions (glyphs, 1.0, 0.0, x);
			
			this.AddPlainGlyphs (font, glyphs, x, xx, xy, yx, yy, tx, ty);
			
			return advance;
		}
		
		#region IDisposable Members
		
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		
		#endregion
		
		protected abstract void Dispose(bool disposing);

		protected abstract void SyncFillMode();
		protected abstract void SyncGamma();
		protected abstract void SyncTransform();

		protected FillMode						fillMode  = FillMode.NonZero;
		protected double						gamma     = 1.0;
		protected Transform						transform = Transform.Identity;
	}
}
