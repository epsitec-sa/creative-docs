//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class TreeTableCellComputedAmount : AbstractTreeTableCell
	{
		public TreeTableCellComputedAmount(bool isValid, ComputedAmount? value, bool isSelected = false, bool isEvent = false, bool isError = false, bool isUnavailable = false)
			: base (isValid, isSelected, isEvent, isError, isUnavailable)
		{
			this.Value = value;
		}


		public readonly ComputedAmount?			Value;

		
		public override string ToString()
		{
			return this.Value.ToString () + base.ToString ();
		}
	}
}
