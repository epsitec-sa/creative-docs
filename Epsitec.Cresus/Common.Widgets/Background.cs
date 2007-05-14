using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Background permet de dessiner un fond coloré uni.
	/// </summary>
	public class Background : Widget
	{
		public Background()
		{
		}
		
		public Background(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public Drawing.Color					Color
		{
			get
			{
				return this.color;
			}
			set
			{
				if (this.color != value)
				{
					this.color = value;
					this.Invalidate();
				}
			}
		}
		

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			//	Dessine le fond coloré.
			if (!this.color.IsEmpty)
			{
				graphics.AddFilledRectangle(this.Client.Bounds);
				graphics.RenderSolid(this.color);
			}
		}


		private Drawing.Color					color = Drawing.Color.Empty;
	}
}
