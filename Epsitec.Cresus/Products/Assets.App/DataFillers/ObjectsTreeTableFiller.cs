//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public class ObjectsTreeTableFiller : AbstractTreeTableFiller
	{
		public ObjectsTreeTableFiller(DataAccessor accessor, BaseType baseType, NavigationTreeTableController controller, AbstractNodeFiller nodeFiller)
			: base (accessor, baseType, controller, nodeFiller)
		{
		}


		public override void UpdateColumns()
		{
			this.controller.SetColumns (ObjectsTreeTableFiller.TreeTableColumns, 1);
		}

		public override void UpdateContent(int firstRow, int count, int selection, bool crop = true)
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
				if (firstRow+i >= this.nodeFiller.NodesCount)
				{
					break;
				}

				var node  = this.nodeFiller.GetNode (firstRow+i);
				var guid  = node.Guid;
				var level = node.Level;
				var type  = node.Type;
				var obj   = this.accessor.GetObject (this.baseType, guid);

				var nom         = ObjectCalculator.GetObjectPropertyString         (obj, this.Timestamp, ObjectField.Nom);
				var numéro      = ObjectCalculator.GetObjectPropertyString         (obj, this.Timestamp, ObjectField.Numéro);
				var responsable = ObjectCalculator.GetObjectPropertyString         (obj, this.Timestamp, ObjectField.Responsable);
				var couleur     = ObjectCalculator.GetObjectPropertyString         (obj, this.Timestamp, ObjectField.Couleur);
				var série       = ObjectCalculator.GetObjectPropertyString         (obj, this.Timestamp, ObjectField.NuméroSérie);
				var valeur1     = ObjectCalculator.GetObjectPropertyComputedAmount (obj, this.Timestamp, ObjectField.Valeur1);
				var valeur2     = ObjectCalculator.GetObjectPropertyComputedAmount (obj, this.Timestamp, ObjectField.Valeur2);
				var valeur3     = ObjectCalculator.GetObjectPropertyComputedAmount (obj, this.Timestamp, ObjectField.Valeur3);

				if (this.Timestamp.HasValue &&
					!ObjectCalculator.IsExistingObject (obj, this.Timestamp.Value))
				{
					nom = "<i>Inconnu à cette date</i>";
				}

				var sf = new TreeTableCellTree           (true, level, type, nom, isSelected: (i == selection));
				var s1 = new TreeTableCellString         (true, numéro,      isSelected: (i == selection));
				var s2 = new TreeTableCellString         (true, responsable, isSelected: (i == selection));
				var s3 = new TreeTableCellString         (true, couleur,     isSelected: (i == selection));
				var s4 = new TreeTableCellString         (true, série,       isSelected: (i == selection));
				var s5 = new TreeTableCellComputedAmount (true, valeur1,     isSelected: (i == selection));
				var s6 = new TreeTableCellComputedAmount (true, valeur2,     isSelected: (i == selection));
				var s7 = new TreeTableCellComputedAmount (true, valeur3,     isSelected: (i == selection));

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


		private static TreeTableColumnDescription[] TreeTableColumns
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
	}
}
