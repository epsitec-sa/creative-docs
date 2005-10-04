//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// DockLayout.
	/// </summary>
	public sealed class DockLayout : ILayout
	{
		public void UpdateLayout(Visual container, System.Collections.ICollection children)
		{
			System.Collections.Queue fill_queue = null;
			
			Drawing.Rectangle client = container.ClientBounds;
			
			client.Deflate (container.DockPadding);
			
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
				bounds = child.Bounds;
				bounds.Inflate (child.DockMargins);
				
				double dx = bounds.Width;
				double dy = bounds.Height;
				
				switch (child.Dock)
				{
					case DockStyle.Top:
						bounds = new Drawing.Rectangle (client.Left, client.Top - dy, client.Width, dy);
						bounds.Deflate (child.DockMargins);
						child.SetBounds (bounds);
						client.Top -= dy;
						break;
						
					case DockStyle.Bottom:
						bounds = new Drawing.Rectangle (client.Left, client.Bottom, client.Width, dy);
						bounds.Deflate (child.DockMargins);
						child.SetBounds (bounds);
						client.Bottom += dy;
						break;
					
					case DockStyle.Left:
						bounds = new Drawing.Rectangle (client.Left, client.Bottom, dx, client.Height);
						bounds.Deflate (child.DockMargins);
						child.SetBounds (bounds);
						client.Left += dx;
						break;
					
					case DockStyle.Right:
						bounds = new Drawing.Rectangle (client.Right - dx, client.Bottom, dx, client.Height);
						bounds.Deflate (child.DockMargins);
						child.SetBounds (bounds);
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
							double min_dx = child.MinSize.Width;
							double new_dx = fill_dx / n;
							
							if (new_dx < min_dx)
							{
								push_dx += min_dx - new_dx;
								new_dx   = min_dx;
							}
							
							bounds = new Drawing.Rectangle (client.Left, client.Bottom, new_dx, client.Height);
							bounds.Deflate (child.DockMargins);
							
							child.SetBounds (bounds);
							client.Left += new_dx;
						}
						break;
					
					case ContainerLayoutMode.VerticalFlow:
						foreach (Visual child in fill_queue)
						{
							double min_dy = child.MinSize.Height;
							double new_dy = fill_dy / n;
							
							if (new_dy < min_dy)
							{
								push_dy += min_dy - new_dy;
								new_dy   = min_dy;
							}
							
							bounds = new Drawing.Rectangle (client.Left, client.Top - new_dy, client.Width, new_dy);
							bounds.Deflate (child.DockMargins);
							
							child.SetBounds (bounds);
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
		
	}
}
