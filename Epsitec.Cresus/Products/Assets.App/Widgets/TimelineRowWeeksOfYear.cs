//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class TimelineRowWeeksOfYear : AbstractTimelineRow
	{
		public TimelineRowWeeksOfYear(TimelineDisplay display)
			: base (display)
		{
		}


		protected override bool IsSame(TimelineCell c1, TimelineCell c2)
		{
			return TimelineCell.IsSameWeeksOfYear (c1, c2);
		}

		protected override Color GetCellColor(TimelineCell cell, bool isHover, int index)
		{
			if (cell.IsValid)
			{
				return ColorManager.GetCheckerboardColor (index%2 == 0, isHover);
			}
			else
			{
				return Color.Empty;
			}
		}

		protected override string GetCellText(TimelineCell cell)
		{
			//	Retourne le numéro de semaine sous la forme "1" ou "52".
			if (cell.IsValid)
			{
				return cell.Date.WeekOfYear.ToString ();
			}
			else
			{
				return null;
			}
		}
	}
}
