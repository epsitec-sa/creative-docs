//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class CategoriesView : AbstractView
	{
		public override void CreateUI(Widget parent, MainToolbar toolbar)
		{
			base.CreateUI (parent, toolbar);

			this.lastSelectedRow = -2;

			this.treeTable = new SimpleTreeTableController ();
			this.treeTable.CreateUI (this.listBox, footerHeight: 0);
			this.treeTable.SetColumns (this.Descriptions, 0);
			this.treeTable.SetContent (this.Content);

			this.treeTable.RowClicked += delegate
			{
				if (this.lastSelectedRow == this.treeTable.SelectedRow)
				{
					this.OnCommandEdit ();
				}
				else
				{
					this.lastSelectedRow = this.treeTable.SelectedRow;
					this.Update ();
				}
			};

			this.Update ();
		}


		protected override string Title
		{
			get
			{
				return "Catégories d'immobilisation";
			}
		}


		protected override int SelectedRow
		{
			get
			{
				return this.treeTable.SelectedRow;
			}
			set
			{
				this.treeTable.SelectedRow = value;
			}
		}


		private TreeTableColumnDescription[] Descriptions
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,  200, "Nom"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Decimal,  80, "Taux"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,   80, "Type"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Decimal, 100, "Résidu"));

				return list.ToArray ();
			}
		}

		private List<List<AbstractSimpleTreeTableCell>> Content
		{
			get
			{
				var list = new List<List<AbstractSimpleTreeTableCell>> ();

				{
					var line = new List<AbstractSimpleTreeTableCell> ();
					line.Add (new SimpleTreeTableCellString ("Immobilier"));
					line.Add (new SimpleTreeTableCellDecimal (10.0m));
					line.Add (new SimpleTreeTableCellString ("Linéaire"));
					line.Add (new SimpleTreeTableCellDecimal (1000.0m));
					list.Add (line);
				}

				{
					var line = new List<AbstractSimpleTreeTableCell> ();
					line.Add (new SimpleTreeTableCellString ("Véhicules"));
					line.Add (new SimpleTreeTableCellDecimal (20.0m));
					line.Add (new SimpleTreeTableCellString ("Progressif"));
					line.Add (new SimpleTreeTableCellDecimal (1.0m));
					list.Add (line);
				}

				{
					var line = new List<AbstractSimpleTreeTableCell> ();
					line.Add (new SimpleTreeTableCellString ("Machines"));
					line.Add (new SimpleTreeTableCellDecimal (7.5m));
					line.Add (new SimpleTreeTableCellString ("Progressif"));
					line.Add (new SimpleTreeTableCellDecimal (1.0m));
					list.Add (line);
				}

				{
					var line = new List<AbstractSimpleTreeTableCell> ();
					line.Add (new SimpleTreeTableCellString ("Mobilier"));
					line.Add (new SimpleTreeTableCellDecimal (8.0m));
					line.Add (new SimpleTreeTableCellString ("Linéaire"));
					line.Add (new SimpleTreeTableCellDecimal (1.0m));
					list.Add (line);
				}

				return list;
			}
		}


		private SimpleTreeTableController		treeTable;
		private int								lastSelectedRow;
	}
}
