//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public struct ItemListMarkOffset
	{
		public ItemListMarkOffset(int offset)
		{
			this.offset = offset ^ ItemListMarkOffset.Valid;
		}


		public bool IsVisible
		{
			get
			{
				return (this.Offset > System.Int32.MinValue)
					&& (this.Offset < System.Int32.MaxValue);
			}
		}

		public bool IsBefore
		{
			get
			{
				return this.Offset == System.Int32.MinValue;
			}
		}

		public bool IsAfter
		{
			get
			{
				return this.Offset == System.Int32.MaxValue;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.offset == 0;
			}
		}

		public int Offset
		{
			get
			{
				if (this.IsEmpty)
				{
					return 0;
				}

				return this.offset ^ ItemListMarkOffset.Valid;
			}
		}

		public static readonly ItemListMarkOffset Empty  = new ItemListMarkOffset ();
		public static readonly ItemListMarkOffset Before = new ItemListMarkOffset (System.Int32.MinValue);
		public static readonly ItemListMarkOffset After  = new ItemListMarkOffset (System.Int32.MaxValue);



		public override string ToString()
		{
			if (this.IsBefore)
			{
				return "<";
			}
			if (this.IsAfter)
			{
				return ">";
			}
			if (this.IsEmpty)
			{
				return "*";
			}

			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}", this.offset);
		}


		private const int Valid = 0x10000000;
		
		private readonly int offset;
	}
}
