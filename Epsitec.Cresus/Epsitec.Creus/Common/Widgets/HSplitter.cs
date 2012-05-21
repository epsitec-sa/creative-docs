//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
