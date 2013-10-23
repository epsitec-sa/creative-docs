//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Colonne de TreeTable affichant des chaînes avec une indication du niveau
	/// dans l'arborescence. Cette colonne est habituellement en mode DockToLeft.
	/// </summary>
	public class TreeTableColumnTree : AbstractTreeTableColumn
	{
		public void SetCells(TreeTableCellTree[] cells)
		{
			this.cells = cells;

			this.CreateTreeButtons ();
			this.Invalidate ();
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation(graphics, clipRect);

			if (this.cells != null)
			{
				int y = 0;

				foreach (var cell in this.cells)
				{
					//	Dessine le fond.
					var rect = this.GetCellsRect (y);

					graphics.AddFilledRectangle (rect);
					graphics.RenderSolid (this.GetCellColor (y == this.hilitedHoverRow, cell.IsSelected));

					//	Dessine le texte.
					if (!string.IsNullOrEmpty (cell.Value))
					{
						var textRect = this.GetTextRectangle (y, cell.Level);
						this.PaintText (graphics, textRect, cell.Value);
					}

					//	Dessine la grille.
					this.PaintGrid (graphics, rect, y, this.hilitedHoverRow);

					y++;
				}
			}
		}

		protected override void OnSizeChanged(Size oldValue, Size newValue)
		{
			base.OnSizeChanged (oldValue, newValue);

			this.CreateTreeButtons ();
		}

		private void CreateTreeButtons()
		{
			//	Crée les petits boutons pour la gestion de l'arborescence.
			this.Children.Clear ();

			if (this.cells != null)
			{
				int y = 0;

				foreach (var cell in this.cells)
				{
					if (cell.Type == TreeTableTreeType.Compacted ||
						cell.Type == TreeTableTreeType.Expanded)
					{
						var rect = this.GetGlyphRectangle (y, cell.Level);

						var button = new GlyphButton
						{
							GlyphShape    = cell.Type == TreeTableTreeType.Compacted ? GlyphShape.ArrowRight : GlyphShape.ArrowDown,
							ButtonStyle   = ButtonStyle.ToolItem,
							PreferredSize = rect.Size,
							Anchor        = AnchorStyles.BottomLeft,
							Margins       = new Margins (rect.Left, 0, 0, rect.Bottom),
							Name          = TreeTableColumnTree.Serialize (y, cell.Type),
						};

						this.Children.Add (button);

						button.MouseMove += delegate (object sender, MessageEventArgs e)
						{
							//	Si la souris est bougée dans le bouton, il faut passer l'information
							//	au widget sous-jacent (TreeTable), afin que la ligne survolée soit
							//	mise en évidence.
							int row;
							TreeTableTreeType type;
							TreeTableColumnTree.Deserialize (button.Name, out row, out type);
							this.OnChildrenMouseMove (row);
						};

						button.Clicked += delegate
						{
							int row;
							TreeTableTreeType type;
							TreeTableColumnTree.Deserialize (button.Name, out row, out type);
							this.OnTreeButtonClicked (row, type);
						};
					}

					y++;
				}
			}
		}

		private Rectangle GetGlyphRectangle(int y, int level)
		{
			//	Retourne le rectangle pour le petit GlyphButton.
			var rect = this.GetCellsRect (y);
			rect.Deflate (this.DescriptionMargin*level, 0, 0, 0);

			return new Rectangle (rect.Left, rect.Bottom, rect.Height, rect.Height);
		}

		private Rectangle GetTextRectangle(int y, int level)
		{
			//	Retourne le rectangle pour le texte, en laissant la place pour le GlyphButton à gauche.
			var rect = this.GetCellsRect (y);
			rect.Deflate (this.DescriptionMargin*level + this.DescriptionMargin*5/2, 0, 0, 0);

			return rect;
		}


		private static string Serialize(int row, TreeTableTreeType type)
		{
			string s1 = row.ToString ();
			string s2 = ((int) type).ToString ();

			return string.Concat (s1, " ", s2);
		}

		private static void Deserialize(string text, out int row, out TreeTableTreeType type)
		{
			var p = text.Split (' ');

			row = int.Parse (p[0]);

			int t = int.Parse (p[1]);
			type = (TreeTableTreeType) t;
		}


		#region Events handler
		private void OnTreeButtonClicked(int row, TreeTableTreeType type)
		{
			if (this.TreeButtonClicked != null)
			{
				this.TreeButtonClicked (this, row, type);
			}
		}

		public delegate void TreeButtonClickedEventHandler(object sender, int row, TreeTableTreeType type);
		public event TreeButtonClickedEventHandler TreeButtonClicked;
		#endregion


		private TreeTableCellTree[] cells;
	}
}
