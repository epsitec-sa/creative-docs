//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

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
		public AbstractDataAccessor(BusinessContext businessContext, ComptabilitéEntity comptabilitéEntity)
		{
			this.businessContext    = businessContext;
			this.comptabilitéEntity = comptabilitéEntity;

			this.readonlyData = new List<AbstractData> ();
			this.editionData = new List<AbstractEditionData> ();
			this.searchResults = new List<SearchResult> ();
		}


		public AbstractOptions AccessorOptions
		{
			get
			{
				return this.options;
			}
		}

		public virtual void UpdateAfterOptionsChanged()
		{
		}


		public void SearchUpdate(IEnumerable<ColumnType> columns, SearchingData data)
		{
			//	Met à jour les recherches, en fonction d'une nouvelle chaîne cherchée.
			this.searchingColumns = columns;
			this.searchingData    = data;

			this.SearchUpdate ();
		}

		protected void SearchUpdate()
		{
			this.searchResults.Clear ();

			if (this.searchingData != null && !this.searchingData.IsEmpty)
			{
				int count = this.Count;
				for (int row = 0; row < this.Count; row++)
				{
					var list = new List<SearchResult> ();

					foreach (var column in this.searchingColumns)
					{
						var text = this.GetText (row, column);

						foreach (var tab in this.searchingData.TabsData)
						{
							if (tab.Column != ColumnType.None && tab.Column != column)
							{
								continue;
							}

							var result = new SearchResult (row, column, text, tab);
							if (result.Count > 0)
							{
								list.Add (result);

								if (this.searchingData.OrMode)
								{
									break;
								}
							}
						}
					}

					if (this.searchingData.OrMode || list.Count == this.searchingData.TabsData.Count)
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
				if (this.searchingData == null || this.searchingData.IsEmpty)
				{
					return null;
				}
				else
				{
					return this.searchResults.Count;
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


		public virtual int Count
		{
			get
			{
				if (this.readonlyData != null)
				{
					return this.readonlyData.Count;
				}
				else
				{
					return 0;
				}
			}
		}

		public AbstractData GetReadOnlyData(int row)
		{
			if (this.readonlyData == null || row < 0 || row >= this.readonlyData.Count)
			{
				return null;
			}
			else
			{
				return this.readonlyData[row];
			}
		}

		public virtual AbstractEntity GetEditionData(int row)
		{
			return null;
		}

		public virtual FormattedText GetText(int row, ColumnType column)
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
		}

		public virtual void ResetCreationData()
		{
		}

		protected virtual void PrepareEditionLine(int line)
		{
		}

		public virtual void StartModificationData(int row)
		{
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


		protected decimal? GetBudget(ComptabilitéCompteEntity compte)
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

				if (this.options.DateFin.HasValue)
				{
					day = this.options.DateFin.Value.DayOfYear;

					if (this.options.DateDébut.HasValue)
					{
						day -= this.options.DateDébut.Value.DayOfYear;
					}
				}
				else
				{
					var écriture = this.comptabilitéEntity.Journal.LastOrDefault ();
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


		protected readonly BusinessContext				businessContext;
		protected readonly ComptabilitéEntity			comptabilitéEntity;
		protected readonly List<AbstractData>			readonlyData;
		protected readonly List<AbstractEditionData>	editionData;
		protected readonly List<SearchResult>			searchResults;

		protected AbstractOptions						options;
		protected int									firstEditedRow;
		protected int									countEditedRow;
		protected int									initialCountEditedRow;
		protected bool									isModification;
		protected bool									justCreated;
		protected int									searchLocator;
		protected IEnumerable<ColumnType>				searchingColumns;
		protected SearchingData							searchingData;
		protected decimal								minValue;
		protected decimal								maxValue;
	}
}