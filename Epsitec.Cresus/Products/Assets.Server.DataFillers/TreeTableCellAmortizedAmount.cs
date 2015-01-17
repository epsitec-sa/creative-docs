//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class TreeTableCellAmortizedAmount : AbstractTreeTableCell
	{
		public TreeTableCellAmortizedAmount(AmortizedAmount? value, CellState cellState, string tooltip = null)
			: base (cellState, tooltip)
		{
			this.Value = value;
		}


		public readonly AmortizedAmount?		Value;

		
		public override string ToString()
		{
			return this.Value.ToString () + base.ToString ();
		}
	}
}
