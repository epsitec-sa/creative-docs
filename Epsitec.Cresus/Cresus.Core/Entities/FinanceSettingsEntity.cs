//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business.Accounting;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class FinanceSettingsEntity
	{
		public IList<CresusChartOfAccounts> GetChartsOfAccounts()
		{
			this.EnsureThatChartsOfAccountsAreDeserialized ();
			
			return this.chartsOfAccounts.AsReadOnly ();
		}

		public void AddChartOfAccounts(CresusChartOfAccounts chart)
		{
			this.EnsureThatChartsOfAccountsAreDeserialized ();

			string chartId = chart.Id.ToString ("D");

			//	TODO: ajouter un XmlBlob dans SerializedChartsOfAccounts
			//	Le lien entre XmlBlob et chart se fait par leur 'Id'
			throw new System.NotImplementedException ();
		}

		public void RemoveChartOfAccounts(CresusChartOfAccounts chart)
		{
			this.EnsureThatChartsOfAccountsAreDeserialized ();

			string chartId = chart.Id.ToString ("D");

			//	TODO: ajouter un XmlBlob dans SerializedChartsOfAccounts
			throw new System.NotImplementedException ();
		}


		private void EnsureThatChartsOfAccountsAreDeserialized()
		{
			if (this.chartsOfAccounts == null)
			{
				this.chartsOfAccounts = new List<CresusChartOfAccounts> ();
				//	TODO: désérialiser les CresusChartOfAccounts basés sur le XML stocké dans les XmlBlobs
				throw new System.NotImplementedException ();
			}
		}

		private List<CresusChartOfAccounts> chartsOfAccounts;
	}
}
