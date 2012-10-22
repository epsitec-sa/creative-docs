using Epsitec.Cresus.Core.Business.Accounting;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Cresus.Core.Business
{
	// This class is an ugly hack, as explained in UIBuilder, around line 637.

	public class ChartProvider : IChartProvider
	{
		public ChartProvider(BusinessContext businessContext)
		{
			this.businessContext = businessContext;
		}

		public CresusChartOfAccounts GetChart()
		{
			var businessSettings = this.businessContext.GetCached<BusinessSettingsEntity> ();
			var financeSettings = businessSettings.Finance;

			System.Diagnostics.Debug.Assert (financeSettings != null);

			var referenceDate = this.businessContext.GetReferenceDate ();

			return financeSettings.GetChartOfAccountsOrDefaultToNearest (referenceDate);
		}

		private readonly BusinessContext businessContext;
	}
}