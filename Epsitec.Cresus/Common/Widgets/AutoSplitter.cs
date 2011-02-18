//	Copyright © 2009, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	public class AutoSplitter : AbstractSplitter
	{
		public AutoSplitter()
		{
		}

		public AutoSplitter(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}

		static AutoSplitter()
		{
			var metadataWidth  = Visual.PreferredWidthProperty.DefaultMetadata.Clone ();
			var metadataHeight = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			metadataWidth.DefineDefaultValue (4.0);
			metadataHeight.DefineDefaultValue (4.0);

			Visual.PreferredWidthProperty.OverrideMetadata (typeof (AutoSplitter), metadataWidth);
			Visual.PreferredHeightProperty.OverrideMetadata (typeof (AutoSplitter), metadataHeight);
		}

		public override bool IsVertical
		{
			get
			{
				switch (this.Dock)
				{
					case DockStyle.Left:
					case DockStyle.Right:
						return true;

					case DockStyle.Top:
					case DockStyle.Bottom:
						return false;

					default:
						System.Diagnostics.Debug.WriteLine ("IsVertical cannot derive its orientation based on the docking info");
						return false;
				}
			}
		}
	}
}
