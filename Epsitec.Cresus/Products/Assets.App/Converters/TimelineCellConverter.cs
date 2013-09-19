//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Assets.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Converters
{
	public sealed class TimelineCellConverter
	{
		public Widgets.TimelineCell Convert(TimelineEventCell item)
		{
			Widgets.TimelineCellGlyph glyph = Widgets.TimelineCellGlyph.FilledCircle;

			switch (item.Date.DayOfWeek)
			{
				case System.DayOfWeek.Monday:
					glyph = Widgets.TimelineCellGlyph.FilledCircle;
					break;
				case System.DayOfWeek.Tuesday:
				case System.DayOfWeek.Wednesday:
					glyph = Widgets.TimelineCellGlyph.FilledSquare;
					break;
				case System.DayOfWeek.Thursday:
				case System.DayOfWeek.Friday:
					glyph = Widgets.TimelineCellGlyph.OutlinedCircle;
					break;
				case System.DayOfWeek.Saturday:
				case System.DayOfWeek.Sunday:
					glyph = Widgets.TimelineCellGlyph.OutlinedSquare;
					break;
			}

			return new Widgets.TimelineCell (item.Date, glyph);
		}
	}
}
