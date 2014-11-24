//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.App.Views.ToolbarControllers
{
	public class MethodsToolbarTreeTableController : AbstractToolbarBothTreesController<SortableNode>, IDirty, System.IDisposable
	{
		public MethodsToolbarTreeTableController(DataAccessor accessor, CommandContext commandContext, BaseType baseType)
			: base (accessor, commandContext, baseType)
		{
			this.title = AbstractView.GetViewTitle (this.accessor, ViewType.Methods);

			this.primaryGetter = this.accessor.GetNodeGetter (BaseType.Methods);
			this.secondaryGetter = new SortableNodeGetter (this.primaryGetter, this.accessor, BaseType.Methods);
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
			this.toolbar = new MethodsToolbar (this.accessor, this.commandContext);
			this.ConnectSearch ();
		}

		protected override void CreateNodeFiller()
		{
			this.dataFiller = new MethodsTreeTableFiller (this.accessor, this.nodeGetter)
			{
				Title = this.title,
			};

			TreeTableFiller<SortableNode>.FillColumns (this.treeTableController, this.dataFiller, "View.Expression");

			this.sortingInstructions = TreeTableFiller<SortableNode>.GetSortingInstructions (this.treeTableController);
		}


		[Command (Res.CommandIds.Methods.First)]
		protected override void OnFirst()
		{
			base.OnFirst ();
		}

		[Command (Res.CommandIds.Methods.Prev)]
		protected override void OnPrev()
		{
			base.OnPrev ();
		}

		[Command (Res.CommandIds.Methods.Next)]
		protected override void OnNext()
		{
			base.OnNext ();
		}

		[Command (Res.CommandIds.Methods.Last)]
		protected override void OnLast()
		{
			base.OnLast ();
		}

		[Command (Res.CommandIds.Methods.Deselect)]
		protected void OnDeselect()
		{
			this.VisibleSelectedRow = -1;
		}

		[Command (Res.CommandIds.Methods.New)]
		protected void OnNew(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);
			this.ShowCreatePopup (target);
		}

		[Command (Res.CommandIds.Methods.Delete)]
		protected void OnDelete(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);

			var name = MethodsLogic.GetSummary (this.accessor, this.SelectedGuid);
			var question = string.Format (Res.Strings.ToolbarControllers.MethodsTreeTable.DeleteQuestion.ToString (), name);

			YesNoPopup.Show (target, question, delegate
			{
				this.accessor.UndoManager.Start ();
				var desc = UndoManager.GetDescription (Res.Commands.Methods.Delete.Description, MethodsLogic.GetSummary (this.accessor, this.SelectedGuid));
				this.accessor.UndoManager.SetDescription (desc);

				this.accessor.RemoveObject (BaseType.Methods, this.SelectedGuid);
				this.UpdateData ();
				this.OnUpdateAfterDelete ();

				this.accessor.UndoManager.SetAfterViewState ();
			});
		}

		[Command (Res.CommandIds.Methods.Copy)]
		protected override void OnCopy(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			base.OnCopy (dispatcher, e);
		}

		[Command (Res.CommandIds.Methods.Paste)]
		protected override void OnPaste(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.accessor.UndoManager.Start ();

			base.OnPaste (dispatcher, e);

			var desc = UndoManager.GetDescription (Res.Commands.Methods.Paste.Description, MethodsLogic.GetSummary (this.accessor, this.SelectedGuid));
			this.accessor.UndoManager.SetDescription (desc);
			this.accessor.UndoManager.SetAfterViewState ();
		}

		[Command (Res.CommandIds.Methods.Export)]
		protected override void OnExport(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			base.OnExport (dispatcher, e);
		}


		protected override void ShowContextMenu(Point pos)
		{
			//	Affiche le menu contextuel.
			MenuPopup.Show (this.toolbar, this.treeTableFrame, pos,
				Res.Commands.Methods.New,
				Res.Commands.Methods.Delete,
				null,
				Res.Commands.Methods.Copy,
				Res.Commands.Methods.Paste);
		}


		private void ShowCreatePopup(Widget target)
		{
			var popup = new CreateMethodPopup (this.accessor);

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
			var guid = this.accessor.CreateObject (BaseType.Methods, date, Guid.Empty, p1);

			var obj = this.accessor.GetObject (BaseType.Methods, guid);
			System.Diagnostics.Debug.Assert (obj != null);

			this.UpdateData ();

			this.SelectedGuid = guid;
			this.OnUpdateAfterCreate (guid, EventType.Input, Timestamp.Now);  // Timestamp quelconque !

			var desc = UndoManager.GetDescription (Res.Commands.Methods.New.Description, MethodsLogic.GetSummary (this.accessor, guid));
			this.accessor.UndoManager.SetDescription (desc);
			this.accessor.UndoManager.SetAfterViewState ();
		}


		protected override void UpdateToolbar()
		{
			base.UpdateToolbar ();

			int row = this.VisibleSelectedRow;

			this.UpdateSelCommand (Res.Commands.Methods.First, row, this.FirstRowIndex);
			this.UpdateSelCommand (Res.Commands.Methods.Prev,  row, this.PrevRowIndex);
			this.UpdateSelCommand (Res.Commands.Methods.Next,  row, this.NextRowIndex);
			this.UpdateSelCommand (Res.Commands.Methods.Last,  row, this.LastRowIndex);

			this.toolbar.SetEnable (Res.Commands.Methods.New,      true);
			this.toolbar.SetEnable (Res.Commands.Methods.Delete,   row != -1);
			this.toolbar.SetEnable (Res.Commands.Methods.Deselect, row != -1);

			this.toolbar.SetEnable (Res.Commands.Methods.Copy,   this.IsCopyEnable);
			this.toolbar.SetEnable (Res.Commands.Methods.Paste,  this.accessor.Clipboard.HasObject (this.baseType));
			this.toolbar.SetEnable (Res.Commands.Methods.Export, !this.IsEmpty);
		}


		private GuidNodeGetter					primaryGetter;
		private SortableNodeGetter				secondaryGetter;
	}
}
