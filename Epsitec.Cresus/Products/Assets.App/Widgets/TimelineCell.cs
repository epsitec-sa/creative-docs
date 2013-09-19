//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public struct TimelineCell
	{
		public TimelineCell(Date date, TimelineCellGlyph glyph, bool isSelected = false, bool isError = false)
		{
			this.Date = date;
			this.Glyph = glyph;
			this.IsSelected = isSelected;
			this.IsError = isError;
		}

		
		public bool								IsInvalid
		{
			get
			{
				return this.Date.IsNull;
			}
		}

		public bool								IsValid
		{
			get
			{
				return !this.Date.IsNull;
			}
		}

		public readonly Date					Date;
		
		public readonly TimelineCellGlyph		Glyph;
		
		public readonly bool					IsSelected;
		
		public readonly bool					IsError;

		
		public override string ToString()
		{
			var buffer = new System.Text.StringBuilder ();

			buffer.Append (this.Date.ToString ());
			buffer.Append (" ");
			buffer.Append (this.Glyph);

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
