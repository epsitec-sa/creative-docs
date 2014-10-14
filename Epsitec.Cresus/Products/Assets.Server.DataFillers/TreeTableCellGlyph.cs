//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class TreeTableCellGlyph : AbstractTreeTableCell
	{
		public TreeTableCellGlyph(TimelineGlyph? value, CellState cellState, string tooltip = null)
			: base (cellState, tooltip)
		{
			this.Value = value;
		}


		public readonly TimelineGlyph?			Value;

		
		public override string ToString()
		{
			return this.Value.ToString () + base.ToString ();
		}
	}
}
