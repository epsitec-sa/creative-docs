//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComptabilitéControllers
{
	/// <summary>
	/// Ce contrôleur gère le pied de page générique pour l'édition de la comptabilité.
	/// </summary>
	public abstract class AbstractFooterController<ColumnType, Entity>
		where Entity : class
	{
		public AbstractFooterController(TileContainer tileContainer, ComptabilitéEntity comptabilitéEntity, AbstractDataAccessor<ColumnType, Entity> dataAccessor, List<AbstractColumnMapper<ColumnType>> columnMappers, ArrayController<Entity> arrayController)
		{
			this.tileContainer      = tileContainer;
			this.comptabilitéEntity = comptabilitéEntity;
			this.dataAccessor       = dataAccessor;
			this.columnMappers      = columnMappers;
			this.arrayController    = arrayController;

			this.footerContainers = new List<Widget> ();
			this.footerFields = new List<AbstractTextField> ();
			this.footerValidatedTexts = new List<FormattedText> ();
		}


		public virtual void CreateUI(FrameBox parent, System.Action updateArrayContentAction)
		{
			this.updateArrayContentAction = updateArrayContentAction;

			this.bottomToolbarController = new BottomToolbarController (this.tileContainer);
			this.bottomToolbarController.CreateUI (parent, this.AcceptAction, this.CancelAction, null, null);
			this.bottomToolbarController.CancelEnable = true;

			this.tileContainer.Window.FocusedWidgetChanging += new Common.Support.EventHandler<FocusChangingEventArgs> (this.HandleFocusedWidgetChanging);
			this.tileContainer.Window.FocusedWidgetChanged += new Common.Support.EventHandler<DependencyPropertyChangedEventArgs> (this.HandleFocusedWidgetChanged);
		}

		private void HandleFocusedWidgetChanging(object sender, FocusChangingEventArgs e)
		{
#if true
			if (this.footerFields.Contains (e.OldFocus))
			{
				int column = e.OldFocus.Index;
				var mapper = this.columnMappers[column];
				var field = this.GetTextField (column);

				var currentText = this.footerValidatedTexts[column];
				if (field.FormattedText != currentText)
				{
					this.arrayController.IgnoreChanged = true;
					field.FormattedText = currentText;
					this.arrayController.IgnoreChanged = false;
				}
			}
#endif
		}

		private void HandleFocusedWidgetChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var field = sender as Widget;

			if (field != null && field.Window != null)
			{
				var focused = field.Window.FocusedWidget;

				if (this.footerFields.Contains (focused))
				{
					this.arrayController.HiliteHeaderColumn (focused.Index);
				}
				else
				{
					this.arrayController.HiliteHeaderColumn (-1);
				}
			}
		}


		public void FinalizeUI(FrameBox parent)
		{
			this.bottomToolbarController.FinalizeUI (parent);
		}

		public virtual void FinalUpdate()
		{
		}

		public void UpdateToolbar()
		{
			this.bottomToolbarController.AcceptEnable    =  this.dirty && !this.hasError;
			this.bottomToolbarController.DuplicateEnable = !this.dirty && this.arrayController.SelectedRow != -1 && !this.JustCreate;
			this.bottomToolbarController.DeleteEnable    = !this.dirty && this.arrayController.SelectedRow != -1 && !this.JustCreate;

			if (this.arrayController.SelectedRow == -1 || this.JustCreate)
			{
				this.bottomToolbarController.SetOperationDescription (this.GetOperationDescription (false), false);
			}
			else
			{
				this.bottomToolbarController.SetOperationDescription (this.GetOperationDescription (true), true);
			}
		}

		protected virtual FormattedText GetOperationDescription(bool modify)
		{
			return FormattedText.Empty;
		}

		public AbstractTextField GetTextField(int column)
		{
			return this.footerFields[column];
		}


		protected virtual void AcceptAction()
		{
			this.AcceptAction (false, this.arrayController.SelectedEntity);
		}

		public void AcceptAction(bool silent, Entity entity)
		{
			if (entity == null || this.JustCreate)
			{
				this.CreateEntity (silent);

				this.JustCreate = true;
				this.UpdateToolbar ();
				this.FooterValidate ();
			}
			else
			{
				this.UpdateEntity (silent, entity);

				this.JustCreate = false;
				this.UpdateToolbar ();
				this.FooterValidate ();
			}
		}

		protected virtual void CreateEntity(bool silent)
		{
		}

		protected void UpdateEntity(bool silent, Entity entity)
		{
			int row = this.dataAccessor.IndexOf (entity);
			int columnCount = this.columnMappers.Count;

			for (int column = 0; column < columnCount; column++)
			{
				var mapper = this.columnMappers[column];
				var field = this.GetTextField (column);

				this.dataAccessor.SetText (row, mapper.Column, field.FormattedText);
			}

			this.FinalUpdateEntities ();

			if (silent)
			{
				this.dataAccessor.UpdateSortedList ();
				this.updateArrayContentAction ();
			}
			else
			{
				this.dataAccessor.UpdateSortedList ();
				row = this.dataAccessor.IndexOf (entity);

				this.dirty = false;
				this.UpdateToolbar ();
				this.updateArrayContentAction ();

				this.arrayController.IgnoreChanged = true;
				this.arrayController.SelectedRow = row;
				this.arrayController.IgnoreChanged = false;

				this.FooterSelect (0);
			}
		}

		protected virtual void FinalUpdateEntities()
		{
		}

		private void CancelAction()
		{
			this.dirty = false;
			this.arrayController.SelectedRow = -1;
			this.arrayController.SelectedEntity = null;
			this.JustCreate = false;
			this.FooterSelect (0);
		}


		protected virtual void FooterTextChanged(int column, FormattedText text)
		{
			//	Appelé lorsqu'un texte éditable a changé.
			if (!this.arrayController.IgnoreChanged)
			{
				this.dirty = true;
				this.UpdateToolbar ();
			}

			this.FooterValidate ();
		}

		public void FooterValidate()
		{
			this.hasError = false;

			int columnCount = this.columnMappers.Count;

			for (int column = 0; column < columnCount; column++)
			{
				var mapper = this.columnMappers[column];

				if (mapper.Validate == null)
				{
					ToolTip.Default.SetToolTip (this.footerContainers[column], mapper.Tooltip);
				}
				else
				{
					var text = this.footerFields[column].FormattedText;
					var error = mapper.Validate (mapper.Column, ref text);
					this.footerValidatedTexts[column] = text;
					bool ok = error.IsNullOrEmpty;

					this.footerFields[column].SetError (!ok);

					if (ok)
					{
						ToolTip.Default.SetToolTip (this.footerContainers[column], mapper.Tooltip);
					}
					else
					{
						ToolTip.Default.SetToolTip (this.footerContainers[column], error);
						this.hasError = true;
					}
				}
			}

			this.UpdateToolbar ();
		}

		public virtual void UpdateFooterContent()
		{
		}

		public virtual void UpdateFooterGeometry()
		{
			int columnCount = this.columnMappers.Count;

			for (int column = 0; column < columnCount; column++)
			{
				this.footerContainers[column].PreferredWidth = this.arrayController.GetColumnsAbsoluteWidth (column) - (column == 0 ? 0 : 1);
			}
		}

		public void FooterSelect(int column)
		{
			if (column != -1)
			{
				this.footerFields[column].SelectAll ();
				this.footerFields[column].Focus ();
			}
		}


		public bool Dirty
		{
			get
			{
				return this.dirty;
			}
			set
			{
				this.dirty = value;
			}
		}

		public bool HasError
		{
			get
			{
				return this.hasError;
			}
			set
			{
				this.hasError = value;
			}
		}

		public bool JustCreate
		{
			get
			{
				return this.justCreate;
			}
			set
			{
				this.justCreate = value;
			}
		}


		protected readonly TileContainer								tileContainer;
		protected readonly ComptabilitéEntity							comptabilitéEntity;
		protected AbstractDataAccessor<ColumnType, Entity>				dataAccessor;
		protected List<AbstractColumnMapper<ColumnType>>				columnMappers;
		protected ArrayController<Entity>								arrayController;
		protected readonly List<Widget>									footerContainers;
		protected readonly List<AbstractTextField>						footerFields;
		protected readonly List<FormattedText>							footerValidatedTexts;

		protected System.Action											updateArrayContentAction;
		protected BottomToolbarController								bottomToolbarController;
		protected bool													dirty;
		protected bool													hasError;
		protected bool													justCreate;
	}
}
