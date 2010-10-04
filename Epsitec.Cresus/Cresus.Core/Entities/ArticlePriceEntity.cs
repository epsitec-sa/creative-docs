//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class ArticlePriceEntity
	{
		public override FormattedText GetSummary()
		{
			var builder = new TextBuilder ();

			builder.Append (Misc.PriceToString (this.ValueBeforeTax));

			if (this.CurrencyCode.HasValue)
			{
				var c = Business.Enumerations.GetAllPossibleCurrencyCodes ().Where (x => x.Key == this.CurrencyCode).First ();
				builder.Append (" ");
				builder.Append (c.Values[0]);  // code de la monnaie, par exemple "CHF"
			}

			if (this.BeginDate.HasValue)
			{
				builder.Append (" du ");
				builder.Append (Misc.GetDateTimeShortDescription (this.BeginDate));
			}

			if (this.EndDate.HasValue)
			{
				builder.Append (" au ");
				builder.Append (Misc.GetDateTimeShortDescription (this.EndDate));
			}

			return builder.ToFormattedText ();
		}

		public override EntityStatus GetEntityStatus()
		{
			return EntityStatus.Valid;
		}
	}
}
