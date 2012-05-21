//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta;

using System.Collections.Generic;

namespace Epsitec.Cresus.Compta.Graph
{
	/// <summary>
	/// Cette classe représente une surface quelconque correspondant à un élément d'un graphique,
	/// tel qu'une barre, un secteur, la boîte des légendes, etc.
	/// </summary>
	public class GraphSurface
	{
		public GraphSurface(GraphEngine engine, GraphSurfaceId id, Rectangle rect)
		{
			//	Crée une surface rectangulaire.
			this.engine = engine;
			this.id     = id;
			this.Rect   = rect;
		}

		public GraphSurface(GraphEngine engine, GraphSurfaceId id, Path path)
		{
			//	Crée une surface quelconque, selon un chemin.
			this.engine = engine;
			this.id     = id;
			this.Path   = path;
		}


		public GraphSurfaceId Id
		{
			get
			{
				return this.id;
			}
		}

		public Rectangle Rect
		{
			get;
			private set;
		}

		public Path Path
		{
			get;
			private set;
		}


		public int HandleCount
		{
			get
			{
				switch (this.id.Type)
				{
					case GraphSurfaceType.Legend:
						return 1;

					case GraphSurfaceType.Margins:
					case GraphSurfaceType.Title:
						return 8;

					default:
						return 0;
				}
			}
		}

		public Point? GetHandle(int rank)
		{
			switch (this.id.Type)
			{
				case GraphSurfaceType.Legend:
					return this.GetHandleLegend (rank);

				case GraphSurfaceType.Margins:
					return this.GetHandleMargins (rank);

				case GraphSurfaceType.Title:
					return this.GetHandleTitle (rank);

				default:
					return null;
			}
		}

		public void SetHandle(int rank, Point pos)
		{
			switch (this.id.Type)
			{
				case GraphSurfaceType.Legend:
					this.SetHandleLegend (rank, pos);
					break;

				case GraphSurfaceType.Margins:
					this.SetHandleMargins (rank, pos);
					break;

				case GraphSurfaceType.Title:
					this.SetHandleTitle (rank, pos);
					break;
			}
		}


		private Point? GetHandleLegend(int rank)
		{
			if (rank == 0)
			{
				var rect = this.engine.LegendsRect;
				return new Point (rect.Right, rect.Center.Y);
			}

			return null;
		}

		private void SetHandleLegend(int rank, Point pos)
		{
			if (rank == 0)
			{
				var rect = this.engine.LegendsRect;
				var columnWidth = rect.Width / this.engine.Options.LegendColumns;
				int n = System.Math.Max ((int) ((pos.X-rect.Left+columnWidth/2) / columnWidth), 1);

				if (this.engine.Options.LegendColumns != n)
				{
					this.engine.Options.LegendColumns = n;
				}
			}
		}


		private Point? GetHandleMargins(int rank)
		{
			var rect = this.engine.DrawingRect;
			var center = rect.Center;

			switch (rank)  // 0 = top right, puis cw
			{
				case 0:
					return rect.TopRight;

				case 1:
					return new Point (rect.Right, center.Y);

				case 2:
					return rect.BottomRight;

				case 3:
					return new Point (center.X, rect.Bottom);

				case 4:
					return rect.BottomLeft;

				case 5:
					return new Point (rect.Left, center.Y);

				case 6:
					return rect.TopLeft;

				case 7:
					return new Point (center.X, rect.Top);
			}

			return null;
		}

