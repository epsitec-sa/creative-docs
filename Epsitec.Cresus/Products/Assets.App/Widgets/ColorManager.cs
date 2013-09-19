//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public static class ColorManager
	{
		public static Color BackgroundColor
		{
			get
			{
				return Color.FromBrightness (1.0);
			}
		}

		public static Color HolidayColor
		{
			get
			{
				return Color.FromBrightness (0.95);
			}
		}

		public static Color EvenMonthColor
		{
			get
			{
				return Color.FromBrightness (0.90);
			}
		}

		public static Color OddMonthColor
		{
			get
			{
				return Color.FromBrightness (0.95);
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
