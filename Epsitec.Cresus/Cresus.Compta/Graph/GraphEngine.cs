//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta;

using System.Collections.Generic;

namespace Epsitec.Cresus.Compta.Graph
{
	public class GraphEngine
	{
		public GraphEngine()
		{
		}


		public Cube Cube
		{
			get
			{
				return this.cube;
			}
			set
			{
				this.cube = value;
			}
		}

		public GraphOptions Options
		{
			get
			{
				return this.options;
			}
			set
			{
				this.options = value;
			}
		}


		public void PaintFull(Graphics graphics, Rectangle rect)
		{
			//	Dessine un graphique complet.
			int nx = this.cube.GetCount (this.options.PrimaryDimension);
			int ny = this.cube.GetCount (this.options.SecondaryDimension);

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (this.BackExtColor);

			rect.Deflate (10);

			if (this.options.Mode == GraphMode.Array)
			{
				int dxHope = (int) (rect.Width  / (nx+1));
				int dyHope = (int) (rect.Height / (ny+1));
				this.fontSize = System.Math.Min (dxHope * 0.2, dyHope * 0.6);
			}
			else
			{
				this.fontSize = 10.0;
			}

			int labelDx;
			int labelDy = this.GetHorizontalLabelsHeight (rect);

			if (this.HasVerticalLabels)
			{
				labelDx = this.GetVerticalLabelsWidth (rect, ny);
			}
			else
			{
				labelDx = 0;
			}

			int dx = ((int) rect.Width  - labelDx) / nx;
			int dy = ((int) rect.Height - labelDy) / ny;

			if (dx <= 5 || dy <= 5)
			{
				this.PaintError (graphics, rect);
				return;
			}

			var frameRect = new Rectangle (rect.Left+labelDx, rect.Bottom+labelDy, rect.Width-labelDx, rect.Height-labelDy);
			this.PaintGrid (graphics, frameRect, nx, dx, ny, dy);

			switch (this.options.Mode)
			{
				case GraphMode.SideBySide:
					this.InitSideBySide (graphics, frameRect, nx, dx, ny);
					break;

				case GraphMode.Stacked:
					this.InitStacked (graphics, frameRect, nx, dx, ny);
					break;

				case GraphMode.Lines:
					this.InitLines (graphics, frameRect, nx, dx, ny);
					break;

				case GraphMode.Array:
					break;
			}

			if (this.HasHorizontalLabels)
			{
				var labelsRect = new Rectangle (rect.Left+labelDx, rect.Bottom, rect.Width-labelDx, labelDy);
				this.PaintHorizontalLabels (graphics, labelsRect, nx, dx);
			}

			if (this.HasVerticalLabels)
			{
				var labelsRect = new Rectangle (rect.Left, rect.Bottom+labelDy, labelDx, rect.Height-labelDy);
				this.PaintVerticalLabels (graphics, labelsRect, ny, dy);
			}

			switch (this.options.Mode)
			{
				case GraphMode.SideBySide:
					this.PaintSideBySide (graphics, frameRect, nx, dx, ny);
					break;

				case GraphMode.Stacked:
					this.PaintStacked (graphics, frameRect, nx, dx, ny);
					break;

				case GraphMode.Lines:
					this.PaintLines (graphics, frameRect, nx, dx, ny);
					break;

				case GraphMode.Array:
					this.PaintArray (graphics, frameRect, nx, dx, ny, dy);
					break;
			}

			if (this.options.Mode != GraphMode.Array)
			{
				var size = this.GetLegendsSize (this.options.SecondaryDimension);
				var legendsRect = new Rectangle (rect.Right-size.Width-10, rect.Top-size.Height-10, size.Width, size.Height);
				this.PaintLegends (graphics, legendsRect, this.options.SecondaryDimension);
			}
		}


		private void PaintError(Graphics graphics, Rectangle rect)
		{
			graphics.AddLine (rect.BottomLeft, rect.TopRight);
			graphics.AddLine (rect.BottomRight, rect.TopLeft);
			graphics.RenderSolid (this.BorderColor);
		}


