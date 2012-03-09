//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Entities
{
	public partial class ComptaEcritureEntity
	{
		public decimal MontantDébit
		{
			//	Retourne le montant au débit d'une écriture, en tenant compte de la TVA éventuelle.
			get
			{
				if (this.CodeTVA == null)  // écriture sans TVA ?
				{
					return this.MontantTTC;
				}
				else  // écriture avec TVA ?
				{
					return this.TVAAuDébit ? this.MontantHT.GetValueOrDefault () : this.MontantTTC;
				}
			}
		}

		public decimal MontantCrédit
		{
			//	Retourne le montant au crédit d'une écriture, en tenant compte de la TVA éventuelle.
			get
			{
				if (this.CodeTVA == null)  // écriture sans TVA ?
				{
					return this.MontantTTC;
				}
				else  // écriture avec TVA ?
				{
					return !this.TVAAuDébit ? this.MontantHT.GetValueOrDefault () : this.MontantTTC;
				}
			}
		}


		public override FormattedText GetCompactSummary()
		{
			return this.GetSummary ();
		}
		
		public override FormattedText GetSummary()
		{
			return Core.TextFormatter.FormatText (this.Date, this.Débit.Numéro, this.Crédit.Numéro, this.Pièce, this.Libellé, this.MontantTTC.ToString ());
		}
	}
}
