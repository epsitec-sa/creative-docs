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
	/// Gère l'accès aux données des soldes de la comptabilité.
	/// </summary>
	public class SoldesDataAccessor : AbstractDataAccessor
	{
		public SoldesDataAccessor(AbstractController controller)
			: base (controller)
		{
			this.viewSettingsList = this.mainWindowController.GetViewSettingsList ("Présentation.Soldes.ViewSettings");
			this.searchData = this.mainWindowController.GetSettingsSearchData ("Présentation.Soldes.Search");
			this.filterData = this.viewSettingsList.Selected.CurrentFilter;
			this.options    = this.viewSettingsList.Selected.CurrentOptions;

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
			this.soldesJournalManager.Initialize (this.période.Journal, this.lastBeginDate, this.lastEndDate);

			this.columnCount = this.Options.SoldesColumns.Count;

			for (int t = 0; t < this.Options.Count; t++)
			{
				var data = new SoldesData
				{
					Description = FormattedText.Concat ("t", Converters.IntToString (t+1)),
				};

				if (this.Options.Cumul)
				{
					for (int c = 0; c < this.columnCount; c++)
					{
						data.SetSolde (c, 0);
					}
				}

				this.readonlyAllData.Add (data);
			}

			foreach (var période in this.compta.Périodes)
			{
				foreach (var écriture in période.Journal)
				{
					for (int c = 0; c < this.columnCount; c++)
					{
						ComptaCompteEntity compte = null;
						decimal sens1 = 1;

						if (écriture.Débit != null && écriture.Débit.Numéro == this.Options.SoldesColumns[c].NuméroCompte)
						{
							compte = écriture.Débit;
							sens1 = 1;
						}

						if (écriture.Crédit != null && écriture.Crédit.Numéro == this.Options.SoldesColumns[c].NuméroCompte)
						{
							compte = écriture.Crédit;
							sens1 = -1;
						}

						if (compte != null)
						{
							decimal sens2 = (compte.Catégorie == CatégorieDeCompte.Passif || compte.Catégorie == CatégorieDeCompte.Produit) ? -1 : 1;
							var montant = écriture.Montant * sens1 * sens2;

							var dateDébut = this.Options.SoldesColumns[c].DateDébut;
							var dateFin = Dates.AddDays (dateDébut, this.Options.Resolution*this.Options.Count);

							int t = Dates.NumberOfDays (écriture.Date, dateDébut) / this.Options.Resolution;

							if (this.Options.Cumul)
							{
								for (int i = 0; i < this.readonlyAllData.Count; i++)
								{
									if (t <= i)
									{
										var data = this.readonlyAllData[i] as SoldesData;
										data.AddSolde (c, montant);
									}
								}
							}
							else
							{
								if (t >= 0 && t < this.readonlyAllData.Count)
								{
									var data = this.readonlyAllData[t] as SoldesData;
									data.AddSolde (c, montant);
								}
							}
						}
					}
				}
			}

#if false
			if (this.Options.Cumul)
			{
				for (int i = 0; i < this.columnCount; i++)
				{
					decimal cumul = 0;

					foreach (var d in this.readonlyAllData)
					{
						var data = d as SoldesData;

						cumul += data.GetSolde (i).GetValueOrDefault ();
						data.SetSolde (i, cumul);
					}
				}
			}
#endif

			base.UpdateFilter ();
		}



		public override void UpdateGraphData(bool force)
		{
			//	Appelé après la mise à jour du filtre, pour mettre à jour les données graphiques.
			if (!force && !this.Options.HasStackedGraph && !this.Options.HasSideBySideGraph && !this.options.ViewGraph)
			{
				return;
			}

			this.cube.Dimensions = 2;
			this.cube.SetDimensionTitle (0, "Comptes");
			this.cube.SetDimensionTitle (1, "Temps");
			this.cube.Clear ();
			this.arrayGraphOptions.Mode = this.Options.HasSideBySideGraph ? GraphMode.SideBySide : GraphMode.Stacked;

			int index = 0;
			foreach (var soldesColumn in this.Options.SoldesColumns)
			{
				this.cube.SetShortTitle (0, index++, soldesColumn.Description);
			}

			int y = 0;
			foreach (var d in this.readonlyData)
			{
				var data = d as SoldesData;

				this.cube.SetShortTitle (1, y, data.Description);

				for (int i = 0; i < this.columnCount; i++)
				{
					decimal? solde = data.GetSolde (i);
					if (solde.HasValue)
					{
						this.cube.SetValue (i, y, solde.Value);
					}
				}

				y++;
			}

			this.UpdateGraphDataToDraw ();
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var data = this.GetReadOnlyData (row, all) as SoldesData;

			if (data == null)
			{
				return FormattedText.Null;
			}

			if (column == ColumnType.Titre)
			{
				return data.Description;
			}

			if (column >= ColumnType.Solde1 && column <= ColumnType.Solde12)
			{
				int rank = column - ColumnType.Solde1;
				var solde = data.GetSolde (rank);
				return Converters.MontantToString (solde, this.compta.Monnaies[0]);
			}

			return FormattedText.Null;
		}


		private new SoldesOptions Options
		{
			get
			{
				return this.options as SoldesOptions;
			}
		}


		private int			columnCount;
	}
}