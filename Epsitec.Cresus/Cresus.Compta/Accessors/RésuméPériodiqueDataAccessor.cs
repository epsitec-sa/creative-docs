//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Graph;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux données du résumé périodique de la comptabilité.
	/// </summary>
	public class RésuméPériodiqueDataAccessor : AbstractDataAccessor
	{
		public RésuméPériodiqueDataAccessor(AbstractController controller)
			: base (controller)
		{
			this.viewSettingsList = this.mainWindowController.GetViewSettingsList (controller.ViewSettingsKey);
			this.searchData       = this.mainWindowController.GetSettingsSearchData (controller.SearchKey);
			this.filterData       = this.viewSettingsList.Selected.CurrentFilter;
			this.options          = this.viewSettingsList.Selected.CurrentOptions;

			this.arrayGraphOptions = new GraphOptions ();

			this.UpdateAfterOptionsChanged ();
		}

		private void FilterInitialize(SearchData data)
		{
			data.FirstTabData.Column              = ColumnType.Solde;
			data.FirstTabData.SearchText.Mode     = SearchMode.WholeContent;
			data.FirstTabData.SearchText.Invert   = true;
			data.FirstTabData.SearchText.FromText = Converters.MontantToString (0, this.compta.Monnaies[0]);
		}


		public int ColumnCount
		{
			get
			{
				return this.columnCount;
			}
		}


		public override void UpdateFilter()
		{
			this.UpdateReadonlyAllData ();
		}

		public override void UpdateAfterOptionsChanged()
		{
			this.UpdateReadonlyAllData ();

			base.UpdateAfterOptionsChanged ();
		}

		private void UpdateReadonlyAllData()
		{
			this.readonlyAllData.Clear ();

			this.filterData.GetBeginnerDates (out this.lastBeginDate, out this.lastEndDate);
			this.mainWindowController.TemporalData.MergeDates (ref this.lastBeginDate, ref this.lastEndDate);
			this.soldesJournalManager.Initialize (this.période.Journal, this.lastBeginDate, this.lastEndDate);

			//	Crée un SoldesJournalManager par colonne.
			var soldesManagers = new List<SoldesJournalManager> ();
			RésuméPériodiqueDataAccessor.ColumnsProcess (this.période, this.Options, (index, dateDébut, dateFin) =>
			{
				var soldesManager = new SoldesJournalManager (this.compta);
				soldesManager.Initialize (this.période.Journal, dateDébut, dateFin);
				soldesManagers.Add (soldesManager);
			});

			this.columnCount = soldesManagers.Count;

			//	Génère les différentes lignes, une par compte.
			foreach (var compte in this.compta.PlanComptable)
			{
				if (compte.Catégorie == CatégorieDeCompte.Inconnu)
				{
					continue;
				}

				var data = new RésuméPériodiqueData
				{
					Entity    = compte,
					Numéro    = compte.Numéro,
					Titre     = compte.Titre,
					Catégorie = compte.Catégorie,
					Type      = compte.Type,
					Niveau    = compte.Niveau,
				};

				decimal total = 0;
				for (int i = 0; i < soldesManagers.Count; i++)
				{
					var solde = soldesManagers[i].GetSolde (compte).GetValueOrDefault ();
					data.SetSolde (i, solde);
					total += System.Math.Max (solde, 0);
				}

				data.Solde = total;

				this.readonlyAllData.Add (data);
			}

			base.UpdateFilter ();
		}

		public static void ColumnsProcess(ComptaPériodeEntity période, RésuméPériodiqueOptions options, System.Action<int, Date, Date> action)
		{
			var dateDébut    = période.DateDébut;
			var dateFin      = dateDébut;
			var débutPériode = période.DateDébut;
			var finPériode   = Dates.AddDays (période.DateFin, 1);  // normalement, 1er janvier de l'année suivante

			int index = 0;
			do
			{
				dateFin = Dates.AddMonths (dateDébut, options.NumberOfMonths);

				if (dateFin > finPériode)
				{
					dateFin = finPériode;
				}

				action (index++, options.Cumul ? débutPériode : dateDébut, Dates.AddDays (dateFin, -1));

				dateDébut = dateFin;
			}
			while (dateFin < finPériode);
		}


		public override void UpdateGraphData(bool force)
		{
			//	Appelé après la mise à jour du filtre, pour mettre à jour les données graphiques.
			if (!force && !this.Options.HasStackedGraph && !this.Options.HasSideBySideGraph && !this.options.ViewGraph)
			{
				return;
			}

			this.cube.Dimensions = 2;
			this.cube.SetDimensionTitle (0, "Périodes");
			this.cube.SetDimensionTitle (1, "Comptes");
			this.cube.Clear ();
			this.arrayGraphOptions.Mode = this.Options.HasSideBySideGraph ? GraphMode.SideBySide : GraphMode.Stacked;

			RésuméPériodiqueDataAccessor.ColumnsProcess (this.période, this.Options, (index, dateDébut, dateFin) =>
			{
				this.cube.SetShortTitle (0, index, Dates.GetMonthShortDescription (dateDébut, dateFin));
			});

			int y = 0;
			foreach (var d in this.readonlyData)
			{
				var data = d as RésuméPériodiqueData;

				this.cube.SetShortTitle (1, y, data.Numéro);
				this.cube.SetFullTitle (1, y, FormattedText.Concat (data.Numéro, " ", data.Titre));

				decimal last = 0;
				for (int i = 0; i < this.columnCount; i++)
				{
					decimal solde = data.GetSolde (i).GetValueOrDefault ();
					this.cube.SetValue (i, y, solde-last);

					if (this.Options.Cumul)
					{
						last = solde;
					}
				}

				y++;
			}

			this.UpdateGraphDataToDraw ();
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var data = this.GetReadOnlyData (row, all) as RésuméPériodiqueData;

			if (data == null)
			{
				return FormattedText.Null;
			}

			if (column >= ColumnType.Solde1 && column <= ColumnType.Solde12)
			{
				int rank = column - ColumnType.Solde1;
				var solde = data.GetSolde (rank);
				return Converters.MontantToString (solde, this.compta.Monnaies[0]);
			}

			switch (column)
			{
				case ColumnType.Numéro:
					return data.Numéro;

				case ColumnType.Titre:
					return data.Titre;

				case ColumnType.Catégorie:
					return Converters.CatégorieToString (data.Catégorie);

				case ColumnType.Type:
					return Converters.TypeToString (data.Type);

				case ColumnType.Profondeur:
					return (data.Niveau+1).ToString ();

				case ColumnType.SoldeGraphique:
					return AbstractDataAccessor.GetGraphicText (row);

				case ColumnType.Solde:
					return Converters.MontantToString (data.Solde, this.compta.Monnaies[0]);

				default:
					return FormattedText.Null;
			}
		}


		private new RésuméPériodiqueOptions Options
		{
			get
			{
				return this.options as RésuméPériodiqueOptions;
			}
		}


		private int			columnCount;
	}
}