//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class ExchangeRateSourceEntity
	{
		public override string[] GetEntityKeywords()
		{
			List<string> keywords = new List<string> ();

			keywords.Add (string.IsNullOrWhiteSpace (this.Originator) ? "—" : this.Originator);
			keywords.AddRange (EnumKeyValues.GetEnumKeyValue (this.Type).Values.Select (x => x.ToString ()));

			return keywords.ToArray ();
		}
		
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Type);
		}
		
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Type, "\n", this.Originator);
		}
	}
}
