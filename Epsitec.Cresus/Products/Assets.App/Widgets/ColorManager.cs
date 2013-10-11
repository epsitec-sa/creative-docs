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
		public static Color WindowBackgroundColor
		{
			get
			{
				return Color.FromBrightness (0.85);
			}
		}

		public static Color GetBackgroundColor(bool isHover = false)
		{
			if (isHover)
			{
				return ColorManager.hoverColor;
			}
			else
			{
				return Color.FromBrightness (1.0);
			}
		}

		public static Color GetTreeTableDockToLeftBackgroundColor(bool isHover = false)
		{
			if (isHover)
			{
				return ColorManager.hoverColor;
			}
			else
			{
				return Color.FromBrightness (0.97);
			}
		}

		public static Color GetHolidayColor(bool isHover = false)
		{
			if (isHover)
			{
				return ColorManager.hoverColor;
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
				return ColorManager.hoverColor;
			}
			else
			{
				return Color.FromBrightness (1.0);
				//?return Color.FromBrightness (even ? 0.95 : 0.90);
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

		public static Color GridColor
		{
			get
			{
				return Color.FromHexa ("d2ca9b");  // gris-jaune
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
				return Color.FromHexa ("ffd200");  // jaune
			}
		}

		public static Color HoverColor
		{
			get
			{
				return ColorManager.hoverColor;
			}
		}

		public static Color MoveColumnColor
		{
			get
			{
				return ColorManager.HoverColor;
			}
		}


		private static readonly Color hoverColor = Color.FromHexa ("ebe8d6");  // gris-jaune
	}
}
