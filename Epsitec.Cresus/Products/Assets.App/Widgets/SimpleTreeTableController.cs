﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Ce contrôleur simplifie l'usage d'un NavigationTreeTableController, en supposant
	/// une liste complète connue à l'avance.
	/// A n'utiliser que pour les listes pas trop longues !
	/// </summary>
	public class SimpleTreeTableController
	{
		public SimpleTreeTableController()
		{
			this.treeTable = new NavigationTreeTableController ();

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
				this.OnRowClicked (column, row);
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

		public void SetContent(List<List<AbstractSimpleTreeTableCell>> content)
		{
			this.content = content;
			this.CheckContent ();
			this.UpdateContent ();
		}


		private void CheckContent()
		{
			foreach (var row in this.content)
			{
				for (int c=0; c<this.columnDescriptions.Length; c++)
				{
					var description = this.columnDescriptions[c];

					System.Diagnostics.Debug.Assert (row.Count == this.columnDescriptions.Length);

					if (description.Type == TreeTableColumnType.String)
					{
						System.Diagnostics.Debug.Assert (row[c] is SimpleTreeTableCellString);
					}
					else if (description.Type == TreeTableColumnType.Decimal ||
							 description.Type == TreeTableColumnType.Rate)
					{
						System.Diagnostics.Debug.Assert (row[c] is SimpleTreeTableCellDecimal);
					}
					else
					{
						System.Diagnostics.Debug.Fail ("Unsupported type exception");
					}
				}
			}
		}

		private void UpdateContent()
		{
			this.treeTable.RowsCount = this.content.Count;
			this.UpdateTreeTableController ();
		}

		private void UpdateTreeTableController()
		{
			var firstRow = this.treeTable.TopVisibleRow;
			int selection = this.selectedRow - this.treeTable.TopVisibleRow;

			//	Construit la liste des conteneurs.
			var list = new List<object> ();

			foreach (var description in this.columnDescriptions)
			{
				if (description.Type == TreeTableColumnType.String)
				{
					list.Add (new List<TreeTableCellString> ());
				}
				else if (description.Type == TreeTableColumnType.Decimal ||
						 description.Type == TreeTableColumnType.Rate)
				{
					list.Add (new List<TreeTableCellDecimal> ());
				}
			}

			//	Rempli les conteneurs en fonction de this.Content.
			var count = this.treeTable.VisibleRowsCount;
			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.content.Count)
				{
					break;
				}

				var row = this.content[firstRow+i];

				for (int c=0; c<this.columnDescriptions.Length; c++)
				{
					var description = this.columnDescriptions[c];
					var content = row[c];

					if (description.Type == TreeTableColumnType.String)
					{
						var x = (content as SimpleTreeTableCellString).Value;
						var s = new TreeTableCellString (true, x, isSelected: (i == selection));

						var l = list[c] as List<TreeTableCellString>;
						l.Add (s);
					}
					else if (description.Type == TreeTableColumnType.Decimal ||
							 description.Type == TreeTableColumnType.Rate)
					{
						var x = (content as SimpleTreeTableCellDecimal).Value;
						var s = new TreeTableCellDecimal (true, x, isSelected: (i == selection));

						var l = list[c] as List<TreeTableCellDecimal>;
						l.Add (s);
					}
				}
			}

			//	Passe les données des conteneurs au TreeTable.
			for (int c=0; c<this.columnDescriptions.Length; c++)
			{
				var description = this.columnDescriptions[c];

				if (description.Type == TreeTableColumnType.String)
				{
					var l = list[c] as List<TreeTableCellString>;
					this.treeTable.SetColumnCells (c, l.ToArray ());
				}
				else if (description.Type == TreeTableColumnType.Decimal ||
						 description.Type == TreeTableColumnType.Rate)
				{
					var l = list[c] as List<TreeTableCellDecimal>;
					this.treeTable.SetColumnCells (c, l.ToArray ());
				}
			}
		}


		#region Events handler
		private void OnRowClicked(int column, int row)
		{
			if (this.RowClicked != null)
			{
				this.RowClicked (this, column, row);
			}
		}

		public delegate void RowClickedEventHandler(object sender, int column, int row);
		public event RowClickedEventHandler RowClicked;
		#endregion


		private readonly NavigationTreeTableController		treeTable;

		private TreeTableColumnDescription[]				columnDescriptions;
		private List<List<AbstractSimpleTreeTableCell>>		content;
		private int											selectedRow;
	}
}