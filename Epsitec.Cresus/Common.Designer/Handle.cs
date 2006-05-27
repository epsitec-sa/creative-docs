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
			//	Cr�e une poign�e, sans pr�ciser la position.
			this.type = type;
			this.isHilite = false;
		}

		public Handle.Type HandleType
		{
			//	Retourne le type d'une poign�e.
			get
			{
				return this.type;
			}
		}

		public Point Position
		{
			//	Position du centre de la poign�e.
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
			//	Poign�e survol�e par la souris ?
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
				Rectangle bounds = new Rectangle(this.position, this.position);
				bounds.Inflate(3.5);
				return bounds;
			}
		}

		protected Type				type;
		protected Point				position;
		protected bool				isHilite;
	}
}
