using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Helpers;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Ce widget est un conteneur générique, qui peut être sélectionné. L'un de ses côté est
	/// alors une flèche (qui déborde de son Client.Bounds) qui pointe vers son enfant.
	/// </summary>
	public class SubFrameBox : FrameBox
	{
		public enum ChildrenLocationEnum
		{
			None,
			Left,
			Right,
			Top,
			Bottom,
		}


		public SubFrameBox()
		{
		}

		public SubFrameBox(Widget embedder)
			: this()
		{
			this.SetEmbedder(embedder);
		}


		static SubFrameBox()
		{
			DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			metadataDy.DefineDefaultValue(SubFrameBox.arrowWidth);

			Common.Widgets.Visual.PreferredHeightProperty.OverrideMetadata(typeof(SubFrameBox), metadataDy);
		}


		public ChildrenLocationEnum ChildrenLocation
		{
			get
			{
				return (ChildrenLocationEnum) this.GetValue(SubFrameBox.ChildrenLocationProperty);
			}
			set
			{
				this.SetValue(SubFrameBox.ChildrenLocationProperty, value);
			}
		}

#if false
		public double PreferredHeight
		{
			get
			{
			}
		}
#endif


		public override Margins GetShapeMargins()
		{
			switch (this.ChildrenLocation)
			{
				case ChildrenLocationEnum.Left:
					return new Margins(SubFrameBox.arrowDeep, 0, 0, 0);

				case ChildrenLocationEnum.Right:
					return new Margins(0, SubFrameBox.arrowDeep, 0, 0);

				case ChildrenLocationEnum.Top:
					return new Margins(0, 0, SubFrameBox.arrowDeep, 0);

				case ChildrenLocationEnum.Bottom:
					return new Margins(0, 0, 0, SubFrameBox.arrowDeep);

				default:
					return new Margins(0, 0, 0, 0);
			}

		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle bounds = this.Client.Bounds;
			Path path = this.FramePath;

			if (this.BackColor.IsVisible)
			{
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(this.BackColor);
			}
			else
			{
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(adorner.ColorTextBackground);
			}

			graphics.Rasterizer.AddOutline(path);
			graphics.RenderSolid(adorner.ColorBorder);

		}

		private Path FramePath
		{
			get
			{
				Path path = new Path();

				Rectangle bounds = this.Client.Bounds;
				bounds.Deflate(0.5);

				Rectangle arrowRectangle = this.ArrowRectangle;

				switch (this.ChildrenLocation)
				{
					case ChildrenLocationEnum.Left:
						path.MoveTo(Point.Scale(arrowRectangle.TopLeft, arrowRectangle.BottomLeft, 0.5));
						path.LineTo(arrowRectangle.TopRight);
						path.LineTo(bounds.TopLeft);
						path.LineTo(bounds.TopRight);
						path.LineTo(bounds.BottomRight);
						path.LineTo(bounds.BottomLeft);
						path.LineTo(arrowRectangle.BottomRight);
						path.Close();
						break;

					case ChildrenLocationEnum.Right:
						path.MoveTo(Point.Scale(arrowRectangle.TopRight, arrowRectangle.BottomRight, 0.5));
						path.LineTo(arrowRectangle.TopLeft);
						path.LineTo(bounds.TopRight);
						path.LineTo(bounds.TopLeft);
						path.LineTo(bounds.BottomLeft);
						path.LineTo(bounds.BottomRight);
						path.LineTo(arrowRectangle.BottomLeft);
						path.Close();
						break;

					case ChildrenLocationEnum.Top:
						path.MoveTo(Point.Scale(arrowRectangle.TopLeft, arrowRectangle.TopRight, 0.5));
						path.LineTo(arrowRectangle.BottomRight);
						path.LineTo(bounds.TopRight);
						path.LineTo(bounds.BottomRight);
						path.LineTo(bounds.BottomLeft);
						path.LineTo(bounds.TopLeft);
						path.LineTo(arrowRectangle.BottomLeft);
						path.Close();
						break;

					case ChildrenLocationEnum.Bottom:
						path.MoveTo(Point.Scale(arrowRectangle.BottomLeft, arrowRectangle.BottomRight, 0.5));
						path.LineTo(arrowRectangle.TopRight);
						path.LineTo(bounds.BottomRight);
						path.LineTo(bounds.TopRight);
						path.LineTo(bounds.TopLeft);
						path.LineTo(bounds.BottomLeft);
						path.LineTo(arrowRectangle.TopLeft);
						path.Close();
						break;

					default:
						path.AppendRectangle(bounds);
						break;
				}

				return path;
			}
		}

		private Rectangle ArrowRectangle
		{
			get
			{
				Rectangle bounds = this.Client.Bounds;
				Point p;
				double width;

				switch (this.ChildrenLocation)
				{
					case ChildrenLocationEnum.Left:
						p = Point.Scale(bounds.TopLeft, bounds.BottomLeft, 0.5);
						width = System.Math.Min(SubFrameBox.arrowWidth, bounds.Height);
						return new Rectangle(bounds.Left-SubFrameBox.arrowDeep, p.Y-width/2, SubFrameBox.arrowDeep, width);

					case ChildrenLocationEnum.Right:
						p = Point.Scale(bounds.TopRight, bounds.BottomRight, 0.5);
						width = System.Math.Min(SubFrameBox.arrowWidth, bounds.Height);
						return new Rectangle(bounds.Right, p.Y-width/2, SubFrameBox.arrowDeep, width);

					case ChildrenLocationEnum.Top:
						p = Point.Scale(bounds.TopLeft, bounds.TopRight, 0.5);
						width = System.Math.Min(SubFrameBox.arrowWidth, bounds.Width);
						return new Rectangle(p.X-width/2, bounds.Top, width, SubFrameBox.arrowDeep);

					case ChildrenLocationEnum.Bottom:
						p = Point.Scale(bounds.BottomLeft, bounds.BottomRight, 0.5);
						width = System.Math.Min(SubFrameBox.arrowWidth, bounds.Width);
						return new Rectangle(p.X-width/2, bounds.Bottom-SubFrameBox.arrowDeep, width, SubFrameBox.arrowDeep);

					default:
						return Rectangle.Empty;
				}
			}
		}

	
		private static readonly double arrowWidth = 24;
		private static readonly double arrowDeep = 8;

		public static readonly DependencyProperty ChildrenLocationProperty = DependencyProperty.Register("ChildrenLocation", typeof(ChildrenLocationEnum), typeof(Visual), new VisualPropertyMetadata(ChildrenLocationEnum.None, VisualPropertyMetadataOptions.AffectsDisplay));
	}
}
