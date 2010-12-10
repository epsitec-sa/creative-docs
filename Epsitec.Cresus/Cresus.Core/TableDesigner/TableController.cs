//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.Finance.PriceCalculators;
using Epsitec.Cresus.Core.Widgets;

using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.TableDesigner
{
	public class TableController
	{
		public TableController(DesignerTable table)
		{
			this.table = table;
		}

		public void CreateUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (10),
			};

			this.dimensionsPane = new FrameBox
			{
				Parent = frame,
				PreferredWidth = 24+24+80+100+25,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 10, 0, 0),
			};

			var valuesPane = new FrameBox
			{
				Parent = frame,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
			};

			this.CreateDimensionsTableUI (this.dimensionsPane);
			this.CreateValuesTableUI (valuesPane);

			this.Update ();
		}

		public void Update()
		{
			if (this.table.Dimensions.Count == 0)
			{
				this.columnDimensionIndex = -1;
				this.rowDimensionIndex    = -1;
			}
			else if (this.table.Dimensions.Count == 1)
			{
				//	Avec une seule dimension, on préfère utiliser les lignes.
				this.columnDimensionIndex = -1;
				this.rowDimensionIndex    = 0;
			}
			else
			{
				this.columnDimensionIndex = 0;
				this.rowDimensionIndex    = 1;
			}

			this.UpdateDimensionsTable ();
			this.RefreshDimensionsTable ();

			this.UpdateValuesTable ();
		}


		private void CreateDimensionsTableUI(Widget parent)
		{
			var tile = new ArrowedFrame
			{
				Parent = parent,
				ArrowDirection = Direction.Right,
				Dock = DockStyle.Fill,
				Padding = new Margins (5, TileArrow.Breadth+5, 5, 5),
			};

			tile.SetSelected (true);  // conteneur orange

			this.dimensionsTable = new CellTable
			{
				Parent = tile,
				StyleH = CellArrayStyles.ScrollMagic | CellArrayStyles.Separator | CellArrayStyles.Header,
				StyleV = CellArrayStyles.ScrollMagic | CellArrayStyles.Separator,
				DefHeight = 24,
				Dock = DockStyle.Fill,
			};
		}

		private void CreateValuesTableUI(Widget parent)
		{
			var left = new FrameBox
			{
				Parent = parent,
				PreferredWidth = TableController.legendHeight,
				Dock = DockStyle.Left,
				Margins = new Margins (0, -1, 0, 0),
			};

			var swapBox = new FrameBox
			{
				Parent = left,
				PreferredSize = new Size (TableController.legendHeight-3, TableController.legendHeight-3),
				Dock = DockStyle.Top,
				Margins = new Margins (0, 3, 0, 2),
			};

			this.swapButton = new Button
			{
				Parent = swapBox,
				Text = "\\",
				PreferredSize = new Size (TableController.legendHeight-3, TableController.legendHeight-3),
				Dock = DockStyle.Fill,
			};

			ToolTip.Default.SetToolTip (this.swapButton, "Permute les lignes et les colonnes");

			var rowsHeader = new FrameBox
			{
				Parent = left,
				DrawFullFrame = true,
				BackColor = Tile.SurfaceSelectedContainerColors.First (),
				PreferredWidth = TableController.legendHeight,
				Dock = DockStyle.Fill,
			};

			var right = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
			};

			var columnsHeader = new FrameBox
			{
				Parent = right,
				DrawFullFrame = true,
				BackColor = Tile.SurfaceSelectedContainerColors.First (),
				PreferredHeight = TableController.legendHeight,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, -1),
			};

			var table = new FrameBox
			{
				Parent = right,
				DrawFullFrame = true,
				BackColor = Tile.SurfaceSelectedContainerColors.First (),
				Dock = DockStyle.Fill,
				Padding = new Margins (5),
			};

			this.valuesTable = new CellTable
			{
				Parent = table,
				DefHeight = 20,
				HeaderWidth = 80,
				Dock = DockStyle.Fill,
			};

			//	Met les textes statiques des légendes dans les bandes correspondantes.
			this.rowsLegend = new VStaticText
			{
				Parent = rowsHeader,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
				Dock = DockStyle.Fill,
			};

			this.columnsLegend = new StaticText
			{
				Parent = columnsHeader,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
				Dock = DockStyle.Fill,
			};

			//	Connexion des événements.
			this.swapButton.Clicked += new EventHandler<MessageEventArgs> (this.HandleSwapButtonClicked);
		}


		private void UpdateDimensionsTable()
		{
			int count = this.table.Dimensions.Count;

			this.dimensionsPane.Visibility = (count > 2);  // inutile avec moins de 3 dimensions !

			this.dimensionsTable.SetArraySize (4, count);

			this.dimensionsTable.SetWidthColumn (0, 24);
			this.dimensionsTable.SetWidthColumn (1, 24);
			this.dimensionsTable.SetWidthColumn (2, 80);
			this.dimensionsTable.SetWidthColumn (3, 100);

			this.dimensionsTable.SetHeaderTextH (0, "C");
			this.dimensionsTable.SetHeaderTextH (1, "L");
			this.dimensionsTable.SetHeaderTextH (2, "Axe");
			this.dimensionsTable.SetHeaderTextH (3, "Valeur");

			int tabIndex = 1;

			for (int row = 0; row < count; row++)
			{
				if (this.dimensionsTable[0, row].IsEmpty)
				{
					var radio = new RadioButton
					{
						Name = string.Concat ("C", row.ToString (System.Globalization.CultureInfo.InvariantCulture)),
						Dock = DockStyle.Fill,
						Margins = new Margins (5, 0, 0, 0),
					};

					radio.Clicked += new EventHandler<MessageEventArgs> (this.HandleRadioClicked);

					this.dimensionsTable[0, row].Insert (radio);
					this.dimensionsTable[0, row].TabIndex = tabIndex++;
				}

				if (this.dimensionsTable[1, row].IsEmpty)
				{
					var radio = new RadioButton
					{
						Name = string.Concat ("R", row.ToString (System.Globalization.CultureInfo.InvariantCulture)),
						Dock = DockStyle.Fill,
						Margins = new Margins (5, 0, 0, 0),
					};

					radio.Clicked += new EventHandler<MessageEventArgs> (this.HandleRadioClicked);

					this.dimensionsTable[1, row].Insert (radio);
					this.dimensionsTable[1, row].TabIndex = tabIndex++;
				}

				if (this.dimensionsTable[2, row].IsEmpty)
				{
					var label = new StaticText
					{
						ContentAlignment = Common.Drawing.ContentAlignment.MiddleLeft,
						Dock = DockStyle.Fill,
						Margins = new Margins (5, 5, 0, 0),
					};

					this.dimensionsTable[2, row].Insert (label);
					this.dimensionsTable[2, row].TabIndex = tabIndex++;
				}

				if (!this.dimensionsTable[2, row].IsEmpty)
				{
					var label = this.dimensionsTable[2, row].Children[0] as StaticText;

					label.Text = this.table.Dimensions[row].Name.ToString ();
				}

				if (this.dimensionsTable[3, row].IsEmpty)
				{
					var combo = new TextFieldCombo
					{
						Name = row.ToString (System.Globalization.CultureInfo.InvariantCulture),
						IsReadOnly = true,
						Dock = DockStyle.Fill,
						Margins = new Margins (-1),
						TabIndex = tabIndex++,
					};

					combo.ComboClosed += new EventHandler (this.HandleComboClosed);

					this.dimensionsTable[3, row].Insert (combo);
					this.dimensionsTable[3, row].TabIndex = tabIndex++;
				}

				if (!this.dimensionsTable[3, row].IsEmpty)
				{
					var combo = this.dimensionsTable[3, row].Children[0] as TextFieldCombo;

					this.UpdateCombo (combo, row);
				}
			}
		}

		private void UpdateCombo(TextFieldCombo combo, int row)
		{
			combo.Items.Clear ();

			foreach (var point in this.table.Dimensions[row].Values)
			{
				combo.Items.Add (point);
			}

			combo.Text = this.table.Dimensions[row].Values.FirstOrDefault ();
		}

		private void RefreshDimensionsTable()
		{
			int count = this.table.Dimensions.Count;

			for (int row = 0; row < count; row++)
			{
				var radioX = this.dimensionsTable[0, row].Children[0] as RadioButton;
				var radioY = this.dimensionsTable[1, row].Children[0] as RadioButton;
				var combo  = this.dimensionsTable[3, row].Children[0] as TextFieldCombo;

				radioX.ActiveState = (this.columnDimensionIndex == row) ? ActiveState.Yes : ActiveState.No;
				radioY.ActiveState = (this.rowDimensionIndex    == row) ? ActiveState.Yes : ActiveState.No;

				combo.Visibility = (this.columnDimensionIndex != row && this.rowDimensionIndex != row);
			}
		}


		private void UpdateValuesTable()
		{
			int count = this.table.Dimensions.Count;

			this.swapButton.Visibility = (count > 1);  // inutile avec une seule dimension !

			if (count == 0)
			{
				this.valuesTable.StyleH = CellArrayStyles.Separator;
				this.valuesTable.StyleV = CellArrayStyles.Separator;
			}
			else if (count == 1)
			{
				this.valuesTable.StyleH = CellArrayStyles.Separator;
				this.valuesTable.StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Header | CellArrayStyles.Separator;
			}
			else
			{
				this.valuesTable.StyleH = CellArrayStyles.ScrollNorm | CellArrayStyles.Header | CellArrayStyles.Separator;
				this.valuesTable.StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Header | CellArrayStyles.Separator;
			}

			var columnsDimension = (this.columnDimensionIndex == -1) ? null : this.table.Dimensions[this.columnDimensionIndex];
			var rowsDimension    = (this.rowDimensionIndex    == -1) ? null : this.table.Dimensions[this.rowDimensionIndex];

			this.columnsLegend.Text = (columnsDimension == null) ? null : TableController.GetLegendText (columnsDimension.Name.ToString ());
			this.rowsLegend.Text    = (rowsDimension    == null) ? null : TableController.GetLegendText (rowsDimension.Name.ToString ());

			int totalColumns = (columnsDimension == null) ? 1 : columnsDimension.Values.Count ();
			int totalRows    = (rowsDimension    == null) ? 1 : rowsDimension.Values.Count ();

			this.valuesTable.SetArraySize (totalColumns, totalRows);

			for (int i = 0; i < totalColumns; i++)
			{
				this.valuesTable.SetWidthColumn (i, 80);
				this.valuesTable.SetHeaderTextH (i, (columnsDimension == null) ? null : columnsDimension.Values[i]);
			}

			for (int i = 0; i < totalRows; i++)
			{
				this.valuesTable.SetHeaderTextV (i, (rowsDimension == null) ? null : rowsDimension.Values[i]);
			}

			int tabIndex = 1;
			for (int row = 0; row < totalRows; row++)
			{
				for (int column = 0; column < totalColumns; column++)
				{
					if (this.valuesTable[column, row].IsEmpty)
					{
						var field = new TextField
						{
							Name = string.Concat (column.ToString (System.Globalization.CultureInfo.InvariantCulture), ".", row.ToString (System.Globalization.CultureInfo.InvariantCulture)),
							ContentAlignment = Common.Drawing.ContentAlignment.MiddleRight,
							Dock = DockStyle.Fill,
							Margins = new Margins (-1),
							TabIndex = tabIndex++,
						};

						field.TextChanged += new EventHandler (this.HandleFieldTextChanged);

						this.valuesTable[column, row].Insert (field);
						this.valuesTable[column, row].TabIndex = tabIndex++;
					}

					if (!this.valuesTable[column, row].IsEmpty)
					{
						var field = this.valuesTable[column, row].Children[0] as TextField;

						field.Text = this.GetValue (this.GetKey (column, row));
					}
				}
			}
		}


		private void HandleSwapButtonClicked(object sender, MessageEventArgs e)
		{
			int t = this.columnDimensionIndex;
			this.columnDimensionIndex = this.rowDimensionIndex;
			this.rowDimensionIndex = t;

			this.RefreshDimensionsTable ();
			this.UpdateValuesTable ();
		}

		private void HandleRadioClicked(object sender, MessageEventArgs e)
		{
			var radio = sender as RadioButton;
			string column = radio.Name.Substring (0, 1);
			int row = int.Parse (radio.Name.Substring (1));

			if (column == "C")
			{
				this.columnDimensionIndex = row;
			}

			if (column == "R")
			{
				this.rowDimensionIndex = row;
			}

			this.RefreshDimensionsTable ();
			this.UpdateValuesTable ();
		}

		private void HandleComboClosed(object sender)
		{
			var combo = sender as TextFieldCombo;
			int row = int.Parse (combo.Name);

			this.UpdateValuesTable ();
		}

		private void HandleFieldTextChanged(object sender)
		{
			var field = sender as TextField;
			var parts = field.Name.Split ('.');
			int column = int.Parse (parts[0]);
			int row    = int.Parse (parts[1]);

			this.SetValue (this.GetKey (column, row), field.Text);
		}


		private int[] GetKey(int column, int row)
		{
			var list = new List<int> ();

			for (int n = 0; n < this.table.Dimensions.Count; n++)
			{
				int choice = this.GetDimensionChoice (n);

				if (choice == TableController.usedForColumn)
				{
					list.Add (column);
				}
				else if (choice == TableController.usedForRow)
				{
					list.Add (row);
				}
				else
				{
					list.Add (choice);
				}
			}

			return list.ToArray ();
		}

		private int GetDimensionChoice(int index)
		{
			//	Retourne le choix effectué dans l'interface, pour une dimension donnée.
			var radioX = this.dimensionsTable[0, index].Children[0] as RadioButton;
			var radioY = this.dimensionsTable[1, index].Children[0] as RadioButton;
			var combo  = this.dimensionsTable[3, index].Children[0] as TextFieldCombo;

			if (radioX.ActiveState == ActiveState.Yes)
			{
				return TableController.usedForColumn;
			}

			if (radioY.ActiveState == ActiveState.Yes)
			{
				return TableController.usedForRow;
			}

			int i = this.table.Dimensions[index].Values.IndexOf (combo.Text);

			if (i == -1)
			{
				i = 0;
			}

			return i;
		}


		private string GetValue(int[] indexes)
		{
			decimal? d = this.table.Values.GetValue (indexes);

			return TableController.PriceToString (d);
		}

		private void SetValue(int[] indexes, string value)
		{
			decimal d;
			if (decimal.TryParse (value, out d))
			{
				this.table.Values.SetValue (indexes, d);
			}
			else
			{
				this.table.Values.SetValue (indexes, null);
			}
		}


		private static string GetLegendText(string text)
		{
			return string.Concat ("<font size=\"16\"><b>", text, "</b></font>");
		}

		private static string PriceToString(decimal? value)
		{
			if (!value.HasValue)
			{
				return null;
			}

			DecimalRange decimalRange001 = new DecimalRange (-1000000000M, 1000000000M, 0.01M);
			return decimalRange001.ConvertToString (value.Value);
		}



		private static readonly double						legendHeight = 28;
		private static readonly int							usedForColumn = -2;
		private static readonly int							usedForRow = -3;

		private readonly DesignerTable						table;

		private FrameBox									dimensionsPane;
		private CellTable									dimensionsTable;
		private Button										swapButton;
		private VStaticText									rowsLegend;
		private StaticText									columnsLegend;
		private CellTable									valuesTable;

		private int											columnDimensionIndex;
		private int											rowDimensionIndex;
	}
}