		private void InitSideBySide(Graphics graphics, Rectangle frameRect, int nx, int dx, int ny)
		{
			this.cube.GetMinMax (null, null, out this.minValue, out this.maxValue);

			this.minValue = System.Math.Min (this.minValue, 0);
			this.maxValue = System.Math.Max (this.maxValue, 0);

			this.drawBottom = frameRect.Bottom+0.5;
			this.drawHeight = frameRect.Height;
		}

		private void PaintSideBySide(Graphics graphics, Rectangle frameRect, int nx, int dx, int ny)
		{
			for (int x = 0; x < nx; x++)
			{
				var rect = new Rectangle (frameRect.Left+x*dx+0.5, frameRect.Bottom+0.5, dx, frameRect.Height);
				rect.Deflate (2, 2, 0, 0);

				double zero = this.ConvValueToY (0);
				int barWidth = (int) rect.Width / ny;

				for (int pass = 0; pass < 2; pass++)
				{
					for (int y = 0; y < ny; y++)
					{
						decimal? value = this.GetValue (x, y);

						if (value.HasValue)
						{
							double h = this.ConvValueToY (value.Value);
							double h1 = System.Math.Min (h, zero);
							double h2 = System.Math.Max (h, zero);
							var barRect = new Rectangle (rect.Left+barWidth*y, h1, barWidth, h2-h1);

							if (pass == 0)
							{
								graphics.AddFilledRectangle (barRect);
								graphics.RenderSolid (this.GetIndexedColor (y, ny));
							}
							else
							{
								graphics.AddRectangle (barRect);
								graphics.RenderSolid (this.BorderColor);
							}
						}
					}
				}
			}
		}


		private void InitStacked (Graphics graphics, Rectangle frameRect, int nx, int dx, int ny)
		{
			this.minValue = 0;
			this.maxValue = 0;

			for (int x = 0; x < nx; x++)
			{
				decimal positivSum = 0;
				decimal negativSum = 0;

				for (int y = 0; y < ny; y++)
				{
					var value = this.GetValue (x, y).GetValueOrDefault ();

					if (value >= 0)
					{
						positivSum += value;
						this.maxValue = System.Math.Max (this.maxValue, positivSum);
					}
					else
					{
						negativSum += value;
						this.minValue = System.Math.Min (this.minValue, negativSum);
					}
				}
			}

			this.drawBottom = frameRect.Bottom+0.5;
			this.drawHeight = frameRect.Height;
		}

		private void PaintStacked(Graphics graphics, Rectangle frameRect, int nx, int dx, int ny)
		{
			for (int x = 0; x < nx; x++)
			{
				var rect = new Rectangle (frameRect.Left+x*dx+0.5, frameRect.Bottom+0.5, dx, frameRect.Height);
				int margin = (int) (rect.Width*0.25);

				if (margin == 1)
				{
					margin = 0;
				}

				rect.Deflate (margin, margin, 0, 0);

				for (int pass = 0; pass < 2; pass++)
				{
					decimal lastPositiv = 0;
					decimal lastNegativ = 0;

					for (int y = 0; y < ny; y++)
					{
						var value = this.GetValue (x, y).GetValueOrDefault ();
						decimal v1, v2;

						if (value >= 0)
						{
							v1 = lastPositiv;
							v2 = lastPositiv + value;

							lastPositiv += value;
						}
						else
						{
							v1 = lastNegativ + value;
							v2 = lastNegativ;

							lastNegativ += value;
						}

						double h1 = this.ConvValueToY (v1);
						double h2 = this.ConvValueToY (v2);
						var barRect = new Rectangle (rect.Left, h1, rect.Width, h2-h1);

						if (pass == 0)
						{
							graphics.AddFilledRectangle (barRect);
							graphics.RenderSolid (this.GetIndexedColor (y, ny));
						}
						else
						{
							graphics.AddRectangle (barRect);
							graphics.RenderSolid (this.BorderColor);
						}
					}
				}
			}
		}


		private void InitLines(Graphics graphics, Rectangle frameRect, int nx, int dx, int ny)
		{
			this.cube.GetMinMax (null, null, out this.minValue, out this.maxValue);

			this.drawBottom = frameRect.Bottom+7;
			this.drawHeight = frameRect.Height-7*2;
		}
		
