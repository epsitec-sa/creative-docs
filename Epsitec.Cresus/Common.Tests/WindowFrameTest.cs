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
			window.Root.Text = "Hel&lo";
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
	}
}
