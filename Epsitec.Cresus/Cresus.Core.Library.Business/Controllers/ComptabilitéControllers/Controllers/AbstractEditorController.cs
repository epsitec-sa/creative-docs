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
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Business.Finance.Comptabilité;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComptabilitéControllers
{
	/// <summary>
	/// Ce contrôleur gère un éditeur générique pour la comptabilité.
	/// </summary>
	public abstract class AbstractEditorController<ColumnType, Entity, Options>
		where Entity : class
		where Options : class
	{
		public AbstractEditorController(TileContainer tileContainer, ComptabilitéEntity comptabilitéEntity)
		{
			this.tileContainer = tileContainer;
			this.comptabilitéEntity = comptabilitéEntity;
		}


		public FrameBox CreateUI(FrameBox parent)
		{
			this.frameBox = new FrameBox ()
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
				Margins = new Margins (0, 10, 1, 1),
			};

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

		protected virtual void FinalUpdate()
		{
			this.ShowHideToolbar ();

			if (this.footerController != null)
			{
				this.footerController.FinalUpdate ();
			}
		}


		#region Toolbar
		private void CreateTopToolbar(FrameBox parent)
		{
			this.topToolbarController = new TopToolbarController (this.tileContainer);
			this.topToolbarController.CreateUI (parent, this.ImportAction, this.ShowHideToolbar);
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
			this.dataAccessor.UpdateSortedList ();
			this.UpdateArrayContent ();
		}
		#endregion


		#region Array
		private void CreateArray(FrameBox parent)
		{
			//	Crée le tableau principal avec son en-tête.
			this.arrayController = new ArrayController<Entity> ();

			var descriptions   = this.columnMappers.Select (x => x.Description);
			var relativeWidths = this.columnMappers.Select (x => x.RelativeWidth);
			var alignments     = this.columnMappers.Select (x => x.Alignment);

			this.arrayController.CreateUI (parent, descriptions, relativeWidths, alignments, this.ArrayUpdateCellContent, this.ArrayColumnsWidthChanged, this.ArraySelectedRowChanged);
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
			var entity = this.dataAccessor.ElementAt (this.arrayController.SelectedRow);

			if (this.footerController != null && this.footerController.Dirty && !this.footerController.HasError)
			{
				this.footerController.AcceptAction (true, this.arrayController.SelectedEntity);
			}

			this.arrayController.IgnoreChanged = true;

			if (this.footerController != null)
			{
				this.footerController.Dirty = false;
				this.footerController.JustCreate = false;
			}

			this.arrayController.SelectedEntity = entity;
			this.arrayController.SelectedRow = this.dataAccessor.IndexOf (entity);

			if (this.footerController != null)
			{
				this.footerController.UpdateFooterContent ();
				this.footerController.FooterSelect (this.arrayController.SelectedColumn);
			}

			this.arrayController.IgnoreChanged = false;

			this.ShowSelection ();
		}

		protected void UpdateArrayContent()
		{
			//	Met à jour le contenu du tableau.
			this.arrayController.UpdateArrayContent (this.dataAccessor.Count, this.GetArrayText);
		}

		protected virtual FormattedText GetArrayText(int row, int column)
		{
			//	Retourne le texte contenu dans une cellule.
			var mapper = this.columnMappers[column];
			return this.dataAccessor.GetText (row, mapper.Column);
		}

		private void ShowSelection()
		{
			//	Montre la sélection dans le tableau.
			if (this.arrayController.SelectedRow != -1)
			{
				this.arrayController.ShowSelection ();
			}
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


		protected readonly TileContainer								tileContainer;
		protected readonly ComptabilitéEntity							comptabilitéEntity;

		protected AbstractDataAccessor<ColumnType, Entity>				dataAccessor;
		protected List<AbstractColumnMapper<ColumnType>>				columnMappers;

		protected TopToolbarController									topToolbarController;
		protected AbstractOptionsController<Entity>						optionsController;
		protected ArrayController<Entity>								arrayController;
		protected AbstractFooterController<ColumnType, Entity, Options>	footerController;
		protected FrameBox												frameBox;
	}
}
