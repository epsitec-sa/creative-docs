//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// StackLayout.
	/// </summary>
	public sealed class StackLayoutEngine : ILayoutEngine
	{
		public void UpdateLayout(Visual container, Drawing.Rectangle rect, IEnumerable<Visual> children)
		{
			Drawing.Rectangle client = rect;
			ContainerLayoutMode mode = container.ContainerLayoutMode;
			
			double h1 = 0;
			double h2 = 0;

			LayoutContext.GetMeasuredBaseLine (container, out h1, out h2);
			
			double baseLine = (client.Height - (h1+h2)) / 2 + h2;

			foreach (Visual child in children)
			{
				if ((child.Dock != DockStyle.Stacked) ||
					(child.Visibility == false))
				{
					//	Saute les widgets qui ne sont pas "stacked", car ils doivent être
					//	positionnés par d'autres moyens.
					
					continue;
				}

				Drawing.Rectangle bounds  = client;
				Drawing.Margins   margins = child.Margins;
				Drawing.Size      size    = LayoutContext.GetResultingMeasuredSize (child);

				double dx = size.Width;
				double dy = size.Height;

				if (double.IsNaN (dx))
				{
					dx = child.GetCurrentBounds ().Width;		//	TODO: améliorer
				}
				if (double.IsNaN (dy))
				{
					dy = child.GetCurrentBounds ().Height;		//	TODO: améliorer
				}

				dx += margins.Width;
				dy += margins.Height;

				switch (mode)
				{
					case ContainerLayoutMode.HorizontalFlow:
						bounds.Width = dx;
						client.Left  = bounds.Right;
						break;
					
					case ContainerLayoutMode.VerticalFlow:
						bounds.Bottom = bounds.Top - dy;
						client.Top    = bounds.Bottom;
						break;
				}

				bounds.Deflate (margins);
				DockLayoutEngine.SetChildBounds (child, bounds, baseLine - margins.Bottom);
			}
		}

		public void UpdateMinMax(Visual container, LayoutContext context, IEnumerable<Visual> children, ref Drawing.Size minSize, ref Drawing.Size maxSize)
		{
			ContainerLayoutMode mode = container.ContainerLayoutMode;

			double currentMinDx = 0;
			double currentMinDy = 0;
			double currentMinH1 = 0;
			double currentMinH2 = 0;
			double currentMaxDx = double.PositiveInfinity;
			double currentMaxDy = double.PositiveInfinity;
			
			foreach (Visual child in children)
			{
				if ((child.Dock != DockStyle.Stacked) ||
					(child.Visibility == false))
				{
					//	Saute les widgets qui ne sont pas "stacked", car leur taille n'est pas prise
					//	en compte dans le calcul des minima/maxima.
					
					continue;
				}

				Drawing.Margins margins = child.Margins;

				Layouts.LayoutMeasure measureDx = Layouts.LayoutMeasure.GetWidth (child);
				Layouts.LayoutMeasure measureDy = Layouts.LayoutMeasure.GetHeight (child);

				Drawing.Size clientMin = new Drawing.Size (measureDx.Min + margins.Width, measureDy.Min + margins.Height);
				Drawing.Size clientMax = new Drawing.Size (measureDx.Max + margins.Width, measureDy.Max + margins.Width);

				double clientDx = measureDx.Desired + margins.Width;
				double clientDy = measureDy.Desired + margins.Height;

				switch (mode)
				{
					case ContainerLayoutMode.HorizontalFlow:
						currentMinDx += clientDx;
						currentMaxDx += clientDx;

						if (child.VerticalAlignment == VerticalAlignment.BaseLine)
						{
							double h1;
							double h2;
							
							LayoutContext.GetMeasuredBaseLine (child, out h1, out h2);

							currentMinH1 = System.Math.Max (currentMinH1, h1 + margins.Top);
							currentMinH2 = System.Math.Max (currentMinH2, h2 + margins.Bottom);
						}
						else
						{
							currentMinDy = System.Math.Max (currentMinDy, clientMin.Height);
							currentMaxDy = System.Math.Min (currentMaxDy, clientMax.Height);
						}
						break;
					
					case ContainerLayoutMode.VerticalFlow:
						currentMinDy += clientDy;
						currentMaxDy += clientDy;
						currentMinDx  = System.Math.Max (currentMinDx, clientMin.Width);
						currentMaxDx  = System.Math.Min (currentMaxDx, clientMax.Width);
						break;
				}
			}

			currentMinDy = System.Math.Max (currentMinDy, currentMinH1 + currentMinH2);

			double minWidth  = System.Math.Max (minSize.Width,  currentMinDx);
			double minHeight = System.Math.Max (minSize.Height, currentMinDy);
			double maxWidth  = System.Math.Min (maxSize.Width,  currentMaxDx);
			double maxHeight = System.Math.Min (maxSize.Height, currentMaxDy);

			minSize = new Drawing.Size (minWidth, minHeight);
			maxSize = new Drawing.Size (maxWidth, maxHeight);

			if (context != null)
			{
				context.DefineBaseLine (container, currentMinH1, currentMinH2);
			}
		}
	}
}
