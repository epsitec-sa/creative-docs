using System;
using NUnit.Framework;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests
{
	[TestFixture]
	public class CustomWidgetTest
	{
		static CustomWidgetTest()
		{
			try { System.Diagnostics.Debug.WriteLine (""); } 
			catch { }
		}
		
		[Test] public void CheckSimpleWindow()
		{
			WindowFrame window = new WindowFrame ();
			window.Text = "Simple demo";
			
			CustomWidget a = new CustomWidget ();
			CustomWidget b = new CustomWidget ();
			
			window.Root.Children.Add (a);
			window.Root.Children.Add (b);
			
			double x = window.ClientSize.Width - 90;

			a.Name = "A"; a.Location = new Point (x, 40); a.Size = new Size (80, 20); a.Text = "OK";	 a.Anchor = Widget.AnchorStyles.Bottom | Widget.AnchorStyles.Right;
			b.Name = "B"; b.Location = new Point (x, 10); b.Size = new Size (80, 20); b.Text = "Cancel"; b.Anchor = Widget.AnchorStyles.Bottom | Widget.AnchorStyles.Right;
			
			window.Show ();
		}
	}
	
	public class CustomWidget : Widget
	{
		public CustomWidget()
		{
		}
		
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clip_rect)
		{
			Path path = new Path ();
			
			double dx = this.Client.Width;
			double dy = this.Client.Height;
			
			if (this.highlight)
			{
				graphics.ScaleTransform (1.03, 1.03, dx/2, dy/2);
			}
			
			path.MoveTo (5, 0);
			path.LineTo (dx-5, 0);
			path.CurveTo (dx, 0, dx, 5);
			path.LineTo (dx, dy-5);
			path.CurveTo (dx, dy, dx-5, dy);
			path.LineTo (5, dy);
			path.CurveTo (0, dy, 0, dy-5);
			path.LineTo (0, 5);
			path.CurveTo (0, 0, 5, 0);
			path.Close ();
			
			graphics.Solid.Color = Color.FromName ("White");
			graphics.Rasterizer.AddSurface (path);
			graphics.RenderSolid ();
			
			double x = 10;
			double y = dy * 0.25;
			
			Font   font = Font.GetFont ("Tahoma", "Regular");
			double size = dy * 0.8;
			
			graphics.Solid.Color = Color.FromName ("Black");
			graphics.AddText (x, y, this.Text, font, size);
			graphics.Rasterizer.AddOutline (path, 0.6);
			graphics.RenderSolid ();
		}
		
		public override Rectangle GetPaintBounds()
		{
			double growth = System.Math.Max (this.Client.Width, this.Client.Height) * 0.03;
			double margin = 0.3 + (this.highlight ? growth : 0);
			
			return Rectangle.Inflate (base.GetPaintBounds (), margin, margin);
		}

		
		protected override void ProcessMessage(Message message, Point pos)
		{
			if (message.Type == MessageType.MouseEnter)
			{
				this.highlight = true;
				this.Invalidate ();
			}
			else if (message.Type == MessageType.MouseLeave)
			{
				this.Invalidate ();
				this.highlight = false;
			}
		}
		
		protected bool					highlight;
	}
}
