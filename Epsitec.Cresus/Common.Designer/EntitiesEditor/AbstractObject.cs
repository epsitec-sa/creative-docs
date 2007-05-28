using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using System.Runtime.Serialization;

namespace Epsitec.Common.Designer.EntitiesEditor
{
	/// <summary>
	/// La classe AbstractObject est la classe de base des objets graphiques.
	/// </summary>
	public abstract class AbstractObject
	{
		public AbstractObject(Editor editor)
		{
			//	Constructeur.
			this.editor = editor;
		}


		public Rectangle Bounds
		{
			//	Boîte de l'objet.
			//	Attention: le dessin peut déborder, par exemple pour l'ombre.
			get
			{
				return this.bounds;
			}
			set
			{
				this.bounds = value;
			}
		}

		public virtual void Draw(Graphics graphics)
		{
			//	Dessine l'objet.
		}


		protected void PaintShadow(Graphics graphics, Rectangle rect, double radius, int smooth, double alpha)
		{
			//	Dessine une ombre douce.
			alpha /= smooth;

			for (int i=0; i<smooth; i++)
			{
				Path path = this.PathRoundRectangle(rect, radius);
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(Color.FromAlphaRgb(alpha, 0, 0, 0));

				rect.Deflate(1);
				radius -= 1;
			}
		}

		protected void RenderHorizontalGradient(Graphics graphics, Rectangle rect, Color leftColor, Color rightColor)
		{
			//	Peint la surface avec un dégradé horizontal.
			graphics.FillMode = FillMode.NonZero;
			graphics.GradientRenderer.Fill = GradientFill.X;
			graphics.GradientRenderer.SetColors(leftColor, rightColor);
			graphics.GradientRenderer.SetParameters(-100, 100);
			
			Transform ot = graphics.GradientRenderer.Transform;
			Transform t = new Transform();
			Point center = rect.Center;
			t.Scale(rect.Width/100/2, rect.Height/100/2);
			t.Translate(center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient();
			graphics.GradientRenderer.Transform = ot;
		}

		protected void RenderVerticalGradient(Graphics graphics, Rectangle rect, Color bottomColor, Color topColor)
		{
			//	Peint la surface avec un dégradé vertical.
			graphics.FillMode = FillMode.NonZero;
			graphics.GradientRenderer.Fill = GradientFill.Y;
			graphics.GradientRenderer.SetColors(bottomColor, topColor);
			graphics.GradientRenderer.SetParameters(-100, 100);
			
			Transform ot = graphics.GradientRenderer.Transform;
			Transform t = new Transform();
			Point center = rect.Center;
			t.Scale(rect.Width/100/2, rect.Height/100/2);
			t.Translate(center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient();
			graphics.GradientRenderer.Transform = ot;
		}

		protected Path PathRoundRectangle(Rectangle rect, double radius)
		{
			//	Retourne le chemin d'un rectangle à coins arrondis.
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			radius = System.Math.Min(radius, System.Math.Min(dx, dy)/2);

			Path path = new Path();
			path.MoveTo (ox+radius, oy);
			path.LineTo (ox+dx-radius, oy);
			path.CurveTo(ox+dx, oy, ox+dx, oy+radius);
			path.LineTo (ox+dx, oy+dy-radius);
			path.CurveTo(ox+dx, oy+dy, ox+dx-radius, oy+dy);
			path.LineTo (ox+radius, oy+dy);
			path.CurveTo(ox, oy+dy, ox, oy+dy-radius);
			path.LineTo (ox, oy+radius);
			path.CurveTo(ox, oy, ox+radius, oy);
			path.Close();

			return path;
		}


		protected Editor editor;
		protected Rectangle bounds;
	}
}
