//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderGroupParticipantEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Function.Name, "(", this.StartDate, "—", TextFormatter.Command.IfEmpty, "…", this.EndDate, "—", TextFormatter.Command.IfEmpty, ")");
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Function.Name, "(", this.StartDate, "—", TextFormatter.Command.IfEmpty, "…", this.EndDate, "—", TextFormatter.Command.IfEmpty, ")");
		}

		public override IEnumerable<FormattedText> GetFormattedEntityKeywords()
		{
			yield break;
		}
	}
}
