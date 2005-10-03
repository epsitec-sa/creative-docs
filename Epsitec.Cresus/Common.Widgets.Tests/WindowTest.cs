using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

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

		[Test] public void CheckBounds()
		{
			Window window = new Window ();
			
			System.Console.Out.WriteLine ("(1) Common.Widgets says Window is at " + window.WindowBounds.ToString ());
			System.Console.Out.WriteLine ("    Windows.Forms says Window is at " + window.PlatformBounds.ToString ());
			
			window.WindowLocation = new Point (50, 100);
			
			Assert.AreEqual ( 50.0, window.WindowLocation.X);
			Assert.AreEqual (100.0, window.WindowLocation.Y);
			
			System.Console.Out.WriteLine ("(2) Common.Widgets says Window is at " + window.WindowBounds.ToString ());
			System.Console.Out.WriteLine ("    Windows.Forms says Window is at " + window.PlatformBounds.ToString ());
			
			window.PlatformLocation = new Point (0, 0);
			
			System.Console.Out.WriteLine ("(3) Common.Widgets says Window is at " + window.WindowBounds.ToString ());
			System.Console.Out.WriteLine ("    Windows.Forms says Window is at " + window.PlatformBounds.ToString ());
			
			Assert.AreEqual (ScreenInfo.GlobalArea.Top, window.WindowBounds.Top);
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
			
			Assert.AreEqual (0.0, pt2.X);
			Assert.AreEqual (0.0, pt2.Y);
			
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

		[Test] public void CheckMakeToolWindow()
		{
			Window owner  = Window.FindFromName ("CheckAdornerWidgets");
			Window window = new Window ();
			window.MakeToolWindow ();
			window.Name = "ToolWindow";
			window.Owner = owner;
			window.ClientSize = new Size (300, 50);
			window.Root.BackColor = Color.FromRGB (1, 1, 1);
			
			Button button = new Button (window.Root);
			button.Bounds = new Drawing.Rectangle (250, 10, 40, 20);
			button.Text   = "?";
			
			TextField text = new TextField (window.Root);
			text.Bounds = new Drawing.Rectangle (150, 10, 95, 20);
			
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
			back.SetParent (window.Root);
			back.SetClientZoom (zoom);
			
			CheckButton button = new CheckButton ();
			VScroller scroller = new VScroller ();
			
			button.Location = new Point (10, 10);
			button.Size     = new Size (60, 24);
			button.Text     = "Test";
			button.SetParent (back);
			button.Clicked += new MessageEventHandler(button_Clicked);
			
			scroller.Location = new Point (80, 10);
			scroller.Size = new Size(17, 180);
			scroller.MaxValue = 1.0M;
			scroller.VisibleRangeRatio = 0.1M;
			scroller.Value = 0.0M;
			scroller.SmallChange = 0.01M;
			scroller.LargeChange = 0.10M;
			scroller.SetParent (back);
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
			window.MouseCursor = MouseCursor.FromImage (image, 4, 28);
			window.Text = "CheckMouseCursor";
			window.Show ();
		}
		
		[Test] public void CheckTabNavigation()
		{
			Window window = new Window ();
			window.Text = "CheckTabNavigation";
			window.ClientSize = new Drawing.Size (450, 230);
			window.MakeFixedSizeWindow ();
			
			Assert.IsNotNull (window.CommandDispatcher);
			Assert.IsNotNull (window.Root.CommandDispatcher);
			
			CommandDispatcher dispatcher = window.CommandDispatcher;
			
			dispatcher.RegisterController (new MyController ());
			
			Assert.AreSame (dispatcher, window.Root.CommandDispatcher);
			
			CommandState command_open = new CommandState ("Open", dispatcher, KeyCode.ModifierControl | KeyCode.AlphaO);
			CommandState command_save = new CommandState ("Save", dispatcher, KeyCode.ModifierAlt | KeyCode.AlphaS);
			CommandState command_cut  = new CommandState ("ClipCut", dispatcher, KeyCode.ModifierControl | KeyCode.AlphaX);
			
			Button      button;
			GroupBox    group;
			Widget      widget;
			RadioButton radio;
			CheckButton check;
			
			button = new Button ("A");
			button.Bounds = new Drawing.Rectangle (10, 170, 40, 25);
			button.SetParent (window.Root);
			button.TabIndex = 1;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			button.Focus();
			button.ButtonStyle = ButtonStyle.DefaultAccept;
			
			button = new Button ("B");
			button.Bounds = new Drawing.Rectangle (10, 140, 40, 25);
			button.SetParent (window.Root);
			button.TabIndex = 2;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			button.SetEnabled (false);
			
			button = new Button ("C");
			button.Bounds = new Drawing.Rectangle (10, 110, 40, 25);
			button.SetParent (window.Root);
			button.TabIndex = 3;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			button = new Button ("D");
			button.Bounds = new Drawing.Rectangle (10, 80, 40, 25);
			button.SetParent (window.Root);
			button.TabIndex = 4;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			group = new GroupBox ();
			group.Bounds = new Drawing.Rectangle (60, 110, 110, 85);
			group.SetParent (window.Root);
			group.TabIndex = 10;
			group.Text = "Group 1";
			group.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			button = new Button ("A");
			button.Bounds = new Drawing.Rectangle (10, 40, 40, 25);
			button.SetParent (group);
			button.TabIndex = 1;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			button = new Button ("B");
			button.Bounds = new Drawing.Rectangle (10, 10, 40, 25);
			button.SetParent (group);
			button.TabIndex = 2;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			button = new Button ("C");
			button.Bounds = new Drawing.Rectangle (55, 40, 40, 25);
			button.SetParent (group);
			button.TabIndex = 3;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			button = new Button ("D");
			button.Bounds = new Drawing.Rectangle (55, 10, 40, 25);
			button.SetParent (group);
			button.TabIndex = 4;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			group = new GroupBox ();
			group.Bounds = new Drawing.Rectangle (180, 110, 110, 85);
			group.SetParent (window.Root);
			group.TabIndex = 11;
			group.Text = "Group 2";
			group.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren;
			
			button = new Button ("A");
			button.Bounds = new Drawing.Rectangle (10, 40, 40, 25);
			button.SetParent (group);
			button.TabIndex = 1;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			button = new Button ("B");
			button.Bounds = new Drawing.Rectangle (10, 10, 40, 25);
			button.SetParent (group);
			button.TabIndex = 2;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			button = new Button ("C");
			button.Bounds = new Drawing.Rectangle (55, 40, 40, 25);
			button.SetParent (group);
			button.TabIndex = 3;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			button = new Button ("D");
			button.Bounds = new Drawing.Rectangle (55, 10, 40, 25);
			button.SetParent (group);
			button.TabIndex = 4;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			group = new GroupBox ();
			group.Bounds = new Drawing.Rectangle (300, 110, 110, 85);
			group.SetParent (window.Root);
			group.TabIndex = 12;
			group.Text = "Group 3";
			group.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
			
			button = new Button ("A");
			button.Bounds = new Drawing.Rectangle (10, 40, 40, 25);
			button.SetParent (group);
			button.TabIndex = 1;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			button = new Button ("B");
			button.Bounds = new Drawing.Rectangle (10, 10, 40, 25);
			button.SetParent (group);
			button.TabIndex = 2;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			button = new Button ("C");
			button.Bounds = new Drawing.Rectangle (55, 40, 40, 25);
			button.SetParent (group);
			button.TabIndex = 3;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			button = new Button ("D");
			button.Bounds = new Drawing.Rectangle (55, 10, 40, 25);
			button.SetParent (group);
			button.TabIndex = 4;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			button = new Button ("E");
			button.Bounds = new Drawing.Rectangle (10, 50, 40, 25);
			button.SetParent (window.Root);
			button.TabIndex = 20;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			button = new Button ("F");
			button.Bounds = new Drawing.Rectangle (10, 20, 40, 25);
			button.SetParent (window.Root);
			button.TabIndex = 21;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			widget = new TextField ();
			widget.Bounds = new Drawing.Rectangle (60, 74, 100, 22);
			widget.SetParent (window.Root);
			widget.TabIndex = 30;
			widget.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			widget = new TextFieldUpDown ();
			widget.Bounds = new Drawing.Rectangle (165, 74, 40, 22);
			widget.SetParent (window.Root);
			widget.TabIndex = 31;
			widget.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			widget = new TextFieldUpDown ();
			widget.Bounds = new Drawing.Rectangle (210, 74, 40, 22);
			widget.SetParent (window.Root);
			widget.TabIndex = 32;
			widget.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			StaticText text = new StaticText ();
			
			text.Bounds    = new Drawing.Rectangle (60, 15, 420, 50);
			text.SetParent (window.Root);
			text.Alignment = Drawing.ContentAlignment.TopLeft;
			text.Text      = "<b>Group 1:</b> cannot be entered with TAB<br/>"
				/**/       + "<b>Group 2:</b> can be focused and entered with TAB<br/>"
				/**/       + "<b>Group 3:</b> cannot be focused, but can be entered with TAB<br/>";
			text.TextLayout.BreakMode = Drawing.TextBreakMode.Hyphenate;
			
			text = new StaticText ();
			text.Bounds = new Drawing.Rectangle (10, 200, 230, 25);
			text.Text   = "<font size=\"130%\">Press <b>TAB</b> to move the focus...</font>";
			text.SetParent (window.Root);
			
			radio = new RadioButton ();
			radio.Bounds = new Drawing.Rectangle (260, 75+7, 40, 20);
			radio.Text   = "A";
			radio.Group  = "Option1";
			radio.Index  = 0;
			radio.SetParent (window.Root);
			radio.TabIndex = 40;
			radio.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			radio.ActiveState = WidgetState.ActiveYes;
			
			radio = new RadioButton ();
			radio.Bounds = new Drawing.Rectangle (260, 61+7, 40, 20);
			radio.Text   = "B";
			radio.Group  = "Option1";
			radio.Index  = 1;
			radio.SetParent (window.Root);
			radio.TabIndex = 40;
			radio.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			radio = new RadioButton ();
			radio.Bounds = new Drawing.Rectangle (260, 47+7, 40, 20);
			radio.Text   = "C";
			radio.Group  = "Option1";
			radio.Index  = 2;
			radio.SetParent (window.Root);
			radio.TabIndex = 40;
			radio.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			radio = new RadioButton ();
			radio.Bounds = new Drawing.Rectangle (300, 75+7, 40, 20);
			radio.Text   = "D";
			radio.Group  = "Option2";
			radio.Index  = 0;
			radio.SetParent (window.Root);
			radio.TabIndex = 41;
			radio.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			radio.ActiveState = WidgetState.ActiveYes;
			
			radio = new RadioButton ();
			radio.Bounds = new Drawing.Rectangle (300, 61+7, 40, 20);
			radio.Text   = "E";
			radio.Group  = "Option2";
			radio.Index  = 1;
			radio.SetParent (window.Root);
			radio.TabIndex = 41;
			radio.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			radio = new RadioButton ();
			radio.Bounds = new Drawing.Rectangle (300, 47+7, 40, 20);
			radio.Text   = "F";
			radio.Group  = "Option2";
			radio.Index  = 2;
			radio.SetParent (window.Root);
			radio.TabIndex = 41;
			radio.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			check = new CheckButton ();
			check.Bounds = new Drawing.Rectangle (340, 75+7, 40, 20);
			check.Text   = "G";
			check.SetParent (window.Root);
			check.TabIndex = 50;
			check.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			check.ActiveState = WidgetState.ActiveYes;
			
			check = new CheckButton ();
			check.Bounds = new Drawing.Rectangle (340, 61+7, 40, 20);
			check.Text   = "H";
			check.SetParent (window.Root);
			check.TabIndex = 51;
			check.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			check = new CheckButton ();
			check.Bounds = new Drawing.Rectangle (340, 47+7, 40, 20);
			check.Text   = "I";
			check.SetParent (window.Root);
			check.TabIndex = 52;
			check.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			window.Show ();
		}
		
		
		private class MyController
		{
			[Command ("Open")]		public void CommandOpen()
			{
				System.Diagnostics.Debug.WriteLine ("Open executed.");
			}
			[Command ("Save")]		public void CommandSave()
			{
				System.Diagnostics.Debug.WriteLine ("Save executed.");
			}
			[Command ("ClipCut")]	public void CommandCut()
			{
				System.Diagnostics.Debug.WriteLine ("Clipboard Cut executed.");
			}
		}
		
		[Test] public void CheckMinSize()
		{
			Window window;
			
			window = new Window();
			window.ClientSize = new Size(300, 250);
			window.Text = "Informations";
			window.MakeSecondaryWindow();
			window.MakeButtonlessWindow ();
			window.Root.MinSize = new Size(200, 100);
			
			window.Show ();
		}
		
		[Test] public void CheckAlphaWindow()
		{
			Window     window = new Window ();
			StaticText text   = new StaticText ();
			Button     button = new Button ();
			
			window.Text = "Layered";
			window.Root.BackColor = Color.Transparent;
			window.MakeFramelessWindow ();
			window.MakeLayeredWindow ();
			window.Alpha = 0.75;
			window.WindowBounds = new Rectangle (ScreenInfo.GlobalArea.Left+50, 200, 200, 200);
			
			button.SetParent (window.Root);
			button.Bounds = new Rectangle (10, 10, 80, 24);
			button.Text   = "Test";
			button.Clicked += new MessageEventHandler(AlphaTestButtonClicked);
			
			window.WindowLocation = new Point (3840 + 20, 1000);
			
			window.Show ();
			Window.PumpEvents ();
			
			text.SetParent (window.Root);
			text.Dock   = DockStyle.Top;
			text.Text   = window.DebugWindowHandle + "  <font color=\"white\">" + window.DebugWindowHandle + "</font>";
			
			window.Root.Invalidate ();
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
			window.Alpha = scroller.DoubleValue / 2 + 0.5;
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
		
		
//		[Test] public void Zzz()
//		{
//			Window window = new Window ();
//			window.Text = "Zzz...";
//			StaticText text = new StaticText ("Close this window to stop test");
//			text.Parent = window.Root;
//			text.Dock   = DockStyle.Fill;
//			window.WindowSize = new Drawing.Size (200, 80);
//			window.Show ();
//			window.Run ();
		//		}

		private void AlphaTestButtonClicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Button clicked.");
		}
	}
}
