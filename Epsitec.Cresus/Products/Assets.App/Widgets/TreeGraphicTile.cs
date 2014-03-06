//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Une tuile de AbstractTreeGraphicController.
	/// </summary>
	public class TreeGraphicTile : Widget
	{
		public TreeGraphicTile(int level, double fontSize, double columnWidth, NodeType nodeType, TreeGraphicMode graphicViewMode)
		{
			this.level           = level;
			this.fontSize        = fontSize;
			this.columnWidth     = columnWidth;
			this.nodeType        = nodeType;
			this.graphicViewMode = graphicViewMode;

			this.textLayout = new TextLayout ();

			this.AutoDoubleClick = true;
		}


		public decimal							MinAmount;
		public decimal							MaxAmount;


		public void SetContent(TreeGraphicValue[] values, double[] fontFactors)
		{
			this.values      = values;
			this.fontFactors = fontFactors;

			//	La largeur donnée est automatiquement dépassée si les tuiles filles
			//	le demandent. Par exemple, en mode GraphicViewMode.VerticalFinalNode,
			//	on donne la largeur la plus petite, qui est systématiquement dépassée,
			//	sauf pour les tuiles finales.
			this.PreferredWidth = this.RequiredWidth;
			this.Margins        = new Margins (0, -1, 0, 0);
			this.Padding        = new Margins (TreeGraphicTile.externalMargins, TreeGraphicTile.externalMargins+1, TreeGraphicTile.externalMargins+this.TopPadding, TreeGraphicTile.externalMargins);
		}


		protected override void OnMouseMove(MessageEventArgs e)
		{
			this.IsInside = this.CheckInside (e.Point);
			base.OnMouseMove (e);
		}

		protected override void OnClicked(MessageEventArgs e)
		{
			if (this.TreeButtonRectangle.Contains (e.Point))
			{
				this.OnTreeButtonClicked ();
			}
			else
			{
				base.OnClicked (e);
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var rect = this.Client.Bounds;

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (this.BackgroundColor);

			graphics.Color = Color.FromBrightness (0.2);

			if ((this.graphicViewMode & TreeGraphicMode.VerticalFinalNode) != 0 &&
				this.nodeType == NodeType.Final)
			{
				//	Une tuile finale s'affiche verticalement (tournée de 90 degrés CCW),
				//	avec une seule ligne qui appond tous les contenus.
				var t = graphics.Transform;
				graphics.RotateTransformDeg (90.0, rect.Center.X, rect.Center.Y);

				var fontSize = TreeGraphicTile.verticalFinalWidth * 0.5;
				this.PaintText (graphics, this.RotatedRect, this.RotatedText, fontSize, ContentAlignment.MiddleLeft);

				graphics.Transform = t;
			}
			else
			{
				for (int i=0; i<this.values.Length; i++)
				{
					var textRect = this.GetTextRect (i);
					var amountRect = this.GetAmountRect (i);

					var x = this.HorizontalOffset;
					if (x != 0)
					{
						//	Déplace le rectangle, afin de toujours voir le début du texte.
						textRect.Offset (x, 0);
					}

					this.PaintValue (graphics, textRect, amountRect, this.GetValue (i), this.GetFontSize (i), ContentAlignment.TopLeft);
				}
			}

			this.PaintTreeButton (graphics);

			//	Dessine le cadre.
			rect.Deflate (0.5);
			graphics.AddRectangle (rect);
			graphics.RenderSolid (Color.FromBrightness (0.5));
		}


		private void PaintValue(Graphics graphics, Rectangle textRect, Rectangle amountRect, TreeGraphicValue value, double fontSize, ContentAlignment alignment)
		{
			if (value.IsAmount)
			{
				var r1 = new Rectangle (textRect.Left, textRect.Bottom+textRect.Height/2, textRect.Width, textRect.Height/2);
				var r2 = new Rectangle (amountRect.Left, amountRect.Bottom, amountRect.Width, amountRect.Height/2);

				this.PaintText (graphics, r1, value.Text, fontSize/2, alignment);
				this.PaintAmount (graphics, r2, value);
			}
			else
			{
				this.PaintText (graphics, textRect, value.Text, fontSize, alignment);
			}
		}

		private void PaintText(Graphics graphics, Rectangle rect, string text, double fontSize, ContentAlignment alignment)
		{
			//	Dessine un texte inclu dans un rectangle.
			this.textLayout.Text            = text;
			this.textLayout.DefaultFont     = Font.DefaultFont;
			this.textLayout.DefaultFontSize = fontSize;
			this.textLayout.DefaultColor    = ColorManager.TextColor;
			this.textLayout.LayoutSize      = rect.Size;
			this.textLayout.BreakMode       = TextBreakMode.Split | TextBreakMode.SingleLine;
			this.textLayout.Alignment       = alignment;

			this.textLayout.Paint (rect.BottomLeft, graphics, rect, Color.Empty, GlyphPaintStyle.Normal);
		}

		private void PaintAmount(Graphics graphics, Rectangle rect, TreeGraphicValue value)
		{
			//	Dessine un montant sous la forme d'une barre graphique.
			rect = graphics.Align (rect);
			rect.Deflate (0.5, 1.5);

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (this.BackgroundColor.Delta (0.1));

			var amount = value.Amount.GetValueOrDefault ();

			if (amount < 0.0m)
			{
				int x1 = this.GetX (rect, amount);
				int x2 = this.GetX (rect, 0.0m);
				var r = new Rectangle (x1, rect.Bottom, x2-x1, rect.Height);

				graphics.AddFilledRectangle (r);
				graphics.RenderSolid (this.BackgroundColor.Delta (-0.1));
			}

			if (amount > 0.0m)
			{
				int x1 = this.GetX (rect, 0.0m);
				int x2 = this.GetX (rect, amount);
				var r = new Rectangle (x1, rect.Bottom, x2-x1, rect.Height);

				graphics.AddFilledRectangle (r);
				graphics.RenderSolid (this.BackgroundColor.Delta (-0.1));
			}

			graphics.AddRectangle (rect);
			graphics.RenderSolid (ColorManager.TextColor);
		}

		private int GetX(Rectangle rect, decimal amount)
		{
			decimal factor = (amount - this.MinAmount) / (this.MaxAmount - this.MinAmount);
			return (int) (rect.Left + rect.Width * (double) factor);
		}

		private void PaintTreeButton(Graphics graphics)
		{
			//	Dessine le bouton en forme de petit triangle en haut à gauche.
			if (this.IsEntered)
			{
				var rect = this.TreeButtonRectangle;

				if (!rect.IsEmpty)
				{
					graphics.AddFilledPath (this.TreeButtonPath);
					graphics.RenderSolid (ColorManager.GlyphColor);
				}
			}
		}


		private double RequiredWidth
		{
			//	Retourne la largeur requise pour la tuile.
			get
			{
				if ((this.graphicViewMode & TreeGraphicMode.VerticalFinalNode) != 0 &&
					this.nodeType == NodeType.Final)
				{
					return TreeGraphicTile.verticalFinalWidth;
				}
				else
				{
					if ((this.graphicViewMode & TreeGraphicMode.FixedWidth) != 0)
					{
						return this.columnWidth;
					}
					else
					{
						double width = 0.0;

						int count;
						if ((this.graphicViewMode & TreeGraphicMode.AutoWidthFirstLine) != 0)
						{
							count = 1;
						}
						else
						{
							count = this.values.Length;
						}

						for (int i=0; i<count; i++)
						{
							var text = this.GetValue (i).Text;
							var size = this.GetFontSize (i);

							var w = text.GetTextWidth (Font.DefaultFont, size);
							width = System.Math.Max (width, w);
						}

						width += TreeGraphicTile.internalMargins*2;
						return System.Math.Max (width, this.columnWidth);
					}
				}
			}
		}


		private string RotatedText
		{
			//	Retourne le texte tourné, constitué de toutes les lignes.
			get
			{
				var list = new List<string> ();

				for (int i=0; i<this.values.Length; i++)
				{
					if (i == 0)
					{
						list.Add (this.values[i].Text.Bold ());
					}
					else
					{
						list.Add (this.values[i].Text);
					}
				}

				return string.Join (" ", list);
			}
		}

		private Rectangle RotatedRect
		{
			//	Retourne le rectangle tourné de 90 degrés CCW, qui déborde volontairement.
			get
			{
				var b = this.Client.Bounds;
				var rect = new Rectangle (b.Left, b.Bottom-100, b.Width, b.Height+100);
				rect.Deflate (0, TreeGraphicTile.internalMargins);
				return this.Rotate (rect);
			}
		}

		private Rectangle Rotate(Rectangle rect)
		{
			var p1 = Transform.RotatePointDeg (this.Client.Bounds.Center, 90.0, rect.BottomLeft);
			var p2 = Transform.RotatePointDeg (this.Client.Bounds.Center, 90.0, rect.TopRight);

			return new Rectangle (p1, p2);
		}

		private TreeGraphicValue GetValue(int index)
		{
			return this.values[index];
		}

		private Rectangle GetTextRect(int index)
		{
			//	Retourne le rectangle pour un texte, qui déborde volontairement à droite.
			var rect = this.Client.Bounds;
			var offset = this.GetTopOffset (index);
			var h = this.GetFontSize (index) * 1.5;
			return new Rectangle (rect.Left+TreeGraphicTile.internalMargins, rect.Top-offset-h, rect.Width, h);
		}

		private Rectangle GetAmountRect(int index)
		{
			//	Retourne le rectangle pour un montant.
			var rect = this.Client.Bounds;
			var offset = this.GetTopOffset (index);
			var h = this.GetFontSize (index) * 1.5;
			return new Rectangle (rect.Left+TreeGraphicTile.internalMargins, rect.Top-offset-h, rect.Width-TreeGraphicTile.internalMargins*2, h);
		}

		private double TopPadding
		{
			get
			{
				return this.GetTopOffset (this.values.Length);
			}
		}

		private double GetTopOffset(int index)
		{
			var offset = TreeGraphicTile.internalMargins;

			for (int i=0; i<index; i++)
			{
				offset += this.GetFontSize (i) * 1.5;
			}

			return offset;
		}

		private double GetFontSize(int index)
		{
			var factor = 1.0;

			if (index < this.fontFactors.Length)
			{
				factor = this.fontFactors[index];
			}

			return this.fontSize * factor;
		}

		private Color BackgroundColor
		{
			get
			{
				if (this.ActiveState == ActiveState.Yes)
				{
					return ColorManager.SelectionColor;
				}
				else
				{
					if (this.IsInside)
					{
						return ColorManager.HoverColor;
					}
					else
					{
						var color = Color.FromBrightness (0.75);
						var v = 0.75 + this.level * 0.05;  // 0.75 .. 0.95
						return color.ForceV (v);
					}
				}
			}
		}


		private Path TreeButtonPath
		{
			get
			{
				var path = new Path ();
				var rect = this.TreeButtonRectangle;
				rect.Deflate (3.0, 4.0);

				if (this.nodeType == NodeType.Expanded)  // triangle ^ ?
				{
					path.MoveTo (new Point (rect.Center.X, rect.Top));
					path.LineTo (rect.BottomLeft);
					path.LineTo (rect.BottomRight);
					path.Close ();
				}
				else  // triangle v ?
				{
					path.MoveTo (new Point (rect.Center.X, rect.Bottom));
					path.LineTo (rect.TopLeft);
					path.LineTo (rect.TopRight);
					path.Close ();
				}

				return path;
			}
		}

		private Rectangle TreeButtonRectangle
		{
			get
			{
				if (this.nodeType == NodeType.Final)
				{
					return Rectangle.Empty;
				}
				else
				{
					var x = this.HorizontalOffset;
					return new Rectangle (x, this.ActualHeight-14, 14, 14);
				}
			}
		}


		private bool IsInside
		{
			get
			{
				return this.isInside && this.IsEntered;
			}
			set
			{
				if (this.isInside != value)
				{
					this.isInside = value;
					this.Invalidate ();  // il faut redessiner
				}
			}
		}

		private bool CheckInside(Point pos)
		{
			//	Retourne true si la souris est dans la tuile, sans être dans
			//	une tuile fille.
			foreach (var children in this.Children)
			{
				if (children.ActualBounds.Contains (pos))
				{
					return false;
				}
			}

			return true;
		}


		private double HorizontalOffset
		{
			//	Retourne l'offset horizontal pour afficher le texte dans la tuile, afin
			//	que le début du texte soit visible si la partie gauche de la tuile est
			//	partiellement cachée.
			get
			{
				double x = 0.0;
				Widget w = this;

				//	On remonte jusqu'au parent de type Scrollable.
				while (w.Parent != null)
				{
					if (w is Scrollable)
					{
						if (x > 0.0 && x < this.ActualWidth)
						{
							return x;
						}
						else
						{
							return 0.0;
						}
					}

					x -= w.ActualLocation.X;
					w = w.Parent;
				}

				throw new System.InvalidOperationException ("Invalid parent");
			}
		}


		#region Events handler
		private void OnTreeButtonClicked()
		{
			this.TreeButtonClicked.Raise (this);
		}

		public event EventHandler TreeButtonClicked;
		#endregion

	
		private const double externalMargins    = 10.0;
		private const double internalMargins    = 10.0;
		private const double verticalFinalWidth = 22.0;

		private readonly int					level;
		private readonly double					fontSize;
		private readonly double					columnWidth;
		private readonly NodeType				nodeType;
		private readonly TreeGraphicMode		graphicViewMode;
		private readonly TextLayout				textLayout;

		private TreeGraphicValue[]				values;
		private double[]						fontFactors;
		private bool							isInside;
	}
}