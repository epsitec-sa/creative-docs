//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class ArticlePriceEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (
				this.Value, TextFormatter.FormatCommand ("#price()"), this.CurrencyCode, "(", this.MinQuantity, "0", TextFormatter.Command.IfEmpty, "à", this.MaxQuantity, "∞", TextFormatter.Command.IfEmpty, ")", "\n",
				"Du ", this.BeginDate, "—", TextFormatter.Command.IfEmpty, " au ", this.EndDate, "—", TextFormatter.Command.IfEmpty);
		}

		public override EntityStatus GetEntityStatus()
		{
			return EntityStatus.Valid;
		}
	}
}
