//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	public class TaxCalculator
	{
		public TaxCalculator(Date date)
		{
			this.date = date;
		}

		public TaxCalculator(IDateRange dateRange)
		{
			this.dateRange = dateRange;
		}


		/// <summary>
		/// Gets the tax rates and amounts based on the specified amount and VAT code.
		/// Uses the date specified in the constructor. There will be exactly one output
		/// for every rate we find.
		/// </summary>
		/// <param name="amount">The amount.</param>
		/// <param name="vatCode">The VAT code.</param>
		/// <returns>The collection of tax rates and amounts.</returns>
		public TaxRateAmount[] GetTaxes(decimal amount, VatCode vatCode)
		{
			if (this.date.HasValue)
			{
				var vatDef = TaxContext.Current.GetVatDefinition (this.date.Value, vatCode);

				return new TaxRateAmount[1]
				{
					new TaxRateAmount (vatDef.Rate, amount)
				};
			}

			if (this.dateRange != null)
			{
				var durationRates = (from vatDef in TaxContext.Current.GetVatDefinitions (this.dateRange, vatCode)
									 group vatDef by vatDef.Rate into sameRateVatDef
									 select new
									 {
										 Rate     = sameRateVatDef.Key,
										 Duration = sameRateVatDef.Sum (vatDefItem => this.dateRange.GetIntersection (vatDefItem).GetDuration ())
									 }).ToArray ();

				int totalDays  = durationRates.Sum (x => x.Duration);

				if (totalDays == 0)
				{
					throw new System.InvalidOperationException ("Cannot find VAT rate for specified date range");
				}

				var taxes = from range in durationRates
							select new TaxRateAmount (range.Rate, amount * range.Duration / totalDays);

				return taxes.ToArray ();
			}

			return new TaxRateAmount[0];
		}

		

		private readonly Date?					date;
		private readonly IDateRange				dateRange;
	}
}