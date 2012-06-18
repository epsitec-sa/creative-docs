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
using Epsitec.Cresus.Compta.Graph;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Permanents;
using Epsitec.Cresus.Compta.ViewSettings.Data;

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

			this.cube = new Cube ();

			this.searchDataCollection = this.mainWindowController.GetSettingsSearchDataCollection (Présentations.GetSearchSettingsCollectionKey (this.controller.ControllerType));
			this.filterDataCollection = this.mainWindowController.GetSettingsSearchDataCollection (Présentations.GetFilterSettingsCollectionKey (this.controller.ControllerType));
		}


		public List<ColumnMapper> ColumnMappers
		{
			get
			{
				return this.columnMappers;
			}
		}

		public ViewSettingsList ViewSettingsList
		{
			get
			{
				return this.viewSettingsList;
			}
		}

		public SearchDataCollection SearchDataCollection
		{
			get
			{
				return this.searchDataCollection;
			}
		}

		public SearchDataCollection FilterDataCollection
		{
			get
			{
				return this.filterDataCollection;
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

		public Cube Cube
		{
			get
			{
				return this.cube;
			}
		}

		public Cube CubeToDraw
		{
			get
			{
				return this.cubeToDraw;
			}
		}

		public GraphOptions ArrayGraphOptions
		{
			get
			{
				return this.arrayGraphOptions;
			}
		}

		public virtual void UpdateAfterOptionsChanged()
		{
			this.UpdateGraphDataToDraw ();
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
					var results = new List<SearchResult> ();

					if (this.searchData.Process (results, row, (x, y) => this.GetText (x, y), this.columnMappers.Where (x => x.Show).Select (x => x.Column)))
					{
						results.ForEach (x => this.searchResults.Add (x));
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


		public virtual void UpdateFilter()
		{
			//	Met à jour le filtre.
			this.UpdateMergedFilter ();
			this.readonlyData.Clear ();

			if (!this.HasFilter)
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

			this.UpdateGraphData (force: false);
		}

		public virtual void UpdateGraphData(bool force)
		{
			//	Appelé après la mise à jour du filtre, pour mettre à jour les données graphiques.
		}

		protected void UpdateGraphDataToDraw()
		{
			//	Génère le cube final, prêt à être dessiné, en fonction des options (filtres, seuils, etc.).
			if (this.cube != null && this.cube.Dimensions != 0)
			{
				var options = this.options.GraphOptions;

				this.cubeToDraw = new Cube ();
				this.cubeToDraw.FilteredCopy (this.cube, options.PrimaryDimension, options.SecondaryDimension, options.PrimaryFilter, options.SecondaryFilter, null);

				if (options.HasThreshold0)
				{
					var pc = new Cube ();
					pc.ThresholdCopy0 (this.cubeToDraw, options.ThresholdValue0);
					this.cubeToDraw = pc;
				}

				if (options.HasThreshold1)
				{
					var pc = new Cube ();
					pc.ThresholdCopy1 (this.cubeToDraw, options.ThresholdValue1);
					this.cubeToDraw = pc;
				}
			}
		}

		protected bool HasFilter
		{
			get
			{
				return (this.mergedFilterData != null && !this.mergedFilterData.IsEmpty) ||
					   (this.searchData != null && this.searchData.QuickFilter && !this.searchData.IsEmpty) ||
					   (this.mainWindowController.TemporalData != null && !this.mainWindowController.TemporalData.IsEmpty);
			}
		}

		protected bool FilterLine(int row)
		{
			if (!this.mergedFilterData.Process (null, row, (x, y) => this.GetText (x, y, true), this.columnMappers.Where (x => x.Show).Select (x => x.Column)))
			{
				return false;
			}

			if (this.searchData != null && this.searchData.QuickFilter && !this.searchData.IsEmpty &&
				!this.searchData.Process (null, row, (x, y) => this.GetText (x, y, true), this.columnMappers.Where (x => x.Show).Select (x => x.Column)))
			{
				this.hasQuickFilter = true;
				return false;
			}

			if (this.mainWindowController.TemporalData != null && !this.mainWindowController.TemporalData.IsEmpty)
			{
				var date = Converters.ParseDate (this.GetText (row, ColumnType.Date, true));
				if (!this.mainWindowController.TemporalData.Match (date))
				{
					return false;
				}
			}

			return true;
		}

		public bool HasQuickFilter
		{
			get
			{
				return this.hasQuickFilter;
			}
		}

		protected void UpdateMergedFilter()
		{
			//	Met à jour mergedFilterData à partir de filterData, en y incorporant les données à
			//	filtrer provenant des options. Cela est nécessaire, puisque, contre toute logique, il
			//	a été décidé de mettre la plupart des choix concernant le filtre dans les options !
			this.mergedFilterData = this.filterData.CopyFrom ();
			this.hasQuickFilter = false;

			if (!this.filterData.Enable)
			{
				this.mergedFilterData.Clear ();
			}

			if (this.options != null)
			{
				if (this.options.Catégories != CatégorieDeCompte.Tous)
				{
					this.mergedFilterData.BeginnerCatégories = this.options.Catégories;
				}

				if (this.options.DeepFrom != 1 || this.options.DeepTo != int.MaxValue)
				{
					this.mergedFilterData.SetBeginnerProfondeurs (this.options.DeepFrom, this.options.DeepTo);
				}

				if (this.options.ZeroFiltered)
				{
					this.mergedFilterData.BeginnerHideNuls = true;
				}
			}
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
			for (int i=0; i<this.readonlyData.Count; i++)
			{
				var data = this.readonlyData[i];

				if (data.Entity == entity)
				{
					return i;
				}
			}

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

		public int CountEditedRowWithoutEmpty
		{
			get
			{
				return this.countEditedRow - this.CountEmptyRow;
			}
		}

		public virtual int CountEmptyRow
		{
			get
			{
				return 0;
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

		public bool IsActive
		{
			get
			{
				return this.isCreation || this.isModification;
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
			this.isCreation = false;
			this.isModification = false;

			this.firstEditedRow = -1;
			this.countEditedRow = 1;

			foreach (var e in this.editionLine)
			{
				e.Clear ();
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
			this.editionLine[0].Prepare ();
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


		protected static FormattedText GetGraphicText(int row)
		{
			//	Retourne un texte au format "$${_graphic_}$$;row", qui indique qu'il faut dessiner
			//	le graphique contenu dans le Cube à la ligne "row".
			return StringArray.SpecialContentGraphicValue + ";" + Converters.IntToString (row);
		}


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

		protected ViewSettingsList						viewSettingsList;
		protected SearchData							searchData;
		protected SearchData							filterData;
		protected SearchData							mergedFilterData;
		protected AbstractOptions						options;
		protected AbstractPermanents					permanents;
		protected SearchDataCollection					searchDataCollection;
		protected SearchDataCollection					filterDataCollection;
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
		protected Cube									cube;
		protected Cube									cubeToDraw;
		protected GraphOptions							arrayGraphOptions;
		protected bool									hasQuickFilter;
	}
}