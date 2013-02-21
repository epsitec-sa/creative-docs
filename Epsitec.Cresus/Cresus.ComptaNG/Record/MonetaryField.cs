using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.ComptaNG.Record
{
	public class MonetaryField : AbstractField
	{
		public decimal Content
		{
			get;
			set;
		}

		public string Currency  // utiliser la classe Data.Monnaie ?
		{
			get;
			set;
		}
	}
}
