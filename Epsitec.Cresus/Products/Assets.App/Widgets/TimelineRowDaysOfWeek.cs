//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class TimelineRowDaysOfWeek : AbstractTimelineRow
	{
		public TimelineRowDaysOfWeek(TimelineDisplay display)
			: base (display)
		{
		}


		protected override bool IsSame(TimelineCell c1, TimelineCell c2)
		{
			return TimelineCell.IsSameDays (c1, c2);
		}

		protected override Color GetCellColor(TimelineCell cell, bool isHover, int index)
		{
			if (cell.IsValid)
			{
				if (cell.Date.DayOfWeek == System.DayOfWeek.Saturday ||
					cell.Date.DayOfWeek == System.DayOfWeek.Sunday)
				{
					return ColorManager.GetHolidayColor (isHover);
				}
				else
				{
					return ColorManager.GetBackgroundColor (isHover);
				}
			}
			else
			{
				return Color.Empty;
			}
		}

		protected override string GetCellText(TimelineCell cell)
		{
			//	Retourne le jour sous la forme "Lu" ou "Ma".
			if (cell.IsValid)
			{
				var text = cell.Date.ToString ("ddd", System.Globalization.DateTimeFormatInfo.CurrentInfo);

				if (text.Length > 2)
				{
					text = text.Substring (0, 2);
				}

				return text;
			}
			else
			{
				return null;
			}
		}
	}
}
