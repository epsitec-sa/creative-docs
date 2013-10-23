//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public struct TreeTableCellDate
	{
		public TreeTableCellDate(bool isValid, System.DateTime? value, bool isSelected = false, bool isError = false)
		{
			this.IsValid    = isValid;
			this.Value      = value;
			this.IsSelected = isSelected;
			this.IsError    = isError;
		}


		public readonly bool					IsValid;
		public readonly System.DateTime?		Value;
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
