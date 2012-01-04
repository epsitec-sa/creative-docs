//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		public override IEnumerable<FormattedText> GetFormattedEntityKeywords()
		{
			yield return TextFormatter.FormatText (this.BeginDate);
			yield return TextFormatter.FormatText (this.Name);
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

		public bool ParseDate(FormattedText text, out Date? date)
		{
			//	Transforme un texte en une date valide pour la comptabilité.
			if (text.IsNullOrEmpty)
			{
				date = null;
				return true;
			}

			var brut = text.ToSimpleText ();
			brut = brut.Replace (".", " ");
			brut = brut.Replace (",", " ");
			brut = brut.Replace ("/", " ");
			brut = brut.Replace ("-", " ");
			brut = brut.Replace (":", " ");
			brut = brut.Replace (";", " ");
			brut = brut.Replace ("  ", " ");
			brut = brut.Replace ("  ", " ");

			var words = brut.Split (' ');
			var defaultDate = this.DefaultDate;
			int day, month, year;

			int.TryParse (words[0], out day);

			if (words.Length <= 1 || !int.TryParse (words[1], out month))
			{
				month = defaultDate.Month;
			}

			if (words.Length <= 2 || !int.TryParse (words[2], out year))
			{
				year = defaultDate.Year;
			}

			try
			{
				date = new Date (year, month, day);
			}
			catch
			{
				date = defaultDate;
				return false;
			}

			if (this.BeginDate.HasValue && date < this.BeginDate.Value)
			{
				date = this.BeginDate.Value;
				return false;
			}

			if (this.EndDate.HasValue && date > this.EndDate.Value)
			{
				date = this.EndDate.Value;
				return false;
			}

			return true;
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

		public decimal? GetSoldeCompte(ComptabilitéCompteEntity compte, Date? dateDébut = null, Date? dateFin = null)
		{
			//	Calcule le solde d'un compte.
			if (compte.Type != TypeDeCompte.Normal &&
				compte.Type != TypeDeCompte.Groupe)
			{
				return null;
			}

			var débit  = this.GetSoldeCompteDébit  (compte, dateDébut, dateFin);
			var crédit = this.GetSoldeCompteCrédit (compte, dateDébut, dateFin);

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

		public decimal? GetSoldeCompteDébit(ComptabilitéCompteEntity compte, Date? dateDébut = null, Date? dateFin = null)
		{
			//	Calcule le solde au débit d'un compte.
			if (compte.Type != TypeDeCompte.Normal &&
				compte.Type != TypeDeCompte.Groupe)
			{
				return null;
			}
			else
			{
				return this.Journal.Where (x => ComptabilitéEntity.Match (dateDébut, dateFin, x.Date) && ComptabilitéEntity.Match (x.Débit, compte.Numéro)).Sum (x => x.Montant);
			}
		}

		public decimal? GetSoldeCompteCrédit(ComptabilitéCompteEntity compte, Date? dateDébut = null, Date? dateFin = null)
		{
			//	Calcule le solde au crédit d'un compte.
			if (compte.Type != TypeDeCompte.Normal &&
				compte.Type != TypeDeCompte.Groupe)
			{
				return null;
			}
			else
			{
				return this.Journal.Where (x => ComptabilitéEntity.Match (dateDébut, dateFin, x.Date) && ComptabilitéEntity.Match (x.Crédit, compte.Numéro)).Sum (x => x.Montant);
			}
		}

		private static bool Match(Date? dateDébut, Date? dateFin, Date? date)
		{
			if (date.HasValue)
			{
				if (dateDébut.HasValue && date.Value < dateDébut.Value)
				{
					return false;
				}

				if (dateFin.HasValue && date.Value > dateFin.Value)
				{
					return false;
				}
			}

			return true;
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
