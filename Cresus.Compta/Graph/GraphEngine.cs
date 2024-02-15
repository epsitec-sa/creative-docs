//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Graph
{
	public class GraphEngine
	{
		public GraphEngine()
		{
			this.surfacesList = new List<GraphSurface> ();
			this.surfacesDict = new Dictionary<GraphSurfaceId, GraphSurface> ();

			this.hilitedSurfaceId = GraphSurfaceId.Empty;
			this.selectedSurfaceId = GraphSurfaceId.Empty;
		}


		public GraphOptions Options
		{
			get
			{
				return this.options;
			}
		}

		public Rectangle FullRect
		{
			get
			{
				return this.fullRect;
			}
		}


		public GraphSurfaceId HilitedSurfaceId
		{
			//	Identificateur de la surface survolée par la souris.
			get
			{
				return this.hilitedSurfaceId;
			}
			set
			{
				this.hilitedSurfaceId = value;
			}
		}

		public GraphSurfaceId SelectedSurfaceId
		{
			//	Identificateur de la surface sélectionnée.
			get
			{
				return this.selectedSurfaceId;
			}
			set
			{
				this.selectedSurfaceId = value;
			}
		}


		public void SetHandle(int rank, Point pos)
		{
			//	Modifie la position d'une poignée. Ceci est enregistré dans les options GraphOptions.
			if (this.selectedSurfaceId != GraphSurfaceId.Empty)
			{
				var surface = this.GetSurface (this.selectedSurfaceId);
				if (surface != null)
				{
					surface.SetHandle (rank, pos);
				}
			}
		}

		public int DetectHandle(Point pos)
		{
			//	Retourne la poignée visée par la souris.
			if (this.selectedSurfaceId != GraphSurfaceId.Empty)
			{
				var surface = this.GetSurface (this.selectedSurfaceId);
				if (surface != null)
				{
					return surface.DetectHandle (pos);
				}
			}

			return -1;
		}

		public GraphSurfaceId DetectSurface(Point pos)
		{
			//	Retourne la surface visée par la souris.
			for (int i = this.surfacesList.Count-1; i >= 0; i--)  // du dernier (avant-plan) au premier (arrière-plan)
			{
				var surface = this.surfacesList[i];

				if (surface.Contains (pos))
				{
					return surface.Id;
				}
			}

			return GraphSurfaceId.Empty;
		}

		public string GetTooltip(GraphSurfaceId surface)
		{
			//	Retourne le texte du tooltip à utiliser pour une surface donnée.
			if (surface != null && surface.CubeX >= 0 && surface.CubeY >= 0)
			{
				var value = this.cube.GetValue (surface.CubeX, surface.CubeY);
				if (value.HasValue)
				{
					var xx = this.cube.GetTitle (0, surface.CubeX);
					var yy = this.cube.GetTitle (1, surface.CubeY);
					var vv = Converters.MontantToString (value.Value, null);

					return string.Format ("{0} ; {1} = {2}", xx, yy, vv);
				}
			}

			return null;  // pas de tooltip
		}


		public void PaintFull(Cube cube, GraphOptions options, Graphics graphics, Rectangle rect)
		{
			//	Dessine un graphique complet et construit les informations sur les surfaces.
			this.cube     = cube;
			this.options  = options;
			this.fullRect = rect;

			this.surfacesList.Clear ();
			this.surfacesDict.Clear ();

			if (this.cube.Dimensions == 0 || this.cube.IsEmpty)
			{
				return;
			}

			graphics.AddFilledRectangle (this.fullRect);
			graphics.RenderSolid (this.BackExtColor);

			var drawingRect = this.DrawingRect;
			this.AddSurface (GraphSurfaceType.Margins, drawingRect);

			this.fontSize = this.options.FontSize;

			int nx = this.cube.GetCount (0);
			int ny = this.cube.GetCount (1);

			int labelDx, labelDy;

			if (this.HasVerticalLabels)
			{
				labelDx = this.GetVerticalLabelsWidth (drawingRect, ny);
			}
			else
			{
				labelDx = 0;
			}

			if (this.HasHorizontalLabels)
			{
				labelDy = this.GetHorizontalLabelsHeight (drawingRect);
			}
			else
			{
				labelDy = 0;
			}

			int dx = ((int) drawingRect.Width  - labelDx) / nx;
			int dy = ((int) drawingRect.Height - labelDy) / ny;

			if (dx <= 5 || dy <= 5)
			{
				this.PaintError (graphics, drawingRect);
				return;
			}

			//	Dessine la grille.
			var frameRect = new Rectangle (drawingRect.Left+labelDx, drawingRect.Bottom+labelDy, drawingRect.Width-labelDx, drawingRect.Height-labelDy);
			this.PaintGrid (graphics, frameRect, nx, dx, ny, dy);

			//	Initialisation des échelles.
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

			//	Dessine les légendes des axes.
			if (this.HasHorizontalLabels)
			{
				var labelsRect = new Rectangle (drawingRect.Left+labelDx, drawingRect.Bottom, drawingRect.Width-labelDx, labelDy);
				this.PaintHorizontalLabels (graphics, labelsRect, nx, dx);
			}

			if (this.HasVerticalLabels)
			{
				var labelsRect = new Rectangle (drawingRect.Left, drawingRect.Bottom+labelDy, labelDx, drawingRect.Height-labelDy);
				this.PaintVerticalLabels (graphics, labelsRect, ny, dy);
			}

			//	Dessine le graphique.
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

				case GraphMode.Pie:
					this.PaintPie (graphics, frameRect, nx, ny);
					break;

				case GraphMode.Array:
					this.PaintArray (graphics, frameRect, nx, dx, ny, dy);
					break;
			}

			//	Dessine le titre.
			this.PaintTitle (graphics);

			//	Dessine la boîte des légendes.
			if (this.options.Mode != GraphMode.Array && this.options.HasLegend)
			{
				this.PaintLegends (graphics, this.LegendsRect);
			}

			//	Dessine les surfaces mises en évidence.
			var surface = this.GetSurface (this.hilitedSurfaceId);
			if (surface != null)
			{
				surface.PaintHilited (graphics);
			}

			surface = this.GetSurface (this.selectedSurfaceId);
			if (surface != null)
			{
				surface.PaintSelected (graphics);
			}
		}

		public Rectangle DrawingRect
		{
			get
			{
				var x1 = System.Math.Floor (this.fullRect.Left   + this.options.MarginsAbs.Left);
				var x2 = System.Math.Floor (this.fullRect.Right  - this.options.MarginsAbs.Right);
				var y1 = System.Math.Floor (this.fullRect.Bottom + this.options.MarginsAbs.Bottom);
				var y2 = System.Math.Floor (this.fullRect.Top    - this.options.MarginsAbs.Top);

				return new Rectangle (x1, y1, x2-x1, y2-y1);
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

			if (this.options.StartAtZero)
			{
				this.minValue = System.Math.Min (this.minValue, 0);
				this.maxValue = System.Math.Max (this.maxValue, 0);
			}

			this.drawBottom = frameRect.Bottom+0.5;
			this.drawHeight = frameRect.Height;
		}

		private void PaintSideBySide(Graphics graphics, Rectangle frameRect, int nx, int dx, int ny)
		{
			for (int x = 0; x < nx; x++)
			{
				var rect = new Rectangle (frameRect.Left+x*dx+0.5, frameRect.Bottom+0.5, dx, frameRect.Height);
				int margin = System.Math.Min ((int) (rect.Width*0.5*(1-this.options.BarThickness)), (int) (rect.Width*0.5-1));
				rect.Deflate (margin, 0);

				double zero = this.ConvValueToY (0);

				int overlap = (int) (rect.Width / ny * this.options.BarOverlap);
				int barShift = (int) ((rect.Width-overlap) / ny);
				int barWidth = barShift + overlap;

				for (int y = 0; y < ny; y++)
				{
					decimal? value = this.cube.GetValue (x, y);

					if (value.HasValue)
					{
						double h = this.ConvValueToY (value.Value);
						double h1 = System.Math.Max (System.Math.Min (h, zero), this.drawBottom);
						double h2 = System.Math.Max (System.Math.Max (h, zero), this.drawBottom);
						var barRect = new Rectangle (rect.Left+barShift*y, h1, barWidth, h2-h1);

						graphics.AddFilledRectangle (barRect);
						graphics.RenderSolid (this.GetIndexedColor (y, ny));

						graphics.LineWidth = this.options.BorderThickness;
						graphics.AddRectangle (barRect);
						graphics.RenderSolid (this.BorderColor);
						graphics.LineWidth = 1;

						this.AddSurface (GraphSurfaceType.Graph, x, y, barRect);
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
					var value = this.cube.GetValue (x, y).GetValueOrDefault ();

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
				int margin = System.Math.Min ((int) (rect.Width*0.5*(1-this.options.BarThickness)), (int) (rect.Width*0.5-1));
				rect.Deflate (margin, 0);

				for (int pass = 0; pass < 2; pass++)
				{
					decimal lastPositiv = 0;
					decimal lastNegativ = 0;

					for (int y = 0; y < ny; y++)
					{
						var value = this.cube.GetValue (x, y).GetValueOrDefault ();
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

							this.AddSurface (GraphSurfaceType.Graph, x, y, barRect);
						}
						else
						{
							graphics.LineWidth = this.options.BorderThickness;
							graphics.AddRectangle (barRect);
							graphics.RenderSolid (this.BorderColor);
							graphics.LineWidth = 1;
						}
					}
				}
			}
		}


		private void InitLines(Graphics graphics, Rectangle frameRect, int nx, int dx, int ny)
		{
			this.cube.GetMinMax (null, null, out this.minValue, out this.maxValue);

			if (this.options.StartAtZero)
			{
				this.minValue = System.Math.Min (this.minValue, 0);
				this.maxValue = System.Math.Max (this.maxValue, 0);
			}

			var margin = LinesVerticalMargin;
			this.drawBottom = frameRect.Bottom+margin;
			this.drawHeight = frameRect.Height-margin*2;
		}
		
		private void PaintLines(Graphics graphics, Rectangle frameRect, int nx, int dx, int ny)
		{
			var rect = frameRect;
			rect.Deflate (0, this.LinesVerticalMargin);

			for (int y = 0; y < ny; y++)
			{
				for (int pass = 0; pass < 2; pass++)
				{
					var last = Point.Zero;

					for (int x = 0; x < nx; x++)
					{
						decimal? value = this.cube.GetValue (x, y);

						if (value.HasValue)
						{
							double h = this.ConvValueToY (value.Value);
							var pos = new Point (rect.Left+dx*x+dx*0.5, h);
							var color = Color.FromAlphaColor (this.options.LineAlpha, this.GetIndexedColor (y, ny));

							if (pass == 0)
							{
								if (!last.IsZero && this.options.HasLines)
								{
									graphics.LineCap = CapStyle.Round;
									graphics.LineWidth = this.options.LineWidth+this.options.BorderThickness*2;
									graphics.AddLine (last, pos);
									graphics.RenderSolid (Color.FromAlphaColor (this.options.LineAlpha, this.BorderColor));

									graphics.LineWidth = this.options.LineWidth;
									graphics.AddLine (last, pos);
									graphics.RenderSolid (color);

									graphics.LineCap = CapStyle.Square;
									graphics.LineWidth = 1;
								}

								var d = System.Math.Max (this.options.PointWidth, 6) + 2;
								var pointRect = new Rectangle (pos.X-d/2, pos.Y-d/2, d, d);
								this.AddSurface (GraphSurfaceType.Graph, x, y, pointRect);
							}
							else
							{
								if (this.options.GraphPoints != GraphPoint.None)
								{
									this.PaintPoint (graphics, pos, this.GetGraphPoint (y, ny), color);
								}
							}

							last = pos;
						}
					}
				}
			}
		}

		private double LinesVerticalMargin
		{
			get
			{
				double margin = 0;

				if (this.options.HasLines)
				{
					margin = System.Math.Max (margin, this.options.LineWidth);
				}

				if (this.options.GraphPoints != GraphPoint.None)
				{
					margin = System.Math.Max (margin, this.options.PointWidth);
				}

				return System.Math.Floor (margin*0.5);
			}
		}

		private void PaintPoint(Graphics graphics, Point pos, GraphPoint type, Color color)
		{
			pos = graphics.Align (pos);

			double r1 = System.Math.Floor ((this.options.PointWidth+this.options.BorderThickness+1)*0.5);
			double r2 = System.Math.Floor (r1*0.8);
			double r3 = System.Math.Floor (r1*0.5);

			var path = this.GetPointPath (pos, type, r1, r2, r3);
			graphics.Color = Color.FromAlphaColor (this.options.LineAlpha, this.BorderColor);
			graphics.PaintSurface (path);

			r1 -= this.options.BorderThickness;
			r2 -= this.options.BorderThickness;
			r3 -= this.options.BorderThickness;

			path = this.GetPointPath (pos, type, r1, r2, r3);
			graphics.Color = this.options.HasLines ? Color.FromName ("White") : color;
			graphics.PaintSurface (path);
		}

		private Path GetPointPath(Point pos, GraphPoint type, double r1, double r2, double r3)
		{
			var rect = new Rectangle (pos.X-r1, pos.Y-r1, r1*2, r1*2);
			var path = new Path ();

			if (type == GraphPoint.Square)
			{
				path.AppendRectangle (new Rectangle (pos.X-r2, pos.Y-r2, r2*2, r2*2));
			}
			else if (type == GraphPoint.TriangleUp)
			{
				path.MoveTo (rect.Center.X, rect.Top);
				path.LineTo (rect.BottomLeft);
				path.LineTo (rect.BottomRight);
				path.Close ();
			}
			else if (type == GraphPoint.TriangleDown)
			{
				path.MoveTo (rect.Center.X, rect.Bottom);
				path.LineTo (rect.TopLeft);
				path.LineTo (rect.TopRight);
				path.Close ();
			}
			else if (type == GraphPoint.Diamond)
			{
				path.MoveTo (rect.Center.X, rect.Top);
				path.LineTo (rect.Right, rect.Center.Y);
				path.LineTo (rect.Center.X, rect.Bottom);
				path.LineTo (rect.Left, rect.Center.Y);
				path.Close ();
			}
			else if (type == GraphPoint.Cross)
			{
				path.MoveTo (pos.X-r3, pos.Y+r1);
				path.LineTo (pos.X+r3, pos.Y+r1);
				path.LineTo (pos.X+r3, pos.Y+r3);
				path.LineTo (pos.X+r1, pos.Y+r3);
				path.LineTo (pos.X+r1, pos.Y-r3);
				path.LineTo (pos.X+r3, pos.Y-r3);
				path.LineTo (pos.X+r3, pos.Y-r1);
				path.LineTo (pos.X-r3, pos.Y-r1);
				path.LineTo (pos.X-r3, pos.Y-r3);
				path.LineTo (pos.X-r1, pos.Y-r3);
				path.LineTo (pos.X-r1, pos.Y+r3);
				path.LineTo (pos.X-r3, pos.Y+r3);
				path.Close ();
			}
			else  // cercle ?
			{
				path.AppendCircle (pos, r1);
			}

			return path;
		}


		private void PaintPie(Graphics graphics, Rectangle frameRect, int nx, int ny)
		{
			int max = int.MinValue;
			int tx = 0;
			int ty = 0;
			int titleHeight = (int) (this.fontSize*1.6);

			//	On cherche la disposition tx/ty générant une surface maximale pour chaque cammembert.
			for (int x = 1; x <= nx; x++)
			{
				for (int y = 1; y <= nx; y++)
				{
					if (x*y >= nx)
					{
						int size = this.GetPieSize (frameRect, x, y, titleHeight);
						if (max < size)
						{
							max = size;
							tx = x;
							ty = y;
						}
					}
				}
			}

			if (tx == 0 || ty == 0)
			{
				return;
			}

			//?int ox = (int) (frameRect.Width - tx*max) / 2;
			int ox = 0;  // à gauche pour laisser la place pour la légende
			int oy = (int) (frameRect.Height - ty*titleHeight - ty*max) / 2;

			for (int x = 0; x < tx; x++)
			{
				for (int y = 0; y < ty; y++)
				{
					int i = x + y*tx;
					if (i < nx)
					{
						var square = new Rectangle (frameRect.Left+ox+x*max, frameRect.Top-oy-y*(max+titleHeight)-max, max, max);
						this.PaintOnePie (graphics, square, i, ny, titleHeight);
					}
				}
			}
		}

		private int GetPieSize(Rectangle rect, int tx, int ty, int titleHeight)
		{
			int dx = (int) (rect.Width / tx);
			int dy = (int) ((rect.Height - titleHeight*ty) / ty);
			return System.Math.Min (dx, dy);
		}

		private void PaintOnePie(Graphics graphics, Rectangle square, int i, int ny, int titleHeight)
		{
			square.Offset (0, -titleHeight);

			double offset = this.options.ExplodedPieFactor * square.Width * 0.03;

			square.Deflate (5+offset);
			var center = square.Center;
			var radius = square.Width/2;

			decimal sum = 0;
			for (int y = 0; y < ny; y++)
			{
				sum += System.Math.Abs (this.cube.GetValue (i, y).GetValueOrDefault ());
			}

			if (sum == 0)
			{
				graphics.LineWidth = this.options.BorderThickness;
				graphics.AddCircle (center, radius);
				graphics.RenderSolid (this.BorderColor);
				graphics.LineWidth = 1;
			}
			else
			{
				for (int pass = 0; pass < 2; pass++)
				{
					decimal a1 = 0;

					for (int y = 0; y < ny; y++)
					{
						decimal value = System.Math.Abs (this.cube.GetValue (i, y).GetValueOrDefault ());
						decimal a2 = a1 + value * 360.0m / sum;

						if (a2-a1 == 360)  // une seule tranche ?
						{
							if (pass == 0)
							{
								Path path = new Path ();
								path.AppendCircle (center, radius);

								graphics.Color = this.GetIndexedColor (y, ny);
								graphics.PaintSurface (path);

								this.AddSurface (GraphSurfaceType.Graph, i, y, path);
							}
							else
							{
								graphics.LineWidth = this.options.BorderThickness;
								graphics.AddCircle (center, radius);
								graphics.RenderSolid (this.BorderColor);
								graphics.LineWidth = 1;

								this.PaintPiePercent (graphics, center, (a2-a1)/360.0m, value, this.GetIndexedColor (y, ny));
							}
						}
						else if (a2 > a1)
						{
							var c = center;

							if (offset != 0)
							{
								c = Transform.RotatePointDeg (center, (double) this.GetPieAngle ((a1+a2)/2), new Point (center.X+offset, center.Y));
							}

							Path path = new Path ();

							path.MoveTo (c);
							path.ArcToDeg (c, radius, radius, (double) this.GetPieAngle(a1), (double) this.GetPieAngle(a2), false);
							path.LineTo (c);
							path.Close ();

							if (pass == 0)
							{
								graphics.Color = this.GetIndexedColor (y, ny);
								graphics.PaintSurface (path);

								this.AddSurface (GraphSurfaceType.Graph, i, y, path);
							}
							else
							{
								graphics.LineJoin = JoinStyle.Round;
								graphics.LineWidth = this.options.BorderThickness;
								graphics.Color = this.BorderColor;
								graphics.PaintOutline (path);
								graphics.LineWidth = 1;
								graphics.LineJoin = JoinStyle.Miter;

								if (a2-a1 >= 20)  // plus de 20 degrés ?
								{
									var p = Transform.RotatePointDeg (center, (double) this.GetPieAngle((a1+a2)/2), new Point (center.X+offset+radius*0.5, center.Y));
									this.PaintPiePercent (graphics, p, (a2-a1)/360.0m, value, this.GetIndexedColor (y, ny));
								}
							}
						}

						a1 = a2;
					}
				}
			}

			//	Dessine le nom du cammembert.
			square.Inflate (offset);

			var box = new Rectangle (square.Left, square.Top, square.Width, titleHeight);
			var title = this.cube.GetTitle (0, i);
			this.PaintText (graphics, box, title, ContentAlignment.TopCenter);
		}

		private void PaintPiePercent(Graphics graphics, Point center, decimal percent, decimal value, Color backColor)
		{
			//	Dessine le chiffre du %.
			FormattedText text = FormattedText.Null;

			if (this.options.PiePercents)
			{
				text = Converters.PercentToString (percent);
			}

			if (this.options.PieValues)
			{
				text = Converters.MontantToString (value, 0);
			}

			if (!text.IsNullOrEmpty ())
			{
				var rect = new Rectangle (center.X-200, center.Y-100, 400, 200);
				var color = backColor.GetBrightness () < 0.25 ? Color.FromName ("White") : Color.FromName ("Black");

				this.PaintText (graphics, rect, text, ContentAlignment.MiddleCenter, color);
			}
		}

		private decimal GetPieAngle(decimal angle)
		{
			//	In:  0 -> à droite, ccw
			//	Out: 0 -> en haut, cw
			angle = 90-angle;

			if (angle < 0)
			{
				angle += 360;
			}

			return angle;
		}


		private void PaintArray(Graphics graphics, Rectangle frameRect, int nx, int dx, int ny, int dy)
		{
			for (int x = 0; x < nx; x++)
			{
				for (int y = 0; y < ny; y++)
				{
					decimal value = this.cube.GetValue (x, y).GetValueOrDefault ();

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
			if (this.minValue == decimal.MaxValue ||
				this.maxValue == decimal.MinValue ||
				this.minValue == this.maxValue)
			{
				return this.drawBottom;
			}
			else
			{
				return this.drawBottom + System.Math.Floor (this.drawHeight * (double) (value-this.minValue) / (double) (this.maxValue-this.minValue));
			}
		}


		#region Title
		public Rectangle TitleRect
		{
			get
			{
				var w = this.options.TitleSizeAbs.Width;
				var h = this.options.TitleSizeAbs.Height;
				var x = System.Math.Floor (System.Math.Max ((this.fullRect.Width-w)*this.options.TitlePositionRel.X, 0));
				var y = System.Math.Floor (System.Math.Max ((this.fullRect.Height-h)*this.options.TitlePositionRel.Y, 0));

				return new Rectangle (x, y, w, h);
			}
			set
			{
				var rect = Rectangles.MoveInside (value, this.fullRect);
				this.options.TitleSizeAbs = rect.Size;

				var x = rect.Left   / (this.fullRect.Width -rect.Size.Width);
				var y = rect.Bottom / (this.fullRect.Height-rect.Size.Height);
				this.options.TitlePositionRel = new Point (x, y);
			}
		}

		private void PaintTitle(Graphics graphics)
		{
			var rect = this.TitleRect;
			this.AddSurface (GraphSurfaceType.Title, rect);
			this.PaintText (graphics, rect, this.options.TitleText, ContentAlignment.MiddleCenter, TextBreakMode.Hyphenate, 1.5);
		}
		#endregion


		#region Legends
		public Rectangle LegendsRect
		{
			//	Retourne le rectangle occupé par la légende.
			get
			{
				var size = this.LegendsSize;

				var x = System.Math.Floor (System.Math.Max ((this.fullRect.Width-size.Width)*this.options.LegendPositionRel.X, 0));
				var y = System.Math.Floor (System.Math.Max ((this.fullRect.Height-size.Height)*this.options.LegendPositionRel.Y, 0));

				return new Rectangle (x, y, size.Width, size.Height);
			}
		}

		private Size LegendsSize
		{
			//	Retourne la taille nécessaire pour la légende.
			get
			{
				int c = this.options.LegendColumns;
				int n = (this.cube.GetCount (1)+c-1)/c;
				int h = (int) (this.fontSize / 0.7);
				int w = this.LegendsColumnWidth;

				return new Size ((w+h)*c, n*h);
			}
		}

		private int LegendsColumnWidth
		{
			//	Retourne la largeur pour une colonne.
			get
			{
				var textLayout = new TextLayout
				{
					LayoutSize      = new Size (1000, 100),
					DefaultFontSize = this.fontSize,
				};

				int max = 0;
				int n = this.cube.GetCount (1);

				for (int i = 0; i < n; i++)
				{
					textLayout.FormattedText = this.cube.GetTitle (1, i);
					double width = textLayout.GetSingleLineSize ().Width;
					max = System.Math.Max (max, (int) width);
				}

				return max + 10;
			}
		}

		private void PaintLegends(Graphics graphics, Rectangle rect)
		{
			//	Dessine la légende complète.
			this.AddSurface (GraphSurfaceType.Legend, rect);

			int total = this.cube.GetCount (1);
			int nc = this.options.LegendColumns;
			int n = (total+nc-1)/nc;
			int dy = (int) (this.fontSize / 0.7);
			int columnWidth = (int) (rect.Width / nc);

			rect.Inflate (2);
			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (this.BackLegendsColor);
			rect.Deflate (2);

			int index = 0;
			for (int c = 0; c < nc; c++)
			{
				for (int y = 0; y < n; y++)
				{
					if (index >= total)
					{
						break;
					}

					var lineRect = new Rectangle (rect.Left+columnWidth*c, rect.Top-(y+1)*dy, columnWidth, dy);
					var text = this.cube.GetTitle (1, index);

					this.PaintLegend (graphics, lineRect, this.GetIndexedColor (index, total), text);

					index++;
				}
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

			if (this.options.BorderThickness > 0)
			{
				sampleRect.Deflate (0.5);
				graphics.AddRectangle (sampleRect);
				graphics.RenderSolid (this.BorderColor);
			}

			var textRect = new Rectangle (rect.Left+rect.Height+5, rect.Bottom, rect.Width-rect.Height-5, rect.Height);
			this.PaintText (graphics, textRect, text, ContentAlignment.MiddleLeft);
		}
		#endregion


		#region Grid
		private void PaintGrid(Graphics graphics, Rectangle frameRect, int nx, int dx, int ny, int dy)
		{
			//	Dessine la grille de fond.
			if (this.options.Mode == GraphMode.Pie)
			{
				return;
			}

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
		#endregion


		#region Axes
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
				this.PaintText (graphics, rect, this.cube.GetShortTitle (0, x), ContentAlignment.MiddleCenter);
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
				this.PaintVerticalLabelsUnits (graphics, labelsRect, ny);
			}
		}

		private void PaintVerticalLabelsTitles(Graphics graphics, Rectangle labelsRect, int ny, int dy)
		{
			//	Dessine les légendes de l'axe vertical.
			for (int y = 0; y < ny; y++)
			{
				var rect = new Rectangle (labelsRect.Left, labelsRect.Bottom+dy*y, labelsRect.Width-5, dy);
				this.PaintText (graphics, rect, this.cube.GetShortTitle (1, y), ContentAlignment.MiddleRight);
			}
		}

		private void PaintVerticalLabelsUnits(Graphics graphics, Rectangle labelsRect, int ny)
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
			bool found = false;
			for (int i = 0; i < steps.Length-1; i++)
			{
				step    = steps[i];
				bigStep = steps[i+1];

				var y2 = this.ConvValueToY (step);

				if (y2-y1 > this.fontSize*3)  // assez espacés ?
				{
					found = true;
					break;
				}
			}

			if (!found)
			{
				return;
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

		private void PaintText(Graphics graphics, Rectangle labelRect, FormattedText text, ContentAlignment alignment, TextBreakMode breakMode = TextBreakMode.Ellipsis|TextBreakMode.Split|TextBreakMode.SingleLine, double fontFactor = 1)
		{
			this.PaintText (graphics, labelRect, text, alignment, Color.FromName ("Black"), breakMode, fontFactor);
		}

		private void PaintText(Graphics graphics, Rectangle labelRect, FormattedText text, ContentAlignment alignment, Color color, TextBreakMode breakMode = TextBreakMode.Ellipsis|TextBreakMode.Split|TextBreakMode.SingleLine, double fontFactor = 1)
		{
			//	Dessine un texte quelconque dans un rectangle.
			var textLayout = new TextLayout
			{
				FormattedText   = text,
				LayoutSize      = labelRect.Size,
				Alignment       = alignment,
				BreakMode       = breakMode,
				DefaultFontSize = this.fontSize * fontFactor,
			};

			textLayout.Paint (labelRect.BottomLeft, graphics, labelRect, color, GlyphPaintStyle.Normal);
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
					textLayout.FormattedText = this.cube.GetShortTitle (1, y);
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
		#endregion


		#region Surfaces
		private void AddSurface(GraphSurfaceType type, Rectangle rect)
		{
			var id = new GraphSurfaceId (type);
			var surface = new GraphSurface (this, id, rect);

			this.AddSurface (id, surface);
		}

		private void AddSurface(GraphSurfaceType type, int cubeX, int cubeY, Rectangle rect)
		{
			var id = new GraphSurfaceId (type, cubeX, cubeY);
			var surface = new GraphSurface (this, id, rect);

			this.AddSurface (id, surface);
		}

		private void AddSurface(GraphSurfaceType type, int cubeX, int cubeY, Path path)
		{
			var id = new GraphSurfaceId (type, cubeX, cubeY);
			var surface = new GraphSurface (this, id, path);

			this.AddSurface (id, surface);
		}

		private void AddSurface(GraphSurfaceId id, GraphSurface surface)
		{
			//	La liste permet de parcourir les surfaces dans l'ordre inverse du dessin, pour la détection.
			//	Le dictionnaire permet de retrouver rapidement une surface d'après son id.
			this.surfacesList.Add (surface);
			this.surfacesDict.Add (id, surface);
		}

		private GraphSurface GetSurface(GraphSurfaceId id)
		{
			//	Retrouve rapidement une surface d'après son identificateur.
			if (!id.IsEmpty)
			{
				GraphSurface surface;
				if (this.surfacesDict.TryGetValue (id, out surface))
				{
					return surface;
				}
			}

			return null;
		}
		#endregion


		#region Direct array painting
		public void PaintRow(Cube cube, GraphOptions options, Graphics graphics, Rectangle rect, string text)
		{
			//	Dessine une cellule correspondant à une ligne, d'après le texte contenu dans StringList.
			//	Le texte est au format "$${_graphic_}$$;row".
			this.cube = cube;
			this.options = options;

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
		#endregion


		#region Resources
		private GraphPoint GetGraphPoint(int index, int total)
		{
			if (this.options.GraphPoints == GraphPoint.Mix)
			{
				//?index *= System.Math.Max (GraphEngine.graphPoints.Length/total, 1);
				return GraphEngine.graphPoints[index%GraphEngine.graphPoints.Length];
			}
			else
			{
				return this.options.GraphPoints;
			}
		}

		private static GraphPoint[] graphPoints = { GraphPoint.Circle, GraphPoint.Square, GraphPoint.TriangleUp, GraphPoint.Diamond, GraphPoint.TriangleDown, GraphPoint.Cross };

		private Color BackExtColor
		{
			//	Couleur du cadre extérieur contenant les légendes des axes.
			get
			{
				if (this.options.Mode == GraphMode.Pie)
				{
					return this.BackIntColor;
				}

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
			//	Couleur du fond du graphe.
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
			//	Couleur du fond de la légende.
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
			//	Couleur des bordures, axes, traits de séparation, etc.
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
			//	Couleur d'un élément numéroté du graphe.
			switch (this.options.Style)
			{
				case GraphStyle.Rainbow:
					{
						//	Retourne une couleur de l'arc-en-ciel.
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
		#endregion


		private readonly List<GraphSurface>							surfacesList;
		private readonly Dictionary<GraphSurfaceId, GraphSurface>	surfacesDict;

		private Cube												cube;
		private GraphOptions										options;
		private Rectangle											fullRect;
		private double												fontSize;
		private decimal												minValue;
		private decimal												maxValue;
		private double												drawBottom;
		private double												drawHeight;
		private double												drawRight;
		private GraphSurfaceId										hilitedSurfaceId;
		private GraphSurfaceId										selectedSurfaceId;
	}
}
