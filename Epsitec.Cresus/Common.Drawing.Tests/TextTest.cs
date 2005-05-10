using NUnit.Framework;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Drawing
{
	[TestFixture]
	public class TextTest
	{
		[Test] public void CheckPainting()
		{
			Window  window  = new Window ();
			Painter painter = new Painter ();
			
			window.ClientSize     = new Size (500, 400);
			window.WindowLocation = ScreenInfo.Find (new Point (10, 10)).WorkingArea.TopLeft + new Point (100, -100 - window.WindowSize.Height);
			window.Text           = "TextTest/CheckPainting";
			
			painter.Parent = window.Root;
			painter.Dock   = DockStyle.Fill;
			
			window.Show ();
		}
		
		private class Painter : Widget
		{
			public Painter()
			{
			}
			
			protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clip_rect)
			{
				graphics.AddFilledRectangle (0, 0, this.Width, this.Height);
				graphics.RenderSolid (Color.FromName ("White"));
			}
		}
	}
}

