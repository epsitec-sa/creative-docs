//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class ObjectsTreeTableFiller : AbstractTreeTableFiller<TreeNode>
	{
		public ObjectsTreeTableFiller(DataAccessor accessor, AbstractNodesGetter<TreeNode> nodesGetter)
			: base (accessor, nodesGetter)
		{
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
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         120, "Responsable"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          60, "Couleur"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         200, "Numéro de série"));

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

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodesGetter.Count)
				{
					break;
				}

				var node  = this.nodesGetter[firstRow+i];
				var guid  = node.Guid;
				var level = node.Level;
				var type  = node.Type;
				var obj   = this.accessor.GetObject (BaseType.Objects, guid);

				if (obj == null)
				{
					obj   = this.accessor.GetObject (BaseType.Groups, guid);
				}

				var regroupement = ObjectCalculator.GetObjectPropertyInt            (obj, this.Timestamp, ObjectField.Regroupement);
				var nom          = ObjectCalculator.GetObjectPropertyString         (obj, this.Timestamp, ObjectField.Nom);
				var numéro       = ObjectCalculator.GetObjectPropertyString         (obj, this.Timestamp, ObjectField.Numéro);
				var valeur1      = ObjectCalculator.GetObjectPropertyComputedAmount (obj, this.Timestamp, ObjectField.Valeur1);
				var valeur2      = ObjectCalculator.GetObjectPropertyComputedAmount (obj, this.Timestamp, ObjectField.Valeur2);
				var valeur3      = ObjectCalculator.GetObjectPropertyComputedAmount (obj, this.Timestamp, ObjectField.Valeur3);
				var responsable  = ObjectCalculator.GetObjectPropertyString         (obj, this.Timestamp, ObjectField.Responsable);
				var couleur      = ObjectCalculator.GetObjectPropertyString         (obj, this.Timestamp, ObjectField.Couleur);
				var série        = ObjectCalculator.GetObjectPropertyString         (obj, this.Timestamp, ObjectField.NuméroSérie);

				if (this.Timestamp.HasValue &&
					!ObjectCalculator.IsExistingObject (obj, this.Timestamp.Value))
				{
					nom = DataDescriptions.OutOfDateName;
				}

				var grouping = regroupement.HasValue && regroupement.Value == 1;

				var sf = new TreeTableCellTree           (true, level, type, nom, isSelected: (i == selection));
				var s1 = new TreeTableCellString         (true, numéro,           isSelected: (i == selection));
				var s2 = new TreeTableCellComputedAmount (true, valeur1,          isSelected: (i == selection), isUnavailable: grouping);
				var s3 = new TreeTableCellComputedAmount (true, valeur2,          isSelected: (i == selection), isUnavailable: grouping);
				var s4 = new TreeTableCellComputedAmount (true, valeur3,          isSelected: (i == selection), isUnavailable: grouping);
				var s5 = new TreeTableCellString         (true, responsable,      isSelected: (i == selection), isUnavailable: grouping);
				var s6 = new TreeTableCellString         (true, couleur,          isSelected: (i == selection), isUnavailable: grouping);
				var s7 = new TreeTableCellString         (true, série,            isSelected: (i == selection), isUnavailable: grouping);

				cf.AddRow (sf);
				c1.AddRow (s1);
				c2.AddRow (s2);
				c3.AddRow (s3);
				c4.AddRow (s4);
				c5.AddRow (s5);
				c6.AddRow (s6);
				c7.AddRow (s7);
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

			return content;
		}
	}
}
