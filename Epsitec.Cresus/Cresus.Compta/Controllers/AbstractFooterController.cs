//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère le pied de page générique pour l'édition de la comptabilité.
	/// </summary>
	public abstract class AbstractFooterController
	{
		public AbstractFooterController(AbstractController controller)
		{
			this.controller = controller;

			this.comptaEntity    = this.controller.ComptaEntity;
			this.périodeEntity   = this.controller.PériodeEntity;
			this.columnMappers   = this.controller.ColumnMappers;
			this.dataAccessor    = this.controller.DataAccessor;
			this.arrayController = this.controller.ArrayController;
			this.businessContext = this.controller.BusinessContext;

			this.linesFrames      = new List<FrameBox> ();
			this.fieldControllers = new List<List<AbstractFieldController>> ();
		}


		public bool ShowInfoPanel
		{
			get
			{
				return this.bottomToolbarController.ShowPanel;
			}
			set
			{
				this.bottomToolbarController.ShowPanel = value;
				this.UpdateFooterInfo ();
			}
		}


		public virtual void CreateUI(FrameBox parent, System.Action updateArrayContentAction)
		{
			this.parent = parent;

			this.updateArrayContentAction = updateArrayContentAction;

			this.bottomToolbarController = new BottomToolbarController (this.businessContext);
			this.bottomToolbarController.CreateUI (parent);

			this.controller.SetCommandEnable (Res.Commands.Edit.Cancel, true);

			this.parent.Window.FocusedWidgetChanging += new Common.Support.EventHandler<FocusChangingEventArgs> (this.HandleFocusedWidgetChanging);
			this.parent.Window.FocusedWidgetChanged += new Common.Support.EventHandler<DependencyPropertyChangedEventArgs> (this.HandleFocusedWidgetChanged);
		}

		public void Dispose()
		{
			this.parent.Window.FocusedWidgetChanging -= new Common.Support.EventHandler<FocusChangingEventArgs> (this.HandleFocusedWidgetChanging);
			this.parent.Window.FocusedWidgetChanged -= new Common.Support.EventHandler<DependencyPropertyChangedEventArgs> (this.HandleFocusedWidgetChanged);
		}

		private void HandleFocusedWidgetChanging(object sender, FocusChangingEventArgs e)
		{
#if false
			//	Le focus va changer de widget. Il faut modifier le contenu du widget initial, en
			//	fonction de la validation. Par exemple, si on a tapé "20" dans une date, ce texte
			//	sera remplacé par "20.03.2012".
			if (e.OldFocus != null)
			{
				ColumnType columnType;
				int line;
				if (this.GetWidgetColumnLine (e.OldFocus.Name, out columnType, out line))
				{
					if (line < this.dataAccessor.EditionLine.Count && this.columnMappers.Where (x => x.Show && x.Column == columnType).Any ())
					{
						var field = e.OldFocus as AbstractTextField;
						System.Diagnostics.Debug.Assert (field != null);

						if (field is AutoCompleteTextField)
						{
							return;
						}

						var currentText = this.dataAccessor.GetEditionText (line, columnType);

						if (field.FormattedText != currentText)
						{
							this.controller.IgnoreChanged = true;
							field.FormattedText = currentText;
							field.HintText = null;
							this.controller.IgnoreChanged = false;
						}
					}
				}
			}
#endif
		}

		private void HandleFocusedWidgetChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
#if false
			//	Le focus a changé de widget. Il faut mettre en évidence la colonne correspondant au
			//	nouveau widget, dans l'en-tête du tableau principal.
			var window = sender as Window;

			if (window != null)
			{
				var focused = window.FocusedWidget;

				if (focused != null)
				{
					ColumnType columnType;
					int line;
					if (this.GetWidgetColumnLine (focused.Name, out columnType, out line))
					{
						this.arrayController.HiliteHeaderColumn (columnType);
						this.selectedColumn = columnType;

						if (this.selectedLine != line)
						{
							this.selectedLine = line;

							if (this.selectedLine != -1)
							{
								this.UpdateAfterSelectedLineChanged ();
							}
						}

						return;
					}
				}

				this.arrayController.HiliteHeaderColumn (ColumnType.None);
			}
#endif
		}

		protected virtual void UpdateAfterSelectedLineChanged()
		{
		}


		public virtual void UpdateToolbar()
		{
			this.controller.SetCommandEnable (Res.Commands.Edit.Accept,     this.dirty && !this.hasError);
			this.controller.SetCommandEnable (Res.Commands.Edit.Up,        !this.dirty && this.arrayController.SelectedRow != -1 && !this.dataAccessor.JustCreated && this.dataAccessor.IsEditionCreationEnable);
			this.controller.SetCommandEnable (Res.Commands.Edit.Down,      !this.dirty && this.arrayController.SelectedRow != -1 && !this.dataAccessor.JustCreated && this.dataAccessor.IsEditionCreationEnable);
			this.controller.SetCommandEnable (Res.Commands.Edit.Duplicate, !this.dirty && this.arrayController.SelectedRow != -1 && !this.dataAccessor.JustCreated && this.dataAccessor.IsEditionCreationEnable);
			this.controller.SetCommandEnable (Res.Commands.Edit.Delete,    !this.dirty && this.arrayController.SelectedRow != -1 && !this.dataAccessor.JustCreated && this.dataAccessor.IsEditionCreationEnable);

			if (this.arrayController.SelectedRow == -1 || this.dataAccessor.JustCreated)
			{
				this.bottomToolbarController.SetOperationDescription (this.GetOperationDescription (modify: false), hilited: false);
			}
			else
			{
				this.bottomToolbarController.SetOperationDescription (this.GetOperationDescription (modify: true), hilited: true);
			}
		}

		protected virtual FormattedText GetOperationDescription(bool modify)
		{
			return FormattedText.Empty;
		}


		protected void SetWidgetVisibility(ColumnType columnType, int line, bool visibility)
		{
			var container = this.GetContainer (columnType, line);

			if (container != null)
			{
				container.Visibility = visibility;
			}
		}

		private Widget GetContainer(ColumnType columnType, int line = 0)
		{
			var controller = this.GetFieldController (columnType, line);

			if (controller == null)
			{
				return null;
			}
			else
			{
				return controller.Container;
			}
		}

		protected void SetTextFieldReadonly(ColumnType columnType, int line, bool readOnly)
		{
			var controller = this.GetFieldController (columnType, line);

			if (controller != null)
			{
				controller.IsReadOnly = readOnly;
			}
		}

		protected AbstractFieldController GetFieldController(ColumnType columnType, int line = 0)
		{
			var column = this.GetMapperColumnRank (columnType);

			if (line   < 0 || line   >= this.fieldControllers.Count      ||
				column < 0 || column >= this.fieldControllers[line].Count)
			{
				return null;
			}
			else
			{
				return this.fieldControllers[line][column];
			}
		}


		private int GetMapperColumnRank(ColumnType columnType)
		{
			int i = 0;

			foreach (var mapper in this.columnMappers)
			{
				if (mapper.Column == columnType)
				{
					return i;
				}

				if (mapper.Show)
				{
					i++;
				}
			}

			return -1;
		}


		public virtual void AcceptAction()
		{
			if (!this.controller.GetCommandEnable (Res.Commands.Edit.Accept))
			{
				return;
			}

			this.dataAccessor.UpdateEditionLine ();
			this.EditionDataToWidgets (ignoreFocusField: false);

			this.controller.SearchUpdateAfterModification ();

			this.dirty = false;
			this.UpdateToolbar ();
			this.UpdateInsertionRow (forceUpdate: true);
			this.updateArrayContentAction ();

			using (this.controller.IgnoreChanges.Enter ())  // il ne faut surtout pas exécuter AbstractController.ArraySelectedRowChanged !
			{
				this.arrayController.SelectedRow = this.dataAccessor.FirstEditedRow;

				if (this.dataAccessor.JustCreated)
				{
					this.arrayController.ColorSelection = Color.FromAlphaColor (0.4, Color.FromHexa ("b3d7ff"));  // bleu pastel
				}
				else
				{
					this.arrayController.ColorSelection = Color.FromName ("Gold");
				}

				this.arrayController.SetHilitedRows (this.dataAccessor.FirstEditedRow, this.dataAccessor.CountEditedRow);
				this.dataAccessor.ResetCreationLine ();
			}

			if (this.controller.OptionsController != null)
			{
				this.controller.OptionsController.UpdateContent ();
			}

			this.UpdateFooterContent ();
			this.FooterSelect (this.arrayController.SelectedColumn);
			this.ShowSelection ();
			this.FooterSelect (0);
		}

		public void CancelAction()
		{
			if (this.controller.GetCommandEnable (Res.Commands.Edit.Cancel))
			{
				this.dirty = false;
				this.arrayController.SelectedRow = -1;
				this.dataAccessor.StartCreationLine ();
				this.arrayController.ColorSelection = Color.FromName ("Gold");
				this.arrayController.SetHilitedRows (this.dataAccessor.FirstEditedRow, this.dataAccessor.CountEditedRow);
				this.FooterSelect (0);
			}
		}

		public virtual void MoveAction(int direction)
		{
			//	Déplace la ou les lignes sélectionnées vers le haut ou vers le bas.
			if (this.dataAccessor.MoveEditionLine (direction))
			{
				int firstRow = this.dataAccessor.FirstEditedRow;
				int countRow = this.dataAccessor.CountEditedRow;

				this.updateArrayContentAction ();

				this.arrayController.SelectedRow = firstRow;
				this.arrayController.SetHilitedRows (firstRow, countRow);
				this.arrayController.ShowRow (firstRow, countRow);
			}
		}

		public virtual void DuplicateAction()
		{
		}

		public virtual void DeleteAction()
		{
		}

		public virtual void MultiInsertLineAction()
		{
		}

		public virtual void MultiDeleteLineAction()
		{
		}

		public virtual void MultiMoveLineAction(int direction)
		{
		}

		public virtual void MultiLineSwapAction()
		{
		}

		public virtual void MultiLineAutoAction()
		{
		}

		public virtual void InsertModèle(int n)
		{
		}


		public void ShowSelection()
		{
			//	Montre la sélection dans le tableau.
			if (this.arrayController.SelectedRow != -1)
			{
				int firstRow = this.arrayController.SelectedRow;
				int countRow = 1;

				int hilitedFirstRow, hilitedCountRow;
				this.arrayController.GetHilitedRows (out hilitedFirstRow, out hilitedCountRow);

				if (hilitedFirstRow != -1)
				{
					firstRow = hilitedFirstRow;
					countRow = hilitedCountRow;
				}

				this.arrayController.ShowRow (firstRow, countRow);
			}

#if false
			if (this.dataAccessor.JustCreated)
			{
				this.arrayController.ColorSelection = Color.FromBrightness (0.8);
			}
			else
			{
				this.arrayController.ColorSelection = Color.FromName ("Gold");
			}
#endif
		}


		private void EditionDataToWidgets(bool ignoreFocusField)
		{
			//	Effectue le transfert this.dataAccessor.EditionData -> widgets éditables.
			using (this.controller.IgnoreChanges.Enter ())
			{
				for (int line = 0; line < this.dataAccessor.EditionLine.Count; line++)
				{
					foreach (var mapper in this.columnMappers.Where (x => x.Show))
					{
						var controller = this.GetFieldController (mapper.Column, line);

						if (controller != null)
						{
							controller.EditionData = this.dataAccessor.GetEditionData (line, mapper.Column);

							//	Le widget en cours d'édition ne doit absolument pas être modifié.
							//	Par exemple, s'il contient "123" et qu'on a tapé "4", la chaîne actuellement contenue
							//	est "1234". Si on le mettait à jour, il contiendrait "1234.00", ce qui serait une
							//	catastrophe !
							if (!ignoreFocusField || !controller.HasFocus)
							{
								controller.EditionDataToWidget ();
							}
						}
					}
				}
			}
		}

