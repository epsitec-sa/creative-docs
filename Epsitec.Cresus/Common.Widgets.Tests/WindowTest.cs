using NUnit.Framework;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	[TestFixture]
	public class WindowTest
	{
		static WindowTest()
		{
		}
		
		
		[Test] public void CheckFrameCreation()
		{
			Window window = new Window ();
			
			window.Root.Clicked += new MessageEventHandler(Root_Clicked);
			window.Root.DoubleClicked += new MessageEventHandler(Root_DoubleClicked);
			window.Root.Text = "Hel<m>l</m>o";
			window.Root.ShortcutPressed += new EventHandler(Root_ShortcutPressed);
			
			window.Text = "CheckFrameCreation";
			window.Show ();
		}

		[Test] public void CheckDragSource()
		{
			Window window = new Window ();
			
			DragSource  source1 = new DragSource ();
			DragSource  source2 = new DragSource ();
			DragSource  source3 = new DragSource ();
			DragSource  source4 = new DragSource ();
			DragSource  source5 = new DragSource ();
			DragSource  source6 = new DragSource ();
			DragSource  source7 = new DragSource ();
			CheckButton button1 = new CheckButton ();
			RadioButton button2 = new RadioButton ();
			Button      button3 = new Button ();
			TextField   textfld = new TextField ();
			GroupBox    groupbx = new GroupBox ();
			StaticText  textlbl = new StaticText ();
			TextFieldUpDown textfud = new TextFieldUpDown ();
			
			button1.Text = "CheckBox";
			button1.Size = new Drawing.Size (80, button1.DefaultHeight);
			
			button2.Text = "RadioButton";
			button2.Size = new Drawing.Size (80, button2.DefaultHeight);
			
			button3.Text = "Button";
			button3.Size = new Drawing.Size (80, button3.DefaultHeight);
			
			textfld.Text = "TextField";
			textfld.Size = new Drawing.Size (80, textfld.DefaultHeight);
			
			groupbx.Text = "GroupBox";
			groupbx.Size = new Drawing.Size (120, 80);
			
			textlbl.Text = "StaticText";
			textlbl.Size = new Drawing.Size (80, textlbl.DefaultHeight);
			
			textfud.Text = "TextFieldUpDown";
			textfud.Size = new Drawing.Size (80, textfud.DefaultHeight);
			
			source1.Bounds = new Drawing.Rectangle (10, 10, button1.Width, button1.Height);
			source1.Parent = window.Root;
			source1.Widget = button1;
			
			source2.Bounds = new Drawing.Rectangle (10, 40, button2.Width, button2.Height);
			source2.Parent = window.Root;
			source2.Widget = button2;
			
			source3.Bounds = new Drawing.Rectangle (10, 70, button3.Width, button3.Height);
			source3.Parent = window.Root;
			source3.Widget = button3;
			
			source4.Bounds = new Drawing.Rectangle (10, 100, textfld.Width, textfld.Height);
			source4.Parent = window.Root;
			source4.Widget = textfld;
			
			source5.Bounds = new Drawing.Rectangle (100, 10, groupbx.Width, groupbx.Height);
			source5.Parent = window.Root;
			source5.Widget = groupbx;
			
			source6.Bounds = new Drawing.Rectangle (10, 130, textlbl.Width, textlbl.Height);
			source6.Parent = window.Root;
			source6.Widget = textlbl;
			
			source7.Bounds = new Drawing.Rectangle (10, 160, textfud.Width, textfud.Height);
			source7.Parent = window.Root;
			source7.Widget = textfud;
			
			window.Root.IsEditionDisabled = true;
			
			window.Text = "CheckDragSource";
			window.Show ();
		}

		[Test] public void CheckBounds()
		{
			Window window = new Window ();
			
			System.Console.Out.WriteLine ("(1) Common.Widgets says Window is at " + window.WindowBounds.ToString ());
			System.Console.Out.WriteLine ("    Windows.Forms says Window is at " + window.PlatformBounds.ToString ());
			
			window.WindowLocation = new Point (50, 100);
			
			Assertion.AssertEquals ( 50.0, window.WindowLocation.X);
			Assertion.AssertEquals (100.0, window.WindowLocation.Y);
			
			System.Console.Out.WriteLine ("(2) Common.Widgets says Window is at " + window.WindowBounds.ToString ());
			System.Console.Out.WriteLine ("    Windows.Forms says Window is at " + window.PlatformBounds.ToString ());
			
			window.PlatformLocation = new Point (0, 0);
			
			System.Console.Out.WriteLine ("(3) Common.Widgets says Window is at " + window.WindowBounds.ToString ());
			System.Console.Out.WriteLine ("    Windows.Forms says Window is at " + window.PlatformBounds.ToString ());
			
			Assertion.AssertEquals (ScreenInfo.GlobalArea.Top, window.WindowBounds.Top);
		}
		
		[Test] public void CheckMapWindowAndScreen()
		{
			Window window = new Window ();
			window.WindowLocation = new Point (10, 40);
			
			Point pt0 = new Point (0, 0);
			Point pt1 = window.MapWindowToScreen (pt0);
			Point pt2 = window.MapScreenToWindow (pt1);
			
			System.Console.Out.WriteLine ("Position in window is {0}", pt0);
			System.Console.Out.WriteLine ("Position in screen is {0}", pt1);
			
			Assertion.AssertEquals (0.0, pt2.X);
			Assertion.AssertEquals (0.0, pt2.Y);
			
			window.Show ();
		}
		
		[Test] public void CheckMakeFramelessWindow()
		{
			ScreenInfo info = ScreenInfo.Find (new Point (10, 10));
			
			double oy = info.WorkingArea.Bottom;
			double ox = info.WorkingArea.Left;
			
			Window window = new Window ();
			window.MakeFramelessWindow ();
			window.WindowActivated += new EventHandler(window_Activated);
			window.WindowDeactivated += new EventHandler(window_Deactivated);
			window.WindowBounds = new Rectangle (ox+10, oy+30, 50, 10);
			window.Root.BackColor = Color.FromRGB (1, 0, 0);
			window.Show ();
		}

		[Test] public void CheckWindowParentChildRelationship()
		{
			Window w1 = new Window ();
			Window w2 = new Window ();
			Window w3 = new Window ();
			
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
			
			Window.MessageFilter += new MessageHandler (Window_MessageFilter);
		}

		[Test] public void CheckLayeredWindows()
		{
			double zoom = 2.0;
			
			Window window = new Window ();
			window.Root.BackColor = Color.Transparent;
			window.MakeFramelessWindow ();
			window.MakeLayeredWindow ();
			window.Alpha = 0.5;
			window.WindowBounds = new Rectangle (ScreenInfo.GlobalArea.Left+50, 200, 100*zoom, 200*zoom);
			
			Widget back = new Widget ();
			back.Dock = DockStyle.Fill;
			back.Parent = window.Root;
			back.SetClientZoom (zoom);
			
			CheckButton button = new CheckButton ();
			VScroller scroller = new VScroller ();
			
			button.Location = new Point (10, 10);
			button.Size     = new Size (60, 24);
			button.Text     = "Test";
			button.Parent   = back;
			button.Clicked += new MessageEventHandler(button_Clicked);
			
			scroller.Location = new Point (80, 10);
			scroller.Size = new Size(17, 180);
			scroller.Range = 1.0;
			scroller.VisibleRangeRatio = 0.1;
			scroller.Value = 0.0;
			scroller.SmallChange = 0.01;
			scroller.LargeChange = 0.10;
			scroller.Parent = back;
			scroller.ValueChanged += new EventHandler(scroller_ValueChanged);
			
			window.Show ();
			window.Root.Invalidate ();
		}

		[Test] public void CheckAppActivation()
		{
			Window.ApplicationActivated   += new EventHandler(Window_ApplicationActivated);
			Window.ApplicationDeactivated += new EventHandler(Window_ApplicationDeactivated);
		}

		[Test] public void CheckWindowList()
		{
			Window[] windows = Epsitec.Common.Widgets.Platform.WindowList.GetVisibleWindows ();
			for (int i = 0; i < windows.Length; i++)
			{
				System.Console.Out.WriteLine ("{0}: {1}", i, windows[i].Text);
			}
		}
		
		[Test] public void CheckMouseCursor()
		{
			Drawing.Image image = Drawing.Bitmap.FromFile (@"..\..\cursor.png");
			
			Window window = new Window ();
			window.ClientSize = new Size (200, 200);
			window.MouseCursor = MouseCursor.FromImage (image, 4, 4);
			window.Text = "CheckMouseCursor";
			window.Show ();
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

		private void window_Activated(object sender)
		{
			System.Diagnostics.Debug.WriteLine ("Activated window");
		}

		private void window_Deactivated(object sender)
		{
			System.Diagnostics.Debug.WriteLine ("Deactivated window");
		}

		private void Window_MessageFilter(object sender, Message message)
		{
			Window window = sender as Window;
			System.Diagnostics.Debug.WriteLine (string.Format ("Window {0}: message={1}", window.Name, message.ToString ()));
		}

		private void scroller_ValueChanged(object sender)
		{
			VScroller scroller = sender as VScroller;
			Window window = scroller.Window;
			window.Alpha = scroller.Value / 2 + 0.5;
		}

		private void button_Clicked(object sender, MessageEventArgs e)
		{
			Widget button = sender as Widget;
			button.Location = new Point (button.Location.X, button.Location.Y + 5);
		}

		private void Window_ApplicationActivated(object sender)
		{
			System.Diagnostics.Debug.Assert (Window.IsApplicationActive == true);
			System.Diagnostics.Debug.WriteLine ("Application activated");
		}
		
		private void Window_ApplicationDeactivated(object sender)
		{
			System.Diagnostics.Debug.Assert (Window.IsApplicationActive == false);
			System.Diagnostics.Debug.WriteLine ("Application deactivated");
		}
	}
}
