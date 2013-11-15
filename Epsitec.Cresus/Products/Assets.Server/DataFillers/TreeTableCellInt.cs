//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public struct TreeTableCellInt : ITreeTableCell
	{
		public TreeTableCellInt(bool isValid, int? value, bool isSelected = false, bool isError = false)
		{
			this.IsValid    = isValid;
			this.Value      = value;
			this.IsSelected = isSelected;
			this.IsError    = isError;
		}


		public readonly bool					IsValid;
		public readonly int?					Value;
		public readonly bool					IsSelected;
		public readonly bool					IsError;

		
		public override string ToString()
		{
			var buffer = new System.Text.StringBuilder ();

			if (!this.IsValid)
			{
				buffer.Append ("invalid ");
			}

			buffer.Append (this.Value);
			buffer.Append (" ");

			if (this.IsSelected)
			{
				buffer.Append (" selected");
			}
			if (this.IsError)
			{
				buffer.Append (" error");
			}

			return buffer.ToString ();
		}
	}
}
