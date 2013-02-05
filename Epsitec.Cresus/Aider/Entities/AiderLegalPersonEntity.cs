//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers;
using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Entities.Helpers;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderLegalPersonEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Name, "\n", this.Type);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Name.GetEntityStatus ());
				a.Accumulate (this.Comment);
				a.Accumulate ((this.Type == LegalPersonType.None ? EntityStatus.Empty : EntityStatus.Valid).TreatAsOptional ());

				return a.EntityStatus;
			}
		}
	}
}
