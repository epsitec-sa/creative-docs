//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	/// <summary>
	/// The <c>TaxCalculator</c> class knows how to compute the tax based on
	/// a date or a date range, given an amount and a VAT code. It relies on
	/// the <see cref="TaxContext"/> to retrieve <see cref="VatDefinitionEntity"/>
	/// instances.
	/// </summary>
	public class TaxCalculator
	{
		public TaxCalculator(CoreData data, Date date)
		{
			this.data = data;
			this.date = date;
		}

		public TaxCalculator(CoreData data, IDateRange dateRange)
		{
			this.data = data;
			this.dateRange = dateRange;
		}


		/// <summary>
		/// Computes the tax based on the specified amount and VAT code. Uses the date
		/// specified in the constructor. There will be exactly one output for every
		/// rate we find.
		/// </summary>
		/// <param name="amount">The amount.</param>
		/// <param name="vatRateType">The VAT rate type.</param>
		/// <returns>The computed tax.</returns>
		public Tax ComputeTax(decimal amount, VatRateType vatRateType)
		{
			var taxContext = this.data.GetComponent<TaxContext> ();

			if (this.date.HasValue)
			{
				var vatDef = taxContext.GetVatDefinition (this.date.Value, vatRateType);

				return vatDef == null ? new Tax () : new Tax (new TaxRateAmount (amount, vatRateType, vatDef.Rate));
			}

			if (this.dateRange != null)
			{
				VatDefinitionEntity[] vatDefs = taxContext.GetVatDefinitions (this.dateRange, vatRateType);

				if (vatDefs.Length == 0)
				{
					if (taxContext.GetVatDefinitions (vatRateType).IsEmpty ())
					{
						System.Diagnostics.Debug.WriteLine ("Cannot find VAT rate for VatCode." + vatRateType);
						return new Tax ();
					}
				}

				var durationRates = (from vatDef in vatDefs
									 group vatDef by vatDef.Rate into sameRateVatDef
									 select new
									 {
										 Rate     = sameRateVatDef.Key,
										 Duration = sameRateVatDef.Sum (vatDefItem => this.dateRange.GetIntersection (vatDefItem).GetDuration ())
									 }).ToArray ();

				int totalDays  = durationRates.Sum (x => x.Duration);

				if (totalDays == 0)
				{
					System.Diagnostics.Debug.WriteLine ("Cannot find VAT rate for specified date range");
					return new Tax ();
				}

				var taxes = from range in durationRates
							select new TaxRateAmount (amount * range.Duration / totalDays, vatRateType, range.Rate);

				return new Tax (taxes);
			}

			return new Tax ();
		}


		private readonly CoreData				data;
		private readonly Date?					date;
		private readonly IDateRange				dateRange;
	}
}