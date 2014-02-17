//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Assets.Server.NodeGetters;
namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class SortedColumn
	{
		public SortedColumn(int column, SortedType type)
		{
			this.Column = column;
			this.Type   = type;
		}

		public bool								IsEmpty
		{
			get
			{
				return this.Column == -1
					&& this.Type   == SortedType.None;
			}
		}

		public static SortedColumn Empty = new SortedColumn (-1, SortedType.None);

		public readonly int						Column;		// 0..n
		public readonly SortedType				Type;
	};
}