		private void PaintLines(Graphics graphics, Rectangle frameRect, int nx, int dx, int ny)
		{
			var rect = frameRect;
			rect.Deflate (0, 7);

			for (int y = 0; y < ny; y++)
			{
				for (int pass = 0; pass < 2; pass++)
				{
					var last = Point.Zero;

					for (int x = 0; x < nx; x++)
					{
						decimal? value = this.GetValue (x, y);

						if (value.HasValue)
						{
							double h = this.ConvValueToY (value.Value);
							var pos = new Point (rect.Left+dx*x+dx*0.5, h);

							if (pass == 0)
							{
								if (!last.IsZero)
								{
									graphics.LineWidth = 2+2;
									graphics.AddLine (last, pos);
									graphics.RenderSolid (this.BorderColor);

									graphics.LineWidth = 2;
									graphics.AddLine (last, pos);
									graphics.RenderSolid (this.GetIndexedColor (y, ny));

									graphics.LineWidth = 1;
								}
							}
							else
							{
								graphics.AddFilledCircle (pos, 4);
								graphics.RenderSolid (this.BorderColor);

								graphics.AddFilledCircle (pos, 3);
								graphics.RenderSolid (this.GetIndexedColor (y, ny));

								graphics.AddFilledCircle (pos, 2);
								graphics.RenderSolid (Color.FromName ("White"));
							}

							last = pos;
						}
					}
				}
			}
		}


		private void PaintArray(Graphics graphics, Rectangle frameRect, int nx, int dx, int ny, int dy)
		{
			for (int x = 0; x < nx; x++)
			{
				for (int y = 0; y < ny; y++)
				{
					decimal value = this.GetValue (x, y).GetValueOrDefault ();

					if (value != 0)
					{
						var rect = new Rectangle (frameRect.Left+x*dx, frameRect.Bottom+y*dy, dx-5, dy);
						var text = Converters.MontantToString (value, null);
						this.PaintText (graphics, rect, text, ContentAlignment.MiddleRight);
					}
				}
			}
		}


		private double ConvValueToY(decimal value)
		{
			//	Conversion d'un montant en coordonnée Y, selon le type du graphe.
			return this.drawBottom + System.Math.Floor (this.drawHeight * (double) (value-this.minValue) / (double) (this.maxValue-this.minValue));
		}

		private decimal? GetValue(int x, int y)
		{
			if (this.options.PrimaryDimension == 0)
			{
				return this.cube.GetValue (x, y);
			}
			else
			{
				return this.cube.GetValue (y, x);
			}
		}


		private Size GetLegendsSize(int dimension)
		{
			//	Retourne la taille nécessaire pour la légende.
			var textLayout = new TextLayout
			{
				LayoutSize      = new Size (1000, 100),
				DefaultFontSize = this.fontSize,
			};

			int n = this.cube.GetCount (dimension);
			int dy = (int) (this.fontSize / 0.7);
			int max = 0;

			for (int i = 0; i < n; i++)
			{
				textLayout.FormattedText = this.cube.GetTitle (dimension, i);
				double width = textLayout.GetSingleLineSize ().Width;
				max = System.Math.Max (max, (int) width);
			}

			return new Size (max+dy+10, n*dy);
		}

		private void PaintLegends(Graphics graphics, Rectangle rect, int dimension)
		{
			//	Dessine la légende complète.
			int n = this.cube.GetCount (dimension);
			int dy = (int) (this.fontSize / 0.7);

			rect.Inflate (2);
			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (this.BackLegendsColor);
			rect.Deflate (2);

			for (int y = 0; y < n; y++)
			{
				var lineRect = new Rectangle (rect.Left, rect.Top-(y+1)*dy, rect.Width, dy);
				var text = this.cube.GetTitle (dimension, y);

				this.PaintLegend (graphics, lineRect, this.GetIndexedColor (y, n), text);
			}

			rect.Inflate (2.5);
			graphics.AddRectangle (rect);
			graphics.RenderSolid (this.BorderColor);
		}

