using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
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
			MarginBottom,
			MarginTop,
			MarginLeft,
			MarginRight,
		}

		public Handle(Type type)
		{
			//	Crée une poignée, sans préciser la position.
			this.type = type;
			this.isHilite = false;
		}

		public Handle.Type HandleType
		{
			//	Retourne le type d'une poignée.
			get
			{
				return this.type;
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

		public bool IsMargin
		{
			//	Indique s'il s'agit d'une poignée pour une marge.
			get
			{
				return Handle.IsMarginType(this.type);
			}
		}

		public static bool IsMarginType(Type type)
		{
			//	Indique s'il s'agit d'une poignée pour une marge.
			return (type == Type.MarginBottom || type == Type.MarginTop || type == Type.MarginLeft || type == Type.MarginRight);
		}

		public bool Detect(Point mouse)
		{
			//	Indique si la souris est dans la poignée.
			return this.Bounds.Contains(mouse);
		}

		public void Draw(Graphics graphics)
		{
			//	Dessine la poignée.
			Rectangle rect = this.Bounds;
			graphics.Align(ref rect);

			Color color = this.isHilite ? PanelsContext.ColorHandleHilited : PanelsContext.ColorHandleNormal;

			if (this.IsMargin)
			{
				Handle.PaintTriangle(graphics, rect, this.type, Color.FromBrightness(0));
				rect.Deflate(1.0);
				Handle.PaintTriangle(graphics, rect, this.type, color);
			}
			else
			{
				rect.Offset(0.5, 0.5);
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(color);

				graphics.AddRectangle(rect);
				graphics.RenderSolid(Color.FromBrightness(0));
			}
		}

		protected static void PaintTriangle(Graphics graphics, Rectangle rect, Type type, Color color)
		{
			//	Dessine un triangle orienté.
			Path path = new Path();

			if (type == Type.MarginLeft)
			{
				path.MoveTo(rect.Left, rect.Center.Y);
				path.LineTo(rect.TopRight);
				path.LineTo(rect.BottomRight);
			}

			if (type == Type.MarginRight)
			{
				path.MoveTo(rect.Right, rect.Center.Y);
				path.LineTo(rect.BottomLeft);
				path.LineTo(rect.TopLeft);
			}

			if (type == Type.MarginBottom)
			{
				path.MoveTo(rect.Center.X, rect.Bottom);
				path.LineTo(rect.TopLeft);
				path.LineTo(rect.TopRight);
			}

			if (type == Type.MarginTop)
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

				if (this.type == Type.MarginLeft)
				{
					bounds.Inflate(1.0);
					bounds.Left -= 2.0;
				}

				if (this.type == Type.MarginRight)
				{
					bounds.Inflate(1.0);
					bounds.Right += 2.0;
				}

				if (this.type == Type.MarginBottom)
				{
					bounds.Inflate(1.0);
					bounds.Bottom -= 2.0;
				}

				if (this.type == Type.MarginTop)
				{
					bounds.Inflate(1.0);
					bounds.Top += 2.0;
				}

				return bounds;
			}
		}

		protected Type type;
		protected Point				position;
		protected bool				isHilite;
	}
}
