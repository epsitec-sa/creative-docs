//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Extensions;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Factories;

namespace Epsitec.Cresus.Core.Business.Finance
{
	/// <summary>
	/// The <c>TaxContext</c> class provides access to the VAT definitions for
	/// specified date ranges and VAT codes.
	/// </summary>
	public class TaxContext : CoreDataComponent
	{
		internal TaxContext(CoreData data)
			: base (data)
		{
		}

		public override bool CanExecuteSetupPhase()
		{
			return this.Host.IsReady && this.Host.ConnectionManager.IsReady;
		}

		public override void ExecuteSetupPhase()
		{
			base.ExecuteSetupPhase ();

			this.vatDefs = this.Host.GetAllEntities<VatDefinitionEntity> ().ToArray ();			
		}


		public VatDefinitionEntity[] GetVatDefinitions(VatCode vatCode)
		{
			var results = from def in this.vatDefs
						  where def.VatCode == vatCode
						  select def;

			return results.ToArray ();
		}

		public VatDefinitionEntity GetVatDefinition(Date date, VatCode vatCode)
		{
			var results = from def in this.vatDefs
						  where date.InRange (def.BeginDate, def.EndDate)
						  where def.VatCode == vatCode
						  select def;

			return results.FirstOrDefault ();
		}

		public VatDefinitionEntity[] GetVatDefinitions(IDateRange dateRange, VatCode vatCode)
		{
			var results = from def in this.vatDefs
						  where dateRange.Overlaps (def)
						  where def.VatCode == vatCode
						  select def;

			return results.ToArray ();
		}

		#region Factory Class

		private sealed class Factory : ICoreDataComponentFactory
		{
			#region ICoreDataComponentFactory Members

			public bool CanCreate(CoreData data)
			{
				return true;
			}

			public CoreDataComponent Create(CoreData data)
			{
				return new TaxContext (data);
			}

			public System.Type GetComponentType()
			{
				return typeof (TaxContext);
			}

			#endregion
		}

		#endregion


		private VatDefinitionEntity[]	vatDefs;
	}
}
