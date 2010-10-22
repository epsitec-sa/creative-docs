//	Copyright © 2004-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Helpers
{
	public sealed class HypertextInfo : System.ICloneable, System.IComparable
	{
		internal HypertextInfo(TextLayout layout, Drawing.Rectangle bounds, int index)
		{
			this.layout = layout;
			this.bounds = bounds;
			this.index  = index;
		}


		#region ICloneable Members
		public object Clone()
		{
			return new HypertextInfo (this.layout, this.bounds, this.index);
		}
		#endregion

		#region IComparable Members
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}

			HypertextInfo that = obj as HypertextInfo;

			if ((that == null) || (that.layout != this.layout))
			{
				throw new System.ArgumentException ("Invalid argument");
			}

			return this.index.CompareTo (that.index);
		}
		#endregion

		public override bool Equals(object obj)
		{
			return this.CompareTo (obj) == 0;
		}

		public override int GetHashCode()
		{
			return this.index;
		}


		public Drawing.Rectangle Bounds
		{
			get
			{
				return this.bounds;
			}
		}

		public string Anchor
		{
			get
			{
				return this.layout.FindAnchor (this.index);
			}
		}


		private TextLayout				layout;
		private Drawing.Rectangle		bounds;
		private int						index;
	}
}
