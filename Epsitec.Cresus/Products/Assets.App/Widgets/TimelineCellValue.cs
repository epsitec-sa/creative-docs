//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public struct TimelineCellValue
	{
		public TimelineCellValue(int rank, decimal?[] values, bool isSelected = false, bool isError = false)
		{
			this.Rank       = rank;
			this.Values     = values;
			this.IsSelected = isSelected;
			this.IsError    = isError;
		}

		
		public bool								IsInvalid
		{
			get
			{
				return !this.IsValid;
			}
		}

		public bool								IsValid
		{
			get
			{
				return this.Values != null;
			}
		}

		public decimal? GetValue(int rank)
		{
			return this.Values[rank];
		}

		public readonly int						Rank;
		public readonly decimal?[]				Values;
		public readonly bool					IsSelected;
		public readonly bool					IsError;

		
		public override string ToString()
		{
			var buffer = new System.Text.StringBuilder ();

			foreach (var value in this.Values)
			{
				buffer.Append (value.ToString ());
				buffer.Append (" ");
			}

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
