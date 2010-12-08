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
		public TableController(DimensionTable table)
		{
			this.table = table;

			this.columnDimensionIndex = 0;
			this.rowDimensionIndex = 1;
		}

		public void CreateUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (10),
			};

			var dimensionsPane = new FrameBox
			{
				Parent = frame,
				PreferredWidth = 24+24+100+70+20,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 10, 0, 0),
			};

			var valuesPane = new FrameBox
			{
				Parent = frame,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
			};

			this.CreateDimensionsTableUI (dimensionsPane);
			this.CreateValuesTableUI (valuesPane);

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
				Padding = new Margins (3, TileArrow.Breadth+3, 3, 3),
			};

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
			var rowsHeader = new FrameBox
			{
				Parent = parent,
				DrawFullFrame = true,
				PreferredWidth = 20,
				Dock = DockStyle.Left,
				Margins = new Margins (0, -1, 20-1, 0),
			};

			var f = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
			};

			var columnsHeader = new FrameBox
			{
				Parent = f,
				DrawFullFrame = true,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, -1),
			};

			this.valuesTable = new CellTable
			{
				Parent = f,
				StyleH = CellArrayStyles.ScrollNorm | CellArrayStyles.Header | CellArrayStyles.Separator,
				StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Header | CellArrayStyles.Separator,
				DefHeight = 24,
				Dock = DockStyle.Fill,
			};

			this.rowsLegend = new StaticText
			{
				Parent = rowsHeader,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
				Dock = DockStyle.Fill,
				// TODO: Comment afficher le texte verticalement ?
			};

			this.columnsLegend = new StaticText
			{
				Parent = columnsHeader,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
				Dock = DockStyle.Fill,
			};
		}


		private void UpdateDimensionsTable()
		{
			int count = this.table.Dimensions.Count ();

			this.dimensionsTable.SetArraySize (4, count);

			this.dimensionsTable.SetWidthColumn (0, 24);
			this.dimensionsTable.SetWidthColumn (1, 24);
			this.dimensionsTable.SetWidthColumn (2, 100);
			this.dimensionsTable.SetWidthColumn (3, 70);

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
					var text = new StaticText
					{
						Text = this.table.Dimensions.ElementAt (row).Name,
						ContentAlignment = Common.Drawing.ContentAlignment.MiddleLeft,
						Dock = DockStyle.Fill,
						Margins = new Margins (5, 5, 0, 0),
					};

					this.dimensionsTable[2, row].Insert (text);
					this.dimensionsTable[2, row].TabIndex = tabIndex++;
				}

				if (this.dimensionsTable[3, row].IsEmpty)
				{
					var field = new TextField
					{
						Dock = DockStyle.Fill,
						Margins = new Margins (-1),
						TabIndex = tabIndex++,
					};

					this.dimensionsTable[3, row].Insert (field);
					this.dimensionsTable[3, row].TabIndex = tabIndex++;
				}
			}
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

		private void RefreshDimensionsTable()
		{
			int count = this.table.Dimensions.Count ();

			for (int row = 0; row < count; row++)
			{
				var radioX = this.dimensionsTable[0, row].Children[0] as RadioButton;
				var radioY = this.dimensionsTable[1, row].Children[0] as RadioButton;
				var field  = this.dimensionsTable[3, row].Children[0] as TextField;

				radioX.ActiveState = (this.columnDimensionIndex == row) ? ActiveState.Yes : ActiveState.No;
				radioY.ActiveState = (this.rowDimensionIndex == row) ? ActiveState.Yes : ActiveState.No;

				field.Enable = (this.columnDimensionIndex != row && this.rowDimensionIndex != row);
			}
		}


		private void UpdateValuesTable()
		{
			AbstractDimension columnsDimension = this.table.Dimensions.ElementAt (this.columnDimensionIndex);
			AbstractDimension rowsDimension    = this.table.Dimensions.ElementAt (this.rowDimensionIndex);

			this.columnsLegend.Text = columnsDimension.Name;
			this.rowsLegend.Text = rowsDimension.Name;

			int totalColumns = columnsDimension.Values.Count ();
			int totalRows    = rowsDimension.Values.Count ();

			this.valuesTable.SetArraySize (totalColumns, totalRows);

			for (int i = 0; i < totalColumns; i++)
			{
				this.valuesTable.SetWidthColumn (i, 70);
				this.valuesTable.SetHeaderTextH (i, columnsDimension.GetValueAt (i));
			}

			for (int i = 0; i < totalRows; i++)
			{
				this.valuesTable.SetHeaderTextV (i, rowsDimension.GetValueAt (i));
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
							Text = this.GetValue (column, row),
							ContentAlignment = Common.Drawing.ContentAlignment.MiddleRight,
							Dock = DockStyle.Fill,
							Margins = new Margins (-1),
							TabIndex = tabIndex++,
						};

						this.valuesTable[column, row].Insert (field);
						this.valuesTable[column, row].TabIndex = tabIndex++;
					}
				}
			}
		}

		private string GetValue(params int[] indexes)
		{
			decimal? d = this.table[this.table.GetKey (indexes)];

			return TableController.PriceToString (d.Value);
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


		private readonly DimensionTable					table;

		private CellTable								dimensionsTable;
		private StaticText								rowsLegend;
		private StaticText								columnsLegend;
		private CellTable								valuesTable;

		private int										columnDimensionIndex;
		private int										rowDimensionIndex;
	}
}
