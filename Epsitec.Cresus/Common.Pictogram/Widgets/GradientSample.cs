using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	using BundleAttribute = Epsitec.Common.Support.BundleAttribute;
	
	/// <summary>
	/// La classe GradientSample permet de représenter une dégradé.
	/// </summary>
	public class GradientSample : Epsitec.Common.Widgets.AbstractButton
	{
		public GradientSample()
		{
			this.colorBlack   = Drawing.Color.FromName("WindowFrame");
			this.colorWhite   = Drawing.Color.FromName("Window");
			this.colorCaption = Drawing.Color.FromName("ActiveCaption");
		}
		
		public GradientSample(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		// Dégradé.
		public PropertyGradient Gradient
		{
			get
			{
				return this.gradient;
			}

			set
			{
				this.gradient = value;
				this.Invalidate();
			}
		}


		// Dessine la couleur.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);

			if ( this.IsEnabled )
			{
				graphics.AddLine(rect.Left, rect.Bottom, rect.Right, rect.Top);
				graphics.AddLine(rect.Left, rect.Top, rect.Right, rect.Bottom);
				graphics.RenderSolid(this.colorBlack);

				if ( this.gradient != null )
				{
					Drawing.Path path = new Drawing.Path();
					path.MoveTo(rect.Left, rect.Bottom);
					path.LineTo(rect.Left, rect.Top);
					path.LineTo(rect.Right, rect.Top);
					path.LineTo(rect.Right, rect.Bottom);
					path.Close();
					graphics.Rasterizer.AddSurface(path);

					gradient.Render(graphics, null, rect);
				}
			}

			rect.Inflate(-0.5, -0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.colorBlack);
		}



		protected Drawing.Color				colorBlack;
		protected Drawing.Color				colorWhite;
		protected Drawing.Color				colorCaption;
		protected PropertyGradient			gradient;
	}
}
