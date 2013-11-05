//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public struct TimelineCellGlyph
	{
		public TimelineCellGlyph(TimelineGlyph glyph, bool isLocked, string tooltip, bool isSelected = false, bool isError = false)
		{
			this.Glyph      = glyph;
			this.IsLocked   = isLocked;
			this.Tooltip    = tooltip;
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
		public readonly bool					IsLocked;
		public readonly string					Tooltip;
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


		public static bool IsSameGlyphs(TimelineCellGlyph c1, TimelineCellGlyph c2)
		{
			//	Deux glyhps successifs sont toujours considérés comme différents.
			//	Il faut juste considérer comme identiques les glyphs hors bornes.
			int g1 = (c1.IsValid) ? 0 : -1;
			int g2 = (c2.IsValid) ? 1 : -1;

			return g1 == g2;
		}
	}
}
