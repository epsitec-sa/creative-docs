//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public struct TimelineGlyph
	{
		public TimelineGlyph(TimelineGlyphShape shape, TimelineGlyphMode mode = TimelineGlyphMode.Undefined)
		{
			this.Shape = shape;
			this.Mode  = mode;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Shape == TimelineGlyphShape.Empty;
			}
		}

		public static TimelineGlyph Empty     = new TimelineGlyph (TimelineGlyphShape.Empty);
		public static TimelineGlyph Undefined = new TimelineGlyph (TimelineGlyphShape.Undefined);

		public readonly TimelineGlyphShape		Shape;
		public readonly TimelineGlyphMode		Mode;
	};
}
