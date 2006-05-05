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
		
		
		static HSlider()
		{
			Helpers.VisualPropertyMetadata metadataMinDx = new Helpers.VisualPropertyMetadata (AbstractSlider.minimalThumb+6, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);
			Helpers.VisualPropertyMetadata metadataMinDy = new Helpers.VisualPropertyMetadata (AbstractSlider.defaultBreadth/2, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);
			Helpers.VisualPropertyMetadata metadataDy = new Helpers.VisualPropertyMetadata (AbstractSlider.defaultBreadth, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);

			Visual.MinWidthProperty.OverrideMetadata (typeof (HSlider), metadataMinDx);
			Visual.MinHeightProperty.OverrideMetadata (typeof (HSlider), metadataMinDy);
			Visual.PreferredHeightProperty.OverrideMetadata (typeof (HSlider), metadataDy);
		}
	}
}
