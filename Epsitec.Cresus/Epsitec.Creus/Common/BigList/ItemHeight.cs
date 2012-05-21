//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	/// <summary>
	/// The <c>ItemHeight</c> structure stores the height of an item, including the
	/// external margins (if any), as a compact, 32-bit value.
	/// </summary>
	public struct ItemHeight : System.IEquatable<ItemHeight>
	{
		public ItemHeight(int height, int marginBefore, int marginAfter)
		{
			this.height       = (ushort) height;
			this.marginBefore = (byte) marginBefore;
			this.marginAfter  = (byte) marginAfter;
		}

		public ItemHeight(int height)
		{
			this.height       = (ushort) height;
			this.marginBefore = 0;
			this.marginAfter  = 0;
		}

		
		public int								Height
		{
			get
			{
				return this.height;
			}
		}

		public int								TotalHeight
		{
			get
			{
				return this.Height + this.MarginBefore + this.MarginAfter;
			}
		}

		public int								MarginBefore
		{
			get
			{
				return this.marginBefore;
			}
		}

		public int								MarginAfter
		{
			get
			{
				return this.marginAfter;
			}
		}


		#region IEquatable<ItemHeight> Members

		public bool Equals(ItemHeight other)
		{
			return this.height == other.height
					&& this.marginBefore == other.marginBefore
					&& this.marginAfter == other.marginAfter;
		}

		#endregion

		private readonly ushort					height;
		private readonly byte					marginBefore;
		private readonly byte					marginAfter;
	}
}
