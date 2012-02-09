//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Entities
{
	public partial class ComptaCompteEntity
	{
		public ComptaBudgetEntity GetBudget(ComptaPériodeEntity période)
		{
			return this.Budgets.Where (x => x.Période == période).FirstOrDefault ();
		}


		public override IEnumerable<FormattedText> GetFormattedEntityKeywords()
		{
			yield return TextFormatter.FormatText (this.Numéro);
			yield return TextFormatter.FormatText (this.Titre);
		}
		
		public override FormattedText GetCompactSummary()
		{
			return this.GetSummary ();
		}
		
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Numéro, this.Titre);
		}
	}
}
