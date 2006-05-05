//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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

		public override bool IsVertical
		{
			get
			{
				return true;
			}
		}
	}
}
