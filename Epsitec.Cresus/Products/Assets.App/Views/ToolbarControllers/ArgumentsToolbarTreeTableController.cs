//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.App.Views.ToolbarControllers
{
	public class ArgumentsToolbarTreeTableController : AbstractToolbarBothTreesController<SortableNode>, IDirty, System.IDisposable
	{
		public ArgumentsToolbarTreeTableController(DataAccessor accessor, CommandContext commandContext, BaseType baseType)
			: base (accessor, commandContext, baseType)
		{
			this.title = AbstractView.GetViewTitle (this.accessor, ViewType.Arguments);

			this.primaryGetter = this.accessor.GetNodeGetter (BaseType.Arguments);
			this.secondaryGetter = new SortableNodeGetter (this.primaryGetter, this.accessor, BaseType.Arguments);
			this.nodeGetter = new SorterNodeGetter (this.secondaryGetter);
		}

		public override void Dispose()
		{
			base.Dispose ();
		}


		#region IDirty Members
		public bool DirtyData
		{
			get;
			set;
		}
		#endregion


		public override void UpdateData()
		{
			this.primaryGetter.Update ();
			this.secondaryGetter.SetParams (null, this.sortingInstructions);
			(this.nodeGetter as SorterNodeGetter).SetParams (this.sortingInstructions);

			this.UpdateController ();
			this.UpdateToolbar ();
		}


		public override Guid					SelectedGuid
		{
			//	Retourne le Guid de l'objet actuellement sélectionné.
			get
			{
				int sel = this.VisibleSelectedRow;
				if (sel != -1 && sel < this.nodeGetter.Count)
				{
					return this.nodeGetter[sel].Guid;
				}
				else
				{
					return Guid.Empty;
				}
			}
			//	Sélectionne l'objet ayant un Guid donné.
			set
			{
				this.VisibleSelectedRow = this.nodeGetter.GetNodes ().ToList ().FindIndex (x => x.Guid == value);
			}
		}


		protected override void CreateToolbar()
		{
			this.toolbar = new ArgumentsToolbar (this.accessor, this.commandContext);
			this.ConnectSearch ();
		}

		protected override void CreateNodeFiller()
		{
			this.dataFiller = new ArgumentsTreeTableFiller (this.accessor, this.nodeGetter)
			{
				Title = this.title,
			};

			TreeTableFiller<SortableNode>.FillColumns (this.treeTableController, this.dataFiller, "View.Expression");

			this.sortingInstructions = TreeTableFiller<SortableNode>.GetSortingInstructions (this.treeTableController);
		}


		[Command (Res.CommandIds.Arguments.First)]
		protected override void OnFirst()
		{
			base.OnFirst ();
		}

		[Command (Res.CommandIds.Arguments.Prev)]
		protected override void OnPrev()
		{
			base.OnPrev ();
		}

		[Command (Res.CommandIds.Arguments.Next)]
		protected override void OnNext()
		{
			base.OnNext ();
		}

		[Command (Res.CommandIds.Arguments.Last)]
		protected override void OnLast()
		{
			base.OnLast ();
		}

		[Command (Res.CommandIds.Arguments.Deselect)]
		protected void OnDeselect()
		{
			this.VisibleSelectedRow = -1;
		}

		[Command (Res.CommandIds.Arguments.New)]
		protected void OnNew(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);
			this.ShowCreatePopup (target);
		}

		[Command (Res.CommandIds.Arguments.Delete)]
		protected void OnDelete(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);

			var assetsCount = ArgumentsLogic.GetReferencedMethods (this.accessor, this.SelectedGuid).Count ();
			if (assetsCount > 0)
			{
				//?string message = string.Format (Res.Strings.ToolbarControllers.PersonsTreeTable.DeleteError.ToString (), assetsCount);
				string message = string.Format ("Il y a {0} méthodes qui référencent cet argument. Il ne peut pas être supprimé", assetsCount);
				MessagePopup.ShowError (target, message);
				return;
			}

			var name = ArgumentsLogic.GetSummary (this.accessor, this.SelectedGuid);
			var question = string.Format (Res.Strings.ToolbarControllers.ArgumentsTreeTable.DeleteQuestion.ToString (), name);

			YesNoPopup.Show (target, question, delegate
			{
				this.accessor.UndoManager.Start ();
				var desc = UndoManager.GetDescription (Res.Commands.Arguments.Delete.Description, ArgumentsLogic.GetSummary (this.accessor, this.SelectedGuid));
				this.accessor.UndoManager.SetDescription (desc);

				this.accessor.RemoveObject (BaseType.Arguments, this.SelectedGuid);
				this.UpdateData ();
				this.OnUpdateAfterDelete ();

				this.accessor.UndoManager.SetAfterViewState ();
			});
		}

		[Command (Res.CommandIds.Arguments.Copy)]
		protected override void OnCopy(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			base.OnCopy (dispatcher, e);
		}

		[Command (Res.CommandIds.Arguments.Paste)]
		protected override void OnPaste(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.accessor.UndoManager.Start ();

			base.OnPaste (dispatcher, e);

			var desc = UndoManager.GetDescription (Res.Commands.Arguments.Paste.Description, ArgumentsLogic.GetSummary (this.accessor, this.SelectedGuid));
			this.accessor.UndoManager.SetDescription (desc);
			this.accessor.UndoManager.SetAfterViewState ();
		}

		[Command (Res.CommandIds.Arguments.Export)]
		protected override void OnExport(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			base.OnExport (dispatcher, e);
		}


		protected override void ShowContextMenu(Point pos)
		{
			//	Affiche le menu contextuel.
			MenuPopup.Show (this.toolbar, this.treeTableFrame, pos,
				Res.Commands.Arguments.New,
				Res.Commands.Arguments.Delete,
				null,
				Res.Commands.Arguments.Copy,
				Res.Commands.Arguments.Paste);
		}


		private void ShowCreatePopup(Widget target)
		{
			var popup = new CreateArgumentPopup (this.accessor);

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					this.CreateObject (popup.ObjectName);
				}
			};
		}

		private void CreateObject(string name)
		{
			this.accessor.UndoManager.Start ();

			var date = this.accessor.Mandat.StartDate;
			var p1 = new DataStringProperty (ObjectField.Name, name);
			var p2 = new DataIntProperty (ObjectField.ArgumentType, (int) ArgumentType.Amount);
			var p3 = new DataIntProperty (ObjectField.ArgumentField, (int) ArgumentsLogic.GetUnusedField (this.accessor));
			var guid = this.accessor.CreateObject (BaseType.Arguments, date, Guid.Empty, p1, p2, p3);

			var obj = this.accessor.GetObject (BaseType.Arguments, guid);
			System.Diagnostics.Debug.Assert (obj != null);

			this.UpdateData ();

			this.SelectedGuid = guid;
			this.OnUpdateAfterCreate (guid, EventType.Input, Timestamp.Now);  // Timestamp quelconque !

			var desc = UndoManager.GetDescription (Res.Commands.Arguments.New.Description, ArgumentsLogic.GetSummary (this.accessor, guid));
			this.accessor.UndoManager.SetDescription (desc);
			this.accessor.UndoManager.SetAfterViewState ();
		}


		protected override void UpdateToolbar()
		{
			base.UpdateToolbar ();

			int row = this.VisibleSelectedRow;

			this.UpdateSelCommand (Res.Commands.Arguments.First, row, this.FirstRowIndex);
			this.UpdateSelCommand (Res.Commands.Arguments.Prev,  row, this.PrevRowIndex);
			this.UpdateSelCommand (Res.Commands.Arguments.Next,  row, this.NextRowIndex);
			this.UpdateSelCommand (Res.Commands.Arguments.Last,  row, this.LastRowIndex);

			this.toolbar.SetEnable (Res.Commands.Arguments.New,      true);
			this.toolbar.SetEnable (Res.Commands.Arguments.Delete,   row != -1);
			this.toolbar.SetEnable (Res.Commands.Arguments.Deselect, row != -1);

			this.toolbar.SetEnable (Res.Commands.Arguments.Copy,   this.IsCopyEnable);
			this.toolbar.SetEnable (Res.Commands.Arguments.Paste,  this.accessor.Clipboard.HasObject (this.baseType));
			this.toolbar.SetEnable (Res.Commands.Arguments.Export, !this.IsEmpty);
		}


		private GuidNodeGetter					primaryGetter;
		private SortableNodeGetter				secondaryGetter;
	}
}
