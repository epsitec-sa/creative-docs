//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class ArticleDefinitionEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
				(
					"N°~", this.IdA, "\n",
					this.ShortDescription
				);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.IdA, "~-", this.ShortDescription);
		}

		public override string[] GetEntityKeywords()
		{
			return new string[] { this.IdA, this.ShortDescription.ToSimpleText () };
		}

		public override EntityStatus GetEntityStatus()
		{
			var s1 = this.IdA.GetEntityStatus ();
			var s2 = this.IdB.GetEntityStatus ().TreatAsOptional ();
			var s3 = this.IdC.GetEntityStatus ().TreatAsOptional ();
			var s4 = this.ShortDescription.GetEntityStatus ();
			var s5 = this.LongDescription.GetEntityStatus ().TreatAsOptional ();

			return EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, s1, s2, s3, s4, s5);
		}
	}
}
