//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public abstract class AbstractTreeTableCell
	{
		public AbstractTreeTableCell(bool isValid, bool isSelected = false, bool isEvent = false, bool isError = false, bool isUnavailable = false)
		{
			this.IsValid       = isValid;
			this.IsSelected    = isSelected;
			this.IsEvent       = isEvent;
			this.IsError       = isError;
			this.IsUnavailable = isUnavailable;
		}

		public readonly bool					IsValid;
		public readonly bool					IsSelected;
		public readonly bool					IsEvent;
		public readonly bool					IsError;
		public readonly bool					IsUnavailable;


		public override string ToString()
		{
			var buffer = new System.Text.StringBuilder ();

			if (!this.IsValid)
			{
				buffer.Append (" Invalid");
			}

			if (this.IsSelected)
			{
				buffer.Append (" Selected");
			}

			if (this.IsEvent)
			{
				buffer.Append (" Event");
			}

			if (this.IsError)
			{
				buffer.Append (" Error");
			}

			if (this.IsUnavailable)
			{
				buffer.Append (" Unavailable");
			}

			return buffer.ToString ();
		}
	}
}
