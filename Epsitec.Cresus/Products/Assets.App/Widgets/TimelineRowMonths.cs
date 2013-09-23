//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Ligne de Timeline affichant les noms des mois et années.
	/// Par exemple "Septembre 2013", "Octobre 2013".
	/// Si la place manque, le nom du mois est compacté et l'année
	/// éventuellement supprimée.
	/// </summary>
	public class TimelineRowMonths : AbstractTimelineRow
	{
		public TimelineRowMonths(TimelineRowDescription row)
			: base (row)
		{
		}


		protected override bool IsSame(TimelineCell c1, TimelineCell c2)
		{
			return TimelineCell.IsSameMonths (c1, c2);
		}

		protected override void PaintCellForeground(Graphics graphics, Rectangle rect, TimelineCell cell, bool isHover, int index)
		{
			//	Dessine le contenu, plus ou moins détaillé selon la place disponible.
			var font = Font.DefaultFont;

			for (int detailLevel = 3; detailLevel >= 0; detailLevel--)
			{
				var text = TimelineRowMonths.GetMonthText (cell, detailLevel);
				if (string.IsNullOrEmpty (text))
				{
					break;
				}

				var width = new TextGeometry (0, 0, 1000, 100, text, font, rect.Height*0.6, ContentAlignment.MiddleLeft).Width;
				if (width <= rect.Width)
				{
					graphics.Color = ColorManager.TextColor;
					graphics.PaintText (rect, text, font, rect.Height*0.6, ContentAlignment.MiddleCenter);
					break;
				}
			}
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

		private static string GetMonthText(TimelineCell cell, int detailLevel)
		{
			//	Retourne le mois sous une forme plus ou moins détaillée.
			//	detailLevel = 3 retourne "Septembre 2013"
			//	detailLevel = 2 retourne "Sept. 2013"
			//	detailLevel = 1 retourne "Septembre"
			//	detailLevel = 0 retourne "Sept."
			//	Voir http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx
			if (cell.IsValid)
			{
				switch (detailLevel)
				{
					case 3:
						return cell.Date.ToString ("MMMM yyyy", System.Globalization.DateTimeFormatInfo.CurrentInfo);

					case 2:
						return cell.Date.ToString ("MMM yyyy", System.Globalization.DateTimeFormatInfo.CurrentInfo);

					case 1:
						return cell.Date.ToString ("MMMM", System.Globalization.DateTimeFormatInfo.CurrentInfo);

					case 0:
						return cell.Date.ToString ("MMM", System.Globalization.DateTimeFormatInfo.CurrentInfo);
				}
			}

			return null;
		}

	}
}
