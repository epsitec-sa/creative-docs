namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe VSlider implémente le potentiomètre linéaire vertical.
	/// </summary>
	public class VSlider : AbstractSlider
	{
		public VSlider()
			: base (true)
		{
			this.ArrowUp.Name   = "Up";
			this.ArrowDown.Name = "Down";
		}

		public VSlider(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		static VSlider()
		{
			Helpers.VisualPropertyMetadata metadataDx = new Helpers.VisualPropertyMetadata (AbstractSlider.defaultBreadth, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);
			Helpers.VisualPropertyMetadata metadataDy = new Helpers.VisualPropertyMetadata (AbstractSlider.minimalThumb+6, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);

			Visual.PreferredWidthProperty.OverrideMetadata (typeof (VSlider), metadataDx);
			Visual.MinWidthProperty.OverrideMetadata (typeof (VSlider), metadataDx);
			Visual.MinHeightProperty.OverrideMetadata (typeof (VScroller), metadataDy);
		}
	}
}
