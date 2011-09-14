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
	public sealed partial class BusinessDocumentLinesController
	{
		private class CommandProcessor
		{
			public CommandProcessor(BusinessDocumentLinesController host)
			{
				this.host = host;
			}


			[Command (Library.Business.Res.CommandIds.Lines.Deselect)]
			private void ProcessDeselect()
			{
				//	Désélectionne toutes les lignes.
				this.host.lineTableController.DeselectAll ();
			}

			[Command (Library.Business.Res.CommandIds.Lines.GroupSelect)]
			private void ProcessGroupSelect()
			{
				//	Sélectionne toutes les lignes du groupe.
				var selection = this.host.Selection;

				if (selection.Count == 0)
				{
					return;
				}

				int groupIndex = selection[0].DocumentItem.GroupIndex;
				int level = AbstractDocumentItemEntity.GetGroupLevel (groupIndex);

				if (level == 0)
				{
					return;
				}

				selection = new List<LineInformations> ();

				foreach (var info in this.host.lineInformations)
				{
					if (AbstractDocumentItemEntity.GroupCompare (groupIndex, info.DocumentItem.GroupIndex, level))
					{
						selection.Add (info);
					}
				}

				this.host.Selection = selection;
			}

			[Command (Library.Business.Res.CommandIds.Lines.ViewCompact)]
			private void ProcessViewCompact()
			{
				this.host.CurrentViewMode = ViewMode.Compact;
				this.host.UpdateAfterChange ();
			}

			[Command (Library.Business.Res.CommandIds.Lines.ViewDefault)]
			private void ProcessViewDefault()
			{
				this.host.CurrentViewMode = ViewMode.Default;
				this.host.UpdateAfterChange ();
			}

			[Command (Library.Business.Res.CommandIds.Lines.ViewFull)]
			private void ProcessViewFull()
			{
				this.host.CurrentViewMode = ViewMode.Full;
				this.host.UpdateAfterChange ();
			}

			[Command (Library.Business.Res.CommandIds.Lines.ViewDebug)]
			private void ProcessViewDebug()
			{
				this.host.CurrentViewMode = ViewMode.Debug;
				this.host.UpdateAfterChange ();
			}

			[Command (Library.Business.Res.CommandIds.Lines.EditName)]
			private void ProcessEditName()
			{
				this.host.CurrentEditMode = EditMode.Name;
				this.host.UpdateAfterChange ();
				this.host.HandleLinesControllerSelectionChanged (this);
			}

			[Command (Library.Business.Res.CommandIds.Lines.EditDescription)]
			private void ProcessEditDescription()
			{
				this.host.CurrentEditMode = EditMode.Description;
				this.host.UpdateAfterChange ();
				this.host.HandleLinesControllerSelectionChanged (this);
			}

			[Command (Library.Business.Res.CommandIds.Lines.CreateArticle)]
			private void ProcessCreateArticle()
			{
				var selection = this.host.linesEngine.CreateArticle (this.host.Selection, isTax: false);
				this.host.UpdateAfterChange (this.host.linesEngine.LastError, selection);
			}

			[Command (Library.Business.Res.CommandIds.Lines.CreateQuantity)]
			private void ProcessCreateQuantity()
			{
				//	Insère une nouvelle quantité.
				var selection = this.host.linesEngine.CreateQuantity (this.host.Selection);
				this.host.UpdateAfterChange (this.host.linesEngine.LastError, selection);
			}

			[Command (Library.Business.Res.CommandIds.Lines.CreateText)]
			private void ProcessCreateText()
			{
				//	Insère une nouvelle ligne de texte.
				var selection = this.host.linesEngine.CreateText (this.host.Selection, isTitle: false);
				this.host.UpdateAfterChange (this.host.linesEngine.LastError, selection);
			}

			[Command (Library.Business.Res.CommandIds.Lines.CreateTitle)]
			private void ProcessCreateTitle()
			{
				//	Insère une nouvelle ligne de titre.
				var selection = this.host.linesEngine.CreateText (this.host.Selection, isTitle: true);
				this.host.UpdateAfterChange (selection);
			}

			[Command (Library.Business.Res.CommandIds.Lines.CreateDiscount)]
			private void ProcessCreateDiscount()
			{
				//	Insère une nouvelle ligne de rabais.
				var selection = this.host.linesEngine.CreateDiscount (this.host.Selection);
				this.host.UpdateAfterChange (this.host.linesEngine.LastError, selection);
			}

			[Command (Library.Business.Res.CommandIds.Lines.CreateTax)]
			private void ProcessCreateTax()
			{
				//	Insère une nouvelle ligne de frais.
				var selection = this.host.linesEngine.CreateArticle (this.host.Selection, isTax: true);
				this.host.UpdateAfterChange (this.host.linesEngine.LastError, selection);
			}

			[Command (Library.Business.Res.CommandIds.Lines.CreateGroup)]
			private void ProcessCreateGroup()
			{
				//	Insère un nouveau groupe contenant un titre.
				var selection = this.host.linesEngine.CreateGroup (this.host.Selection);
				this.host.UpdateAfterChange (this.host.linesEngine.LastError, selection);
			}

			[Command (Library.Business.Res.CommandIds.Lines.MoveUp)]
			private void ProcessMoveUp()
			{
				//	Monte la ligne sélectionnée.
				this.host.linesEngine.Move (this.host.Selection, -1);
				this.host.UpdateAfterChange (this.host.linesEngine.LastError);
			}

			[Command (Library.Business.Res.CommandIds.Lines.MoveDown)]
			private void ProcessMoveDown()
			{
				//	Descend la ligne sélectionnée.
				this.host.linesEngine.Move (this.host.Selection, 1);
				this.host.UpdateAfterChange (this.host.linesEngine.LastError);
			}

			[Command (Library.Business.Res.CommandIds.Lines.Duplicate)]
			private void ProcessDuplicate()
			{
				//	Duplique la ligne sélectionnée.
				var selection = this.host.linesEngine.Duplicate (this.host.Selection);
				this.host.UpdateAfterChange (this.host.linesEngine.LastError, selection);
			}

			[Command (Library.Business.Res.CommandIds.Lines.Delete)]
			private void ProcessDelete()
			{
				//	Supprime la ligne sélectionnée.
				var selection = this.host.linesEngine.Delete (this.host.Selection);
				this.host.UpdateAfterChange (this.host.linesEngine.LastError, selection);
			}

			[Command (Library.Business.Res.CommandIds.Lines.Group)]
			private void ProcessGroup()
			{
				//	Groupe toutes les lignes sélectionnées.
				this.host.linesEngine.MakeGroup (this.host.Selection);
				this.host.UpdateAfterChange (this.host.linesEngine.LastError);
			}

			[Command (Library.Business.Res.CommandIds.Lines.Ungroup)]
			private void ProcessUngroup()
			{
				//	Défait le groupe sélectionné.
				this.host.linesEngine.MakeUngroup (this.host.Selection);
				this.host.UpdateAfterChange (this.host.linesEngine.LastError);
			}

			[Command (Library.Business.Res.CommandIds.Lines.Split)]
			private void ProcessSplit()
			{
				//	Sépare la ligne d'avec la précédente.
				this.host.linesEngine.MakeSplit (this.host.Selection);
				this.host.UpdateAfterChange (this.host.linesEngine.LastError);
			}

			[Command (Library.Business.Res.CommandIds.Lines.Combine)]
			private void ProcessCombine()
			{
				//	Soude la ligne avec la précédente.
				this.host.linesEngine.MakeCombine (this.host.Selection);
				this.host.UpdateAfterChange (this.host.linesEngine.LastError);
			}

			[Command (Library.Business.Res.CommandIds.Lines.Flat)]
			private void ProcessFlat()
			{
				//	Remet à plat toutes les lignes.
				this.host.linesEngine.MakeFlat (this.host.Selection);
				this.host.UpdateAfterChange (this.host.linesEngine.LastError);
			}

			private readonly BusinessDocumentLinesController host;
		}
	}
}
