using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class ObjectBundlerTest
	{
		[SetUp] public void SetUp()
		{
			Widgets.Widget.Initialise ();
			Resources.SetupApplication ("test");
		}
		
		
		[Test] public void CheckRegisterAssembly()
		{
			ObjectBundler.Initialise ();
		}
		
		[Test] public void CheckCreateFromBundle()
		{
			ResourceBundle bundle = Resources.GetBundle ("file:button.cancel");
			ObjectBundler bundler = new ObjectBundler (Resources.DefaultManager);
			
			Assert.IsNotNull (bundle);
			
			object         obj    = bundler.CreateFromBundle (bundle);
			Widgets.Button button = obj as Widgets.Button;
			
			Assert.IsNotNull (obj);
			Assert.IsNotNull (button);
			Assert.AreEqual ("cancel", button.Name);
			Assert.IsTrue (button.Text.Length > 0);
			Assert.AreEqual (Widgets.AnchorStyles.None, button.Anchor);
			Assert.AreEqual (100, button.Width);
			Assert.AreEqual (new Widgets.Button ().Height, button.Height);
		}
		
		[Test] public void CheckCreateABFromBundle()
		{
			ResourceBundle bundle = Resources.GetBundle ("file:ab");
			ObjectBundler bundler = new ObjectBundler (Resources.DefaultManager);
			
			Assert.IsNotNull (bundle);
			
			object obj = bundler.CreateFromBundle (bundle);
			
			A obj_a = obj as A;
			B obj_b = obj as B;
			
			Assert.IsNotNull (obj);
			Assert.IsNotNull (obj_a);
			Assert.IsNotNull (obj_b);
			
			Assert.AreEqual ("hello world !", obj_a.Value);
			Assert.AreEqual ("hello world !", obj_b.Value);
		}
		
		[Test] public void CheckFillBundleFromObject()
		{
			ResourceBundle bundle = Resources.GetBundle ("file:button.cancel");
			ObjectBundler bundler = new ObjectBundler (Resources.DefaultManager);
			
			Assert.IsNotNull (bundle);
			
			object         obj    = bundler.CreateFromBundle (bundle);
			Widgets.Button button = obj as Widgets.Button;
			
			bundle = ResourceBundle.Create (Resources.DefaultManager, "button.cancel");
			bundler.PropertyBundled += new BundlingPropertyEventHandler(this.HandleBundlerPropertyBundled);
			bundler.FillBundleFromObject (bundle, button);
			bundler.PropertyBundled -= new BundlingPropertyEventHandler(this.HandleBundlerPropertyBundled);
			
			bundle.CreateXmlDocument (false).Save (System.Console.Out);
		}
		
		[Test] public void CheckFillBundleFromA()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "a_soft");
			ObjectBundler bundler = new ObjectBundler (Resources.DefaultManager);
			
			A obj = new A ();
			
			obj.Value = "Hello World !";
			
			bundler.FillBundleFromObject (bundle, obj);
			
			string xml = bundle.CreateXmlDocument (false).InnerXml;
			
			Assert.AreEqual (@"<bundle name=""a_soft""><data name=""class"">A</data><data name=""Value"">Hello World !</data></bundle>", xml);
		}
		
		[Test] public void CheckFillBundleFromB()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "b_soft");
			ObjectBundler bundler = new ObjectBundler (Resources.DefaultManager);
			
			A obj = new B ();
			
			obj.Value = "Hello World !";
			
			bundler.FillBundleFromObject (bundle, obj);
			
			string xml = bundle.CreateXmlDocument (false).InnerXml;
			
			Assert.AreEqual (@"<bundle name=""b_soft""><data name=""class"">B</data><data name=""Value"">hello world !</data></bundle>", xml);
		}
		
		[Test] public void CheckFindPropertyInfo()
		{
			ObjectBundler bundler = new ObjectBundler (Resources.DefaultManager);
			Widgets.Button button = new Widgets.Button ();
			
			Assert.IsNotNull (bundler.FindPropertyInfo (button, "Text"));
		}
		
		[Test] public void CheckIsPropertyEqual()
		{
			ObjectBundler bundler = new ObjectBundler (Resources.DefaultManager);
			Widgets.Button button = new Widgets.Button ();
			
			button.Text = "Hello";
			
			Assert.IsTrue (bundler.IsPropertyEqual (button, "Text", "Hello"));
			Assert.IsTrue (bundler.IsPropertyEqual (button, "Anchor", button.Anchor.ToString ()));
		}
		
		[Test] public void CheckSimpleWindow()
		{
			ResourceBundle bundle = Resources.GetBundle ("file:simple_window");
			ObjectBundler bundler = new ObjectBundler (Resources.DefaultManager);
			
			Assert.IsNotNull (bundle);
			
			bundler.EnableMapping ();
			
			object             obj  = bundler.CreateFromBundle (bundle);
			Widgets.WindowRoot root = obj as Widgets.WindowRoot;
			
			Assert.IsNotNull (obj);
			Assert.IsNotNull (root);
			
			this.test_window = root.Window;
			
			root.Window.Show ();
		}
		
		[Test] public void CheckFillBundleFromSimpleWindow()
		{
			Types.Time time_1 = Types.Time.Now;
			
			ResourceBundle bundle = Resources.GetBundle ("file:simple_window");
			ObjectBundler bundler = new ObjectBundler (Resources.DefaultManager);
			
			Assert.IsNotNull (bundle);
			
			bundler.EnableMapping ();
			
			object             obj  = bundler.CreateFromBundle (bundle);
			Widgets.WindowRoot root = obj as Widgets.WindowRoot;
			
			Assert.IsNotNull (obj);
			Assert.IsNotNull (root);
			
			Types.Time time_2 = Types.Time.Now;
			
			bundler = new ObjectBundler (Resources.DefaultManager);
			bundle  = ResourceBundle.Create (Resources.DefaultManager, "file", "cloned_simple_window", ResourceLevel.Default, System.Globalization.CultureInfo.CurrentCulture);
			
			bundler.SetupPrefix ("file");
			bundler.FillBundleFromObject (bundle, root);
			
			Types.Time time_3 = Types.Time.Now;
			
			bundle.CreateXmlDocument (false).Save ("test.xml");
			
			Types.Time time_4 = Types.Time.Now;
			
			
			System.Console.Out.WriteLine ("Load:  {0} ms", (time_2.Ticks-time_1.Ticks)/10000);
			System.Console.Out.WriteLine ("Store: {0} ms", (time_3.Ticks-time_2.Ticks)/10000);
			System.Console.Out.WriteLine ("XML:   {0} ms", (time_4.Ticks-time_3.Ticks)/10000);
		}
		
		[Test] public void CheckFillBundleFromSimpleWindowAndRestore()
		{
			ResourceBundle bundle = Resources.GetBundle ("file:simple_window");
			ObjectBundler bundler = new ObjectBundler (Resources.DefaultManager);
			
			Assert.IsNotNull (bundle);
			
			bundler.EnableMapping ();
			
			object             obj  = bundler.CreateFromBundle (bundle);
			Widgets.WindowRoot root = obj as Widgets.WindowRoot;
			
			Assert.IsNotNull (obj);
			Assert.IsNotNull (root);
			
			bundler = new ObjectBundler (Resources.DefaultManager);
			bundle  = ResourceBundle.Create (Resources.DefaultManager, "file", "cloned_simple_window", ResourceLevel.Default, System.Globalization.CultureInfo.CurrentCulture);
			
			bundler.SetupPrefix ("file");
			bundler.FillBundleFromObject (bundle, root);
			
			bundler = new ObjectBundler (Resources.DefaultManager);
			
			obj  = bundler.CreateFromBundle (bundle);
			root = obj as Widgets.WindowRoot;
			
			Assert.IsNotNull (obj);
			Assert.IsNotNull (root);
			
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
		
		[Test] public void CheckFindXmlRef()
		{
			Widgets.Widget widget = new Widgets.Widget ();
			
			widget.SetProperty (ObjectBundler.GetPropNameForXmlRef ("PropA"), @"<ref target=""strings#label.OK"" />");
			widget.SetProperty (ObjectBundler.GetPropNameForXmlRef ("PropB"), @"<ref target='strings#label.OK' />");
			widget.SetProperty (ObjectBundler.GetPropNameForXmlRef ("PropC"), @"<ref target="""" />");
			widget.SetProperty (ObjectBundler.GetPropNameForXmlRef ("PropD"), @"<ref />");
			widget.SetProperty (ObjectBundler.GetPropNameForXmlRef ("PropE"), @"foo");
			
			string s1, s2, s3, s4, s5, s6;
			
			Assert.IsTrue (ObjectBundler.FindXmlRef (widget, "PropA", out s1));
			Assert.IsTrue (ObjectBundler.FindXmlRef (widget, "PropB", out s2));
			
			Assert.IsFalse (ObjectBundler.FindXmlRef (widget, "PropC", out s3));
			Assert.IsFalse (ObjectBundler.FindXmlRef (widget, "PropD", out s4));
			Assert.IsFalse (ObjectBundler.FindXmlRef (widget, "PropE", out s5));
			Assert.IsFalse (ObjectBundler.FindXmlRef (widget, "PropX", out s6));
			
			Assert.AreEqual ("strings#label.OK", s1);
			Assert.AreEqual ("strings#label.OK", s2);
		}
		
		[Test] public void CheckDefineXmlRef()
		{
			Widgets.Widget widget = new Widgets.Widget ();
			
			widget.SetProperty (ObjectBundler.GetPropNameForXmlRef ("PropA"), @"A");
			widget.SetProperty (ObjectBundler.GetPropNameForXmlRef ("PropB"), @"B");
			
			ObjectBundler.DefineXmlRef (widget, "PropA", null);
			ObjectBundler.DefineXmlRef (widget, "PropB", "a#b");
			ObjectBundler.DefineXmlRef (widget, "PropC", "a#<b>");
			
			Assert.IsFalse (widget.IsPropertyDefined (ObjectBundler.GetPropNameForXmlRef ("PropA")));
			Assert.IsTrue (widget.IsPropertyDefined (ObjectBundler.GetPropNameForXmlRef ("PropB")));
			Assert.IsTrue (widget.IsPropertyDefined (ObjectBundler.GetPropNameForXmlRef ("PropC")));
			
			Assert.AreEqual (@"<ref target=""a#b"" />", widget.GetProperty (ObjectBundler.GetPropNameForXmlRef ("PropB")));
			Assert.AreEqual (@"<ref target=""a#&lt;b&gt;"" />", widget.GetProperty (ObjectBundler.GetPropNameForXmlRef ("PropC")));
		}
		
		[Test] public void CheckCopyObject()
		{
			A a1 = new A ();
			A a2 = new A ();
			C c1 = new C ();
			C c2 = new C ();
			
			a1.Value = "A1";
			c1.Value = "C1-Value";
			c1.Extra = "C1-Extra";
			
			ObjectBundler bundler = new ObjectBundler (Resources.DefaultManager);
			
			Assert.IsTrue (bundler.CopyObject (a1, a2));
			Assert.IsTrue (bundler.CopyObject (a1, c2));
			Assert.AreEqual ("A1", a2.Value);
			Assert.AreEqual ("A1", c2.Value);
			
			Assert.IsTrue (bundler.CopyObject (c1, a2));
			Assert.IsTrue (bundler.CopyObject (c1, c2));
			
			Assert.AreEqual ("C1-Value", a2.Value);
			Assert.AreEqual ("C1-Value", c2.Value);
			Assert.AreEqual ("C1-Extra", c2.Extra);
		}
		
		[Test] public void CheckCopyWidgetWithValidator()
		{
			Widgets.Widget w1 = new Widgets.Widget ();
			Widgets.Validators.RegexValidator v1 = new Epsitec.Common.Widgets.Validators.RegexValidator (w1, "[a-z][1-9]*", false);
			Widgets.Validators.RegexValidator v2 = new Epsitec.Common.Widgets.Validators.RegexValidator (w1, "....", false);
			
			ObjectBundler bundler = new ObjectBundler (Resources.DefaultManager);
			
			Widgets.Widget w2 = bundler.CopyObject (w1) as Widgets.Widget;
			
			Assert.AreEqual (2, Support.MulticastValidator.ToArray (w1.Validator).Length);
			
			Widgets.Validators.RegexValidator v3 = Support.MulticastValidator.ToArray (w1.Validator)[0] as Widgets.Validators.RegexValidator;
			Widgets.Validators.RegexValidator v4 = Support.MulticastValidator.ToArray (w1.Validator)[1] as Widgets.Validators.RegexValidator;
			
			Assert.AreEqual (v1.Regex, v3.Regex);
			Assert.AreEqual (v2.Regex, v4.Regex);
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
		
		
		private class A : IBundleSupport
		{
			public A()
			{
			}
			
			
			[Bundle] public string				Value
			{
				get
				{
					return this.GetValue ();
				}
				set
				{
					this.value = value;
				}
			}
			
			
			protected virtual string GetValue()
			{
				return this.value;
			}
			
			
			#region IBundleSupport Members
			public void AttachResourceManager(ResourceManager resource_manager)
			{
			}
			
			public void SerializeToBundle(ObjectBundler bundler, ResourceBundle bundle)
			{
			}

			public void RestoreFromBundle(ObjectBundler bundler, ResourceBundle bundle)
			{
			}
			
			public string						BundleName
			{
				get
				{
					return null;
				}
			}

			public string PublicClassName
			{
				get
				{
					return this.GetType ().Name;
				}
			}
			#endregion
			
			#region IDisposable Members
			public void Dispose()
			{
			}
			#endregion
			
			protected string					value = "";
		}
		
		private class B : A
		{
			public B()
			{
			}
			
			
			protected override string GetValue()
			{
				return base.GetValue ().ToLower ();
			}
		}
		
		private class C : IBundleSupport
		{
			public C()
			{
			}
			
			
			[Bundle] public string				Value
			{
				get
				{
					return this.GetValue ();
				}
				set
				{
					this.value = value;
				}
			}
			
			[Bundle] public string				Extra
			{
				get
				{
					return this.extra;
				}
				set
				{
					this.extra = value;
				}
			}
			
			
			protected virtual string GetValue()
			{
				return this.value;
			}
			
			
			#region IBundleSupport Members
			public void AttachResourceManager(ResourceManager resource_manager)
			{
			}
			
			public void SerializeToBundle(ObjectBundler bundler, ResourceBundle bundle)
			{
			}

			public void RestoreFromBundle(ObjectBundler bundler, ResourceBundle bundle)
			{
			}
			
			public string						BundleName
			{
				get
				{
					return null;
				}
			}

			public string PublicClassName
			{
				get
				{
					return this.GetType ().Name;
				}
			}
			#endregion
			
			#region IDisposable Members
			public void Dispose()
			{
			}
			#endregion
			
			protected string					value = "";
			protected string					extra = "";
		}
	}
}
