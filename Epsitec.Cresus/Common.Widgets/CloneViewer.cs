using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe CloneViewer implémente un widget qui prend l'aspect d'un autre.
	/// </summary>
	public class CloneViewer : Widget
	{
		public CloneViewer()
		{
		}
		
		public CloneViewer(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		/// <summary>
		/// Détermine le widget dont on prend l'aspect.
		/// </summary>
		/// <value>Le widget dont on prend l'aspect</value>
		public Widget Clone
		{
			get
			{
				return this.clone;
			}
			set
			{
				this.clone = value;
			}
		}


		public override void PaintHandler(Graphics graphics, Rectangle repaint, IPaintFilter paintFilter)
		{
#if false
			if (this.clone == null)
			{
				base.PaintHandler(graphics, repaint, paintFilter);
			}
			else
			{
				this.clone.PaintHandler(graphics, repaint, paintFilter);
			}
#else
			graphics.AddFilledRectangle(this.Client.Bounds);
			graphics.RenderSolid(Color.FromAlphaRgb(0.5, 1, 0, 0));
#endif
		}


		protected Widget				clone;
	}
}
