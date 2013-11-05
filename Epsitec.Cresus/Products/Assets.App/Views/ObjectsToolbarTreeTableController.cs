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

			var nodeFiller = new NodeFiller (this);

			switch (this.baseType)
			{
				case BaseType.Objects:
					this.dataFiller = new ObjectsDataFiller (this.accessor, this.baseType, this.controller, nodeFiller);
					this.title = "Objets d'immobilisation";
					break;

				case BaseType.Categories:
					this.dataFiller = new CategoriesDataFiller (this.accessor, this.baseType, this.controller, nodeFiller);
					this.title = "Catégories d'immobilisation";
					break;

				case BaseType.Groups:
					this.dataFiller = new GroupsDataFiller (this.accessor, this.baseType, this.controller, nodeFiller);
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
			var nodeFiller = new NodeFiller (this);

			switch (this.baseType)
			{
				case BaseType.Objects:
					this.dataFiller = new ObjectsDataFiller (this.accessor, this.baseType, this.controller, nodeFiller);
					break;

				case BaseType.Categories:
					this.dataFiller = new CategoriesDataFiller (this.accessor, this.baseType, this.controller, nodeFiller);
					break;

				case BaseType.Groups:
					this.dataFiller = new GroupsDataFiller (this.accessor, this.baseType, this.controller, nodeFiller);
					break;
			}

			base.CreateNodeFiller ();
		}

		private class NodeFiller : AbstractNodeFiller
		{
			public NodeFiller(ObjectsToolbarTreeTableController controller)
			{
				this.controller = controller;
			}

			public override int NodesCount
			{
				get
				{
					return this.controller.NodesCount;
				}
			}

			public override Node GetNode(int index)
			{
				return this.controller.GetNode (index);
			}

			private readonly ObjectsToolbarTreeTableController controller;
		}


		protected override int DataCount
		{
			get
			{
				return this.accessor.GetObjectsCount (this.baseType);
			}
		}

		protected override void GetData(int row, out Guid guid, out int level)
		{
			guid = Guid.Empty;
			level = 0;

			if (row >= 0 && row < this.objectGuids.Count)
			{
				guid = this.objectGuids[row];

				var obj = this.accessor.GetObject (this.baseType, guid);
				var p = ObjectCalculator.GetObjectSyntheticProperty (obj, this.timestamp, ObjectField.Level) as DataIntProperty;
				if (p != null)
				{
					level = p.Value;
				}
			}
		}


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
