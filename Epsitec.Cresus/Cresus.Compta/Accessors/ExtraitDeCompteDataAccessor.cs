//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux données de la balance de vérification de la comptabilité.
	/// </summary>
	public class ExtraitDeCompteDataAccessor : AbstractDataAccessor
	{
		public ExtraitDeCompteDataAccessor(BusinessContext businessContext, ComptabilitéEntity comptabilitéEntity)
			: base (businessContext, comptabilitéEntity)
		{
			this.options = new ExtraitDeCompteOptions (this.comptabilitéEntity);

			this.UpdateAfterOptionsChanged ();
		}


		public override void UpdateAfterOptionsChanged()
		{
			this.readonlyData.Clear ();
			this.MinMaxClear ();

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

				bool débit  = (ExtraitDeCompteDataAccessor.Match (écriture.Débit, filter));
				bool crédit = (ExtraitDeCompteDataAccessor.Match (écriture.Crédit, filter));

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
						solde = -solde;
					}

					data.Solde = solde;

					this.readonlyData.Add (data);
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
						solde = -solde;
					}

					data.Solde = solde;

					this.readonlyData.Add (data);
				}

				this.SetMinMaxValue (solde);
			}

			this.SetBottomSeparatorToPreviousLine ();

			//	Génère la dernière ligne.
			{
				var data = new ExtraitDeCompteData ();

				data.Libellé  = "Mouvement";
				data.Débit    = totalDébit;
				data.Crédit   = totalCrédit;
				data.IsItalic = true;

				this.readonlyData.Add (data);
			}
		}


		public override FormattedText GetText(int row, ColumnType column)
		{
			if (row < 0 || row >= this.Count)
			{
				return FormattedText.Null;
			}

			var data = this.readonlyData[row] as ExtraitDeCompteData;

			switch (column)
			{
				case ColumnType.Date:
					if (data.Date.HasValue)
					{
						return data.Date.Value.ToString ();
					}
					else
					{
						return FormattedText.Empty;
					}

				case ColumnType.CP:
					return ExtraitDeCompteDataAccessor.GetNuméro (data.CP);

				case ColumnType.Pièce:
					return data.Pièce;

				case ColumnType.Libellé:
					return data.Libellé;

				case ColumnType.Débit:
					return AbstractDataAccessor.GetMontant (data.Débit);

				case ColumnType.Crédit:
					return AbstractDataAccessor.GetMontant (data.Crédit);

				case ColumnType.Solde:
					return AbstractDataAccessor.GetMontant (data.Solde);

				case ColumnType.SoldeGraphique:
					return this.GetMinMaxText (data.Solde);

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

		private ExtraitDeCompteOptions Options
		{
			get
			{
				return this.options as ExtraitDeCompteOptions;
			}
		}


		private static bool Match(ComptabilitéCompteEntity compte, FormattedText numéro)
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