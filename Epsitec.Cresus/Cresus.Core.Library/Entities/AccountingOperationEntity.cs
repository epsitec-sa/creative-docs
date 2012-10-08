//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	partial class AccountingOperationEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (
				this.Name, "\n",
				this.Description, "\n",
				this.StandardVatCode, "/", this.ReducedVatCode, "/", this.SpecialVatCode);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Name.GetEntityStatus ());
				a.Accumulate (this.Description.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.StandardVatCode.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.ReducedVatCode.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.SpecialVatCode.GetEntityStatus ().TreatAsOptional ());

				return a.EntityStatus;
			}
		}
	}
}
