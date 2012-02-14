//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities.Helpers;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderPersonWarningEntity
	{
		public override FormattedText GetTitle()
		{
			return AiderWarningImplementation.GetTitle (this);
		}

		public override FormattedText GetSummary()
		{
			return AiderWarningImplementation.GetSummary (this);
		}

		public override FormattedText GetCompactSummary()
		{
			return AiderWarningImplementation.GetCompactSummary (this);
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				AiderWarningImplementation.Accumulate (this, a);

				a.Accumulate (this.Person.IsNull () ? EntityStatus.Empty : EntityStatus.Valid);

				return a.EntityStatus;
			}
		}
	}
}
