using System;
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
			try { System.Diagnostics.Debug.WriteLine (""); } 
			catch { }
		}
		
		[Test] public void TestCreation()
		{
			WindowFrame window = new WindowFrame ();
			window.Show ();
			window.Root.Clicked += new MessageEventHandler(Root_Clicked);
			window.Root.DoubleClicked += new MessageEventHandler(Root_DoubleClicked);
			window.Root.Text = "Hel&lo";
			window.Root.ShortcutPressed += new EventHandler(Root_ShortcutPressed);
		}

		private void Root_Clicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Root Clicked");
		}

		private void Root_DoubleClicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Root Double Clicked");
		}

		private void Root_ShortcutPressed(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Shortcut key pressed");
		}
	}
}
