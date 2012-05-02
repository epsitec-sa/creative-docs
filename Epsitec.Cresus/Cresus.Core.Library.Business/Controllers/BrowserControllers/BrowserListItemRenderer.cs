//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.BigList;
using Epsitec.Common.BigList.Renderers;
using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	public class BrowserListItemRenderer : StringRenderer<BrowserListItem>
	{
		public BrowserListItemRenderer(BrowserList list)
			: base (x => x.GetDisplayText (list))
		{
		}
	}
}
