//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta
{
	/// <summary>
	/// Contrôleur générique pour la comptabilité, qui sert de base aux contrôleurs permettant l'édition
	/// (journal et plan comptable) ainsi qu'à ceux qui représentent des données 'readonly' (balance,
	/// bilan, etc.).
	/// </summary>
	public abstract class AbstractController
	{
		public AbstractController(CoreApp app, BusinessContext businessContext, ComptabilitéEntity comptabilitéEntity, List<AbstractController> controllers)
		{
			this.app                = app;
			this.businessContext    = businessContext;
			this.comptabilitéEntity = comptabilitéEntity;
			this.controllers        = controllers;

			this.app.CommandDispatcher.RegisterController (this);
		}


		public FrameBox CreateUI(FrameBox parent)
		{
			this.frameBox = new CatcherFrameBox
			{
				Parent        = parent,
				Dock          = DockStyle.Fill,
				CatcherAction = this.CatcherAction,
			};

			this.frameBox.KeyCodes.Add (KeyCode.Return);
			this.frameBox.KeyCodes.Add (KeyCode.NumericEnter);
			this.frameBox.KeyCodes.Add (KeyCode.Escape);

			this.CreateTopToolbar (this.frameBox);
			this.CreateOptions (this.frameBox);
			this.CreateArray (this.frameBox);
			this.CreateFooter (this.frameBox);
			this.FinalizeToolbars (this.frameBox);
			this.FinalizeOptions (this.frameBox);
			this.FinalizeFooter (this.frameBox);

			this.UpdateArrayContent ();

			if (this.footerController != null)
			{
				this.footerController.UpdateFooterGeometry ();
				this.footerController.UpdateFooterContent ();
			}

			this.FinalUpdate ();
			
			if (this.footerController != null)
			{
				this.footerController.FooterSelect (0);
			}

			return this.frameBox;
		}

		public void Dispose()
		{
			if (this.footerController != null)
			{
				this.footerController.Dispose ();
			}
		}

		protected virtual void FinalUpdate()
		{
			this.ShowHideToolbar ();

			if (this.footerController != null)
			{
				this.footerController.FinalUpdate ();
			}
		}

		private void CatcherAction(KeyCode keyCode, bool isShiftPressed)
		{
			if (this.footerController != null)
			{
				if (keyCode == KeyCode.Return      ||
					keyCode == KeyCode.NumericEnter)
				{
					this.footerController.AcceptAction ();
				}

				if (keyCode == KeyCode.Escape)
				{
					this.footerController.CancelAction ();
				}
			}
		}

		public virtual void UpdateData()
		{
		}


		#region Toolbar
		private void CreateTopToolbar(FrameBox parent)
		{
			this.topToolbarController = new TopToolbarController (this.businessContext, this.columnMappers);
			this.topToolbarController.CreateUI (parent, this.ImportAction, this.ShowHideToolbar, this.SearchStartAction, this.SearchNextAction);
		}

		private void FinalizeToolbars(FrameBox parent)
		{
			this.topToolbarController.FinalizeUI (parent);
		}

		protected virtual void ImportAction()
		{
		}

		private void ShowHideToolbar()
		{
			if (this.optionsController != null)
			{
				this.optionsController.TopOffset = this.topToolbarController.TopOffset;
			}
		}

		private void SearchStartAction()
		{
			//	Appelé lorsque le critère de recherche a été modifié, et qu'il faut commencer une recherche.
			this.dataAccessor.SearchUpdate (this.columnMappers.Select (x => x.Column), this.topToolbarController.SearchingData);
			this.BaseUpdateArrayContent ();
			this.SearchUpdateLocator (true);
			this.SearchUpdateTopToolbar ();
		}

		private void SearchNextAction(int direction)
		{
			this.dataAccessor.SearchMoveLocator (direction);
			this.SearchUpdateLocator (true);
		}

		private void SearchUpdateLocator(bool show)
		{
			int row;
			ColumnType columnType;
			this.dataAccessor.SearchLocatorInfo (out row, out columnType);

			int column = this.columnMappers.FindIndex (x => x.Column == columnType);
			this.arrayController.SetSearchLocator (row, column);

			if (show)
			{
				this.arrayController.ShowRow (row, 1);
			}
		}

		public void SearchUpdateAfterModification()
		{
			//	Appelé lorsque les données ont été modifiées, et qu'il faut mettre à jour les recherches.
			this.dataAccessor.SearchUpdate (this.columnMappers.Select (x => x.Column), this.topToolbarController.SearchingData);
			this.SearchUpdateLocator (false);
			this.SearchUpdateTopToolbar ();
		}

		public void SearchUpdateTopToolbar()
		{
			this.topToolbarController.SetSearchingCount (this.dataAccessor.Count, this.dataAccessor.SearchCount);
		}
		#endregion


		#region Options
		protected virtual void CreateOptions(FrameBox parent)
		{
		}

		protected virtual void FinalizeOptions(FrameBox parent)
		{
		}

		protected virtual void OptinsChanged()
		{
			this.dataAccessor.UpdateAfterOptionsChanged ();
			this.UpdateArrayContent ();
		}
		#endregion


		#region Array
		private void CreateArray(FrameBox parent)
		{
			//	Crée le tableau principal avec son en-tête.
			this.arrayController = new ArrayController ();
			this.arrayController.CreateUI (parent, this.ArrayUpdateCellContent, this.ArrayColumnsWidthChanged, this.ArraySelectedRowChanged);

			this.UpdateArray ();
		}

		protected void UpdateArray()
		{
			var descriptions   = this.columnMappers.Select (x => x.Description);
			var relativeWidths = this.columnMappers.Select (x => x.RelativeWidth);
			var alignments     = this.columnMappers.Select (x => x.Alignment);

			this.arrayController.UpdateColumnsHeader (descriptions, relativeWidths, alignments);
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
			if (this.arrayController.IgnoreChanged)
			{
				return;
			}

			int row = this.arrayController.SelectedRow;

			if (row == -1)
			{
				this.dataAccessor.StartCreationData ();
			}
			else
			{
				this.dataAccessor.StartModificationData (row);
			}

			this.arrayController.ColorSelection = Color.FromName ("Gold");
			this.arrayController.SetHilitedRows (this.dataAccessor.FirstEditedRow, this.dataAccessor.CountEditedRow);

			if (this.footerController != null)
			{
				this.footerController.UpdateFooterContent ();
				this.footerController.FooterSelect (this.arrayController.SelectedColumn);
				this.footerController.ShowSelection ();
			}
		}

		protected void UpdateArrayContent()
		{
			//	Met à jour le contenu du tableau.
			this.BaseUpdateArrayContent ();

			foreach (var controller in this.controllers)
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
			this.arrayController.UpdateArrayContent (this.dataAccessor.Count, this.GetSearchArrayText, this.GetArrayBottomSeparator);
		}

		private FormattedText GetSearchArrayText(int row, int column)
		{
			var text = this.GetArrayText (row, column);

			if (this.dataAccessor.SearchAny)
			{
				var mapper = this.columnMappers[column];
				var result = this.dataAccessor.GetSearchResult (row, mapper.Column);

				if (result == null)  // pas trouvé ?
				{
					if (!text.IsNullOrEmpty && !text.ToString ().StartsWith (StringArray.SpecialContentStart))
					{
						text = text.ApplyFontColor (SearchResult.TextOutsideSearch);  // gris
					}
				}
				else  // est-ce que le texte de recherche a été trouvé dans cette cellule ?
				{
					text = result.HilitedText;  // utilise le texte marqué
				}
			}

			return text;
		}

		protected virtual FormattedText GetArrayText(int row, int column)
		{
			//	Retourne le texte contenu dans une cellule.
			var mapper = this.columnMappers[column];
			return this.dataAccessor.GetText (row, mapper.Column);
		}

		protected bool GetArrayBottomSeparator(int row)
		{
			return this.dataAccessor.HasBottomSeparator (row);
		}


		protected void HiliteHeaderColumn(int column)
		{
			//	Met en évidence une en-tête de colonne à choix.
			this.arrayController.HiliteHeaderColumn (column);
		}
		#endregion


		#region Footer
		protected virtual void CreateFooter(FrameBox parent)
		{
		}

		protected virtual void FinalizeFooter(FrameBox parent)
		{
		}
		#endregion


		protected void InitializeColumnMapper()
		{
			this.columnMappers = new List<ColumnMapper> ();

			foreach (var mapper in this.ColumnMappers)
			{
				this.columnMappers.Add (mapper);
			}
		}

		protected virtual IEnumerable<ColumnMapper> ColumnMappers
		{
			get
			{
				return null;
			}
		}


		protected readonly CoreApp								app;
		protected readonly BusinessContext						businessContext;
		protected readonly ComptabilitéEntity					comptabilitéEntity;
		protected readonly List<AbstractController>				controllers;

		protected AbstractDataAccessor							dataAccessor;
		protected List<ColumnMapper>							columnMappers;

		protected TopToolbarController							topToolbarController;
		protected AbstractOptionsController						optionsController;
		protected ArrayController								arrayController;
		protected AbstractFooterController						footerController;
		protected CatcherFrameBox								frameBox;
	}
}
