//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public struct TimelineCellGlyph
	{
		public TimelineCellGlyph(TimelineGlyph glyph, bool isSelected = false, bool isError = false)
		{
			this.Glyph      = glyph;
			this.IsSelected = isSelected;
			this.IsError    = isError;
		}

		
		public bool								IsInvalid
		{
			get
			{
				return this.Glyph == TimelineGlyph.Undefined;
			}
		}

		public bool								IsValid
		{
			get
			{
				return this.Glyph != TimelineGlyph.Undefined;
			}
		}

		public readonly TimelineGlyph			Glyph;
		public readonly bool					IsSelected;
		public readonly bool					IsError;

		
		public override string ToString()
		{
			var buffer = new System.Text.StringBuilder ();

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
