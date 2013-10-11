//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class CategoriesView : AbstractView
	{
		public CategoriesView(DataAccessor accessor)
			: base (accessor)
		{
		}

		public override void CreateUI(Widget parent, MainToolbar toolbar)
		{
			base.CreateUI (parent, toolbar);

			this.lastSelectedRow = -2;

			this.treeTable = new SimpleTreeTableController ();
			this.treeTable.CreateUI (this.listFrameBox, footerHeight: 0);
			this.treeTable.SetColumns (this.Columns, 0);
			this.treeTable.SetContent (this.Content);

			this.treeTable.RowClicked += delegate
			{
				if (this.lastSelectedRow == this.treeTable.SelectedRow)
				{
					//?this.OnCommandEdit ();
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


		private TreeTableColumnDescription[] Columns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,   50, "N°"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,  200, "Nom"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Rate,     80, "Taux"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,   80, "Type"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Decimal, 100, "Résidu"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,  100, "Fréquence"));

				return list.ToArray ();
			}
		}

		private List<List<AbstractSimpleTreeTableCell>> Content
		{
			get
			{
				var list = new List<List<AbstractSimpleTreeTableCell>> ();

				{
					var row = new List<AbstractSimpleTreeTableCell> ();
					row.Add (new SimpleTreeTableCellString ("100"));
					row.Add (new SimpleTreeTableCellString ("Immobilier"));
					row.Add (new SimpleTreeTableCellDecimal (10.0m));
					row.Add (new SimpleTreeTableCellString ("Linéaire"));
					row.Add (new SimpleTreeTableCellDecimal (1000.0m));
					row.Add (new SimpleTreeTableCellString ("Annuel"));
					list.Add (row);
				}

				{
					var row = new List<AbstractSimpleTreeTableCell> ();
					row.Add (new SimpleTreeTableCellString ("200"));
					row.Add (new SimpleTreeTableCellString ("Véhicules"));
					row.Add (new SimpleTreeTableCellDecimal (20.0m));
					row.Add (new SimpleTreeTableCellString ("Progressif"));
					row.Add (new SimpleTreeTableCellDecimal (1.0m));
					row.Add (new SimpleTreeTableCellString ("Semestriel"));
					list.Add (row);
				}

				{
					var row = new List<AbstractSimpleTreeTableCell> ();
					row.Add (new SimpleTreeTableCellString ("205"));
					row.Add (new SimpleTreeTableCellString ("Machines"));
					row.Add (new SimpleTreeTableCellDecimal (7.5m));
					row.Add (new SimpleTreeTableCellString ("Progressif"));
					row.Add (new SimpleTreeTableCellDecimal (1.0m));
					row.Add (new SimpleTreeTableCellString ("Annuel"));
					list.Add (row);
				}

				{
					var row = new List<AbstractSimpleTreeTableCell> ();
					row.Add (new SimpleTreeTableCellString ("300"));
					row.Add (new SimpleTreeTableCellString ("Stock"));
					row.Add (new SimpleTreeTableCellDecimal (null));
					row.Add (new SimpleTreeTableCellString (null));
					row.Add (new SimpleTreeTableCellDecimal (100.0m));
					row.Add (new SimpleTreeTableCellString ("Mensuel"));
					list.Add (row);
				}

				{
					var row = new List<AbstractSimpleTreeTableCell> ();
					row.Add (new SimpleTreeTableCellString ("301"));
					row.Add (new SimpleTreeTableCellString ("Mobilier"));
					row.Add (new SimpleTreeTableCellDecimal (8.0m));
					row.Add (new SimpleTreeTableCellString ("Linéaire"));
					row.Add (new SimpleTreeTableCellDecimal (1.0m));
					row.Add (new SimpleTreeTableCellString ("Annuel"));
					list.Add (row);
				}

				return list;
			}
		}


		private SimpleTreeTableController		treeTable;
		private int								lastSelectedRow;
	}
}
