using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class ObjectBundlerTest
	{
		[SetUp] public void SetUp()
		{
			Resources.SetupProviders ("test");
		}
		
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
		
		[Test] public void CheckFillBundleFromObject()
		{
			ResourceBundle bundle = Resources.GetBundle ("file:button.cancel");
			ObjectBundler bundler = new ObjectBundler ();
			
			Assertion.AssertNotNull (bundle);
			
			object         obj    = bundler.CreateFromBundle (bundle);
			Widgets.Button button = obj as Widgets.Button;
			
			bundle = ResourceBundle.Create ("button.cancel");
			bundler.PropertyBundled += new BundlingPropertyEventHandler(this.HandleBundlerPropertyBundled);
			bundler.FillBundleFromObject (bundle, button);
			bundler.PropertyBundled -= new BundlingPropertyEventHandler(this.HandleBundlerPropertyBundled);
			
			bundle.CreateXmlDocument (false).Save (System.Console.Out);
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
			
			object             obj  = bundler.CreateFromBundle (bundle);
			Widgets.WindowRoot root = obj as Widgets.WindowRoot;
			
			Assertion.AssertNotNull (obj);
			Assertion.AssertNotNull (root);
			
			this.test_window = root.Window;
			
			root.Window.Show ();
		}
		
		[Test] public void CheckCommandDispatcher()
		{
			if (ObjectBundlerTest.register_window_cancel)
			{
				CommandDispatcher.Default.RegisterController (this);
				
				ObjectBundlerTest.register_window_cancel = false;
			}
		}
		
		private static bool		register_window_cancel = true;
		private Widgets.Window	test_window;
		
		[Command ("CancelSimpleWindow")]
		[Command ("QuitSimpleWindow")]
		private void HandleCommandCancel(CommandDispatcher sender, CommandEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Execute command " + e.CommandName + " from " + e.Source);
			
			if (this.test_window != null)
			{
				this.test_window.Dispose ();
				this.test_window = null;
			}
		}

		private void HandleBundlerPropertyBundled(object sender, BundlingPropertyEventArgs e)
		{
			System.Console.Out.WriteLine ("Processing property {0}, data='{1}', suppress={2}.", e.PropertyName, e.PropertyData, e.SuppressProperty);
			
			if (e.SuppressProperty == false)
			{
				if (e.PropertyType == typeof (Drawing.Size))
				{
					Drawing.Size def_value = (Drawing.Size) e.PropertyDefault;
					Drawing.Size cur_value = (Drawing.Size) e.PropertyValue;
					
					bool sw = (def_value.Width == cur_value.Width);
					bool sh = (def_value.Height == cur_value.Height);
					
					e.PropertyData = Drawing.Size.Converter.ToString (cur_value, System.Globalization.CultureInfo.InvariantCulture, sw, sh);
				}
			}
		}
	}
}
