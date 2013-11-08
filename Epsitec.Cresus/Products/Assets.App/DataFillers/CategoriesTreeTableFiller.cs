//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public class CategoriesTreeTableFiller : AbstractTreeTableFiller
	{
		public CategoriesTreeTableFiller(DataAccessor accessor, BaseType baseType, NavigationTreeTableController controller, AbstractNodesGetter nodesGetter)
			: base (accessor, baseType, controller, nodesGetter)
		{
		}


		public override void UpdateColumns()
		{
			this.controller.SetColumns (CategoriesTreeTableFiller.TreeTableColumns, 1);
		}

		public override void UpdateContent(int firstRow, int count, int selection, bool crop = true)
		{
			var cf = new List<TreeTableCellTree> ();
			var c1 = new List<TreeTableCellString> ();
			var c2 = new List<TreeTableCellDecimal> ();
			var c3 = new List<TreeTableCellString> ();
			var c4 = new List<TreeTableCellString> ();
			var c5 = new List<TreeTableCellDecimal> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodesGetter.NodesCount)
				{
					break;
				}

				var node  = this.nodesGetter.GetNode (firstRow+i);
				var guid  = node.Guid;
				var level = node.Level;
				var type  = node.Type;
				var obj   = this.accessor.GetObject (this.baseType, guid);

				var nom    = ObjectCalculator.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Nom);
				var numéro = ObjectCalculator.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Numéro);
				var taux   = ObjectCalculator.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.TauxAmortissement);
				var typeAm = ObjectCalculator.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.TypeAmortissement);
				var period = ObjectCalculator.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Périodicité);
				var residu = ObjectCalculator.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.ValeurRésiduelle);

				if (this.Timestamp.HasValue &&
					!ObjectCalculator.IsExistingObject (obj, this.Timestamp.Value))
				{
					nom = "<i>Inconnu à cette date</i>";
				}

				var sf = new TreeTableCellTree    (true, level, type, nom, isSelected: (i == selection));
				var s1 = new TreeTableCellString  (true, numéro, isSelected: (i == selection));
				var s2 = new TreeTableCellDecimal (true, taux,   isSelected: (i == selection));
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


		private static TreeTableColumnDescription[] TreeTableColumns
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
	}
}
