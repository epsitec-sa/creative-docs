//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public static class ColorManager
	{
		public static Color GetBackgroundColor(bool isHover = false)
		{
			if (isHover)
			{
				return Color.FromHexa ("ebe8d6");  // gris-jaune
			}
			else
			{
				return Color.FromBrightness (1.0);
			}
		}

		public static Color GetHolidayColor(bool isHover = false)
		{
			if (isHover)
			{
				return Color.FromHexa ("ebe8d6");  // gris-jaune
			}
			else
			{
				return Color.FromBrightness (0.95);
			}
		}

		public static Color GetCheckerboardColor(bool even, bool isHover = false)
		{
			if (isHover)
			{
				return Color.FromHexa ("ebe8d6");  // gris-jaune
			}
			else
			{
				return Color.FromBrightness (even ? 0.95 : 0.90);
			}
		}

		public static Color TextColor
		{
			get
			{
				return Color.FromBrightness (0.2);
			}
		}

		public static Color GlyphColor
		{
			get
			{
				return Color.FromBrightness (0.2);
			}
		}

		public static Color TreeTableBackgroundColor
		{
			get
			{
				return Color.FromBrightness (0.90);
			}
		}

		public static Color GetTreeTableBackgroundDockToLeftColor(bool isHover = false)
		{
			if (isHover)
			{
				return Color.FromHexa ("ebe8d6");  // gris-jaune
			}
			else
			{
				return Color.FromBrightness (0.97);
			}
		}

		public static Color ValueDotColor
		{
			get
			{
				return Color.FromBrightness (0.5);
			}
		}

		public static Color ValueSurfaceColor
		{
			get
			{
				return Color.FromBrightness (0.9);
			}
		}

		public static Color SelectionColor
		{
			get
			{
				return Color.FromHexa ("88c8ff");  // bleu
			}
		}

		public static Color HoverColor
		{
			get
			{
				return Color.FromHexa ("fff088");  // jaune
			}
		}
	}
}
