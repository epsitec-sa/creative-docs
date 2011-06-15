//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.Helpers;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Resolvers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.ReferenceNumberFormatters
{
	public class DocumentInAffairReferenceNumberFormatter : IFormatTokenFormatter
	{
		#region IFormatTokenFormatter Members

		public Helpers.FormatToken GetFormatToken()
		{
			return new ArgumentFormatToken ("#doc", this.CreateReferenceNumber);
		}

		#endregion

		private string CreateReferenceNumber(FormatterHelper helper, string argument)
		{
			var businessContext = Logic.Current.GetComponent<BusinessContext> ();
			var affair = businessContext.GetMasterEntity<AffairEntity> ();

			return string.Format ("{0}-{1:00}", affair.IdA, affair.Documents.Count);
		}
	}
}
