//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;

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
			var s1 = this.IdA.GetEntityStatus ();
			var s2 = this.IdB.GetEntityStatus ().TreatAsOptional ();
			var s3 = this.IdC.GetEntityStatus ().TreatAsOptional ();
			var s4 = this.Person.GetEntityStatus ();
			var s5 = EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, this.Affairs.Select (x => x.GetEntityStatus ()).ToArray ());
			var s6 = EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, this.Comments.Select (x => x.GetEntityStatus ()).ToArray ());
			var s7 = this.DefaultAddress.GetEntityStatus ().TreatAsOptional ();
			var s8 = this.SalesRepresentative.GetEntityStatus ().TreatAsOptional ();

			return EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, s1, s2, s3, s4, s5, s6, s7, s8);
		}
	}
}
