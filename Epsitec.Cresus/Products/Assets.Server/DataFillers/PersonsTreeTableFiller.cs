//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class PersonsTreeTableFiller : AbstractTreeTableFiller<SortableNode>
	{
		public PersonsTreeTableFiller(DataAccessor accessor, AbstractNodesGetter<SortableNode> nodesGetter)
			: base (accessor, nodesGetter)
		{
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				yield return ObjectField.Nom;
				yield return ObjectField.Prénom;
				yield return ObjectField.Titre;
				yield return ObjectField.Entreprise;
				yield return ObjectField.Adresse;
				yield return ObjectField.Npa;
				yield return ObjectField.Ville;
				yield return ObjectField.Pays;
				yield return ObjectField.Téléphone1;
				yield return ObjectField.Téléphone2;
				yield return ObjectField.Téléphone3;
				yield return ObjectField.Mail;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 120, "Nom"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 120, "Prénom"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,  80, "Titre"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 120, "Entreprise"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 150, "Adresse"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,  50, "NPA"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 120, "Ville"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 120, "Pays"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 100, "Tél. prof."));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 100, "Tél. privé"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 100, "Tél. portable"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 200, "E-mail"));

				return list.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var c0  = new TreeTableColumnItem<TreeTableCellString> ();
			var c1  = new TreeTableColumnItem<TreeTableCellString> ();
			var c2  = new TreeTableColumnItem<TreeTableCellString> ();
			var c3  = new TreeTableColumnItem<TreeTableCellString> ();
			var c4  = new TreeTableColumnItem<TreeTableCellString> ();
			var c5  = new TreeTableColumnItem<TreeTableCellString> ();
			var c6  = new TreeTableColumnItem<TreeTableCellString> ();
			var c7  = new TreeTableColumnItem<TreeTableCellString> ();
			var c8  = new TreeTableColumnItem<TreeTableCellString> ();
			var c9  = new TreeTableColumnItem<TreeTableCellString> ();
			var c10 = new TreeTableColumnItem<TreeTableCellString> ();
			var c11 = new TreeTableColumnItem<TreeTableCellString> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodesGetter.Count)
				{
					break;
				}

				var node  = this.nodesGetter[firstRow+i];
				var guid  = node.Guid;
				var obj   = this.accessor.GetObject (BaseType.Persons, guid);

				var text0  = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Nom, inputValue: true);
				var text1  = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Prénom);
				var text2  = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Titre);
				var text3  = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Entreprise);
				var text4  = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Adresse);
				var text5  = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Npa);
				var text6  = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Ville);
				var text7  = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Pays);
				var text8  = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Téléphone1);
				var text9  = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Téléphone2);
				var text10 = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Téléphone3);
				var text11 = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Mail);

				var s0  = new TreeTableCellString (true, text0,  isSelected: (i == selection));
				var s1  = new TreeTableCellString (true, text1,  isSelected: (i == selection));
				var s2  = new TreeTableCellString (true, text2,  isSelected: (i == selection));
				var s3  = new TreeTableCellString (true, text3,  isSelected: (i == selection));
				var s4  = new TreeTableCellString (true, text4,  isSelected: (i == selection));
				var s5  = new TreeTableCellString (true, text5,  isSelected: (i == selection));
				var s6  = new TreeTableCellString (true, text6,  isSelected: (i == selection));
				var s7  = new TreeTableCellString (true, text7,  isSelected: (i == selection));
				var s8  = new TreeTableCellString (true, text8,  isSelected: (i == selection));
				var s9  = new TreeTableCellString (true, text9,  isSelected: (i == selection));
				var s10 = new TreeTableCellString (true, text10, isSelected: (i == selection));
				var s11 = new TreeTableCellString (true, text11, isSelected: (i == selection));

				c0.AddRow (s0);
				c1.AddRow (s1);
				c2.AddRow (s2);
				c3.AddRow (s3);
				c4.AddRow (s4);
				c5.AddRow (s5);
				c6.AddRow (s6);
				c7.AddRow (s7);
				c8.AddRow (s8);
				c9.AddRow (s9);
				c10.AddRow (s10);
				c11.AddRow (s11);
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
			content.Columns.Add (c8);
			content.Columns.Add (c9);
			content.Columns.Add (c10);
			content.Columns.Add (c11);

			return content;
		}
	}
}
