using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	using BundleAttribute = Epsitec.Common.Support.BundleAttribute;
	
	/// <summary>
	/// La classe GradientSample permet de représenter une dégradé.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class GradientSample : Epsitec.Common.Widgets.AbstractButton
	{
		public GradientSample()
		{
		}
		
		public GradientSample(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public PropertyGradient Gradient
		{
			//	Dégradé.
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


		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			//	Dessine la couleur.
			IAdorner adorner = Epsitec.Common.Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);

			if ( this.IsEnabled )
			{
				graphics.AddLine(rect.Left+0.5, rect.Bottom+0.5, rect.Right-0.5, rect.Top-0.5);
				graphics.AddLine(rect.Left+0.5, rect.Top-0.5, rect.Right-0.5, rect.Bottom+0.5);
				graphics.RenderSolid(adorner.ColorBorder);  // dessine la croix

				if ( this.gradient != null )
				{
					Drawing.Path path = new Drawing.Path();
					path.MoveTo(rect.Left, rect.Bottom);
					path.LineTo(rect.Left, rect.Top);
					path.LineTo(rect.Right, rect.Top);
					path.LineTo(rect.Right, rect.Bottom);
					path.Close();

					gradient.Render(graphics, null, path, rect);
				}
			}

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(adorner.ColorBorder);
		}


		protected PropertyGradient			gradient;
	}
}
