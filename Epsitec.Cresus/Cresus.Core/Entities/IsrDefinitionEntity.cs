//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;
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
			var s1 = this.SubscriberNumber.GetEntityStatus ();
			var s2 = this.SubscriberAddress.GetEntityStatus ();
			var s3 = this.BankReferenceNumberPrefix.GetEntityStatus ().TreatAsOptional ();
			var s4 = this.BankAddressLine1.GetEntityStatus ().TreatAsOptional ();
			var s5 = this.BankAddressLine2.GetEntityStatus ().TreatAsOptional ();
			var s6 = this.BankAccount.GetEntityStatus ().TreatAsOptional ();
			var s7 = this.IncomingBookAccount.GetEntityStatus ().TreatAsOptional ();

			return EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, s1, s2, s3, s4, s5, s6, s7);
		}
	}
}
