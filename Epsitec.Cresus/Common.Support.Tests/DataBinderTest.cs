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
			data.AddData ("b", false, new DataType ("boolean"));
			
			data.GetDataField ("a").SetAttribute ("label", "Option vitale 'A'", ResourceLevel.Default);
			data.GetDataField ("b").SetAttribute ("label", "Copie de l'option vitale 'A'", ResourceLevel.Default);
			
			data.ValidateChanges ();
			data.AttachObserver ("a", new DataChangedHandler (HandleDataChanged));
			data.AttachObserver ("b", new DataChangedHandler (HandleDataChanged));
			
			Assertion.AssertNotNull (bundle);
			Assertion.AssertNotNull (bundler);
			
			object         obj    = bundler.CreateFromBundle (bundle);
			Widgets.Window window = obj as Widgets.Window;
			
			Assertion.AssertNotNull (obj);
			Assertion.AssertNotNull (window);
			
			window.Show ();
		}

		private void HandleDataChanged(DataRecord sender, string path)
		{
			System.Diagnostics.Debug.WriteLine ("Data changed: " + path);
			
			if (path == "a")
			{
				//	On change la valeur "b" dès que la valeur "a" change... Mais pas
				//	le contraire. Ca permet de montrer que les événements de l'interface
				//	modifient bien les données, et la modification de celles-ci met bien
				//	à jour l'interface...
				
				DataSet data_set = sender as DataSet;
				data_set.UpdateData ("b", data_set.GetData ("a"));
			}
		}
	}
}
