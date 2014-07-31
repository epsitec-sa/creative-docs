//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public struct TimelineRowDescription
	{
		public TimelineRowDescription
		(
			TimelineRowType				type,
			string						description,
			double						relativeHeight   = 1.0,
			TimelineValueDisplayMode	valueDisplayMode = TimelineValueDisplayMode.All
		)
		{
			this.Type             = type;
			this.Description      = description;
			this.RelativeHeight   = relativeHeight;
			this.ValueDisplayMode = valueDisplayMode;
		}


		public readonly TimelineRowType				Type;
		public readonly string						Description;
		public readonly double						RelativeHeight;
		public readonly TimelineValueDisplayMode	ValueDisplayMode;

		
		public override string ToString()
		{
			var buffer = new System.Text.StringBuilder ();

			buffer.Append (this.Type);
			buffer.Append (" ");
			buffer.Append (this.Description);

			return buffer.ToString ();
		}


		public static AbstractTimelineRow Create(TimelineRowDescription description)
		{
			AbstractTimelineRow row = null;

			switch (description.Type)
			{
				case TimelineRowType.Years:
					row = new TimelineRowYears ();
					break;

				case TimelineRowType.Months:
					row = new TimelineRowMonths ();
					break;

				case TimelineRowType.WeekOfYear:
					row = new TimelineRowWeeksOfYear ();
					break;

				case TimelineRowType.DaysOfWeek:
					row = new TimelineRowDaysOfWeek ();
					break;

				case TimelineRowType.Days:
					row = new TimelineRowDays ();
					break;

				case TimelineRowType.DaysMonths:
					row = new TimelineRowDaysMonths ();
					break;

				case TimelineRowType.Glyph:
					row = new TimelineRowGlyphs ();
					break;

				case TimelineRowType.Value:
					row = new TimelineRowValues ();

					var v = row as TimelineRowValues;
					v.ValueDisplayMode = description.ValueDisplayMode;
					break;
			}

			System.Diagnostics.Debug.Assert (row != null);

			row.Description    = description.Description;
			row.RelativeHeight = description.RelativeHeight;

			return row;
		}
	}
}
