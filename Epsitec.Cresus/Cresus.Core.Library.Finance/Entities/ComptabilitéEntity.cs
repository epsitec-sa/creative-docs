//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business.Finance.Comptabilité;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class ComptabilitéEntity
	{
		public override string[] GetEntityKeywords()
		{
			return new string[] { this.BeginDate.ToString (), this.Name.ToSimpleText () };
		}
		
		public override FormattedText GetCompactSummary()
		{
			return this.GetSummary ();
		}
		
		public override FormattedText GetSummary()
		{
			if (this.BeginDate.HasValue && this.EndDate.HasValue)
			{
				var b = this.BeginDate.Value;
				var e = this.EndDate.Value;

				if (b.Year == e.Year &&
					b.Day ==  1 && b.Month ==  1 &&
					e.Day == 31 && e.Month == 12)  // du 1.1 au 31.12 ?
				{
					return TextFormatter.FormatText (b.Year.ToString (), this.Name);
				}
			}

			return TextFormatter.FormatText (this.BeginDate, "—", this.EndDate, this.Name);
		}


		public Date DefaultDate
		{
			//	Retourne la date par défaut pour la prochaine écriture.
			get
			{
				if (this.Journal.Count == 0)
				{
					if (this.BeginDate.HasValue)
					{
						return this.BeginDate.Value;
					}
					else
					{
						return new Date (System.DateTime.Now);
					}
				}
				else
				{
					return this.Journal.Last ().Date;
				}
			}
		}

		public void UpdateNiveauCompte(ComptabilitéCompteEntity compte)
		{
			//	Met à jour le niveau d'imbrication d'un compte (0..n).
			var initialCOmpte = compte;
			int niveau = 0;

			while (compte != null && compte.Groupe != null && !compte.Groupe.Numéro.IsNullOrEmpty)
			{
				compte = this.PlanComptable.Where (x => x.Numéro == compte.Groupe.Numéro).FirstOrDefault ();

				if (compte == null)
				{
					break;
				}

				niveau++;
			}

			if (initialCOmpte.Niveau != niveau)
			{
				initialCOmpte.Niveau = niveau;
			}
		}

		public decimal? GetSoldeCompte(ComptabilitéCompteEntity compte)
		{
			//	Calcule le solde d'un compte.
			if (compte.Type != TypeDeCompte.Normal &&
				compte.Type != TypeDeCompte.Groupe)
			{
				return null;
			}

			var débit  = this.Journal.Where (x => ComptabilitéEntity.Match (x.Débit,  compte.Numéro)).Sum (x => x.Montant);
			var crédit = this.Journal.Where (x => ComptabilitéEntity.Match (x.Crédit, compte.Numéro)).Sum (x => x.Montant);

			if (compte.Catégorie == CatégorieDeCompte.Passif ||
				compte.Catégorie == CatégorieDeCompte.Produit)
			{
				return crédit - débit;
			}
			else
			{
				return débit - crédit;
			}
		}

		public static bool Match(ComptabilitéCompteEntity compte, FormattedText numéro)
		{
			//	Retroune true si le compte ou ses fils correspond au numéro.
			while (compte != null && !compte.Numéro.IsNullOrEmpty)
			{
				if (compte.Numéro == numéro)
				{
					return true;
				}

				compte = compte.Groupe;
			}

			return false;
		}
	}
}
