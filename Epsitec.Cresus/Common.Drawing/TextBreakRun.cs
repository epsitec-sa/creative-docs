//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// The <c>TextBreakRun</c> structure defines a text run (a series of
	/// characters which share the same font and scale).
	/// </summary>
	public struct TextBreakRun
	{
		public TextBreakRun(Font font, int length, double scale, string locale)
		{
			this.font = font;
			this.length = length;
			this.scale = scale;
			this.locale = locale;
		}

		public Font Font
		{
			get
			{
				return this.font;
			}
		}

		public int Length
		{
			get
			{
				return this.length;
			}
		}

		public double Scale
		{
			get
			{
				return this.scale;
			}
		}

		public string Locale
		{
			get
			{
				return this.locale;
			}
		}
		
		private Font							font;
		private int								length;
		private double							scale;
		private string							locale;
	}
}
