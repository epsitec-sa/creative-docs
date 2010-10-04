//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class NaturalPersonEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
				(
					this.Title.Name, "\n",
					this.Firstname, this.Lastname, "(", this.Gender.Name, ")", "\n",
					this.BirthDate
				);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Firstname, this.Lastname);
		}

		public override string[] GetEntityKeywords()
		{
			return new string[] { this.Firstname.ToSimpleText (), this.Lastname.ToSimpleText () };
		}

		public override EntityStatus GetEntityStatus()
		{
			var s1 = this.Firstname.GetEntityStatus ();
			var s2 = this.Lastname.GetEntityStatus ();
			var s3 = this.Title.GetEntityStatus ().TreatAsOptional ();

			var s12 = EntityStatusHelper.CombineStatus (StatusHelperCardinality.AtLeastOne, s1, s2);
			return EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, s12, s3);
		}
	}
}