		private void SetHandleMargins(int rank, Point pos)
		{
			var rect = this.engine.FullRect;

			var left   = this.engine.Options.MarginsAbs.Left;
			var right  = this.engine.Options.MarginsAbs.Right;
			var bottom = this.engine.Options.MarginsAbs.Bottom;
			var top    = this.engine.Options.MarginsAbs.Top;

			switch (rank)
			{
				case 0:
					right = rect.Right - pos.X;
					top = rect.Top - pos.Y;
					break;

				case 1:
					right = rect.Right - pos.X;
					break;

				case 2:
					right = rect.Right - pos.X;
					bottom = pos.Y - rect.Bottom;
					break;

				case 3:
					bottom = pos.Y - rect.Bottom;
					break;

				case 4:
					left = pos.X - rect.Left;
					bottom = pos.Y - rect.Bottom;
					break;

				case 5:
					left = pos.X - rect.Left;
					break;

				case 6:
					left = pos.X - rect.Left;
					top = rect.Top - pos.Y;
					break;

				case 7:
					top = rect.Top - pos.Y;
					break;
			}

			left   = System.Math.Max (left,   0);
			right  = System.Math.Max (right,  0);
			bottom = System.Math.Max (bottom, 0);
			top    = System.Math.Max (top,    0);

			if (rect.Width-left-right > 100 && rect.Height-bottom-top > 100)
			{
				this.engine.Options.MarginsAbs = new Margins (left, right, top, bottom);
			}
		}


		private Point? GetHandleTitle(int rank)
		{
			var rect = this.engine.TitleRect;
			var center = rect.Center;

			switch (rank)  // 0 = top right, puis cw
			{
				case 0:
					return rect.TopRight;

				case 1:
					return new Point (rect.Right, center.Y);

				case 2:
					return rect.BottomRight;

				case 3:
					return new Point (center.X, rect.Bottom);

				case 4:
					return rect.BottomLeft;

				case 5:
					return new Point (rect.Left, center.Y);

				case 6:
					return rect.TopLeft;

				case 7:
					return new Point (center.X, rect.Top);
			}

			return null;
		}

		private void SetHandleTitle(int rank, Point pos)
		{
			var rect = this.engine.TitleRect;

			var left   = rect.Left;
			var right  = rect.Right;
			var bottom = rect.Bottom;
			var top    = rect.Top;

			switch (rank)
			{
				case 0:
					right = pos.X;
					top = pos.Y;
					break;

				case 1:
					right = pos.X;
					break;

				case 2:
					right = pos.X;
					bottom = pos.Y;
					break;

				case 3:
					bottom = pos.Y;
					break;

				case 4:
					left = pos.X;
					bottom = pos.Y;
					break;

				case 5:
					left = pos.X;
					break;

				case 6:
					left = pos.X;
					top = pos.Y;
					break;

				case 7:
					top = pos.Y;
					break;
			}

			var cx = (left+right)/2;
			var cy = (bottom+top)/2;
			var dx = System.Math.Max (System.Math.Abs (right-left), 20);
			var dy = System.Math.Max (System.Math.Abs (top-bottom), 10);

			this.engine.TitleRect = new Rectangle (cx-dx/2, cy-dy/2, dx, dy);
		}


		public bool Contains(Point pos)
		{
			//	Retourne true si la surface contient la position.
			if (!this.Rect.IsSurfaceZero)
			{
				return this.Rect.Contains (pos);
			}

			if (this.Path != null)
			{
				return this.Path.SurfaceContainsPoint(pos.X, pos.Y, 1);
			}

			return false;
		}


		public int DetectHandle(Point pos)
		{
			for (int i = 0; i < this.HandleCount; i++)
			{
				var p = this.GetHandle (i);
				if (p.HasValue)
				{
					var rect = new Rectangle (p.Value.X-GraphSurface.handleSize/2, p.Value.Y-GraphSurface.handleSize/2, GraphSurface.handleSize, GraphSurface.handleSize);
					rect.Inflate (1);

					if (rect.Contains (pos))
					{
						return i;
					}
				}
			}

			return -1;
		}


