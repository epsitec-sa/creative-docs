//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business.Finance;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	partial class VatDefinitionEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (
				this.Name, "\n",
				this.Description, "\n",
				"Type ", this.VatRateType, "\n",
				"Taux ", this.Rate * 100, TextFormatter.FormatCommand ("#string {0:0.0}%"), "\n",
//-				"Date de début : ", this.BeginDate, TextFormatter.FormatCommand ("#string {0:yyyy}"), "inconnue", "...", TextFormatter.Command.IfElseEmpty, "\n",
//-				"Date de fin : ", this.EndDate, TextFormatter.FormatCommand ("#string {0:yyyy}"), "inconnue", "...", TextFormatter.Command.IfElseEmpty, "\n",
				"Du", this.BeginDate, "—", TextFormatter.Command.IfEmpty, "au", this.EndDate, "—", TextFormatter.Command.IfEmpty);
		}
		
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name, "(", this.VatRateType, ") à ", this.Rate * 100, "%");
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Name.GetEntityStatus ());
				a.Accumulate (this.Description.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.BeginDate.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.EndDate.GetEntityStatus ().TreatAsOptional ());
				
				a.Accumulate (this.VatRateType == VatRateType.None ? EntityStatus.Empty : EntityStatus.Valid);

				return a.EntityStatus;
			}
		}
	}
}
