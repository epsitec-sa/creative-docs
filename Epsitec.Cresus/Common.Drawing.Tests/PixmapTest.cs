using NUnit.Framework;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Drawing
{
	[TestFixture] public class PixmapTest
	{
		[Test] public void CheckPixmapCopy()
		{
			Graphics cache = new Agg.Graphics ();
			Pixmap[] stack = new Pixmap[3];
			
			cache.SetPixmapSize (400, 400);
			
			cache.SolidRenderer.ClearARGB (1, 1, 1, 1);
			
			cache.Color = Color.FromARGB (0.5, 1, 0, 0);
			cache.PaintSurface (Path.FromCircle (40, 40, 20));
			cache.Color = Color.FromARGB (0.5, 0, 1, 0);
			cache.PaintSurface (Path.FromCircle (52, 32, 16));
			cache.Color = Color.FromARGB (0.5, 0, 0, 1);
			cache.PaintSurface (Path.FromCircle (52, 48, 12));
			
			stack[0] = new Pixmap ();
			stack[0].Size = new System.Drawing.Size (400, 400);
			stack[0].Copy (0, 0, cache.Pixmap, 0, 0, 400, 400);
			
			cache.SolidRenderer.ClearARGB (0, 0, 0, 0);
			cache.Color = Color.FromARGB (0.5, 1, 0, 0);
			cache.PaintSurface (Path.FromCircle (200, 200, 100));
			
			Color c1 = cache.Pixmap.GetPixel (50, 50);
			Color c2 = cache.Pixmap.GetPixel (200, 200);
			
			System.Console.Out.WriteLine ("Background: {0}", c1);
			System.Console.Out.WriteLine ("Red 50%:    {0}", c2);
			
			stack[1] = new Pixmap ();
			stack[1].Size = new System.Drawing.Size (400, 400);
			stack[1].Copy (0, 0, cache.Pixmap, 0, 0, 400, 400);
			
			cache.SolidRenderer.ClearARGB (0, 0, 0, 0);
			cache.Color = Color.FromARGB (0.5, 0, 1, 0);
			cache.PaintSurface (Path.FromCircle (260, 160, 80));
			cache.Color = Color.FromARGB (0.5, 0, 0, 1);
			cache.PaintSurface (Path.FromCircle (260, 240, 60));
			
			c1 = cache.Pixmap.GetPixel (50, 50);
			c2 = cache.Pixmap.GetPixel (260, 160);
			
			System.Console.Out.WriteLine ("Background: {0}", c1);
			System.Console.Out.WriteLine ("Green 50%:  {0}", c2);
			
			stack[2] = new Pixmap ();
			stack[2].Size = new System.Drawing.Size (400, 400);
			stack[2].Copy (0, 0, cache.Pixmap, 0, 0, 400, 400);
			
			Widgets.Window window = new Widgets.Window ();
			window.ClientSize = new Size (400, 400);
			Widgets.Widget widget = new TestWidget (stack);
			
			widget.Dock   = Widgets.DockStyle.Fill;
			widget.Parent = window.Root;
			
			window.Show ();
		}
		
		protected class TestWidget : Widgets.Widget
		{
			public TestWidget(Pixmap[] stack)
			{
				this.stack = stack;
			}
			
			protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clip_rect)
			{
				int dx = (int) (this.Client.Width);
				int dy = (int) (this.Client.Height);
				
				for (int i = 0; i < this.stack.Length; i++)
				{
					if (i == 0)
					{
						graphics.Pixmap.Copy (0, 0, this.stack[i], 0, 0, dx, dy);
					}
					else
					{
						graphics.Pixmap.Compose (0, 0, this.stack[i], 0, 0, dx, dy);
					}
				}
			}
			
			Pixmap[]				stack;
		}
	}
}
