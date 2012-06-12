//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class IsrDefinitionEntity
	{
		public override FormattedText GetSummary()
		{
			var builder = new TextBuilder ();

			builder.Append (Isr.FormatSubscriberNumber (this.SubscriberNumber));

			return builder.ToFormattedText ();
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.SubscriberNumber.GetEntityStatus ());
				a.Accumulate (this.SubscriberAddress.GetEntityStatus ());
				a.Accumulate (this.BankReferenceNumberPrefix.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.BankAddressLine1.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.BankAddressLine2.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.BankAccount.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.IncomingBookAccount.GetEntityStatus ().TreatAsOptional ());

				return a.EntityStatus;
			}
		}
	}
}
