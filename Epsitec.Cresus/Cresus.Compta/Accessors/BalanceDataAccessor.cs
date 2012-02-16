//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux données de la balance de vérification de la comptabilité.
	/// </summary>
	public class BalanceDataAccessor : AbstractDataAccessor
	{
		public BalanceDataAccessor(AbstractController controller)
			: base (controller)
		{
			this.options    = this.mainWindowController.GetSettingsOptions<BalanceOptions> ("Présentation.Balance.Options", this.comptaEntity);
			this.searchData = this.mainWindowController.GetSettingsSearchData ("Présentation.Balance.Search");
			this.filterData = this.mainWindowController.GetSettingsSearchData ("Présentation.Balance.Filter");
			//?this.filterData = this.mainWindowController.GetSettingsSearchData<SearchData> ("Présentation.Balance.Filter", this.FilterInitialize);

			this.UpdateAfterOptionsChanged ();
		}

		private void FilterInitialize(SearchData data)
		{
			data.TabsData[0].Column              = ColumnType.Solde;
			data.TabsData[0].SearchText.Mode     = SearchMode.WholeContent;
			data.TabsData[0].SearchText.Invert   = true;
			data.TabsData[0].SearchText.FromText = Converters.MontantToString (0);
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
			this.UpdateTypo ();
		}

		public override void UpdateAfterOptionsChanged()
		{
			this.readonlyAllData.Clear ();

			decimal totalDébit  = 0;
			decimal totalCrédit = 0;
			decimal totalSoldeD = 0;
			decimal totalSoldeC = 0;

			this.filterData.GetBeginnerDates (out this.lastBeginDate, out this.lastEndDate);
			this.soldesJournalManager.Initialize (this.périodeEntity.Journal, this.lastBeginDate, this.lastEndDate);

			foreach (var compte in this.comptaEntity.PlanComptable)
			{
				if (compte.Catégorie == CatégorieDeCompte.Inconnu)
				{
					continue;
				}

				var soldeDébit  = this.soldesJournalManager.GetSoldeDébit  (compte).GetValueOrDefault ();
				var soldeCrédit = this.soldesJournalManager.GetSoldeCrédit (compte).GetValueOrDefault ();
				var différence = soldeCrédit - soldeDébit;

				var data = new BalanceData ();

				data.Numéro    = compte.Numéro;
				data.Titre     = compte.Titre;
				data.Catégorie = compte.Catégorie;
				data.Type      = compte.Type;
				data.Niveau    = compte.Niveau;
				data.Débit     = soldeDébit;
				data.Crédit    = soldeCrédit;

				if (différence < 0)
				{
					data.SoldeDébit = -différence;
				}
				if (différence > 0)
				{
					data.SoldeCrédit = différence;
				}

				if (compte.Type == TypeDeCompte.Normal)
				{
					totalDébit  += soldeDébit;
					totalCrédit += soldeCrédit;

					if (différence < 0)
					{
						totalSoldeD -= différence;
					}
					if (différence > 0)
					{
						totalSoldeC += différence;
					}
				}

				this.readonlyAllData.Add (data);
			}

			this.SetBottomSeparatorToPreviousLine ();

			// Génère la dernière ligne.
			{
				var data = new BalanceData ();

				data.Titre         = "Mouvement";
				data.Débit         = totalDébit;
				data.Crédit        = totalCrédit;
				data.SoldeDébit    = totalSoldeD;
				data.SoldeCrédit   = totalSoldeC;
				data.IsItalic      = true;
				data.NeverFiltered = true;

				this.readonlyAllData.Add (data);
			}

			this.FilterUpdate ();
		}

		private void UpdateTypo()
		{
			BalanceData lastData = null;

			foreach (var d in this.readonlyData)
			{
				var data = d as BalanceData;

				if (lastData != null)
				{
					lastData.HasBottomSeparator = (lastData.Catégorie != data.Catégorie);
				}

				data.IsBold = !data.NeverFiltered && (lastData == null || lastData.Catégorie != data.Catégorie);

				lastData = data;
			}

			if (this.readonlyData.Any ())
			{
				this.readonlyData.Last ().HasBottomSeparator = true;
			}
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var data = this.GetReadOnlyData (row, all) as BalanceData;

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

				case ColumnType.Catégorie:
					return PlanComptableDataAccessor.CatégorieToText (data.Catégorie);

				case ColumnType.Type:
					return PlanComptableDataAccessor.TypeToText (data.Type);

				case ColumnType.Débit:
					return Converters.MontantToString (data.Débit);

				case ColumnType.Crédit:
					return Converters.MontantToString (data.Crédit);

				case ColumnType.SoldeDébit:
					return Converters.MontantToString (data.SoldeDébit);

				case ColumnType.SoldeCrédit:
					return Converters.MontantToString (data.SoldeCrédit);

				case ColumnType.Budget:
					return Converters.MontantToString (data.Budget);

				case ColumnType.Solde:
					if (data.Catégorie == CatégorieDeCompte.Passif ||
						data.Catégorie == CatégorieDeCompte.Produit)
					{
						return Converters.MontantToString (data.Crédit.GetValueOrDefault () - data.Débit.GetValueOrDefault ());
					}
					else
					{
						return Converters.MontantToString (data.Débit.GetValueOrDefault () - data.Crédit.GetValueOrDefault ());
					}

				case ColumnType.Profondeur:
					return (data.Niveau+1).ToString ();

				default:
					return FormattedText.Null;
			}
		}


		private new BalanceOptions Options
		{
			get
			{
				return this.options as BalanceOptions;
			}
		}
	}
}