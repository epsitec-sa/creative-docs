//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class ObjectsTreeTableFiller : AbstractTreeTableFiller<CumulNode>
	{
		public ObjectsTreeTableFiller(DataAccessor accessor, AbstractNodesGetter<CumulNode> nodesGetter)
			: base (accessor, nodesGetter)
		{
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				yield return ObjectField.Nom;
				yield return ObjectField.Numéro;
				yield return ObjectField.Valeur1;
				yield return ObjectField.Valeur2;
				yield return ObjectField.Valeur3;
				yield return ObjectField.Maintenance;
				yield return ObjectField.Couleur;
				yield return ObjectField.NuméroSérie;

				yield return ObjectField.GroupGuid+0;
				yield return ObjectField.GroupRatio+0;
				yield return ObjectField.GroupGuid+1;
				yield return ObjectField.GroupRatio+1;
				yield return ObjectField.GroupGuid+2;
				yield return ObjectField.GroupRatio+2;
				yield return ObjectField.GroupGuid+3;
				yield return ObjectField.GroupRatio+3;
				yield return ObjectField.GroupGuid+4;
				yield return ObjectField.GroupRatio+4;
				yield return ObjectField.GroupGuid+5;
				yield return ObjectField.GroupRatio+5;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Tree,           180, "Objet"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          50, "N°"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.ComputedAmount, 120, "Valeur comptable"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.ComputedAmount, 120, "Valeur assurance"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.ComputedAmount, 120, "Valeur imposable"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         120, "Maintenance"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          60, "Couleur"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         200, "Numéro de série"));

				for (int i=0; i<6; i++)
				{
					list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 120, "Groupe"));
					list.Add (new TreeTableColumnDescription (TreeTableColumnType.Rate,    50, "Taux"));
				}

				return list.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var cf = new TreeTableColumnItem<TreeTableCellTree> ();
			var c1 = new TreeTableColumnItem<TreeTableCellString> ();
			var c2 = new TreeTableColumnItem<TreeTableCellComputedAmount> ();
			var c3 = new TreeTableColumnItem<TreeTableCellComputedAmount> ();
			var c4 = new TreeTableColumnItem<TreeTableCellComputedAmount> ();
			var c5 = new TreeTableColumnItem<TreeTableCellString> ();
			var c6 = new TreeTableColumnItem<TreeTableCellString> ();
			var c7 = new TreeTableColumnItem<TreeTableCellString> ();

			var cg0 = new TreeTableColumnItem<TreeTableCellString> ();
			var cr0 = new TreeTableColumnItem<TreeTableCellDecimal> ();
			var cg1 = new TreeTableColumnItem<TreeTableCellString> ();
			var cr1 = new TreeTableColumnItem<TreeTableCellDecimal> ();
			var cg2 = new TreeTableColumnItem<TreeTableCellString> ();
			var cr2 = new TreeTableColumnItem<TreeTableCellDecimal> ();
			var cg3 = new TreeTableColumnItem<TreeTableCellString> ();
			var cr3 = new TreeTableColumnItem<TreeTableCellDecimal> ();
			var cg4 = new TreeTableColumnItem<TreeTableCellString> ();
			var cr4 = new TreeTableColumnItem<TreeTableCellDecimal> ();
			var cg5 = new TreeTableColumnItem<TreeTableCellString> ();
			var cr5 = new TreeTableColumnItem<TreeTableCellDecimal> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodesGetter.Count)
				{
					break;
				}

				var node     = this.nodesGetter[firstRow+i];
				var guid     = node.Guid;
				var baseType = node.BaseType;
				var level    = node.Level;
				var type     = node.Type;

				var obj = this.accessor.GetObject (baseType, guid);

				var valeur1     = this.NodesGetter.GetValue (obj, node, 0);
				var valeur2     = this.NodesGetter.GetValue (obj, node, 1);
				var valeur3     = this.NodesGetter.GetValue (obj, node, 2);

				var nom         = ObjectCalculator.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Nom, inputValue: true);
				var numéro      = ObjectCalculator.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Numéro);
				var maintenance = ObjectCalculator.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Maintenance);
				var couleur     = ObjectCalculator.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Couleur);
				var série       = ObjectCalculator.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.NuméroSérie);

				var guid0       = ObjectCalculator.GetObjectPropertyGuid    (obj, this.Timestamp, ObjectField.GroupGuid+0);
				var rate0       = ObjectCalculator.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.GroupRatio+0);
				var guid1       = ObjectCalculator.GetObjectPropertyGuid    (obj, this.Timestamp, ObjectField.GroupGuid+1);
				var rate1       = ObjectCalculator.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.GroupRatio+1);
				var guid2       = ObjectCalculator.GetObjectPropertyGuid    (obj, this.Timestamp, ObjectField.GroupGuid+2);
				var rate2       = ObjectCalculator.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.GroupRatio+2);
				var guid3       = ObjectCalculator.GetObjectPropertyGuid    (obj, this.Timestamp, ObjectField.GroupGuid+3);
				var rate3       = ObjectCalculator.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.GroupRatio+3);
				var guid4       = ObjectCalculator.GetObjectPropertyGuid    (obj, this.Timestamp, ObjectField.GroupGuid+4);
				var rate4       = ObjectCalculator.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.GroupRatio+4);
				var guid5       = ObjectCalculator.GetObjectPropertyGuid    (obj, this.Timestamp, ObjectField.GroupGuid+5);
				var rate5       = ObjectCalculator.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.GroupRatio+5);

				var group0 = GroupsLogic.GetShortName (this.accessor, guid0);
				var group1 = GroupsLogic.GetShortName (this.accessor, guid1);
				var group2 = GroupsLogic.GetShortName (this.accessor, guid2);
				var group3 = GroupsLogic.GetShortName (this.accessor, guid3);
				var group4 = GroupsLogic.GetShortName (this.accessor, guid4);
				var group5 = GroupsLogic.GetShortName (this.accessor, guid5);

				var grouping = (type != NodeType.Final);

				var sf = new TreeTableCellTree           (true, level, type, nom, isSelected: (i == selection));
				var s1 = new TreeTableCellString         (true, numéro,           isSelected: (i == selection));
				var s2 = new TreeTableCellComputedAmount (true, valeur1,          isSelected: (i == selection), isUnavailable: grouping);
				var s3 = new TreeTableCellComputedAmount (true, valeur2,          isSelected: (i == selection), isUnavailable: grouping);
				var s4 = new TreeTableCellComputedAmount (true, valeur3,          isSelected: (i == selection), isUnavailable: grouping);
				var s5 = new TreeTableCellString         (true, maintenance,      isSelected: (i == selection), isUnavailable: grouping);
				var s6 = new TreeTableCellString         (true, couleur,          isSelected: (i == selection), isUnavailable: grouping);
				var s7 = new TreeTableCellString         (true, série,            isSelected: (i == selection), isUnavailable: grouping);

				var sg0 = new TreeTableCellString        (true, group0,           isSelected: (i == selection), isUnavailable: grouping);
				var sr0 = new TreeTableCellDecimal       (true, rate0,            isSelected: (i == selection), isUnavailable: grouping);
				var sg1 = new TreeTableCellString        (true, group1,           isSelected: (i == selection), isUnavailable: grouping);
				var sr1 = new TreeTableCellDecimal       (true, rate1,            isSelected: (i == selection), isUnavailable: grouping);
				var sg2 = new TreeTableCellString        (true, group2,           isSelected: (i == selection), isUnavailable: grouping);
				var sr2 = new TreeTableCellDecimal       (true, rate2,            isSelected: (i == selection), isUnavailable: grouping);
				var sg3 = new TreeTableCellString        (true, group3,           isSelected: (i == selection), isUnavailable: grouping);
				var sr3 = new TreeTableCellDecimal       (true, rate3,            isSelected: (i == selection), isUnavailable: grouping);
				var sg4 = new TreeTableCellString        (true, group4,           isSelected: (i == selection), isUnavailable: grouping);
				var sr4 = new TreeTableCellDecimal       (true, rate4,            isSelected: (i == selection), isUnavailable: grouping);
				var sg5 = new TreeTableCellString        (true, group5,           isSelected: (i == selection), isUnavailable: grouping);
				var sr5 = new TreeTableCellDecimal       (true, rate5,            isSelected: (i == selection), isUnavailable: grouping);

				cf.AddRow (sf);
				c1.AddRow (s1);
				c2.AddRow (s2);
				c3.AddRow (s3);
				c4.AddRow (s4);
				c5.AddRow (s5);
				c6.AddRow (s6);
				c7.AddRow (s7);

				cg0.AddRow (sg0);
				cr0.AddRow (sr0);
				cg1.AddRow (sg1);
				cr1.AddRow (sr1);
				cg2.AddRow (sg2);
				cr2.AddRow (sr2);
				cg3.AddRow (sg3);
				cr3.AddRow (sr3);
				cg4.AddRow (sg4);
				cr4.AddRow (sr4);
				cg5.AddRow (sg5);
				cr5.AddRow (sr5);
			}

			var content = new TreeTableContentItem ();

			content.Columns.Add (cf);
			content.Columns.Add (c1);
			content.Columns.Add (c2);
			content.Columns.Add (c3);
			content.Columns.Add (c4);
			content.Columns.Add (c5);
			content.Columns.Add (c6);
			content.Columns.Add (c7);

			content.Columns.Add (cg0);
			content.Columns.Add (cr0);
			content.Columns.Add (cg1);
			content.Columns.Add (cr1);
			content.Columns.Add (cg2);
			content.Columns.Add (cr2);
			content.Columns.Add (cg3);
			content.Columns.Add (cr3);
			content.Columns.Add (cg4);
			content.Columns.Add (cr4);
			content.Columns.Add (cg5);
			content.Columns.Add (cr5);

			return content;
		}


		private ObjectsNodesGetter NodesGetter
		{
			get
			{
				return this.nodesGetter as ObjectsNodesGetter;
			}
		}
	}
}
