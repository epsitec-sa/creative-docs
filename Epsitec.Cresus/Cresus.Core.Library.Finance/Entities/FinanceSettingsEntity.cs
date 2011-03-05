//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business.Accounting;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.Core.Repositories;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Epsitec.Cresus.Core.Data;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class FinanceSettingsEntity
	{
		public IList<CresusChartOfAccounts> GetChartsOfAccounts()
		{
			this.EnsureThatChartsOfAccountsAreDeserialized ();

			return this.chartsOfAccounts.AsReadOnly ();
		}

		public CresusChartOfAccounts GetRecentChartOfAccounts(Date date)
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

		/// <summary>
		/// Gets the chart of accounts.
		/// </summary>
		/// <param name="date">The date.</param>
		/// <returns></returns>
		private CresusChartOfAccounts GetChartOfAccounts(Date date)
		{
			//	Retourne le plan comptable correspondant à une date.
			//	Retourne null si aucun plan comptable ne correspond.
			this.EnsureThatChartsOfAccountsAreDeserialized ();

			return this.GetChartsOfAccounts ().Where (chart => date >= chart.BeginDate && date <= chart.EndDate).FirstOrDefault ();
		}

		public void AddChartOfAccounts(IBusinessContext businessContext, CresusChartOfAccounts chart)
		{
			this.EnsureThatChartsOfAccountsAreDeserialized ();

			string chartId  = ItemCodeGenerator.FromGuid (chart.Id);
			var    chartXml = chart.SerializeToXml ("chartOfAccounts");

			var blob = businessContext.CreateEntity<XmlBlobEntity> ();
			
			blob.Code    = chartId;
			blob.XmlData = chartXml;

			this.SerializedChartsOfAccounts.Add (blob);
		}

		public void RemoveChartOfAccounts(IBusinessContext businessContext, CresusChartOfAccounts chart)
		{
			this.EnsureThatChartsOfAccountsAreDeserialized ();

			string chartId = chart.Id.ToString ("D");

			var repository = businessContext.GetRepository<XmlBlobEntity> ();
			var example    = repository.CreateExample ();

			example.Code = chartId;

			foreach (var blob in repository.GetByExample (example))
			{
				businessContext.DeleteEntity (blob);
				this.SerializedChartsOfAccounts.Remove (blob);
			}
		}


		private void EnsureThatChartsOfAccountsAreDeserialized()
		{
			//	Crée les listes si elles n'existent pas.
			if (this.chartsOfAccounts == null)
			{
				this.chartsOfAccounts = new List<CresusChartOfAccounts> ();
			}

			if (this.lastCodes == null)
			{
				this.lastCodes = new List<string> ();
			}

			if (!this.HasSameCodes)  // y a-t-il eu un changement de plan comptable ?
			{
				this.chartsOfAccounts.Clear ();
				this.chartsOfAccounts.AddRange (this.SerializedChartsOfAccounts.Select (blob => CresusChartOfAccounts.DeserializeFromXml (blob.XmlData)));
				this.CopyCodes ();
			}
		}

		private bool HasSameCodes
		{
			get
			{
				if (this.lastCodes.Count != this.SerializedChartsOfAccounts.Count)
				{
					return false;
				}

				return Comparer.EqualObjects (this.lastCodes, this.SerializedChartsOfAccounts.Select (x => x.Code));
			}
		}

		private void CopyCodes()
		{
			this.lastCodes.Clear ();
			this.lastCodes.AddRange (this.SerializedChartsOfAccounts.Select (x => x.Code));
		}


		private List<CresusChartOfAccounts>		chartsOfAccounts;
		private List<string>					lastCodes;
	}
}
