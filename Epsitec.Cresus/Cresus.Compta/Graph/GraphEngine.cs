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
			this.cube = new Cube (1);
		}


		public void PaintGraph(Graphics graphics, Rectangle rect, GraphicData data)
		{
			switch (data.Mode)
			{
				case GraphicMode.Normal:
					this.PaintGraphNormal (graphics, rect, data.MinValue, data.MaxValue, data.Values[0]);
					break;

				case GraphicMode.Budget:
					this.PaintGraphBudget (graphics, rect, data.MinValue, data.MaxValue, data.Values[0], data.Values[1]);
					break;

				case GraphicMode.Cumulé:
					this.PaintGraphCumulé (graphics, rect, data.MinValue, data.MaxValue, data.Values);
					break;

				case GraphicMode.Empilé:
					this.PaintGraphEmpilé (graphics, rect, data.MinValue, data.MaxValue, data.Values);
					break;
			}
		}

		private void PaintGraphNormal(Graphics graphics, Rectangle rect, decimal min, decimal max, decimal value)
		{
			if (max-min == 0)
			{
				return;
			}

			var borderColor = GraphEngine.GetBorderColor ();

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromAlphaColor (0.5, borderColor));

			rect.Deflate (2);
			rect = graphics.Align (rect);
			rect.Deflate (0.5);

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromBrightness (1));
			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromAlphaColor (0.2, borderColor));

			double sep = System.Math.Floor (rect.Width * (double) -min  / (double) (max-min));
			double val = System.Math.Floor (rect.Width * (double) value / (double) (max-min));

			if (val < 0)
			{
				var r = new Rectangle (rect.Left+sep+val+0.5, rect.Bottom, -val, rect.Height);
				graphics.AddFilledRectangle (r);
				graphics.RenderSolid (UIBuilder.GraphicRedColor);
			}

			if (val > 0)
			{
				var r = new Rectangle (rect.Left+sep, rect.Bottom, val+0.5, rect.Height);
				graphics.AddFilledRectangle (r);
				graphics.RenderSolid (UIBuilder.GraphicGreenColor);
			}

			graphics.AddLine (rect.Left+sep, rect.Bottom, rect.Left+sep, rect.Top);
			graphics.RenderSolid (borderColor);

			graphics.AddRectangle (rect);
			graphics.RenderSolid (borderColor);
		}

		private void PaintGraphBudget(Graphics graphics, Rectangle rect, decimal min, decimal max, decimal value, decimal solde)
		{
			if (max-min == 0)
			{
				return;
			}

			var borderColor = GraphEngine.GetBorderColor ();

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromAlphaColor (0.5, borderColor));

			rect.Deflate (2);
			rect = graphics.Align (rect);
			rect.Deflate (0.5);

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromBrightness (1));
			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromAlphaColor (0.2, borderColor));

			double sep = System.Math.Floor (rect.Width * (double) -min  / (double) (max-min));
			double val = System.Math.Floor (rect.Width * (double) value / (double) (max-min));
			double sol = System.Math.Floor (rect.Width * (double) solde / (double) (max-min));

			var color = value >= solde ? UIBuilder.GraphicGreenColor : UIBuilder.GraphicRedColor;

			if (val < 0)
			{
				var r = new Rectangle (rect.Left+sep+val+0.5, rect.Bottom, -val, rect.Height);
				graphics.AddFilledRectangle (r);
				graphics.RenderSolid (color);
			}

			if (val > 0)
			{
				var r = new Rectangle (rect.Left+sep, rect.Bottom, val+0.5, rect.Height);
				graphics.AddFilledRectangle (r);
				graphics.RenderSolid (color);
			}

			graphics.AddLine (rect.Left+sep, rect.Bottom, rect.Left+sep, rect.Top);
			graphics.AddLine (rect.Left+sol, rect.Bottom, rect.Left+sol, rect.Top);
			graphics.RenderSolid (borderColor);

			graphics.AddRectangle (rect);
			graphics.RenderSolid (borderColor);
		}

		private void PaintGraphCumulé(Graphics graphics, Rectangle rect, decimal min, decimal max, List<decimal> values)
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

			var cumuls = new List<decimal> ();
			decimal sum = 0;
			for (int i = 0; i < values.Count; i++)
			{
				sum += values[i];
				cumuls.Add (sum);
			}

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
					graphics.RenderSolid (GraphEngine.GetRainbowColor (i));

					graphics.AddLine (rect.Left+x2-0.5, rect.Bottom+0.5, rect.Left+x2-0.5, rect.Top-0.5);
					graphics.RenderSolid (borderColor);
				}
			}

			rect.Inflate (0.5);

			graphics.AddRectangle (rect);
			graphics.RenderSolid (borderColor);
		}

		private void PaintGraphEmpilé(Graphics graphics, Rectangle rect, decimal min, decimal max, List<decimal> values)
		{
			//	Dessine le graphique en mode "résumé périodique", avec plusieurs barres empilées.
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

			int dy = (int) System.Math.Max (rect.Height/values.Count, 1);
			int h = dy*values.Count - 1;
			int o = (int) (rect.Height-h)/2;
			rect = new Rectangle (rect.Left, rect.Bottom+o, rect.Width, h);

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromBrightness (1));
			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromAlphaColor (0.2, borderColor));

			double zero = System.Math.Floor (rect.Width * (double) -min / (double) (max-min));

			double y = 0;

			for (int i = 0; i < values.Count; i++)
			{
				var value = values[i];

				double x = System.Math.Floor (rect.Width * (double) (value-min) / (double) (max-min));
				double x1 = System.Math.Min (x, zero);
				double x2 = System.Math.Max (x, zero);

				var r = new Rectangle (rect.Left+x1, rect.Top-y-dy, x2-x1, dy);
				graphics.AddFilledRectangle (r);
				graphics.RenderSolid (GraphEngine.GetRainbowColor (i));

				graphics.AddLine (rect.Left+x-0.5, rect.Top-y-dy+1.5, rect.Left+x-0.5, rect.Top-y-0.5);
				graphics.RenderSolid (borderColor);

				if (i < values.Count-1)
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


		private Cube			cube;
	}
}
