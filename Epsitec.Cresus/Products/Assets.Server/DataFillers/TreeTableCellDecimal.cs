//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class TreeTableCellDecimal : AbstractTreeTableCell
	{
		public TreeTableCellDecimal(bool isValid, decimal? value, bool isSelected = false, bool isEvent = false, bool isError = false, bool isUnavailable = false)
			: base (isValid, isSelected, isEvent, isError, isUnavailable)
		{
			this.Value = value;
		}


		public readonly decimal?				Value;

		
		public override string ToString()
		{
			return this.Value.ToString () + base.ToString ();
		}
	}
}
