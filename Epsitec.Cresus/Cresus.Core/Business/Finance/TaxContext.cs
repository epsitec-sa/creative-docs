//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	/// <summary>
	/// The <c>TaxContext</c> class provides access to the VAT definitions for
	/// specified date ranges and VAT codes.
	/// </summary>
	public class TaxContext
	{
		private TaxContext(CoreData data)
		{
			this.vatDefs = data.GetAllEntities<VatDefinitionEntity> ().ToArray ();
		}

		
		public static TaxContext				Current
		{
			get
			{
				return TaxContext.current;
			}
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
		
		public static void Initialize(CoreData data)
		{
			TaxContext.current = new TaxContext (data);
		}

		
		
		private readonly VatDefinitionEntity[]	vatDefs;
		private static TaxContext				current;
	}
}
