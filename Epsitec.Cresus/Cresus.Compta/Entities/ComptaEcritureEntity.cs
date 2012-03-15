//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Entities
{
	public partial class ComptaEcritureEntity
	{
		public FormattedText LibelléTVA
		{
			get
			{
				if (this.CodeTVA == null)
				{
					return FormattedText.Empty;
				}
				else
				{
					return ComptaEcritureEntity.GetLibelléTVA (this.CodeTVA.Code, this.TauxTVA);
				}
			}
		}

		public static FormattedText GetLibelléTVA(FormattedText code, decimal? taux)
		{
			//	Retourne par exemple "TVA 8.0% (IPM)".
			if (!code.IsNullOrEmpty && taux.HasValue)
			{
				return string.Format ("TVA {0} ({1})", Converters.PercentToString (taux), code);
			}
			else
			{
				return FormattedText.Empty;
			}
		}


		public override FormattedText GetCompactSummary()
		{
			return this.GetSummary ();
		}
		
		public override FormattedText GetSummary()
		{
			return Core.TextFormatter.FormatText (this.Date, this.Débit.Numéro, this.Crédit.Numéro, this.Pièce, this.Libellé, this.Montant.ToString ());
		}
	}
}
