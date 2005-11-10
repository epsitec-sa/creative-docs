namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe VSlider implémente le potentiomètre linéaire vertical.
	/// </summary>
	public class VSlider : AbstractSlider
	{
		public VSlider() : base(true)
		{
			this.ArrowUp.Name   = "Up";
			this.ArrowDown.Name = "Down";
		}
		
		public VSlider(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public override double				DefaultWidth
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
				return new Drawing.Size (AbstractSlider.defaultBreadth, AbstractSlider.minimalThumb+6);
			}
		}
	}
}
