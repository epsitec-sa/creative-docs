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
	public partial class ArticleAccountingDefinitionEntity
	{
		public override FormattedText GetSummary()
		{
			return GetCompactSummary ();
		}

		public override FormattedText GetCompactSummary()
		{
			var builder = new TextBuilder ();

			if (this.BeginDate.HasValue || this.EndDate.HasValue)
			{
				if (this.BeginDate.HasValue)
				{
					builder.Append ("Du");
					builder.Append (Misc.GetDateShortDescription (this.BeginDate));
				}

				if (this.EndDate.HasValue)
				{
					builder.Append ("au");
					builder.Append (Misc.GetDateShortDescription (this.EndDate));
				}
			}

			if (this.CurrencyCode.HasValue)
			{
				var c = EnumKeyValues.FromEnum<CurrencyCode> ().Where (x => x.Key == this.CurrencyCode).First ();
				builder.Append (c.Values[0]);  // code de la monnaie, par exemple "CHF"
			}

			builder.Append ("<br/>");

			if (string.IsNullOrEmpty (this.SellingBookAccount))
			{
				builder.Append ("—");
			}
			else
			{
				builder.Append (this.SellingBookAccount);
			}

			builder.Append ("/");

			if (string.IsNullOrEmpty (this.SellingDiscountBookAccount))
			{
				builder.Append ("—");
			}
			else
			{
				builder.Append (this.SellingDiscountBookAccount);
			}

			builder.Append ("/");

			if (string.IsNullOrEmpty (this.PurchaseBookAccount))
			{
				builder.Append ("—");
			}
			else
			{
				builder.Append (this.PurchaseBookAccount);
			}

			builder.Append ("/");

			if (string.IsNullOrEmpty (this.PurchaseDiscountBookAccount))
			{
				builder.Append ("—");
			}
			else
			{
				builder.Append (this.PurchaseDiscountBookAccount);
			}

			return builder.ToFormattedText ();
		}


		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.SellingBookAccount.GetEntityStatus ());
				a.Accumulate (this.SellingDiscountBookAccount.GetEntityStatus ());

				a.Accumulate (this.PurchaseBookAccount.GetEntityStatus ());
				a.Accumulate (this.PurchaseDiscountBookAccount.GetEntityStatus ());

				return a.EntityStatus;
			}
		}
	}
}
