//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.DataFillers;
using Epsitec.Cresus.Assets.Server.NodesGetter;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Colonne de TreeTable affichant des chaînes avec une indication du niveau
	/// dans l'arborescence. Cette colonne est habituellement en mode DockToLeft.
	/// </summary>
	public class TreeTableColumnTree : AbstractTreeTableColumn
	{
		public bool								IndependentColumn;

		public override void SetGenericCells(TreeTableColumnItem columnItem)
		{
			this.cells = columnItem.GetArray<TreeTableCellTree> ();

			this.CreateTreeButtons ();
			this.Invalidate ();
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			if (this.IndependentColumn)
			{
				if (message.IsMouseType)
				{
					if (message.MessageType == MessageType.MouseDown)
					{
						this.ProcessMouseClick (pos);
					}
					else if (message.MessageType == MessageType.MouseMove)
					{
						this.ProcessMouseMove (pos);
					}
					else if (message.MessageType == MessageType.MouseLeave)
					{
						this.ProcessMouseLeave (pos);
					}
				}
			}

			base.ProcessMessage (message, pos);
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


		private void ProcessMouseClick(Point pos)
		{
			int row = this.DetectRow (pos);
			if (row != -1)
			{
				this.OnRowClicked (row);
			}
		}

		private void ProcessMouseMove(Point pos)
		{
			this.HilitedHoverRow = this.DetectRow (pos);
		}

		private void ProcessMouseLeave(Point pos)
		{
			this.HilitedHoverRow = -1;
		}

		private int DetectRow(Point pos)
		{
			int max = this.VisibleRowsCount;
			double h = this.ActualHeight - this.HeaderHeight - this.FooterHeight;
			double dy = h / max;

			double y = this.ActualHeight - this.HeaderHeight - pos.Y;
			if (y >= 0)
			{
				int row = (int) (y / dy);

				if (row >= 0 && row < max)
				{
					return row;
				}
			}

			return -1;
		}

		private int VisibleRowsCount
		{
			get
			{
				return (int) ((this.ActualHeight - this.HeaderHeight - this.FooterHeight) / this.RowHeight);
			}
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
					if (cell.Type == NodeType.Compacted ||
						cell.Type == NodeType.Expanded)
					{
						var rect = this.GetGlyphRectangle (y, cell.Level);

						var button = new GlyphButton
						{
							Parent      = this,
							GlyphShape  = cell.Type == NodeType.Compacted ? GlyphShape.ArrowRight : GlyphShape.ArrowDown,
							ButtonStyle = ButtonStyle.ToolItem,
							Name        = TreeTableColumnTree.Serialize (y, cell.Type),
						};

						button.SetManualBounds (rect);

						button.MouseMove += delegate (object sender, MessageEventArgs e)
						{
							//	Si la souris est bougée dans le bouton, il faut passer l'information
							//	au widget sous-jacent (TreeTable), afin que la ligne survolée soit
							//	mise en évidence.
							int row;
							NodeType type;
							TreeTableColumnTree.Deserialize (button.Name, out row, out type);
							this.OnChildrenMouseMove (row);
						};

						button.Clicked += delegate
						{
							int row;
							NodeType type;
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


		private static string Serialize(int row, NodeType type)
		{
			string s1 = row.ToString ();
			string s2 = ((int) type).ToString ();

			return string.Concat (s1, " ", s2);
		}

		private static void Deserialize(string text, out int row, out NodeType type)
		{
			var p = text.Split (' ');

			row = int.Parse (p[0]);

			int t = int.Parse (p[1]);
			type = (NodeType) t;
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

	
		private void OnTreeButtonClicked(int row, NodeType type)
		{
			if (this.TreeButtonClicked != null)
			{
				this.TreeButtonClicked (this, row, type);
			}
		}

		public delegate void TreeButtonClickedEventHandler(object sender, int row, NodeType type);
		public event TreeButtonClickedEventHandler TreeButtonClicked;
		#endregion


		private TreeTableCellTree[] cells;
	}
}
