//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	public class VSplitter : AbstractSplitter
	{
		public VSplitter()
		{
		}

		public VSplitter(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}

		static VSplitter()
		{
			Types.DependencyPropertyMetadata metadataWidth = Visual.PreferredWidthProperty.DefaultMetadata.Clone ();

			metadataWidth.DefineDefaultValue (4.0);
			
			Visual.PreferredWidthProperty.OverrideMetadata(typeof(VSplitter), metadataWidth);
		}

		public override bool IsVertical
		{
			get
			{
				return true;
			}
		}
	}
}
