using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.PanelEditor
{
	/// <summary>
	/// Description d'une poignée pour PanelEditor.
	/// </summary>
	public class Handle
	{
		public enum Type
		{
			None,
			BottomLeft,
			BottomRight,
			TopLeft,
			TopRight,
			Bottom,
			Top,
			Left,
			Right,
		}

		public enum Glyph
		{
			Hide,
			Square,
			ArrowLeft,
			ArrowRight,
			ArrowDown,
			ArrowUp,
		}

		public Handle(Type type)
		{
			//	Crée une poignée, sans préciser la position.
			this.type = type;
			this.isHilite = false;
		}

		public Type HandleType
		{
			//	Retourne le type d'une poignée.
			get
			{
				return this.type;
			}
		}

		public Glyph GlyphType
		{
			//	Forme de la poignée.
			get
			{
				return this.glyph;
			}
			set
			{
				this.glyph = value;
			}
		}

		public Point Position
		{
			//	Position du centre de la poignée.
			get
			{
				return this.position;
			}
			set
			{
				this.position = value;
			}
		}

		public bool IsHilite
		{
			//	Poignée survolée par la souris ?
			get
			{
				return this.isHilite;
			}
			set
			{
				this.isHilite = value;
			}
		}

		public bool Detect(Point mouse)
		{
			//	Indique si la souris est dans la poignée.
			if (this.glyph == Glyph.Hide)
			{
				return false;
			}

			return this.Bounds.Contains(mouse);
		}

		public void Draw(Graphics graphics)
		{
			//	Dessine la poignée.
			if (this.glyph == Glyph.Hide)
			{
				return;
			}

			Rectangle rect = this.Bounds;
			rect = graphics.Align (rect);

			Color color = this.isHilite ? PanelsContext.ColorHandleHilited : PanelsContext.ColorHandleNormal;

			if (this.glyph == Glyph.Square)
			{
				rect.Offset(0.5, 0.5);
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(color);

				graphics.AddRectangle(rect);
				graphics.RenderSolid(Color.FromBrightness(0));
			}

			if (this.glyph == Glyph.ArrowLeft || this.glyph == Glyph.ArrowRight || this.glyph == Glyph.ArrowUp || this.glyph == Glyph.ArrowDown)
			{
				Handle.PaintTriangle(graphics, rect, this.glyph, Color.FromBrightness(0));
				rect.Deflate(1.0);
				Handle.PaintTriangle(graphics, rect, this.glyph, color);
			}
		}

		protected static void PaintTriangle(Graphics graphics, Rectangle rect, Glyph glyph, Color color)
		{
			//	Dessine un triangle orienté.
			Path path = new Path();

			if (glyph == Glyph.ArrowLeft)
			{
				path.MoveTo(rect.Left, rect.Center.Y);
				path.LineTo(rect.TopRight);
				path.LineTo(rect.BottomRight);
			}

			if (glyph == Glyph.ArrowRight)
			{
				path.MoveTo(rect.Right, rect.Center.Y);
				path.LineTo(rect.BottomLeft);
				path.LineTo(rect.TopLeft);
			}

			if (glyph == Glyph.ArrowDown)
			{
				path.MoveTo(rect.Center.X, rect.Bottom);
				path.LineTo(rect.TopLeft);
				path.LineTo(rect.TopRight);
			}

			if (glyph == Glyph.ArrowUp)
			{
				path.MoveTo(rect.Center.X, rect.Top);
				path.LineTo(rect.BottomRight);
				path.LineTo(rect.BottomLeft);
			}

			path.Close();
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(color);
		}

		public Rectangle Bounds
		{
			//	Retourne le rectangle de la poignée.
			get
			{
				Rectangle bounds = new Rectangle(this.position, this.position);
				bounds.Inflate(3.5);

				if (this.glyph == Glyph.ArrowLeft)
				{
					bounds.Inflate(1.0);
					bounds.Left -= 2.0;
				}

				if (this.glyph == Glyph.ArrowRight)
				{
					bounds.Inflate(1.0);
					bounds.Right += 2.0;
				}

				if (this.glyph == Glyph.ArrowDown)
				{
					bounds.Inflate(1.0);
					bounds.Bottom -= 2.0;
				}

				if (this.glyph == Glyph.ArrowUp)
				{
					bounds.Inflate(1.0);
					bounds.Top += 2.0;
				}

				return bounds;
			}
		}

		protected Type				type;
		protected Glyph				glyph;
		protected Point				position;
		protected bool				isHilite;
	}
}
