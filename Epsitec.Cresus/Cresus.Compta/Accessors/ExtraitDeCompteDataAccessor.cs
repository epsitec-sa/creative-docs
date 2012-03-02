//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Permanents.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux données de la balance de vérification de la comptabilité.
	/// </summary>
	public class ExtraitDeCompteDataAccessor : AbstractDataAccessor
	{
		public ExtraitDeCompteDataAccessor(AbstractController controller)
			: base (controller)
		{
			this.permanents = this.mainWindowController.GetSettingsPermanents<ExtraitDeComptePermanents> ("Présentation.ExtraitDeCompte.Permanents", this.compta);
			this.options    = this.mainWindowController.GetSettingsOptions<ExtraitDeCompteOptions> ("Présentation.ExtraitDeCompte.Options", this.compta);
			this.searchData = this.mainWindowController.GetSettingsSearchData ("Présentation.ExtraitDeCompte.Search");
			this.filterData = this.mainWindowController.GetSettingsSearchData ("Présentation.ExtraitDeCompte.Filter");

			this.UpdateAfterOptionsChanged ();
		}


		public override void FilterUpdate()
		{
			Date? beginDate, endDate;
			this.filterData.GetBeginnerDates (out beginDate, out endDate);

			if (this.lastBeginDate != beginDate || this.lastEndDate != endDate)
			{
				this.UpdateAfterOptionsChanged ();
			}

			base.FilterUpdate ();
		}

		public override void UpdateAfterOptionsChanged()
		{
			this.readonlyAllData.Clear ();
			this.MinMaxClear ();

			FormattedText numéroCompte = this.Permanents.NuméroCompte;
			if (numéroCompte.IsNullOrEmpty)
			{
				return;
			}

			this.filterData.GetBeginnerDates (out this.lastBeginDate, out this.lastEndDate);
			this.soldesJournalManager.Initialize (this.période.Journal, this.lastBeginDate, this.lastEndDate);

			var compte = this.compta.PlanComptable.Where (x => x.Numéro == numéroCompte).FirstOrDefault ();

			decimal solde       = 0;
			decimal totalDébit  = 0;
			decimal totalCrédit = 0;

			foreach (var écriture in this.période.Journal)
			{
				if (!Dates.DateInRange (écriture.Date, this.lastBeginDate, this.lastEndDate))
				{
					continue;
				}

				bool débit  = (ExtraitDeCompteDataAccessor.Match (écriture.Débit,  numéroCompte));
				bool crédit = (ExtraitDeCompteDataAccessor.Match (écriture.Crédit, numéroCompte));

				if (débit)
				{
					var data = new ExtraitDeCompteData ();

					data.IsDébit = true;
					data.Entity  = écriture;
					data.Date    = écriture.Date;
					data.Pièce   = écriture.Pièce;
					data.Libellé = écriture.Libellé;
					data.CP      = écriture.Crédit;
					data.Débit   = écriture.Montant;
					data.Journal = écriture.Journal.Nom;

					solde        += écriture.Montant;
					totalDébit   += écriture.Montant;

					if (compte != null &&
						(compte.Catégorie == CatégorieDeCompte.Passif ||
						 compte.Catégorie == CatégorieDeCompte.Produit))
					{
						solde = -solde;
					}

					data.Solde = solde;

					this.readonlyAllData.Add (data);
				}

				if (crédit)
				{
					var data = new ExtraitDeCompteData ();

					data.IsDébit = false;
					data.Entity  = écriture;
					data.Date    = écriture.Date;
					data.Pièce   = écriture.Pièce;
					data.Libellé = écriture.Libellé;
					data.CP      = écriture.Débit;
					data.Crédit  = écriture.Montant;
					data.Journal = écriture.Journal.Nom;

					solde        -= écriture.Montant;
					totalCrédit  += écriture.Montant;

					if (compte != null &&
						(compte.Catégorie == CatégorieDeCompte.Passif ||
						 compte.Catégorie == CatégorieDeCompte.Produit))
					{
						solde = -solde;
					}

					data.Solde = solde;

					this.readonlyAllData.Add (data);
				}

				this.SetMinMaxValue (solde);
			}

			this.SetBottomSeparatorToPreviousLine ();

			//	Génère la dernière ligne.
			{
				var data = new ExtraitDeCompteData ();

				data.Libellé       = "Mouvement";
				data.Débit         = totalDébit;
				data.Crédit        = totalCrédit;
				data.IsItalic      = true;
				data.NeverFiltered = true;

				this.readonlyAllData.Add (data);
			}

			this.FilterUpdate ();
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var data = this.GetReadOnlyData (row, all) as ExtraitDeCompteData;

			if (data == null)
			{
				return FormattedText.Null;
			}

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
					return Converters.MontantToString (data.Débit);

				case ColumnType.Crédit:
					return Converters.MontantToString (data.Crédit);

				case ColumnType.Solde:
					return Converters.MontantToString (data.Solde);

				case ColumnType.SoldeGraphique:
					return this.GetMinMaxText (data.Solde);

				case ColumnType.Journal:
					return data.Journal;

				default:
					return FormattedText.Null;
			}
		}


		private static FormattedText GetNuméro(ComptaCompteEntity compte)
		{
			if (compte == null)
			{
				return JournalDataAccessor.multi;
			}
			else
			{
				return compte.Numéro;
			}
		}

		private new ExtraitDeComptePermanents Permanents
		{
			get
			{
				return this.permanents as ExtraitDeComptePermanents;
			}
		}

		private new ExtraitDeCompteOptions Options
		{
			get
			{
				return this.options as ExtraitDeCompteOptions;
			}
		}


		private static bool Match(ComptaCompteEntity compte, FormattedText numéro)
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