//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public class ObjectsTreeTableFiller2 : AbstractTreeTableFiller2
	{
		public ObjectsTreeTableFiller2(DataAccessor accessor, BaseType baseType, AbstractNodesGetter<TreeNode> nodesGetter)
			: base (accessor, baseType, nodesGetter)
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

		public override TreeTableContent GetContent(int firstRow, int count, int selection)
		{
			var cf = new TreeTableColumn ();
			var c1 = new TreeTableColumn ();
			var c2 = new TreeTableColumn ();
			var c3 = new TreeTableColumn ();
			var c4 = new TreeTableColumn ();
			var c5 = new TreeTableColumn ();
			var c6 = new TreeTableColumn ();
			var c7 = new TreeTableColumn ();

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
					nom = DataDescriptions.OutOfDateName;
				}

				var sf = new TreeTableCellTree           (true, level, type, nom, isSelected: (i == selection));
				var s1 = new TreeTableCellString         (true, numéro,      isSelected: (i == selection));
				var s2 = new TreeTableCellString         (true, responsable, isSelected: (i == selection));
				var s3 = new TreeTableCellString         (true, couleur,     isSelected: (i == selection));
				var s4 = new TreeTableCellString         (true, série,       isSelected: (i == selection));
				var s5 = new TreeTableCellComputedAmount (true, valeur1,     isSelected: (i == selection));
				var s6 = new TreeTableCellComputedAmount (true, valeur2,     isSelected: (i == selection));
				var s7 = new TreeTableCellComputedAmount (true, valeur3,     isSelected: (i == selection));

				cf.AddRow (sf);
				c1.AddRow (s1);
				c2.AddRow (s5);
				c3.AddRow (s6);
				c4.AddRow (s7);
				c5.AddRow (s2);
				c6.AddRow (s3);
				c7.AddRow (s4);
			}

			var content = new TreeTableContent ();

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
