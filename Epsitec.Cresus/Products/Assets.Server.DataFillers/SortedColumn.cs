//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class SortedColumn
	{
		public SortedColumn(ObjectField field, SortedType type)
		{
			this.Field = field;
			this.Type  = type;
		}

		public bool								IsEmpty
		{
			get
			{
				return this.Field == ObjectField.Unknown
					&& this.Type  == SortedType.None;
			}
		}

		public static SortedColumn Empty = new SortedColumn (ObjectField.Unknown, SortedType.None);

		public readonly ObjectField				Field;
		public readonly SortedType				Type;
	};
}
