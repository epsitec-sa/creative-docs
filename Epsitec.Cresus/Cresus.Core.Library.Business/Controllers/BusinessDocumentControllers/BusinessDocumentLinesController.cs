﻿//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Dialogs;

using Epsitec.Cresus.Core.Library.Business.ContentAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public sealed partial class BusinessDocumentLinesController : System.IDisposable, ILineProvider
	{
		public BusinessDocumentLinesController(AccessData accessData)
		{
			this.accessData = accessData;

			this.commandProcessor  = new CommandProcessor (this);
			this.commandDispatcher = new CommandDispatcher ("BusinessDocumentLinesController", CommandDispatcherLevel.Secondary, CommandDispatcherOptions.AutoForwardCommands | CommandDispatcherOptions.ActivateWithoutFocus);
			this.commandContext    = new CommandContext ("BusinessDocumentLinesController", CommandContextOptions.ActivateWithoutFocus);

			this.commandDispatcher.RegisterController (this.commandProcessor);
			
			//	Si les états ne sont pas définis, met les valeurs par défaut.
			if (this.CurrentViewMode == ViewMode.Unknown)
			{
				this.CurrentViewMode = BusinessDocumentLinesController.persistantViewMode;
			}

			if (this.CurrentEditMode == EditMode.Unknown)
			{
				this.CurrentEditMode = BusinessDocumentLinesController.persistantEditMode;
			}

			this.lines = new List<Line> ();
			this.linesEngine = new LineEngine (this.accessData.BusinessContext, this.accessData.BusinessDocument, this.accessData.DocumentLogic);
			
			this.UpdateLines ();
		}

		
		private List<Line>						Selection
		{
			// Le getter retourne la liste des lignes sélectionnées.
			// Le setter sélectionne les lignes données dans la liste.
			get
			{
				var list = new List<Line> ();

				foreach (var selection in this.lineTableController.Selection)
				{
					var info = this.lines[selection];
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
						int i = this.GetLineIndex (info);

						if (i != -1)
						{
							list.Add (i);
						}
					}
				}

				this.lineTableController.Selection = list;
			}
		}

		private ViewMode						CurrentViewMode
		{
			get
			{
				if (this.commandContext.IsActive (Library.Business.Res.Commands.Lines.ViewCompact))
				{
					return ViewMode.Compact;
				}

				if (this.commandContext.IsActive (Library.Business.Res.Commands.Lines.ViewDefault))
				{
					return ViewMode.Default;
				}

				if (this.commandContext.IsActive (Library.Business.Res.Commands.Lines.ViewFull))
				{
					return ViewMode.Full;
				}

				if (this.commandContext.IsActive (Library.Business.Res.Commands.Lines.ViewDebug))
				{
					return ViewMode.Debug;
				}

				return ViewMode.Unknown;
			}
			set
			{
				this.commandContext.SetActiveState (Library.Business.Res.Commands.Lines.ViewCompact, value == ViewMode.Compact);
				this.commandContext.SetActiveState (Library.Business.Res.Commands.Lines.ViewDefault, value == ViewMode.Default);
				this.commandContext.SetActiveState (Library.Business.Res.Commands.Lines.ViewFull, value == ViewMode.Full);
				this.commandContext.SetActiveState (Library.Business.Res.Commands.Lines.ViewDebug, value == ViewMode.Debug);

				this.accessData.DocumentLogic.IsDebug = (value == ViewMode.Debug);

				BusinessDocumentLinesController.persistantViewMode = value;
			}
		}

		private EditMode						CurrentEditMode
		{
			get
			{
				if (this.commandContext.IsActive (Library.Business.Res.Commands.Lines.EditName))
				{
					return EditMode.Name;
				}

				if (this.commandContext.IsActive (Library.Business.Res.Commands.Lines.EditDescription))
				{
					return EditMode.Description;
				}

				return EditMode.Unknown;
			}
			set
			{
				this.commandContext.SetActiveState (Library.Business.Res.Commands.Lines.EditName, value == EditMode.Name);
				this.commandContext.SetActiveState (Library.Business.Res.Commands.Lines.EditDescription, value == EditMode.Description);

				BusinessDocumentLinesController.persistantEditMode = value;
			}
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
			this.lineToolbarController = new LineToolbarController (this.accessData.CoreData, this.accessData.DocumentMetadata, this.accessData.BusinessDocument);
			var toolbarWidget = this.lineToolbarController.CreateUI (frame);
			toolbarWidget.Visibility = BusinessDocumentLinesController.persistantShowToolbar;

			//	Crée la liste.
			this.lineTableController = new LineTableController (this.accessData);
			this.lineTableController.CreateUI (frame);
			this.lineTableController.SelectionChanged += this.HandleLinesControllerSelectionChanged;

			//	Crée l'éditeur pour une ligne.
			this.lineEditionPanelController = new LineEditionPanelController (this.accessData);
			this.lineEditionPanelController.CreateUI (frame);

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

			BusinessDocumentLinesController.actionRibbonShow ("Business", true);
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

			BusinessDocumentLinesController.actionRibbonShow ("Business", false);
		}

		#endregion

		#region ILineProvider Members

		EditMode ILineProvider.CurrentEditMode
		{
			get
			{
				return this.CurrentEditMode;
			}
		}

		ViewMode ILineProvider.CurrentViewMode
		{
			get
			{
				return this.CurrentViewMode;
			}
		}

		int ILineProvider.Count
		{
			get
			{
				return this.lines.Count;
			}
		}

		Line ILineProvider.GetLine(int index)
		{
			//	Retourne les informations sur l'état d'une ligne du tableau.
			return this.lines[index];
		}

		CellContent ILineProvider.GetCellContent(int index, ColumnType columnType)
		{
			//	Retourne le contenu permettant de peupler une cellule du tableau.
			var info = this.lines[index];
			FormattedText text;
			DocumentItemAccessorError error;

			switch (columnType)
			{
				case ColumnType.GroupIndex:
					text = info.DocumentItem.GroupIndex.ToString ();
					return new CellContent (text);

				case ColumnType.GroupNumber:
					text = info.GetColumnContent (DocumentItemAccessorColumn.GroupNumber);
					error = info.GetColumnError (DocumentItemAccessorColumn.GroupNumber);
					return new CellContent (text, error);

				case ColumnType.LineNumber:
					text = info.GetColumnContent (DocumentItemAccessorColumn.LineNumber);
					error = info.GetColumnError (DocumentItemAccessorColumn.LineNumber);
					return new CellContent (text, error);

				case ColumnType.FullNumber:
					text = info.GetColumnContent (DocumentItemAccessorColumn.FullNumber);
					error = info.GetColumnError (DocumentItemAccessorColumn.FullNumber);
					return new CellContent (text, error);

				case ColumnType.QuantityAndUnit:
					var q = info.GetColumnContent (DocumentItemAccessorColumn.AdditionalQuantity).ToString ();
					var u = info.GetColumnContent (DocumentItemAccessorColumn.AdditionalUnit).ToString ();

					if (string.IsNullOrEmpty (q))
					{
						return CellContent.Empty;
					}
					else
					{
						text = Misc.FormatUnit (decimal.Parse (q), u);
						error = info.GetColumnError (DocumentItemAccessorColumn.AdditionalQuantity);
						return new CellContent (text, error);
					}

				case ColumnType.AdditionalType:
					text = info.GetColumnContent (DocumentItemAccessorColumn.AdditionalType);
					error = info.GetColumnError (DocumentItemAccessorColumn.AdditionalType);
					return new CellContent (text, error);

				case ColumnType.AdditionalDate:
					text = info.GetColumnContent (DocumentItemAccessorColumn.AdditionalBeginDate);
					error = info.GetColumnError (DocumentItemAccessorColumn.AdditionalBeginDate);
					return new CellContent (text, error);

				case ColumnType.ArticleId:
					text = info.GetColumnContent (DocumentItemAccessorColumn.ArticleId);
					error = info.GetColumnError (DocumentItemAccessorColumn.ArticleId);
					return new CellContent (text, error);

				case ColumnType.ArticleDescription:
					text = info.GetColumnContent (DocumentItemAccessorColumn.ArticleDescription);
					error = info.GetColumnError (DocumentItemAccessorColumn.ArticleDescription);
					return new CellContent (text, error);

				case ColumnType.Discount:
					text = info.GetColumnContent (DocumentItemAccessorColumn.LineDiscount);
					error = info.GetColumnError (DocumentItemAccessorColumn.LineDiscount);
					return new CellContent (text, error);

				case ColumnType.UnitPrice:
					text = info.GetColumnContent (DocumentItemAccessorColumn.UnitPrice);
					error = info.GetColumnError (DocumentItemAccessorColumn.UnitPrice);
					return new CellContent (text, error);

				case ColumnType.LinePrice:
					text = info.GetColumnContent (DocumentItemAccessorColumn.LinePrice);
					error = info.GetColumnError (DocumentItemAccessorColumn.LinePrice);
					return new CellContent (text, error);

				case ColumnType.Vat:
					text = info.GetColumnContent (DocumentItemAccessorColumn.VatInfo);
					error = info.GetColumnError (DocumentItemAccessorColumn.VatInfo);
					return new CellContent (text, error);

				case ColumnType.Total:
					text = info.GetColumnContent (DocumentItemAccessorColumn.TotalPrice);
					error = info.GetColumnError (DocumentItemAccessorColumn.TotalPrice);
					return new CellContent (text, error);
			}

			return CellContent.Empty;
		}

		#endregion

		
		private void HandleLinesControllerSelectionChanged(object sender)
		{
			Line info = null;

			//	Appelé lorsque la sélection dans la liste a changé.
			if (this.lineTableController.HasSingleSelection)
			{
				int? sel = this.lineTableController.LastSelection;
				info = this.lines[sel.Value];
			}

			if (this.activeLine == info)
			{
				//	Do nothing
			}
			else
			{
				this.activeLine = info;
				this.lineEditionPanelController.UpdateUI (this.CurrentEditMode, info);
			}

			this.UpdateCommands ();
		}


		private void UpdateAfterChange(LineError error)
		{
			this.UpdateAfterChange (error, this.Selection);
		}

		private void UpdateAfterChange(LineError error, List<Line> selection)
		{
			if (error == LineError.OK)
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

		private void UpdateAfterChange(List<Line> selection)
		{
			this.UpdateLines ();

			this.lineTableController.UpdateUI (this);
			this.Selection = selection;

			this.UpdateCommands ();
		}


		private void UpdateLines()
		{
			this.lines.Clear ();

			var mode = DocumentItemAccessorMode.ShowMyEyesOnly;

			if (this.CurrentEditMode == EditMode.Name)
			{
				mode |= DocumentItemAccessorMode.EditArticleName;
			}
			else
			{
				mode |= DocumentItemAccessorMode.EditArticleDescription;
			}

			var lines = this.accessData.BusinessDocument.Lines.Select (x => new DocumentAccessorContentLine (x));

			foreach (var accessor in DocumentItemAccessor.CreateAccessors (this.accessData.DocumentMetadata, this.accessData.DocumentLogic, mode, lines))
			{
				for (int row = 0; row < accessor.RowsCount; row++)
				{
					var quantity = accessor.GetArticleQuantity (row);
					var error = accessor.GetError (row);
					var item = accessor.Item;
					this.lines.Add (new Line (accessor, item, quantity, row, error));

					var cellError = accessor.GetError (row);
				}
			}
		}

		private void UpdateCommands()
		{
			var selection = this.Selection;

			var isEditionEnabled    = this.accessData.DocumentLogic.IsLinesEditionEnabled;
			var isQuantityEnabled   = this.accessData.DocumentLogic.IsArticleQuantityEditionEnabled;
			var isMyEyesOnlyEnabled = this.accessData.DocumentLogic.IsMyEyesOnlyEditionEnabled;

			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateArticle,  isEditionEnabled);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateQuantity, (isEditionEnabled || isQuantityEnabled) && this.linesEngine.IsCreateQuantityEnabled (selection));
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateText,     isEditionEnabled || isMyEyesOnlyEnabled);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateTitle,    isEditionEnabled);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateGroup,    isEditionEnabled);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateDiscount, isEditionEnabled);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateTax,      isEditionEnabled);

			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.MoveUp,         (isEditionEnabled || isMyEyesOnlyEnabled) && this.linesEngine.IsMoveEnabled (selection, -1));
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.MoveDown,       (isEditionEnabled || isMyEyesOnlyEnabled) && this.linesEngine.IsMoveEnabled (selection, 1));
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Duplicate,      isEditionEnabled && this.linesEngine.IsDuplicateEnabled (selection));
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Delete,         (isEditionEnabled || isQuantityEnabled || isMyEyesOnlyEnabled) && this.linesEngine.IsDeleteEnabled (selection));

			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Group,          isEditionEnabled && this.linesEngine.IsGroupEnabled (selection));
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Ungroup,        isEditionEnabled && this.linesEngine.IsUngroupEnabled (selection));
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Split,          isEditionEnabled && this.linesEngine.IsSplitEnabled (selection));
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Combine,        isEditionEnabled && this.linesEngine.IsCombineEnabled (selection));
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Flat,           isEditionEnabled && this.linesEngine.IsFlatEnabled);

			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Deselect,       selection.Count != 0);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.GroupSelect,    selection.Count != 0);

			if (selection.Count != 1)
			{
				this.lineEditionPanelController.SetError (DocumentItemAccessorError.None);
			}
			else
			{
				this.lineEditionPanelController.SetError (selection[0].Error);
			}
		}

		private int GetLineIndex(Line info)
		{
			if (info.ArticleQuantity != null)  // cherche une quantité précise ?
			{
				for (int i = 0; i < this.lines.Count; i++)
				{
					var nextInfo = this.lines[i];

					if (nextInfo.DocumentItem == info.DocumentItem &&
						nextInfo.ArticleQuantity      == info.ArticleQuantity      )
					{
						return i;
					}
				}
			}

			for (int i = 0; i < this.lines.Count; i++)
			{
				var nextInfo = this.lines[i];

				if (nextInfo.DocumentItem == info.DocumentItem)
				{
					return i;
				}
			}

			return -1;
		}


		public static System.Action<string, bool>	actionRibbonShow;

		// TODO: Il faudra un jour que ces variables survivent à l'extinction de l'application !
		private static ViewMode					persistantViewMode    = ViewMode.Default;
		private static EditMode					persistantEditMode    = EditMode.Name;
		private static bool						persistantShowToolbar = false;

		private readonly AccessData				accessData;
		private readonly List<Line>				lines;
		private readonly CommandProcessor		commandProcessor;
		private readonly CommandContext			commandContext;
		private readonly CommandDispatcher		commandDispatcher;
		private readonly LineEngine				linesEngine;

		private LineToolbarController			lineToolbarController;
		private LineTableController				lineTableController;
		private LineEditionPanelController		lineEditionPanelController;

		private Line							activeLine;
	}
}
