using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe <c>CloneView</c> implémente un widget qui prend l'aspect d'un autre.
	/// </summary>
	public class CloneView : Widget
	{
		public CloneView()
		{
		}
		
		public CloneView(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		/// <summary>
		/// Détermine le widget dont on prend l'aspect.
		/// </summary>
		/// <value>Le widget dont on prend l'aspect.</value>
		public Widget Model
		{
			get
			{
				return this.model;
			}
			set
			{
				this.model = value;
			}
		}


		public override void PaintHandler(Graphics graphics, Rectangle repaint, IPaintFilter paintFilter)
		{
#if true
			if (this.model == null)
			{
				base.PaintHandler(graphics, repaint, paintFilter);
			}
			else
			{
				Drawing.Point offset = this.model.ActualLocation - this.ActualLocation;
				Drawing.Point pos = this.MapClientToRoot (Drawing.Point.Zero);
				Drawing.Point originalClipOffset = graphics.ClipOffset;
				Drawing.Transform originalTransform = graphics.Transform;
				Drawing.Transform graphicsTransform = Drawing.Transform.FromTranslation (- offset);

				Drawing.Point clipOffset = pos - this.model.MapClientToRoot (this.model.GetClipBounds ()).Location;

				repaint.Offset (offset);
				
				graphicsTransform.MultiplyBy (originalTransform);
				graphics.Transform = graphicsTransform;
				graphics.ClipOffset = clipOffset;
				
				this.model.PaintHandler(graphics, repaint, paintFilter);

				graphics.Transform = originalTransform;
				graphics.ClipOffset = originalClipOffset;
			}
#else
			graphics.AddFilledRectangle(this.Client.Bounds);
			graphics.RenderSolid(Color.FromAlphaRgb(0.5, 1, 0, 0));
#endif
		}


		protected Widget				model;
	}
}
