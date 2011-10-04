//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

			builder.Append (this.Name);
			
			var c = EnumKeyValues.GetEnumKeyValue<CurrencyCode> (this.CurrencyCode);
			builder.Append (FormattedText.Concat ("(", c.Values[0], ")"));  // code de la monnaie, par exemple "CHF"
			builder.Append ("<br/>");

			builder.Append (TextFormatter.FormatText (this.AccountingOperation.GetSummary ()));
			builder.Append ("<br/>");

			if (this.BeginDate.HasValue || this.EndDate.HasValue)
			{
				builder.Append ("Du");
				builder.Append (Misc.GetDateShortDescription (this.BeginDate) ?? " ...");
				builder.Append ("au");
				builder.Append (Misc.GetDateShortDescription (this.EndDate) ?? " ...");
			}

			builder.Append (ArticleAccountingDefinitionEntity.GetBookAccountText (this.TransactionBookAccount));
			builder.Append ("/");
			builder.Append (ArticleAccountingDefinitionEntity.GetBookAccountText (this.DiscountBookAccount));
			builder.Append ("/");
			builder.Append (ArticleAccountingDefinitionEntity.GetBookAccountText (this.RoundingBookAccount));
			builder.Append ("/");
			builder.Append (ArticleAccountingDefinitionEntity.GetBookAccountText (this.VatBookAccount));

			return builder.ToFormattedText ();
		}

		private static FormattedText GetBookAccountText(string bookAccount)
		{
			if (string.IsNullOrWhiteSpace (bookAccount))
			{
				return new FormattedText ("-");
			}
			else
			{
				return FormattedText.FromSimpleText (bookAccount);
			}
		}


		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Name.GetEntityStatus ());
				a.Accumulate (this.Description.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.AccountingOperation.GetEntityStatus ());
				a.Accumulate (this.TransactionBookAccount.GetEntityStatus ());
				a.Accumulate (this.DiscountBookAccount.GetEntityStatus ());
				a.Accumulate (this.RoundingBookAccount.GetEntityStatus ());
				a.Accumulate (this.VatBookAccount.GetEntityStatus ());

				a.Accumulate (this.CurrencyCode == CurrencyCode.None ? EntityStatus.Empty : EntityStatus.Valid);

				return a.EntityStatus;
			}
		}
	}
}
