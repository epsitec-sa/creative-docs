//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// The <c>FontClassInfo</c> structure stores width and stretchability
	/// information about a range of glyphs.
	/// </summary>
	public struct FontClassInfo
	{
		public FontClassInfo(GlyphClass id, int count, double width, double elasticity)
		{
			this.classId   = id;
			this.count      = count;
			this.width      = width;
			this.elasticity = elasticity;
			this.scale      = 1.0;
		}


		public GlyphClass						GlyphClass
		{
			get
			{
				return this.classId;
			}
		}

		public int								Count
		{
			get
			{
				return this.count;
			}
		}

		public double							Width
		{
			get
			{
				return this.width;
			}
		}

		public double							Elasticity
		{
			get
			{
				return this.elasticity;
			}
		}

		public double							Scale
		{
			get
			{
				return this.scale;
			}
			set
			{
				this.scale = value;
			}
		}


		private GlyphClass						classId;
		private int								count;				//	number of glyphs belonging to this class
		private double							width;				//	accumulated glyph width
		private double							elasticity;			//	glyph elasticity
		private double							scale;				//	horizontal glyph scale
	}
}
