using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Tests.Widgets
{
	[TestFixture]
	public class WindowTest
	{
		static WindowTest()
		{
		}


		[Test] public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}
		
		[Test] public void CheckFrameCreation()
		{
			Window window = new Window ();
			
			window.Root.Clicked += Root_Clicked;
			window.Root.DoubleClicked += Root_DoubleClicked;
			window.Root.Text = "Hel<m>l</m>o";
			window.Root.ShortcutPressed += Root_ShortcutPressed;
			
			window.Text = "CheckFrameCreation";
			window.Show ();
			Window.RunInTestEnvironment (window);
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
			window.WindowActivated += window_Activated;
			window.WindowDeactivated += window_Deactivated;
			window.WindowBounds = new Rectangle (ox+10, oy+30, 50, 10);
			window.Root.BackColor = Color.FromRgb (1, 0, 0);
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
			window.Root.BackColor = Color.FromRgb (1, 1, 1);
			
			Button button = new Button (window.Root);
			button.SetManualBounds(new Rectangle (250, 10, 40, 20));
			button.Text   = "?";
			
			TextField text = new TextField (window.Root);
			text.SetManualBounds(new Rectangle (150, 10, 95, 20));
			
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
//			back.SetClientZoom (zoom);
			
			CheckButton button = new CheckButton ();
			VScroller scroller = new VScroller ();
			
			button.SetManualBounds(new Rectangle (10, 10, 60, 24));
			button.Text = "Test";
			button.SetParent (back);
			button.Clicked += button_Clicked;
			
			scroller.SetManualBounds(new Rectangle (80, 10, 17, 180));
			scroller.MaxValue = 1.0M;
			scroller.VisibleRangeRatio = 0.1M;
			scroller.Value = 0.0M;
			scroller.SmallChange = 0.01M;
			scroller.LargeChange = 0.10M;
			scroller.SetParent (back);
			scroller.ValueChanged += scroller_ValueChanged;
			
			window.Show ();
			window.Root.Invalidate ();
		}

		[Test] public void CheckAppActivation()
		{
			Window.ApplicationActivated   += Window_ApplicationActivated;
			Window.ApplicationDeactivated += Window_ApplicationDeactivated;
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
			Image image = Bitmap.FromFile (@"Images\cursor.png");
			
			Window window = new Window ();
			window.ClientSize = new Size (200, 200);
			window.MouseCursor = MouseCursor.FromImage (image, 4, 28);
			window.Text = "CheckMouseCursor";
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test]
		public void CheckWindowPlacement()
		{
			Window window = new Window ();
			Timer timer = new Timer ();
			
			timer.AutoRepeat = 1.0;
			timer.Delay      = 1.0;
			timer.TimeElapsed += (s) => System.Diagnostics.Debug.WriteLine (window.WindowPlacement.ToString ());
			timer.Start ();

			window.Text = "CheckWindowPlacement";
			window.Root.WindowStyles = WindowStyles.DefaultDocumentWindow;
			window.Show ();
			Window.RunInTestEnvironment (window);
		}
		
		[Test]
		public void CheckTabNavigation()
		{
			Window window = new Window ();
			window.Text = "CheckTabNavigation";
			window.ClientSize = new Size (450, 230);
			window.MakeFixedSizeWindow ();

//-			Assert.IsNotNull (window.CommandDispatchers[0]);
//-			Assert.IsNotNull (window.Root.CommandDispatchers[0]);

			CommandDispatcher dispatcher = new CommandDispatcher ();
			
			dispatcher.RegisterController (new MyController ());

			CommandDispatcher.SetDispatcher (window, dispatcher);
			
			Command command_open = Command.Get ("Open"); command_open.Shortcuts.Add (KeyCode.ModifierControl | KeyCode.AlphaO);
			Command command_save = Command.Get ("Save"); command_save.Shortcuts.Add (KeyCode.ModifierAlt | KeyCode.AlphaS);
			Command command_cut  = Command.Get ("ClipCut"); command_cut.Shortcuts.Add (KeyCode.ModifierControl | KeyCode.AlphaX);
			
			Button      button;
			GroupBox    group;
			Widget      widget;
			RadioButton radio;
			CheckButton check;
			
			button = new Button ("A");
			button.SetManualBounds(new Rectangle (10, 170, 40, 25));
			button.SetParent (window.Root);
			button.TabIndex = 1;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			button.Focus();
			button.ButtonStyle = ButtonStyle.DefaultAccept;
			
			button = new Button ("B");
			button.SetManualBounds(new Rectangle (10, 140, 40, 25));
			button.SetParent (window.Root);
			button.TabIndex = 2;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			button.Enable = false;
			
			button = new Button ("C");
			button.SetManualBounds(new Rectangle (10, 110, 40, 25));
			button.SetParent (window.Root);
			button.TabIndex = 3;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			button = new Button ("D");
			button.SetManualBounds(new Rectangle (10, 80, 40, 25));
			button.SetParent (window.Root);
			button.TabIndex = 4;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			group = new GroupBox ();
			group.SetManualBounds(new Rectangle (60, 110, 110, 85));
			group.SetParent (window.Root);
			group.TabIndex = 10;
			group.Text = "Group 1";
			group.SetTabNavigation (TabNavigationMode.ActivateOnTab);
			
			button = new Button ("A");
			button.SetManualBounds(new Rectangle (10, 40, 40, 25));
			button.SetParent (group);
			button.TabIndex = 1;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			button = new Button ("B");
			button.SetManualBounds(new Rectangle (10, 10, 40, 25));
			button.SetParent (group);
			button.TabIndex = 2;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			button = new Button ("C");
			button.SetManualBounds(new Rectangle (55, 40, 40, 25));
			button.SetParent (group);
			button.TabIndex = 3;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			button = new Button ("D");
			button.SetManualBounds(new Rectangle (55, 10, 40, 25));
			button.SetParent (group);
			button.TabIndex = 4;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			group = new GroupBox ();
			group.SetManualBounds(new Rectangle (180, 110, 110, 85));
			group.SetParent (window.Root);
			group.TabIndex = 11;
			group.Text = "Group 2";
			group.SetTabNavigation (TabNavigationMode.ActivateOnTab | TabNavigationMode.ForwardToChildren);
			
			button = new Button ("A");
			button.SetManualBounds(new Rectangle (10, 40, 40, 25));
			button.SetParent (group);
			button.TabIndex = 1;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			button = new Button ("B");
			button.SetManualBounds(new Rectangle (10, 10, 40, 25));
			button.SetParent (group);
			button.TabIndex = 2;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			button = new Button ("C");
			button.SetManualBounds(new Rectangle (55, 40, 40, 25));
			button.SetParent (group);
			button.TabIndex = 3;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			button = new Button ("D");
			button.SetManualBounds(new Rectangle (55, 10, 40, 25));
			button.SetParent (group);
			button.TabIndex = 4;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			group = new GroupBox ();
			group.SetManualBounds(new Rectangle (300, 110, 110, 85));
			group.SetParent (window.Root);
			group.TabIndex = 12;
			group.Text = "Group 3";
			group.SetTabNavigation (TabNavigationMode.ActivateOnTab | TabNavigationMode.ForwardToChildren | TabNavigationMode.ForwardOnly);
			
			button = new Button ("A");
			button.SetManualBounds(new Rectangle (10, 40, 40, 25));
			button.SetParent (group);
			button.TabIndex = 1;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			button = new Button ("B");
			button.SetManualBounds(new Rectangle (10, 10, 40, 25));
			button.SetParent (group);
			button.TabIndex = 2;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			button = new Button ("C");
			button.SetManualBounds(new Rectangle (55, 40, 40, 25));
			button.SetParent (group);
			button.TabIndex = 3;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			button = new Button ("D");
			button.SetManualBounds(new Rectangle (55, 10, 40, 25));
			button.SetParent (group);
			button.TabIndex = 4;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			button = new Button ("E");
			button.SetManualBounds(new Rectangle (10, 50, 40, 25));
			button.SetParent (window.Root);
			button.TabIndex = 20;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			button = new Button ("F");
			button.SetManualBounds(new Rectangle (10, 20, 40, 25));
			button.SetParent (window.Root);
			button.TabIndex = 21;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			widget = new TextField ();
			widget.SetManualBounds(new Rectangle (60, 74, 100, 22));
			widget.SetParent (window.Root);
			widget.TabIndex = 30;
			widget.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			widget = new TextFieldUpDown ();
			widget.SetManualBounds(new Rectangle (165, 74, 40, 22));
			widget.SetParent (window.Root);
			widget.TabIndex = 31;
			widget.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			widget = new TextFieldUpDown ();
			widget.SetManualBounds(new Rectangle (210, 74, 40, 22));
			widget.SetParent (window.Root);
			widget.TabIndex = 32;
			widget.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			StaticText text = new StaticText ();
			
			text.SetManualBounds(new Rectangle (60, 15, 420, 50));
			text.SetParent (window.Root);
			text.ContentAlignment = ContentAlignment.TopLeft;
			text.Text      = "<b>Group 1:</b> cannot be entered with TAB<br/>"
				/**/       + "<b>Group 2:</b> can be focused and entered with TAB<br/>"
				/**/       + "<b>Group 3:</b> cannot be focused, but can be entered with TAB<br/>";
			text.TextLayout.BreakMode = TextBreakMode.Hyphenate;
			
			text = new StaticText ();
			text.SetManualBounds(new Rectangle (10, 200, 230, 25));
			text.Text   = "<font size=\"130%\">Press <b>TAB</b> to move the focus...</font>";
			text.SetParent (window.Root);
			
			radio = new RadioButton ();
			radio.SetManualBounds(new Rectangle (260, 75+7, 40, 20));
			radio.Text   = "A";
			radio.Group  = "Option1";
			radio.Index  = 0;
			radio.SetParent (window.Root);
			radio.TabIndex = 40;
			radio.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			radio.ActiveState = ActiveState.Yes;
			
			radio = new RadioButton ();
			radio.SetManualBounds(new Rectangle (260, 61+7, 40, 20));
			radio.Text   = "B";
			radio.Group  = "Option1";
			radio.Index  = 1;
			radio.SetParent (window.Root);
			radio.TabIndex = 40;
			radio.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			radio = new RadioButton ();
			radio.SetManualBounds(new Rectangle (260, 47+7, 40, 20));
			radio.Text   = "C";
			radio.Group  = "Option1";
			radio.Index  = 2;
			radio.SetParent (window.Root);
			radio.TabIndex = 40;
			radio.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			radio = new RadioButton ();
			radio.SetManualBounds(new Rectangle (300, 75+7, 40, 20));
			radio.Text   = "D";
			radio.Group  = "Option2";
			radio.Index  = 0;
			radio.SetParent (window.Root);
			radio.TabIndex = 41;
			radio.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			radio.ActiveState = ActiveState.Yes;
			
			radio = new RadioButton ();
			radio.SetManualBounds(new Rectangle (300, 61+7, 40, 20));
			radio.Text   = "E";
			radio.Group  = "Option2";
			radio.Index  = 1;
			radio.SetParent (window.Root);
			radio.TabIndex = 41;
			radio.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			radio = new RadioButton ();
			radio.SetManualBounds(new Rectangle (300, 47+7, 40, 20));
			radio.Text   = "F";
			radio.Group  = "Option2";
			radio.Index  = 2;
			radio.SetParent (window.Root);
			radio.TabIndex = 41;
			radio.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			check = new CheckButton ();
			check.SetManualBounds(new Rectangle (340, 75+7, 40, 20));
			check.Text   = "G";
			check.SetParent (window.Root);
			check.TabIndex = 50;
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveState = ActiveState.Yes;
			
			check = new CheckButton ();
			check.SetManualBounds(new Rectangle (340, 61+7, 40, 20));
			check.Text   = "H";
			check.SetParent (window.Root);
			check.TabIndex = 51;
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			check = new CheckButton ();
			check.SetManualBounds(new Rectangle (340, 47+7, 40, 20));
			check.Text   = "I";
			check.SetParent (window.Root);
			check.TabIndex = 52;
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test]
		public void CheckTabNavigationOverride()
		{
			Window window = new Window ();
			window.Text = "CheckTabNavigation";
			window.ClientSize = new Size (450, 230);
			window.MakeFixedSizeWindow ();

			CommandDispatcher dispatcher = new CommandDispatcher ();

			dispatcher.RegisterController (new MyController ());

			CommandDispatcher.SetDispatcher (window, dispatcher);

			Command command_open = Command.Get ("Open");
			command_open.Shortcuts.Add (KeyCode.ModifierControl | KeyCode.AlphaO);
			Command command_save = Command.Get ("Save");
			command_save.Shortcuts.Add (KeyCode.ModifierAlt | KeyCode.AlphaS);
			Command command_cut  = Command.Get ("ClipCut");
			command_cut.Shortcuts.Add (KeyCode.ModifierControl | KeyCode.AlphaX);

			Button      button;
			GroupBox    group;
			Widget      widget;
			RadioButton radio;
			CheckButton check;

			Widget buttonC;
			Widget buttonBGroup1;
			Widget buttonCGroup1;
			Widget group2;
			Widget group3;
			Widget buttonBGroup3;
			Widget buttonCGroup3;

			button = new Button ("A");
			button.SetManualBounds (new Rectangle (10, 170, 40, 25));
			button.SetParent (window.Root);
			button.TabIndex = 1;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			button.Focus ();
			button.ButtonStyle = ButtonStyle.DefaultAccept;

			button = new Button ("B");
			button.SetManualBounds (new Rectangle (10, 140, 40, 25));
			button.SetParent (window.Root);
			button.TabIndex = 2;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			button.Enable = false;

			button = new Button ("C");
			button.SetManualBounds (new Rectangle (10, 110, 40, 25));
			button.SetParent (window.Root);
			button.TabIndex = 3;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			buttonC = button;

			button = new Button ("D");
			button.SetManualBounds (new Rectangle (10, 80, 40, 25));
			button.SetParent (window.Root);
			button.TabIndex = 4;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			group = new GroupBox ();
			group.SetManualBounds (new Rectangle (60, 110, 110, 85));
			group.SetParent (window.Root);
			group.TabIndex = 10;
			group.Text = "Group 1";
			group.SetTabNavigation (TabNavigationMode.ActivateOnTab);

			button = new Button ("A");
			button.SetManualBounds (new Rectangle (10, 40, 40, 25));
			button.SetParent (group);
			button.TabIndex = 1;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			button = new Button ("B");
			button.SetManualBounds (new Rectangle (10, 10, 40, 25));
			button.SetParent (group);
			button.TabIndex = 2;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			buttonBGroup1 = button;

			button = new Button ("C");
			button.SetManualBounds (new Rectangle (55, 40, 40, 25));
			button.SetParent (group);
			button.TabIndex = 3;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			buttonCGroup1 = button;

			button = new Button ("D");
			button.SetManualBounds (new Rectangle (55, 10, 40, 25));
			button.SetParent (group);
			button.TabIndex = 4;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			group = new GroupBox ();
			group.SetManualBounds (new Rectangle (180, 110, 110, 85));
			group.SetParent (window.Root);
			group.TabIndex = 11;
			group.Text = "Group 2";
			group.SetTabNavigation (TabNavigationMode.ActivateOnTab | TabNavigationMode.ForwardToChildren);
			group2 = group;

			button = new Button ("A");
			button.SetManualBounds (new Rectangle (10, 40, 40, 25));
			button.SetParent (group);
			button.TabIndex = 1;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			button = new Button ("B");
			button.SetManualBounds (new Rectangle (10, 10, 40, 25));
			button.SetParent (group);
			button.TabIndex = 2;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			button = new Button ("C");
			button.SetManualBounds (new Rectangle (55, 40, 40, 25));
			button.SetParent (group);
			button.TabIndex = 3;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			button = new Button ("D");
			button.SetManualBounds (new Rectangle (55, 10, 40, 25));
			button.SetParent (group);
			button.TabIndex = 4;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			group = new GroupBox ();
			group.SetManualBounds (new Rectangle (300, 110, 110, 85));
			group.SetParent (window.Root);
			group.TabIndex = 12;
			group.Text = "Group 3";
			group.SetTabNavigation (TabNavigationMode.ActivateOnTab | TabNavigationMode.ForwardToChildren | TabNavigationMode.ForwardOnly);
			group3 = group;

			button = new Button ("A");
			button.SetManualBounds (new Rectangle (10, 40, 40, 25));
			button.SetParent (group);
			button.TabIndex = 1;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			button = new Button ("B");
			button.SetManualBounds (new Rectangle (10, 10, 40, 25));
			button.SetParent (group);
			button.TabIndex = 2;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			buttonBGroup3 = button;

			button = new Button ("C");
			button.SetManualBounds (new Rectangle (55, 40, 40, 25));
			button.SetParent (group);
			button.TabIndex = 3;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			buttonCGroup3 = button;

			button = new Button ("D");
			button.SetManualBounds (new Rectangle (55, 10, 40, 25));
			button.SetParent (group);
			button.TabIndex = 4;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			button = new Button ("E");
			button.SetManualBounds (new Rectangle (10, 50, 40, 25));
			button.SetParent (window.Root);
			button.TabIndex = 20;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			button = new Button ("F");
			button.SetManualBounds (new Rectangle (10, 20, 40, 25));
			button.SetParent (window.Root);
			button.TabIndex = 21;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			widget = new TextField ();
			widget.SetManualBounds (new Rectangle (60, 74, 100, 22));
			widget.SetParent (window.Root);
			widget.TabIndex = 30;
			widget.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			widget = new TextFieldUpDown ();
			widget.SetManualBounds (new Rectangle (165, 74, 40, 22));
			widget.SetParent (window.Root);
			widget.TabIndex = 31;
			widget.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			widget = new TextFieldUpDown ();
			widget.SetManualBounds (new Rectangle (210, 74, 40, 22));
			widget.SetParent (window.Root);
			widget.TabIndex = 32;
			widget.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			StaticText text = new StaticText ();

			text.SetManualBounds (new Rectangle (60, 5, 420, 60));
			text.SetParent (window.Root);
			text.ContentAlignment = ContentAlignment.TopLeft;
			text.Text      = "<b>Group 1:</b> cannot be entered with TAB<br/>"
				/**/       + "<b>Group 2:</b> can be focused and entered with TAB<br/>"
				/**/       + "<b>Group 3:</b> cannot be focused, but can be entered with TAB<br/>"
				/**/	   + "<i>Overrides: C to Group 1.B, Group 1.C to Group 2; Group 3: enter &gt;B, C&lt;</i><br/>";
			text.TextLayout.BreakMode = TextBreakMode.Hyphenate;

			text = new StaticText ();
			text.SetManualBounds (new Rectangle (10, 200, 230, 25));
			text.Text   = "<font size=\"130%\">Press <b>TAB</b> to move the focus...</font>";
			text.SetParent (window.Root);

			radio = new RadioButton ();
			radio.SetManualBounds (new Rectangle (260, 75+7, 40, 20));
			radio.Text   = "A";
			radio.Group  = "Option1";
			radio.Index  = 0;
			radio.SetParent (window.Root);
			radio.TabIndex = 40;
			radio.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			radio.ActiveState = ActiveState.Yes;

			radio = new RadioButton ();
			radio.SetManualBounds (new Rectangle (260, 61+7, 40, 20));
			radio.Text   = "B";
			radio.Group  = "Option1";
			radio.Index  = 1;
			radio.SetParent (window.Root);
			radio.TabIndex = 40;
			radio.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			radio = new RadioButton ();
			radio.SetManualBounds (new Rectangle (260, 47+7, 40, 20));
			radio.Text   = "C";
			radio.Group  = "Option1";
			radio.Index  = 2;
			radio.SetParent (window.Root);
			radio.TabIndex = 40;
			radio.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			radio = new RadioButton ();
			radio.SetManualBounds (new Rectangle (300, 75+7, 40, 20));
			radio.Text   = "D";
			radio.Group  = "Option2";
			radio.Index  = 0;
			radio.SetParent (window.Root);
			radio.TabIndex = 41;
			radio.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			radio.ActiveState = ActiveState.Yes;

			radio = new RadioButton ();
			radio.SetManualBounds (new Rectangle (300, 61+7, 40, 20));
			radio.Text   = "E";
			radio.Group  = "Option2";
			radio.Index  = 1;
			radio.SetParent (window.Root);
			radio.TabIndex = 41;
			radio.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			radio = new RadioButton ();
			radio.SetManualBounds (new Rectangle (300, 47+7, 40, 20));
			radio.Text   = "F";
			radio.Group  = "Option2";
			radio.Index  = 2;
			radio.SetParent (window.Root);
			radio.TabIndex = 41;
			radio.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			check = new CheckButton ();
			check.SetManualBounds (new Rectangle (340, 75+7, 40, 20));
			check.Text   = "G";
			check.SetParent (window.Root);
			check.TabIndex = 50;
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveState = ActiveState.Yes;

			check = new CheckButton ();
			check.SetManualBounds (new Rectangle (340, 61+7, 40, 20));
			check.Text   = "H";
			check.SetParent (window.Root);
			check.TabIndex = 51;
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			check = new CheckButton ();
			check.SetManualBounds (new Rectangle (340, 47+7, 40, 20));
			check.Text   = "I";
			check.SetParent (window.Root);
			check.TabIndex = 52;
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			buttonC.ForwardTabOverride = buttonBGroup1;
			buttonBGroup1.BackwardTabOverride = buttonC;
			buttonCGroup1.ForwardTabOverride = group2;
			group2.BackwardTabOverride = buttonCGroup1;

			group3.ForwardEnterTabOverride = buttonBGroup3;
			group3.BackwardEnterTabOverride = buttonCGroup3;

			window.Show ();
			Window.RunInTestEnvironment (window);
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
			Window.RunInTestEnvironment (window);
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
			button.SetManualBounds(new Rectangle (10, 10, 80, 24));
			button.Text   = "Test";
			button.Clicked += AlphaTestButtonClicked;
			
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
			button.SetManualBounds(new Rectangle (button.ActualLocation.X, button.ActualLocation.Y + 5, button.ActualWidth, button.ActualHeight));
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
		
		
		private void AlphaTestButtonClicked(object sender, MessageEventArgs e)
		{
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
	
			System.Diagnostics.Debug.WriteLine ("Button clicked.");
		}
	}
}
