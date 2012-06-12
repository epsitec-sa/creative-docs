//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;

using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Entities
{
	public partial class ComptaModèleEntity
	{
		public FormattedText ShortSummary
		{
			get
			{
				var l = this.Libellé.ToString ().Replace ("@", "...");
				var m = Converters.MontantToString (this.Montant, null);
				return TextFormatter.FormatText (this.Code, this.GetCompteSummary (this.Débit), "/", this.GetCompteSummary (this.Crédit), this.Pièce, l, m);
			}
		}

		private FormattedText GetCompteSummary(ComptaCompteEntity compte)
		{
			if (compte == null)
			{
				return "—";
			}
			else
			{
				return compte.Numéro;
			}
		}
	}
}
