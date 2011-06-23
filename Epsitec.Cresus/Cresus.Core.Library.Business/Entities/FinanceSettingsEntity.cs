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
		public FinanceSettingsEntity()
		{
			this.chartsOfAccounts = new List<CresusChartOfAccounts> ();
		}

		/// <summary>
		/// Gets all charts of accounts.
		/// </summary>
		/// <returns>The collection of all charts of accounts.</returns>
		public IList<CresusChartOfAccounts> GetAllChartsOfAccounts()
		{
			this.DeserializeChartsOfAccountsIfNeeded ();
			return this.chartsOfAccounts.AsReadOnly ();
		}

		/// <summary>
		/// Gets the best chart of accounts (either the one matching the specified date,
		/// or the most recent one which is older than the specified date, or else the
		/// one nearest to the specified date), if any.
		/// </summary>
		/// <param name="date">The date.</param>
		/// <returns>The chart of accounts if one can be found; otherwise, <c>null</c>.</returns>
		public CresusChartOfAccounts GetChartOfAccountsOrDefaultToNearest(Date date)
		{
			return this.GetChartOfAccounts (date)
				?? this.GetAllChartsOfAccounts ().Where (x => x.EndDate < date).OrderByDescending (x => x.EndDate).FirstOrDefault ()
				?? this.GetAllChartsOfAccounts ().OrderBy (x => x.EndDate).FirstOrDefault ();
		}

		/// <summary>
		/// Gets the chart of accounts for the specified date or <c>null</c> if none could
		/// be found.
		/// </summary>
		/// <param name="date">The date.</param>
		/// <returns>The chart of accounts if one can be found; otherwise, <c>null</c>.</returns>
		public CresusChartOfAccounts GetChartOfAccounts(Date date)
		{
			return this.GetAllChartsOfAccounts ().Where (chart => (date >= chart.BeginDate) && (date <= chart.EndDate)).FirstOrDefault ();
		}

		public void AddChartOfAccounts(IBusinessContext businessContext, CresusChartOfAccounts chart)
		{
			this.DeserializeChartsOfAccountsIfNeeded ();

			var chartId  = chart.Id;
			var chartXml = chart.Save ("chartOfAccounts");

			var xmlBlob = businessContext.CreateEntity<XmlBlobEntity> ();
			
			xmlBlob.GuidCode = chartId;
			xmlBlob.XmlData  = chartXml;

			this.SerializedChartsOfAccounts.Add (xmlBlob);
			this.chartsOfAccounts.Add (chart);
		}

		public void RemoveChartOfAccounts(IBusinessContext businessContext, CresusChartOfAccounts chart)
		{
			var repository = businessContext.GetRepository<XmlBlobEntity> ();
			var example    = repository.CreateExample ();

			example.GuidCode = chart.Id;

			foreach (var xmlBlob in repository.GetByExample (example))
			{
				this.SerializedChartsOfAccounts.Remove (xmlBlob);
				
				businessContext.DeleteEntity (xmlBlob);
			}

			this.chartsOfAccounts.Remove (chart);
		}


		private void DeserializeChartsOfAccountsIfNeeded()
		{
			if (this.NeedsDeserialization)
			{
				var xmlBlobs = this.SerializedChartsOfAccounts;
				var deserializedChartsOfAccounts = xmlBlobs.Select (blob => CresusChartOfAccounts.Restore (blob.XmlData));

				this.chartsOfAccounts.Clear ();
				this.chartsOfAccounts.AddRange (deserializedChartsOfAccounts);
			}
		}

		private bool NeedsDeserialization
		{
			get
			{
				IEnumerable<System.Guid> guidsInCache  = this.chartsOfAccounts.Select (x => x.Id);
				IEnumerable<System.Guid> guidsInSource = this.SerializedChartsOfAccounts.Select (x => x.GuidCode);

				return ! Comparer.EqualValues (guidsInCache, guidsInSource);
			}
		}


		private readonly List<CresusChartOfAccounts>	chartsOfAccounts;
	}
}
