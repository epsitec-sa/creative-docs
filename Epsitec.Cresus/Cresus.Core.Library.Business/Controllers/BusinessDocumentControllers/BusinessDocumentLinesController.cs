//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

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

			this.lineInformations = new List<LineInformations> ();
			this.UpdateLineInformations ();
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
			//?this.lineToolbarController = new LineToolbarController (this.accessData.DocumentMetadataEntity, this.accessData.BusinessDocumentEntity);
			//?this.lineToolbarController.CreateUI (frame);

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
				case ColumnType.Group:
					return info.AbstractDocumentItemEntity.GroupIndex.ToString ();  // TODO: provisoire (pour le debug)

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

				this.lineEditorController.UpdateUI (info);
			}
			else
			{
				this.lineEditorController.UpdateUI (null);
			}

			this.UpdateCommands ();

			return true;
		}


		[Command (Library.Business.Res.CommandIds.Lines.CreateArticle)]
		public void ProcessCreateArticle()
		{
			//	Insère une nouvelle ligne d'article.
			int? sel = this.linesController.LastSelection;
			int index;

			if (sel == null)
			{
				index = this.GetLDefaultArticleInsertionIndex ();
			}
			else
			{
				var info = this.lineInformations[sel.Value];
				index = info.LineIndex+1;
			}

			var quantityColumnEntity = this.SearchArticleQuantityColumnEntity (ArticleQuantityType.Ordered);

			if (quantityColumnEntity != null)
			{
				var model = this.accessData.BusinessDocumentEntity.Lines[index-1];

				var newQuantity = this.accessData.BusinessContext.CreateEntity<ArticleQuantityEntity> ();
				newQuantity.Quantity = 1;
				newQuantity.QuantityColumn = quantityColumnEntity;

				var newLine = this.accessData.BusinessContext.CreateEntity<ArticleDocumentItemEntity> ();
				newLine.GroupIndex = model.GroupIndex;
				newLine.ArticleQuantities.Add (newQuantity);

				this.accessData.BusinessDocumentEntity.Lines.Insert (index, newLine);
				this.UpdateAfterChange (newLine, null);
			}
		}

		[Command (Library.Business.Res.CommandIds.Lines.CreateQuantity)]
		public void ProcessCreateQuantity()
		{
			//	Insère une nouvelle quantité.
			int? sel = this.linesController.LastSelection;

			if (sel != null)
			{
				var info = this.lineInformations[sel.Value];
				var line = this.accessData.BusinessDocumentEntity.Lines[info.LineIndex];

				if (line is ArticleDocumentItemEntity)
				{
					var quantityColumnEntity = this.SearchArticleQuantityColumnEntity (ArticleQuantityType.Delayed);

					if (quantityColumnEntity != null)
					{
						var article = line as ArticleDocumentItemEntity;
						var quantity = article.ArticleQuantities[info.SublineIndex];

						var newQuantity = this.accessData.BusinessContext.CreateEntity<ArticleQuantityEntity> ();
						newQuantity.Quantity = 1;
						newQuantity.Unit = quantity.Unit;
						newQuantity.QuantityColumn = quantityColumnEntity;
						newQuantity.BeginDate = new Date (Date.Today.Ticks + Time.TicksPerDay*7);  // arbitrairement dans une semaine !

						article.ArticleQuantities.Add (newQuantity);

						this.UpdateAfterChange (line, newQuantity);
					}
				}
			}
		}

		[Command (Library.Business.Res.CommandIds.Lines.CreateText)]
		public void ProcessCreateText()
		{
			//	Insère une nouvelle ligne de texte.
			int? sel = this.linesController.LastSelection;
			int index;

			if (sel == null)
			{
				index = this.GetLDefaultArticleInsertionIndex ();
			}
			else
			{
				var info = this.lineInformations[sel.Value];
				index = info.LineIndex+1;
			}

			var model = this.accessData.BusinessDocumentEntity.Lines[index-1];

			var newLine = this.accessData.BusinessContext.CreateEntity<TextDocumentItemEntity> ();
			newLine.GroupIndex = model.GroupIndex;

			this.accessData.BusinessDocumentEntity.Lines.Insert (index, newLine);
			this.UpdateAfterChange (newLine, null);
		}

		[Command (Library.Business.Res.CommandIds.Lines.CreateTitle)]
		public void ProcessCreateTitle()
		{
			//	Insère une nouvelle ligne de titre.
			int? sel = this.linesController.LastSelection;
			int index;

			if (sel == null)
			{
				index = this.GetLDefaultArticleInsertionIndex ();
			}
			else
			{
				var info = this.lineInformations[sel.Value];
				index = info.LineIndex+1;
			}

			var model = this.accessData.BusinessDocumentEntity.Lines[index-1];

			var newLine = this.accessData.BusinessContext.CreateEntity<TextDocumentItemEntity> ();
			newLine.Text = string.Concat (BusinessDocumentLinesController.titlePrefixTags, BusinessDocumentLinesController.titlePostfixTags);
			newLine.GroupIndex = model.GroupIndex;

			this.accessData.BusinessDocumentEntity.Lines.Insert (index, newLine);
			this.UpdateAfterChange (newLine, null);
		}

		[Command (Library.Business.Res.CommandIds.Lines.CreateDiscount)]
		public void ProcessCreateDiscount()
		{
			//	Insère une nouvelle ligne de rabais.
			int? sel = this.linesController.LastSelection;
			int index;

			if (sel == null)
			{
				index = this.GetLDefaultArticleInsertionIndex ();
			}
			else
			{
				var info = this.lineInformations[sel.Value];
				index = info.LineIndex+1;
			}

			var model = this.accessData.BusinessDocumentEntity.Lines[index-1];

			var newLine = this.accessData.BusinessContext.CreateEntity<SubTotalDocumentItemEntity> ();
			newLine.GroupIndex = model.GroupIndex;

			this.accessData.BusinessDocumentEntity.Lines.Insert (index, newLine);
			this.UpdateAfterChange (newLine, null);
		}

		[Command (Library.Business.Res.CommandIds.Lines.CreateTax)]
		public void ProcessCreateTax()
		{
			//	Insère une nouvelle ligne de frais.
			int? sel = this.linesController.LastSelection;
			int index;

			if (sel == null)
			{
				index = this.GetLDefaultArticleInsertionIndex ();
			}
			else
			{
				var info = this.lineInformations[sel.Value];
				index = info.LineIndex+1;
			}

			var newLine = this.accessData.BusinessContext.CreateEntity<ArticleDocumentItemEntity> ();
			newLine.GroupIndex = 0;

			this.accessData.BusinessDocumentEntity.Lines.Insert (index, newLine);
			this.UpdateAfterChange (newLine, null);
		}

		[Command (Library.Business.Res.CommandIds.Lines.CreateGroup)]
		private void ProcessCreateGroup()
		{
			//	Insère un nouveau groupe.
		}

		[Command (Library.Business.Res.CommandIds.Lines.CreateGroupSeparator)]
		private void ProcessCreateGroupSeparator()
		{
			//	Insère un nouveau groupe après le groupe en cours (donc au même niveau).
		}

		[Command (Library.Business.Res.CommandIds.Lines.MoveUp)]
		private void ProcessMoveUp()
		{
			//	Monte la ligne sélectionnée.
			this.ProcessMove (-1);
		}

		[Command (Library.Business.Res.CommandIds.Lines.MoveDown)]
		private void ProcessMoveDown()
		{
			//	Descend la ligne sélectionnée.
			this.ProcessMove (1);
		}

		private void ProcessMove(int direction)
		{
			//	Monte ou descend la ligne sélectionnée.
			int? sel = this.linesController.LastSelection;

			if (sel != null)
			{
				var info = this.lineInformations[sel.Value];
				var line = this.accessData.BusinessDocumentEntity.Lines[info.LineIndex];
				var index = info.LineIndex;

				if (index+direction >= 0 && index+direction < this.accessData.BusinessDocumentEntity.Lines.Count)
				{
					this.accessData.BusinessDocumentEntity.Lines.RemoveAt (index);
					this.accessData.BusinessDocumentEntity.Lines.Insert (index+direction, line);

					this.UpdateAfterChange (line, null);
				}
			}
		}

		[Command (Library.Business.Res.CommandIds.Lines.Duplicate)]
		private void ProcessDuplicate()
		{
			//	Duplique la ligne sélectionnée.
			int? sel = this.linesController.LastSelection;

			if (sel != null)
			{
				var info = this.lineInformations[sel.Value];
				var line = this.accessData.BusinessDocumentEntity.Lines[info.LineIndex];
				var index = info.LineIndex;

				if (index+1 < this.accessData.BusinessDocumentEntity.Lines.Count)
				{
					var copy = line.CloneEntity (this.accessData.BusinessContext);
					this.accessData.BusinessDocumentEntity.Lines.Insert (index+1, copy);

					this.UpdateAfterChange (copy, null);
				}
			}
		}

		[Command (Library.Business.Res.CommandIds.Lines.Delete)]
		private void ProcessDelete()
		{
			//	Supprime la ligne sélectionnée.
			int? sel = this.linesController.LastSelection;

			if (sel != null)
			{
				var info = this.lineInformations[sel.Value];
				var line = this.accessData.BusinessDocumentEntity.Lines[info.LineIndex];

				if (line is ArticleDocumentItemEntity && info.SublineIndex > 0)  // quantité ?
				{
					var article = line as ArticleDocumentItemEntity;
					article.ArticleQuantities.RemoveAt (info.SublineIndex);
				}
				else
				{
					this.accessData.BusinessDocumentEntity.Lines.RemoveAt (info.LineIndex);
				}

				this.UpdateAfterChange (line, null);
			}
		}

		[Command (Library.Business.Res.CommandIds.Lines.Group)]
		private void ProcessGroup()
		{
			//	Groupe toutes les lignes sélectionnées.
		}

		[Command (Library.Business.Res.CommandIds.Lines.Ungroup)]
		private void ProcessUngroup()
		{
			//	Défait le groupe sélectionné.
		}

		[Command (Library.Business.Res.CommandIds.Lines.Split)]
		private void ProcessSplit()
		{
			//	Sépare la ligne d'avec la précédente.
			this.ProcessIndexAdd (1);
		}

		[Command (Library.Business.Res.CommandIds.Lines.Combine)]
		private void ProcessCombine()
		{
			//	Soude la ligne avec la précédente.
			this.ProcessIndexAdd (-1);
		}

		private bool ProcessIndexAdd(int increment)
		{
			int? sel = this.linesController.LastSelection;

			if (sel == null)
			{
				return false;
			}

			var info = this.lineInformations[sel.Value];
			var index = info.LineIndex;
			var line = this.accessData.BusinessDocumentEntity.Lines[index];

			var list = BusinessDocumentLinesController.GroupIndexSplit (line.GroupIndex);
			var level = list.Count-1;

			//	TODO: remplacer le '5' ici par une constante MaxGroupingDepth qui vaudra 4.

			if (list[level]+increment == 0 || list[level]+increment >= 5)  // faut pas pousser (valeur arbitraire de 5 imbrications) !
			{
				return false;
			}

			using (this.accessData.BusinessContext.SuspendUpdates ())
			{
				for (int i = index; i < this.accessData.BusinessDocumentEntity.Lines.Count; i++)
				{
					var item = this.accessData.BusinessDocumentEntity.Lines[i];

					list = BusinessDocumentLinesController.GroupIndexSplit (item.GroupIndex);

					if (list.Count < level+1)
					{
						break;
					}

					list[level] += increment;
					item.GroupIndex = BusinessDocumentLinesController.GroupIndexCombine (list);
				}
			}

			this.UpdateAfterChange (line, null);
			return true;
		}



		private void UpdateAfterChange()
		{
			int? sel = this.linesController.LastSelection;
			AbstractDocumentItemEntity line = null;
			ArticleQuantityEntity quantity = null;

			if (sel != null)
			{
				var info = this.lineInformations[sel.Value];

				line     = info.AbstractDocumentItemEntity;
				quantity = info.ArticleQuantityEntity;
			}

			this.UpdateAfterChange (line, quantity);
		}

		private void UpdateAfterChange(AbstractDocumentItemEntity line, ArticleQuantityEntity quantity)
		{
			this.UpdateLineInformations ();

			int? sel = this.GetLineInformationsIndex (line, quantity);
			this.linesController.UpdateUI (this.lineInformations.Count, this.CallbackGetLineInformations, this.CallbackGetCellContent, sel);
			this.UpdateCommands ();
		}

		private ArticleQuantityColumnEntity SearchArticleQuantityColumnEntity(ArticleQuantityType type)
		{
			var example = new ArticleQuantityColumnEntity ();
			example.QuantityType = type;

			return this.accessData.BusinessContext.DataContext.GetByExample (example).FirstOrDefault ();
		}


		private int? GetLineInformationsIndex(AbstractDocumentItemEntity line, ArticleQuantityEntity quantity)
		{
			for (int i = 0; i < this.lineInformations.Count; i++)
			{
				var info = this.lineInformations[i];

				if (quantity == null)
				{
					if (info.AbstractDocumentItemEntity == line)
					{
						return i;
					}
				}
				else
				{
					if (info.AbstractDocumentItemEntity == line &&
						info.ArticleQuantityEntity == quantity)
					{
						return i;
					}
				}
			}

			return null;
		}

		private int GetLDefaultArticleInsertionIndex()
		{
			for (int i = this.lineInformations.Count-1; i >= 0; i--)
			{
				var info = this.lineInformations[i];

				if (info.AbstractDocumentItemEntity is ArticleDocumentItemEntity ||
					info.AbstractDocumentItemEntity is TextDocumentItemEntity)
				{
					return info.LineIndex+1;
				}
			}

			return 0;
		}

		private void UpdateLineInformations()
		{
			this.lineInformations.Clear ();

			for (int i = 0; i < this.accessData.BusinessDocumentEntity.Lines.Count; i++)
			{
				var line = this.accessData.BusinessDocumentEntity.Lines[i];

				var accessor = new DocumentItemAccessor (this.accessData.BusinessDocumentEntity);
				accessor.BuildContent (line, this.accessData.DocumentMetadataEntity.DocumentCategory.DocumentType, DocumentItemAccessorMode.ForceAllLines|DocumentItemAccessorMode.SpecialQuantitiesToDistinctLines);

				for (int row = 0; row < accessor.RowsCount; row++)
				{
					this.lineInformations.Add (new LineInformations (line, accessor, i, row));
				}
			}
		}


		private static List<int> GroupIndexSplit(int groupIndex)
		{
			//	30201 retourne la liste 1,2,3.
			var list = new List<int> ();

			while (groupIndex != 0)
			{
				list.Add (groupIndex%100);
				groupIndex /= 100;
			}


			return list;
		}

		private static int GroupIndexCombine(List<int> list)
		{
			//	La liste 1,2,3 retourne 30201.
			int groupIndex = 0;
			int factor = 1;

			foreach (var n in list)
			{
				groupIndex += factor * n;
				factor *= 100;
			}

			return groupIndex;
		}


		private void UpdateCommands()
		{
			var selection     = this.linesController.Selection;
			var lastSelection = this.linesController.LastSelection;
			var isCoherentSelection = this.IsCoherentSelection;

			LineInformations info = null;
			bool autoGenerated = false;

			if (lastSelection != null)
			{
				info = this.lineInformations[lastSelection.Value];
				autoGenerated = info.AbstractDocumentItemEntity.Attributes.HasFlag (DocumentItemAttributes.AutoGenerated);
			}

			bool isArticle  = info != null && info.AbstractDocumentItemEntity is ArticleDocumentItemEntity;
			bool isText     = info != null && info.AbstractDocumentItemEntity is TextDocumentItemEntity;
			bool isTax      = info != null && info.AbstractDocumentItemEntity is TaxDocumentItemEntity;
			bool isSubTotal = info != null && info.AbstractDocumentItemEntity is SubTotalDocumentItemEntity;
			bool isEndTotal = info != null && info.AbstractDocumentItemEntity is EndTotalDocumentItemEntity;

			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateArticle,        true);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateText,           true);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateTitle,          true);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateDiscount,       true);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateTax,            true);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateQuantity,       isArticle);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateGroup,          false);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.CreateGroupSeparator, false);

			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.MoveUp,    selection.Count == 1 && !autoGenerated);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.MoveDown,  selection.Count == 1 && !autoGenerated);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Duplicate, selection.Count == 1 && !autoGenerated);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Delete,    selection.Count == 1 && !autoGenerated);

			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Group,     isCoherentSelection  && !autoGenerated);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Ungroup,   isCoherentSelection  && !autoGenerated);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Split,     selection.Count == 1 && !autoGenerated && lastSelection != 0);
			this.commandContext.SetLocalEnable (Library.Business.Res.Commands.Lines.Combine,   selection.Count == 1 && !autoGenerated && lastSelection != 0);
		}

		private bool IsCoherentSelection
		{
			get
			{
				var selection = this.linesController.Selection;

				if (selection.Count == 0)
				{
					return false;
				}

				var groupIndex = -1;
				foreach (var sel in selection)
				{
					var info = this.lineInformations[sel];

					if (groupIndex == -1)
					{
						groupIndex = info.AbstractDocumentItemEntity.GroupIndex;
					}
					else
					{
						if (groupIndex != info.AbstractDocumentItemEntity.GroupIndex)
						{
							return false;
						}
					}
				}

				return true;
			}
		}


		public static readonly string titlePrefixTags  = "<font size=\"150%\"><b>";
		public static readonly string titlePostfixTags = "</b></font>";

		private readonly AccessData						accessData;
		private readonly List<LineInformations>			lineInformations;
		private readonly CommandContext					commandContext;
		private readonly CommandDispatcher				commandDispatcher;

		private LineToolbarController					lineToolbarController;
		private LinesController							linesController;
		private LineEditorController					lineEditorController;
	}
}
