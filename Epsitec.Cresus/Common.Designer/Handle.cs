using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Description d'une poign�e pour PanelEditor.
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

		public Handle(Type type)
		{
			this.type = type;
			this.isHilite = false;
		}

		public Handle.Type HandleType
		{
			get
			{
				return this.type;
			}
		}

		public Point Position
		{
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
			//	Indique si la souris est dans la poign�e.
			return this.Bounds.Contains(mouse);
		}

		public void Draw(Graphics graphics)
		{
			//	Dessine la poign�e.
			Rectangle rect = this.Bounds;
			graphics.Align(ref rect);
			rect.Offset(0.5, 0.5);

			graphics.AddFilledRectangle(rect);
			if (this.isHilite)
			{
				graphics.RenderSolid(PanelsContext.ColorHandleHilited);
			}
			else
			{
				graphics.RenderSolid(PanelsContext.ColorHandleNormal);
			}

			graphics.AddRectangle(rect);
			graphics.RenderSolid(Color.FromBrightness(0));
		}

		public Rectangle Bounds
		{
			//	Retourne le rectangle de la poign�e.
			get
			{
				Point dim = new Point(3.5, 3.5);
				return new Rectangle(this.position-dim, this.position+dim);
			}
		}

		protected Type				type;
		protected Point				position;
		protected bool				isHilite;
	}
}
