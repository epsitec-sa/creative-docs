//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class SimpleTreeTableCellDecimal : AbstractSimpleTreeTableCell
	{
		public SimpleTreeTableCellDecimal(decimal? value, DecimalFormat format = DecimalFormat.Real)
		{
			this.Value  = value;
			this.Format = format;
		}

		public readonly decimal?				Value;
		public readonly DecimalFormat			Format;
	}
}
