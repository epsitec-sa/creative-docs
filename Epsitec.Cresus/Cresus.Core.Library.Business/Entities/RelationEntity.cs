//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class RelationEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
				(
					"N°", this.IdA, "\n",
					this.Person.GetSummary (), "\n",
					"Représentant: ~", this.SalesRepresentative.GetCompactSummary ()
				);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Person.GetCompactSummary ());
		}

		public override string[] GetEntityKeywords()
		{
			return this.Person.GetEntityKeywords ();
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.IdA.GetEntityStatus ());
				a.Accumulate (this.IdB.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.IdC.GetEntityStatus ().TreatAsOptional ());

				a.Accumulate (this.Person.GetEntityStatus ());
				a.Accumulate (this.Affairs.Select (x => x.GetEntityStatus ()));
				a.Accumulate (this.Comments.Select (x => x.GetEntityStatus ()));
				a.Accumulate (this.DefaultAddress.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.SalesRepresentative.GetEntityStatus ().TreatAsOptional ());

				return a.EntityStatus;
			}
		}
	}
}
