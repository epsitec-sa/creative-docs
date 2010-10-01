//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class StreetEntity
	{
		public override EntityStatus EntityStatus
		{
			get
			{
				bool ok1 = !this.StreetName.IsNullOrWhiteSpace;
				bool ok2 = !this.Complement.IsNullOrWhiteSpace;

				if (!ok1 && !ok2)
				{
					return EntityStatus.Empty;
				}

				if (ok1)
				{
					return EntityStatus.Valid;
				}

				return EntityStatus.Invalid;
			}
		}
	}
}
