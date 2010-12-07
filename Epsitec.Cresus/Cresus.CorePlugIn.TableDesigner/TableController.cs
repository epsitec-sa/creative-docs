//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.Finance.PriceCalculators;

using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.CorePlugIn.TableDesigner
{
	public class TableController
	{
		public TableController(DimensionTable table)
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

			var rowsHeader = new FrameBox
			{
				Parent = frame,
				DrawFullFrame = true,
				PreferredWidth = 20,
				Dock = DockStyle.Left,
				Margins = new Margins (0, -1, 20-1, 0),
			};

			var f = new FrameBox
			{
				Parent = frame,
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

			this.cellTable = new CellTable
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

			this.UpdateCellTable (this.table.Dimensions.ElementAt (0), this.table.Dimensions.ElementAt (1));
		}

		private void UpdateCellTable(AbstractDimension columnsDimension, AbstractDimension rowsDimension)
		{
			this.columnsLegend.Text = columnsDimension.Name;
			this.rowsLegend.Text = rowsDimension.Name;

			int totalColumns = columnsDimension.Values.Count ();
			int totalRows    = rowsDimension.Values.Count ();

			this.cellTable.SetArraySize (totalColumns, totalRows);

			for (int i = 0; i < totalColumns; i++)
			{
				this.cellTable.SetWidthColumn (i, 70);
				this.cellTable.SetHeaderTextH (i, this.GetDimensionsName (columnsDimension, i));
			}

			for (int i = 0; i < totalRows; i++)
			{
				this.cellTable.SetHeaderTextV (i, this.GetDimensionsName (rowsDimension, i));
			}

			int tabIndex = 1;
			for (int row = 0; row < totalRows; row++)
			{
				for (int column = 0; column < totalColumns; column++)
				{
					if (this.cellTable[column, row].IsEmpty)
					{
						var field = new TextField
						{
							Text = this.GetValue (columnsDimension, rowsDimension, column, row),
							ContentAlignment = Common.Drawing.ContentAlignment.MiddleRight,
							Dock = DockStyle.Fill,
							Margins = new Margins (-1),
							TabIndex = tabIndex++,
						};

						this.cellTable[column, row].Insert (field);
						this.cellTable[column, row].TabIndex = tabIndex++;
					}
				}
			}
		}

		private string GetDimensionsName(AbstractDimension dimension, int index)
		{
			if (dimension is NumericDimension)
			{
				var d = dimension as NumericDimension;

				decimal value = (decimal) d.Values.ElementAt (index);
				return value.ToString ();
			}

			if (dimension is CodeDimension)
			{
				var d = dimension as CodeDimension;

				return d.Values.ElementAt (index) as string;
			}

			return null;
		}

		private string GetValue(AbstractDimension columnsDimension, AbstractDimension rowsDimension, int column, int row)
		{
			// TODO: devrait obtenir toutes les dimensions !
			object c = columnsDimension.Values.ElementAt (column);
			object r = rowsDimension.Values.ElementAt (row);

			decimal? d = this.table[c, r];

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
		private StaticText								rowsLegend;
		private StaticText								columnsLegend;
		private CellTable								cellTable;
	}
}
