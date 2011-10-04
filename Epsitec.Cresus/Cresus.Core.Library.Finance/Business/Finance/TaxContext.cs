//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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


		public VatDefinitionEntity[] GetVatDefinitions(VatRateType vatRateType)
		{
			var results = from def in this.vatDefs
						  where def.VatRateType == vatRateType
						  select def;

			return results.ToArray ();
		}

		public VatDefinitionEntity GetVatDefinition(Date date, VatRateType vatRateType)
		{
			var results = from def in this.vatDefs
						  where date.InRange (def.BeginDate, def.EndDate)
						  where def.VatRateType == vatRateType
						  select def;

			return results.FirstOrDefault ();
		}

		public VatDefinitionEntity[] GetVatDefinitions(IDateRange dateRange, VatRateType vatRateType)
		{
			var results = from def in this.vatDefs
						  where dateRange.Overlaps (def)
						  where def.VatRateType == vatRateType
						  select def;

			return results.ToArray ();
		}


		public static VatRateType GetVatRateType(VatCode vatCode)
		{
			switch (vatCode)
			{
				case VatCode.None:
				case VatCode.Excluded:
				case VatCode.ZeroRated:
					return VatRateType.None;

				case VatCode.StandardTaxOnTurnover:
				case VatCode.StandardInputTaxOnMaterialOrServiceExpenses:
				case VatCode.StandardInputTaxOnInvestementOrOperatingExpenses:
					return VatRateType.StandardTax;

				case VatCode.SpecialTaxOnTurnover:
				case VatCode.SpecialInputTaxOnMaterialOrServiceExpenses:
				case VatCode.SpecialInputTaxOnInvestementOrOperatingExpenses:
					return VatRateType.SpecialTax;

				case VatCode.ReducedTaxOnTurnover:
				case VatCode.ReducedInputTaxOnMaterialOrServiceExpenses:
				case VatCode.ReducedInputTaxOnInvestementOrOperatingExpenses:
					return VatRateType.ReducedTax;
			}

			throw new System.NotSupportedException (string.Format ("Unsupported value: {0}", vatCode.GetQualifiedName ()));
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