		private void PaintLegend(Graphics graphics, Rectangle rect, Color color, FormattedText text)
		{
			//	Dessine un élément de la légende.
			rect.Deflate (1);

			var sampleRect = new Rectangle (rect.Left, rect.Bottom, rect.Height, rect.Height);
			graphics.AddFilledRectangle (sampleRect);
			graphics.RenderSolid (color);
			sampleRect.Deflate (0.5);
			graphics.AddRectangle (sampleRect);
			graphics.RenderSolid (this.BorderColor);

			var textRect = new Rectangle (rect.Left+rect.Height+5, rect.Bottom, rect.Width-rect.Height-5, rect.Height);
			this.PaintText (graphics, textRect, text, ContentAlignment.MiddleLeft);
		}


		private void PaintGrid(Graphics graphics, Rectangle frameRect, int nx, int dx, int ny, int dy)
		{
			//	Dessine la grille de fond.
			if (this.options.Mode == GraphMode.Lines)
			{
				frameRect.Offset (dx/2, 0);
				nx--;
			}

			int w = (int) (this.HasHorizontalLabels             ? nx*dx : frameRect.Width);
			int h = (int) (this.options.Mode == GraphMode.Array ? ny*dy : frameRect.Height);

			var rect = new Rectangle (frameRect.Left, frameRect.Bottom, w, h);

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (this.BackIntColor);

			if (this.HasHorizontalLabels)
			{
				this.PaintGridX (graphics, frameRect, nx, dx, h);
			}

			if (this.options.Mode == GraphMode.Array)
			{
				this.PaintGridY (graphics, frameRect, ny, dy, w);
			}
		}

		private void PaintGridX(Graphics graphics, Rectangle frameRect, int nx, int dx, int h)
		{
			//	Dessine la grille en avançant horizontalement, composée de traits verticaux.
			for (int x = 0; x < nx; x++)
			{
				var rect = new Rectangle (frameRect.Left+x*dx+0.5, frameRect.Bottom+0.5, dx, h);
				graphics.AddRectangle (rect);

				this.drawRight = rect.Right;
			}

			graphics.RenderSolid (this.BorderColor);
		}

		private void PaintGridY(Graphics graphics, Rectangle frameRect, int ny, int dy, int w)
		{
			//	Dessine la grille en avançant verticalement, composée de traits horizontaux.
			for (int y = 0; y < ny; y++)
			{
				var rect = new Rectangle (frameRect.Left+0.5, frameRect.Bottom+y*dy+0.5, w, dy);
				graphics.AddRectangle (rect);
			}

			graphics.RenderSolid (this.BorderColor);
		}


		private bool HasHorizontalLabels
		{
			//	Y a-t-il un axe horizontal avec des noms ?
			get
			{
				return this.options.Mode != GraphMode.Pie;
			}
		}

		private bool HasVerticalLabels
		{
			//	Y a-t-il un axe vertical avec des noms ?
			get
			{
				return this.options.Mode != GraphMode.Pie;
			}
		}

		private void PaintHorizontalLabels(Graphics graphics, Rectangle labelsRect, int nx, int dx)
		{
			//	Dessine l'axe horizontal.
			for (int x = 0; x < nx; x++)
			{
				var rect = new Rectangle (labelsRect.Left+dx*x, labelsRect.Bottom, dx, labelsRect.Height);
				this.PaintText (graphics, rect, this.cube.GetShortTitle (this.options.PrimaryDimension, x), ContentAlignment.MiddleCenter);
			}
		}

		private void PaintVerticalLabels(Graphics graphics, Rectangle labelsRect, int ny, int dy)
		{
			//	Dessine l'axe vertical.
			if (this.options.Mode == GraphMode.Array)
			{
				this.PaintVerticalLabelsTitles (graphics, labelsRect, ny, dy);
			}
			else
			{
				this.PaintVerticalLabelsUnits (graphics, labelsRect, ny, dy);
			}
		}

		private void PaintVerticalLabelsTitles(Graphics graphics, Rectangle labelsRect, int ny, int dy)
		{
			//	Dessine les légendes de l'axe vertical.
			for (int y = 0; y < ny; y++)
			{
				var rect = new Rectangle (labelsRect.Left, labelsRect.Bottom+dy*y, labelsRect.Width-5, dy);
				this.PaintText (graphics, rect, this.cube.GetShortTitle (this.options.SecondaryDimension, y), ContentAlignment.MiddleRight);
			}
		}

