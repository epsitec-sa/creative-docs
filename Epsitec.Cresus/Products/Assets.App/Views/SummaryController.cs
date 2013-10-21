//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	/// <summary>
	/// Affiche un tableau résumé contenant des textes (définis dans SummaryControllerTile).
	/// </summary>
	public class SummaryController
	{
		public SummaryController()
		{
			this.TileSize = new Size (100, 17);
		}


		public Size								TileSize;

		public void CreateUI(Widget parent)
		{
			this.frameBox = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};
		}

		public void SetTiles(List<List<SummaryControllerTile?>> tiles)
		{
			this.tiles = tiles;

			this.CreateTiles ();
		}

		private void CreateTiles()
		{
			this.frameBox.Children.Clear ();

			int columnsCount = this.ColumnsCount;
			int rowsCount    = this.RowsCount;

			for (int column = 0; column < columnsCount; column++ )
			{
				var columnFrame = new FrameBox
				{
					Parent         = this.frameBox,
					Dock           = DockStyle.Left,
					PreferredWidth = 120,
				};

				for (int row = 0; row < rowsCount; row++)
				{
					var button = new ColoredButton
					{
						Parent        = columnFrame,
						Name          = SummaryController.PutRowColumn (row, column),
						Dock          = DockStyle.Top,
						PreferredSize = this.TileSize,
						Margins       = new Margins (0, 1, 0, 1),
						TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
					};

					this.UpdateButton (button, this.GetTile (column, row));

					button.Clicked += delegate
					{
						int r, c;
						SummaryController.GetRowColumn (button.Name, out r, out c);
						this.OnTileClicked (r, c);
					};
				}
			}
		}


		private void UpdateButton(ColoredButton button, SummaryControllerTile? tile)
		{
			if (tile.HasValue)
			{
				if (tile.Value.Readonly)
				{
					button.NormalColor   = ColorManager.ReadonlyColor;
					button.SelectedColor = ColorManager.EditSinglePropertyColor;
					button.HoverColor    = ColorManager.ReadonlyColor;
				}
				else
				{
					button.NormalColor   = Color.FromBrightness (1.0);
					button.SelectedColor = ColorManager.EditSinglePropertyColor;
					button.HoverColor    = ColorManager.HoverColor;
				}

				if (tile.Value.Hilited)
				{
					button.ActiveState = ActiveState.Yes;
				}
				else
				{
					button.ActiveState = ActiveState.No;
				}
			}
			else
			{
				button.NormalColor   = ColorManager.EditBackgroundColor;
				button.SelectedColor = ColorManager.EditBackgroundColor;
				button.HoverColor    = ColorManager.EditBackgroundColor;

				button.ActiveState = ActiveState.No;
			}

			if (tile.HasValue)
			{
				button.ContentAlignment = tile.Value.Alignment;

				switch (tile.Value.Alignment)
				{
					case ContentAlignment.TopRight:
					case ContentAlignment.MiddleRight:
					case ContentAlignment.BottomRight:
						button.Text = tile.Value.Text + " ";
						break;

					case ContentAlignment.TopLeft:
					case ContentAlignment.MiddleLeft:
					case ContentAlignment.BottomLeft:
					button.Text = " " + tile.Value.Text;
						break;

					default:
						button.Text = tile.Value.Text;
						break;
				}
			}
			else
			{
				button.Text = null;
			}

			if (tile.HasValue && !string.IsNullOrEmpty (tile.Value.Tootip))
			{
				ToolTip.Default.SetToolTip (button, tile.Value.Tootip);
			}
			else
			{
				ToolTip.Default.ClearToolTip (button);
			}
		}

		private SummaryControllerTile? GetTile(int column, int row)
		{
			if (column < this.tiles.Count)
			{
				var rows = this.tiles[column];

				if (row < rows.Count)
				{
					return rows[row];
				}
			}

			return null;
		}

		private int ColumnsCount
		{
			get
			{
				return this.tiles.Count;
			}
		}

		private int RowsCount
		{
			get
			{
				return this.tiles.Max (column => column.Count);
			}
		}


		private static string PutRowColumn(int row, int column)
		{
			return string.Concat
			(
				row.ToString (System.Globalization.CultureInfo.InstalledUICulture),
				"/",
				column.ToString (System.Globalization.CultureInfo.InstalledUICulture)
			);
		}

		private static void GetRowColumn(string text, out int row, out int column)
		{
			var p = text.Split ('/');
			row    = int.Parse (p[0], System.Globalization.CultureInfo.InstalledUICulture);
			column = int.Parse (p[1], System.Globalization.CultureInfo.InstalledUICulture);
		}


		#region Events handler
		private void OnTileClicked(int row, int column)
		{
			if (this.TileClicked != null)
			{
				this.TileClicked (this, row, column);
			}
		}

		public delegate void TileClickedEventHandler(object sender, int row, int column);
		public event TileClickedEventHandler TileClicked;
		#endregion


		private FrameBox							frameBox;
		private List<List<SummaryControllerTile?>>	tiles;
	}
}
