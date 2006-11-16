//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Helpers;

[assembly: DependencyClass (typeof(FrameBox))]

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>FrameBox</c> class is a very simple group which knows how to
	/// paint a thin frame around itself.
	/// </summary>
	public class FrameBox : AbstractGroup
	{
		public FrameBox()
		{
		}

		public FrameBox(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		public bool DrawFullFrame
		{
			get
			{
				return (bool) this.GetValue (FrameBox.DrawFullFrameProperty);
			}
			set
			{
				this.SetValue (FrameBox.DrawFullFrameProperty, value);
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if (this.DrawFullFrame)
			{
				IAdorner adorner = Widgets.Adorners.Factory.Active;

				Rectangle rect = this.Client.Bounds;
				rect.Deflate (0.5);
				graphics.AddRectangle (rect);
				graphics.RenderSolid (adorner.ColorBorder);
			}

			if (this.DrawDesignerFrame)
			{
				IAdorner adorner = Widgets.Adorners.Factory.Active;

				Rectangle rect = this.Client.Bounds;
				rect.Deflate (0.5);
				
				using (Path path = new Path (rect))
				{
					FrameBox.DrawPathDash (graphics, path, 1, 4, 4, adorner.ColorBorder);
				}
			}
		}

		static protected void DrawPathDash(Graphics graphics, Path path, double width, double dash, double gap, Color color)
		{
			//	Dessine un traitillé simple (dash/gap) le long d'un chemin.

			if (path.IsEmpty)
			{
				return;
			}

			using (DashedPath dp = new DashedPath ())
			{
				dp.Append (path);

				if (dash == 0.0)  // juste un point ?
				{
					dash = 0.00001;
					gap -= dash;
				}
				
				dp.AddDash (dash, gap);

				using (Path temp = dp.GenerateDashedPath ())
				{
					graphics.Rasterizer.AddOutline (temp, width, CapStyle.Square, JoinStyle.Round, 5.0);
					graphics.RenderSolid (color);
				}
			}
		}
		
		public static readonly DependencyProperty DrawFullFrameProperty = DependencyProperty.Register ("DrawFullFrame", typeof (bool), typeof (FrameBox), new VisualPropertyMetadata (false, VisualPropertyMetadataOptions.AffectsDisplay));
	}
}
