//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Server.NodeGetters;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class GraphicViewTile : Widget
	{
		public GraphicViewTile(int level, double columnWidth, NodeType nodeType, GraphicViewMode graphicViewMode)
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
			this.PreferredWidth = (this.graphicViewMode == GraphicViewMode.VerticalFinalNode) ? GraphicViewTile.verticalFinalWidth : this.columnWidth;
			this.Margins        = new Margins (0, -1, 0, 0);
			this.Padding        = new Margins (GraphicViewTile.margins, GraphicViewTile.margins, GraphicViewTile.margins+this.TopPadding, GraphicViewTile.margins);
		}


		protected override void OnMouseMove(MessageEventArgs e)
		{
			this.IsInside = this.CheckInside (e.Point);
			base.OnMouseMove (e);
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var rect = this.Client.Bounds;

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (this.BackgroundColor);

			graphics.Color = Color.FromBrightness (0.2);

			if (this.graphicViewMode == GraphicViewMode.VerticalFinalNode &&
				this.nodeType == NodeType.Final)
			{
				//	Une tuile finale s'affiche verticalement (tournée de 90 degrés CCW),
				//	avec une seule ligne qui appond tous les contenus.
				var t = graphics.Transform;
				graphics.RotateTransformDeg (90.0, rect.Center.X, rect.Center.Y);

				var fontSize = GraphicViewTile.verticalFinalWidth * 0.5;
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
			this.textLayout.LayoutSize      = rect.Size;
			this.textLayout.BreakMode       = TextBreakMode.Split | TextBreakMode.SingleLine;
			this.textLayout.Alignment       = alignment;

			this.textLayout.Paint (rect.BottomLeft, graphics, rect, ColorManager.TextColor, GlyphPaintStyle.Normal);
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
				rect.Deflate (0, GraphicViewTile.margins);
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
			if (index == 0 && this.IsEntered)
			{
				return this.texts[index].Bold ();
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
			return new Rectangle (rect.Left+GraphicViewTile.margins, rect.Top-offset-h, rect.Width, h);
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
			var offset = GraphicViewTile.margins;

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

				while (w.Parent != null)
				{
					if (w is Scrollable)
					{
						if (-x > 0.0 && -x < this.ActualWidth)
						{
							return -x;
						}
						else
						{
							return 0.0;
						}
					}

					x += w.ActualLocation.X;
					w = w.Parent;
				}

				throw new System.InvalidOperationException ("Invalid parent");
			}
		}


		private const double margins            = 10.0;
		private const double verticalFinalWidth = 22.0;

		private readonly int					level;
		private readonly double					columnWidth;
		private readonly NodeType				nodeType;
		private readonly GraphicViewMode		graphicViewMode;
		private readonly TextLayout				textLayout;

		private string[]						texts;
		private double[]						fontFactors;
		private bool							isInside;
	}
}