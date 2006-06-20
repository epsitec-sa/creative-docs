//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// DockLayoutEngine.
	/// </summary>
	public sealed class DockLayoutEngine : ILayoutEngine
	{
		public void UpdateLayout(Visual container, Drawing.Rectangle rect, IEnumerable<Visual> children)
		{
			System.Collections.Queue fill_queue = null;
			
			Drawing.Rectangle client = rect;
			
			double push_dx = 0;
			double push_dy = 0;
			
			foreach (Visual child in children)
			{
				DockStyle dock = child.Dock;
				
				if ((dock == DockStyle.None) ||
					(dock == DockStyle.Stacked) ||
					(dock == DockStyle.StackBegin) ||
					(dock == DockStyle.StackFill) ||
					(dock == DockStyle.StackEnd))
				{
					//	Saute les widgets qui ne sont pas "docked", car ils doivent être
					//	positionnés par d'autres moyens.
					
					continue;
				}
				
				if (child.Visibility == false)
				{
					continue;
				}
				
				Drawing.Rectangle bounds;
				Drawing.Size size = LayoutContext.GetResultingMeasuredSize (child);

				if (size == Drawing.Size.NegativeInfinity)
				{
					return;
				}

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
				
				switch (dock)
				{
					case DockStyle.Top:
						bounds = new Drawing.Rectangle (client.Left, client.Top - dy, client.Width, dy);
						bounds.Deflate (child.Margins);
						DockLayoutEngine.SetChildBounds (child, bounds);
						client.Top -= dy;
						break;
						
					case DockStyle.Bottom:
						bounds = new Drawing.Rectangle (client.Left, client.Bottom, client.Width, dy);
						bounds.Deflate (child.Margins);
						DockLayoutEngine.SetChildBounds (child, bounds);
						client.Bottom += dy;
						break;
					
					case DockStyle.Left:
						bounds = new Drawing.Rectangle (client.Left, client.Bottom, dx, client.Height);
						bounds.Deflate (child.Margins);
						DockLayoutEngine.SetChildBounds (child, bounds);
						client.Left += dx;
						break;
					
					case DockStyle.Right:
						bounds = new Drawing.Rectangle (client.Right - dx, client.Bottom, dx, client.Height);
						bounds.Deflate (child.Margins);
						DockLayoutEngine.SetChildBounds (child, bounds);
						client.Right -= dx;
						break;
					
					case DockStyle.Fill:
						if (fill_queue == null)
						{
							fill_queue = new System.Collections.Queue ();
						}
						fill_queue.Enqueue (child);
						break;
				}
			}
			
			if (fill_queue != null)
			{
				Drawing.Rectangle bounds;
				int n = fill_queue.Count;
				
				double fill_dx = client.Width;
				double fill_dy = client.Height;
				
				switch (container.ContainerLayoutMode)
				{
					case ContainerLayoutMode.HorizontalFlow:
						foreach (Visual child in fill_queue)
						{
							double min_dx = child.MinWidth;
							double new_dx = fill_dx / n;
							
							if (new_dx < min_dx)
							{
								push_dx += min_dx - new_dx;
								new_dx   = min_dx;
							}
							
							bounds = new Drawing.Rectangle (client.Left, client.Bottom, new_dx, client.Height);
							bounds.Deflate (child.Margins);

							DockLayoutEngine.SetChildBounds (child, bounds);
							client.Left += new_dx;
						}
						break;
					
					case ContainerLayoutMode.VerticalFlow:
						foreach (Visual child in fill_queue)
						{
							double min_dy = child.MinHeight;
							double new_dy = fill_dy / n;
							
							if (new_dy < min_dy)
							{
								push_dy += min_dy - new_dy;
								new_dy   = min_dy;
							}
							
							bounds = new Drawing.Rectangle (client.Left, client.Top - new_dy, client.Width, new_dy);
							bounds.Deflate (child.Margins);

							DockLayoutEngine.SetChildBounds (child, bounds);
							client.Top -= new_dy;
						}
						break;
				}
			}
			
			if (push_dy > 0)
			{
				foreach (Visual child in children)
				{
					Drawing.Rectangle bounds;
					
					if ((child.Dock != DockStyle.Bottom) ||
						(child.Visibility == false))
					{
						continue;
					}
					
					bounds = child.GetCurrentBounds ();
					bounds.Offset (0, - push_dy);
					child.SetBounds (bounds);
				}
			}
			
			if (push_dx > 0)
			{
				foreach (Visual child in children)
				{
					Drawing.Rectangle bounds;
					
					if ((child.Dock != DockStyle.Right) ||
						(child.Visibility == false))
					{
						continue;
					}
					
					bounds = child.GetCurrentBounds ();
					bounds.Offset (push_dx, 0);
					child.SetBounds (bounds);
				}
			}
		}

		public void UpdateMinMax(Visual container, LayoutContext context, IEnumerable<Visual> children, ref Drawing.Size minSize, ref Drawing.Size maxSize)
		{
			//	Décompose les dimensions comme suit :
			//
			//	|											  |
			//	|<---min_ox1--->| zone de travail |<-min_ox2->|
			//	|											  |
			//	|<-------------------min_dx------------------>|
			//
			//	min_ox = min_ox1 + min_ox2
			//	min_dx = minimum courant
			//
			//	La partie centrale (DockStyle.Fill) va s'additionner au reste de manière
			//	indépendante au moyen du fill_min_dx.
			//
			//	Idem par analogie pour dy et max.

			double minOx = 0;
			double minOy = 0;
			double maxOx = 0;
			double maxOy = 0;

			double minDx = minSize.Width;
			double minDy = minSize.Height;
			double maxDx = maxSize.Width;
			double maxDy = maxSize.Height;
			
			double fillMinDx = 0;
			double fillMinDy = 0;
			double fillMaxDx = 0;
			double fillMaxDy = 0;
			
			switch (container.ContainerLayoutMode)
			{
				case ContainerLayoutMode.HorizontalFlow:
					fillMaxDy = maxDy;
					break;
				
				case ContainerLayoutMode.VerticalFlow:
					fillMaxDx = maxDx;
					break;
			}
			
			foreach (Visual child in children)
			{
				DockStyle dock = child.Dock;
				
				if ((dock == DockStyle.None) ||
					(dock == DockStyle.Stacked) ||
					(dock == DockStyle.StackBegin) ||
					(dock == DockStyle.StackFill) ||
					(dock == DockStyle.StackEnd))
				{
					//	Saute les widgets qui ne sont pas "docked", car leur taille n'est pas prise
					//	en compte dans le calcul des minima/maxima.
					
					continue;
				}
				
				if (child.Visibility == false)
				{
					continue;
				}

				Drawing.Size margins = child.Margins.Size;

				Layouts.LayoutMeasure measureDx = Layouts.LayoutMeasure.GetWidth (child);
				Layouts.LayoutMeasure measureDy = Layouts.LayoutMeasure.GetHeight (child);

				Drawing.Size min = new Drawing.Size (measureDx.Min + margins.Width, measureDy.Min + margins.Height);
				Drawing.Size max = new Drawing.Size (measureDx.Max + margins.Width, measureDy.Max + margins.Width);

				double clientDx = measureDx.Desired + margins.Width;
				double clientDy = measureDy.Desired + margins.Height;

				switch (dock)
				{
					case DockStyle.Top:
						minDx  = System.Math.Max (minDx, min.Width + minOx);
						minDy  = System.Math.Max (minDy, clientDy + minOy);
						minOy += clientDy;
						maxDx  = System.Math.Min (maxDx, max.Width + maxOx);
//						maxDy  = System.Math.Min (maxDy, child.Height + maxOy);
						maxOy += clientDy;
						break;
					
					case DockStyle.Bottom:
						minDx  = System.Math.Max (minDx, min.Width + minOx);
						minDy  = System.Math.Max (minDy, clientDy + minOy);
						minOy += clientDy;
						maxDx  = System.Math.Min (maxDx, max.Width + maxOx);
//						maxDy  = System.Math.Min (maxDy, child.Height + maxOy);
						maxOy += clientDy;
						break;
						
					case DockStyle.Left:
						minDx  = System.Math.Max (minDx, clientDx + minOx);
						minDy  = System.Math.Max (minDy, min.Height + minOy);
						minOx += clientDx;
//						maxDx  = System.Math.Min (maxDx, child.Width + maxOx);
						maxDy  = System.Math.Min (maxDy, max.Height + maxOy);
						maxOx += clientDx;
						break;
					
					case DockStyle.Right:
						minDx  = System.Math.Max (minDx, clientDx + minOx);
						minDy  = System.Math.Max (minDy, min.Height + minOy);
						minOx += clientDx;
//						maxDx  = System.Math.Min (maxDx, child.Width + maxOx);
						maxDy  = System.Math.Min (maxDy, max.Height + maxOy);
						maxOx += clientDx;
						break;
					
					case DockStyle.Fill:
						switch (container.ContainerLayoutMode)
						{
							case ContainerLayoutMode.HorizontalFlow:
								fillMinDx += min.Width;
								fillMinDy  = System.Math.Max (fillMinDy, min.Height);
								fillMaxDx += max.Width;
								fillMaxDy  = System.Math.Min (fillMaxDy, max.Height);
								break;
							
							case ContainerLayoutMode.VerticalFlow:
								fillMinDx  = System.Math.Max (fillMinDx, min.Width);
								fillMinDy += min.Height;
								fillMaxDx  = System.Math.Min (fillMaxDx, max.Width);
								fillMaxDy += max.Height;
								break;
						}
						break;
				}
			}
			
			if (fillMaxDx == 0)
			{
				fillMaxDx = double.PositiveInfinity;
			}
			
			if (fillMaxDy == 0)
			{
				fillMaxDy = double.PositiveInfinity;
			}

			double minWidth  = System.Math.Max (minDx, fillMinDx + minOx);
			double minHeight = System.Math.Max (minDy, fillMinDy + minOy);
			double maxWidth  = System.Math.Min (maxDx, fillMaxDx + maxOx);
			double maxHeight = System.Math.Min (maxDy, fillMaxDy + maxOy);
			
			minSize = new Drawing.Size (minWidth, minHeight);
			maxSize = new Drawing.Size (maxWidth, maxHeight);
		}

		public LayoutMode LayoutMode
		{
			get
			{
				return LayoutMode.Docked;
			}
		}

		internal static void SetChildBounds(Visual child, Drawing.Rectangle bounds)
		{
			DockLayoutEngine.SetChildBounds (child, bounds, 0);
		}
		internal static void SetChildBounds(Visual child, Drawing.Rectangle bounds, double baseOffset)
		{
			double dx = child.PreferredWidth;
			double dy = child.PreferredHeight;
			
			double h, h1, h2;
			
			switch (child.VerticalAlignment)
			{
				case VerticalAlignment.Stretch:
					break;
				case VerticalAlignment.Top:
					bounds.Bottom = bounds.Top - dy;
					break;
				case VerticalAlignment.Center:
					h = bounds.Height;
					bounds.Top = bounds.Top - (h - dy) / 2;
					bounds.Bottom = bounds.Bottom + (h - dy) / 2;
					break;
				case VerticalAlignment.Bottom:
					bounds.Top = bounds.Bottom + dy;
					break;
				case VerticalAlignment.BaseLine:
					Layouts.LayoutContext.GetMeasuredBaseLine (child, out h1, out h2);
					bounds.Top    = bounds.Bottom + baseOffset + h1;
					bounds.Bottom = bounds.Bottom + baseOffset - h2;
					break;
			}
			
			switch (child.HorizontalAlignment)
			{
				case HorizontalAlignment.Stretch:
					break;
				case HorizontalAlignment.Left:
					bounds.Right = bounds.Left + dx;
					break;
				case HorizontalAlignment.Center:
					double w = bounds.Width;
					bounds.Left = bounds.Left + (w - dx) / 2;
					bounds.Right = bounds.Right - (w - dx) / 2;
					break;
				case HorizontalAlignment.Right:
					bounds.Left = bounds.Right - dx;
					break;
			}

			Drawing.Rectangle oldBounds = child.GetCurrentBounds ();
			Drawing.Rectangle newBounds = bounds;

			child.SetBounds (newBounds);

			if (oldBounds != newBounds)
			{
				child.Arrange (Helpers.VisualTree.FindLayoutContext (child));
			}
		}
	}
}
