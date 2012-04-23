//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

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


		public void PaintGraph(Graphics graphics, Rectangle rect, Cube cube, string param)
		{
			var words = param.Split (';');
			var row = int.Parse (words[1]);

			this.PaintGraph (graphics, rect, cube, row);
		}

		private void PaintGraph(Graphics graphics, Rectangle rect, Cube cube, int row)
		{
			System.Diagnostics.Debug.Assert (cube.Dimensions == 2);

			int nx = cube.GetCount (0);
			int ny = cube.GetCount (1);

			decimal finalMin = 0;
			decimal finalMax = 0;

			if (cube.Mode == GraphicMode.Cumulé)
			{
				for (int yy = 0; yy < ny; yy++)
				{
					decimal sum = 0;

					for (int xx = 0; xx < nx; xx++)
					{
						sum += System.Math.Max (cube.GetValue (xx, yy).GetValueOrDefault (), 0);
					}

					finalMax = System.Math.Max (finalMax, sum);
				}
			}
			else
			{
				cube.GetMinMax (null, null, out finalMin, out finalMax);
			}

			switch (cube.Mode)
			{
				case GraphicMode.Cumulé:
					this.PaintGraphCumulé (graphics, rect, finalMin, finalMax, cube, row);
					break;

				case GraphicMode.Empilé:
					this.PaintGraphEmpilé (graphics, rect, finalMin, finalMax, cube, row);
					break;
			}
		}


		private void PaintGraphCumulé(Graphics graphics, Rectangle rect, decimal min, decimal max, Cube cube, int row)
		{
			//	Dessine le graphique en mode "résumé périodique", avec plusieurs barres cumulées.
			if (max == 0)
			{
				return;
			}

			var borderColor = GraphEngine.GetBorderColor ();

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromAlphaColor (0.5, borderColor));

			rect.Deflate (2);
			rect = graphics.Align (rect);
			rect.Deflate (1);

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromBrightness (1));
			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromAlphaColor (0.2, borderColor));

			int nx = cube.GetCount (0);
			var cumuls = new List<decimal> ();
			decimal sum = 0;
			for (int i = 0; i < nx; i++)
			{
				sum += cube.GetValue (i, row).GetValueOrDefault ();
				cumuls.Add (sum);
			}

			int step = System.Math.Max (11/cumuls.Count, 1);

			for (int i = cumuls.Count-1; i >= 0; i--)
			{
				var v1 = (i == 0) ? 0 : cumuls[i-1];
				var v2 = cumuls[i];

				double x1 = System.Math.Floor (rect.Width * (double) v1 / (double) max);
				double x2 = System.Math.Floor (rect.Width * (double) v2 / (double) max);
				double dx = x2-x1;

				if (dx > 0)
				{
					var r = new Rectangle (rect.Left+x1, rect.Bottom, dx, rect.Height);
					graphics.AddFilledRectangle (r);
					graphics.RenderSolid (GraphEngine.GetRainbowColor (i*step));

					graphics.AddLine (rect.Left+x2-0.5, rect.Bottom+0.5, rect.Left+x2-0.5, rect.Top-0.5);
					graphics.RenderSolid (borderColor);
				}
			}

			rect.Inflate (0.5);

			graphics.AddRectangle (rect);
			graphics.RenderSolid (borderColor);
		}

		private void PaintGraphEmpilé(Graphics graphics, Rectangle rect, decimal min, decimal max, Cube cube, int row)
		{
			//	Dessine le graphique en mode "résumé périodique", avec plusieurs barres empilées.
			min = System.Math.Min (min, 0);
			max = System.Math.Max (max, 0);

			if (max-min == 0)
			{
				return;
			}

			var borderColor = GraphEngine.GetBorderColor ();

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromAlphaColor (0.5, borderColor));

			rect.Deflate (2);
			rect = graphics.Align (rect);
			rect.Deflate (1);

			int nx = cube.GetCount (0);

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
			int step = System.Math.Max (11/nx, 1);

			for (int i = 0; i < nx; i++)
			{
				var value = cube.GetValue (i, row).GetValueOrDefault ();

				double x  = System.Math.Floor (rect.Width * (double) (value-min) / (double) (max-min));
				double x1 = System.Math.Min (x, zero);
				double x2 = System.Math.Max (x, zero);

				var r = new Rectangle (rect.Left+x1, rect.Top-y-dy, x2-x1, dy);
				graphics.AddFilledRectangle (r);
				graphics.RenderSolid (GraphEngine.GetRainbowColor (i*step));

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


		private static Color GetBorderColor()
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			return adorner.ColorTextFieldBorder (true);
		}

		private static Color GetRainbowColor(int index)
		{
			//	Retourne une couleur de l'arc-en-ciel visuellement identifiables.
			return Color.FromHsv (GraphEngine.rainbow[index%GraphEngine.rainbow.Length], 1, 1);
		}

		private static int[] rainbow = { 0, 40, 60, 90, 180, 190, 200, 210, 240, 270, 300 };
	}
}
