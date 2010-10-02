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

		public override EntityStatus EntityStatus
		{
			get
			{
				var s1 = EntityStatusHelper.GetStatus (this.IdA);
				var s2 = EntityStatusHelper.Optional (EntityStatusHelper.GetStatus (this.IdB));
				var s3 = EntityStatusHelper.Optional (EntityStatusHelper.GetStatus (this.IdC));
				var s4 = this.Person.EntityStatus;
				var s5 = EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, this.Affairs.Select (x => x.EntityStatus).ToArray ());
				var s6 = EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, this.Comments.Select (x => x.EntityStatus).ToArray ());
				var s7 = EntityStatusHelper.Optional (this.DefaultAddress.EntityStatus);
				var s8 = EntityStatusHelper.Optional (this.SalesRepresentative.EntityStatus);

				return EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, s1, s2, s3, s4, s5, s6, s7, s8);
			}
		}
	}
}
