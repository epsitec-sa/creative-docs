//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public struct TimelineRowDescription
	{
		public TimelineRowDescription
		(
			TimelineRowType type, string description, double relativeHeight = 1.0,
			TimelineValueDisplayMode valueDisplayMode = TimelineValueDisplayMode.All,
			Color? valueColor1 = null,
			Color? valueColor2 = null
		)
		{
			this.Type           = type;
			this.Description    = description;
			this.RelativeHeight = relativeHeight;
			this.ValueDisplayMode    = valueDisplayMode;
			this.ValueColor1         = valueColor1;
			this.ValueColor2         = valueColor2;
		}


		public readonly TimelineRowType			Type;
		public readonly string					Description;
		public readonly double					RelativeHeight;
		public readonly TimelineValueDisplayMode ValueDisplayMode;
		public readonly Color?					ValueColor1;
		public readonly Color?					ValueColor2;

		
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
				case TimelineRowType.Month:
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

				case TimelineRowType.Glyph:
					row = new TimelineRowGlyphs ();
					break;

				case TimelineRowType.Value:
					row = new TimelineRowValues ();

					var v = row as TimelineRowValues;
					v.ValueDisplayMode = description.ValueDisplayMode;
					v.ValueColor1      = description.ValueColor1;
					v.ValueColor2      = description.ValueColor2;
					break;
			}

			System.Diagnostics.Debug.Assert (row != null);

			row.Description    = description.Description;
			row.RelativeHeight = description.RelativeHeight;

			return row;
		}
	}
}
