namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe HSlider implémente le potentiomètre linéaire horizontal.
	/// </summary>
	public class HSlider : AbstractSlider
	{
		public HSlider()
			: base (false, true)
		{
			this.ArrowUp.Name   = "Right";
			this.ArrowDown.Name = "Left";
		}

		public HSlider(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		static HSlider()
		{
			Types.DependencyPropertyMetadata metadataMinDx = Visual.MinWidthProperty.DefaultMetadata.Clone ();
			Types.DependencyPropertyMetadata metadataMinDy = Visual.MinHeightProperty.DefaultMetadata.Clone ();
			Types.DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			double minDx = AbstractSlider.minimalThumb+6;
			double minDy = AbstractSlider.defaultBreadth/2;
			double dy = AbstractSlider.defaultBreadth;
			
			metadataMinDx.DefineDefaultValue (minDx);
			metadataMinDy.DefineDefaultValue (minDy);
			metadataDy.DefineDefaultValue (dy);

			Visual.MinWidthProperty.OverrideMetadata (typeof (HSlider), metadataMinDx);
			Visual.MinHeightProperty.OverrideMetadata (typeof (HSlider), metadataMinDy);
			Visual.PreferredHeightProperty.OverrideMetadata (typeof (HSlider), metadataDy);
		}
	}
}
