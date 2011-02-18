//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>MultiSelectEventArgs</c> class represents a selection in the
	/// <see cref="ScrollListMultiSelect"/>, which is defined by two indexes.
	/// </summary>
	public class MultiSelectEventArgs : Support.EventArgs
	{
		public MultiSelectEventArgs()
		{
			this.BeginIndex = -1;
			this.EndIndex   = -1;
		}

		public MultiSelectEventArgs(int beginIndex, int endIndex)
		{
			this.BeginIndex = beginIndex;
			this.EndIndex   = endIndex;
		}


		public int BeginIndex
		{
			get;
			set;
		}

		public int EndIndex
		{
			get;
			set;
		}

		public int Count
		{
			get
			{
				if ((this.BeginIndex < 0) ||
					(this.EndIndex < this.BeginIndex))
				{
					return 0;
				}
				else
				{
					return this.EndIndex - this.BeginIndex + 1;
				}
			}
		}
	}
}
