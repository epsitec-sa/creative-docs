using NUnit.Framework;
using Epsitec.Cresus.UserInterface;
using Epsitec.Cresus.DataLayer;

namespace Epsitec.Common.Support.Tests
{
	[TestFixture]
	public class DataBinderTest
	{
		[SetUp] public void SetUp()
		{
			ObjectBundler.RegisterAssembly (typeof (Widgets.Widget).Assembly);
		}
		
		[Test] public void CheckBinderFactory()
		{
			IBinder binder = BinderFactory.FindBinder ("Test");
			Assertion.AssertNull (binder);
		}
		
		[Test] public void CheckBinderWithSimpleWindow()
		{
			ResourceBundle bundle  = Resources.GetBundle ("file:simple_window");
			DataBinder     binder  = new DataBinder ();
			ObjectBundler  bundler = binder.ObjectBundler;
			DataSet        data    = new DataSet ("test");
			
			binder.DataSet = data;
			
			data.AddData ("a", true, new DataType ("boolean"));
			data.ValidateChanges ();
			data.AttachObserver ("a", new DataChangedHandler (HandleDataChanged));
			
			Assertion.AssertNotNull (bundle);
			Assertion.AssertNotNull (bundler);
			
			object              obj    = bundler.CreateFromBundle (bundle);
			Widgets.WindowFrame window = obj as Widgets.WindowFrame;
			
			Assertion.AssertNotNull (obj);
			Assertion.AssertNotNull (window);
			
			window.Show ();
		}

		private void HandleDataChanged(DataRecord sender, string path)
		{
			System.Diagnostics.Debug.WriteLine ("Data changed: " + path);
		}
	}
}
