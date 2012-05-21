//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing.Platform
{
	internal struct Color32
	{
		public Color32(byte[] array, int offset)
		{
			this.Blue  = array[offset+0];
			this.Green = array[offset+1];
			this.Red   = array[offset+2];
			this.Alpha = array[offset+3];

		}

		public byte Blue;
		public byte Green;
		public byte Red;
		public byte Alpha;

		public int ARGB
		{
			get
			{
				return ((((((this.Alpha << 8) | this.Red) << 8) | this.Green) << 8) | this.Blue);
			}
		}
	}
}
