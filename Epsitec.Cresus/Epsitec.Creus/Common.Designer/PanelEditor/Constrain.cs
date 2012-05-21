using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.PanelEditor
{
	/// <summary>
	/// Description d'une contrainte pour PanelEditor.
	/// </summary>
	public class Constrain
	{
		public enum Type
		{
			Left,		// contrainte verticale à gauche
			Right,		// contrainte verticale à droite
			Bottom,		// contrainte horizontale en bas
			Top,		// contrainte horizontale en haut
			BaseLine,	// contrainte horizontale sur la ligne de base
		}

		public Constrain(Point position, Type type, double margin)
		{
			this.position   = position;
			this.type       = type;
			this.margin     = margin;
			this.isLimit    = false;
			this.isActivate = false;
		}

		public bool IsLimit
		{
			get
			{
				return this.isLimit;
			}
			set
			{
				this.isLimit = value;
			}
		}

		public bool IsVertical
		{
			get
			{
				return (this.type == Type.Left || this.type == Type.Right);
			}
		}

		public bool IsLeft
		{
			get
			{
				return (this.type == Type.Left);
			}
		}

		public bool IsRight
		{
			get
			{
				return (this.type == Type.Right);
			}
		}

		public bool IsBottom
		{
			get
			{
				return (this.type == Type.Bottom);
			}
		}

		public bool IsTop
		{
			get
			{
				return (this.type == Type.Top);
			}
		}

		public bool IsBaseLine
		{
			get
			{
				return (this.type == Type.BaseLine);
			}
		}

		public bool IsActivate
		{
			get
			{
				return this.isActivate;
			}
			set
			{
				this.isActivate = value;
			}
		}

		public bool IsEqualTo(Constrain constrain)
		{
			//	Teste si deux contraintes sont identiques (sans tenir compte de l'activation).
			if (this.type != constrain.type)
			{
				return false;
			}

			if (this.IsVertical)
			{
				return (this.position.X == constrain.position.X);
			}
			else
			{
				return (this.position.Y == constrain.position.Y);
			}
		}

		public void Draw(Graphics graphics, Rectangle box)
		{
			//	Dessine une contrainte.
			if (this.IsVertical)
			{
				graphics.AddLine(this.position.X+0.5, box.Bottom, this.position.X+0.5, box.Top);
			}
			else
			{
				graphics.AddLine(box.Left, this.position.Y+0.5, box.Right, this.position.Y+0.5);
			}

			Color color = PanelsContext.ColorHiliteOutline;
			if (this.isLimit)
			{
				color = Color.FromAlphaRgb(0.7, 1,0,0);  // rouge transparent
			}
			if (!this.isActivate)
			{
				color = Color.FromAlphaColor(color.A*0.2, color);  // plus transparent s'il s'agit d'une contrainte inactive
			}
			graphics.RenderSolid(color);
		}

		public bool Detect(Point position)
		{
			//	Détecte si une position est proche d'une contrainte.
			if (this.IsVertical)
			{
				if (System.Math.Abs(position.X-this.position.X) <= this.margin)
				{
					return true;
				}
			}
			else
			{
				if (System.Math.Abs(position.Y-this.position.Y) <= this.margin)
				{
					return true;
				}
			}

			return false;
		}

		public Rectangle Snap(Rectangle rect, double baseLine)
		{
			//	Adapte un rectangle à une contrainte.
			if (this.IsVertical)
			{
				double adjust;
				this.AdjustX(rect, out adjust);
				rect.Offset(adjust, 0);
			}
			else
			{
				double adjust;
				this.AdjustY(rect, baseLine, out adjust);
				rect.Offset(0, adjust);
			}

			return rect;
		}

		public Point Snap(Point pos)
		{
			//	Adapte un point à une contrainte.
			if (this.IsVertical)
			{
				pos.X = this.position.X;
			}
			else
			{
				pos.Y = this.position.Y;
			}

			return pos;
		}

		public bool AdjustX(Rectangle rect, out double adjust)
		{
			//	Calcule l'ajustement horizontal nécessaire pour s'adapter à une contrainte.
			if (this.IsLeft && this.Detect(rect.BottomLeft))
			{
				adjust = this.position.X-rect.Left;
				return true;
			}

			if (this.IsRight && this.Detect(rect.BottomRight))
			{
				adjust = this.position.X-rect.Right;
				return true;
			}

			adjust = 0;
			return false;
		}

		public bool AdjustY(Rectangle rect, double baseLine, out double adjust)
		{
			//	Calcule l'ajustement vertical nécessaire pour s'adapter à une contrainte.
			if (this.IsBottom && this.Detect(rect.BottomLeft))
			{
				adjust = this.position.Y-rect.Bottom;
				return true;
			}

			if (this.IsTop && this.Detect(rect.TopLeft))
			{
				adjust = this.position.Y-rect.Top;
				return true;
			}

			if (this.IsBaseLine && this.Detect(new Point(0, rect.Bottom+baseLine)))
			{
				adjust = this.position.Y-(rect.Bottom+baseLine);
				return true;
			}

			adjust = 0;
			return false;
		}

		protected Point					position;
		protected Type					type;
		protected double				margin;
		protected bool					isLimit;
		protected bool					isActivate;
	}
}
