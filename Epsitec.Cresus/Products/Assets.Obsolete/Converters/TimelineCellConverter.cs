//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Assets.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Converters
{
	public sealed class TimelineCellConverter
	{
#if false
		public Widgets.TimelineCell Convert(TimelineEventCell item)
		{
			Widgets.TimelineGlyph glyph = Widgets.TimelineGlyph.FilledCircle;

			switch (item.Date.DayOfWeek)
			{
				case System.DayOfWeek.Monday:
					glyph = Widgets.TimelineGlyph.FilledCircle;
					break;
				case System.DayOfWeek.Tuesday:
				case System.DayOfWeek.Wednesday:
					glyph = Widgets.TimelineGlyph.FilledSquare;
					break;
				case System.DayOfWeek.Thursday:
				case System.DayOfWeek.Friday:
					glyph = Widgets.TimelineGlyph.OutlinedCircle;
					break;
				case System.DayOfWeek.Saturday:
				case System.DayOfWeek.Sunday:
					glyph = Widgets.TimelineGlyph.OutlinedSquare;
					break;
			}

			return new Widgets.TimelineCell (item.Date, glyph);
		}
#endif
	}
}
