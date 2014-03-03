//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views;
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
		}


		public void SetContent(string[] texts, double[] fontFactors)
		{
			this.texts       = texts;
			this.fontFactors = fontFactors;

			this.PreferredWidth   = (this.graphicViewMode == GraphicViewMode.VerticalFinalNode) ? 20.0 : this.columnWidth;
			this.Margins          = new Margins (0, -1, 0, 0);
			this.Padding          = new Margins (GraphicViewTile.margins, GraphicViewTile.margins, GraphicViewTile.margins+this.TopPadding, GraphicViewTile.margins);
		}


		protected override void OnEntered(MessageEventArgs e)
		{
			this.entered = true;
			base.OnEntered (e);
		}

		protected override void OnExited(MessageEventArgs e)
		{
			this.entered = false;
			base.OnExited (e);
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
				var t = graphics.Transform;
				graphics.RotateTransformDeg (90.0, rect.Center.X, rect.Center.Y);

				var text = string.Join (" ", this.texts);
				var fontSize = this.GetFontSize (this.texts.Length-1);
				graphics.PaintText (this.RotatedRect, text, Font.DefaultFont, fontSize, ContentAlignment.MiddleLeft);

				graphics.Transform = t;
			}
			else
			{
				for (int i=0; i<this.texts.Length; i++)
				{
					graphics.PaintText (this.GetRect (i), this.texts[i], Font.DefaultFont, this.GetFontSize (i), ContentAlignment.TopLeft);
				}
			}

			rect.Deflate (0.5);
			graphics.AddRectangle (rect);
			graphics.RenderSolid (Color.FromBrightness (0.5));
		}


		private Rectangle RotatedRect
		{
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

		private Rectangle GetRect(int index)
		{
			var rect = this.Client.Bounds;
			var offset = this.GetTopOffset (index);
			var h = this.GetFontSize (index) * 1.2;
			return new Rectangle (rect.Left+GraphicViewTile.margins, rect.Top-offset-h, rect.Width-GraphicViewTile.margins*2, h);
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
				offset += this.GetFontSize (i) * 1.2;
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

			return (18.0 - this.level * 2.0) * factor;  // 18 .. 10 (si fontFactor = 1.0)
		}

		private Color BackgroundColor
		{
			get
			{
				Color color;

				if (this.entered)
				{
					color = ColorManager.HoverColor;
				}
				else
				{
					color = Color.FromBrightness (0.75);
				}

				var v = 0.75 + this.level * 0.05;  // 0.75 .. 0.95
				return color.ForceV (v);
			}
		}


		private const double margins = 10.0;

		private readonly int					level;
		private readonly double					columnWidth;
		private readonly NodeType				nodeType;
		private readonly GraphicViewMode		graphicViewMode;

		private string[]						texts;
		private double[]						fontFactors;
		private bool							entered;
	}
}