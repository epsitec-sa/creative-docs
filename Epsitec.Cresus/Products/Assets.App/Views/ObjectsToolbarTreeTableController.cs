//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectsToolbarTreeTableController : AbstractToolbarTreeTableController
	{
		public ObjectsToolbarTreeTableController(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Objets d'immobilisation";
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


		protected override TreeTableColumnDescription[] TreeTableColumns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Tree,           180, "Objet"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          50, "N°"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.ComputedAmount, 120, "Valeur comptable"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.ComputedAmount, 120, "Valeur assurance"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.ComputedAmount, 120, "Valeur imposable"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         120, "Responsable"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          60, "Couleur"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         200, "Numéro de série"));

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
			var c7 = new List<TreeTableCellComputedAmount> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.NodesCount)
				{
					break;
				}

				var node  = this.GetNode (firstRow+i);
				var guid  = node.Guid;
				var level = node.Level;
				var type  = node.Type;
				var obj   = this.accessor.GetObject (guid);

				var nom         = ObjectCalculator.GetObjectPropertyString (obj, this.timestamp, ObjectField.Nom);
				var numéro      = ObjectCalculator.GetObjectPropertyString (obj, this.timestamp, ObjectField.Numéro);
				var responsable = ObjectCalculator.GetObjectPropertyString (obj, this.timestamp, ObjectField.Responsable);
				var couleur     = ObjectCalculator.GetObjectPropertyString (obj, this.timestamp, ObjectField.Couleur);
				var série       = ObjectCalculator.GetObjectPropertyString (obj, this.timestamp, ObjectField.NuméroSérie);
				var valeur1     = ObjectCalculator.GetObjectPropertyComputedAmount (obj, this.timestamp, ObjectField.Valeur1);
				var valeur2     = ObjectCalculator.GetObjectPropertyComputedAmount (obj, this.timestamp, ObjectField.Valeur2);
				var valeur3     = ObjectCalculator.GetObjectPropertyComputedAmount (obj, this.timestamp, ObjectField.Valeur3);

				var sf = new TreeTableCellTree (true, level, type, nom, isSelected: (i == selection));
				var s1 = new TreeTableCellString (true, numéro, isSelected: (i == selection));
				var s2 = new TreeTableCellString (true, responsable, isSelected: (i == selection));
				var s3 = new TreeTableCellString (true, couleur, isSelected: (i == selection));
				var s4 = new TreeTableCellString (true, série, isSelected: (i == selection));
				var s5 = new TreeTableCellComputedAmount (true, valeur1, isSelected: (i == selection));
				var s6 = new TreeTableCellComputedAmount (true, valeur2, isSelected: (i == selection));
				var s7 = new TreeTableCellComputedAmount (true, valeur3, isSelected: (i == selection));

				cf.Add (sf);
				c1.Add (s1);
				c2.Add (s2);
				c3.Add (s3);
				c4.Add (s4);
				c5.Add (s5);
				c6.Add (s6);
				c7.Add (s7);
			}

			{
				int i = 0;
				this.controller.SetColumnCells (i++, cf.ToArray ());
				this.controller.SetColumnCells (i++, c1.ToArray ());
				this.controller.SetColumnCells (i++, c5.ToArray ());
				this.controller.SetColumnCells (i++, c6.ToArray ());
				this.controller.SetColumnCells (i++, c7.ToArray ());
				this.controller.SetColumnCells (i++, c2.ToArray ());
				this.controller.SetColumnCells (i++, c3.ToArray ());
				this.controller.SetColumnCells (i++, c4.ToArray ());
			}
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
			guid = Guid.Empty;
			level = 0;

			if (row >= 0 && row < this.objectGuids.Count)
			{
				guid = this.objectGuids[row];

				var obj = this.accessor.GetObject(guid);
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
			this.objectGuids.AddRange (this.accessor.GetObjectGuids ());
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
