//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;

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

			
			var list = new List<string> ();

			if (!string.IsNullOrEmpty (this.SellingBookAccount))
			{
				list.Add (this.SellingBookAccount);
			}

			if (!string.IsNullOrEmpty (this.SellingDiscountBookAccount))
			{
				list.Add (this.SellingDiscountBookAccount);
			}

			if (!string.IsNullOrEmpty (this.PurchaseBookAccount))
			{
				list.Add (this.PurchaseBookAccount);
			}

			if (!string.IsNullOrEmpty (this.PurchaseDiscountBookAccount))
			{
				list.Add (this.PurchaseDiscountBookAccount);
			}

			builder.Append (string.Join ("/", list));


			if (this.BeginDate.HasValue)
			{
				builder.Append ("du");
				builder.Append (Misc.GetDateTimeShortDescription (this.BeginDate));
			}

			if (this.EndDate.HasValue)
			{
				builder.Append ("au");
				builder.Append (Misc.GetDateTimeShortDescription (this.EndDate));
			}


			if (this.CurrencyCode.HasValue)
			{
				var c = Business.Enumerations.GetAllPossibleCurrencyCodes ().Where (x => x.Key == this.CurrencyCode).First ();
				builder.Append (c.Values[0]);  // code de la monnaie, par exemple "CHF"
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
