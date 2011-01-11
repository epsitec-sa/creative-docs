//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business.Accounting;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.Core.Repositories;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class FinanceSettingsEntity
	{
		public IList<CresusChartOfAccounts> GetChartsOfAccounts()
		{
			this.EnsureThatChartsOfAccountsAreDeserialized ();
			
			return this.chartsOfAccounts.AsReadOnly ();
		}

		public void AddChartOfAccounts(BusinessContext businessContext, CresusChartOfAccounts chart)
		{
			this.EnsureThatChartsOfAccountsAreDeserialized ();

			string chartId = chart.Id.ToString ("D");
			var xml = chart.SerializeToXml ("chartOfAccounts");

			var blob = businessContext.DataContext.CreateEntity<XmlBlobEntity> ();
			blob.Code = chartId;
			blob.XmlData = xml;

			this.SerializedChartsOfAccounts.Add (blob);
			this.chartsOfAccounts.Add (chart);
		}

		public void RemoveChartOfAccounts(BusinessContext businessContext, CresusChartOfAccounts chart)
		{
			this.EnsureThatChartsOfAccountsAreDeserialized ();

			string chartId = chart.Id.ToString ("D");

			XmlBlobRepository repository = new XmlBlobRepository (businessContext.Data, businessContext.DataContext);
			var blob = repository.GetByCode (chartId).FirstOrDefault ();
			if (blob != null)
			{
				businessContext.DataContext.DeleteEntity (blob);
			}
			
			this.chartsOfAccounts.Remove (chart);
		}


		private void EnsureThatChartsOfAccountsAreDeserialized()
		{
			if (this.chartsOfAccounts == null)
			{
				this.chartsOfAccounts = new List<CresusChartOfAccounts> ();

				foreach (var blob in this.SerializedChartsOfAccounts)
				{
					var cresusChartOfAccounts = CresusChartOfAccounts.DeserializeFromXml (blob.XmlData);
					this.chartsOfAccounts.Add (cresusChartOfAccounts);
				}
			}
		}


		private List<CresusChartOfAccounts>		chartsOfAccounts;
	}
}
