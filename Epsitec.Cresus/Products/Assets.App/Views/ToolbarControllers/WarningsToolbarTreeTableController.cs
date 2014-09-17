//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Cresus.Assets.App.DataFillers;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Views.EditorPages;
using Epsitec.Cresus.Assets.App.Views.ViewStates;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.ToolbarControllers
{
	public class WarningsToolbarTreeTableController : AbstractToolbarBothTreesController<Warning>, IDirty, System.IDisposable
	{
		public WarningsToolbarTreeTableController(DataAccessor accessor, CommandContext commandContext, BaseType baseType)
			: base (accessor, commandContext, baseType)
		{
			this.hasFilter         = false;
			this.hasTreeOperations = false;
			this.hasMoveOperations = false;

			this.title = AbstractView.GetViewTitle (this.accessor, ViewType.Warnings);

			var warnings = new List<Warning>();
			WarningsLogic.GetWarnings (warnings, this.accessor);
			this.nodeGetter = new WarningNodeGetter (this.accessor, warnings);
		}

		public void Dispose()
		{
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
			this.NodeGetter.SetParams (this.sortingInstructions);

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

		public string SelectedPersistantUniqueId
		{
			//	Comme les avertissements sont recalculés chaque fois que la vue est activée,
			//	il n'est pas possible d'utiliser le Guid pour retrouver la sélection dans la
			//	vue. Pour cela, on a besoin d'un identificateur persistant qui dépend uniquement
			//	des données réelles.
			get
			{
				if (this.VisibleSelectedRow == -1)
				{
					return null;
				}
				else
				{
					return this.NodeGetter[this.VisibleSelectedRow].PersistantUniqueId;
				}
			}
			set
			{
				this.VisibleSelectedRow = this.nodeGetter.GetNodes ().ToList ().FindIndex (x => x.PersistantUniqueId == value);
			}
		}

		public string PrevPersistantUniqueId
		{
			get
			{
				if (this.VisibleSelectedRow == -1 ||
					this.VisibleSelectedRow-1 < 0)
				{
					return null;
				}
				else
				{
					return this.NodeGetter[this.VisibleSelectedRow-1].PersistantUniqueId;
				}
			}
		}

		public string NextPersistantUniqueId
		{
			get
			{
				if (this.VisibleSelectedRow == -1 ||
					this.VisibleSelectedRow+1 >= this.NodeGetter.Count)
				{
					return null;
				}
				else
				{
					return this.NodeGetter[this.VisibleSelectedRow+1].PersistantUniqueId;
				}
			}
		}


		#region Goto Logic
		public AbstractViewState Goto(Guid warningGuid)
		{
			var warning = this.NodeGetter.GetNodes ().Where (x => x.Guid == warningGuid).FirstOrDefault ();

			switch (warning.BaseType.Kind)
			{
				case BaseTypeKind.Assets:
					return this.GotoAsset (warning);

				case BaseTypeKind.Categories:
					return this.GotoCategory (warning);

				case BaseTypeKind.Groups:
					return this.GotoGroup (warning);

				case BaseTypeKind.Persons:
					return this.GotoPerson (warning);

				default:
					return null;
			}
		}

		private AbstractViewState GotoAsset(Warning warning)
		{
			var obj  = this.accessor.GetObject (warning.BaseType, warning.ObjectGuid);
			var e    = obj.GetEvent (warning.EventGuid);
			var page = EditorPageSummary.GetPageType (this.accessor, warning.Field);

			return AssetsView.GetViewState (warning.ObjectGuid, e.Timestamp, page, warning.Field);
		}

		private AbstractViewState GotoCategory(Warning warning)
		{
			var obj = this.accessor.GetObject (warning.BaseType, warning.ObjectGuid);
			var e = obj.GetEvent (warning.EventGuid);

			return CategoriesView.GetViewState (warning.ObjectGuid, warning.Field);
		}

		private AbstractViewState GotoGroup(Warning warning)
		{
			var obj = this.accessor.GetObject (warning.BaseType, warning.ObjectGuid);
			var e = obj.GetEvent (warning.EventGuid);

			return GroupsView.GetViewState (warning.ObjectGuid, warning.Field);
		}

		private AbstractViewState GotoPerson(Warning warning)
		{
			var obj = this.accessor.GetObject (warning.BaseType, warning.ObjectGuid);
			var e = obj.GetEvent (warning.EventGuid);

			return PersonsView.GetViewState (warning.ObjectGuid, warning.Field);
		}
		#endregion


		protected override void CreateToolbar()
		{
			this.toolbar = new WarningsToolbar (this.accessor, this.commandContext);
		}
		//?protected override void AdaptToolbarCommand()
		//?{
		//?	this.toolbar.SetCommandDescription (ToolbarCommand.New,      CommandDescription.Empty);
		//?	this.toolbar.SetCommandDescription (ToolbarCommand.Delete,   CommandDescription.Empty);
		//?	this.toolbar.SetCommandDescription (ToolbarCommand.Deselect, null, Res.Strings.ToolbarControllers.WarningsTreeTable.Deselect.ToString ());
		//?	this.toolbar.SetCommandDescription (ToolbarCommand.Copy,     CommandDescription.Empty);
		//?	this.toolbar.SetCommandDescription (ToolbarCommand.Paste,    CommandDescription.Empty);
		//?	this.toolbar.SetCommandDescription (ToolbarCommand.Export,   null, Res.Strings.ToolbarControllers.WarningsTreeTable.Export.ToString ());
		//?	this.toolbar.SetCommandDescription (ToolbarCommand.Import,   CommandDescription.Empty);
		//?	this.toolbar.SetCommandDescription (ToolbarCommand.Goto,     null, Res.Strings.ToolbarControllers.WarningsTreeTable.Goto.ToString ());
		//?}

		protected override void CreateNodeFiller()
		{
			this.dataFiller = new WarningsTreeTableFiller (this.accessor, this.nodeGetter)
			{
				Title = this.title,
			};

			TreeTableFiller<Warning>.FillColumns (this.treeTableController, this.dataFiller, "View.Warnings");

			this.sortingInstructions = TreeTableFiller<Warning>.GetSortingInstructions (this.treeTableController);
		}


		[Command (Res.CommandIds.Warnings.First)]
		protected override void OnFirst()
		{
			base.OnFirst ();
		}

		[Command (Res.CommandIds.Warnings.Prev)]
		protected override void OnPrev()
		{
			base.OnPrev ();
		}

		[Command (Res.CommandIds.Warnings.Next)]
		protected override void OnNext()
		{
			base.OnNext ();
		}

		[Command (Res.CommandIds.Warnings.Last)]
		protected override void OnLast()
		{
			base.OnLast ();
		}

		[Command (Res.CommandIds.Warnings.Deselect)]
		protected void OnDeselect()
		{
			this.VisibleSelectedRow = -1;
		}

		[Command (Res.CommandIds.Warnings.Goto)]
		protected void OnGoto()
		{
			this.OnRowDoubleClicked (this.VisibleSelectedRow);
		}


		protected override void UpdateToolbar()
		{
			base.UpdateToolbar ();

			this.toolbar.SetEnable (Res.Commands.Warnings.Goto, this.VisibleSelectedRow != -1);
		}


		private WarningNodeGetter NodeGetter
		{
			get
			{
				return this.nodeGetter as WarningNodeGetter;
			}
		}
	}
}
