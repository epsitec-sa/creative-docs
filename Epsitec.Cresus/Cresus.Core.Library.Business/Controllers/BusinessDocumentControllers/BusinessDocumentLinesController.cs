//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support;
using Epsitec.Common.Dialogs;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library.Business.ContentAccessors;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public sealed class BusinessDocumentLinesController : System.IDisposable
	{
		public BusinessDocumentLinesController(AccessData accessData)
		{
			this.accessData = accessData;

			this.commandDispatcher = new CommandDispatcher ("BusinessDocumentLinesController", CommandDispatcherLevel.Secondary, CommandDispatcherOptions.AutoForwardCommands | CommandDispatcherOptions.ActivateWithoutFocus);
			this.commandDispatcher.RegisterController (this);

			this.commandContext = new CommandContext ("BusinessDocumentLinesController", CommandContextOptions.ActivateWithoutFocus);

			//	Si les états ne sont pas définis, met les valeurs par défaut.
			if (this.CurrentViewMode == ViewMode.Unknown)
			{
				this.CurrentViewMode = BusinessDocumentLinesController.persistantViewMode;
			}

			if (this.CurrentEditMode == EditMode.Unknown)
			{
				this.CurrentEditMode = BusinessDocumentLinesController.persistantEditMode;
			}

			this.lineInformations = new List<LineInformations> ();
			this.UpdateLineInformations ();

			this.linesEngine = new LinesEngine (this.accessData.BusinessContext, this.accessData.BusinessDocumentEntity);
		}

		public void CreateUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
			};

			CommandDispatcher.SetDispatcher (frame, this.commandDispatcher);
			CommandContext.SetContext (frame, this.commandContext);

			//	Crée la toolbar.
			this.lineToolbarController = new LineToolbarController (this.accessData.DocumentMetadataEntity, this.accessData.BusinessDocumentEntity);
			var toolbarWidget = this.lineToolbarController.CreateUI (frame);
			toolbarWidget.Visibility = BusinessDocumentLinesController.persistantShowToolbar;

			//	Crée la liste.
			this.linesController = new LinesController (this.accessData);
			this.linesController.CreateUI (frame, this.CallbackSelectionChanged);

			//	Crée l'éditeur pour une ligne.
			this.lineEditorController = new LineEditorController (this.accessData);
			this.lineEditorController.CreateUI (frame);

			//	Crée un splitter juste au-dessus de l'éditeur de ligne.
			var splitter = new HSplitter
			{
				Parent = frame,
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, 0, 5, 0),
			};

			//	Crée le bouton 'v'.
			var showToolbarButton = new GlyphButton
			{
				Parent = frame,
				Anchor = AnchorStyles.TopRight,
				PreferredSize = new Size (16, 16),
				Margins = new Margins (0, 2, 2, 0),
				GlyphShape = BusinessDocumentLinesController.persistantShowToolbar ? GlyphShape.TriangleUp : GlyphShape.TriangleDown,
				ButtonStyle = ButtonStyle.Slider,
			};

			ToolTip.Default.SetToolTip (showToolbarButton, "Montre ou cache la barre d'icônes");

			showToolbarButton.Clicked += delegate
			{
				toolbarWidget.Visibility = !toolbarWidget.Visibility;
				showToolbarButton.GlyphShape = toolbarWidget.Visibility ? GlyphShape.TriangleUp : GlyphShape.TriangleDown;
				BusinessDocumentLinesController.persistantShowToolbar = toolbarWidget.Visibility;
			};
		}

		public void UpdateUI()
		{
			this.UpdateAfterChange ();
			this.UpdateCommands ();
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.commandContext.Dispose ();
			this.commandDispatcher.Dispose ();
		}

		#endregion


		private LineInformations CallbackGetLineInformations(int index)
		{
			//	Retourne les informations sur l'état d'une ligne du tableau.
			return this.lineInformations[index];
		}

		private FormattedText CallbackGetCellContent(int index, ColumnType columnType)
		{
			//	Retourne le contenu permettant de peupler une cellule du tableau.
			var info = this.lineInformations[index];

			switch (columnType)
			{
				case ColumnType.GroupNumber:
					return info.GetColumnContent (DocumentItemAccessorColumn.GroupNumber);

				case ColumnType.LineNumber:
					return info.GetColumnContent (DocumentItemAccessorColumn.LineNumber);

				case ColumnType.QuantityAndUnit:
					var q = info.GetColumnContent (DocumentItemAccessorColumn.UniqueQuantity).ToString ();
					var u = info.GetColumnContent (DocumentItemAccessorColumn.UniqueUnit).ToString ();

					if (string.IsNullOrEmpty (q))
					{
						return null;
					}
					else
					{
						return Misc.FormatUnit (decimal.Parse (q), u);
					}

				case ColumnType.Type:
					return info.GetColumnContent (DocumentItemAccessorColumn.UniqueType);

				case ColumnType.Date:
					return info.GetColumnContent (DocumentItemAccessorColumn.UniqueBeginDate);

				case ColumnType.ArticleId:
					return info.GetColumnContent (DocumentItemAccessorColumn.ArticleId);

				case ColumnType.ArticleDescription:
					return info.GetColumnContent (DocumentItemAccessorColumn.ArticleDescription);

				case ColumnType.Discount:
					return info.GetColumnContent (DocumentItemAccessorColumn.Discount);

				case ColumnType.UnitPrice:
					return info.GetColumnContent (DocumentItemAccessorColumn.UnitPrice);

				case ColumnType.LinePrice:
					return info.GetColumnContent (DocumentItemAccessorColumn.LinePrice);

				case ColumnType.Vat:
					return info.GetColumnContent (DocumentItemAccessorColumn.Vat);

				case ColumnType.Total:
					return info.GetColumnContent (DocumentItemAccessorColumn.Total);

				case ColumnType.GroupIndex:
					return info.AbstractDocumentItemEntity.GroupIndex.ToString ();
			}

			return null;
		}

		private bool CallbackSelectionChanged()
		{
			//	Appelé lorsque la sélection dans la liste a changé.
			if (this.linesController.HasSingleSelection)
			{
				int? sel = this.linesController.LastSelection;
				var info = this.lineInformations[sel.Value];

				this.lineEditorController.UpdateUI (this.CurrentEditMode, info);
			}
			else
			{
				this.lineEditorController.UpdateUI (this.CurrentEditMode, null);
			}

			this.UpdateCommands ();

			return true;
		}


		[Command (Library.Business.Res.CommandIds.Lines.Deselect)]
		public void ProcessDeselect()
		{
			//	Désélectionne toutes les lignes.
			this.linesController.Deselect ();
		}

		[Command (Library.Business.Res.CommandIds.Lines.GroupSelect)]
		public void ProcessGroupSelect()
		{
			//	Sélectionne toutes les lignes du groupe.
			var selection = this.Selection;

			if (selection.Count == 0)
			{
				return;
			}

			int groupIndex = selection[0].AbstractDocumentItemEntity.GroupIndex;
			int level = LinesEngine.GetLevel (groupIndex);

			if (level == 0)
			{
				return;
			}

			selection = new List<LineInformations> ();

			foreach (var info in this.lineInformations)
			{
				if (LinesEngine.LevelCompare (groupIndex, info.AbstractDocumentItemEntity.GroupIndex, level))
				{
					selection.Add (info);
				}
			}

			this.Selection = selection;
		}

		[Command (Library.Business.Res.CommandIds.Lines.ViewCompact)]
		public void ProcessViewCompact()
		{
			this.CurrentViewMode = ViewMode.Compact;
			this.UpdateAfterChange ();
		}

		[Command (Library.Business.Res.CommandIds.Lines.ViewDefault)]
		public void ProcessViewDefault()
		{
			this.CurrentViewMode = ViewMode.Default;
			this.UpdateAfterChange ();
		}

		[Command (Library.Business.Res.CommandIds.Lines.ViewFull)]
		public void ProcessViewFull()
		{
			this.CurrentViewMode = ViewMode.Full;
			this.UpdateAfterChange ();
		}

		[Command (Library.Business.Res.CommandIds.Lines.EditName)]
		public void ProcessEditName()
		{
			this.CurrentEditMode = EditMode.Name;
			this.UpdateAfterChange ();
			this.CallbackSelectionChanged ();
		}

		[Command (Library.Business.Res.CommandIds.Lines.EditDescription)]
		public void ProcessEditDescription()
		{
			this.CurrentEditMode = EditMode.Description;
			this.UpdateAfterChange ();
			this.CallbackSelectionChanged ();
		}


		[Command (Library.Business.Res.CommandIds.Lines.CreateArticle)]
		public void ProcessCreateArticle()
		{
			var selection = this.linesEngine.CreateArticle (this.Selection, isTax: false);
			this.UpdateAfterChange (this.linesEngine.LastError, selection);
		}

		[Command (Library.Business.Res.CommandIds.Lines.CreateQuantity)]
		public void ProcessCreateQuantity()
		{
			//	Insère une nouvelle quantité.
			var selection = this.linesEngine.CreateQuantity (this.Selection, ArticleQuantityType.Delayed, 7);
			this.UpdateAfterChange (this.linesEngine.LastError, selection);
		}

		[Command (Library.Business.Res.CommandIds.Lines.CreateText)]
		public void ProcessCreateText()
		{
			//	Insère une nouvelle ligne de texte.
			var selection = this.linesEngine.CreateText (this.Selection, isTitle: false);
			this.UpdateAfterChange (this.linesEngine.LastError, selection);
		}

		[Command (Library.Business.Res.CommandIds.Lines.CreateTitle)]
		public void ProcessCreateTitle()
		{
			//	Insère une nouvelle ligne de titre.
			var selection = this.linesEngine.CreateText (this.Selection, isTitle: true);
			this.UpdateAfterChange (selection);
		}

		[Command (Library.Business.Res.CommandIds.Lines.CreateDiscount)]
		public void ProcessCreateDiscount()
		{
			//	Insère une nouvelle ligne de rabais.
			var selection = this.linesEngine.CreateDiscount (this.Selection);
			this.UpdateAfterChange (this.linesEngine.LastError, selection);
		}

		[Command (Library.Business.Res.CommandIds.Lines.CreateTax)]
		public void ProcessCreateTax()
		{
			//	Insère une nouvelle ligne de frais.
			var selection = this.linesEngine.CreateArticle (this.Selection, isTax: true);
			this.UpdateAfterChange (this.linesEngine.LastError, selection);
		}

		[Command (Library.Business.Res.CommandIds.Lines.CreateGroup)]
		private void ProcessCreateGroup()
		{
			//	Insère un nouveau groupe contenant un titre.
			var selection = this.linesEngine.CreateGroup (this.Selection);
			this.UpdateAfterChange (this.linesEngine.LastError, selection);
		}

		[Command (Library.Business.Res.CommandIds.Lines.MoveUp)]
		private void ProcessMoveUp()
		{
			//	Monte la ligne sélectionnée.
			this.linesEngine.Move (this.Selection, -1);
			this.UpdateAfterChange (this.linesEngine.LastError);
		}

		[Command (Library.Business.Res.CommandIds.Lines.MoveDown)]
		private void ProcessMoveDown()
		{
			//	Descend la ligne sélectionnée.
			this.linesEngine.Move (this.Selection, 1);
			this.UpdateAfterChange (this.linesEngine.LastError);
		}

		[Command (Library.Business.Res.CommandIds.Lines.Duplicate)]
		private void ProcessDuplicate()
		{
			//	Duplique la ligne sélectionnée.
			var selection = this.linesEngine.Duplicate (this.Selection);
			this.UpdateAfterChange (this.linesEngine.LastError, selection);
		}

		[Command (Library.Business.Res.CommandIds.Lines.Delete)]
		private void ProcessDelete()
		{
			//	Supprime la ligne sélectionnée.
			var selection = this.linesEngine.Delete (this.Selection);
			this.UpdateAfterChange (this.linesEngine.LastError, selection);
		}

		[Command (Library.Business.Res.CommandIds.Lines.Group)]
		private void ProcessGroup()
		{
			//	Groupe toutes les lignes sélectionnées.
			this.linesEngine.MakeGroup (this.Selection);
			this.UpdateAfterChange (this.linesEngine.LastError);
		}

		[Command (Library.Business.Res.CommandIds.Lines.Ungroup)]
		private void ProcessUngroup()
		{
			//	Défait le groupe sélectionné.
			this.linesEngine.MakeUngroup (this.Selection);
			this.UpdateAfterChange (this.linesEngine.LastError);
		}

		[Command (Library.Business.Res.CommandIds.Lines.Split)]
		private void ProcessSplit()
		{
			//	Sépare la ligne d'avec la précédente.
			this.linesEngine.MakeSplit (this.Selection);
			this.UpdateAfterChange (this.linesEngine.LastError);
		}

		[Command (Library.Business.Res.CommandIds.Lines.Combine)]
		private void ProcessCombine()
		{
			//	Soude la ligne avec la précédente.
			this.linesEngine.MakeCombine (this.Selection);
			this.UpdateAfterChange (this.linesEngine.LastError);
		}

		[Command (Library.Business.Res.CommandIds.Lines.Flat)]
		private void ProcessFlat()
		{
			//	Remet à plat toutes les lignes.
			this.linesEngine.MakeFlat (this.Selection);
			this.UpdateAfterChange (this.linesEngine.LastError);
		}


		private void UpdateAfterChange(LinesError error)
		{
			this.UpdateAfterChange (error, this.Selection);
		}

		private void UpdateAfterChange(LinesError error, List<LineInformations> selection)
		{
			if (error == LinesError.OK)
			{
				this.UpdateAfterChange (selection);
			}
			else
			{
				var message = linesEngine.GetError (error);
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, message.ToString ()).OpenDialog ();
			}
		}

		private void UpdateAfterChange()
		{
			this.UpdateAfterChange (this.Selection);
		}

		private void UpdateAfterChange(List<LineInformations> selection)
		{
			this.UpdateLineInformations ();

			this.linesController.UpdateUI (this.CurrentViewMode, this.CurrentEditMode, this.lineInformations.Count, this.CallbackGetLineInformations, this.CallbackGetCellContent);
			this.Selection = selection;

			this.UpdateCommands ();
		}


		private void UpdateLineInformations()
		{
			this.lineInformations.Clear ();

			var mode = DocumentItemAccessorMode.ForceAllLines |
					   DocumentItemAccessorMode.ShowMyEyesOnly |
					   DocumentItemAccessorMode.SpecialQuantitiesToDistinctLines;

			if (this.CurrentEditMode == EditMode.Name)
			{
				mode |= DocumentItemAccessorMode.EditArticleName;
			}
			else
			{
				mode |= DocumentItemAccessorMode.EditArticleDescription;
			}

			for (int i = 0; i < this.accessData.BusinessDocumentEntity.Lines.Count; i++)
			{
				var line = this.accessData.BusinessDocumentEntity.Lines[i];

				var accessor = new DocumentItemAccessor (this.accessData.BusinessDocumentEntity);
				accessor.BuildContent (line, i, this.accessData.DocumentMetadataEntity.DocumentCategory.DocumentType, mode);

				for (int row = 0; row < accessor.RowsCount; row++)
				{
					var quantity = accessor.GetArticleQuantityEntity (row);
					this.lineInformations.Add (new LineInformations (accessor, line, quantity, row));
				}
			}
		}


		private void UpdateCommands()
		{
#if false
			var selection     = this.linesController.Selection;
			var lastSelection = this.linesController.LastSelection;
			var isCoherentSelection = this.linesEngine.IsCoherentSelection (this.Selection);

			LineInformations info = null;
			bool autoGenerated = false;

			if (lastSelection != null)
			{
				info = this.lineInformations[lastSelection.Value];
				autoGenerated = info.AbstractDocumentItemEntity.Attributes.HasFlag (DocumentItemAttributes.AutoGenerated);
			}

			bool isArticle = info != null && info.AbstractDocumentItemEntity is ArticleDocumentItemEntity;

			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateArticle,  true);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateQuantity, selection.Count == 1 && isArticle);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateText,     true);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateTitle,    true);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateGroup,    true);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateDiscount, true);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateTax,      true);

			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.MoveUp,         selection.Count == 1 && !autoGenerated);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.MoveDown,       selection.Count == 1 && !autoGenerated);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Duplicate,      selection.Count == 1 && !autoGenerated);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Delete,         selection.Count >= 1 && !autoGenerated);

			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Group,          isCoherentSelection && !autoGenerated);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Ungroup,        isCoherentSelection && !autoGenerated);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Split,          this.linesEngine.IsSplitEnabled (this.Selection));
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Combine,        this.linesEngine.IsCombineEnabled (this.Selection));
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Flat,           this.linesEngine.IsFlatEnabled);

			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Deselect,       selection.Count != 0);
