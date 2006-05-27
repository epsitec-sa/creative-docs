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
		}

		public Handle(Type type, Point position)
		{
			this.type = type;
			this.position = position;
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

		public bool Detect(Point mouse)
		{
			return this.Bounds.Contains(mouse);
		}

		public void Draw(Graphics graphics)
		{
			Rectangle rect = this.Bounds;
			graphics.Align(ref rect);
			rect.Offset(0.5, 0.5);

			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(Color.FromRgb(1, 0, 0));

			graphics.AddRectangle(rect);
			graphics.RenderSolid(Color.FromBrightness(0));
		}

		protected Rectangle Bounds
		{
			get
			{
				Point dim = new Point(3.5, 3.5);
				return new Rectangle(this.position-dim, this.position+dim);
			}
		}

		protected Type				type;
		protected Point				position;
	}
}
