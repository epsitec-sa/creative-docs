//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public struct Warning
	{
		public Warning(BaseType baseType, Guid objectGuid, Guid eventGuid, ObjectField field, string description)
		{
			this.Guid        = Guid.NewGuid ();

			this.BaseType    = baseType;
			this.ObjectGuid  = objectGuid;
			this.EventGuid   = eventGuid;
			this.Field       = field;
			this.Description = description;
		}

		public bool IsEmpty
		{
			get
			{
				return this.BaseType == BaseType.Unknown
					&& this.ObjectGuid.IsEmpty
					&& this.EventGuid.IsEmpty
					&& this.Field == ObjectField.Unknown
					&& string.IsNullOrEmpty (this.Description);
			}
		}

		public static Warning Empty = new Warning (BaseType.Unknown, Guid.Empty, Guid.Empty, ObjectField.Unknown, null);

		public readonly Guid				Guid;
		public readonly BaseType			BaseType;
		public readonly Guid				ObjectGuid;
		public readonly Guid				EventGuid;
		public readonly ObjectField			Field;
		public readonly string				Description;
	}
}
