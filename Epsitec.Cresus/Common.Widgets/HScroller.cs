namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe HScroller implémente l'ascenceur horizontal.
	/// </summary>
	public class HScroller : AbstractScroller
	{
		public HScroller() : base(false)
		{
			this.ArrowUp.Name   = "Right";
			this.ArrowDown.Name = "Left";
		}
		
		public HScroller(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		static HScroller()
		{
			Helpers.VisualPropertyMetadata metadataDx = new Helpers.VisualPropertyMetadata (AbstractScroller.minimalThumb+6, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);
			Helpers.VisualPropertyMetadata metadataDy = new Helpers.VisualPropertyMetadata (AbstractScroller.defaultBreadth, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);

			Visual.PreferredHeightProperty.OverrideMetadata (typeof (HScroller), metadataDy);
			Visual.MinWidthProperty.OverrideMetadata (typeof (HScroller), metadataDx);
			Visual.MinHeightProperty.OverrideMetadata (typeof (HScroller), metadataDy);
		}
	}
}
