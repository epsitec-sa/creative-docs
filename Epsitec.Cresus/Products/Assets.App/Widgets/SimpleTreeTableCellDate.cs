//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Views;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class SimpleTreeTableCellDate : AbstractSimpleTreeTableCell
	{
		public SimpleTreeTableCellDate(System.DateTime? value)
		{
			this.Value = value;
		}

		public readonly System.DateTime?		Value;
	}
}
