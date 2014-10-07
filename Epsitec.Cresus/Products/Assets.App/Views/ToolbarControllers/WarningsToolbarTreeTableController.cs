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
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Popups;

namespace Epsitec.Cresus.Assets.App.Views.ToolbarControllers
{
	public class WarningsToolbarTreeTableController : AbstractToolbarBothTreesController<Warning>, IDirty, System.IDisposable
	{
		public WarningsToolbarTreeTableController(DataAccessor accessor, CommandContext commandContext, BaseType baseType)
			: base (accessor, commandContext, baseType)
		{
			this.title = AbstractView.GetViewTitle (this.accessor, ViewType.Warnings);

			var warnings = new List<Warning>();
			WarningsLogic.GetWarnings (warnings, this.accessor);
			this.nodeGetter = new WarningNodeGetter (this.accessor, warnings);
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
			this.NodeGetter.SetParams (this.sortingInstructions);

			this.UpdateController ();
			this.UpdateToolbar ();
		}


		public override bool					HelplineDesired
		{
			get
			{
				//	Le bouton d'aide est toujours visible, pour éviter des sauts verticaux
				//	très gênant lors d'un double-clic dans le TreeTable.
				return true;
			}
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

				case BaseTypeKind.AssetsUserFields:
				case BaseTypeKind.PersonsUserFields:
					return this.GotoUserFields (warning);

				case BaseTypeKind.Accounts:
					return this.GotoAccounts (warning);

				default:
					return null;
			}
		}

		private AbstractViewState GotoAsset(Warning warning)
		{
			if (warning.ObjectGuid.IsEmpty)
			{
				return AssetsView.GetViewState (Guid.Empty, null, PageType.Unknown, ObjectField.Unknown);
			}
			else
			{
				var obj  = this.accessor.GetObject (warning.BaseType, warning.ObjectGuid);
				var e    = obj.GetEvent (warning.EventGuid);
				var page = EditorPageSummary.GetPageType (this.accessor, warning.Field);

				return AssetsView.GetViewState (warning.ObjectGuid, e.Timestamp, page, warning.Field);
			}
		}

		private AbstractViewState GotoCategory(Warning warning)
		{
			return CategoriesView.GetViewState (warning.ObjectGuid, warning.Field);
		}

		private AbstractViewState GotoGroup(Warning warning)
		{
			return GroupsView.GetViewState (warning.ObjectGuid, warning.Field);
		}

		private AbstractViewState GotoPerson(Warning warning)
		{
			return PersonsView.GetViewState (warning.ObjectGuid, warning.Field);
		}

		private AbstractViewState GotoUserFields(Warning warning)
		{
			return UserFieldsView.GetViewState (warning.BaseType);
		}

		private AbstractViewState GotoAccounts(Warning warning)
		{
			return AccountsView.GetViewState (this.accessor, null, null);
		}
		#endregion


		protected override void CreateToolbar()
		{
			this.toolbar = new WarningsToolbar (this.accessor, this.commandContext);
			this.ConnectSearch ();
		}

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


		protected override void ShowContextMenu(Point pos)
		{
			//	Affiche le menu contextuel.
			MenuPopup.Show (this.toolbar, this.treeTableFrame, pos,
				Res.Commands.Warnings.Goto);
		}


		protected override void UpdateToolbar()
		{
			base.UpdateToolbar ();

			int row = this.VisibleSelectedRow;

			this.UpdateSelCommand (Res.Commands.Warnings.First, row, this.FirstRowIndex);
			this.UpdateSelCommand (Res.Commands.Warnings.Prev,  row, this.PrevRowIndex);
			this.UpdateSelCommand (Res.Commands.Warnings.Next,  row, this.NextRowIndex);
			this.UpdateSelCommand (Res.Commands.Warnings.Last,  row, this.LastRowIndex);

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
