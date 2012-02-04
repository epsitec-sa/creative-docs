//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès à des données génériques de la comptabilité.
	/// </summary>
	public abstract class AbstractDataAccessor
	{
		public AbstractDataAccessor(AbstractController controller)
		{
			this.controller = controller;

			this.businessContext      = this.controller.BusinessContext;
			this.comptaEntity         = this.controller.ComptaEntity;
			this.périodeEntity        = this.controller.PériodeEntity;
			this.columnMappers        = this.controller.ColumnMappers;
			this.mainWindowController = this.controller.MainWindowController;

			this.soldesJournalManager = new SoldesJournalManager (this.comptaEntity);

			this.readonlyAllData = new List<AbstractData> ();
			this.readonlyData    = new List<AbstractData> ();
			this.editionData     = new List<AbstractEditionData> ();
			this.searchResults   = new List<SearchResult> ();
		}


		public List<ColumnMapper> ColumnMappers
		{
			get
			{
				return this.columnMappers;
			}
		}

		public AbstractOptions AccessorOptions
		{
			get
			{
				return this.options;
			}
		}

		public SearchData SearchData
		{
			get
			{
				return this.searchData;
			}
		}

		public SearchData FilterData
		{
			get
			{
				return this.filterData;
			}
		}

		public SoldesJournalManager SoldesJournalManager
		{
			get
			{
				return this.soldesJournalManager;
			}
		}

		public virtual void UpdateAfterOptionsChanged()
		{
		}


		public void SearchUpdate()
		{
			//	Met à jour les recherches, en fonction d'une nouvelle chaîne cherchée.
			this.searchResults.Clear ();

			if (this.searchData != null && !this.searchData.IsEmpty)
			{
				int count = this.Count;
				for (int row = 0; row < count; row++)
				{
					var list = new List<SearchResult> ();

					foreach (var column in this.columnMappers.Where (x => x.Show).Select (x => x.Column))
					{
						foreach (var tab in this.searchData.TabsData)
						{
							if (tab.Column != ColumnType.None && tab.Column != column)
							{
								continue;
							}

							if (tab.IsEmpty)
							{
								continue;
							}

							var text = this.GetText (row, column);
							int n = tab.SearchText.Search (ref text);

							if (n != 0)  // trouvé ?
							{
								list.Add (new SearchResult (row, column, text));

								if (this.searchData.OrMode)
								{
									break;
								}
							}
						}
					}

					if (this.searchData.OrMode || list.Count == this.searchData.TabsData.Count)
					{
						list.ForEach (x => this.searchResults.Add (x));
					}
				}
			}

			this.searchLocator = 0;
		}

		public bool SearchAny
		{
			get
			{
				return this.searchResults.Any ();
			}
		}

		public int? SearchCount
		{
			get
			{
				if (this.searchData == null || this.searchData.IsEmpty)
				{
					return null;
				}
				else
				{
					return this.searchResults.Count;
				}
			}
		}

		public int? SearchLocator
		{
			get
			{
				if (this.searchData == null || this.searchData.IsEmpty)
				{
					return null;
				}
				else
				{
					return this.searchLocator;
				}
			}
		}

		public void SearchLocatorInfo(out int row, out ColumnType columnType)
		{
			if (this.searchResults.Count == 0)
			{
				row        = -1;
				columnType = ColumnType.None;
			}
			else
			{
				row        = this.searchResults[this.searchLocator].Row;
				columnType = this.searchResults[this.searchLocator].Column;
			}
		}

		public void SearchMoveLocator(int direction)
		{
			if (direction > 0)
			{
				this.searchLocator++;

				if (this.searchLocator >= this.searchResults.Count)
				{
					this.searchLocator = 0;
				}
			}
			else
			{
				this.searchLocator--;

				if (this.searchLocator < 0)
				{
					this.searchLocator = this.searchResults.Count-1;
				}
			}
		}

		public SearchResult GetSearchResult(int row, ColumnType columnType)
		{
			//	Retourne les données de recherche, pour une cellule.
			return this.searchResults.Where (x => x.Row == row && x.Column == columnType).FirstOrDefault ();
		}


		public virtual void FilterUpdate()
		{
			//	Met à jour le filtre.
			this.readonlyData.Clear ();

			if (this.filterData == null || this.filterData.IsEmpty)
			{
				this.readonlyData.AddRange (this.readonlyAllData);
			}
			else
			{
				int count = this.readonlyAllData.Count;
				for (int row = 0; row < count; row++)
				{
					int founds = 0;

					if (this.readonlyAllData[row].NeverFiltered)
					{
						founds = this.filterData.TabsData.Count;
					}
					else
					{
						founds = this.FilterLine (row);
					}

					if (founds != 0 && (this.filterData.OrMode || founds == this.filterData.TabsData.Count))
					{
						this.readonlyData.Add (this.readonlyAllData[row]);
					}
				}
			}
		}

		protected int FilterLine(int row)
		{
			int founds = 0;

			foreach (var column in this.columnMappers.Select (x => x.Column))
			{
				foreach (var tab in this.filterData.TabsData)
				{
					if (tab.Column != ColumnType.None && tab.Column != column)
					{
						continue;
					}

					if (tab.IsEmpty)
					{
						continue;
					}

					var text = this.GetText (row, column, true);
					int n = tab.SearchText.Search (ref text);

					if (n != 0)  // trouvé ?
					{
						founds++;

						if (this.filterData.OrMode)
						{
							break;
						}
					}
				}
			}

			return founds;
		}


		public virtual int AllCount
		{
			get
			{
				if (this.readonlyAllData == null)
				{
					return this.Count;
				}
				else
				{
					return this.readonlyAllData.Count;
				}
			}
		}

		public virtual int Count
		{
			get
			{
				if (this.readonlyData == null)
				{
					return 0;
				}
				else
				{
					return this.readonlyData.Count;
				}
			}
		}

		public AbstractData GetReadOnlyData(int row, bool all = false)
		{
			List<AbstractData> data;

			if (all)
			{
				data = this.readonlyAllData;
			}
			else
			{
				data = this.readonlyData;
			}

			if (data == null || row < 0 || row >= data.Count)
			{
				return null;
			}
			else
			{
				return data[row];
			}
		}

		public virtual AbstractEntity GetEditionData(int row)
		{
			return null;
		}

		public virtual FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			return FormattedText.Empty;
		}

		public virtual bool HasBottomSeparator(int row)
		{
			if (this.readonlyData == null || row < 0 || row >= this.readonlyData.Count)
			{
				return false;
			}
			else
			{
				return this.readonlyData[row].HasBottomSeparator;
			}
		}

		protected void SetBottomSeparatorToPreviousLine()
		{
			if (this.readonlyData.Count != 0)
			{
				this.readonlyData[this.readonlyData.Count-1].HasBottomSeparator = true;
			}
		}


		public virtual bool IsEditionCreationEnable
		{
			get
			{
				return true;
			}
		}

		public List<AbstractEditionData> EditionData
		{
			get
			{
				return this.editionData;
			}
		}

		public virtual void InsertEditionData(int index)
		{
		}

		public void RemoveAtEditionData(int index)
		{
			this.editionData.RemoveAt (index);
			this.countEditedRow = this.editionData.Count;
		}

		public int FirstEditedRow
		{
			get
			{
				return this.firstEditedRow;
			}
		}

		public int CountEditedRow
		{
			get
			{
				return this.countEditedRow;
			}
		}

		public virtual int InsertionPointRow
		{
			get
			{
				return -1;
			}
		}

		public bool IsModification
		{
			get
			{
				return this.isModification;
			}
		}

		public bool JustCreated
		{
			get
			{
				return this.justCreated;
			}
		}

		public virtual void StartCreationData()
		{
			this.firstEditedRow = -1;
			this.countEditedRow = 1;
		}

		public virtual void ResetCreationData()
		{
		}

		protected virtual void PrepareEditionLine(int line)
		{
		}

		public virtual void StartModificationData(int row)
		{
			this.firstEditedRow = row;
			this.countEditedRow = 1;
		}

		public virtual void UpdateEditionData()
		{
		}

		public virtual void RemoveModificationData()
		{
		}


		public FormattedText GetEditionText(int row, ColumnType columnType)
		{
			if (row >= 0 && row < this.editionData.Count)
			{
				return this.editionData[row].GetText (columnType);
			}

			return FormattedText.Null;
		}

		public void Validate(int row, ColumnType columnType)
		{
			if (row >= 0 && row < this.editionData.Count)
			{
				this.editionData[row].Validate (columnType);
			}
		}

		public bool HasEditionError(int row, ColumnType columnType)
		{
			if (row >= 0 && row < this.editionData.Count)
			{
				return this.editionData[row].HasError (columnType);
			}

			return false;
		}

		public FormattedText GetEditionError(int row, ColumnType columnType)
		{
			if (row >= 0 && row < this.editionData.Count)
			{
				return this.editionData[row].GetError (columnType);
			}

			return FormattedText.Null;
		}


		protected decimal? GetBudget(ComptaCompteEntity compte)
		{
			//	Retourne le montant d'un compte à considérer pour la colonne "budget".
			if (!this.options.BudgetEnable)
			{
				return null;
			}

			decimal? budget;

			switch (this.options.BudgetShowed)
			{
				case BudgetShowed.Budget:
					budget = compte.Budget;
					break;

				case BudgetShowed.Prorata:
					budget = compte.Budget * this.ProrataFactor;
					break;

				case BudgetShowed.Futur:
					budget = compte.BudgetFutur;
					break;

				case BudgetShowed.FuturProrata:
					budget = compte.BudgetFutur * this.ProrataFactor;
					break;

				case BudgetShowed.Précédent:
					budget = compte.BudgetPrécédent;
					break;

				default:
					budget = null;
					break;
			}

			this.SetMinMaxValue (budget);

			return budget;
		}

		private decimal ProrataFactor
		{
			get
			{
				int day = 0;

				if (this.lastEndDate.HasValue)
				{
					day = this.lastEndDate.Value.DayOfYear;

					if (this.lastBeginDate.HasValue)
					{
						day -= this.lastBeginDate.Value.DayOfYear;
					}
				}
				else
				{
					var écriture = this.périodeEntity.Journal.LastOrDefault ();
					if (écriture != null)
					{
						day = écriture.Date.DayOfYear;
					}
					else
					{
						day = Date.Today.DayOfYear / 365;
					}
				}

				return (decimal) day / 365.0M;
			}
		}

		protected FormattedText GetBudgetText(decimal? solde, decimal? budget)
		{
			//	Retourne le texte permettant d'afficher le budget, de différentes manières.
			if (!this.options.BudgetEnable)
			{
				return FormattedText.Empty;
			}

			if (this.options.BudgetDisplayMode == BudgetDisplayMode.Montant)
			{
				return AbstractDataAccessor.GetMontant (budget);
			}
			else
			{
				if (this.options.BudgetDisplayMode == BudgetDisplayMode.Pourcent)
				{
					return AbstractDataAccessor.GetPercent (solde, budget);
				}
				else if (this.options.BudgetDisplayMode == BudgetDisplayMode.PourcentMontant)
				{
					var percent = AbstractDataAccessor.GetPercent (solde, budget);
					var montant = AbstractDataAccessor.GetMontant (budget);

					if (percent.IsNullOrEmpty || montant.IsNullOrEmpty)
					{
						return FormattedText.Empty;
					}
					else
					{
						return TextFormatter.FormatText (percent, "de", montant);
					}
				}
				else if (this.options.BudgetDisplayMode == BudgetDisplayMode.Graphique)
				{
					return this.GetMinMaxText (budget, solde);
				}
				else
				{
					return AbstractDataAccessor.GetMontant (budget-solde);
				}
			}
		}

		protected static FormattedText GetPercent(decimal? m1, decimal? m2)
		{
			if (m1.HasValue && m2.HasValue && m2.Value != 0)
			{
				int n = (int) (m1.Value/m2.Value*100);
				return n.ToString () + "%";
			}
			else
			{
				return FormattedText.Empty;
			}
		}

		protected static FormattedText GetMontant(decimal? montant)
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


		#region Min/max tiny engine
		protected void MinMaxClear()
		{
			this.minValue = decimal.MaxValue;
			this.maxValue = decimal.MinValue;
		}

		protected void SetMinMaxValue(decimal? value)
		{
			if (value.HasValue)
			{
				this.minValue = System.Math.Min (this.minValue, value.Value);
				this.maxValue = System.Math.Max (this.maxValue, value.Value);
			}
		}

		protected FormattedText GetMinMaxText(decimal? value)
		{
			if (this.minValue == decimal.MaxValue ||
				this.maxValue == decimal.MinValue)
			{
				return FormattedText.Empty;
			}
			else
			{
				return FormattedText.Concat
					(
						StringArray.SpecialContentGraphicValue,
						"/",
						AbstractDataAccessor.GetMontant (this.minValue),
						"/",
						AbstractDataAccessor.GetMontant (this.maxValue),
						"/",
						AbstractDataAccessor.GetMontant (value)
					);
			}
		}

		protected FormattedText GetMinMaxText(decimal? value1, decimal? value2)
		{
			if (this.minValue == decimal.MaxValue ||
				this.maxValue == decimal.MinValue)
			{
				return FormattedText.Empty;
			}
			else
			{
				return FormattedText.Concat
					(
						StringArray.SpecialContentGraphicValue,
						"/",
						AbstractDataAccessor.GetMontant (this.minValue),
						"/",
						AbstractDataAccessor.GetMontant (this.maxValue),
						"/",
						AbstractDataAccessor.GetMontant (value1),
						"/",
						AbstractDataAccessor.GetMontant (value2)
					);
			}
		}
		#endregion


		protected readonly AbstractController			controller;
		protected readonly BusinessContext				businessContext;
		protected readonly ComptaEntity					comptaEntity;
		protected readonly ComptaPériodeEntity			périodeEntity;
		protected readonly List<ColumnMapper>			columnMappers;
		protected readonly MainWindowController			mainWindowController;
		protected readonly SoldesJournalManager			soldesJournalManager;
		protected readonly List<AbstractData>			readonlyAllData;
		protected readonly List<AbstractData>			readonlyData;
		protected readonly List<AbstractEditionData>	editionData;
		protected readonly List<SearchResult>			searchResults;

		protected SearchData							filterData;
		protected SearchData							searchData;
		protected AbstractOptions						options;
		protected int									firstEditedRow;
		protected int									countEditedRow;
		protected int									initialCountEditedRow;
		protected bool									isModification;
		protected bool									justCreated;
		protected int									searchLocator;
		protected decimal								minValue;
		protected decimal								maxValue;
		protected Date?									lastBeginDate;
		protected Date?									lastEndDate;
	}
}