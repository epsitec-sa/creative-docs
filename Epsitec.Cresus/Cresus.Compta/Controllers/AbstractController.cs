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
using Epsitec.Cresus.Compta.ViewSettings.Data;
using Epsitec.Cresus.Compta.ViewSettings.Controllers;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Graph;

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
		public AbstractController(ComptaApplication app, BusinessContext businessContext, MainWindowController mainWindowController)
		{
			this.app                  = app;
			this.businessContext      = businessContext;
			this.mainWindowController = mainWindowController;

			this.compta       = this.mainWindowController.Compta;
			this.période      = this.mainWindowController.Période;
			this.settingsList = this.mainWindowController.SettingsList;

			var mappers = this.InitialColumnMappers;
			if (mappers != null)
			{
				this.columnMappers = mappers.ToList ();
			}

			this.app.CommandDispatcher.RegisterController (this);
		}


		public ComptaEntity ComptaEntity
		{
			get
			{
				return this.compta;
			}
		}

		public ComptaPériodeEntity PériodeEntity
		{
			get
			{
				return this.période;
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

		public GraphWidget GraphWidget
		{
			get
			{
				return this.graphWidget;
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


		public ViewSettingsList ViewSettingsList
		{
			get
			{
				if (this.dataAccessor == null)
				{
					return this.DirectViewSettingsList;
				}
				else
				{
					return this.dataAccessor.ViewSettingsList;
				}
			}
		}

		protected virtual ViewSettingsList DirectViewSettingsList
		{
			get
			{
				return null;
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
			this.SetCommandEnable (Res.Commands.Select.Up, false);
			this.SetCommandEnable (Res.Commands.Select.Down, false);
			this.SetCommandEnable (Res.Commands.Select.Home, false);

			this.SetCommandEnable (Res.Commands.Edit.Create, false);
			this.SetCommandEnable (Res.Commands.Edit.Accept, false);
			this.SetCommandEnable (Res.Commands.Edit.Cancel, false);
			this.SetCommandEnable (Res.Commands.Edit.Up, false);
			this.SetCommandEnable (Res.Commands.Edit.Down, false);
			this.SetCommandEnable (Res.Commands.Edit.Duplicate, false);
			this.SetCommandEnable (Res.Commands.Edit.Delete, false);

			this.SetCommandEnable (Res.Commands.Multi.LastLine, false);
			this.SetCommandEnable (Res.Commands.Multi.InsertBefore, false);
			this.SetCommandEnable (Res.Commands.Multi.InsertAfter, false);
			this.SetCommandEnable (Res.Commands.Multi.InsertTVA, false);
			this.SetCommandEnable (Res.Commands.Multi.Delete, false);
			this.SetCommandEnable (Res.Commands.Multi.Up, false);
			this.SetCommandEnable (Res.Commands.Multi.Down, false);
			this.SetCommandEnable (Res.Commands.Multi.Swap, false);
			this.SetCommandEnable (Res.Commands.Multi.Split, false);
			this.SetCommandEnable (Res.Commands.Multi.Join, false);
			this.SetCommandEnable (Res.Commands.Multi.Auto, false);

			this.frameBox = new FrameBox
			{
				Parent	= parent,
				Dock	= DockStyle.Fill,
			};

			this.CreateViewSettings (this.frameBox);
			this.CreateTopOptions (this.frameBox);
			this.CreateTopFilter (this.frameBox);
			this.CreateTopSearch (this.frameBox);

			if (Présentations.HasRightEditor (this.ControllerType))
			{
				var band = new FrameBox
				{
					Parent = this.frameBox,
					Dock   = DockStyle.Fill,
				};

				var left = new FrameBox
				{
					Parent = band,
					Dock   = DockStyle.Fill,
				};

				var right = new FrameBox
				{
					Parent  = band,
					Dock    = DockStyle.Right,
					Margins = new Margins (10, 0, 0, 0),
				};

				this.CreateArray (left);
				this.CreateEditor (right);
			}
			else
			{
				this.CreateArray (this.frameBox);
				this.CreateGraph (this.frameBox);
				this.CreateEditor (this.frameBox);
			}

			this.UpdateArrayContent ();

			if (this.editorController != null)
			{
				this.editorController.UpdateEditorGeometry ();
				this.editorController.UpdateEditorContent ();
			}

			this.SearchStartAction ();
			this.FilterUpdateTopToolbar ();
			
			if (this.editorController != null)
			{
				this.editorController.EditorSelect (0);
			}

			this.CreateSpecificUI (this.frameBox);

			return this.frameBox;
		}

		protected virtual void CreateSpecificUI(FrameBox parent)
		{
		}

		public virtual void Dispose()
		{
			if (this.editorController != null)
			{
				this.editorController.Dispose ();
			}
		}

		public AbstractEditorController EditorController
		{
			get
			{
				return this.editorController;
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


		public virtual double RightEditorWidth
		{
			get
			{
				return 200;
			}
		}


		public void UpdatePanelsShowed(bool search, bool filter, bool options, bool info)
		{
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

			if (this.editorController != null)
			{
				this.editorController.ShowInfoPanel = info;
			}

			this.UpdateViewSettings ();
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


		public virtual void CreateAction()
		{
			if (this.editorController != null)
			{
				this.editorController.CreateAction ();
			}
		}

		public virtual void AcceptAction()
		{
			if (this.editorController != null)
			{
				this.editorController.AcceptAction ();
			}
		}

		public virtual void CancelAction()
		{
			if (this.editorController != null)
			{
				this.editorController.CancelAction ();
			}
		}


		public void LinkHiliteOptionsPanel(bool hilite)
		{
			if (this.optionsController != null)
			{
				this.optionsController.LinkHilitePanel (hilite);
			}
		}

		public void LinkHiliteFilterPanel(bool hilite)
		{
			if (this.topFilterController != null)
			{
				this.topFilterController.LinkHilitePanel (hilite);
			}
		}

		public void LinkHiliteSearchPanel(bool hilite)
		{
			if (this.topSearchController != null)
			{
				this.topSearchController.LinkHilitePanel (hilite);
			}
		}

		public void LinkHiliteOptionsButton(bool hilite)
		{
			if (this.viewSettingsController != null)
			{
				this.viewSettingsController.LinkHiliteOptionsButton (hilite);
			}
		}

		public void LinkHiliteFilterButton(bool hilite)
		{
			if (this.viewSettingsController != null)
			{
				this.viewSettingsController.LinkHiliteFilterButton (hilite);
			}
		}

		public void LinkHiliteSearchButton(bool hilite)
		{
			if (this.viewSettingsController != null)
			{
				this.viewSettingsController.LinkHiliteSearchButton (hilite);
			}
		}


		#region ViewSettings panel
		private void CreateViewSettings(FrameBox parent)
		{
			this.viewSettingsController = new ViewSettingsController (this);
			this.viewSettingsController.CreateUI (parent, this.ViewSettingsChangedAction);
		}

		private void ViewSettingsChangedAction()
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
			this.ViewSettingsChangedAction ();
			this.UpdateViewSettings ();
		}

		protected void UpdateViewSettings()
		{
			if (this.viewSettingsController != null)
			{
				this.viewSettingsController.Update ();
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
				this.topSearchController.ShowPanel = this.ShowSearchPanel;
			}
		}

		public void SearchStartAction()
		{
			//	Appelé lorsque le critère de recherche a été modifié, et qu'il faut commencer une recherche.
			if (this.dataAccessor != null)
			{
				if (this.dataAccessor.SearchData.QuickFilter || this.dataAccessor.HasQuickFilter != this.dataAccessor.SearchData.QuickFilter)
				{
					this.dataAccessor.UpdateFilter ();
				}

				this.dataAccessor.SearchUpdate ();
				this.BaseUpdateArrayContent ();
				this.SearchUpdateLocator (true);
				this.SearchUpdateTopToolbar ();
				this.UpdateViewSettings ();
				this.mainWindowController.UpdateTitle ();
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

		public void QuickFilterClear()
		{
			if (this.topSearchController != null)
			{
				this.topSearchController.QuickFilterClear ();
			}
		}

		public void SearchUpdatePériode()
		{
			if (this.topSearchController != null)
			{
				this.topSearchController.UpdatePériode ();
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
				this.topFilterController.ShowPanel = this.ShowFilterPanel;

				this.dataAccessor.UpdateFilter ();
			}
		}

		public void FilterStartAction()
		{
			if (this.editorController != null)
			{
				this.editorController.Dirty = false;
			}

			if (this.arrayController != null && this.dataAccessor != null)
			{
				this.arrayController.SelectedRow = -1;
				this.dataAccessor.StartDefaultLine ();
				this.arrayController.ColorSelection = UIBuilder.SelectionColor;
				this.arrayController.ColorHilite = UIBuilder.HiliteColor;
				this.arrayController.SetHilitedRows (this.dataAccessor.FirstEditedRow, this.dataAccessor.CountEditedRow);

				this.dataAccessor.UpdateFilter ();
				this.dataAccessor.SearchUpdate ();
				this.BaseUpdateArrayContent ();

				this.FilterUpdateTopToolbar ();
				this.SearchUpdateLocator (true);
				this.SearchUpdateTopToolbar ();
				this.mainWindowController.UpdateTitle ();
			}

			this.UpdateViewSettings ();

			if (this.editorController != null)
			{
				this.editorController.EditorValidate ();
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

		public void FilterClear()
		{
			if (this.topFilterController != null)
			{
				this.topFilterController.SearchClear ();
			}
		}

		public void FilterUpdatePériode()
		{
			if (this.topFilterController != null)
			{
				this.topFilterController.UpdatePériode ();
			}
		}
		#endregion


		#region Options
		protected virtual void CreateTopOptions(FrameBox parent)
		{
		}

		protected virtual void OptionsChanged()
		{
			if (this.dataAccessor != null)
			{
				this.dataAccessor.UpdateAfterOptionsChanged ();
			}

			this.UpdateArrayContent ();
			this.FilterUpdateTopToolbar ();
			this.UpdateViewSettings ();
		}
		#endregion


		public virtual ControllerType ControllerType
		{
			//	Retourne le type du contrôleur.
			get
			{
				return Controllers.ControllerType.Unknown;
			}
		}

		public string ViewSettingsKey
		{
			//	Retourne la clé d'accès pour les données ViewSettingsList.
			get
			{
				return Présentations.GetViewSettingsKey (this.ControllerType);
			}
		}

		public string SearchKey
		{
			//	Retourne la clé d'accès pour les données SearchData.
			get
			{
				return Présentations.GetSearchSettingsKey (this.ControllerType);
			}
		}

		public string PermanentsKey
		{
			//	Retourne la clé d'accès pour les données AbstractPermanents.
			get
			{
				return Présentations.GetPermanentsSettingsKey (this.ControllerType);
			}
		}

		private void UpdatePanelsToolbar()
		{
			if (this.viewSettingsController != null && this.viewSettingsController.PanelsToolbarController != null && this.dataAccessor != null)
			{
				if (this.dataAccessor.SearchData != null)
				{
					this.viewSettingsController.PanelsToolbarController.SearchEnable = !this.dataAccessor.SearchData.IsEmpty && this.dataAccessor.SearchData.QuickFilter;
				}

				if (this.dataAccessor.FilterData != null)
				{
					this.viewSettingsController.PanelsToolbarController.FilterEnable = !this.dataAccessor.FilterData.IsEmpty;
				}
			}
		}


		#region Array
		public virtual int SelectedArrayLine
		{
			get
			{
				if (this.arrayController == null)
				{
					return -1;
				}
				else
				{
					return this.arrayController.SelectedRow;
				}
			}
			set
			{
				if (this.arrayController != null)
				{
					this.arrayController.SelectedRow = value;

					int first, count;
					this.arrayController.GetHilitedRows (out first, out count);
					this.arrayController.ShowRow (first, count);
				}
			}
		}

		private void CreateArray(FrameBox parent)
		{
			//	Crée le tableau principal avec son en-tête.
			if (Présentations.HasArray (this.ControllerType))
			{
				this.arrayController = new ArrayController (this);
				this.arrayController.CreateUI (parent, this.ArrayUpdateCellContent, this.ArrayColumnsWidthChanged, this.ArraySelectedRowChanged, this.ArrayRightClick);
				this.arrayController.Show = this.dataAccessor.Options == null || !this.dataAccessor.Options.ViewGraph;

				this.UpdateArray ();
			}
		}

		protected void UpdateArray()
		{
			if (this.arrayController != null)
			{
				this.arrayController.UpdateColumnsHeader (this.ArrayLineHeight);
			}

			this.UpdateGraph ();
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
			if (this.editorController != null)
			{
				this.editorController.UpdateEditorGeometry ();
			}
		}

		private void ArraySelectedRowChanged()
		{
			//	Appelé lorsque la ligne sélectionnée a changé.
			if (this.editorController == null)
			{
				return;
			}

			int row    = this.arrayController.SelectedRow;
			int column = this.arrayController.SelectedColumn;

			//	Si on sélectionne une ligne dans le tableau appartenant à l'écriture multiple en cours
			//	de modification, il faut simplement éditer la ligne cliquée.
			if (this.editorController != null &&
				this.dataAccessor.IsModification &&
				row >= this.dataAccessor.FirstEditedRow &&
				row < this.dataAccessor.FirstEditedRow+this.dataAccessor.CountEditedRow)
			{
				//?this.editorController.UpdateEditorContent ();
				//?this.editorController.EditorSelect (this.arrayController.SelectedColumnType, row - this.dataAccessor.FirstEditedRow);
				this.editorController.UpdateEditorContent (this.arrayController.SelectedColumnType, row - this.dataAccessor.FirstEditedRow);
				return;
			}

			if (this.editorController.Dirty)
			{
				if (this.editorController.HasError)
				{
					this.editorController.Dirty = false;
				}
				else
				{
					var entity = this.dataAccessor.GetEditionEntity (row);

					this.dataAccessor.UpdateEditionLine ();
					this.UpdateArrayContent ();
					this.editorController.Dirty = false;

					int adjustedRow = this.dataAccessor.GetEditionIndex (entity);

					if (row != adjustedRow)
					{
						row = adjustedRow;

						this.arrayController.SelectedRow = -1;
						this.arrayController.SetSelectedRow (row, column);
					}
				}
			}

			if (row != -1)
			{
				//	Dans le journal des écritures, l'insertion d'une ligne vide peut modifier le row !
				row = this.dataAccessor.StartModificationLine (row);
			}

			this.arrayController.ColorSelection = UIBuilder.SelectionColor;
			this.arrayController.ColorHilite = UIBuilder.HiliteColor;
			this.arrayController.SetHilitedRows (this.dataAccessor.FirstEditedRow, this.dataAccessor.CountEditedRow);

			if (this.editorController != null)
			{
				//?this.editorController.UpdateEditorContent ();
				//?this.editorController.EditorSelect (this.arrayController.SelectedColumnType, row - this.dataAccessor.FirstEditedRow);
				this.editorController.UpdateEditorContent (this.arrayController.SelectedColumnType, row - this.dataAccessor.FirstEditedRow);
				this.editorController.ShowSelection ();
			}
		}

		private void ArrayRightClick(Point pos)
		{
			//	Appelé lorsque un clic-droite dans la liste a été fait.
			var menu = this.ContextMenu;

			if (menu != null && menu.Items.Count != 0)
			{
				menu.Host = this.frameBox;
				menu.ShowAsContextMenu (this.frameBox, pos);
			}
		}

		protected virtual VMenu ContextMenu
		{
			get
			{
				return null;
			}
		}

		protected void PutContextMenuCommand(VMenu menu, Command cmd)
		{
			var item = new MenuItem
			{
				CommandObject = cmd,
				IconSize      = new Size (32, 32),
			};

			menu.Items.Add (item);
		}

		protected MenuItem PutContextMenuItem(VMenu menu, string icon, FormattedText text, bool enable = true)
		{
			var item = new MenuItem
			{
				IconUri       = UIBuilder.GetResourceIconUri (icon),
				FormattedText = text,
				Enable        = enable,
			};

			menu.Items.Add (item);

			return item;
		}

		protected void PutContextMenuSeparator(VMenu menu)
		{
			menu.Items.Add (new MenuSeparator ());
		}


		public void ClearHilite()
		{
			if (this.arrayController != null)
			{
				this.arrayController.SelectedRow = -1;
				this.arrayController.SetHilitedRows (-1, 0);
			}

			this.dataAccessor.StartDefaultLine ();
		}

		public void Update()
		{
			//	Met à jour l'ensemble du contrôleur.
			this.dataAccessor.UpdateAfterOptionsChanged ();
			this.BaseUpdateArrayContent ();

			if (this.editorController != null)
			{
				this.editorController.UpdateEditorContent ();
			}

			//?this.parentWindow.Text = this.mainWindowController.GetTitle (this.commandDocument);
		}

		public void UpdateEditionArray()
		{
			//	Met à jour le contenu du tableau pendant l'édition.
			if (this.arrayController != null)
			{
				this.arrayController.UpdateArrayContent (this.dataAccessor.Count, this.GetSearchArrayText, this.GetArrayBottomSeparator);
				this.arrayController.SetHilitedRows (this.dataAccessor.FirstEditedRow, this.dataAccessor.CountEditedRow);
			}
		}

		protected void UpdateArrayContent()
		{
			//	Met à jour le contenu du tableau, ainsi que des tableaux des fenêtre associées.
			this.BaseUpdateArrayContent ();

#if false
			foreach (var controller in this.mainWindowController.Controllers)
			{
				if (controller != this)
				{
					controller.dataAccessor.UpdateAfterOptionsChanged ();
					controller.BaseUpdateArrayContent ();
				}
			}
#endif
		}

		private void BaseUpdateArrayContent()
		{
			//	Met à jour le contenu du tableau.
			if (this.arrayController != null)
			{
				this.arrayController.UpdateArrayContent (this.dataAccessor.Count, this.GetSearchArrayText, this.GetArrayBottomSeparator);
			}

			this.UpdateGraph ();
			this.UpdatePanelsToolbar ();
		}

		private FormattedText GetSearchArrayText(int row, ColumnType columnType)
		{
			var text = this.GetArrayText (row, columnType);

			if (this.dataAccessor.SearchAny)
			{
				var result = this.dataAccessor.GetSearchResult (row, columnType);

				if (result == null)  // pas trouvé ?
				{
					if (!text.IsNullOrEmpty () && !text.ToString ().StartsWith (Compta.Widgets.StringArray.SpecialContentStart))
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


		#region Graph
		protected void CreateGraph(FrameBox parent)
		{
			if (Présentations.HasGraph (this.ControllerType))
			{
				this.graphWidget = new GraphWidget (this)
				{
					Parent     = parent,
					Options    = this.dataAccessor.Options.GraphOptions,
					Dock       = DockStyle.Fill,
					Visibility = this.dataAccessor.Options != null && this.dataAccessor.Options.ViewGraph,
				};

				ToolTip.Default.RegisterDynamicToolTipHost (this.graphWidget);  // pour voir les tooltips dynamiques

				this.UpdateGraph ();
			}
		}

		protected void UpdateGraph()
		{
			if (this.graphWidget != null)
			{
				this.graphWidget.Invalidate ();
			}
		}
		#endregion


		public bool ShowSearchPanel
		{
			get
			{
				if (this.dataAccessor != null && this.dataAccessor.ViewSettingsList != null && this.dataAccessor.ViewSettingsList.Selected != null)
				{
					return this.dataAccessor.ViewSettingsList.Selected.ShowSearchPanel;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (this.dataAccessor != null && this.dataAccessor.ViewSettingsList != null && this.dataAccessor.ViewSettingsList.Selected != null)
				{
					this.dataAccessor.ViewSettingsList.Selected.ShowSearchPanel = value;
				}
			}
		}

		public bool ShowFilterPanel
		{
			get
			{
				if (this.dataAccessor != null && this.dataAccessor.ViewSettingsList != null && this.dataAccessor.ViewSettingsList.Selected != null)
				{
					return this.dataAccessor.ViewSettingsList.Selected.ShowFilterPanel;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (this.dataAccessor != null && this.dataAccessor.ViewSettingsList != null && this.dataAccessor.ViewSettingsList.Selected != null)
				{
					this.dataAccessor.ViewSettingsList.Selected.ShowFilterPanel = value;
				}
			}
		}

		public bool ShowOptionsPanel
		{
			get
			{
				if (this.dataAccessor != null && this.dataAccessor.ViewSettingsList != null && this.dataAccessor.ViewSettingsList.Selected != null)
				{
					return this.dataAccessor.ViewSettingsList.Selected.ShowOptionsPanel;
				}
				else
				{
					return false;
				}
			}
			set
			{
				if (this.dataAccessor != null && this.dataAccessor.ViewSettingsList != null && this.dataAccessor.ViewSettingsList.Selected != null)
				{
					this.dataAccessor.ViewSettingsList.Selected.ShowOptionsPanel = value;
				}
			}
		}


		protected virtual void CreateEditor(FrameBox parent)
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

		protected void SetColumnParameters(ColumnType columnType, bool show, FormattedText description)
		{
			//	Détermine l'état montré/caché d'une colonne ainsi que son titre.
			//	Si la colonne est cachée, elle sera également invisible dans le menu
			//	combo du filtre.

			//	Une description vide (à spécifier *avant* ShowHideColumn) permet de cacher la colonne
			//	dans le menu combo du filtre !
			this.SetColumnDescription (columnType, show ? description : FormattedText.Null);
			this.ShowHideColumn (columnType, show);
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


		protected readonly ComptaApplication					app;
		protected readonly BusinessContext						businessContext;
		protected readonly ComptaEntity							compta;
		protected readonly ComptaPériodeEntity					période;
		protected readonly SettingsList							settingsList;
		protected readonly List<ColumnMapper>					columnMappers;

		protected MainWindowController							mainWindowController;

		protected AbstractDataAccessor							dataAccessor;

		protected TopSearchController							topSearchController;
		protected TopFilterController							topFilterController;
		protected AbstractOptionsController						optionsController;
		protected ViewSettingsController						viewSettingsController;
		protected ArrayController								arrayController;
		protected GraphWidget									graphWidget;
		protected AbstractEditorController						editorController;
		protected FrameBox										frameBox;
	}
}
