using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Widgets
{
	public class SimpleTile : AbstractTile
	{
		public SimpleTile()
		{
		}

		public SimpleTile(Widget embedder)
			: this()
		{
			this.SetEmbedder(embedder);
		}
	}
}