#if false
		protected void WidgetToEditionData()
		{
			//	Effectue le transfert widgets éditables -> this.dataAccessor.EditionData.
			for (int line = 0; line < this.dataAccessor.EditionLine.Count; line++)
			{
				foreach (var mapper in this.columnMappers.Where (x => x.Show))
				{
					var controller = this.GetFieldController (mapper.Column, line);

					if (controller != null)
					{
						controller.ControllerToEditionData ();
					}
				}
			}
		}
#endif

		protected virtual void UpdateEditionWidgets()
		{
		}

		protected void FooterTextChanged()
		{
			//	Appelé lorsqu'un texte éditable a changé.
			if (this.controller.IgnoreChanges.IsZero)
			{
				this.dirty = true;
				//?this.WidgetToEditionData ();

				this.UpdateEditionWidgets ();
				this.EditionDataToWidgets (ignoreFocusField: true);  // nécessaire pour le feedback du travail de UpdateMultiWidgets !

				this.FooterValidate ();
				this.UpdateToolbar ();
				this.UpdateFooterInfo ();
				this.UpdateInsertionRow ();
			}
		}

		protected void HandleSetFocus(int line, ColumnType columnType)
		{
			this.arrayController.HiliteHeaderColumn (columnType);
			this.selectedColumn = columnType;

			if (this.selectedLine != line)
			{
				this.selectedLine = line;

				if (this.selectedLine != -1)
				{
					this.UpdateAfterSelectedLineChanged ();
				}
			}
		}

		public void FooterValidate()
		{
			this.hasError = false;

			for (int line = 0; line < this.linesFrames.Count; line++)
			{
				this.FooterValidate (line);
			}

			this.UpdateToolbar ();
			this.UpdateInsertionRow ();
		}

		private void FooterValidate(int line)
		{
			foreach (var mapper in this.columnMappers.Where (x => x.Show))
			{
				var controller = this.GetFieldController (mapper.Column, line);

				if (controller != null)
				{
					controller.Validate ();

					if (controller.EditionData != null && controller.EditionData.HasError)
					{
						this.hasError = true;
					}
				}
			}
		}


		public virtual void UpdateFooterContent()
		{
			this.UpdateEditionWidgets ();
			this.EditionDataToWidgets (ignoreFocusField: false);
			this.FooterValidate ();
			this.UpdateFooterInfo ();
		}

		protected virtual void UpdateFooterInfo()
		{
		}

		public virtual void UpdateFooterGeometry()
		{
			this.UpdateArrayColumns ();

			int columnCount = this.columnMappers.Where (x => x.Show).Count ();

			for (int line = 0; line < this.fieldControllers.Count; line++)
			{
				for (int column = 0; column < columnCount; column++)
				{
					this.fieldControllers[line][column].Box.PreferredWidth = this.arrayController.GetColumnsAbsoluteWidth (column) - (column == 0 ? 0 : 1);
				}
			}
		}

		protected virtual void UpdateArrayColumns()
		{
		}


		protected void FooterSelect(ColumnType columnType, int? line = null)
		{
			var column = this.GetMapperColumnRank (columnType);
			this.FooterSelect (column, line);
		}

		public void FooterSelect(int column, int? line = null)
		{
			if (column != -1)
			{
				if (line.HasValue)
				{
					line = System.Math.Max (line.Value, 0);
					line = System.Math.Min (line.Value, this.fieldControllers.Count-1);

					if (this.selectedLine != line.Value)
					{
						this.selectedLine = line.Value;
						this.SelectedLineChanged ();
					}
				}

				this.fieldControllers[this.selectedLine][column].SetFocus ();
			}
		}

		protected virtual void SelectedLineChanged()
		{
		}


		protected void UpdateInsertionRow(bool forceUpdate = false)
		{
			if (forceUpdate || this.selectedColumn == this.dataAccessor.ColumnForInsertionPoint)
			{
				this.arrayController.InsertionPointRow = this.dataAccessor.InsertionPointRow;

				if (this.arrayController.InsertionPointRow != -1)
				{
					this.arrayController.ShowRow (this.arrayController.InsertionPointRow, 1);
				}
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


#if false
		protected string GetWidgetName(ColumnType columnType, int line)
		{
			return string.Concat ("WidgwetComptatiliéDaniel.", line.ToString (System.Globalization.CultureInfo.InvariantCulture), ".", columnType.ToString ());
		}

		protected bool GetWidgetColumnLine(string name, out ColumnType columnType, out int line)
		{
			columnType = ColumnType.None;
			line = -1;

			if (string.IsNullOrEmpty (name) || !name.StartsWith ("WidgwetComptatiliéDaniel."))
			{
				return false;
			}

			var part = name.Split ('.');
			if (part.Length != 3)
			{
				return false;
			}

			if (!int.TryParse (part[1], out line))
			{
				return false;
			}

			if (!System.Enum.TryParse<ColumnType> (part[2], out columnType))
			{
				return false;
			}

			return true;
		}
#endif


		protected readonly AbstractController					controller;
		protected readonly ComptaEntity							comptaEntity;
		protected readonly ComptaPériodeEntity					périodeEntity;
		protected readonly List<ColumnMapper>					columnMappers;
		protected readonly AbstractDataAccessor					dataAccessor;
		protected readonly ArrayController						arrayController;
		protected readonly BusinessContext						businessContext;
		protected readonly List<FrameBox>						linesFrames;
		protected readonly List<List<AbstractFieldController>>	fieldControllers;

		protected FrameBox										parent;
		protected System.Action									updateArrayContentAction;
		protected BottomToolbarController						bottomToolbarController;
		protected ColumnType									selectedColumn;
		protected int											selectedLine;
		protected bool											dirty;
		protected bool											hasError;
	}
}
