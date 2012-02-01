//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

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
			this.columnMappers   = this.controller.ColumnMappers;
			this.dataAccessor    = this.controller.DataAccessor;
			this.arrayController = this.controller.ArrayController;
			this.businessContext = this.controller.BusinessContext;

			this.linesFrames      = new List<FrameBox> ();
			this.footerBoxes      = new List<List<FrameBox>> ();
			this.footerContainers = new List<List<FrameBox>> ();
			this.footerFields     = new List<List<AbstractTextField>> ();
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
				this.UpdateAfterShowInfoPanelChanged ();
			}
		}

		protected virtual void UpdateAfterShowInfoPanelChanged()
		{
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
			//	Le focus va changer de widget. Il faut modifier le contenu du widget initial, en
			//	fonction de la validation. Par exemple, si on a tapé "20" dans une date, ce texte
			//	sera remplacé par "20.03.2012".
			if (e.OldFocus != null)
			{
				ColumnType columnType;
				int line;
				if (this.GetWidgetColumnLine (e.OldFocus.Name, out columnType, out line))
				{
					if (line < this.dataAccessor.EditionData.Count && this.columnMappers.Where (x => x.Show && x.Column == columnType).Any ())
					{
						var field = e.OldFocus as AbstractTextField;
						System.Diagnostics.Debug.Assert (field != null);

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
		}

		private void HandleFocusedWidgetChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
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
		}

		protected virtual void UpdateAfterSelectedLineChanged()
		{
		}


		public virtual void FinalUpdate()
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


		public void SetWidgetVisibility(ColumnType columnType, int line, bool visibility)
		{
			//?this.GetTextField (columnType, line).IsReadOnly = !visibility;
			this.GetContainer (columnType, line).Visibility = visibility;
		}

		public Widget GetContainer(ColumnType columnType, int line = 0)
		{
			var column = this.GetMapperColumnRank (columnType);
			return this.GetContainer (column, line);
		}

		public Widget GetContainer(int column, int line = 0)
		{
			return this.footerContainers[line][column];
		}


		public AbstractTextField GetTextField(ColumnType columnType, int line = 0)
		{
			var column = this.GetMapperColumnRank (columnType);
			return this.GetTextField (column, line);
		}

		public AbstractTextField GetTextField(int column, int line = 0)
		{
			return this.footerFields[line][column];
		}


		protected int GetMapperColumnRank(ColumnType columnType)
		{
			var mapper = this.columnMappers.Where (x => x.Column == columnType).FirstOrDefault ();
			return this.columnMappers.IndexOf (mapper);
		}


		public virtual void AcceptAction()
		{
			if (!this.controller.GetCommandEnable (Res.Commands.Edit.Accept))
			{
				return;
			}

			this.dataAccessor.UpdateEditionData ();
			this.EditionDataToWidgets ();

			this.controller.SearchUpdateAfterModification ();

			this.dirty = false;
			this.UpdateToolbar ();
			this.UpdateInsertionRow ();
			this.updateArrayContentAction ();

			this.controller.IgnoreChanged = true;  // il ne faut surtout pas exécuter AbstractController.ArraySelectedRowChanged !

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
			this.dataAccessor.ResetCreationData ();
			
			this.controller.IgnoreChanged = false;

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
				this.dataAccessor.StartCreationData ();
				this.arrayController.ColorSelection = Color.FromName ("Gold");
				this.arrayController.SetHilitedRows (this.dataAccessor.FirstEditedRow, this.dataAccessor.CountEditedRow);
				this.FooterSelect (0);
			}
		}

		public virtual void MoveAction(int direction)
		{
		}

		public virtual void DuplicateAction()
		{
		}

		public virtual void DeleteAction()
		{
		}

		public virtual void InsertLineAction()
		{
		}

		public virtual void DeleteLineAction()
		{
		}

		public virtual void LineUpAction()
		{
		}

		public virtual void LineDownAction()
		{
		}

		public virtual void LineSwapAction()
		{
		}

		public virtual void LineAutoAction()
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


		protected void EditionDataToWidgets()
		{
			//	Effectue le transfert this.dataAccessor.EditionData -> widgets éditables.
			this.controller.IgnoreChanged = true;

			for (int line = 0; line < this.dataAccessor.EditionData.Count; line++)
			{
				foreach (var mapper in this.columnMappers.Where (x => x.Show))
				{
					var field = this.GetTextField (mapper.Column, line);
					field.FormattedText = this.dataAccessor.EditionData[line].GetText (mapper.Column);
				}
			}

			this.controller.IgnoreChanged = false;
		}

		protected void WidgetToEditionData()
		{
			//	Effectue le transfert widgets éditables -> this.dataAccessor.EditionData.
			for (int line = 0; line < this.dataAccessor.EditionData.Count; line++)
			{
				foreach (var mapper in this.columnMappers.Where (x => x.Show))
				{
					var field = this.GetTextField (mapper.Column, line);
					this.dataAccessor.EditionData[line].SetText (mapper.Column, field.FormattedText);
				}
			}
		}

		protected virtual void FooterTextChanged(AbstractTextField field)
		{
			//	Appelé lorsqu'un texte éditable a changé.
			if (!this.controller.IgnoreChanged)
			{
				this.dirty = true;
				this.WidgetToEditionData ();
				this.FooterValidate ();
				this.UpdateToolbar ();
				this.UpdateInsertionRow ();
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
			int column = 0;
			foreach (var mapper in this.columnMappers.Where (x => x.Show))
			{
				var field = this.footerFields[line][column];
				var text = field.FormattedText;

				if (line >= this.dataAccessor.EditionData.Count)
				{
					ToolTip.Default.SetToolTip (this.footerBoxes[line][column], mapper.Tooltip);
				}
				else
				{
					this.dataAccessor.Validate (line, mapper.Column);

					var error = this.dataAccessor.GetEditionError (line, mapper.Column);
					bool ok = error.IsNullOrEmpty;

					field.SetError (!ok);  // met un fond rouge en cas d'erreur

					if (field.TextDisplayMode == TextFieldDisplayMode.ActiveHint)
					{
						if (mapper.Column == ColumnType.Date)
						{
							var hint = this.dataAccessor.EditionData[line].GetText (mapper.Column);
							field.HintText = AbstractFooterController.AdjustHintDate (field.FormattedText, hint);
						}
					}

					if (ok)
					{
						ToolTip.Default.SetToolTip (this.footerBoxes[line][column], mapper.Tooltip);
					}
					else
					{
						ToolTip.Default.SetToolTip (this.footerBoxes[line][column], error);
						this.hasError = true;
					}
				}

				column++;
			}
		}

		private static string AdjustHintDate(FormattedText entered, FormattedText hint)
		{
			//	Ajuste le texte 'hint' en fonction du texte entré, pour une date.
			//
			//	entered = "5"     hint = "05.03.2012" out = "5.03.2012"
			//	entered = "5."    hint = "05.03.2012" out = "5.03.2012"
			//	entered = "5.3"   hint = "05.03.2012" out = "5.3.2012"
			//	entered = "5.3."  hint = "05.03.2012" out = "5.3.2012"
			//	entered = "5.3.2" hint = "05.03.2012" out = "5.3.2"
			//	entered = "5 3"   hint = "05.03.2012" out = "5 3.2012"

			if (entered.IsNullOrEmpty || hint.IsNullOrEmpty)
			{
				return hint.ToSimpleText ();
			}

			//	Décompose le texte 'entered', en mots et en séparateurs.
			var brut = entered.ToSimpleText ();

			var we = new List<string> ();
			var se = new List<string> ();

			int j = 0;
			bool n = true;
			for (int i = 0; i <= brut.Length; i++)
			{
				bool isDigit;
				
				if (i < brut.Length)
				{
					isDigit = brut[i] >= '0' && brut[i] <= '9';
				}
				else
				{
					isDigit = !n;
				}

				if (n && !isDigit)
				{
					we.Add (brut.Substring (j, i-j));
					j = i;
					n = false;
				}

				if (!n && isDigit)
				{
					se.Add (brut.Substring (j, i-j));
					j = i;
					n = true;
				}
			}

			//	Décompose le texte 'hint', en mots.
			var wh = hint.ToSimpleText ().Split ('.');

			//	
			int count = System.Math.Min (we.Count, wh.Length);
			for (int i = 0; i < count; i++)
			{
				if (!string.IsNullOrEmpty (we[i]))
				{
					wh[i] = we[i];
				}
			}

			//	Recompose la chaîne finale.
			var builder = new System.Text.StringBuilder ();

			for (int i = 0; i < wh.Length; i++)
			{
				builder.Append (wh[i]);

				if (i < se.Count)
				{
					builder.Append (se[i]);
				}
				else
				{
					builder.Append (".");
				}
			}

			return builder.ToString ();
		}


		public virtual void UpdateFooterContent()
		{
		}

		public virtual void UpdateFooterGeometry()
		{
			this.UpdateArrayColumns ();

			int columnCount = this.columnMappers.Where (x => x.Show).Count ();

			for (int line = 0; line < this.footerBoxes.Count; line++)
			{
				for (int column = 0; column < columnCount; column++)
				{
					this.footerBoxes[line][column].PreferredWidth = this.arrayController.GetColumnsAbsoluteWidth (column) - (column == 0 ? 0 : 1);
				}
			}
		}

		protected virtual void UpdateArrayColumns()
		{
		}


		protected void FooterSelect(ColumnType columnType)
		{
			var column = this.GetMapperColumnRank (columnType);
			this.FooterSelect (column);
		}

		public void FooterSelect(int column)
		{
			if (column != -1)
			{
				this.footerFields[this.selectedLine][column].SelectAll ();
				this.footerFields[this.selectedLine][column].Focus ();
			}
		}


		protected void UpdateInsertionRow()
		{
			this.arrayController.InsertionPointRow = this.dataAccessor.InsertionPointRow;

			if (this.arrayController.InsertionPointRow != -1)
			{
				this.arrayController.ShowRow (this.arrayController.InsertionPointRow, 1);
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


		protected readonly AbstractController				controller;
		protected readonly ComptaEntity						comptaEntity;
		protected readonly List<ColumnMapper>				columnMappers;
		protected readonly AbstractDataAccessor				dataAccessor;
		protected readonly ArrayController					arrayController;
		protected readonly BusinessContext					businessContext;
		protected readonly List<FrameBox>					linesFrames;
		protected readonly List<List<FrameBox>>				footerBoxes;
		protected readonly List<List<FrameBox>>				footerContainers;
		protected readonly List<List<AbstractTextField>>	footerFields;

		protected FrameBox									parent;
		protected System.Action								updateArrayContentAction;
		protected BottomToolbarController					bottomToolbarController;
		protected ColumnType								selectedColumn;
		protected int										selectedLine;
		protected bool										dirty;
		protected bool										hasError;
	}
}
