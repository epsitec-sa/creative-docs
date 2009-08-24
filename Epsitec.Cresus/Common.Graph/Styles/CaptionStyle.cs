//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Graph.Styles
{
	public class CaptionStyle
	{
		public CaptionStyle()
		{
			this.Font = Font.GetFont ("Calibri", "Regular");
			this.FontSize = 12;
			this.FontColor = Color.FromBrightness (0);
		}

		
		public Font Font
		{
			get;
			set;
		}

		public double FontSize
		{
			get;
			set;
		}

		public Color FontColor
		{
			get;
			set;
		}


		public double GetTextWidth(string text)
		{
			return System.Math.Ceiling (this.Font.GetTextAdvance (text) * this.FontSize);
		}

		public double GetTextLineHeight()
		{
			return System.Math.Ceiling (this.Font.LineHeight * this.FontSize * 1.2);
		}

		public Point GetTextLineOffset()
		{
			var ht = this.GetTextLineHeight ();
			var h1 = System.Math.Ceiling (ht/10 - this.Font.Descender * this.FontSize);

			return new Point (0, h1);
		}
	}
}
