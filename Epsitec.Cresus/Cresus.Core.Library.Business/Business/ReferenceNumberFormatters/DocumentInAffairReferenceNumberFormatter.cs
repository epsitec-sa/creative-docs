//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Formatters;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Resolvers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.ReferenceNumberFormatters
{
	public class DocumentInAffairReferenceNumberFormatter : IFormatTokenFormatter
	{
		#region IFormatTokenFormatter Members

		public FormatToken GetFormatToken()
		{
			return new ArgumentFormatToken ("#doc", this.CreateReferenceNumber);
		}

		#endregion

		private string CreateReferenceNumber(FormatterHelper helper, string argument)
		{
			var businessContext = helper.GetComponent<IBusinessContext> ();
			var affair = businessContext.GetMasterEntity<AffairEntity> ();

			return string.Format ("{0}-{1:00}", affair.IdA, affair.Documents.Count);
		}
	}
	public class IncludeCustomerReferenceNumberFormatter : IFormatTokenFormatter
	{
		#region IFormatTokenFormatter Members

		public FormatToken GetFormatToken()
		{
			return new ArgumentFormatToken ("#customer", this.CreateReferenceNumber);
		}

		#endregion

		private string CreateReferenceNumber(FormatterHelper helper, string argument)
		{
			var businessContext = helper.GetComponent<IBusinessContext> ();
			var affair = businessContext.GetMasterEntity<AffairEntity> ();
			var customer = affair.Customer;

			switch (argument ?? "A")
			{
				case "A":
					return customer.IdA;
				case "B":
					return customer.IdB;
				case "C":
					return customer.IdC;
			}

			return "?";
		}
	}
}
