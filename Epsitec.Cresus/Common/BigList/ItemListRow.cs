//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public sealed class ItemListRow
	{
		public ItemListRow(int index, int offset, ItemHeight height, bool isLast)
		{
			this.index  = index;
			this.offset = offset;
			this.height = height;
			this.isLast = isLast;
		}

		
		public int								Index
		{
			get
			{
				return this.index;
			}
		}

		public int								Offset
		{
			get
			{
				return this.offset;
			}
		}

		public ItemHeight						Height
		{
			get
			{
				return this.height;
			}
		}

		public bool								IsLast
		{
			get
			{
				return this.isLast;
			}
		}

		public bool								IsFirst
		{
			get
			{
				return this.index == 0;
			}
		}

		public bool								IsEven
		{
			get
			{
				return (this.index & 0x01) == 0x00;
			}
		}

		public bool								IsOdd
		{
			get
			{
				return (this.index & 0x01) == 0x01;
			}
		}

		
		private readonly int					index;
		private readonly int					offset;
		private readonly ItemHeight				height;
		private readonly bool					isLast;
	}
}
