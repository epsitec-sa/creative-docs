//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectsToolbarTreeTableController : AbstractToolbarTreeTableController
	{
		public ObjectsToolbarTreeTableController(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Objets d'immobilisation";
			this.hasTreeOperations = true;
		}


		public Guid								SelectedGuid
		{
			get
			{
				return this.accessor.GetObjectGuid (this.SelectedRow);
			}
			set
			{
				int count = this.accessor.ObjectsCount;
				for (int i=0; i<count; i++)
				{
					var guid = this.accessor.GetObjectGuid (i);
					if (guid == value)
					{
						this.SelectedRow = i;
						return;
					}
				}

				this.SelectedRow = -1;
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

			var timestamp = this.accessor.CreateObject (sel+1, modelGuid);

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


		protected override TreeTableColumnDescription[] TreeTableColumns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Tree,           180, "Objet"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          50, "N°"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         120, "Responsable"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          60, "Couleur"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         200, "Numéro de série"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.ComputedAmount, 120, "Valeur comptable"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.ComputedAmount, 120, "Valeur assurance"));

				return list.ToArray ();
			}
		}

		protected override void UpdateContent(int firstRow, int count, int selection, bool crop = true)
		{
			var cf = new List<TreeTableCellTree> ();
			var c1 = new List<TreeTableCellString> ();
			var c2 = new List<TreeTableCellString> ();
			var c3 = new List<TreeTableCellString> ();
			var c4 = new List<TreeTableCellString> ();
			var c5 = new List<TreeTableCellComputedAmount> ();
			var c6 = new List<TreeTableCellComputedAmount> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.NodesCount)
				{
					break;
				}

				var node = this.GetNode (firstRow+i);
				var guid  = node.Guid;
				var level = node.Level;
				var type  = node.Type;

				var properties = this.accessor.GetObjectSyntheticProperties (guid, this.timestamp);

				var nom         = DataAccessor.GetStringProperty (properties, (int) ObjectField.Nom);
				var numéro      = DataAccessor.GetStringProperty (properties, (int) ObjectField.Numéro);
				var responsable = DataAccessor.GetStringProperty (properties, (int) ObjectField.Responsable);
				var couleur     = DataAccessor.GetStringProperty (properties, (int) ObjectField.Couleur);
				var série       = DataAccessor.GetStringProperty (properties, (int) ObjectField.NuméroSérie);
				var valeur1     = DataAccessor.GetComputedAmountProperty (properties, (int) ObjectField.Valeur1);
				var valeur2     = DataAccessor.GetComputedAmountProperty (properties, (int) ObjectField.Valeur2);

				var sf = new TreeTableCellTree (true, level, type, nom, isSelected: (i == selection));
				var s1 = new TreeTableCellString (true, numéro, isSelected: (i == selection));
				var s2 = new TreeTableCellString (true, responsable, isSelected: (i == selection));
				var s3 = new TreeTableCellString (true, couleur, isSelected: (i == selection));
				var s4 = new TreeTableCellString (true, série, isSelected: (i == selection));
				var s5 = new TreeTableCellComputedAmount (true, valeur1, isSelected: (i == selection));
				var s6 = new TreeTableCellComputedAmount (true, valeur2, isSelected: (i == selection));

				cf.Add (sf);
				c1.Add (s1);
				c2.Add (s2);
				c3.Add (s3);
				c4.Add (s4);
				c5.Add (s5);
				c6.Add (s6);
			}

			this.controller.SetColumnCells (0, cf.ToArray ());
			this.controller.SetColumnCells (1, c1.ToArray ());
			this.controller.SetColumnCells (2, c2.ToArray ());
			this.controller.SetColumnCells (3, c3.ToArray ());
			this.controller.SetColumnCells (4, c4.ToArray ());
			this.controller.SetColumnCells (5, c5.ToArray ());
			this.controller.SetColumnCells (6, c6.ToArray ());
		}


		protected override int DataCount
		{
			get
			{
				return this.accessor.ObjectsCount;
			}
		}

		protected override void GetData(int row, out Guid guid, out int level)
		{
			guid = this.accessor.GetObjectGuid (row);
			var properties = this.accessor.GetObjectSyntheticProperties (guid, this.timestamp);
			level = DataAccessor.GetIntProperty (properties, (int) ObjectField.Level).GetValueOrDefault (-1);
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


		private Timestamp?						timestamp;
	}
}
