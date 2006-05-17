//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	public class HSplitter : AbstractSplitter
	{
		public HSplitter()
		{
		}

		public HSplitter(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}

		static HSplitter()
		{
			Helpers.VisualPropertyMetadata metadataHeight = new Helpers.VisualPropertyMetadata(4.0, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);
			Visual.PreferredHeightProperty.OverrideMetadata(typeof(HSplitter), metadataHeight);
		}

		public override bool IsVertical
		{
			get
			{
				return false;
			}
		}
	}
}
