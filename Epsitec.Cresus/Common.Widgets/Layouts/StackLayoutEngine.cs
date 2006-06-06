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

			foreach (Visual child in children)
			{
				if ((child.Dock != DockStyle.Stacked) ||
					(child.Visibility == false))
				{
					//	Saute les widgets qui ne sont pas "stacked", car ils doivent être
					//	positionnés par d'autres moyens.

					continue;
				}
				
				double childH1;
				double childH2;

				Drawing.Rectangle bounds = client;
				Drawing.Size size = LayoutContext.GetResultingMeasuredBaseLine (child, out childH1, out childH2);

				if (size == Drawing.Size.NegativeInfinity)
				{
					return;
				}

				h1 = System.Math.Max (h1, childH1);
				h2 = System.Math.Max (h2, childH2);
			}

			double baseLine = System.Math.Round ((client.Height - (h1+h2)) / 2 + h2);

			foreach (Visual child in children)
			{
				if ((child.Dock != DockStyle.Stacked) ||
					(child.Visibility == false))
				{
					//	Saute les widgets qui ne sont pas "stacked", car ils doivent être
					//	positionnés par d'autres moyens.
					
					continue;
				}

				Drawing.Rectangle bounds = client;
				Drawing.Size size = LayoutContext.GetResultingMeasuredSize (child);

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

				dx += child.Margins.Width;
				dy += child.Margins.Height;

				switch (mode)
				{
					case ContainerLayoutMode.HorizontalFlow:
						bounds.Width = dx;
						client.Left = bounds.Right;
						break;
					case ContainerLayoutMode.VerticalFlow:
						bounds.Bottom = bounds.Top - dy;
						client.Top = bounds.Bottom;
						break;
				}
				
				DockLayoutEngine.SetChildBounds (child, bounds, baseLine);
			}
		}

		public void UpdateMinMax(Visual container, IEnumerable<Visual> children, ref Drawing.Size min_size, ref Drawing.Size max_size)
		{
			ContainerLayoutMode mode = container.ContainerLayoutMode;

			double minDx = 0;
			double minDy = 0;
			double minH1 = 0;
			double minH2 = 0;
			double maxDx = 0;
			double maxDy = 0;
			
			foreach (Visual child in children)
			{
				if ((child.Dock != DockStyle.Stacked) ||
					(child.Visibility == false))
				{
					//	Saute les widgets qui ne sont pas "stacked", car leur taille n'est pas prise
					//	en compte dans le calcul des minima/maxima.
					
					continue;
				}

				Drawing.Size margins = child.Margins.Size;

				Layouts.LayoutMeasure measure_dx = Layouts.LayoutMeasure.GetWidth (child);
				Layouts.LayoutMeasure measure_dy = Layouts.LayoutMeasure.GetHeight (child);

				Drawing.Size clientMin = new Drawing.Size (measure_dx.Min + margins.Width, measure_dy.Min + margins.Height);
				Drawing.Size clientMax = new Drawing.Size (measure_dx.Max + margins.Width, measure_dy.Max + margins.Width);

				double clientDx = measure_dx.Desired + margins.Width;
				double clientDy = measure_dy.Desired + margins.Height;

				switch (mode)
				{
					case ContainerLayoutMode.HorizontalFlow:
						minDx += clientMin.Width;
						maxDx += clientMax.Width;

						if (child.VerticalAlignment == VerticalAlignment.BaseLine)
						{
							Drawing.Point baseLine = child.GetBaseLine ();

							double h2 = baseLine.Y;
							double h1 = clientDy - h2;

							minH1 = System.Math.Max (minH1, h1);
							minH2 = System.Math.Max (minH2, h2);
						}
						else
						{
							minDy = System.Math.Max (minDy, clientMin.Height);
							maxDy = System.Math.Min (maxDy, clientMax.Height);
						}
						break;
					
					case ContainerLayoutMode.VerticalFlow:
						minDy += clientMin.Height;
						maxDy += clientMax.Height;
						minDx  = System.Math.Max (minDx, clientMin.Width);
						maxDx  = System.Math.Min (maxDx, clientMax.Width);
						break;
				}
			}

			minDy = System.Math.Max (minDy, minH1 + minH2);

			Drawing.Margins padding = container.Padding + container.GetInternalPadding ();
			Layouts.LayoutContext context = Layouts.LayoutContext.GetLayoutContext (container);

			if (context != null)
			{
				context.DefineMinWidth (container, min_size.Width);
				context.DefineMinHeight (container, min_size.Height);
				context.DefineBaseLine (container, minH1, minH2);
				context.DefineMaxWidth (container, max_size.Width);
				context.DefineMaxHeight (container, max_size.Height);
			}
		}
	}
}
