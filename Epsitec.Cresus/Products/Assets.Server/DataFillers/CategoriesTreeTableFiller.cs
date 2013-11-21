//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class CategoriesTreeTableFiller : AbstractTreeTableFiller<OrderNode>
	{
		public CategoriesTreeTableFiller(DataAccessor accessor, AbstractNodesGetter<OrderNode> nodesGetter)
			: base (accessor, nodesGetter)
		{
		}


		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 180, "Catégorie"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,  50, "N°"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Rate,    80, "Taux"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,  80, "Type"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 100, "Périodicité"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Amount, 120, "Valeur résiduelle"));

				return list.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var c0 = new TreeTableColumnItem<TreeTableCellString> ();
			var c1 = new TreeTableColumnItem<TreeTableCellString> ();
			var c2 = new TreeTableColumnItem<TreeTableCellDecimal> ();
			var c3 = new TreeTableColumnItem<TreeTableCellString> ();
			var c4 = new TreeTableColumnItem<TreeTableCellString> ();
			var c5 = new TreeTableColumnItem<TreeTableCellDecimal> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodesGetter.Count)
				{
					break;
				}

				var node  = this.nodesGetter[firstRow+i];
				var guid  = node.Guid;
				var obj   = this.accessor.GetObject (BaseType.Categories, guid);

				var nom          = ObjectCalculator.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Nom);
				var numéro       = ObjectCalculator.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Numéro);
				var taux         = ObjectCalculator.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.TauxAmortissement);
				var typeAm       = ObjectCalculator.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.TypeAmortissement);
				var period       = ObjectCalculator.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Périodicité);
				var residu       = ObjectCalculator.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.ValeurRésiduelle);

				if (this.Timestamp.HasValue &&
					!ObjectCalculator.IsExistingObject (obj, this.Timestamp.Value))
				{
					nom = DataDescriptions.OutOfDateName;
				}

				var s0 = new TreeTableCellString  (true, nom,    isSelected: (i == selection));
				var s1 = new TreeTableCellString  (true, numéro, isSelected: (i == selection));
				var s2 = new TreeTableCellDecimal (true, taux,   isSelected: (i == selection));
				var s3 = new TreeTableCellString  (true, typeAm, isSelected: (i == selection));
				var s4 = new TreeTableCellString  (true, period, isSelected: (i == selection));
				var s5 = new TreeTableCellDecimal (true, residu, isSelected: (i == selection));

				c0.AddRow (s0);
				c1.AddRow (s1);
				c2.AddRow (s2);
				c3.AddRow (s3);
				c4.AddRow (s4);
				c5.AddRow (s5);
			}

			var content = new TreeTableContentItem ();

			content.Columns.Add (c0);
			content.Columns.Add (c1);
			content.Columns.Add (c2);
			content.Columns.Add (c3);
			content.Columns.Add (c4);
			content.Columns.Add (c5);

			return content;
		}
	}
}
