//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Settings.Data;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Search.Controllers;
using Epsitec.Cresus.Compta.Options.Controllers;
using Epsitec.Cresus.Compta.Memory.Data;
using Epsitec.Cresus.Compta.Memory.Controllers;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Contrôleur générique pour la comptabilité, qui sert de base aux contrôleurs permettant l'édition
	/// (journal et plan comptable) ainsi qu'à ceux qui représentent des données 'readonly' (balance,
	/// bilan, etc.).
	/// </summary>
	public abstract class AbstractController
	{
		public AbstractController(Application app, BusinessContext businessContext, MainWindowController mainWindowController)
		{
			this.app                  = app;
			this.businessContext      = businessContext;
			this.mainWindowController = mainWindowController;

			this.comptaEntity  = this.mainWindowController.Compta;
			this.périodeEntity = this.mainWindowController.Période;
			this.settingsList  = this.mainWindowController.SettingsList;

			var mappers = this.InitialColumnMappers;
			if (mappers != null)
			{
				this.columnMappers = mappers.ToList ();
			}

			this.ignoreChanges = new SafeCounter ();

			this.app.CommandDispatcher.RegisterController (this);
		}

		public void SetVariousParameters(Window parentWindow, Command commandDocument)
		{
			this.parentWindow    = parentWindow;
			this.commandDocument = commandDocument;
		}


		public ComptaEntity ComptaEntity
		{
			get
			{
				return this.comptaEntity;
			}
		}

		public ComptaPériodeEntity PériodeEntity
		{
			get
			{
				return this.périodeEntity;
			}
		}

		public List<ColumnMapper> ColumnMappers
		{
			get
			{
				return this.columnMappers;
			}
		}

		public AbstractDataAccessor DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		public ArrayController ArrayController
		{
			get
			{
				return this.arrayController;
			}
		}

		public BusinessContext BusinessContext
		{
			get
			{
				return this.businessContext;
			}
		}

		public SettingsList SettingsList
		{
			get
			{
				return this.settingsList;
			}
		}

		public MemoryList MemoryList
		{
			get
			{
				return this.memoryList;
			}
		}

		public MainWindowController MainWindowController
		{
			get
			{
				return this.mainWindowController;
			}
		}


		public FrameBox CreateUI(FrameBox parent)
		{
			System.Diagnostics.Debug.Assert (this.parentWindow != null);
			System.Diagnostics.Debug.Assert (this.commandDocument != null);

			this.SetCommandEnable (Res.Commands.Edit.Accept, false);
			this.SetCommandEnable (Res.Commands.Edit.Cancel, false);
			this.SetCommandEnable (Res.Commands.Edit.Up, false);
			this.SetCommandEnable (Res.Commands.Edit.Down, false);
			this.SetCommandEnable (Res.Commands.Edit.Duplicate, false);
			this.SetCommandEnable (Res.Commands.Edit.Delete, false);
			this.SetCommandEnable (Res.Commands.Multi.Insert, false);
			this.SetCommandEnable (Res.Commands.Multi.Delete, false);
			this.SetCommandEnable (Res.Commands.Multi.Up, false);
			this.SetCommandEnable (Res.Commands.Multi.Down, false);
			this.SetCommandEnable (Res.Commands.Multi.Swap, false);
			this.SetCommandEnable (Res.Commands.Multi.Auto, false);

			this.mainWindowController.SetTitleComplement (null);

			this.frameBox = new FrameBox
			{
				Parent	= parent,
				Dock	= DockStyle.Fill,
			};

			this.CreateTitle (this.frameBox);
			this.CreateMemory (this.frameBox);
			this.CreateTopSearch (this.frameBox);
			this.CreateTopFilter (this.frameBox);
			this.CreateOptions (this.frameBox);
			this.CreateArray (this.frameBox);
			this.CreateFooter (this.frameBox);

			this.UpdateArrayContent ();

			if (this.footerController != null)
			{
				this.footerController.UpdateFooterGeometry ();
				this.footerController.UpdateFooterContent ();
			}

			this.SearchStartAction ();
			this.FilterUpdateTopToolbar ();
			this.UpdateTitle ();
			
			if (this.footerController != null)
			{
				this.footerController.FooterSelect (0);
			}

			this.CreateSpecificUI (this.frameBox);

			return this.frameBox;
		}

		protected virtual void CreateSpecificUI(FrameBox parent)
		{
		}

		public virtual void Dispose()
		{
			if (this.footerController != null)
			{
				this.footerController.Dispose ();
			}
		}

		public AbstractFooterController FooterController
		{
			get
			{
				return this.footerController;
			}
		}

		public AbstractOptionsController OptionsController
		{
			get
			{
				return this.optionsController;
			}
		}


		public virtual bool AcceptPériodeChanged
		{
			get
			{
				return true;
			}
		}


		public virtual bool HasArray
		{
			get
			{
				return true;
			}
		}

		public virtual bool HasShowSearchPanel
		{
			get
			{
				return false;
			}
		}

		public virtual bool HasShowFilterPanel
		{
			get
			{
				return false;
			}
		}

		public virtual bool HasShowOptionsPanel
		{
			get
			{
				return false;
			}
		}

		public bool HasShowMemoryPanel
		{
			get
			{
				return this.memoryList != null;
			}
		}

		public virtual bool HasShowInfoPanel
		{
			get
			{
				return false;
			}
		}

		public void UpdatePanelsShowed(bool memory, bool search, bool filter, bool options, bool info)
		{
			if (this.memoryController != null)
			{
				this.memoryController.ShowPanel = memory;
			}

			if (this.topSearchController != null)
			{
				this.topSearchController.ShowPanel = search;
			}

			if (this.topFilterController != null)
			{
				this.topFilterController.ShowPanel = filter;
			}

			if (this.optionsController != null)
			{
				this.optionsController.ShowPanel = options;
			}

			if (this.footerController != null)
			{
				this.footerController.ShowInfoPanel = info;
			}

			this.UpdateMemory ();
		}


		public bool SearchSpecialist
		{
			get
			{
				if (this.topSearchController == null)
				{
					return false;
				}
				else
				{
					return this.topSearchController.Specialist;
				}
			}
			set
			{
				if (this.topSearchController != null)
				{
					this.topSearchController.Specialist = value;
				}
			}
		}

		public bool FilterSpecialist
		{
			get
			{
				if (this.topFilterController == null)
				{
					return false;
				}
				else
				{
					return this.topFilterController.Specialist;
				}
			}
			set
			{
				if (this.topFilterController != null)
				{
					this.topFilterController.Specialist = value;
				}
			}
		}

		public bool OptionsSpecialist
		{
			get
			{
				if (this.optionsController == null)
				{
					return false;
				}
				else
				{
					return this.optionsController.Specialist;
				}
			}
			set
			{
				if (this.optionsController != null)
				{
					this.optionsController.Specialist = value;
				}
			}
		}


		#region Memory panel
		private void CreateMemory(FrameBox parent)
		{
			if (this.HasShowMemoryPanel)
			{
				this.memoryController = new MemoryController (this);
				this.memoryController.CreateUI (parent, this.MemoryChangedAction);
				this.memoryController.ShowPanel = this.mainWindowController.ShowMemoryPanel;
			}
		}

		private void MemoryChangedAction()
		{
			if (this.topSearchController != null)
			{
				this.topSearchController.UpdateContent ();
			}

			if (this.topFilterController != null)
			{
				this.topFilterController.UpdateContent ();
			}

			if (this.optionsController != null)
			{
				this.optionsController.UpdateContent ();
			}

			//this.SearchStartAction ();  // pas nécessaire, car FilterStartAction fait déjà tout !
			this.FilterStartAction ();
			this.OptionsChanged ();
		}

		public void UpdateAfterChanged()
		{
			//	Un changement de période ajuste les dates contenues dans les recherches et les filtres.
			//	Il faut donc actualiser ces éléments, après un changement de période.
			this.MemoryChangedAction ();
			this.UpdateMemory ();
		}

		protected void UpdateMemory()
		{
			if (this.memoryController != null)
			{
				this.memoryController.Update ();
			}
		}
		#endregion


		#region Search panel
		private void CreateTopSearch(FrameBox parent)
		{
			if (this.dataAccessor != null && this.dataAccessor.SearchData != null)
			{
				this.topSearchController = new TopSearchController (this);
				this.topSearchController.CreateUI (parent, this.SearchStartAction, this.SearchNextAction);
				this.topSearchController.ShowPanel = this.mainWindowController.ShowSearchPanel;
			}
		}

		private void SearchStartAction()
		{
			//	Appelé lorsque le critère de recherche a été modifié, et qu'il faut commencer une recherche.
			if (this.dataAccessor != null)
			{
				this.dataAccessor.SearchUpdate ();
				this.BaseUpdateArrayContent ();
				this.SearchUpdateLocator (true);
				this.SearchUpdateTopToolbar ();
				this.UpdateMemory ();
			}
		}

		private void SearchNextAction(int direction)
		{
			this.dataAccessor.SearchMoveLocator (direction);
			this.SearchUpdateLocator (true);
			this.SearchUpdateTopToolbar ();
		}

		private void SearchUpdateLocator(bool show)
		{
			if (this.arrayController != null)
			{
				int row;
				ColumnType columnType;
				this.dataAccessor.SearchLocatorInfo (out row, out columnType);

				this.arrayController.SetSearchLocator (row, columnType);

				if (show)
				{
					this.arrayController.ShowRow (row, 1);
				}
			}
		}

		public void SearchUpdateAfterModification()
		{
			//	Appelé lorsque les données ont été modifiées, et qu'il faut mettre à jour les recherches.
			this.dataAccessor.SearchUpdate ();
			this.SearchUpdateLocator (false);
			this.SearchUpdateTopToolbar ();
		}

		public void SearchUpdateTopToolbar()
		{
			if (this.topSearchController != null)
			{
				this.topSearchController.SetSearchCount (this.dataAccessor.Count, this.dataAccessor.SearchCount, this.dataAccessor.SearchLocator);
			}
		}
		#endregion


		#region Filter panel
		private void CreateTopFilter(FrameBox parent)
		{
			if (this.dataAccessor != null && this.dataAccessor.FilterData != null)
			{
				this.topFilterController = new TopFilterController (this);
				this.topFilterController.CreateUI (parent, this.FilterStartAction, this.FilterNextAction);
				this.topFilterController.ShowPanel = this.mainWindowController.ShowSearchPanel;

				this.dataAccessor.FilterUpdate ();
			}
		}

		private void FilterStartAction()
		{
			if (this.footerController != null)
			{
				this.footerController.Dirty = false;
			}

			if (this.arrayController != null && this.dataAccessor != null)
			{
				this.arrayController.SelectedRow = -1;
				this.dataAccessor.StartCreationLine ();
				this.arrayController.ColorSelection = UIBuilder.SelectionColor;
				this.arrayController.SetHilitedRows (this.dataAccessor.FirstEditedRow, this.dataAccessor.CountEditedRow);

				this.dataAccessor.FilterUpdate ();
				this.dataAccessor.SearchUpdate ();
				this.BaseUpdateArrayContent ();

				this.FilterUpdateTopToolbar ();
				this.SearchUpdateLocator (true);
				this.SearchUpdateTopToolbar ();
			}

			this.UpdateMemory ();

			if (this.footerController != null)
			{
				this.footerController.FooterValidate ();
			}
		}

		private void FilterNextAction(int direction)
		{
		}

		public void FilterUpdateTopToolbar()
		{
			if (this.topFilterController != null)
			{
				this.topFilterController.SetFilterCount (this.dataAccessor.Count, this.dataAccessor.Count, this.dataAccessor.AllCount);
			}
		}
		#endregion


		#region Options
		protected virtual void CreateOptions(FrameBox parent)
		{
		}

		protected virtual void OptionsChanged()
		{
			if (this.dataAccessor != null)
			{
				this.dataAccessor.UpdateAfterOptionsChanged ();
			}

			this.UpdateArrayContent ();
			this.UpdateTitle ();
			this.FilterUpdateTopToolbar ();
			this.UpdateMemory ();
		}
		#endregion


		#region Title
		private void CreateTitle(FrameBox parent)
		{
			var frame = new FrameBox
			{
				Parent          = this.frameBox,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 0, 4),
			};

			var toolbar = new PanelsToolbarController (this);
			toolbar.CreateUI (frame);

			this.titleLabel = new StaticText
			{
				Parent           = frame,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleLeft,
				PreferredHeight  = 20,
				Dock             = DockStyle.Fill,
				Margins          = new Margins (20, 0, 0, 0),
			};

			new GlyphButton
			{
				Parent           = frame,
				CommandObject    = Res.Commands.Compta.PériodeSuivante,
				GlyphShape       = GlyphShape.ArrowRight,
				ButtonStyle      = ButtonStyle.ToolItem,
				PreferredWidth   = 20,
				PreferredHeight  = 20,
				Dock             = DockStyle.Right,
			};

			new GlyphButton
			{
				Parent           = frame,
				CommandObject    = Res.Commands.Compta.PériodePrécédente,
				GlyphShape       = GlyphShape.ArrowLeft,
				ButtonStyle      = ButtonStyle.ToolItem,
				PreferredWidth   = 20,
				PreferredHeight  = 20,
				Dock             = DockStyle.Right,
			};

			this.subtitleLabel = new StaticText
			{
				Parent           = frame,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleRight,
				PreferredWidth   = 200,
				PreferredHeight  = 20,
				Dock             = DockStyle.Right,
				Margins          = new Margins (0, 10, 0, 0),
			};
		}

		protected virtual void UpdateTitle()
		{
		}

		protected void SetTitle(FormattedText title)
		{
			this.title = title;
			this.titleLabel.FormattedText = title.ApplyBold ().ApplyFontSize (13.0);
		}

		protected void SetSubtitle(FormattedText subtitle)
		{
			this.subtitle = subtitle;
			this.subtitleLabel.FormattedText = subtitle.ApplyBold ().ApplyFontSize (13.0);
		}

		public FormattedText MixTitle
		{
			get
			{
				if (this.subtitle.IsNullOrEmpty)
				{
					return this.title;
				}
				else
				{
					return this.title + " / " + this.subtitle;
				}
			}
		}
		#endregion

		#region Array
		private void CreateArray(FrameBox parent)
		{
			//	Crée le tableau principal avec son en-tête.
			if (this.HasArray)
			{
				this.arrayController = new ArrayController (this);
				this.arrayController.CreateUI (parent, this.ArrayUpdateCellContent, this.ArrayColumnsWidthChanged, this.ArraySelectedRowChanged);

				this.UpdateArray ();
			}
		}

		protected void UpdateArray()
		{
			if (this.arrayController != null)
			{
				this.arrayController.UpdateColumnsHeader (this.ArrayLineHeight);
			}
		}

		protected virtual int ArrayLineHeight
		{
			get
			{
				return 14;
			}
		}

		private void ArrayUpdateCellContent()
		{
			//	Appelé lorsqu'il faut mettre à jour le contenu du tableau.
			this.UpdateArrayContent ();
		}

		private void ArrayColumnsWidthChanged()
		{
			//	Appelé lorsque la largeur d'une colonne a changé.
			if (this.footerController != null)
			{
				this.footerController.UpdateFooterGeometry ();
			}
		}

		private void ArraySelectedRowChanged()
		{
			//	Appelé lorsque la ligne sélectionnée a changé.
			if (this.ignoreChanges.IsNotZero)
			{
				return;
			}

			int row = this.arrayController.SelectedRow;

			if (row == -1)
			{
				this.dataAccessor.StartCreationLine ();
			}
			else
			{
				this.dataAccessor.StartModificationLine (row);
			}

			this.arrayController.ColorSelection = UIBuilder.SelectionColor;
			this.arrayController.SetHilitedRows (this.dataAccessor.FirstEditedRow, this.dataAccessor.CountEditedRow);

			if (this.footerController != null)
			{
				this.footerController.UpdateFooterContent ();
				this.footerController.FooterSelect (this.arrayController.SelectedColumn, row - this.dataAccessor.FirstEditedRow);
				this.footerController.ShowSelection ();
			}
		}


		public void ClearHilite()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.arrayController.SelectedRow = -1;
				this.arrayController.SetHilitedRows (-1, 0);
			}

			this.dataAccessor.StartCreationLine ();
		}

		public void Update()
		{
			//	Met à jour l'ensemble du contrôleur.
			this.dataAccessor.UpdateAfterOptionsChanged ();
			this.BaseUpdateArrayContent ();

			if (this.footerController != null)
			{
				this.footerController.UpdateFooterContent ();
			}

			this.parentWindow.Text = this.mainWindowController.GetTitle (this.commandDocument);
		}

		protected void UpdateArrayContent()
		{
			//	Met à jour le contenu du tableau, ainsi que des tableaux des fenêtre associées.
			this.BaseUpdateArrayContent ();

			foreach (var controller in this.mainWindowController.Controllers)
			{
				if (controller != this)
				{
					controller.dataAccessor.UpdateAfterOptionsChanged ();
					controller.BaseUpdateArrayContent ();
				}
			}
		}

		private void BaseUpdateArrayContent()
		{
			//	Met à jour le contenu du tableau.
			if (this.arrayController != null)
			{
				this.arrayController.UpdateArrayContent (this.dataAccessor.Count, this.GetSearchArrayText, this.GetArrayBottomSeparator);
			}
		}

		private FormattedText GetSearchArrayText(int row, ColumnType columnType)
		{
			var text = this.GetArrayText (row, columnType);

			if (this.dataAccessor.SearchAny)
			{
				var result = this.dataAccessor.GetSearchResult (row, columnType);

				if (result == null)  // pas trouvé ?
				{
					if (!text.IsNullOrEmpty && !text.ToString ().StartsWith (Compta.Widgets.StringArray.SpecialContentStart))
					{
						text = text.ApplyFontColor (UIBuilder.TextOutsideSearchColor);  // gris
					}
				}
				else  // est-ce que le texte de recherche a été trouvé dans cette cellule ?
				{
					text = result.HilitedText;  // utilise le texte marqué
				}
			}

			return text;
		}

		protected virtual FormattedText GetArrayText(int row, ColumnType columnType)
		{
			//	Retourne le texte contenu dans une cellule.
			return this.dataAccessor.GetText (row, columnType);
		}

		protected bool GetArrayBottomSeparator(int row)
		{
			return this.dataAccessor.HasBottomSeparator (row);
		}


		protected void HiliteHeaderColumn(ColumnType columnType)
		{
			//	Met en évidence une en-tête de colonne à choix.
			this.arrayController.HiliteHeaderColumn (columnType);
		}
		#endregion


		protected virtual void CreateFooter(FrameBox parent)
		{
		}


		protected virtual IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				return null;
			}
		}

		protected virtual void UpdateColumnMappers()
		{
		}

		protected void ShowHideColumn(ColumnType columnType, bool show)
		{
			//	Détermine l'état montré/caché d'une colonne.
			var mapper = this.columnMappers.Where (x => x.Column == columnType).FirstOrDefault ();

			if (mapper != null)  // garde-fou
			{
				mapper.Show = show;

				if (this.topSearchController != null)
				{
					this.topSearchController.UpdateColumns ();
				}

				if (this.topFilterController != null)
				{
					this.topFilterController.UpdateColumns ();
				}
			}
		}

		protected void SetColumnDescription(ColumnType columnType, FormattedText description)
		{
			//	Modifie la description d'une colonne, visible dans l'en-tête du tableau.
			var mapper = this.columnMappers.Where (x => x.Column == columnType).FirstOrDefault ();

			if (mapper != null)  // garde-fou
			{
				mapper.Description = description;
			}
		}


		public bool GetCommandEnable(Command command)
		{
			CommandState cs = this.app.CommandContext.GetCommandState (command);

			if (cs == null)
			{
				return false;
			}
			else
			{
				return cs.Enable;
			}
		}

		public void SetCommandEnable(Command command, bool enable)
		{
			CommandState cs = this.app.CommandContext.GetCommandState (command);

			if (cs != null)
			{
				cs.Enable =enable;
			}
		}


		public SafeCounter IgnoreChanges
		{
			get
			{
				return this.ignoreChanges;
			}
		}


		protected readonly Application							app;
		protected readonly BusinessContext						businessContext;
		protected readonly ComptaEntity							comptaEntity;
		protected readonly ComptaPériodeEntity					périodeEntity;
		protected readonly SettingsList							settingsList;
		protected readonly List<ColumnMapper>					columnMappers;
		protected readonly SafeCounter							ignoreChanges;

		protected MainWindowController							mainWindowController;
		protected Window										parentWindow;
		protected Command										commandDocument;

		protected AbstractDataAccessor							dataAccessor;
		protected MemoryList									memoryList;

		protected TopSearchController							topSearchController;
		protected TopFilterController							topFilterController;
		protected AbstractOptionsController						optionsController;
		protected MemoryController								memoryController;
		protected ArrayController								arrayController;
		protected AbstractFooterController						footerController;
		protected FrameBox										frameBox;
		protected StaticText									titleLabel;
		protected StaticText									subtitleLabel;
		protected FormattedText									title;
		protected FormattedText									subtitle;
	}
}
