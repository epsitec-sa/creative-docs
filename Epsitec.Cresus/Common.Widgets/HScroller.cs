[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.HScroller))]

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
			Types.DependencyPropertyMetadata metadataDx = Visual.PreferredWidthProperty.DefaultMetadata.Clone ();
			Types.DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			double dx = AbstractScroller.minimalThumb+6;
			double dy = AbstractScroller.DefaultBreadth;

			metadataDx.DefineDefaultValue (dx);
			metadataDy.DefineDefaultValue (dy);
			
			Visual.PreferredHeightProperty.OverrideMetadata (typeof (HScroller), metadataDy);
			Visual.MinWidthProperty.OverrideMetadata (typeof (HScroller), metadataDx);
			Visual.MinHeightProperty.OverrideMetadata (typeof (HScroller), metadataDy);
		}
	}
}
