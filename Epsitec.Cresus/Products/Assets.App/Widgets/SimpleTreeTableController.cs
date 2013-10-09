//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class SimpleTreeTableController
	{
		public SimpleTreeTableController()
		{
			this.treeTable = new NavigationTreeTableController ();
			this.content = new List<List<AbstractSimpleTreeTableCell>> ();

			this.selectedRow = -1;
		}


		public void CreateUI(Widget parent, int rowHeight = 18, int headerHeight = 24, int footerHeight = 24)
		{
			this.treeTable.CreateUI (parent, rowHeight, headerHeight, footerHeight);

			this.treeTable.RowChanged += delegate
			{
				this.UpdateTreeTableController ();
			};

			this.treeTable.RowClicked += delegate (object sender, int column, int row)
			{
				this.selectedRow = this.treeTable.TopVisibleRow + row;
				this.UpdateTreeTableController ();
			};

			this.treeTable.ContentChanged += delegate (object sender)
			{
				this.UpdateTreeTableController ();
			};
		}


		public int SelectedRow
		{
			get
			{
				return this.selectedRow;
			}
			set
			{
				if (this.selectedRow != value)
				{
					this.selectedRow = value;
					this.UpdateTreeTableController ();
				}
			}
		}

		public void SetColumns(TreeTableColumnDescription[] descriptions, int dockToLeftCount)
		{
			this.columnDescriptions = descriptions;
			this.treeTable.SetColumns (descriptions, dockToLeftCount);
		}

		public List<List<AbstractSimpleTreeTableCell>> Content
		{
			get
			{
				return this.content;
			}
		}

		public void UpdateContent()
		{
			this.treeTable.RowsCount = this.content.Count;
			this.UpdateTreeTableController ();
		}

		private void UpdateTreeTableController()
		{
			var firstRow = this.treeTable.TopVisibleRow;
			int selection = this.selectedRow - this.treeTable.TopVisibleRow;


			var list = new List<object> ();

			foreach (var description in this.columnDescriptions)
			{
				if (description.Type == TreeTableColumnType.String)
				{
					list.Add (new List<TreeTableCellString> ());
				}
				else if (description.Type == TreeTableColumnType.Decimal)
				{
					list.Add (new List<TreeTableCellDecimal> ());
				}
			}


			var count = this.treeTable.VisibleRowsCount;
			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.content.Count)
				{
					break;
				}

				var line = this.content[firstRow+i];

				for (int c=0; c<this.columnDescriptions.Length; c++)
				{
					var description = this.columnDescriptions[c];
					var content = line[c];

					if (description.Type == TreeTableColumnType.String)
					{
						var x = (content as SimpleTreeTableCellString).Value;
						var s = new TreeTableCellString (true, x, isSelected: (i == selection));

						var l = list[c] as List<TreeTableCellString>;
						l.Add (s);
					}
					else if (description.Type == TreeTableColumnType.Decimal)
					{
						var x = (content as SimpleTreeTableCellDecimal).Value;
						var s = new TreeTableCellDecimal (true, x, isSelected: (i == selection));

						var l = list[c] as List<TreeTableCellDecimal>;
						l.Add (s);
					}
				}

			}


			for (int c=0; c<this.columnDescriptions.Length; c++)
			{
				var description = this.columnDescriptions[c];

				if (description.Type == TreeTableColumnType.String)
				{
					var l = list[c] as List<TreeTableCellString>;
					this.treeTable.SetColumnCells (c, l.ToArray ());
				}
				else if (description.Type == TreeTableColumnType.Decimal)
				{
					var l = list[c] as List<TreeTableCellDecimal>;
					this.treeTable.SetColumnCells (c, l.ToArray ());
				}
			}
		}


		private readonly NavigationTreeTableController treeTable;
		private readonly List<List<AbstractSimpleTreeTableCell>> content;

		private TreeTableColumnDescription[]	columnDescriptions;
		private int								selectedRow;
	}


	// TODO: Move to file ?

	public abstract class AbstractSimpleTreeTableCell
	{
	}

	public class SimpleTreeTableCellString : AbstractSimpleTreeTableCell
	{
		public SimpleTreeTableCellString(string value)
		{
			this.Value = value;
		}

		public readonly string Value;
	}

	public class SimpleTreeTableCellDecimal : AbstractSimpleTreeTableCell
	{
		public SimpleTreeTableCellDecimal(decimal value)
		{
			this.Value = value;
		}

		public readonly decimal Value;
	}
}