		public void PaintHilited(Graphics graphics)
		{
			if (!this.Rect.IsSurfaceZero)
			{
				var rect = this.Rect;
				graphics.Align (ref rect);
				rect.Inflate (0.5);

				if (this.id.Type != GraphSurfaceType.Margins)
				{
					graphics.AddFilledRectangle (rect);
					graphics.RenderSolid (Color.FromAlphaColor (0.5, Color.FromName ("White")));
				}

				graphics.LineWidth = 3;
				graphics.AddRectangle (rect);
				graphics.RenderSolid (Color.FromName ("White"));
				graphics.LineWidth = 1;

				var path = new Path ();
				path.AppendRectangle (rect);
				graphics.Color = Color.FromName ("Black");
				GraphSurface.PaintDashedPath (graphics, path, 5, 3);
			}
			else if (this.Path != null)
			{
				graphics.Color = Color.FromAlphaColor (0.5, Color.FromName ("White"));
				graphics.PaintSurface (this.Path);

				graphics.LineWidth = 3;
				graphics.Color = Color.FromName ("White");
				graphics.PaintOutline (this.Path);
				graphics.LineWidth = 1;

				graphics.Color = Color.FromName ("Black");
				GraphSurface.PaintDashedPath (graphics, this.Path, 5, 3);
			}
		}

		public void PaintSelected(Graphics graphics)
		{
			var rect = this.Rect;
			graphics.Align (ref rect);

			if (!rect.IsSurfaceZero)
			{
				graphics.LineWidth = 4;
				graphics.AddRectangle (rect);
				graphics.RenderSolid (Color.FromName ("White"));

				graphics.LineWidth = 2;
				graphics.AddRectangle (rect);
				graphics.RenderSolid (Color.FromName ("Blue"));

				graphics.LineWidth = 1;
			}
			else if (this.Path != null)
			{
				graphics.LineWidth = 4;
				graphics.Color = Color.FromName ("White");
				graphics.PaintOutline (this.Path);

				graphics.LineWidth = 2;
				graphics.Color = Color.FromName ("Blue");
				graphics.PaintOutline (this.Path);

				graphics.LineWidth = 1;
			}

			this.PaintPrototype (graphics);

			//	Dessine les poignées.
			for (int i = 0; i < this.HandleCount; i++)
			{
				this.PaintHandle (graphics, this.GetHandle (i));
			}
		}

		private void PaintPrototype(Graphics graphics)
		{
			if (this.id.Type == GraphSurfaceType.Legend)
			{
				var lastPos = this.engine.Options.TempDraggedColumnPos;
				if (!lastPos.IsZero)
				{
					var rect = new Rectangle (this.Rect.Left, this.Rect.Bottom, lastPos.X-this.Rect.Left, this.Rect.Height);
					rect.Inflate (0.5);

					var path = new Path ();
					path.AppendRectangle (rect);
					graphics.Color = Color.FromBrightness (0.5);
					GraphSurface.PaintDashedPath (graphics, path, 1, 4);
				}
			}
		}

		private void PaintHandle(Graphics graphics, Point? pos)
		{
			//	Dessine une poignée.
			if (pos.HasValue)
			{
				var p = pos.Value;
				graphics.Align (ref p);

				var rect = new Rectangle (p.X-GraphSurface.handleSize/2, p.Y-GraphSurface.handleSize/2, GraphSurface.handleSize, GraphSurface.handleSize);
				rect.Inflate (0.5);

				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (Color.FromName ("White"));

				graphics.AddRectangle (rect);
				graphics.RenderSolid (Color.FromName ("Blue"));
			}
		}


		private static void PaintDashedPath(Graphics graphics, Path path, double dashLength, double gapLength)
		{
			//	Dessine un chemin sous forme d'un traitillé.
			var dp = new DashedPath ();
			dp.Append (path);
			dp.AddDash (dashLength, gapLength);

			using (Path dashed = dp.GenerateDashedPath ())
			{
				graphics.PaintOutline (dashed);
			}
		}


		private readonly static double handleSize = 6;  // doit être pair

		private readonly GraphEngine				engine;
		private readonly GraphSurfaceId				id;
	}
}
