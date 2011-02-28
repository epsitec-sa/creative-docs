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
		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.StreetName.GetEntityStatus ());
				a.Accumulate (this.Complement.GetEntityStatus ().TreatAsOptional ());

				return a.EntityStatus;
			}
		}
	}
}
