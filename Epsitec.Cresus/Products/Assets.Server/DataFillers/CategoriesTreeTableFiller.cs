//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class CategoriesTreeTableFiller : AbstractTreeTableFiller<SortableNode>
	{
		public CategoriesTreeTableFiller(DataAccessor accessor, AbstractNodeGetter<SortableNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				yield return ObjectField.Name;
				yield return ObjectField.Number;
				yield return ObjectField.AmortizationRate;
				yield return ObjectField.AmortizationType;
				yield return ObjectField.Periodicity;
				yield return ObjectField.Prorata;
				yield return ObjectField.Round;
				yield return ObjectField.ResidualValue;
			}
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
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 100, "Prorata"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Amount, 100, "Arrondi"));
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
			var c5 = new TreeTableColumnItem<TreeTableCellString> ();
			var c6 = new TreeTableColumnItem<TreeTableCellDecimal> ();
			var c7 = new TreeTableColumnItem<TreeTableCellDecimal> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodeGetter.Count)
				{
					break;
				}

				var node  = this.nodeGetter[firstRow+i];
				var guid  = node.Guid;
				var obj   = this.accessor.GetObject (BaseType.Categories, guid);

				var nom    = ObjectCalculator.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Name, inputValue: true);
				var numéro = ObjectCalculator.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Number);
				var taux   = ObjectCalculator.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.AmortizationRate);
				var type   = ObjectCalculator.GetObjectPropertyInt     (obj, this.Timestamp, ObjectField.AmortizationType);
				var period = ObjectCalculator.GetObjectPropertyInt     (obj, this.Timestamp, ObjectField.Periodicity);
				var prorat = ObjectCalculator.GetObjectPropertyInt     (obj, this.Timestamp, ObjectField.Prorata);
				var round  = ObjectCalculator.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.Round);
				var residu = ObjectCalculator.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.ResidualValue);

				var t = EnumDictionaries.GetAmortizationTypeName (type);
				var c = EnumDictionaries.GetPeriodicityName (period);
				var r = EnumDictionaries.GetProrataTypeName (prorat);

				var s0 = new TreeTableCellString  (true, nom,    isSelected: (i == selection));
				var s1 = new TreeTableCellString  (true, numéro, isSelected: (i == selection));
				var s2 = new TreeTableCellDecimal (true, taux,   isSelected: (i == selection));
				var s3 = new TreeTableCellString  (true, t,      isSelected: (i == selection));
				var s4 = new TreeTableCellString  (true, c,      isSelected: (i == selection));
				var s5 = new TreeTableCellString  (true, r,      isSelected: (i == selection));
				var s6 = new TreeTableCellDecimal (true, round,  isSelected: (i == selection));
				var s7 = new TreeTableCellDecimal (true, residu, isSelected: (i == selection));

				c0.AddRow (s0);
				c1.AddRow (s1);
				c2.AddRow (s2);
				c3.AddRow (s3);
				c4.AddRow (s4);
				c5.AddRow (s5);
				c6.AddRow (s6);
				c7.AddRow (s7);
			}

			var content = new TreeTableContentItem ();

			content.Columns.Add (c0);
			content.Columns.Add (c1);
			content.Columns.Add (c2);
			content.Columns.Add (c3);
			content.Columns.Add (c4);
			content.Columns.Add (c5);
			content.Columns.Add (c6);
			content.Columns.Add (c7);

			return content;
		}
	}
}
