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

		public override EntityStatus EntityStatus
		{
			get
			{
				var s1 = EntityStatusHelper.GetStatus (this.IdA);
				var s2 = EntityStatusHelper.Optional (EntityStatusHelper.GetStatus (this.IdB));
				var s3 = EntityStatusHelper.Optional (EntityStatusHelper.GetStatus (this.IdC));
				var s4 = EntityStatusHelper.GetStatus (this.ShortDescription);
				var s5 = EntityStatusHelper.Optional (EntityStatusHelper.GetStatus (this.LongDescription));

				return EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, s1, s2, s3, s4, s5);
			}
		}
	}
}
