//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
					this.Person.GetSummary ()
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
				a.Accumulate (this.Person.GetEntityStatus ());
				a.Accumulate (this.Comments.Select (x => x.GetEntityStatus ()));
				a.Accumulate (this.DefaultMailContact.GetEntityStatus ().TreatAsOptional ());

				return a.EntityStatus;
			}
		}
	}
}
