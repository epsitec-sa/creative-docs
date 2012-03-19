//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public sealed class ItemListRow
	{
		public ItemListRow(int index, int offset, int height)
		{
			this.index  = index;
			this.offset = offset;
			this.height = height;
		}

		public int Index
		{
			get
			{
				return this.index;
			}
		}

		public int Offset
		{
			get
			{
				return this.offset;
			}
		}

		public int Height
		{
			get
			{
				return this.height;
			}
		}

		private int index;
		private int offset;
		private int height;
	}
}
