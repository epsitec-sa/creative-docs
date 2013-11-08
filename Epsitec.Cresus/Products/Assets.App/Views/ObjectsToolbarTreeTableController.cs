//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.DataFillers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectsToolbarTreeTableController : AbstractToolbarTreeTableController
	{
		public ObjectsToolbarTreeTableController(DataAccessor accessor, BaseType baseType)
			: base (accessor)
		{
			this.baseType = baseType;

			var primaryNodesGetter = this.accessor.GetNodesGetter (this.baseType);
			var levelNodesGetter = new LevelNodesGetter (primaryNodesGetter, this.accessor, this.baseType);
			this.nodesGetter = new TreeObjectsNodesGetter (levelNodesGetter);

			switch (this.baseType)
			{
				case BaseType.Objects:
					this.title = "Objets d'immobilisation";
					break;

				case BaseType.Categories:
					this.title = "Catégories d'immobilisation";
					break;

				case BaseType.Groups:
					this.title = "Groupes d'immobilisation";
					break;
			}

			this.hasTreeOperations = true;

			this.objectGuids = new List<Guid> ();
			this.UpdateObjects ();
		}


		public Guid								SelectedGuid
		{
			get
			{
				if (this.SelectedRow >= 0 && this.SelectedRow < this.objectGuids.Count)
				{
					return this.objectGuids[this.SelectedRow];
				}
				else
				{
					return Guid.Empty;
				}
			}
			set
			{
				this.SelectedRow = this.objectGuids.IndexOf (value);
			}
		}

		public Timestamp?						Timestamp
		{
			get
			{
				return this.timestamp;
			}
			set
			{
				if (this.timestamp != value)
				{
					this.timestamp = value;

					this.dataFiller.Timestamp = this.timestamp;
					this.UpdateController ();
					this.UpdateToolbar ();
				}
			}
		}


		protected override void OnNew()
		{
			var modelGuid = this.SelectedGuid;
			if (modelGuid.IsEmpty)
			{
				return;
			}

			int sel = this.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			var timestamp = this.accessor.CreateObject (this.baseType, sel+1, modelGuid);

			this.UpdateObjects ();
			this.UpdateData ();
			this.UpdateController ();
			this.UpdateToolbar ();

			this.SelectedRow = sel+1;
			this.OnStartEditing (EventType.Entrée, timestamp);
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

				popup.Create (target);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "yes")
					{
					}
				};
			}
		}


		protected override void CreateNodeFiller()
		{
			switch (this.baseType)
			{
				case BaseType.Objects:
					this.dataFiller = new ObjectsTreeTableFiller (this.accessor, this.baseType, this.controller, this.nodesGetter);
					break;

				case BaseType.Categories:
					this.dataFiller = new CategoriesTreeTableFiller (this.accessor, this.baseType, this.controller, this.nodesGetter);
					break;

				case BaseType.Groups:
					this.dataFiller = new GroupsTreeTableFiller (this.accessor, this.baseType, this.controller, this.nodesGetter);
					break;
			}

			base.CreateNodeFiller ();
		}


#if false
		protected override int NodesCount
		{
			get
			{
				return this.accessor.GetObjectsCount (this.baseType);
			}
		}

		protected override Node GetNode(int row)
		{
			if (row >= 0 && row < this.objectGuids.Count)
			{
				var guid = this.objectGuids[row];

				var obj = this.accessor.GetObject (this.baseType, guid);
				var p = ObjectCalculator.GetObjectSyntheticProperty (obj, this.timestamp, ObjectField.Level) as DataIntProperty;
				if (p != null)
				{
					return new Node (guid, p.Value);
				}
			}

			return Node.Empty;
		}
#endif


		private void UpdateObjects()
		{
			this.objectGuids.Clear ();
			this.objectGuids.AddRange (this.accessor.GetObjectGuids (this.baseType));
		}


		#region Events handler
		private void OnStartEditing(EventType eventType, Timestamp timestamp)
		{
			if (this.StartEditing != null)
			{
				this.StartEditing (this, eventType, timestamp);
			}
		}

		public delegate void StartEditingEventHandler(object sender, EventType eventType, Timestamp timestamp);
		public event StartEditingEventHandler StartEditing;
		#endregion


		private readonly List<Guid>				objectGuids;
		private Timestamp?						timestamp;
	}
}
