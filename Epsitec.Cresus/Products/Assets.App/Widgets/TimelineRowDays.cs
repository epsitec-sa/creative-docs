//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Ligne de Timeline affichant les numéros des jours.
	/// Par exemple "28", "29", "30", "31".
	/// Les samedis et dimanches ont une couleur de fond légèrement différente.
	/// </summary>
	public class TimelineRowDays : AbstractTimelineRow
	{
		public TimelineRowDays(TimelineDisplay display)
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
			//	Retourne le jour sous la forme "1" ou "31".
			if (cell.IsValid)
			{
				return cell.Date.ToString ("dd", System.Globalization.DateTimeFormatInfo.CurrentInfo);
			}
			else
			{
				return null;
			}
		}
	}
}
