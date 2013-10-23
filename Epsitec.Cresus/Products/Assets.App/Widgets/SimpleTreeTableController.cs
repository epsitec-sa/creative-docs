//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

			this.treeTable.RowClicked += delegate (object sender, int row)
			{
				this.selectedRow = this.treeTable.TopVisibleRow + row;
				this.UpdateTreeTableController ();
				this.OnRowClicked (this.selectedRow);
			};

			this.treeTable.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateTreeTableController (crop);
			};
		}


		public bool AllowsMovement
		{
			get
			{
				return this.treeTable.AllowsMovement;
			}
			set
			{
				this.treeTable.AllowsMovement = value;
			}
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

		public void ShowSelection()
		{
			// TODO...
		}

		public void SetColumns(TreeTableColumnDescription[] descriptions, int dockToLeftCount = 0)
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
							 description.Type == TreeTableColumnType.Amount  ||
							 description.Type == TreeTableColumnType.Rate)
					{
						System.Diagnostics.Debug.Assert (row[c] is SimpleTreeTableCellDecimal);
					}
					else if (description.Type == TreeTableColumnType.ComputedAmount)
					{
						System.Diagnostics.Debug.Assert (row[c] is SimpleTreeTableCellComputedAmount);
					}
					else if (description.Type == TreeTableColumnType.Int)
					{
						System.Diagnostics.Debug.Assert (row[c] is SimpleTreeTableCellInt);
					}
					else if (description.Type == TreeTableColumnType.Date)
					{
						System.Diagnostics.Debug.Assert (row[c] is SimpleTreeTableCellDate);
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

		private void UpdateTreeTableController(bool crop = true)
		{
			int visibleCount = this.treeTable.VisibleRowsCount;
			int rowsCount    = this.content.Count;
			int count        = System.Math.Min (visibleCount, rowsCount);
			int firstRow     = this.treeTable.TopVisibleRow;
			int selection    = this.selectedRow;

			if (selection != -1)
			{
				//	La sélection ne peut pas dépasser le nombre maximal de lignes.
				selection = System.Math.Min (selection, rowsCount-1);

				//	Si la sélection est hors de la zone visible, on choisit un autre cadrage.
				if (crop && (selection < firstRow || selection >= firstRow+count))
				{
					firstRow = this.treeTable.GetTopVisibleRow (selection);
				}

				if (this.treeTable.TopVisibleRow != firstRow)
				{
					this.treeTable.TopVisibleRow = firstRow;
				}

				selection -= this.treeTable.TopVisibleRow;
			}

			//	Construit la liste des conteneurs.
			var list = new List<object> ();

			foreach (var description in this.columnDescriptions)
			{
				if (description.Type == TreeTableColumnType.String)
				{
					list.Add (new List<TreeTableCellString> ());
				}
				else if (description.Type == TreeTableColumnType.Decimal ||
						 description.Type == TreeTableColumnType.Amount  ||
						 description.Type == TreeTableColumnType.Rate)
				{
					list.Add (new List<TreeTableCellDecimal> ());
				}
				else if (description.Type == TreeTableColumnType.ComputedAmount)
				{
					list.Add (new List<TreeTableCellComputedAmount> ());
				}
				else if (description.Type == TreeTableColumnType.Int)
				{
					list.Add (new List<TreeTableCellInt> ());
				}
				else if (description.Type == TreeTableColumnType.Date)
				{
					list.Add (new List<TreeTableCellDate> ());
				}
			}

			//	Rempli les conteneurs en fonction de this.Content.
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
							 description.Type == TreeTableColumnType.Amount  ||
							 description.Type == TreeTableColumnType.Rate)
					{
						var x = (content as SimpleTreeTableCellDecimal).Value;
						var s = new TreeTableCellDecimal (true, x, isSelected: (i == selection));

						var l = list[c] as List<TreeTableCellDecimal>;
						l.Add (s);
					}
					else if (description.Type == TreeTableColumnType.Date)
					{
						var x = (content as SimpleTreeTableCellDate).Value;
						var s = new TreeTableCellDate (true, x, isSelected: (i == selection));

						var l = list[c] as List<TreeTableCellDate>;
						l.Add (s);
					}
					else if (description.Type == TreeTableColumnType.Int)
					{
						var x = (content as SimpleTreeTableCellInt).Value;
						var s = new TreeTableCellInt (true, x, isSelected: (i == selection));

						var l = list[c] as List<TreeTableCellInt>;
						l.Add (s);
					}
					else if (description.Type == TreeTableColumnType.ComputedAmount)
					{
						var x = (content as SimpleTreeTableCellComputedAmount).Value;
						var s = new TreeTableCellComputedAmount (true, x, isSelected: (i == selection));

						var l = list[c] as List<TreeTableCellComputedAmount>;
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
						 description.Type == TreeTableColumnType.Amount  ||
						 description.Type == TreeTableColumnType.Rate)
				{
					var l = list[c] as List<TreeTableCellDecimal>;
					this.treeTable.SetColumnCells (c, l.ToArray ());
				}
				else if (description.Type == TreeTableColumnType.Date)
				{
					var l = list[c] as List<TreeTableCellDate>;
					this.treeTable.SetColumnCells (c, l.ToArray ());
				}
				else if (description.Type == TreeTableColumnType.Int)
				{
					var l = list[c] as List<TreeTableCellInt>;
					this.treeTable.SetColumnCells (c, l.ToArray ());
				}
				else if (description.Type == TreeTableColumnType.ComputedAmount)
				{
					var l = list[c] as List<TreeTableCellComputedAmount>;
					this.treeTable.SetColumnCells (c, l.ToArray ());
				}
			}
		}


		#region Events handler
		private void OnRowClicked(int row)
		{
			if (this.RowClicked != null)
			{
				this.RowClicked (this, row);
			}
		}

		public delegate void RowClickedEventHandler(object sender, int row);
		public event RowClickedEventHandler RowClicked;
		#endregion


		private readonly NavigationTreeTableController		treeTable;

		private TreeTableColumnDescription[]				columnDescriptions;
		private List<List<AbstractSimpleTreeTableCell>>		content;
		private int											selectedRow;
	}
}