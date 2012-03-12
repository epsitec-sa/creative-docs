//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Settings.Data;
using Epsitec.Cresus.Compta.Fields.Controllers;
using Epsitec.Cresus.Compta.Assistants.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère le pied de page générique pour l'édition de la comptabilité.
	/// </summary>
	public abstract class AbstractEditorController
	{
		public AbstractEditorController(AbstractController controller)
		{
			this.controller = controller;

			this.compta          = this.controller.ComptaEntity;
			this.période         = this.controller.PériodeEntity;
			this.columnMappers   = this.controller.ColumnMappers;
			this.dataAccessor    = this.controller.DataAccessor;
			this.arrayController = this.controller.ArrayController;
			this.businessContext = this.controller.BusinessContext;
			this.settingsList    = this.controller.SettingsList;

			this.linesFrames      = new List<FrameBox> ();
			this.fieldControllers = new List<List<AbstractFieldController>> ();

			this.maxLines = this.controller.SettingsList.GetInt (SettingsType.EcritureMultiEditionLineCount).GetValueOrDefault (5);
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
				this.UpdateEditorInfo ();
			}
		}


		public virtual void CreateUI(FrameBox parent, System.Action updateArrayContentAction)
		{
			this.parent = parent;

			this.updateArrayContentAction = updateArrayContentAction;

			if (this.bottomToolbarController == null)
			{
				this.bottomToolbarController = new BottomToolbarController (this.businessContext);
				this.bottomToolbarController.CreateUI (parent);
			}

			this.UpdateToolbar ();
		}

		public void Dispose()
		{
		}


		protected virtual void UpdateAfterSelectedLineChanged()
		{
		}


		public virtual void UpdateToolbar()
		{
			if (this.controller.HasCreateCommand)
			{
				this.controller.SetCommandEnable (Res.Commands.Edit.Create, !this.dirty && this.dataAccessor.IsEditionCreationEnable);
				this.controller.SetCommandEnable (Res.Commands.Edit.Accept, this.dirty && !this.hasError);
				this.controller.SetCommandEnable (Res.Commands.Edit.Cancel, this.dataAccessor.IsActive);

				if (this.dataAccessor.IsActive)
				{
					this.editorFrameBox.Enable = true;
					this.editorFrameBox.BackColor = this.dataAccessor.IsCreation ? UIBuilder.CreationBackColor : UIBuilder.ModificationBackColor;

					this.editorForegroundFrameBox.Visibility = false;
				}
				else
				{
					this.editorFrameBox.Enable = false;
					this.editorFrameBox.BackColor = Color.Empty;

					this.editorForegroundFrameBox.Visibility = true;
				}
			}
			else
			{
				this.controller.SetCommandEnable (Res.Commands.Edit.Create, false);
				this.controller.SetCommandEnable (Res.Commands.Edit.Accept, this.dirty && !this.hasError);
				this.controller.SetCommandEnable (Res.Commands.Edit.Cancel, true);

				this.editorFrameBox.BackColor = this.dataAccessor.IsCreation ? UIBuilder.CreationBackColor : UIBuilder.ModificationBackColor;
				this.editorForegroundFrameBox.Visibility = false;
			}

			this.controller.SetCommandEnable (Res.Commands.Edit.Up,        !this.dirty && this.arrayController.SelectedRow != -1 && !this.dataAccessor.JustCreated && this.dataAccessor.IsEditionCreationEnable);
			this.controller.SetCommandEnable (Res.Commands.Edit.Down,      !this.dirty && this.arrayController.SelectedRow != -1 && !this.dataAccessor.JustCreated && this.dataAccessor.IsEditionCreationEnable);
			this.controller.SetCommandEnable (Res.Commands.Edit.Duplicate, !this.dirty && this.arrayController.SelectedRow != -1 && !this.dataAccessor.JustCreated && this.dataAccessor.IsEditionCreationEnable);
			this.controller.SetCommandEnable (Res.Commands.Edit.Delete,    !this.dirty && this.arrayController.SelectedRow != -1 && !this.dataAccessor.JustCreated && this.dataAccessor.IsEditionCreationEnable);

			if (!this.dataAccessor.IsCreation && !this.dataAccessor.IsModification && !this.dataAccessor.JustCreated)
			{
				this.bottomToolbarController.SetOperationDescription (null, hilited: false);
			}
			else if (this.arrayController.SelectedRow == -1 || this.dataAccessor.JustCreated)
			{
				this.bottomToolbarController.SetOperationDescription (this.GetOperationDescription (modify: false), hilited: false);
			}
			else
			{
				this.bottomToolbarController.SetOperationDescription (this.GetOperationDescription (modify: true), hilited: true);
			}

			this.bottomToolbarController.SetEditionDescription (this.EditionDescription);
		}

		protected virtual FormattedText GetOperationDescription(bool modify)
		{
			return FormattedText.Empty;
		}

		protected virtual FormattedText EditionDescription
		{
			get
			{
				return FormattedText.Empty;
			}
		}


		protected bool GetWidgetVisibility(ColumnType columnType, int line)
		{
			var container = this.GetContainer (columnType, line);

			if (container == null)
			{
				return false;
			}
			else
			{
				return container.Visibility;
			}
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

		private bool GetTextFieldReadonly(ColumnType columnType, int line)
		{
			var controller = this.GetFieldController (columnType, line);

			if (controller == null)
			{
				return false;
			}
			else
			{
				return controller.IsReadOnly;
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

			foreach (var mapper in this.columnMappers.Where (x => x.Show && x.Edition))
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

#if false
		private ColumnType GetMapperColumnType(int column)
		{
			var list = this.columnMappers.Where (x => x.Show && x.Edition).ToList ();

			if (column >= 0 && column < list.Count)
			{
				return list[column].Column;
			}
			else
			{
				return ColumnType.Nom;
			}
		}
#endif


		public virtual void CreateAction()
		{
			if (this.controller.GetCommandEnable (Res.Commands.Edit.Create))
			{
				this.dirty = false;
				this.arrayController.SelectedRow = -1;
				this.dataAccessor.StartCreationLine ();
				this.UpdateToolbar ();
				this.arrayController.ColorSelection = UIBuilder.SelectionColor;
				this.arrayController.SetHilitedRows (this.dataAccessor.FirstEditedRow, this.dataAccessor.CountEditedRow);
				this.UpdateInsertionRow (forceUpdate: true);
				this.UpdateEditorContent ();
				this.EditorSelect (0, 0);
			}
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
			this.updateArrayContentAction ();
			this.UpdateInsertionRow (forceUpdate: true);

			using (this.controller.IgnoreChanges.Enter ())  // il ne faut surtout pas exécuter AbstractController.ArraySelectedRowChanged !
			{
				this.arrayController.SelectedRow = this.dataAccessor.FirstEditedRow;

				if (this.dataAccessor.JustCreated)
				{
					this.arrayController.ColorSelection = UIBuilder.JustCreatedColor;
				}
				else
				{
					this.arrayController.ColorSelection = UIBuilder.SelectionColor;
				}

				this.arrayController.SetHilitedRows (this.dataAccessor.FirstEditedRow, this.dataAccessor.CountEditedRow);
				this.dataAccessor.ResetCreationLine ();
			}

			if (this.controller.OptionsController != null)
			{
				this.controller.OptionsController.UpdateContent ();
			}

			this.UpdateEditorContent ();
			this.EditorSelect (this.arrayController.SelectedColumn);
			this.ShowSelection ();
			this.EditorSelect (0);
		}

		public void CancelAction()
		{
			if (this.controller.GetCommandEnable (Res.Commands.Edit.Cancel))
			{
				this.dirty = false;
				this.arrayController.SelectedRow = -1;
				this.dataAccessor.StartDefaultLine ();
				this.UpdateToolbar ();
				this.arrayController.ColorSelection = UIBuilder.SelectionColor;
				this.arrayController.SetHilitedRows (this.dataAccessor.FirstEditedRow, this.dataAccessor.CountEditedRow);
				this.UpdateEditorContent ();
				this.EditorSelect (0);
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
			this.dirty = true;

			this.duplicate = true;
			this.arrayController.SelectedRow = -1;
			this.duplicate = false;

			this.dataAccessor.DuplicateModificationLine ();
			this.arrayController.ColorSelection = UIBuilder.SelectionColor;
			this.arrayController.SetHilitedRows (this.dataAccessor.FirstEditedRow, this.dataAccessor.CountEditedRow);

			this.UpdateInsertionRow (forceUpdate: true);
		}

		public virtual void DeleteAction()
		{
			var error = this.dataAccessor.GetRemoveModificationLineError ();
			if (!error.IsNullOrEmpty)
			{
				this.controller.MainWindowController.ErrorDialog (error);
				return;
			}

			if (this.controller.SettingsList.GetBool (SettingsType.GlobalRemoveConfirmation))
			{
				var result = this.controller.MainWindowController.QuestionDialog (this.dataAccessor.GetRemoveModificationLineQuestion ());
				if (result != Common.Dialogs.DialogResult.Yes)
				{
					return;
				}
			}

			this.dataAccessor.RemoveModificationLine ();
			this.dirty = false;

			int firstRow = this.dataAccessor.FirstEditedRow;
			int countRow = this.dataAccessor.CountEditedRow;

			this.updateArrayContentAction ();

			this.controller.SearchStartAction ();

			this.arrayController.SelectedRow = -1;  // pour forcer une mise à jour complète
			this.arrayController.SelectedRow = firstRow;
			this.arrayController.SetHilitedRows (firstRow, countRow);
			this.arrayController.ShowRow (firstRow, countRow);
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
		}


		private void EditionDataToWidgets(bool ignoreFocusField)
		{
			//	Effectue le transfert this.dataAccessor.EditionData -> widgets éditables.
			using (this.controller.IgnoreChanges.Enter ())
			{
				for (int line = 0; line < this.dataAccessor.EditionLine.Count; line++)
				{
					foreach (var mapper in this.columnMappers.Where (x => x.Show && x.Edition))
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

		protected virtual void UpdateEditionWidgets()
		{
		}

		protected void EditorTextChanged()
		{
			//	Appelé lorsqu'un texte éditable a changé.
			if (this.controller.IgnoreChanges.IsZero)
			{
				this.dirty = true;
				//?this.WidgetToEditionData ();

				this.UpdateEditionWidgets ();
				this.EditionDataToWidgets (ignoreFocusField: true);  // nécessaire pour le feedback du travail de UpdateMultiWidgets !

				this.EditorValidate ();
				this.UpdateToolbar ();
				this.UpdateEditorInfo ();
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

		public void EditorValidate()
		{
			this.hasError = false;

			for (int line = 0; line < this.linesFrames.Count; line++)
			{
				this.EditorValidate (line);
			}

			this.UpdateToolbar ();
			this.UpdateInsertionRow ();
		}

		private void EditorValidate(int line)
		{
			foreach (var mapper in this.columnMappers.Where (x => x.Show && x.Edition))
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


		public virtual void UpdateEditorContent()
		{
			this.UpdateEditionWidgets ();
			this.EditionDataToWidgets (ignoreFocusField: false);
			this.EditorValidate ();
			this.UpdateEditorInfo ();
		}

		protected virtual void UpdateEditorInfo()
		{
		}

		public virtual void UpdateEditorGeometry()
		{
			if (!this.controller.HasRightEditor)
			{
				//?this.UpdateArrayColumns ();  // provoque un Stack overflow !!!

#if false
				int columnCount = this.columnMappers.Where (x => x.Show && x.Edition).Count ();

				for (int line = 0; line < this.fieldControllers.Count; line++)
				{
					for (int column = 0; column < columnCount; column++)
					{
						if (column < this.fieldControllers[line].Count)
						{
							this.fieldControllers[line][column].Box.PreferredWidth = this.arrayController.GetColumnsAbsoluteWidth (column) - (column == 0 ? 0 : 1);
						}
					}
				}
#else
				if (this.assistantController == null)
				{
					for (int line = 0; line < this.fieldControllers.Count; line++)
					{
						foreach (var mapper in this.columnMappers.Where (x => x.Show && x.Edition))
						{
							var fieldController = this.fieldControllers[line].Where (x => x.ColumnMapper.Column == mapper.Column).FirstOrDefault ();

							if (fieldController != null)
							{
								double left, width;
								if (this.arrayController.GetColumnGeometry (mapper.Column, out left, out width))
								{
									fieldController.Box.Visibility = true;
									fieldController.Box.Margins = new Margins (left, 0, 0, 0);
									fieldController.Box.PreferredWidth = width-1;
								}
								else
								{
									fieldController.Box.Visibility = false;
								}
							}
						}
					}
				}
				else
				{
					this.assistantController.UpdateGeometry ();
				}
#endif
			}
		}

		protected virtual void UpdateArrayColumns()
		{
		}


		protected void EditorSelect(ColumnType columnType, int? line = null, bool selectedLineChanged = true)
		{
			var column = this.GetMapperColumnRank (columnType);
			this.EditorSelect (column, line, selectedLineChanged);
		}

		public void EditorSelect(int column, int? line = null, bool selectedLineChanged = true)
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

						this.UpdateToolbar ();

						if (selectedLineChanged)
						{
							this.SelectedLineChanged ();
						}
					}
				}

				if (this.selectedLine >= 0 && this.selectedLine < this.fieldControllers.Count && 
					column >= 0 && column < this.fieldControllers[this.selectedLine].Count &&
					this.dataAccessor.IsActive)
				{
					//	Normalement, le SetFocus va provoquer l'appel de HandleSetFocus. Mais, si le focus est
					//	déjà présent dans le widget, l'appel n'a pas lieu (c'est sans doute une optimisation de
					//	Widgets). D'où le code ci-dessous qui met d'abord le focus à un parent bidon.

					this.editorFrameBox.ClearFocus ();
					this.editorFrameBox.Focus ();  // met le focus à un parent bidon
					this.fieldControllers[this.selectedLine][column].SetFocus ();
				}

				this.UpdateEditorInfo ();
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


		protected void HandleLinesContainerTabPressed(object sender, Message message)
		{
			//	Appelé lorsque la touche (Shift+)Tab est pressée.
			int direction = message.IsShiftPressed ? -1 : 1;  // en arrière si Shift est pressé

			var column = this.selectedColumn;  // colonne actuelle
			var line   = this.selectedLine;    // ligne actuelle

			do
			{
				//	Cherche la colonne suivante.
				if (!this.GetNextPrevColumn (ref column, direction))  // est-on arrivé à une extrémité ?
				{
					//	Cherche la ligne suivante.
					line += direction;

					if (line < 0)  // remonté avant le début ?
					{
						line = this.dataAccessor.CountEditedRow-1;  // va à la fin
					}

					if (line >= this.dataAccessor.CountEditedRow)  // descendu après la fin ?
					{
						line = 0;  // va au début
					}
				}
			}
			while (!this.GetWidgetVisibility (column, line) || this.GetTextFieldReadonly (column, line));

			//	Effectue éventuellement un scroll vertical.
			int first  = this.firstLine;
			int visibleLines = System.Math.Min (this.dataAccessor.CountEditedRow, this.maxLines);

			if (line < first)  // première ligne invisible car trop haute ?
			{
				first = line;
			}

			if (line >= first + visibleLines)  // première ligne invisible car trop basse ?
			{
				first = line - visibleLines + 1;
			}

			if (this.firstLine != first)
			{
				this.firstLine = first;

				this.UpdateAfterFirstLineChanged ();
			}

			this.EditorSelect (column, line, selectedLineChanged: false);
		}

		private bool GetNextPrevColumn(ref ColumnType columnType, int direction)
		{
			//	Cherche la colonne suivante, en avant ou en arrière.
			//	Retourne false si on est arrivé à une extrémité.
			var mappers = this.columnMappers.Where (x => x.Show && x.Edition).ToList ();  // seulement les colonnes visibles

			var c = columnType;
			int i = mappers.FindIndex (x => x.Column == c);

			if (i+direction >= 0 && i+direction < mappers.Count)
			{
				columnType = mappers[i+direction].Column;
				return true;
			}

			if (direction < 0)  // recule ?
			{
				columnType = mappers.Last ().Column;  // retourne la colonne de droite
			}
			else  // avance ?
			{
				columnType = mappers.First ().Column;  // retourne la colonne de gauche
			}
			return false;
		}


		protected virtual void UpdateAfterFirstLineChanged()
		{
		}


		protected FrameBox CreateEditorUI(Widget parent)
		{
			//	Crée le panneau d'édition.
			this.fieldControllers.Clear ();

			if (this.controller.HasRightEditor)
			{
				return this.CreateRightEditorUI (parent);
			}
			else
			{
				return this.CreateFooterEditorUI (parent);
			}
		}

		private FrameBox CreateFooterEditorUI(Widget parent)
		{
			//	Crée le panneau inférieur, lorsque l'éditeur n'est pas en mode HasRightEditor.
			this.editorBackgroundFrameBox = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Bottom,
			};

			this.CreateForegroundEditorUI ();

			return this.editorFrameBox;
		}

		private FrameBox CreateRightEditorUI(Widget parent)
		{
			//	Crée le panneau de droite, lorsque l'éditeur est en mode HasRightEditor.
			var mainFrame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			//	Met la petite toolbar qui contient des icônes liées à des commandes.
			var commands = this.MiniToolbarCommands;
			if (commands != null && commands.Any ())
			{
				UIBuilder.CreateMiniToolbar (mainFrame, commands.ToArray ());
			}

			//	Met la "toolbar", qui affiche le mode "Création/Modification d'un ...". 
			this.bottomToolbarController = new BottomToolbarController (this.businessContext);
			var toolbar = this.bottomToolbarController.CreateUI (mainFrame);

			toolbar.PreferredWidth = this.controller.RightEditorWidth;
			toolbar.Dock           = DockStyle.Top;
			toolbar.Margins        = new Margins (0);
			toolbar.Padding        = new Margins (0);

			//	Met le panneau éditable.
			this.editorBackgroundFrameBox = new FrameBox
			{
				Parent = mainFrame,
				Dock   = DockStyle.Fill,
			};

			this.CreateForegroundEditorUI ();

			this.editorFrameBox.DrawFullFrame = true;
			this.editorFrameBox.Padding = new Margins (10);

			return this.editorFrameBox;
		}

		private void CreateForegroundEditorUI()
		{
			//	Crée 3 'calques' exactement les uns sur les autres.
			//	Dessus:		this.editorForegroundFrameBox	surcouche cliquable, présente (mais invisible) seulement en mode 'rien'
			//				this.editorFrameBox				zone éditable
			//	Dessous:	this.editorBackgroundFrameBox	parent commun toujours présent
			this.editorFrameBox = new FrameBox
			{
				Parent = this.editorBackgroundFrameBox,
				Anchor = AnchorStyles.All,
			};

			this.editorForegroundFrameBox = new FrameBox
			{
				Parent = this.editorBackgroundFrameBox,
				Anchor = AnchorStyles.All,
			};

			this.CreateForegroundEditorUI (this.editorForegroundFrameBox);

			this.editorForegroundFrameBox.Clicked += delegate
			{
				this.CreateAction ();
			};

			//?ToolTip.Default.SetToolTip (this.editorForegroundFrameBox, "Cliquez ici pour créer une nouvelle ligne");
		}

		protected virtual void CreateForegroundEditorUI(Widget parent)
		{
		}


		protected virtual IEnumerable<Command> MiniToolbarCommands
		{
			get
			{
				return null;
			}
		}


		public void UpdateFieldsEditionData()
		{
			//	Cette méthode est appelé par le DataAccessor, chaque fois qu'une ligne en édition (EditionData)
			//	a été créée, pour mettre à jour les AbstractFieldController.
			for (int line = 0; line < this.fieldControllers.Count; line++)
			{
				int column = 0;
				foreach (var mapper in this.columnMappers.Where (x => x.Show && x.Edition))
				{
					this.fieldControllers[line][column++].EditionData = this.dataAccessor.GetEditionData (line, mapper.Column);
				}
			}
		}


		public bool Duplicate
		{
			get
			{
				return this.duplicate;
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


		protected readonly AbstractController					controller;
		protected readonly ComptaEntity							compta;
		protected readonly ComptaPériodeEntity					période;
		protected readonly List<ColumnMapper>					columnMappers;
		protected readonly AbstractDataAccessor					dataAccessor;
		protected readonly ArrayController						arrayController;
		protected readonly BusinessContext						businessContext;
		protected readonly SettingsList							settingsList;
		protected readonly List<FrameBox>						linesFrames;
		protected readonly List<List<AbstractFieldController>>	fieldControllers;

		protected FrameBox										parent;
		protected System.Action									updateArrayContentAction;
		protected BottomToolbarController						bottomToolbarController;
		protected ColumnType									selectedColumn;
		protected int											selectedLine;
		protected int											maxLines;
		protected int											firstLine;
		protected bool											dirty;
		protected bool											hasError;
		protected bool											duplicate;
		protected FrameBox										editorBackgroundFrameBox;
		protected FrameBox										editorForegroundFrameBox;
		protected FrameBox										editorFrameBox;
		protected AbstractAssistantController					assistantController;
	}
}
