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
	/// Une tuile de AbstractTreeGraphicViewController.
	/// </summary>
	public class TreeGraphicViewTile : Widget
	{
		public TreeGraphicViewTile(int level, double columnWidth, NodeType nodeType, TreeGraphicViewMode graphicViewMode)
		{
			this.level           = level;
			this.columnWidth     = columnWidth;
			this.nodeType        = nodeType;
			this.graphicViewMode = graphicViewMode;

			this.textLayout = new TextLayout ();

			this.AutoDoubleClick = true;
		}


		public void SetContent(string[] texts, double[] fontFactors)
		{
			this.texts       = texts;
			this.fontFactors = fontFactors;

			//	La largeur donnée est automatiquement dépassée si les tuiles filles
			//	le demandent. Par exemple, en mode GraphicViewMode.VerticalFinalNode,
			//	on donne la largeur la plus petite, qui est systématiquement dépassée,
			//	sauf pour les tuiles finales.
			this.PreferredWidth = this.RequiredWidth;
			this.Margins        = new Margins (0, -1, 0, 0);
			this.Padding        = new Margins (TreeGraphicViewTile.externalMargins, TreeGraphicViewTile.externalMargins+1, TreeGraphicViewTile.externalMargins+this.TopPadding, TreeGraphicViewTile.externalMargins);
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

			if ((this.graphicViewMode & TreeGraphicViewMode.VerticalFinalNode) != 0 &&
				this.nodeType == NodeType.Final)
			{
				//	Une tuile finale s'affiche verticalement (tournée de 90 degrés CCW),
				//	avec une seule ligne qui appond tous les contenus.
				var t = graphics.Transform;
				graphics.RotateTransformDeg (90.0, rect.Center.X, rect.Center.Y);

				var fontSize = TreeGraphicViewTile.verticalFinalWidth * 0.5;
				this.PaintText (graphics, this.RotatedRect, this.RotatedText, fontSize, ContentAlignment.MiddleLeft);

				graphics.Transform = t;
			}
			else
			{
				for (int i=0; i<this.texts.Length; i++)
				{
					var r = this.GetRect (i);

					var x = this.HorizontalOffset;
					if (x != 0)
					{
						//	Déplace le rectangle, afin de toujours voir le début du texte.
						r.Offset (x, 0);
					}

					this.PaintText (graphics, r, this.GetText (i), this.GetFontSize (i), ContentAlignment.TopLeft);
				}
			}

			this.PaintTreeButton (graphics);

			//	Dessine le cadre.
			rect.Deflate (0.5);
			graphics.AddRectangle (rect);
			graphics.RenderSolid (Color.FromBrightness (0.5));
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
				if ((this.graphicViewMode & TreeGraphicViewMode.VerticalFinalNode) != 0 &&
					this.nodeType == NodeType.Final)
				{
					return TreeGraphicViewTile.verticalFinalWidth;
				}
				else
				{
					if ((this.graphicViewMode & TreeGraphicViewMode.FixedWidth) != 0)
					{
						return this.columnWidth;
					}
					else
					{
						double width = 0.0;

						int count;
						if ((this.graphicViewMode & TreeGraphicViewMode.AutoWidthFirstLine) != 0)
						{
							count = 1;
						}
						else
						{
							count = this.texts.Length;
						}

						for (int i=0; i<count; i++)
						{
							var text = this.GetText (i);
							var size = this.GetFontSize (i);

							var w = text.GetTextWidth (Font.DefaultFont, size);
							width = System.Math.Max (width, w);
						}

						width += TreeGraphicViewTile.internalMargins*2;
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

				for (int i=0; i<this.texts.Length; i++)
				{
					if (i == 0)
					{
						list.Add (this.texts[i].Bold ());
					}
					else
					{
						list.Add (this.texts[i]);
					}
				}

				return string.Join (" ", list);
			}
		}

		private Rectangle RotatedRect
		{
			//	Retourne le rectangle tourné de 90 degrés CCW.
			get
			{
				var rect = this.Client.Bounds;
				rect.Deflate (0, TreeGraphicViewTile.internalMargins);
				return this.Rotate (rect);
			}
		}

		private Rectangle Rotate(Rectangle rect)
		{
			var p1 = Transform.RotatePointDeg (this.Client.Bounds.Center, 90.0, rect.BottomLeft);
			var p2 = Transform.RotatePointDeg (this.Client.Bounds.Center, 90.0, rect.TopRight);

			return new Rectangle (p1, p2);
		}

		private string GetText(int index)
		{
			if (index == 0 && this.IsEntered)  // titre survolé ?
			{
				return this.texts[index].Color (ColorManager.SelectionColor.ForceV (0.6)).Bold ();
			}
			else
			{
				return this.texts[index];
			}
		}

		private Rectangle GetRect(int index)
		{
			//	Retourne le rectangle pour un texte, qui déborde volontairement à droite.
			var rect = this.Client.Bounds;
			var offset = this.GetTopOffset (index);
			var h = this.GetFontSize (index) * 1.5;
			return new Rectangle (rect.Left+TreeGraphicViewTile.internalMargins, rect.Top-offset-h, rect.Width, h);
		}

		private double TopPadding
		{
			get
			{
				return this.GetTopOffset (this.texts.Length);
			}
		}

		private double GetTopOffset(int index)
		{
			var offset = TreeGraphicViewTile.internalMargins;

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

			var fontSize = (18.0 - this.level * 2.0) * factor;  // 18 .. 10 (si fontFactor = 1.0)
			return System.Math.Max (fontSize, 9.0);
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

				if (this.nodeType == NodeType.Expanded)
				{
					path.MoveTo (new Point (rect.Center.X, rect.Bottom));
					path.LineTo (rect.TopLeft);
					path.LineTo (rect.TopRight);
					path.Close ();
				}
				else
				{
					path.MoveTo (new Point (rect.Center.X, rect.Top));
					path.LineTo (rect.BottomLeft);
					path.LineTo (rect.BottomRight);
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
					return new Rectangle (x, this.ActualHeight-16, 16, 16);
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
		private readonly double					columnWidth;
		private readonly NodeType				nodeType;
		private readonly TreeGraphicViewMode		graphicViewMode;
		private readonly TextLayout				textLayout;

		private string[]						texts;
		private double[]						fontFactors;
		private bool							isInside;
		private GlyphButton						treeButton;
	}
}