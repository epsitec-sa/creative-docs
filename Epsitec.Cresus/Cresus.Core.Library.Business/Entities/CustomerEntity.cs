//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class CustomerEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
				(
					"N°", this.IdA, "\n",
					this.Relation.GetSummary ()
				);
		}

		public override FormattedText GetCompactSummary()
		{
			return this.Relation.GetCompactSummary ();
		}

		public override string[] GetEntityKeywords()
		{
			return this.Relation.GetEntityKeywords ();
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.IdA.GetEntityStatus ());
				a.Accumulate (this.IdB.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.IdC.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.Relation.GetEntityStatus ());

				return a.EntityStatus;
			}
		}
	}
}
