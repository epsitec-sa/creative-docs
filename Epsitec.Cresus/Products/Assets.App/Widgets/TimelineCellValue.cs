//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public struct TimelineCellValue
	{
		public TimelineCellValue(int rank, decimal? value1 = null, decimal? value2 = null, bool isSelected = false, bool isError = false)
		{
			this.Rank       = rank;
			this.Value1     = value1;
			this.Value2     = value2;
			this.IsSelected = isSelected;
			this.IsError    = isError;
		}

		
		public bool								IsInvalid
		{
			get
			{
				return !this.Value1.HasValue && !this.Value2.HasValue;
			}
		}

		public bool								IsValid
		{
			get
			{
				return this.Value1.HasValue || this.Value2.HasValue;
			}
		}

		public decimal? GetValue(int rank)
		{
			return (rank == 0) ? this.Value1 : this.Value2;
		}

		public readonly int						Rank;
		public readonly decimal?				Value1;
		public readonly decimal?				Value2;
		public readonly bool					IsSelected;
		public readonly bool					IsError;

		
		public override string ToString()
		{
			var buffer = new System.Text.StringBuilder ();

			buffer.Append (this.Value1.ToString ());
			buffer.Append (" ");
			buffer.Append (this.Value2.ToString ());

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
