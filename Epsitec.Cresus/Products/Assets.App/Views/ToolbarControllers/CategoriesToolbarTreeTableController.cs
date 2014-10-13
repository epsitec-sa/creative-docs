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
	public class CategoriesToolbarTreeTableController : AbstractToolbarBothTreesController<SortableNode>, IDirty, System.IDisposable
	{
		public CategoriesToolbarTreeTableController(DataAccessor accessor, CommandContext commandContext, BaseType baseType)
			: base (accessor, commandContext, baseType)
		{
			this.title = AbstractView.GetViewTitle (this.accessor, ViewType.Categories);

			var primary = this.accessor.GetNodeGetter (BaseType.Categories);
			this.secondaryGetter = new SortableNodeGetter (primary, this.accessor, BaseType.Categories);
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
			this.toolbar = new CategoriesToolbar (this.accessor, this.commandContext);
			this.ConnectSearch ();
		}

		protected override void CreateNodeFiller()
		{
			this.dataFiller = new CategoriesTreeTableFiller (this.accessor, this.nodeGetter)
			{
				Title = this.title,
			};

			TreeTableFiller<SortableNode>.FillColumns (this.treeTableController, this.dataFiller, "View.Categories");

			this.sortingInstructions = TreeTableFiller<SortableNode>.GetSortingInstructions (this.treeTableController);
		}


		[Command (Res.CommandIds.Categories.First)]
		protected override void OnFirst()
		{
			base.OnFirst ();
		}

		[Command (Res.CommandIds.Categories.Prev)]
		protected override void OnPrev()
		{
			base.OnPrev ();
		}

		[Command (Res.CommandIds.Categories.Next)]
		protected override void OnNext()
		{
			base.OnNext ();
		}

		[Command (Res.CommandIds.Categories.Last)]
		protected override void OnLast()
		{
			base.OnLast ();
		}

		[Command (Res.CommandIds.Categories.Deselect)]
		protected void OnDeselect()
		{
			this.VisibleSelectedRow = -1;
		}

		[Command (Res.CommandIds.Categories.New)]
		protected void OnNew(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);
			this.ShowCreatePopup (target);
		}

		[Command (Res.CommandIds.Categories.Delete)]
		protected void OnDelete(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);

			var name = CategoriesLogic.GetSummary (this.accessor, this.SelectedGuid);
			var question = string.Format (Res.Strings.ToolbarControllers.CategoriesTreeTable.DeleteQuestion.ToString (), name);

			YesNoPopup.Show (target, question, delegate
			{
				this.accessor.RemoveObject (BaseType.Categories, this.SelectedGuid);
				this.UpdateData ();
				this.OnUpdateAfterDelete ();
			});
		}

		[Command (Res.CommandIds.Categories.Copy)]
		protected override void OnCopy(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			base.OnCopy (dispatcher, e);
		}

		[Command (Res.CommandIds.Categories.Paste)]
		protected override void OnPaste(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			base.OnPaste (dispatcher, e);
		}

		[Command (Res.CommandIds.Categories.Export)]
		protected override void OnExport(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			base.OnExport (dispatcher, e);
		}


		protected override void ShowContextMenu(Point pos)
		{
			//	Affiche le menu contextuel.
			MenuPopup.Show (this.toolbar, this.treeTableFrame, pos,
				Res.Commands.Categories.New,
				Res.Commands.Categories.Delete,
				null,
				Res.Commands.Categories.Copy,
				Res.Commands.Categories.Paste);
		}


		private void ShowCreatePopup(Widget target)
		{
			var popup = new CreateCategoryPopup (this.accessor)
			{
				ObjectModel = this.SelectedGuid,
			};

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					this.CreateObject (popup.ObjectName, popup.ObjectModel);
				}
			};
		}

		private void CreateObject(string name, Guid model)
		{
			var date = this.accessor.Mandat.StartDate;

			var p1 = new DataStringProperty  (ObjectField.Name,             name);
			var p2 = new DataIntProperty     (ObjectField.AmortizationType, (int) AmortizationType.Linear);
			var p3 = new DataIntProperty     (ObjectField.Periodicity,      (int) Periodicity.Annual);
			var p4 = new DataIntProperty     (ObjectField.Prorata,          (int) ProrataType.None);
			var p5 = new DataDecimalProperty (ObjectField.Round,            1.0m);
			var p6 = new DataDecimalProperty (ObjectField.ResidualValue,    1.0m);

			var guid = this.accessor.CreateObject (BaseType.Categories, date, Guid.Empty, p1, p2, p3, p4, p5, p6);

			var obj = this.accessor.GetObject (BaseType.Categories, guid);
			System.Diagnostics.Debug.Assert (obj != null);

			if (!model.IsEmpty)
			{
				var objModel = this.accessor.GetObject (BaseType.Categories, model);
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

			this.UpdateSelCommand (Res.Commands.Categories.First, row, this.FirstRowIndex);
			this.UpdateSelCommand (Res.Commands.Categories.Prev,  row, this.PrevRowIndex);
			this.UpdateSelCommand (Res.Commands.Categories.Next,  row, this.NextRowIndex);
			this.UpdateSelCommand (Res.Commands.Categories.Last,  row, this.LastRowIndex);

			this.toolbar.SetEnable (Res.Commands.Categories.New,      true);
			this.toolbar.SetEnable (Res.Commands.Categories.Delete,   row != -1);
			this.toolbar.SetEnable (Res.Commands.Categories.Deselect, row != -1);

			this.toolbar.SetEnable (Res.Commands.Categories.Copy,   this.IsCopyEnable);
			this.toolbar.SetEnable (Res.Commands.Categories.Paste,  this.accessor.Clipboard.HasObject (this.baseType));
			this.toolbar.SetEnable (Res.Commands.Categories.Export, !this.IsEmpty);
		}


		private SortableNodeGetter secondaryGetter;
	}
}
