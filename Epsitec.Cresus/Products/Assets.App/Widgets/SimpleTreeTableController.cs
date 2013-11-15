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

					switch (description.Type)
					{
						case TreeTableColumnType.String:
							System.Diagnostics.Debug.Assert (row[c] is SimpleTreeTableCellString);
							break;

						case TreeTableColumnType.Decimal:
						case TreeTableColumnType.Amount:
						case TreeTableColumnType.Rate:
							System.Diagnostics.Debug.Assert (row[c] is SimpleTreeTableCellDecimal);
							break;

						case TreeTableColumnType.ComputedAmount:
						case TreeTableColumnType.DetailedComputedAmount:
							System.Diagnostics.Debug.Assert (row[c] is SimpleTreeTableCellComputedAmount);
							break;

						case TreeTableColumnType.Int:
							System.Diagnostics.Debug.Assert (row[c] is SimpleTreeTableCellInt);
							break;

						case TreeTableColumnType.Date:
							System.Diagnostics.Debug.Assert (row[c] is SimpleTreeTableCellDate);
							break;

						case TreeTableColumnType.Glyph:
							System.Diagnostics.Debug.Assert (row[c] is SimpleTreeTableCellGlyph);
							break;

						case TreeTableColumnType.Guid:
							System.Diagnostics.Debug.Assert (row[c] is SimpleTreeTableCellGuid);
							break;

						default:
							System.Diagnostics.Debug.Fail ("Unsupported type exception");
							break;
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
				switch (description.Type)
				{
					case TreeTableColumnType.String:
						list.Add (new List<TreeTableCellString> ());
						break;

					case TreeTableColumnType.Decimal:
					case TreeTableColumnType.Amount:
					case TreeTableColumnType.Rate:
						list.Add (new List<TreeTableCellDecimal> ());
						break;

					case TreeTableColumnType.ComputedAmount:
					case TreeTableColumnType.DetailedComputedAmount:
						list.Add (new List<TreeTableCellComputedAmount> ());
						break;

					case TreeTableColumnType.Int:
						list.Add (new List<TreeTableCellInt> ());
						break;

					case TreeTableColumnType.Date:
						list.Add (new List<TreeTableCellDate> ());
						break;

					case TreeTableColumnType.Glyph:
						list.Add (new List<TreeTableCellGlyph> ());
						break;

					case TreeTableColumnType.Guid:
						list.Add (new List<TreeTableCellGuid> ());
						break;
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

					switch (description.Type)
					{
						case TreeTableColumnType.String:
							{
								var x = (content as SimpleTreeTableCellString).Value;
								var s = new TreeTableCellString (true, x, isSelected: (i == selection));

								var l = list[c] as List<TreeTableCellString>;
								l.Add (s);
							}
							break;

						case TreeTableColumnType.Decimal:
						case TreeTableColumnType.Amount:
						case TreeTableColumnType.Rate:
							{
								var x = (content as SimpleTreeTableCellDecimal).Value;
								var s = new TreeTableCellDecimal (true, x, isSelected: (i == selection));

								var l = list[c] as List<TreeTableCellDecimal>;
								l.Add (s);
							}
							break;

						case TreeTableColumnType.ComputedAmount:
						case TreeTableColumnType.DetailedComputedAmount:
							{
								var x = (content as SimpleTreeTableCellComputedAmount).Value;
								var s = new TreeTableCellComputedAmount (true, x, isSelected: (i == selection));

								var l = list[c] as List<TreeTableCellComputedAmount>;
								l.Add (s);
							}
							break;

						case TreeTableColumnType.Int:
							{
								var x = (content as SimpleTreeTableCellInt).Value;
								var s = new TreeTableCellInt (true, x, isSelected: (i == selection));

								var l = list[c] as List<TreeTableCellInt>;
								l.Add (s);
							}
							break;

						case TreeTableColumnType.Date:
							{
								var x = (content as SimpleTreeTableCellDate).Value;
								var s = new TreeTableCellDate (true, x, isSelected: (i == selection));

								var l = list[c] as List<TreeTableCellDate>;
								l.Add (s);
							}
							break;

						case TreeTableColumnType.Glyph:
							{
								var x = (content as SimpleTreeTableCellGlyph).Value;
								var s = new TreeTableCellGlyph (true, x, isSelected: (i == selection));

								var l = list[c] as List<TreeTableCellGlyph>;
								l.Add (s);
							}
							break;

						case TreeTableColumnType.Guid:
							{
								var x = (content as SimpleTreeTableCellGuid).Value;
								var s = new TreeTableCellGuid (true, x, isSelected: (i == selection));

								var l = list[c] as List<TreeTableCellGuid>;
								l.Add (s);
							}
							break;
					}
				}
			}

			//	Passe les données des conteneurs au TreeTable.
			for (int c=0; c<this.columnDescriptions.Length; c++)
			{
				var description = this.columnDescriptions[c];

#if false
				switch (description.Type)
				{
					case TreeTableColumnType.String:
						{
							var l = list[c] as List<TreeTableCellString>;
							this.treeTable.SetColumnCells (c, l.ToArray ());
						}
						break;

					case TreeTableColumnType.Decimal:
					case TreeTableColumnType.Amount:
					case TreeTableColumnType.Rate:
						{
							var l = list[c] as List<TreeTableCellDecimal>;
							this.treeTable.SetColumnCells (c, l.ToArray ());
						}
						break;

					case TreeTableColumnType.ComputedAmount:
					case TreeTableColumnType.DetailedComputedAmount:
						{
							var l = list[c] as List<TreeTableCellComputedAmount>;
							this.treeTable.SetColumnCells (c, l.ToArray ());
						}
						break;

					case TreeTableColumnType.Int:
						{
							var l = list[c] as List<TreeTableCellInt>;
							this.treeTable.SetColumnCells (c, l.ToArray ());
						}
						break;

					case TreeTableColumnType.Date:
						{
							var l = list[c] as List<TreeTableCellDate>;
							this.treeTable.SetColumnCells (c, l.ToArray ());
						}
						break;

					case TreeTableColumnType.Glyph:
						{
							var l = list[c] as List<TreeTableCellGlyph>;
							this.treeTable.SetColumnCells (c, l.ToArray ());
						}
						break;

					case TreeTableColumnType.Guid:
						{
							var l = list[c] as List<TreeTableCellGuid>;
							this.treeTable.SetColumnCells (c, l.ToArray ());
						}
						break;
				}
#endif
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