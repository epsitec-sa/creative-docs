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
			DataRecord test = new DataSet ();
			
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
		
		[Test] public void CheckCreateSet()
		{
			DataSet test = new DataSet ();
			
			test.AddData ("a", 1,		new DataType ("numeric"));
			test.AddData ("b", "hello", new DataType ("text"));
			test.AddData ("c", 10.5,	new DataType ("numeric"));
			
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
			
			test.AddData ("b", "bye", new DataType ("text"));
			
			Assertion.AssertNotNull (test.FindRecord ("b"));
			Assertion.AssertNull (test.FindRecord ("c"));
			
			Assertion.AssertEquals (DataState.Added,   test.FindRecord ("b").DataState);
			Assertion.AssertEquals (DataState.Invalid, test.FindRecord ("c", DataVersion.ActiveOrDead).DataState);
		}
		
		[Test] public void CheckValidateSet()
		{
			DataSet test = new DataSet ();
			
			test.AddData ("a", 1,		new DataType ("numeric"));
			test.AddData ("b", "hello", new DataType ("text"));
			test.AddData ("c", 10.5,	new DataType ("numeric"));
			
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
			
			test.AddData ("b", "bye", new DataType ("text"));
			test.AddData ("d", true,  new DataType ("boolean"));
			
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
			DataSet test = new DataSet ();
			
			test.AddData ("a", 1,		new DataType ("numeric"));
			test.AddData ("b", "hello", new DataType ("text"));
			test.AddData ("c", 10.5,	new DataType ("numeric"));
			
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
			DataField test = new DataField ();
			DataType  type = new DataType ();
			
			test.SetDataType (type);
			test.SetDataType (type);
		}
	}
}
