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
			Types.DependencyPropertyMetadata metadataDx = Visual.PreferredWidthProperty.DefaultMetadata.Clone ();
			Types.DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			double dx = AbstractScroller.DefaultBreadth;
			double dy = AbstractScroller.minimalThumb+6;

			metadataDx.DefineDefaultValue (dx);
			metadataDy.DefineDefaultValue (dy);

			Visual.PreferredWidthProperty.OverrideMetadata (typeof (VScroller), metadataDx);
			Visual.MinWidthProperty.OverrideMetadata (typeof (VScroller), metadataDx);
			Visual.MinHeightProperty.OverrideMetadata (typeof (VScroller), metadataDy);
		}
	}
}
