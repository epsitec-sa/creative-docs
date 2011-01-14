//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

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


		public CresusChartOfAccounts GetRecentChartOfAccounts(BusinessContext businessContext)
		{
			//	Retourne le plan comptable correspondant à la date de référence de la transaction 'business'
			//	en cours, ou le plan comptable le plus récent.
			//	Retourne null s'il n'existe aucun plan comptable.
			var date = businessContext.GetReferenceDate ();
			return this.GetRecentChartOfAccounts (date);
		}

		private CresusChartOfAccounts GetRecentChartOfAccounts(Date date)
		{
			//	Retourne le plan comptable correspondant à une date, ou le plan comptable le plus récent.
			//	Retourne null s'il n'existe aucun plan comptable.
			var chart = this.GetChartOfAccounts (date);

			if (chart == null)
			{
				Date max = new Date (0);  // date très ancienne

				foreach (var c in this.GetChartsOfAccounts ())
				{
					if (max < c.EndDate)
					{
						max = c.EndDate;
						chart = c;
					}
				}
			}

			return chart;
		}

		private CresusChartOfAccounts GetChartOfAccounts(Date date)
		{
			//	Retourne le plan comptable correspondant à une date.
			//	Retourne null si aucun plan comptable ne correspond.
			this.EnsureThatChartsOfAccountsAreDeserialized ();

			return this.GetChartsOfAccounts ().Where (chart => date >= chart.BeginDate && date <= chart.EndDate).FirstOrDefault ();
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

			var repository = new XmlBlobRepository (businessContext.Data, businessContext.DataContext);
			var blob = repository.GetByCode (chartId).FirstOrDefault ();
			if (blob != null)
			{
				businessContext.DataContext.DeleteEntity (blob);
			}

			this.SerializedChartsOfAccounts.Remove (blob);
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
