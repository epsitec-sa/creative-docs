using NUnit.Framework;

namespace Epsitec.Cresus.DataLayer.Tests
{
	[TestFixture]
	public class GeneralTest
	{
		[Test] public void CheckDataField()
		{
			DataRecord test = new DataField ();
			
			Assertion.AssertEquals (true,  test.IsField);
			Assertion.AssertEquals (false, test.IsSet);
			Assertion.AssertEquals (false, test.IsTable);
			
			Assertion.AssertNull (test.DataType);
		}
		
		[Test] public void CheckDataSet()
		{
			DataRecord test = new DataSet ("test");
			
			Assertion.AssertEquals (false, test.IsField);
			Assertion.AssertEquals (true,  test.IsSet);
			Assertion.AssertEquals (false, test.IsTable);
			
			Assertion.AssertNotNull (test.DataType);
		}
		
		[Test] public void CheckDataTable()
		{
			DataRecord test = new DataTable ();
			
			Assertion.AssertEquals (false, test.IsField);
			Assertion.AssertEquals (false, test.IsSet);
			Assertion.AssertEquals (true,  test.IsTable);
			
			Assertion.AssertNotNull (test.DataType);
		}
		
		[Test] public void CheckDataType()
		{
			DataType test1 = new DataType ("name=a", "binder=b", "label=c", "descr=d", "extra=e");
			DataType test2 = new DataType ("name=a", "label=c2", "descr=d2", "extra=e2");
			DataType test3 = new DataType ("name=x");
			
			Assertion.AssertEquals ("a", test1.Name);
			Assertion.AssertEquals ("b", test1.BinderEngine);
			Assertion.AssertEquals ("c", test1.UserLabel);
			Assertion.AssertEquals ("d", test1.UserDescription);
			Assertion.AssertEquals ("e", test1.Attributes.GetAttribute ("extra"));
			
			Assertion.AssertEquals ("a", test2.Name);
			Assertion.AssertEquals ("a", test2.BinderEngine);
			
			Assertion.AssertEquals ("x", test3.Name);
			
			Assertion.Assert (test1.Equals (test2));
			Assertion.Assert (! test1.Equals (test3));
			
			Assertion.AssertNull (test1.Attributes.GetAttribute ("missing"));
		}
		
		[Test] public void CheckCreateSet()
		{
			DataSet test = new DataSet ("test");
			
			test.AddData ("a", 1,		new DataType ("name=numeric"));
			test.AddData ("b", "hello", new DataType ("name=text"));
			test.AddData ("c", 10.5,	new DataType ("name=numeric"));
			
			Assertion.AssertNotNull (test.FindRecord ("a"));
			Assertion.AssertNotNull (test.FindRecord ("b"));
			Assertion.AssertNotNull (test.FindRecord ("c"));
			
			Assertion.Assert (test.FindRecord ("a").IsField);
			Assertion.Assert (test.FindRecord ("b").IsField);
			Assertion.Assert (test.FindRecord ("c").IsField);
			
			Assertion.AssertEquals (DataState.Added, test.FindRecord ("a").DataState);
			Assertion.AssertEquals (DataState.Added, test.FindRecord ("b").DataState);
			Assertion.AssertEquals (DataState.Added, test.FindRecord ("c").DataState);
			
			test.RemoveData ("b");
			test.RemoveData ("c");
			
			test.AddData ("b", "bye", new DataType ("name=text"));
			
			Assertion.AssertNotNull (test.FindRecord ("b"));
			Assertion.AssertNull (test.FindRecord ("c"));
			
			Assertion.AssertEquals (DataState.Added,   test.FindRecord ("b").DataState);
			Assertion.AssertEquals (DataState.Invalid, test.FindRecord ("c", DataVersion.ActiveOrDead).DataState);
		}
		
		[Test] public void CheckValidateSet()
		{
			DataSet test = new DataSet ("test");
			
			test.AddData ("a", 1,		new DataType ("name=numeric"));
			test.AddData ("b", "hello", new DataType ("name=text"));
			test.AddData ("c", 10.5,	new DataType ("name=numeric"));
			
			test.ValidateChanges ();
			
			Assertion.AssertNotNull (test.FindRecord ("a"));
			Assertion.AssertNotNull (test.FindRecord ("b"));
			Assertion.AssertNotNull (test.FindRecord ("c"));
			
			Assertion.Assert (test.FindRecord ("a").IsField);
			Assertion.Assert (test.FindRecord ("b").IsField);
			Assertion.Assert (test.FindRecord ("c").IsField);
			
			Assertion.AssertEquals (DataState.Unchanged, test.FindRecord ("a").DataState);
			Assertion.AssertEquals (DataState.Unchanged, test.FindRecord ("b").DataState);
			Assertion.AssertEquals (DataState.Unchanged, test.FindRecord ("c").DataState);
			
			test.RemoveData ("b");
			test.RemoveData ("c");
			
			test.AddData ("b", "bye", new DataType ("name=text"));
			test.AddData ("d", true,  new DataType ("name=boolean"));
			
			Assertion.AssertNotNull (test.FindRecord ("b"));
			Assertion.AssertNull (test.FindRecord ("c"));
			Assertion.AssertNotNull (test.FindRecord ("d"));
			
			Assertion.AssertEquals (DataState.Modified, test.FindRecord ("b").DataState);
			Assertion.AssertEquals (DataState.Removed,  test.FindRecord ("c", DataVersion.ActiveOrDead).DataState);
			Assertion.AssertEquals (DataState.Added,    test.FindRecord ("d").DataState);
			
			test.ValidateChanges ();
			
			Assertion.AssertEquals (DataState.Unchanged, test.FindRecord ("a").DataState);
			Assertion.AssertEquals (DataState.Unchanged, test.FindRecord ("b").DataState);
			Assertion.AssertEquals (DataState.Unchanged, test.FindRecord ("d").DataState);
			Assertion.AssertNull (test.FindRecord ("c", DataVersion.ActiveOrDead));
		}
		
		[Test] public void CheckResetDataField()
		{
			DataSet test = new DataSet ("test");
			
			test.AddData ("a", 1,		new DataType ("name=numeric"));
			test.AddData ("b", "hello", new DataType ("name=text"));
			test.AddData ("c", 10.5,	new DataType ("name=numeric"));
			
			test.ValidateChanges ();
			
			test.UpdateData ("a", 2);
			test.UpdateData ("b", "bye");
			
			Assertion.AssertEquals (DataState.Modified, test.FindRecord ("a").DataState);
			Assertion.AssertEquals (DataState.Modified, test.FindRecord ("b").DataState);
			Assertion.AssertEquals (2,     test.GetData ("a"));
			Assertion.AssertEquals ("bye", test.GetData ("b"));
			
			test.ResetData ("b");
			Assertion.AssertEquals (DataState.Unchanged, test.FindRecord ("b").DataState);
			Assertion.AssertEquals ("hello", test.GetData ("b"));
			
			test.RemoveData ("b");
			Assertion.AssertEquals (DataState.Removed, test.FindRecord ("b", DataVersion.ActiveOrDead).DataState);
			
			test.ResetData ("b");
			Assertion.AssertEquals (DataState.Unchanged, test.FindRecord ("b").DataState);
			Assertion.AssertEquals ("hello", test.GetData ("b"));
		}
		
		[Test] [ExpectedException (typeof (DataException))] public void CheckDataFieldSetDataTypeEx()
		{
			DataField test  = new DataField ();
			DataType  type1 = new DataType ("name=a");
			DataType  type2 = new DataType ("name=b");
			
			test.SetDataType (type1);
			test.SetDataType (type2);
		}
	}
}
