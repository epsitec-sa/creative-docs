//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectsToolbarTreeTableController : AbstractToolbarTreeTableController<TreeNode>
	{
		public ObjectsToolbarTreeTableController(DataAccessor accessor)
			: base (accessor)
		{
			this.hasFilter         = true;
			this.hasTreeOperations = true;

			//	GuidNode -> ParentPositionNode -> LevelNode -> TreeNode
			var groupNodesGetter  = this.accessor.GetNodesGetter (BaseType.Groups);
			var objectNodesGetter = this.accessor.GetNodesGetter (BaseType.Objects);
			this.nodesGetter = new ObjectsNodesGetter (this.accessor, groupNodesGetter, objectNodesGetter);

			this.title = "Objets d'immobilisation";
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.stateAtController = new StateAtController (this.accessor);
			this.stateAtController.CreateUI (parent);

			this.stateAtController.DateChanged += delegate
			{
				this.SetDate (this.stateAtController.Date);
			};

			this.SetDate (Timestamp.Now.Date);
		}


		public bool								DataFreezed;

		public override void UpdateData()
		{
			this.NodesGetter.SetParams (this.timestamp, this.rootGuid, this.sortingInstructions);
			this.dataFiller.Timestamp = this.timestamp;

			this.UpdateController ();
			this.UpdateToolbar ();
		}


		protected override int					VisibleSelectedRow
		{
			get
			{
				return this.NodesGetter.AllToVisible (this.selectedRow);
			}
			set
			{
				this.SelectedRow = this.NodesGetter.VisibleToAll (value);
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
				if (sel != -1 && sel < this.nodesGetter.Count)
				{
					return this.nodesGetter[sel].Guid;
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
				this.VisibleSelectedRow = this.NodesGetter.SearchBestIndex (value);
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

			var guid = this.SelectedGuid;
			{
				this.UpdateData ();
			}
			this.SelectedGuid = guid;
		}


		protected override void CreateNodeFiller()
		{
			this.dataFiller = new ObjectsTreeTableFiller (this.accessor, this.nodesGetter);
			TreeTableFiller<TreeNode>.FillColumns (this.dataFiller, this.controller);

			this.controller.AddSortedColumn (0);
		}


		protected override void OnFilter()
		{
			var target = this.toolbar.GetCommandWidget (ToolbarCommand.Filter);
			var popup = new FilterPopup (this.accessor, this.rootGuid);

			popup.Create (target, leftOrRight: true);

			popup.Navigate += delegate (object sender, Guid guid)
			{
				this.rootGuid = guid;

				var selectedGuid = this.SelectedGuid;
				this.UpdateData ();
				this.SelectedGuid = selectedGuid;
			};
		}

		private void OnCompactOrExpand(int row)
		{
			//	Etend ou compacte une ligne (inverse son mode actuel).
			var guid = this.SelectedGuid;

			this.NodesGetter.CompactOrExpand (row);
			this.UpdateController ();
			this.UpdateToolbar ();

			this.SelectedGuid = guid;
		}

		protected override void OnCompactAll()
		{
			//	Compacte toutes les lignes.
			var guid = this.SelectedGuid;

			this.NodesGetter.CompactAll ();
			this.UpdateController ();
			this.UpdateToolbar ();

			this.SelectedGuid = guid;
		}

		protected override void OnExpandAll()
		{
			//	Etend toutes les lignes.
			var guid = this.SelectedGuid;

			this.NodesGetter.ExpandAll ();
			this.UpdateController ();
			this.UpdateToolbar ();

			this.SelectedGuid = guid;
		}

		protected override void OnDeselect()
		{
			this.VisibleSelectedRow = -1;
		}

		protected override void OnNew()
		{
			var target = this.toolbar.GetCommandWidget (ToolbarCommand.New);
			this.ShowCreatePopup (target);
		}

		protected override void OnDelete()
		{
			var target = this.toolbar.GetCommandWidget (ToolbarCommand.Delete);

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
						this.accessor.RemoveObject (BaseType.Objects, this.SelectedGuid);
						this.UpdateData ();
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
			var guid = this.accessor.CreateObject (BaseType.Objects, date, name, Guid.Empty);
			var obj = this.accessor.GetObject (BaseType.Objects, guid);
			System.Diagnostics.Debug.Assert (obj != null);
			
			this.UpdateData ();

			this.SelectedGuid = guid;
			this.SelectedTimestamp = ObjectCalculator.GetLastTimestamp (obj);
			
			this.OnStartEditing (EventType.Entrée, this.selectedTimestamp.GetValueOrDefault ());
		}

	
		protected override void CreateTreeTable(Widget parent)
		{
			base.CreateTreeTable (parent);

			this.controller.TreeButtonClicked += delegate (object sender, int row, NodeType type)
			{
				this.OnCompactOrExpand (this.controller.TopVisibleRow + row);
			};
		}


		protected override void UpdateToolbar()
		{
			base.UpdateToolbar ();

			this.toolbar.SetCommandState (ToolbarCommand.Filter,
				this.rootGuid.IsEmpty ? ToolbarCommandState.Enable : ToolbarCommandState.Activate);

			this.toolbar.UpdateCommand (ToolbarCommand.CompactAll, !this.NodesGetter.IsAllCompacted);
			this.toolbar.UpdateCommand (ToolbarCommand.ExpandAll,  !this.NodesGetter.IsAllExpanded);
		}


		private ObjectsNodesGetter NodesGetter
		{
			get
			{
				return this.nodesGetter as ObjectsNodesGetter;
			}
		}


		private StateAtController					stateAtController;
		private Timestamp?							selectedTimestamp;
		private Timestamp?							timestamp;
		private Guid								rootGuid;
	}
}