#else
			var selection = this.Selection;

			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateArticle,  true);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateQuantity, this.linesEngine.IsCreateQuantityEnabled (selection));
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateText,     true);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateTitle,    true);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateGroup,    true);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateDiscount, true);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateTax,      true);

			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.MoveUp,         this.linesEngine.IsMoveEnabled (selection, -1));
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.MoveDown,       this.linesEngine.IsMoveEnabled (selection, 1));
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Duplicate,      this.linesEngine.IsDuplicateEnabled (selection));
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Delete,         this.linesEngine.IsDeleteEnabled (selection));

			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Group,          this.linesEngine.IsGroupEnabled (selection));
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Ungroup,        this.linesEngine.IsUngroupEnabled (selection));
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Split,          this.linesEngine.IsSplitEnabled (selection));
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Combine,        this.linesEngine.IsCombineEnabled (selection));
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Flat,           this.linesEngine.IsFlatEnabled);

			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Deselect,       selection.Count != 0);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.GroupSelect,    selection.Count != 0);
#endif
		}


		private List<LineInformations> Selection
		{
			// Le getter retourne la liste des lignes sélectionnées.
			// Le setter sélectionne les lignes données dans la liste.
			get
			{
				var list = new List<LineInformations> ();

				foreach (var selection in this.linesController.Selection)
				{
					var info = this.lineInformations[selection];
					list.Add (info);
				}

				return list;
			}
			set
			{
				var list = new List<int> ();

				if (value != null)
				{
					foreach (var info in value)
					{
						int i = this.IndexOfLineInformations (info);

						if (i != -1)
						{
							list.Add (i);
						}
					}
				}

				this.linesController.Selection = list;
			}
		}

		private int IndexOfLineInformations(LineInformations info)
		{
			if (info.ArticleQuantityEntity != null)  // cherche une quantité précise ?
			{
				for (int i = 0; i < this.lineInformations.Count; i++)
				{
					var nextInfo = this.lineInformations[i];

					if (nextInfo.AbstractDocumentItemEntity == info.AbstractDocumentItemEntity &&
						nextInfo.ArticleQuantityEntity      == info.ArticleQuantityEntity      )
					{
						return i;
					}
				}
			}

			for (int i = 0; i < this.lineInformations.Count; i++)
			{
				var nextInfo = this.lineInformations[i];

				if (nextInfo.AbstractDocumentItemEntity == info.AbstractDocumentItemEntity)
				{
					return i;
				}
			}

			return -1;
		}


		private ViewMode CurrentViewMode
		{
			get
			{
				if (this.commandContext.GetCommandState (Library.Business.Res.Commands.Lines.ViewCompact).ActiveState == ActiveState.Yes)
				{
					return ViewMode.Compact;
				}

				if (this.commandContext.GetCommandState (Library.Business.Res.Commands.Lines.ViewDefault).ActiveState == ActiveState.Yes)
				{
					return ViewMode.Default;
				}

				if (this.commandContext.GetCommandState (Library.Business.Res.Commands.Lines.ViewFull).ActiveState == ActiveState.Yes)
				{
					return ViewMode.Full;
				}

				return ViewMode.Unknown;
			}
			set
			{
				this.commandContext.GetCommandState (Library.Business.Res.Commands.Lines.ViewCompact).ActiveState = (value == ViewMode.Compact) ? ActiveState.Yes : ActiveState.No;
				this.commandContext.GetCommandState (Library.Business.Res.Commands.Lines.ViewDefault).ActiveState = (value == ViewMode.Default) ? ActiveState.Yes : ActiveState.No;
				this.commandContext.GetCommandState (Library.Business.Res.Commands.Lines.ViewFull   ).ActiveState = (value == ViewMode.Full   ) ? ActiveState.Yes : ActiveState.No;

				BusinessDocumentLinesController.persistantViewMode = value;
			}
		}

		private EditMode CurrentEditMode
		{
			get
			{
				if (this.commandContext.GetCommandState (Library.Business.Res.Commands.Lines.EditName).ActiveState == ActiveState.Yes)
				{
					return EditMode.Name;
				}

				if (this.commandContext.GetCommandState (Library.Business.Res.Commands.Lines.EditDescription).ActiveState == ActiveState.Yes)
				{
					return EditMode.Description;
				}

				return EditMode.Unknown;
			}
			set
			{
				this.commandContext.GetCommandState (Library.Business.Res.Commands.Lines.EditName       ).ActiveState = (value == EditMode.Name       ) ? ActiveState.Yes : ActiveState.No;
				this.commandContext.GetCommandState (Library.Business.Res.Commands.Lines.EditDescription).ActiveState = (value == EditMode.Description) ? ActiveState.Yes : ActiveState.No;

				BusinessDocumentLinesController.persistantEditMode = value;
			}
		}


		// TODO: Il faudra un jour que ces variables survivent à l'extinction de l'application !
		private static ViewMode							persistantViewMode = ViewMode.Default;
		private static EditMode							persistantEditMode = EditMode.Name;
		private static bool								persistantShowToolbar = false;

		private readonly AccessData						accessData;
		private readonly List<LineInformations>			lineInformations;
		private readonly CommandContext					commandContext;
		private readonly CommandDispatcher				commandDispatcher;
		private readonly LinesEngine					linesEngine;

		private LineToolbarController					lineToolbarController;
		private LinesController							linesController;
		private LineEditorController					lineEditorController;
	}
}
