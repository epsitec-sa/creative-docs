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
			this.internal_state |= InternalState.AutoFocus;
			this.internal_state |= InternalState.Focusable;
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
			
			Color color_text = this.IsFocused ? Color.FromRGB (0.0, 0.0, 0.5) : Color.FromRGB (0.0, 0.0, 0.0);
			Color color_back = this.IsFocused ? Color.FromRGB (0.8, 0.8, 1.0) : Color.FromRGB (1.0, 1.0, 1.0);
			
			graphics.Solid.Color = color_back;
			graphics.Rasterizer.AddSurface (path);
			graphics.RenderSolid ();
			
			double x = 10;
			double y = dy * 0.25;
			
			Font   font = Font.GetFont ("Tahoma", "Regular");
			double size = dy * 0.8;
			
			graphics.Solid.Color = color_text;
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
			switch (message.Type)
			{
				case MessageType.MouseEnter:
					this.highlight = true;
					this.Invalidate ();
					break;
				
				case MessageType.MouseDown:
					break;
				
				case MessageType.MouseUp:
					break;
				
				case MessageType.MouseLeave:
					this.Invalidate ();
					this.highlight = false;
					break;
				
				case MessageType.KeyPress:
					System.Diagnostics.Debug.WriteLine ("Key pressed: " + message.KeyChar + " in " + this.Name);
					
					if (message.IsAltPressed)
					{
						return;
					}
					if ((message.IsCtrlPressed == false) &&
						(message.KeyCode == 27))
					{
						this.SetFocus (false);
					}
					
					break;
			}
			
			message.Consumer = this;
		}
		
		protected override bool ProcessShortcut(Shortcut shortcut)
		{
			System.Diagnostics.Debug.WriteLine ("Shortcut pressed: " + shortcut.ToString () + " in " + this.Name);
			
			return base.ProcessShortcut (shortcut);
		}
		
		
		
		protected bool					highlight;
	}
}
