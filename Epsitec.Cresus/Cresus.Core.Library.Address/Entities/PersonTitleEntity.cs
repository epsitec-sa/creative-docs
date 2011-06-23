//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class PersonTitleEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
				(
					"Complet: ", this.Name, "\n",
					"Abrégé: ",  this.ShortName
				);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name, "(", this.ShortName, ")");
		}

		public override string[] GetEntityKeywords()
		{
			return new string[] { this.Name.ToSimpleText (), this.ShortName.ToSimpleText () };
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.ShortName.GetEntityStatus ());
				a.Accumulate (this.Name.GetEntityStatus ());

				return a.EntityStatus;
			}
		}
	}
}
