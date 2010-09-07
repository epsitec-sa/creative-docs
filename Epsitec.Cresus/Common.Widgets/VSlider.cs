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
		}

		public VSlider(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		static VSlider()
		{
			Types.DependencyPropertyMetadata metadataMinDx = Visual.MinWidthProperty.DefaultMetadata.Clone ();
			Types.DependencyPropertyMetadata metadataMinDy = Visual.MinHeightProperty.DefaultMetadata.Clone ();
			Types.DependencyPropertyMetadata metadataDx = Visual.PreferredWidthProperty.DefaultMetadata.Clone ();

			double minDx = AbstractSlider.defaultBreadth/2;
			double minDy = AbstractSlider.minimalThumb+6;
			double dx = AbstractSlider.defaultBreadth;

			metadataMinDx.DefineDefaultValue (minDx);
			metadataMinDy.DefineDefaultValue (minDy);
			metadataDx.DefineDefaultValue (dx);
			
			Visual.MinWidthProperty.OverrideMetadata (typeof (VSlider), metadataMinDx);
			Visual.MinHeightProperty.OverrideMetadata (typeof (VScroller), metadataMinDy);
			Visual.PreferredWidthProperty.OverrideMetadata (typeof (VSlider), metadataDx);
		}
	}
}
