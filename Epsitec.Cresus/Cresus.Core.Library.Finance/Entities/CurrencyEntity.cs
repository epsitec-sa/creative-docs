//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class CurrencyEntity
	{
		public override string[] GetEntityKeywords()
		{
			return EnumKeyValues.GetEnumKeyValue (this.CurrencyCode).Values.Select (x => x.ToString ()).ToArray ();
		}
		
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.CurrencyCode);
		}
		
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (
				this.CurrencyCode, "\n",
				"Du", this.BeginDate, "—", TextFormatter.Command.IfEmpty, "au", this.EndDate, "—", TextFormatter.Command.IfEmpty, "\n",
				this.ExchangeRate, TextFormatter.FormatCommand ("#string {0:0.00000}"), "CHF →", this.ExchangeRateBase, this.CurrencyCode);
		}
	}
}
