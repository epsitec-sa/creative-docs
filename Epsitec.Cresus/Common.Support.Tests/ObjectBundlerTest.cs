using NUnit.Framework;

namespace Epsitec.Common.Support.Tests
{
	[TestFixture]
	public class ObjectBundlerTest
	{
		[Test] public void CheckRegisterAssembly()
		{
			ObjectBundler.RegisterAssembly (typeof (Widgets.Widget).Assembly);
		}
		
		[Test] public void CheckCreateFromBundle()
		{
			ResourceBundle bundle = Resources.GetBundle ("file:button.cancel");
			ObjectBundler bundler = new ObjectBundler ();
			
			Assertion.AssertNotNull (bundle);
			
			object         obj    = bundler.CreateFromBundle (bundle);
			Widgets.Button button = obj as Widgets.Button;
			
			Assertion.AssertNotNull (obj);
			Assertion.AssertNotNull (button);
			Assertion.AssertEquals ("cancel", button.Name);
			Assertion.Assert (button.Text.Length > 0);
			Assertion.AssertEquals (Widgets.AnchorStyles.Left | Widgets.AnchorStyles.Bottom, button.Anchor);
			Assertion.AssertEquals (100, button.Width);
			Assertion.AssertEquals (new Widgets.Button ().Height, button.Height);
		}
		
		[Test] public void CheckFindPropertyInfo()
		{
			ObjectBundler bundler = new ObjectBundler ();
			Widgets.Button button = new Widgets.Button ();
			
			Assertion.AssertNotNull (bundler.FindPropertyInfo (button, "text"));
		}
		
		[Test] public void CheckIsPropertyEqual()
		{
			ObjectBundler bundler = new ObjectBundler ();
			Widgets.Button button = new Widgets.Button ();
			
			button.Text = "Hello";
			
			Assertion.Assert (bundler.IsPropertyEqual (button, "text", "Hello"));
			Assertion.Assert (bundler.IsPropertyEqual (button, "anchor", button.Anchor.ToString ()));
		}
		
		[Test] public void CheckSimpleWindow()
		{
			ResourceBundle bundle = Resources.GetBundle ("file:simple_window");
			ObjectBundler bundler = new ObjectBundler ();
			
			Assertion.AssertNotNull (bundle);
			
			object         obj    = bundler.CreateFromBundle (bundle);
			Widgets.Window window = obj as Widgets.Window;
			
			Assertion.AssertNotNull (obj);
			Assertion.AssertNotNull (window);
			
			window.Show ();
		}
		
		[Test] public void CheckCommandDispatcher()
		{
			if (ObjectBundlerTest.register_window_cancel)
			{
				CommandDispatcher.Default.Register ("my_window.cancel", new CommandEventHandler (this.HandleCommandCancel));
				CommandDispatcher.Default.Register ("main_menu.quit", new CommandEventHandler (this.HandleCommandCancel));
				
				ObjectBundlerTest.register_window_cancel = false;
			}
		}
		
		private static bool register_window_cancel = true;
		
		private void HandleCommandCancel(object sender, CommandEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("User clicked cancel button -> close window");
			
			Widgets.Widget widget = sender as Widgets.Widget;
			widget.Window.Dispose ();
		}
	}
}
