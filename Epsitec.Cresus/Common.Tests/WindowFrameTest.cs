using NUnit.Framework;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests
{
	[TestFixture]
	public class WindowFrameTest
	{
		static WindowFrameTest()
		{
		}
		
		[Test] public void CheckFrameCreation()
		{
			WindowFrame window = new WindowFrame ();
			
			window.Root.Clicked += new MessageEventHandler(Root_Clicked);
			window.Root.DoubleClicked += new MessageEventHandler(Root_DoubleClicked);
			window.Root.Text = "Hel<m>l</m>o";
			window.Root.ShortcutPressed += new EventHandler(Root_ShortcutPressed);
			
			window.Text = "CheckFrameCreation";
			window.Show ();
		}

		[Test] public void CheckBounds()
		{
			WindowFrame window = new WindowFrame ();
			
			System.Console.Out.WriteLine ("(1) Common.Widgets says WindowFrame is at " + window.WindowBounds.ToString ());
			System.Console.Out.WriteLine ("    Windows.Forms says WindowFrame is at " + window.Bounds.ToString ());
			
			window.WindowLocation = new Point (50, 100);
			
			Assertion.AssertEquals ( 50.0, window.WindowLocation.X);
			Assertion.AssertEquals (100.0, window.WindowLocation.Y);
			
			System.Console.Out.WriteLine ("(2) Common.Widgets says WindowFrame is at " + window.WindowBounds.ToString ());
			System.Console.Out.WriteLine ("    Windows.Forms says WindowFrame is at " + window.Bounds.ToString ());
			
			window.Location = new System.Drawing.Point (0, 0);
			
			System.Console.Out.WriteLine ("(3) Common.Widgets says WindowFrame is at " + window.WindowBounds.ToString ());
			System.Console.Out.WriteLine ("    Windows.Forms says WindowFrame is at " + window.Bounds.ToString ());
			
			Assertion.AssertEquals (ScreenInfo.GlobalArea.Top, window.WindowBounds.Top);
		}
		
		[Test] public void CheckMapWindowAndScreen()
		{
			WindowFrame window = new WindowFrame ();
			window.WindowLocation = new Point (10, 40);
			
			Point pt0 = new Point (0, 0);
			Point pt1 = window.MapWindowToScreen (pt0);
			Point pt2 = window.MapScreenToWindow (pt1);
			
			System.Console.Out.WriteLine ("Position in window is {0}", pt0);
			System.Console.Out.WriteLine ("Position in screen is {0}", pt1);
			
			Assertion.AssertEquals (0.0, pt2.X);
			Assertion.AssertEquals (0.0, pt2.Y);
			
			System.Console.Out.WriteLine ("Start {0}", window.StartPosition.ToString ());
			window.Show ();
		}
		
		[Test] public void CheckMakeFramelessWindow()
		{
			ScreenInfo info = ScreenInfo.Find (new Point (10, 10));
			
			double oy = info.WorkingArea.Bottom;
			double ox = info.WorkingArea.Left;
			
			WindowFrame window = new WindowFrame ();
			window.MakeFramelessWindow ();
			window.Paint += new System.Windows.Forms.PaintEventHandler(window_Paint);
			window.WindowActivated += new System.EventHandler(window_Activated);
			window.WindowDeactivated += new System.EventHandler(window_Deactivated);
			window.WindowBounds = new Rectangle (ox+10, oy+10, 50, 25);
			window.Root.BackColor = Color.FromRGB (1, 0, 0);
			window.Show ();
		}

		[Test] public void CheckWindowParentChildRelationship()
		{
			WindowFrame w1 = new WindowFrame ();
			WindowFrame w2 = new WindowFrame ();
			WindowFrame w3 = new WindowFrame ();
			
			w1.Name = "W1";
			w1.Text = "CheckWindowParentChildRelationship: W1";
			w1.WindowSize = new Size (400, 150);
			w1.WindowLocation = new Point (100, 600);
			w1.Show ();
			
			w2.Owner = w1;
			w2.Name = "W2";
			w2.Text = "CheckWindowParentChildRelationship: W2";
			w2.WindowSize = new Size (400, 150);
			w2.WindowLocation = new Point (150, 550);
			w2.Show ();
			
			w3.Owner = w2;
			w3.Name = "W3";
			w3.Text = "CheckWindowParentChildRelationship: W3";
			w3.WindowSize = new Size (400, 150);
			w3.WindowLocation = new Point (200, 500);
			w3.Show ();
			
			WindowFrame.MessageFilter += new MessageHandler (WindowFrame_MessageFilter);
		}

		[Test] public void CheckLayeredWindows()
		{
			double zoom = 2.0;
			
			WindowFrame window = new WindowFrame ();
			window.Root.BackColor = new Color (0, 0, 0, 0);
			window.MakeFramelessWindow ();
			window.IsLayered = true;
			window.Alpha = 0.5;
			window.WindowBounds = new Rectangle (ScreenInfo.GlobalArea.Left+50, 200, 100*zoom, 200*zoom);
			
			Widget back = new Widget ();
			back.Dock = DockStyle.Fill;
			back.Parent = window.Root;
			back.SetClientZoom (zoom);
			
			Button button = new Button ();
			Scroller scroller = new Scroller ();
			
			button.Location = new Point (10, 10);
			button.Size     = new Size (60, 24);
			button.Text     = "Test";
			button.Parent   = back;
			button.Clicked += new MessageEventHandler(button_Clicked);
			
			scroller.Location = new Point (80, 10);
			scroller.Size = new Size(15, 180);
			scroller.Range = 1.0;
			scroller.Display = 0.1;
			scroller.Position = 0.0;
			scroller.ButtonStep = 0.01;
			scroller.PageStep = 0.10;
			scroller.Parent = back;
			scroller.Moved += new EventHandler(scroller_Moved);
			
			window.Show ();
			window.Root.Invalidate ();
		}

		[Test] public void CheckAppActivation()
		{
			WindowFrame.ApplicationActivated   += new EventHandler(WindowFrame_ApplicationActivated);
			WindowFrame.ApplicationDeactivated += new EventHandler(WindowFrame_ApplicationDeactivated);
		}

		private void Root_Clicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Root Clicked");
		}

		private void Root_DoubleClicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Root Double Clicked");
		}

		private void Root_ShortcutPressed(object sender)
		{
			System.Diagnostics.Debug.WriteLine ("Shortcut key pressed");
		}

		private void window_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			System.Windows.Forms.Form window = sender as System.Windows.Forms.Form;
			e.Graphics.FillRectangle (System.Drawing.Brushes.Red, window.ClientRectangle);
		}

		private void window_Activated(object sender, System.EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Activated window");
		}

		private void window_Deactivated(object sender, System.EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Deactivated window");
		}

		private void WindowFrame_MessageFilter(object sender, Message message)
		{
			WindowFrame window = sender as WindowFrame;
			System.Diagnostics.Debug.WriteLine (string.Format ("Window {0}: message={1}", window.Name, message.ToString ()));
		}

		private void scroller_Moved(object sender)
		{
			Scroller scroller = sender as Scroller;
			WindowFrame window = scroller.WindowFrame;
			window.Alpha = scroller.Position / 2 + 0.5;
		}

		private void button_Clicked(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;
			button.Location = new Point (button.Location.X, button.Location.Y + 5);
		}

		private void WindowFrame_ApplicationActivated(object sender)
		{
			System.Diagnostics.Debug.Assert (WindowFrame.IsApplicationActive == true);
			System.Diagnostics.Debug.WriteLine ("Application activated");
		}
		
		private void WindowFrame_ApplicationDeactivated(object sender)
		{
			System.Diagnostics.Debug.Assert (WindowFrame.IsApplicationActive == false);
			System.Diagnostics.Debug.WriteLine ("Application deactivated");
		}
	}
}
