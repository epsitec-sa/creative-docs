//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.Reports;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.Engine
{
	/// <summary>
	/// Cette classe sait fabriquer un nouveau mandat entièrement vide.
	/// </summary>
	public class EmptyMandatFactory : AbstractMandatFactory
	{
		public EmptyMandatFactory(DataAccessor accessor)
			: base (accessor)
		{
		}


		public override DataMandat Create(string name, System.DateTime startDate, bool withSamples)
		{
			this.withSamples = withSamples;

			this.accessor.Mandat = new DataMandat (this.accessor.ComputerSettings, name, startDate);

			this.AddAssetsSettings ();
			this.AddPersonsSettings ();

			this.CreateGroupsSamples ();

			this.AddReports ();

			return this.accessor.Mandat;
		}


		protected override void AddAssetsSettings()
		{
			this.fieldAssetName = this.AddSettings (BaseType.AssetsUserFields, "Nom", FieldType.String, true, 180, 380, 1, 1, 0);
		}

		protected override void AddPersonsSettings()
		{
			this.fieldPersonLastName = this.AddSettings (BaseType.PersonsUserFields, "Nom", FieldType.String, true, 120, 380, 1, 1, 0);
		}

		protected override void CreateGroupsSamples()
		{
			var root = this.AddGroup (null, "Groupes", null);
		}

		protected override void AddReports()
		{
			var dateRange = new DateRange (this.accessor.Mandat.StartDate, this.accessor.Mandat.StartDate.AddYears (1));
			var initialTimestamp = new Timestamp (this.accessor.Mandat.StartDate, 0);
			var finalTimestamp   = new Timestamp (new System.DateTime (2099, 12, 31), 0);

			this.accessor.Mandat.Reports.Add (new MCH2SummaryParams (null, dateRange, Guid.Empty, 1, Guid.Empty, false));

			this.accessor.Mandat.Reports.Add (new AssetsParams ("Etat initial au &lt;DATE&gt;", initialTimestamp, Guid.Empty, null));
			this.accessor.Mandat.Reports.Add (new AssetsParams ("Etat final", finalTimestamp, Guid.Empty, null));

			this.accessor.Mandat.Reports.Add (new PersonsParams ());
		}
	}
}
