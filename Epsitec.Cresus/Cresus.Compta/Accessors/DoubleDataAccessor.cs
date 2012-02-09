//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux données doubles (bilan avec actif/passif ou PP avec charge/produit) de la comptabilité.
	/// </summary>
	public abstract class DoubleDataAccessor : AbstractDataAccessor
	{
		public DoubleDataAccessor(AbstractController controller)
			: base (controller)
		{
		}


		public override void FilterUpdate()
		{
			this.UpdateReadonlyAllData ();
		}

		public override void UpdateAfterOptionsChanged()
		{
			this.UpdateReadonlyAllData ();
		}

		private void UpdateReadonlyAllData()
		{
			this.readonlyAllData.Clear ();
			this.MinMaxClear ();

			this.filterData.GetBeginnerDates (out this.lastBeginDate, out this.lastEndDate);
			this.soldesJournalManager.Initialize (this.périodeEntity.Journal, this.lastBeginDate, this.lastEndDate);

			//	Partie "gauche" (actif ou charge).
			int lignesGauches = this.readonlyAllData.Count ();
			decimal totalGauche = this.UpdateReadonlyAllData (this.CatégorieGauche, true);
			lignesGauches = this.readonlyAllData.Count () - lignesGauches;

			//	Partie "droite" (passif ou produit).
			int lignesDroites = this.readonlyAllData.Count ();
			decimal totalDroite = this.UpdateReadonlyAllData (this.CatégorieDroite, false);
			lignesDroites = this.readonlyAllData.Count () - lignesDroites;

			//	Ligne de résumé.
			if (totalGauche != totalDroite)
			{
				var data = new DoubleData ();
				data.NeverFiltered = true;
				data.IsItalic      = true;

				if (totalGauche < totalDroite)
				{
					data.Gauche = true;
					data.Titre  = this.DifferenceGaucheDescription;
					data.Solde  = totalDroite - totalGauche;

					this.SetMinMaxValue (data.Solde);

					this.readonlyAllData.Insert (lignesGauches, data);
					lignesGauches++;
				}

				if (totalGauche > totalDroite)
				{
					data.Gauche = false;
					data.Titre  = this.DifferenceDroiteDescription;
					data.Solde  = totalGauche - totalDroite;

					this.SetMinMaxValue (data.Solde);

					this.readonlyAllData.Add (data);
					lignesDroites++;
				}
			}

			base.FilterUpdate ();
			this.UpdateTypo ();
		}

		private decimal UpdateReadonlyAllData(CatégorieDeCompte catégorie, bool gauche)
		{
			//	Génère les lignes de la partie "gauche" ou "droite".
			//	Retourne le montant total.
			int fromProfondeur, toProfondeur;
			this.filterData.GetBeginnerProfondeurs (out fromProfondeur, out toProfondeur);

			bool soldesNuls = this.filterData.BeginnerSoldesNuls;

			decimal total = 0;

			foreach (var compte in this.comptaEntity.PlanComptable.Where (x => x.Catégorie == catégorie))
			{
				var solde = this.soldesJournalManager.GetSolde (compte).GetValueOrDefault ();

				if (!soldesNuls && solde == 0)
				{
					continue;
				}

				var data = new DoubleData ();
				this.readonlyAllData.Add (data);

				data.Gauche = gauche;
				data.Numéro = compte.Numéro;
				data.Titre  = compte.Titre;
				data.Niveau = compte.Niveau;

				if (this.HasSolde (compte, fromProfondeur, toProfondeur))
				{
					data.Solde = solde;
					total += solde;
					this.SetMinMaxValue (solde);
				}

				data.Budget             = this.GetBudget (compte, ComparisonShowed.Budget);
				data.BudgetProrata      = this.GetBudget (compte, ComparisonShowed.BudgetProrata);
				data.BudgetFutur        = this.GetBudget (compte, ComparisonShowed.BudgetFutur);
				data.BudgetFuturProrata = this.GetBudget (compte, ComparisonShowed.BudgetFuturProrata);
				data.PériodePrécédente  = this.GetBudget (compte, ComparisonShowed.PériodePrécédente);
				data.PériodePénultième  = this.GetBudget (compte, ComparisonShowed.PériodePénultième);
			}

			return total;
		}

		private bool HasSolde(ComptaCompteEntity compte, int fromProfondeur, int toProfondeur)
		{
			//	Indique si le solde du compte doit figurer dans le tableau.
			//	Si la profondeur n'est pas spécifiée, on accepte tous les comptes normaux.
			//	Si la profondeur est spécifiée, on accepte les comptes qui ont exactement cette profondeur.
			if (compte.Type == TypeDeCompte.Normal)
			{
				return true;
			}
			else
			{
				return (compte.Niveau+1 == toProfondeur);
			}
		}

		private void UpdateTypo()
		{
			DoubleData lastData = null;

			foreach (var d in this.readonlyData)
			{
				var data = d as DoubleData;

				if (lastData != null)
				{
					lastData.HasBottomSeparator = (lastData.Gauche != data.Gauche);
				}

				data.IsBold = !data.NeverFiltered && (lastData == null || lastData.Gauche != data.Gauche);

				lastData = data;
			}

			if (this.readonlyData.Any ())
			{
				this.readonlyData.Last ().HasBottomSeparator = true;
			}
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var data = this.GetReadOnlyData (row, all) as DoubleData;

			if (data == null)
			{
				return FormattedText.Null;
			}

			switch (column)
			{
				case ColumnType.Numéro:
					return data.Numéro;

				case ColumnType.Titre:
					return data.Titre;

				case ColumnType.Solde:
					return Converters.MontantToString (data.Solde);

				case ColumnType.SoldeGraphique:
					return this.GetMinMaxText (data.Solde);

				case ColumnType.Budget:
					return this.GetBudgetText (data.Solde, data.Budget);

				case ColumnType.BudgetProrata:
					return this.GetBudgetText (data.Solde, data.BudgetProrata);

				case ColumnType.BudgetFutur:
					return this.GetBudgetText (data.Solde, data.BudgetFutur);

				case ColumnType.BudgetFuturProrata:
					return this.GetBudgetText (data.Solde, data.BudgetFuturProrata);

				case ColumnType.PériodePrécédente:
					return this.GetBudgetText (data.Solde, data.PériodePrécédente);

				case ColumnType.PériodePénultième:
					return this.GetBudgetText (data.Solde, data.PériodePénultième);

				case ColumnType.Profondeur:
					return (data.Niveau+1).ToString ();

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

		private DoubleOptions Options
		{
			get
			{
				return this.options as DoubleOptions;
			}
		}


		#region Virtual properties
		protected virtual CatégorieDeCompte CatégorieGauche
		{
			get
			{
				return CatégorieDeCompte.Inconnu;
			}
		}

		protected virtual CatégorieDeCompte CatégorieDroite
		{
			get
			{
				return CatégorieDeCompte.Inconnu;
			}
		}


		protected virtual FormattedText DifferenceGaucheDescription
		{
			get
			{
				return FormattedText.Empty;
			}
		}

		protected virtual FormattedText DifferenceDroiteDescription
		{
			get
			{
				return FormattedText.Empty;
			}
		}
		#endregion
	}
}