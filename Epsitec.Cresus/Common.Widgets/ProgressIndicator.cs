namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ProgressIndicator implémente la barre d'avance.
	/// </summary>
	public class ProgressIndicator : AbstractSlider
	{
		public ProgressIndicator() : base (false, true)
		{
			this.ArrowUp.Name   = "Right";
			this.ArrowDown.Name = "Left";

			this.ArrowMax.GlyphShape = GlyphShape.ArrowRight;
			this.ArrowMin.GlyphShape = GlyphShape.ArrowLeft;
		}

		public ProgressIndicator(Widget embedder) : this ()
		{
			this.SetEmbedder(embedder);
		}


		static ProgressIndicator()
		{
			Types.DependencyPropertyMetadata metadataMinDx = Visual.MinWidthProperty.DefaultMetadata.Clone();
			Types.DependencyPropertyMetadata metadataMinDy = Visual.MinHeightProperty.DefaultMetadata.Clone();
			Types.DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone();

			double minDx = AbstractSlider.minimalThumb+6;
			double minDy = AbstractSlider.defaultBreadth/2;
			double dy = AbstractSlider.defaultBreadth;
			
			metadataMinDx.DefineDefaultValue(minDx);
			metadataMinDy.DefineDefaultValue(minDy);
			metadataDy.DefineDefaultValue(dy);

			Visual.MinWidthProperty.OverrideMetadata(typeof (ProgressIndicator), metadataMinDx);
			Visual.MinHeightProperty.OverrideMetadata(typeof (ProgressIndicator), metadataMinDy);
			Visual.PreferredHeightProperty.OverrideMetadata(typeof (ProgressIndicator), metadataDy);
		}
	}
}
