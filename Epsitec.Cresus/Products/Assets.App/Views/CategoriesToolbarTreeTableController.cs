//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class CategoriesToolbarTreeTableController : AbstractToolbarTreeTableController
	{
		public CategoriesToolbarTreeTableController(DataAccessor accessor)
			: base (accessor)
		{
			this.baseType = BaseType.Categories;

			this.title = "Catégories d'immobilisation";
			this.hasTreeOperations = true;

			this.catGuids = new List<Guid> ();
			this.UpdateCategories ();
		}


		public Guid								SelectedGuid
		{
			get
			{
				if (this.SelectedRow >= 0 && this.SelectedRow < this.catGuids.Count)
				{
					return this.catGuids[this.SelectedRow];
				}
				else
				{
					return Guid.Empty;
				}
			}
			set
			{
				this.SelectedRow = this.catGuids.IndexOf (value);
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

			var timestamp = this.accessor.CreateObject (BaseType.Categories, sel+1, modelGuid);

			this.UpdateCategories ();
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
					Question = "Voulez-vous supprimer la catégorie sélectionnée ?",
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

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Tree,   180, "Catégorie"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,  50, "N°"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Rate,    80, "Taux"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,  80, "Type"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 100, "Périodicité"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Amount, 120, "Valeur résiduelle"));

				return list.ToArray ();
			}
		}

		protected override void UpdateContent(int firstRow, int count, int selection, bool crop = true)
		{
			var cf = new List<TreeTableCellTree> ();
			var c1 = new List<TreeTableCellString> ();
			var c2 = new List<TreeTableCellDecimal> ();
			var c3 = new List<TreeTableCellString> ();
			var c4 = new List<TreeTableCellString> ();
			var c5 = new List<TreeTableCellDecimal> ();

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
				var obj   = this.accessor.GetObject (BaseType.Categories, guid);

				var nom    = ObjectCalculator.GetObjectPropertyString  (obj, this.timestamp, ObjectField.Nom);
				var numéro = ObjectCalculator.GetObjectPropertyString  (obj, this.timestamp, ObjectField.Numéro);
				var taux   = ObjectCalculator.GetObjectPropertyDecimal (obj, this.timestamp, ObjectField.TauxAmortissement);
				var typeAm = ObjectCalculator.GetObjectPropertyString  (obj, this.timestamp, ObjectField.TypeAmortissement);
				var period = ObjectCalculator.GetObjectPropertyString  (obj, this.timestamp, ObjectField.Périodicité);
				var residu = ObjectCalculator.GetObjectPropertyDecimal (obj, this.timestamp, ObjectField.ValeurRésiduelle);

				if (this.timestamp.HasValue &&
					!ObjectCalculator.IsExistingObject (obj, this.timestamp.Value))
				{
					nom = "<i>Inconnu à cette date</i>";
				}

				var sf = new TreeTableCellTree    (true, level, type, nom, isSelected: (i == selection));
				var s1 = new TreeTableCellString  (true, numéro, isSelected: (i == selection));
				var s2 = new TreeTableCellDecimal (true, taux, isSelected: (i == selection));
				var s3 = new TreeTableCellString  (true, typeAm, isSelected: (i == selection));
				var s4 = new TreeTableCellString  (true, period, isSelected: (i == selection));
				var s5 = new TreeTableCellDecimal (true, residu, isSelected: (i == selection));

				cf.Add (sf);
				c1.Add (s1);
				c2.Add (s2);
				c3.Add (s3);
				c4.Add (s4);
				c5.Add (s5);
			}

			{
				int i = 0;
				this.controller.SetColumnCells (i++, cf.ToArray ());
				this.controller.SetColumnCells (i++, c1.ToArray ());
				this.controller.SetColumnCells (i++, c2.ToArray ());
				this.controller.SetColumnCells (i++, c3.ToArray ());
				this.controller.SetColumnCells (i++, c4.ToArray ());
				this.controller.SetColumnCells (i++, c5.ToArray ());
			}
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

			if (row >= 0 && row < this.catGuids.Count)
			{
				guid = this.catGuids[row];

				var obj = this.accessor.GetObject (this.baseType, guid);
				var p = ObjectCalculator.GetObjectSyntheticProperty (obj, this.timestamp, ObjectField.Level) as DataIntProperty;
				if (p != null)
				{
					level = p.Value;
				}
			}
		}


		private void UpdateCategories()
		{
			this.catGuids.Clear ();
			this.catGuids.AddRange (this.accessor.GetObjectGuids (this.baseType));
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


		private readonly List<Guid>				catGuids;
		private Timestamp?						timestamp;
	}
}