		private void PaintVerticalLabelsUnits(Graphics graphics, Rectangle labelsRect, int ny, int dy)
		{
			//	Dessine les unités de l'axe vertical.
			decimal step = 1;
			decimal bigStep = 1;
			var y1 = this.ConvValueToY (0);

			decimal[] steps =
			{	
				1, 2, 5,
				10, 20, 50,
				100, 200, 500,
				1000, 2000, 5000,
				10000, 20000, 50000,
				100000, 200000, 500000,
				1000000, 2000000, 5000000,
				10000000, 20000000, 50000000,
				100000000, 200000000, 500000000,
				1000000000,
			};

			//	On cherche le plus petit step suffisamment espacé.
			for (int i = 0; i < steps.Length-1; i++)
			{
				step    = steps[i];
				bigStep = steps[i+1];

				var y2 = this.ConvValueToY (step);

				if (y2-y1 > this.fontSize*3)  // assez espacés ?
				{
					break;
				}
			}

			//	Dessine les unité positives, y compris le zéro.
			decimal value = 0;
			while (true)
			{
				var y = this.ConvValueToY (value);

				if (y > labelsRect.Top)
				{
					break;
				}

				if (y >= labelsRect.Bottom)
				{
					this.PaintUnit (graphics, labelsRect, y, value, bigStep);
				}

				value += step;
			}

			//	Dessine les unité négatives, sans le zéro.
			value = -step;
			while (true)
			{
				var y = this.ConvValueToY (value);

				if (y < labelsRect.Bottom)
				{
					break;
				}

				if (y <= labelsRect.Top)
				{
					this.PaintUnit (graphics, labelsRect, y, value, bigStep);
				}

				value -= step;
			}
		}

		private void PaintUnit(Graphics graphics, Rectangle labelsRect, double y, decimal value, decimal bigStep)
		{
			//	Dessine une unité composée d'un montant et d'un trait horizontal.
			y = System.Math.Floor (y) + 0.5;
			int h = (int) (this.fontSize/0.6);

			var rect = new Rectangle (labelsRect.Left, y-h/2+1, labelsRect.Width-10, h);
			var unit = Converters.MontantToString (value, 0);
			this.PaintText (graphics, rect, unit, ContentAlignment.MiddleRight);

			var div = System.Math.Abs (value / bigStep);
			bool bold = (div == System.Math.Truncate (div));

			graphics.AddLine (labelsRect.Right-5, y, this.drawRight, y);
			graphics.RenderSolid (bold ? this.BorderColor : Color.FromAlphaColor (0.2, this.BorderColor));
		}

		private void PaintText(Graphics graphics, Rectangle labelRect, FormattedText text, ContentAlignment alignment)
		{
			//	Dessine un texte quelconque dans un rectangle.
			var textLayout = new TextLayout
			{
				FormattedText   = text,
				LayoutSize      = labelRect.Size,
				Alignment       = alignment,
				DefaultFontSize = this.fontSize,
			};

			textLayout.Paint (labelRect.BottomLeft, graphics, labelRect, Color.FromName ("Black"), GlyphPaintStyle.Normal);
		}

		private int GetHorizontalLabelsHeight(Rectangle rect)
		{
			//	Retourne la hauteur pour les légendes de l'axe horizontal.
			return (int) (this.fontSize / 0.6);
		}

		private int GetVerticalLabelsWidth(Rectangle rect, int ny)
		{
			//	Retourne la largeur pour les légendes de l'axe vertical.
			if (this.options.Mode == GraphMode.Array)
			{
				var textLayout = new TextLayout
				{
					LayoutSize      = new Size (1000, 100),
					DefaultFontSize = this.fontSize,
				};

				int max = 0;

				for (int y = 0; y < ny; y++)
				{
					textLayout.FormattedText = this.cube.GetShortTitle (this.options.SecondaryDimension, y);
					double width = textLayout.GetSingleLineSize ().Width;
					max = System.Math.Max (max, (int) width);
				}

				return max+10;
			}
			else
			{
				return (int) (this.fontSize*7);  // place fixe ok jusqu'à 1'000'000 environ
			}
		}



