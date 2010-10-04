//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			using (var a = new EntityStatusAccumulator ())
			{
				using (var b = new EntityStatusAccumulator (atLeastOne: true))
				{
					b.Accumulate (this.Firstname.GetEntityStatus ());
					b.Accumulate (this.Lastname.GetEntityStatus ());

					a.Accumulate (b.EntityStatus);
				}

				a.Accumulate (this.Title.GetEntityStatus ().TreatAsOptional ());

				return a.EntityStatus;
			}
		}
	}
}
