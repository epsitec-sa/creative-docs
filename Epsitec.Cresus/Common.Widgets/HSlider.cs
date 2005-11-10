namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe HSlider implémente le potentiomètre linéaire horizontal.
	/// </summary>
	public class HSlider : AbstractSlider
	{
		public HSlider() : base(false)
		{
			this.ArrowUp.Name   = "Right";
			this.ArrowDown.Name = "Left";
		}
		
		public HSlider(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public override double				DefaultHeight
		{
			get
			{
				return AbstractSlider.defaultBreadth;
			}
		}
		
		public override Drawing.Size		DefaultMinSize
		{
			get
			{
				return new Drawing.Size (AbstractSlider.minimalThumb+6, AbstractSlider.defaultBreadth);
			}
		}
	}
}
