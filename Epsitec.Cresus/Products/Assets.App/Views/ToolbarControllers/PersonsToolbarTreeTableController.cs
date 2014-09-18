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

namespace Epsitec.Cresus.Assets.App.Views.ToolbarControllers
{
	public class PersonsToolbarTreeTableController : AbstractToolbarBothTreesController<SortableNode>, IDirty, System.IDisposable
	{
		public PersonsToolbarTreeTableController(DataAccessor accessor, CommandContext commandContext, BaseType baseType)
			: base (accessor, commandContext, baseType)
		{
			this.title = AbstractView.GetViewTitle (this.accessor, ViewType.Persons);

			var primary = this.accessor.GetNodeGetter (BaseType.Persons);
			this.secondaryGetter = new SortableNodeGetter (primary, this.accessor, BaseType.Persons);
			this.nodeGetter = new SorterNodeGetter (this.secondaryGetter);
		}

		public override void Dispose()
		{
			base.Dispose ();
		}


		#region IDirty Members
		public bool InUse
		{
			get;
			set;
		}

		public bool DirtyData
		{
			get;
			set;
		}
		#endregion


		public override void UpdateData()
		{
			this.secondaryGetter.SetParams (null, this.sortingInstructions);
			(this.nodeGetter as SorterNodeGetter).SetParams (this.sortingInstructions);

			this.UpdateController ();
			this.UpdateToolbar ();
		}


		public override Guid SelectedGuid
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
			this.toolbar = new PersonsToolbar (this.accessor, this.commandContext);
		}

		protected override void CreateNodeFiller()
		{
			this.dataFiller = new PersonsTreeTableFiller (this.accessor, this.nodeGetter)
			{
				Title = this.title,
			};

			TreeTableFiller<SortableNode>.FillColumns (this.treeTableController, this.dataFiller, "View.Persons");

			this.sortingInstructions = TreeTableFiller<SortableNode>.GetSortingInstructions (this.treeTableController);
		}


		[Command (Res.CommandIds.Persons.First)]
		protected override void OnFirst()
		{
			base.OnFirst ();
		}

		[Command (Res.CommandIds.Persons.Prev)]
		protected override void OnPrev()
		{
			base.OnPrev ();
		}

		[Command (Res.CommandIds.Groups.Next)]
		protected override void OnNext()
		{
			base.OnNext ();
		}

		[Command (Res.CommandIds.Persons.Last)]
		protected override void OnLast()
		{
			base.OnLast ();
		}

		[Command (Res.CommandIds.Persons.Deselect)]
		protected void OnDeselect()
		{
			this.VisibleSelectedRow = -1;
		}

		[Command (Res.CommandIds.Persons.New)]
		protected void OnNew(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);
			this.ShowCreatePopup (target);
		}

		[Command (Res.CommandIds.Persons.Delete)]
		protected void OnDelete(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);

			YesNoPopup.Show (target, Res.Strings.ToolbarControllers.PersonsTreeTable.DeleteQuestion.ToString (), delegate
			{
				this.accessor.RemoveObject (BaseType.Persons, this.SelectedGuid);
				this.UpdateData ();
				this.OnUpdateAfterDelete ();
			});
		}

		[Command (Res.CommandIds.Persons.Copy)]
		protected override void OnCopy(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			base.OnCopy (dispatcher, e);
		}

		[Command (Res.CommandIds.Persons.Paste)]
		protected override void OnPaste(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			base.OnPaste (dispatcher, e);
		}

		[Command (Res.CommandIds.Persons.Export)]
		protected override void OnExport(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			base.OnExport (dispatcher, e);
		}


		protected override void ShowContextMenu(Point pos)
		{
			//	Affiche le menu contextuel.
			MenuPopup.Show (this.toolbar, this.treeTableFrame, pos,
				Res.Commands.Persons.New,
				Res.Commands.Persons.Delete,
				null,
				Res.Commands.Persons.Copy,
				Res.Commands.Persons.Paste);
		}


		private void ShowCreatePopup(Widget target)
		{
			var popup = new CreatePersonPopup (this.accessor);

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					this.CreateObject (popup.PersonName, popup.PersonModel);
				}
			};
		}

		private void CreateObject(string name, Guid model)
		{
			var date = this.accessor.Mandat.StartDate;
			var guid = this.accessor.CreateObject (BaseType.Persons, date, name, Guid.Empty, addDefaultGroups: false);
			var obj = this.accessor.GetObject (BaseType.Persons, guid);
			System.Diagnostics.Debug.Assert (obj != null);

			if (!model.IsEmpty)
			{
				var objModel = this.accessor.GetObject (BaseType.Persons, model);
				this.accessor.CopyObject (obj, objModel);
			}

			this.UpdateData ();

			this.SelectedGuid = guid;
			this.OnUpdateAfterCreate (guid, EventType.Input, Timestamp.Now);  // Timestamp quelconque !
		}


		protected override void UpdateToolbar()
		{
			base.UpdateToolbar ();

			int row = this.VisibleSelectedRow;

			this.UpdateSelCommand (Res.Commands.Persons.First, row, this.FirstRowIndex);
			this.UpdateSelCommand (Res.Commands.Persons.Prev,  row, this.PrevRowIndex);
			this.UpdateSelCommand (Res.Commands.Persons.Next,  row, this.NextRowIndex);
			this.UpdateSelCommand (Res.Commands.Persons.Last,  row, this.LastRowIndex);

			this.toolbar.SetEnable (Res.Commands.Persons.New,      true);
			this.toolbar.SetEnable (Res.Commands.Persons.Delete,   row != -1);
			this.toolbar.SetEnable (Res.Commands.Persons.Deselect, row != -1);

			this.toolbar.SetEnable (Res.Commands.Persons.Copy,   this.IsCopyEnable);
			this.toolbar.SetEnable (Res.Commands.Persons.Paste,  this.accessor.Clipboard.HasObject (this.baseType));
			this.toolbar.SetEnable (Res.Commands.Persons.Export, true);
		}


		private SortableNodeGetter secondaryGetter;
	}
}
