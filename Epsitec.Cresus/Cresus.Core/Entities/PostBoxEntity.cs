//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class PostBoxEntity
	{
		public override EntityStatus EntityStatus
		{
			get
			{
				bool ok1 = !this.Number.IsNullOrWhiteSpace;

				if (ok1)
				{
					return EntityStatus.Valid;
				}

				return EntityStatus.Empty;
			}
		}
	}
}
