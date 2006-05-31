[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.VScroller))]

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe VScroller implémente l'ascenceur vertical.
	/// </summary>
	public class VScroller : AbstractScroller
	{
		public VScroller()
			: base (true)
		{
			this.ArrowUp.Name   = "Up";
			this.ArrowDown.Name = "Down";
		}

		public VScroller(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		static VScroller()
		{
			Helpers.VisualPropertyMetadata metadataDx = new Helpers.VisualPropertyMetadata (AbstractScroller.defaultBreadth, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);
			Helpers.VisualPropertyMetadata metadataDy = new Helpers.VisualPropertyMetadata (AbstractScroller.minimalThumb+6, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);

			Visual.PreferredWidthProperty.OverrideMetadata (typeof (VScroller), metadataDx);
			Visual.MinWidthProperty.OverrideMetadata (typeof (VScroller), metadataDx);
			Visual.MinHeightProperty.OverrideMetadata (typeof (VScroller), metadataDy);
		}
	}
}
