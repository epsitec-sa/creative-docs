//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class SimpleTreeTableCellGlyph : AbstractSimpleTreeTableCell
	{
		public SimpleTreeTableCellGlyph(TimelineGlyph? value)
		{
			this.Value = value;
		}

		public readonly TimelineGlyph?		Value;
	}
}
