//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class AssetsToolbarTreeTableController : AbstractToolbarTreeTableController<CumulNode>, IDirty
	{
		public AssetsToolbarTreeTableController(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
			this.hasGraphic        = true;
			this.hasFilter         = true;
			this.hasTreeOperations = true;
			this.hasMoveOperations = false;

			//	GuidNode -> ParentPositionNode -> LevelNode -> TreeNode -> CumulNode
			var groupNodeGetter  = this.accessor.GetNodeGetter (BaseType.Groups);
			var objectNodeGetter = this.accessor.GetNodeGetter (BaseType.Assets);
			this.nodeGetter = new ObjectsNodeGetter (this.accessor, groupNodeGetter, objectNodeGetter);

			this.title = StaticDescriptions.GetViewTypeDescription (ViewType.Assets);
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

	
		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.stateAtController = new StateAtController (this.accessor);
			this.stateAtController.CreateUI (parent);

			this.graphicFrame.Margins = new Margins (0, 0, 0, AbstractScroller.DefaultBreadth);

			this.stateAtController.DateChanged += delegate
			{
				this.SetDate (this.stateAtController.Date);
			};

			this.SetDate (Timestamp.Now.Date);
		}


		public override void UpdateData()
		{
			this.NodeGetter.SetParams (this.timestamp, this.rootGuid, this.sortingInstructions);
			this.dataFiller.Timestamp = this.timestamp;

			if (this.graphicController != null)
			{
				this.graphicController.SetParams (this.timestamp, this.rootGuid, this.sortingInstructions);
			}

			this.UpdateController ();
			this.UpdateToolbar ();
		}


		protected override int					VisibleSelectedRow
		{
			get
			{
				return this.NodeGetter.AllToVisible (this.selectedRow);
			}
			set
			{
				this.SelectedRow = this.NodeGetter.VisibleToAll (value);
			}
		}

		public Timestamp?						SelectedTimestamp
		{
			get
			{
				return this.selectedTimestamp;
			}
			set
			{
				this.selectedTimestamp = value;
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
			//	Sélectionne l'objet ayant un Guid donné. Si la ligne correspondante
			//	est cachée, on est assez malin pour sélectionner la prochaine ligne
			//	visible, vers le haut.
			set
			{
				this.VisibleSelectedRow = this.NodeGetter.SearchBestIndex (value);
			}
		}


		private void SetDate(System.DateTime? date)
		{
			//	Choix du timestamp visible dans tout le TreeTable.
			this.timestamp = null;

			if (date.HasValue)
			{
				this.timestamp = new Timestamp (date.Value, 0);
			}

			this.stateAtController.Date = date;

			using (new SaveSelectedGuid (this))
			{
				this.UpdateData ();
			}
		}


		protected override void CreateNodeFiller()
		{
			this.dataFiller = new AssetsTreeTableFiller (this.accessor, this.nodeGetter);
			TreeTableFiller<CumulNode>.FillColumns (this.treeTableController, this.dataFiller);

			this.treeTableController.AddSortedColumn (0);
		}

		protected override void CreateGraphic(Widget parent)
		{
			this.graphicController = new AssetsGraphicViewController (this.accessor, this.baseType);
			this.graphicController.CreateUI (parent);
		}


		protected override void OnFilter()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.Filter);
			var popup = new FilterPopup (this.accessor, this.rootGuid);

			popup.Create (target, leftOrRight: true);

			popup.Navigate += delegate (object sender, Guid guid)
			{
				this.rootGuid = guid;

				using (new SaveSelectedGuid (this))
				{
					this.UpdateData ();
				}
			};
		}

		protected override void OnDeselect()
		{
			this.VisibleSelectedRow = -1;
		}

		protected override void OnNew()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.New);
			this.ShowCreatePopup (target);
		}

		protected override void OnDelete()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.Delete);

			if (target != null)
			{
				var popup = new YesNoPopup
				{
					Question = "Voulez-vous supprimer l'objet sélectionné ?",
				};

				popup.Create (target, leftOrRight: true);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "yes")
					{
						this.accessor.RemoveObject (BaseType.Assets, this.SelectedGuid);
						this.UpdateData ();
						this.OnUpdateAfterDelete ();
					}
				};
			}
		}


		private void ShowCreatePopup(Widget target)
		{
			var popup = new CreateObjectPopup (this.accessor);

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "create")
				{
					this.CreateObject (popup.ObjectDate.Value, popup.ObjectName);
				}
			};
		}

		private void CreateObject(System.DateTime date, string name)
		{
			var guid = this.accessor.CreateObject (BaseType.Assets, date, name, Guid.Empty);
			var obj = this.accessor.GetObject (BaseType.Assets, guid);
			System.Diagnostics.Debug.Assert (obj != null);
			
			this.UpdateData ();

			this.SelectedGuid = guid;
			this.SelectedTimestamp = AssetCalculator.GetLastTimestamp (obj);
			
			this.OnUpdateAfterCreate (guid, EventType.Input, this.selectedTimestamp.GetValueOrDefault ());
		}

	
		protected override void UpdateToolbar()
		{
			base.UpdateToolbar ();

			this.toolbar.SetCommandActivate (ToolbarCommand.Filter, !this.rootGuid.IsEmpty);

			this.toolbar.SetCommandEnable (ToolbarCommand.CompactAll, !this.NodeGetter.IsAllCompacted);
			this.toolbar.SetCommandEnable (ToolbarCommand.ExpandAll,  !this.NodeGetter.IsAllExpanded);
		}


		private ObjectsNodeGetter NodeGetter
		{
			get
			{
				return this.nodeGetter as ObjectsNodeGetter;
			}
		}


		private StateAtController					stateAtController;
		private Timestamp?							selectedTimestamp;
		private Timestamp?							timestamp;
		private Guid								rootGuid;
	}
}
