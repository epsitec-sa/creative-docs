//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// DockLayout.
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
				if (child.Dock == DockStyle.None)
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
					dx = child.Width;		//	TODO: améliorer
				}
				if (double.IsNaN (dy))
				{
					dy = child.Height;		//	TODO: améliorer
				}

				dx += child.Margins.Width;
				dy += child.Margins.Height;
				
				switch (child.Dock)
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
					
					bounds = child.Bounds;
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
					
					bounds = child.Bounds;
					bounds.Offset (push_dx, 0);
					child.SetBounds (bounds);
				}
			}
		}

		public void UpdateMinMax(Visual container, IEnumerable<Visual> children, ref Drawing.Size min_size, ref Drawing.Size max_size)
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
			
			double min_ox = 0;
			double min_oy = 0;
			double max_ox = 0;
			double max_oy = 0;
			
			double min_dx = 0;
			double min_dy = 0;
			double max_dx = double.PositiveInfinity;
			double max_dy = double.PositiveInfinity;
			
			double fill_min_dx = 0;
			double fill_min_dy = 0;
			double fill_max_dx = 0;
			double fill_max_dy = 0;
			
			switch (container.ContainerLayoutMode)
			{
				case ContainerLayoutMode.HorizontalFlow:
					fill_max_dy = max_dy;
					break;
				
				case ContainerLayoutMode.VerticalFlow:
					fill_max_dx = max_dx;
					break;
			}
			
			foreach (Visual child in children)
			{
				if (child.Dock == DockStyle.None)
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

				Layouts.LayoutMeasure measure_dx = Layouts.LayoutMeasure.GetWidth (child);
				Layouts.LayoutMeasure measure_dy = Layouts.LayoutMeasure.GetHeight (child);

				Drawing.Size min = new Drawing.Size (measure_dx.Min + margins.Width, measure_dy.Min + margins.Height);
				Drawing.Size max = new Drawing.Size (measure_dx.Max + margins.Width, measure_dy.Max + margins.Width);

				switch (child.Dock)
				{
					case DockStyle.Top:
						min_dx  = System.Math.Max (min_dx, min.Width    + min_ox);
						min_dy  = System.Math.Max (min_dy, child.Height + min_oy);
						min_oy += child.Height + margins.Height;
						max_dx  = System.Math.Min (max_dx, max.Width    + max_ox);
//						max_dy  = System.Math.Min (max_dy, child.Height + max_oy);
						max_oy += child.Height + margins.Height;
						break;
					
					case DockStyle.Bottom:
						min_dx  = System.Math.Max (min_dx, min.Width    + min_ox);
						min_dy  = System.Math.Max (min_dy, child.Height + min_oy);
						min_oy += child.Height + margins.Height;
						max_dx  = System.Math.Min (max_dx, max.Width    + max_ox);
//						max_dy  = System.Math.Min (max_dy, child.Height + max_oy);
						max_oy += child.Height + margins.Height;
						break;
						
					case DockStyle.Left:
						min_dx  = System.Math.Max (min_dx, child.Width  + min_ox);
						min_dy  = System.Math.Max (min_dy, min.Height   + min_oy);
						min_ox += child.Width + margins.Width;
//						max_dx  = System.Math.Min (max_dx, child.Width  + max_ox);
						max_dy  = System.Math.Min (max_dy, max.Height   + max_oy);
						max_ox += child.Width + margins.Width;
						break;
					
					case DockStyle.Right:
						min_dx  = System.Math.Max (min_dx, child.Width  + min_ox);
						min_dy  = System.Math.Max (min_dy, min.Height   + min_oy);
						min_ox += child.Width + margins.Width;
//						max_dx  = System.Math.Min (max_dx, child.Width  + max_ox);
						max_dy  = System.Math.Min (max_dy, max.Height   + max_oy);
						max_ox += child.Width + margins.Width;
						break;
					
					case DockStyle.Fill:
						switch (container.ContainerLayoutMode)
						{
							case ContainerLayoutMode.HorizontalFlow:
								fill_min_dx += min.Width;
								fill_min_dy  = System.Math.Max (fill_min_dy, min.Height);
								fill_max_dx += max.Width;
								fill_max_dy  = System.Math.Min (fill_max_dy, max.Height);
								break;
							
							case ContainerLayoutMode.VerticalFlow:
								fill_min_dx  = System.Math.Max (fill_min_dx, min.Width);
								fill_min_dy += min.Height;
								fill_max_dx  = System.Math.Min (fill_max_dx, max.Width);
								fill_max_dy += max.Height;
								break;
						}
						break;
				}
			}
			
			if (fill_max_dx == 0)
			{
				fill_max_dx = double.PositiveInfinity;
			}
			
			if (fill_max_dy == 0)
			{
				fill_max_dy = double.PositiveInfinity;
			}
			
			double pad_width  = container.Padding.Width  + container.InternalPadding.Width;
			double pad_height = container.Padding.Height + container.InternalPadding.Height;
			
			double min_width  = System.Math.Max (min_dx, fill_min_dx + min_ox) + pad_width;
			double min_height = System.Math.Max (min_dy, fill_min_dy + min_oy) + pad_height;
			double max_width  = System.Math.Min (max_dx, fill_max_dx + max_ox) + pad_width;
			double max_height = System.Math.Min (max_dy, fill_max_dy + max_oy) + pad_height;
			
			//	Tous les calculs ont été faits en coordonnées client, il faut donc encore transformer
			//	ces dimensions en coordonnées parents.
			
			min_size = Helpers.VisualTree.MapVisualToParent (container, new Drawing.Size (min_width, min_height));
			max_size = Helpers.VisualTree.MapVisualToParent (container, new Drawing.Size (max_width, max_height));

			Widget widget = container as Widget;
			
			//	TODO: supprimer ce hack (AutoMinSize et AutoMaxSize ne devraient plus exister)
			
			if (widget != null)
			{
				widget.AutoMinSize = min_size;
				widget.AutoMaxSize = max_size;
			}
		}

		private static void SetChildBounds(Visual child, Drawing.Rectangle bounds)
		{
			double dx = child.PreferredWidth;
			double dy = child.PreferredHeight;
			
			switch (child.VerticalAlignment)
			{
				case VerticalAlignment.Stretch:
					break;
				case VerticalAlignment.Top:
					bounds.Bottom = bounds.Top - dy;
					break;
				case VerticalAlignment.Center:
					double h = bounds.Height;
					bounds.Top = bounds.Top - (h - dy) / 2;
					bounds.Bottom = bounds.Bottom + (h - dy) / 2;
					break;
				case VerticalAlignment.Bottom:
					bounds.Top = bounds.Bottom + dy;
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

			child.SetBounds (bounds);
		}
	}
}
