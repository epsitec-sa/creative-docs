//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Widgets;
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
	/// Gère l'accès à des données génériques de la comptabilité.
	/// </summary>
	public abstract class AbstractDataAccessor
	{
		public AbstractDataAccessor(AbstractController controller)
		{
			this.controller = controller;

			this.businessContext      = this.controller.BusinessContext;
			this.compta               = this.controller.ComptaEntity;
			this.période              = this.controller.PériodeEntity;
			this.columnMappers        = this.controller.ColumnMappers;
			this.mainWindowController = this.controller.MainWindowController;

			this.soldesJournalManager = new SoldesJournalManager (this.compta);

			this.readonlyAllData = new List<AbstractData> ();
			this.readonlyData    = new List<AbstractData> ();
			this.editionLine     = new List<AbstractEditionLine> ();
			this.searchResults   = new List<SearchResult> ();
		}


		public List<ColumnMapper> ColumnMappers
		{
			get
			{
				return this.columnMappers;
			}
		}

		public AbstractPermanents Permanents
		{
			get
			{
				return this.permanents;
			}
		}

		public AbstractOptions Options
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
#if true
			this.searchResults.Clear ();

			if (this.searchData != null && !this.searchData.IsEmpty)
			{
				int count = this.Count;
				for (int row = 0; row < count; row++)
				{
					var list = new List<SearchResult> ();

					bool allNode = true;
					bool oneNode = false;

					foreach (var node in this.searchData.NodesData)
					{
						if (node.IsEmpty)
						{
							continue;
						}

						bool allTab = true;
						bool oneTab = false;

						foreach (var tab in node.TabsData)
						{
							bool tabFound = false;

							if (tab.Column == ColumnType.None)  // cherche dans toutes les colonnes ?
							{
								foreach (var column in this.columnMappers.Where (x => x.Show).Select (x => x.Column))
								{
									var text = this.GetText (row, column);
									int n = tab.SearchText.Search (ref text);

									if (n != 0)  // trouvé ?
									{
										list.Add (new SearchResult (row, column, text));
										tabFound = true;
									}
								}
							}
							else  // cherche dans une colonne précise ?
							{
								var text = this.GetText (row, tab.Column);
								int n = tab.SearchText.Search (ref text);

								if (n != 0)  // trouvé ?
								{
									list.Add (new SearchResult (row, tab.Column, text));
									tabFound = true;
								}
							}

							if (tabFound)  // trouvé ?
							{
								oneTab = true;
							}
							else  // pas trouvé ?
							{
								allTab = false;
							}
						}

						if (node.OrMode)  // mode "ou" ?
						{
							if (oneTab)
							{
								oneNode = true;
							}
							else
							{
								allNode = false;
							}
						}
						else  // mode "et" ?
						{
							if (allTab)
							{
								oneNode = true;
							}
							else
							{
								allNode = false;
							}
						}
					}

					bool found = false;

					if (this.searchData.OrMode)  // mode "ou" ?
					{
						if (oneNode)
						{
							found = true;
						}
					}
					else  // mode "et" ?
					{
						if (allNode)
						{
							found = true;
						}
					}

					if (found)
					{
						list.ForEach (x => this.searchResults.Add (x));
					}
				}
			}

			this.searchLocator = 0;
#else
			this.searchResults.Clear ();

			if (this.searchData != null && !this.searchData.IsEmpty)
			{
				int count = this.Count;
				for (int row = 0; row < count; row++)
				{
					var list = new List<SearchResult> ();
					bool found = true;

					foreach (var node in this.searchData.NodesData)
					{
						if (node.IsEmpty)
						{
							continue;
						}

						int tabFounds = 0;

						foreach (var column in this.columnMappers.Where (x => x.Show).Select (x => x.Column))
						{
							foreach (var tab in node.TabsData)
							{
								if (tab.Column != ColumnType.None && tab.Column != column)
								{
									continue;
								}

								var text = this.GetText (row, column);
								int n = tab.SearchText.Search (ref text);

								if (n != 0)  // trouvé ?
								{
									list.Add (new SearchResult (row, column, text));
									tabFounds++;
								}
							}
						}

						if (tabFounds == 0)
						{
							found = false;
							break;
						}
					}

					if (found)
					{
						list.ForEach (x => this.searchResults.Add (x));
					}
				}
			}

			this.searchLocator = 0;
#endif
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

			if (this.filterData == null || !this.filterData.IsValid)
			{
				this.readonlyData.AddRange (this.readonlyAllData);
			}
			else
			{
				int count = this.readonlyAllData.Count;
				for (int row = 0; row < count; row++)
				{
					if (this.readonlyAllData[row].NeverFiltered)
					{
						this.readonlyData.Add (this.readonlyAllData[row]);
					}
					else
					{
						if (this.FilterLine (row))
						{
							this.readonlyData.Add (this.readonlyAllData[row]);
						}
					}
				}
			}
		}

		protected bool FilterLine(int row)
		{
			foreach (var node in this.filterData.NodesData)
			{
				if (!node.IsValid)
				{
					continue;
				}

				int tabFounds = 0;

				foreach (var column in this.columnMappers.Select (x => x.Column))
				{
					foreach (var tab in node.TabsData)
					{
						if (tab.Column != ColumnType.None && tab.Column != column)
						{
							continue;
						}

						if (!tab.IsValid)
						{
							continue;
						}

						var text = this.GetText (row, column, true);
						int n = tab.SearchText.Search (ref text);

						if (n != 0)  // trouvé ?
						{
							tabFounds++;
						}
					}
				}

				if (tabFounds == 0)
				{
					return false;
				}
			}

			return true;
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


		public virtual int GetIndexOf(AbstractEntity entity)
		{
			return -1;
		}


		public virtual AbstractEntity GetEditionEntity(int row)
		{
			return null;
		}

		public virtual int GetEditionIndex(AbstractEntity entity)
		{
			return -1;
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

		public EditionData GetEditionData(int line, ColumnType columnType)
		{
			if (line < 0 || line >= this.editionLine.Count)
			{
				return null;
			}
			else
			{
				return this.editionLine[line].GetData (columnType);
			}
		}

		public List<AbstractEditionLine> EditionLine
		{
			get
			{
				return this.editionLine;
			}
		}

		public virtual void InsertEditionLine(int index)
		{
			this.countEditedRow = this.editionLine.Count;
			this.controller.EditorController.UpdateFieldsEditionData ();
		}

		public void RemoveAtEditionLine(int index)
		{
			this.editionLine.RemoveAt (index);
			this.countEditedRow = this.editionLine.Count;
		}

		public virtual bool MoveEditionLine(int direction)
		{
			return false;
		}

		public virtual bool IsMoveEditionLineEnable(int direction)
		{
			return false;
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

		public virtual ColumnType ColumnForInsertionPoint
		{
			get
			{
				return ColumnType.None;
			}
		}

		public virtual int InsertionPointRow
		{
			get
			{
				return -1;
			}
		}

		public bool IsCreation
		{
			get
			{
				return this.isCreation;
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

		public void StartDefaultLine()
		{
			if (this.controller.HasCreateCommand)
			{
				this.isCreation = false;
				this.isModification = false;

				this.firstEditedRow = -1;
				this.countEditedRow = 1;

				foreach (var e in this.editionLine)
				{
					e.Clear ();
				}
			}
			else
			{
				this.StartCreationLine ();
			}
		}

		public virtual void StartCreationLine()
		{
			this.firstEditedRow = -1;
			this.countEditedRow = 1;
		}

		public virtual void ResetCreationLine()
		{
		}

		protected virtual void PrepareEditionLine(int line)
		{
		}

		public virtual void StartModificationLine(int row)
		{
			this.firstEditedRow = row;
			this.countEditedRow = 1;
		}

		public virtual void UpdateEditionLine()
		{
		}

		public virtual FormattedText GetRemoveModificationLineError()
		{
			return FormattedText.Null;  // ok
		}

		public virtual FormattedText GetRemoveModificationLineQuestion()
		{
			return "Voulez-vous supprimer la ligne sélectionnée ?";
		}

		public virtual void RemoveModificationLine()
		{
		}

		public void DuplicateModificationLine()
		{
			this.firstEditedRow = -1;
			this.countEditedRow = 1;

			this.isCreation = true;
			this.isModification = false;
			this.justCreated = false;
		}


		public FormattedText GetEditionText(int row, ColumnType columnType)
		{
			if (row >= 0 && row < this.editionLine.Count)
			{
				return this.editionLine[row].GetText (columnType);
			}

			return FormattedText.Null;
		}

		public void Validate(int row, ColumnType columnType)
		{
			if (row >= 0 && row < this.editionLine.Count)
			{
				this.editionLine[row].Validate (columnType);
			}
		}

		public bool HasEditionError(int row, ColumnType columnType)
		{
			if (row >= 0 && row < this.editionLine.Count)
			{
				return this.editionLine[row].HasError (columnType);
			}

			return false;
		}

		public FormattedText GetEditionError(int row, ColumnType columnType)
		{
			if (row >= 0 && row < this.editionLine.Count)
			{
				return this.editionLine[row].GetError (columnType);
			}

			return FormattedText.Null;
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
						Converters.MontantToString (this.minValue),
						"/",
						Converters.MontantToString (this.maxValue),
						"/",
						Converters.MontantToString (value)
					);
			}
		}
		#endregion


		protected readonly AbstractController			controller;
		protected readonly BusinessContext				businessContext;
		protected readonly ComptaEntity					compta;
		protected readonly ComptaPériodeEntity			période;
		protected readonly List<ColumnMapper>			columnMappers;
		protected readonly MainWindowController			mainWindowController;
		protected readonly SoldesJournalManager			soldesJournalManager;
		protected readonly List<AbstractData>			readonlyAllData;
		protected readonly List<AbstractData>			readonlyData;
		protected readonly List<AbstractEditionLine>	editionLine;
		protected readonly List<SearchResult>			searchResults;

		protected SearchData							filterData;
		protected SearchData							searchData;
		protected AbstractOptions						options;
		protected AbstractPermanents					permanents;
		protected int									firstEditedRow;
		protected int									countEditedRow;
		protected int									initialCountEditedRow;
		protected bool									isCreation;
		protected bool									isModification;
		protected bool									justCreated;
		protected int									searchLocator;
		protected decimal								minValue;
		protected decimal								maxValue;
		protected Date?									lastBeginDate;
		protected Date?									lastEndDate;
	}
}