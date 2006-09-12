namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe VSlider implémente le potentiomètre linéaire vertical.
	/// </summary>
	public class VSlider : AbstractSlider
	{
		public VSlider()
			: base (true, true)
		{
			this.ArrowUp.Name   = "Up";
			this.ArrowDown.Name = "Down";

			this.ArrowMax.GlyphShape = GlyphShape.ArrowUp;
			this.ArrowMin.GlyphShape = GlyphShape.ArrowDown;
		}

		public VSlider(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		static VSlider()
		{
			Helpers.VisualPropertyMetadata metadataMinDx = new Helpers.VisualPropertyMetadata (AbstractSlider.defaultBreadth/2, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);
			Helpers.VisualPropertyMetadata metadataMinDy = new Helpers.VisualPropertyMetadata (AbstractSlider.minimalThumb+6, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);
			Helpers.VisualPropertyMetadata metadataDx = new Helpers.VisualPropertyMetadata (AbstractSlider.defaultBreadth, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);

			Visual.MinWidthProperty.OverrideMetadata (typeof (VSlider), metadataMinDx);
			Visual.MinHeightProperty.OverrideMetadata (typeof (VScroller), metadataMinDy);
			Visual.PreferredWidthProperty.OverrideMetadata (typeof (VSlider), metadataDx);
		}
	}
}
