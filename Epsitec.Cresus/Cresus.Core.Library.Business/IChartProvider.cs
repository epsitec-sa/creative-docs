using Epsitec.Cresus.Core.Business.Accounting;

namespace Epsitec.Cresus.Core.Business
{
	// This interface is an ugly hack, as explained in UIBuilder, around line 637.

	interface IChartProvider
	{
		CresusChartOfAccounts GetChart();
	}
}
