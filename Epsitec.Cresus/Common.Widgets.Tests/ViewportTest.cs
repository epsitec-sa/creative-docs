using NUnit.Framework;

namespace Epsitec.Common.Widgets
{
	[TestFixture] public class ViewportTest
	{
		[Test]
		public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}

		[Test]
		public void CheckScrollableViewport()
		{
			Window window = new Window ();

			window.Text = "ViewportTest.CheckScrollableViewport";
			window.ClientSize = new Drawing.Size (400, 300);
			
			Scrollable surface = new Scrollable ();
			
			surface.SetParent (window.Root);
			surface.SetManualBounds(window.Root.Client.Bounds);
			surface.Dock   = DockStyle.Fill;
			
			surface.Viewport.SurfaceSize = new Drawing.Size (400, 300);
			surface.Viewport.MinSize     = new Drawing.Size (200, 150);
			
			Button b1 = new Button ("Button 1");
			Button b2 = new Button ("Button 2");
			
			b1.SetParent (surface.Viewport);
			b2.SetParent (surface.Viewport);
			b1.Margins = new Epsitec.Common.Drawing.Margins (10, 0, 0, 10);
			b2.Margins = new Epsitec.Common.Drawing.Margins (10 + b1.PreferredWidth + 10, 0, 0, 10);
			b1.Anchor = AnchorStyles.BottomLeft;
			b2.Anchor = AnchorStyles.BottomLeft;
			
			StaticText  text;
			RadioButton radio;
			
			surface.Viewport.Padding = new Drawing.Margins (8, 8, 16, 16);
			
			text  = new StaticText (surface.Viewport);
			text.Dock = DockStyle.Top;
			text.Text = "Horizontal scroller mode :";
			text.Margins = new Drawing.Margins (0, 0, 0, 4);
			
			radio = new RadioButton (surface.Viewport);
			radio.Dock = DockStyle.Top;
			radio.Text = "Auto";
			radio.Group = "h";
			radio.Index = (int) ScrollableScrollerMode.Auto;
			radio.ActiveState = ActiveState.Yes;
			radio.ActiveStateChanged += this.HandleRadioActiveStateChanged;
			
			radio = new RadioButton (surface.Viewport);
			radio.Dock = DockStyle.Top;
			radio.Text = "Hide";
			radio.Group = "h";
			radio.Index = (int) ScrollableScrollerMode.HideAlways;
			radio.ActiveStateChanged += this.HandleRadioActiveStateChanged;
			
			radio = new RadioButton (surface.Viewport);
			radio.Dock = DockStyle.Top;
			radio.Text = "Show";
			radio.Group = "h";
			radio.Index = (int) ScrollableScrollerMode.ShowAlways;
			radio.ActiveStateChanged += this.HandleRadioActiveStateChanged;
			
			text  = new StaticText (surface.Viewport);
			text.Dock = DockStyle.Top;
			text.Text = "Vertical scroller mode :";
			text.Margins = new Drawing.Margins (0, 0, 16, 4);
			
			radio = new RadioButton (surface.Viewport);
			radio.Dock = DockStyle.Top;
			radio.Text = "Auto";
			radio.Group = "v";
			radio.Index = (int) ScrollableScrollerMode.Auto;
			radio.ActiveState = ActiveState.Yes;
			radio.ActiveStateChanged += this.HandleRadioActiveStateChanged;
			
			radio = new RadioButton (surface.Viewport);
			radio.Dock = DockStyle.Top;
			radio.Text = "Hide";
			radio.Group = "v";
			radio.Index = (int) ScrollableScrollerMode.HideAlways;
			radio.ActiveStateChanged += this.HandleRadioActiveStateChanged;
			
			radio = new RadioButton (surface.Viewport);
			radio.Dock = DockStyle.Top;
			radio.Text = "Show";
			radio.Group = "v";
			radio.Index = (int) ScrollableScrollerMode.ShowAlways;
			radio.ActiveStateChanged += this.HandleRadioActiveStateChanged;
			
			Assert.IsTrue (b1.Parent == surface.Viewport);
			Assert.IsTrue (b2.Parent == surface.Viewport);
			Assert.IsTrue (surface.Viewport.Parent == surface);
			Assert.IsTrue (surface.Viewport.Children.Count == 2+4+4);

			System.Console.Out.WriteLine ("Viewport SurfaceSize = {0}", surface.Viewport.SurfaceSize);
			System.Console.Out.WriteLine ("Viewport Bounds = {0}", surface.Viewport.ActualBounds);
			System.Console.Out.WriteLine ("Button Bounds = {0}, {1}", b1.ActualBounds, b2.ActualBounds);
			System.Console.Out.WriteLine ("Button Bounds (root relative) = {0}, {1}", b1.MapClientToRoot (b1.Client.Bounds), b2.MapClientToRoot (b2.Client.Bounds));
			
			window.Show ();
			window.SynchronousRepaint ();

			System.Console.Out.WriteLine ("Viewport SurfaceSize = {0}", surface.Viewport.SurfaceSize);
			System.Console.Out.WriteLine ("Viewport Bounds = {0}", surface.Viewport.ActualBounds);
			System.Console.Out.WriteLine ("Button Bounds = {0}, {1}", b1.ActualBounds, b2.ActualBounds);
			System.Console.Out.WriteLine ("Button Bounds (root relative) = {0}, {1}", b1.MapClientToRoot (b1.Client.Bounds), b2.MapClientToRoot (b2.Client.Bounds));

			surface.Viewport.Clicked += this.HandleViewportClicked;

			Window.RunInTestEnvironment (window);
		}

		private void HandleViewportClicked(object sender, MessageEventArgs e)
		{
			Viewport   viewport   = sender as Viewport;
			Scrollable scrollable = viewport.Parent as Scrollable;

			scrollable.ViewportOffsetY += (e.Message.Button == MouseButtons.Left) ? 10 : -10;
		}

		private void HandleRadioActiveStateChanged(object sender)
		{
			RadioButton radio   = sender as RadioButton;
			Viewport    viewport= radio.Parent as Viewport;
			Scrollable  surface = viewport.Parent as Scrollable;
			
			if (radio.ActiveState == ActiveState.Yes)
			{
				if (radio.Group == "h")
				{
					surface.HorizontalScrollerMode = (ScrollableScrollerMode) radio.Index;
				}
				if (radio.Group == "v")
				{
					surface.VerticalScrollerMode = (ScrollableScrollerMode) radio.Index;
				}
			}
		}
	}
}
