//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			return this.Person.GetSummary ();
		}

		public override FormattedText GetCompactSummary()
		{
			return this.Person.GetCompactSummary ();
		}

		public override IEnumerable<FormattedText> GetFormattedEntityKeywords()
		{
			return this.Person.GetFormattedEntityKeywords ();
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Person, EntityStatusAccumulationMode.NoneIsPartiallyCreated);
				a.Accumulate (this.Comments.Select (x => x.GetEntityStatus ()));
				a.Accumulate (this.DefaultMailContact.GetEntityStatus ().TreatAsOptional ());

				return a.EntityStatus;
			}
		}
	}
}
