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
			Types.DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			metadataDy.DefineDefaultValue (4.0);
			
			Visual.PreferredHeightProperty.OverrideMetadata(typeof(HSplitter), metadataDy);
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
