//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class SimpleTreeTableCellDecimal : AbstractSimpleTreeTableCell
	{
		public SimpleTreeTableCellDecimal(decimal value)
		{
			this.Value = value;
		}

		public readonly decimal Value;
	}
}
