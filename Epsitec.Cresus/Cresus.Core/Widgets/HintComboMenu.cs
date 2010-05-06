//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Controllers;

namespace Epsitec.Cresus.Core.Widgets
{
	public class HintComboMenu : TextFieldComboMenu
	{
		public HintComboMenu()
		{
			this.InternalState &= ~InternalState.Focusable;
		}

		public HintComboMenu(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


	}
}