		public void PaintRow(Graphics graphics, Rectangle rect, string text)
		{
			//	Dessine une cellule correspondant à une ligne, d'après le texte contenu dans StringList.
			//	Le texte est au format "$${_graphic_}$$;row".
			var words = text.Split (';');
			var row = int.Parse (words[1]);

			this.PaintRow (graphics, rect, row);
		}

		private void PaintRow(Graphics graphics, Rectangle rect, int row)
		{
			System.Diagnostics.Debug.Assert (this.cube.Dimensions == 2);

			int nx = this.cube.GetCount (0);
			int ny = this.cube.GetCount (1);

			decimal finalMin = 0;
			decimal finalMax = 0;

			if (this.options.Mode == GraphMode.Stacked)
			{
				for (int yy = 0; yy < ny; yy++)
				{
					decimal positivSum = 0;
					decimal negativSum = 0;

					for (int xx = 0; xx < nx; xx++)
					{
						var value = this.cube.GetValue (xx, yy).GetValueOrDefault ();

						if (value >= 0)
						{
							positivSum += value;
							finalMax = System.Math.Max (finalMax, positivSum);
						}
						else
						{
							negativSum += value;
							finalMin = System.Math.Min (finalMin, negativSum);
						}
					}
				}
			}
			else
			{
				this.cube.GetMinMax (null, null, out finalMin, out finalMax);
			}

			switch (this.options.Mode)
			{
				case GraphMode.Stacked:
					this.PaintStackedRow (graphics, rect, finalMin, finalMax, row);
					break;

				case GraphMode.SideBySide:
					this.PaintSideBySideRow (graphics, rect, finalMin, finalMax, row);
					break;
			}
		}


		private void PaintStackedRow(Graphics graphics, Rectangle rect, decimal min, decimal max, int row)
		{
			//	Dessine le graphique avec plusieurs barres cumulées.
			if (max-min == 0)
			{
				return;
			}

			var borderColor = this.BorderColor;

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromAlphaColor (0.5, borderColor));

