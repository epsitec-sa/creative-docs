using NUnit.Framework;
using Epsitec.Cresus.UserInterface;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.Database;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class DataBinderTest
	{
		[SetUp] public void SetUp()
		{
			ObjectBundler.Initialise ();
		}
		
		[Test] public void CheckBinderFactory()
		{
			IBinder binder = BinderFactory.FindBinderForType ("Test");
			Assertion.AssertNull (binder);
		}
		
		[Test] public void CheckBinderWithSimpleWindow()
		{
			System.Data.DataSet data_set = new System.Data.DataSet ("test");
			
			object o1 = true;
			object o2 = false;
			
			data_set.Tables.Add ("x");
			data_set.Tables["x"].Columns.Add ("a", typeof (bool));
			data_set.Tables["x"].Columns.Add ("b", typeof (bool));
			data_set.Tables["x"].Rows.Add (new object[] { o1, o2 });
			data_set.AcceptChanges ();
			
			System.Data.DataRow row = data_set.Tables["x"].Rows[0];
			
			ResourceBundle bundle  = Resources.GetBundle ("file:simple_window");
			DataBinder     binder  = new DataBinder ();
			ObjectBundler  bundler = binder.ObjectBundler;
			DataStore      data    = new DataStore (data_set);
			
			binder.DataStore = data;
			
			DbType   db_bool_type = new DbTypeNum (DbNumDef.FromRawType (DbRawType.Boolean), "name=boolean");
			DbTable  db_table = new DbTable ("x");
			DbColumn db_col_a = new DbColumn ("a", db_bool_type);
			DbColumn db_col_b = new DbColumn ("b", db_bool_type);
			
			db_col_a.DefineAttributes ("capt=Option vitale 'A'");
			db_col_b.DefineAttributes ("capt=Copie de l'option vitale 'A'");
			
			db_table.Columns.Add (db_col_a);
			db_table.Columns.Add (db_col_b);
			
			data.Attach (db_table);
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append ("\n");
			DbTable.SerialiseToXml (buffer, data.FindDbTable ("x"), true);
			buffer.Append ("\n");
			DbColumn.SerialiseToXml (buffer, data.FindDbColumn ("x.*.a"), true);
			buffer.Append ("\n");
			DbColumn.SerialiseToXml (buffer, data.FindDbColumn ("x.*.b"), true);
			
			System.Console.Out.WriteLine ("XML meta description:{0}", buffer.ToString ());
			
			Assertion.AssertEquals ("boolean", db_table.Columns["a"].Type.Name);
			Assertion.AssertEquals ("boolean", data.FindDbColumn ("x.*.a").Type.Name);
			Assertion.AssertEquals ("boolean", data.FindDbColumn ("x.*.b").Type.Name);
			
			Assertion.AssertEquals ("Option vitale 'A'",			data.FindDbColumn ("x.*.a").Caption);
			Assertion.AssertEquals ("Copie de l'option vitale 'A'",	data.FindDbColumn ("x.*.b").Caption);
			
			data.AttachObserver ("x.*.a", new DataChangeEventHandler (HandleDataChanged));
			data.AttachObserver ("x.*.b", new DataChangeEventHandler (HandleDataChanged));
			
			Assertion.AssertNotNull (bundle);
			Assertion.AssertNotNull (bundler);
			
			object             obj  = bundler.CreateFromBundle (bundle);
			Widgets.WindowRoot root = obj as Widgets.WindowRoot;
			
			Assertion.AssertNotNull (obj);
			Assertion.AssertNotNull (root);
			
			root.Window.Show ();
		}

		private void HandleDataChanged(object sender, DataChangeEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Data changed: " + e.Path);
			
			if (e.Path == "x.0.a")
			{
				//	On change la valeur "b" dès que la valeur "a" change... Mais pas
				//	le contraire. Ca permet de montrer que les événements de l'interface
				//	modifient bien les données, et la modification de celles-ci met bien
				//	à jour l'interface...
				
				DataStore store = sender as DataStore;
				store["x.0.b"] = store["x.0.a"];
			}
		}
	}
}
