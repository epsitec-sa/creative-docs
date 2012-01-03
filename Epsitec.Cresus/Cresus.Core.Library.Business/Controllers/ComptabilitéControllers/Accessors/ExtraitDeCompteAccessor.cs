//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Business.Finance.Comptabilité;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComptabilitéControllers
{
	/// <summary>
	/// Gère l'accès aux données de la balance de vérification de la comptabilité.
	/// </summary>
	public class ExtraitDeCompteAccessor : AbstractDataAccessor<ExtraitDeCompteColumn, ExtraitDeCompteData>
	{
		public ExtraitDeCompteAccessor(ComptabilitéEntity comptabilitéEntity)
			: base (comptabilitéEntity)
		{
			this.options = new ExtraitDeCompteOptions (this.comptabilitéEntity);

			this.UpdateSortedList ();
		}


		public override void UpdateSortedList()
		{
			this.sortedEntities = new List<ExtraitDeCompteData> ();

			FormattedText filter = this.Options.NuméroCompte;
			if (filter.IsNullOrEmpty)
			{
				return;
			}

			var compte = this.comptabilitéEntity.PlanComptable.Where(x => x.Numéro == filter).FirstOrDefault ();

			decimal solde       = 0;
			decimal totalDébit  = 0;
			decimal totalCrédit = 0;

			foreach (var écriture in this.comptabilitéEntity.Journal.OrderBy (x => x.Date))
			{
				if (!this.options.DateInRange (écriture.Date))
				{
					continue;
				}

				bool débit  = (ComptabilitéEntity.Match (écriture.Débit,  filter));
				bool crédit = (ComptabilitéEntity.Match (écriture.Crédit, filter));

				if (débit)
				{
					var data = new ExtraitDeCompteData ();

					data.Date    = écriture.Date;
					data.Pièce   = écriture.Pièce;
					data.Libellé = écriture.Libellé;
					data.CP      = écriture.Crédit;
					data.Débit   = écriture.Montant;

					solde        += écriture.Montant;
					totalDébit   += écriture.Montant;

					if (compte != null &&
						(compte.Catégorie == CatégorieDeCompte.Passif) ||
						(compte.Catégorie == CatégorieDeCompte.Produit))
					{
						data.Solde = -solde;
					}
					else
					{
						data.Solde = solde;
					}

					this.sortedEntities.Add (data);
				}

				if (crédit)
				{
					var data = new ExtraitDeCompteData ();

					data.Date    = écriture.Date;
					data.Pièce   = écriture.Pièce;
					data.Libellé = écriture.Libellé;
					data.CP      = écriture.Débit;
					data.Crédit  = écriture.Montant;

					solde        -= écriture.Montant;
					totalCrédit  += écriture.Montant;

					if (compte != null &&
						(compte.Catégorie == CatégorieDeCompte.Passif) ||
						(compte.Catégorie == CatégorieDeCompte.Produit))
					{
						data.Solde = -solde;
					}
					else
					{
						data.Solde = solde;
					}

					this.sortedEntities.Add (data);
				}
			}

			//	Génère la dernière ligne.
			{
				var data = new ExtraitDeCompteData ();

				data.Libellé  = "Mouvement";
				data.Débit    = totalDébit;
				data.Crédit   = totalCrédit;
				data.IsItalic = true;

				this.sortedEntities.Add (data);
			}
		}


		public override FormattedText GetText(int row, ExtraitDeCompteColumn column)
		{
			if (row < 0 || row >= this.Count)
			{
				return FormattedText.Null;
			}

			var data = this.sortedEntities[row];

			switch (column)
			{
				case ExtraitDeCompteColumn.Date:
					if (data.Date.HasValue)
					{
						return data.Date.Value.ToString ();
					}
					else
					{
						return FormattedText.Empty;
					}

				case ExtraitDeCompteColumn.CP:
					return ExtraitDeCompteAccessor.GetNuméro (data.CP);

				case ExtraitDeCompteColumn.Pièce:
					return data.Pièce;

				case ExtraitDeCompteColumn.Libellé:
					return data.Libellé;

				case ExtraitDeCompteColumn.Débit:
					return ExtraitDeCompteAccessor.GetMontant (data.Débit);

				case ExtraitDeCompteColumn.Crédit:
					return ExtraitDeCompteAccessor.GetMontant (data.Crédit);

				case ExtraitDeCompteColumn.Solde:
					return ExtraitDeCompteAccessor.GetMontant (data.Solde);

				default:
					return FormattedText.Null;
			}
		}


		private static FormattedText GetNuméro(ComptabilitéCompteEntity compte)
		{
			if (compte == null)
			{
				return FormattedText.Empty;
			}
			else
			{
				return compte.Numéro;
			}
		}

		private static FormattedText GetMontant(decimal? montant)
		{
			if (montant.HasValue)
			{
				return montant.Value.ToString ("0.00");
			}
			else
			{
				return FormattedText.Empty;
			}
		}

		private ExtraitDeCompteOptions Options
		{
			get
			{
				return this.options as ExtraitDeCompteOptions;
			}
		}
	}
}