			rect.Deflate (2);
			rect = graphics.Align (rect);
			rect.Deflate (1);

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromBrightness (1));
			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromAlphaColor (0.2, borderColor));

			decimal lastPositiv = 0;
			decimal lastNegativ = 0;

			int nx = this.cube.GetCount (0);
			for (int i = 0; i < nx; i++)
			{
				var value = this.cube.GetValue (i, row).GetValueOrDefault ();
				decimal v1, v2;

				if (value >= 0)
				{
					v1 = lastPositiv;
					v2 = lastPositiv + value;

					lastPositiv += value;
				}
				else
				{
					v1 = lastNegativ + value;
					v2 = lastNegativ;

					lastNegativ += value;
				}

				double x1 = System.Math.Floor (rect.Width * (double) (v1-min) / (double) (max-min));
				double x2 = System.Math.Floor (rect.Width * (double) (v2-min) / (double) (max-min));
				double dx = x2-x1;

				if (dx > 0)
				{
					var r = new Rectangle (rect.Left+x1, rect.Bottom, dx, rect.Height);
					graphics.AddFilledRectangle (r);
					graphics.RenderSolid (this.GetIndexedColor (i, nx));

					graphics.AddLine (rect.Left+x2-0.5, rect.Bottom+0.5, rect.Left+x2-0.5, rect.Top-0.5);
					graphics.RenderSolid (borderColor);
				}
			}

			double zero = System.Math.Floor (rect.Width * (double) (-min) / (double) (max-min));
			graphics.AddLine (rect.Left+zero-0.5, rect.Bottom+0.5, rect.Left+zero-0.5, rect.Top-0.5);
			graphics.RenderSolid (borderColor);

			rect.Inflate (0.5);

			graphics.AddRectangle (rect);
			graphics.RenderSolid (borderColor);
		}

		private void PaintSideBySideRow(Graphics graphics, Rectangle rect, decimal min, decimal max, int row)
		{
			//	Dessine le graphique avec plusieurs barres côte à côte.
			min = System.Math.Min (min, 0);
			max = System.Math.Max (max, 0);

			if (max-min == 0)
			{
				return;
			}

			var borderColor = this.BorderColor;

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromAlphaColor (0.5, borderColor));

			rect.Deflate (2);
			rect = graphics.Align (rect);
			rect.Deflate (1);

			int nx = this.cube.GetCount (0);

			int dy = (int) System.Math.Max (rect.Height/nx, 1);
			int h = dy*nx - 1;
			int o = (int) (rect.Height-h)/2;
			rect = new Rectangle (rect.Left, rect.Bottom+o, rect.Width, h);

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromBrightness (1));
			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromAlphaColor (0.2, borderColor));

			double zero = System.Math.Floor (rect.Width * (double) -min / (double) (max-min));
			double y = 0;

			for (int i = 0; i < nx; i++)
			{
				var value = this.cube.GetValue (i, row).GetValueOrDefault ();

				double x  = System.Math.Floor (rect.Width * (double) (value-min) / (double) (max-min));
				double x1 = System.Math.Min (x, zero);
				double x2 = System.Math.Max (x, zero);

				var r = new Rectangle (rect.Left+x1, rect.Top-y-dy, x2-x1, dy);
				graphics.AddFilledRectangle (r);
				graphics.RenderSolid (this.GetIndexedColor (i, nx));

				graphics.AddLine (rect.Left+x-0.5, rect.Top-y-dy+1.5, rect.Left+x-0.5, rect.Top-y-0.5);
				graphics.RenderSolid (borderColor);

				if (i < nx-1)
				{
					graphics.AddLine (rect.Left, rect.Top-y-dy+0.5, rect.Right, rect.Top-y-dy+0.5);
					graphics.RenderSolid (borderColor);
				}

				y += dy;
			}

			graphics.AddLine (rect.Left+zero-0.5, rect.Bottom, rect.Left+zero-0.5, rect.Top);
			graphics.RenderSolid (borderColor);

			rect.Inflate (0.5);

			graphics.AddRectangle (rect);
			graphics.RenderSolid (borderColor);
		}


		private Color BackExtColor
		{
			get
			{
				if (this.options.Style == GraphStyle.BlackAndWhite)
				{
					return Color.FromBrightness (1.00);
				}
				else if (this.options.Style == GraphStyle.LightRainbow)
				{
					return Color.FromBrightness (0.90);
				}
				else if (this.options.Style == GraphStyle.DarkRainbow)
				{
					return Color.FromBrightness (0.70);
				}
				else if (this.options.Style == GraphStyle.Red)
				{
					return Color.FromHexa ("e5d0d0");
				}
				else if (this.options.Style == GraphStyle.Green)
				{
					return Color.FromHexa ("d0e5d0");
				}
				else if (this.options.Style == GraphStyle.Blue)
				{
					return Color.FromHexa ("d0e2e5");
				}
				else
				{
					return Color.FromBrightness (0.80);
				}
			}
		}

		private Color BackIntColor
		{
			get
			{
				if (this.options.Style == GraphStyle.BlackAndWhite)
				{
					return Color.FromBrightness (1.00);
				}
				else if (this.options.Style == GraphStyle.LightRainbow)
				{
					return Color.FromBrightness (1.00);
				}
				else if (this.options.Style == GraphStyle.DarkRainbow)
				{
					return Color.FromBrightness (0.90);
				}
				else
				{
					return Color.FromBrightness (0.90);
				}
			}
		}

		private Color BackLegendsColor
		{
			get
			{
				if (this.options.Style == GraphStyle.BlackAndWhite)
				{
					return Color.FromBrightness (1.00);
				}
				else if (this.options.Style == GraphStyle.LightRainbow)
				{
					return Color.FromAlphaColor (0.8, Color.FromName ("White"));
				}
				else if (this.options.Style == GraphStyle.DarkRainbow)
				{
					return Color.FromAlphaColor (0.95, Color.FromName ("White"));
				}
				else
				{
					return Color.FromAlphaColor (0.9, Color.FromName ("White"));
					//?return Color.FromBrightness (0.98);
				}
			}
		}

		private Color BorderColor
		{
			get
			{
				if (this.options.Style == GraphStyle.Rainbow)
				{
					IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
					return adorner.ColorTextFieldBorder (true);
				}
				else if (this.options.Style == GraphStyle.BlackAndWhite)
				{
					return Color.FromBrightness (0.0);
				}
				else if (this.options.Style == GraphStyle.DarkRainbow)
				{
					return Color.FromBrightness (0.4);
				}
				else
				{
					return Color.FromBrightness (0.6);
				}
			}
		}

		private Color GetIndexedColor(int index, int total)
		{
			switch (this.options.Style)
			{
				//	Retourne une couleur de l'arc-en-ciel.
				case GraphStyle.Rainbow:
					{
						index *= System.Math.Max (GraphEngine.rainbow.Length/total, 1);
						return Color.FromHsv (GraphEngine.rainbow[index%GraphEngine.rainbow.Length], 1.0, 1.0);
					}

				case GraphStyle.LightRainbow:
					{
						index *= System.Math.Max (GraphEngine.rainbow.Length/total, 1);
						return Color.FromHsv (GraphEngine.rainbow[index%GraphEngine.rainbow.Length], 0.6, 1.0);
					}

				case GraphStyle.DarkRainbow:
					{
						index *= System.Math.Max (GraphEngine.rainbow.Length/total, 1);
						return Color.FromHsv (GraphEngine.rainbow[index%GraphEngine.rainbow.Length], 1.0, 0.8);
					}

				case GraphStyle.Grey:
					{
						index *= System.Math.Max (GraphEngine.grey.Length/total, 1);
						return Color.FromBrightness (GraphEngine.grey[index%GraphEngine.grey.Length]);
					}

				case GraphStyle.BlackAndWhite:
					{
						index *= System.Math.Max (GraphEngine.blackAndWhite.Length/total, 1);
						return Color.FromBrightness (GraphEngine.blackAndWhite[index%GraphEngine.blackAndWhite.Length]);
					}

				case GraphStyle.Red:
					{
						index *= System.Math.Max (GraphEngine.red.Length/total, 1);
						return Color.FromHexa (GraphEngine.red[index%GraphEngine.red.Length]);
					}

				case GraphStyle.Green:
					{
						index *= System.Math.Max (GraphEngine.green.Length/total, 1);
						return Color.FromHexa (GraphEngine.green[index%GraphEngine.green.Length]);
					}

				case GraphStyle.Blue:
					{
						index *= System.Math.Max (GraphEngine.blue.Length/total, 1);
						return Color.FromHexa (GraphEngine.blue[index%GraphEngine.blue.Length]);
					}

				case GraphStyle.Fire:
					{
						index *= System.Math.Max (GraphEngine.fire.Length/total, 1);
						return Color.FromHexa (GraphEngine.fire[index%GraphEngine.fire.Length]);
					}

				default:
					return Color.FromBrightness (0.5);
			}
		}

		//	La difficulté consiste a avoir un maximun de couleurs, tout en garantissant
		//	qu'elles sont visuellement identifiables les unes par rapport aux autres.
		private static int[] rainbow          = { 0, 40, 60, 90, 190, 210, 240, 270, 290 };
		private static double[] grey          = { 0.7, 0.5, 0.9, 0.4, 1.0 };
		private static double[] blackAndWhite = { 0.7, 1.0, 0.0 };
		private static string[] red           = { "ff0000", "ff8d8d", "a80000", "ffe1e1" };
		private static string[] green         = { "00ff00", "b7ffb7", "00c600", "007000" };
		private static string[] blue          = { "00ccff", "b1efff", "00a0c8", "0060ff", "7fafff", "003fa6" };
		private static string[] fire          = { "ff9e9e", "ff0000", "ad0000", "ffe9a6", "ffc000", "e3ab00", "ffffed", "ffff00", "e5e500" };



		private Cube			cube;
		private GraphOptions	options;
		private double			fontSize;
		private decimal			minValue;
		private decimal			maxValue;
		private double			drawBottom;
		private double			drawHeight;
		private double			drawRight;
	}
}
