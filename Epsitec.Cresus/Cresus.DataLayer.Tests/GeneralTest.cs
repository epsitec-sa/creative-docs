using NUnit.Framework;

namespace Epsitec.Cresus.DataLayer.Tests
{
	[TestFixture]
	public class GeneralTest
	{
#if false
		[Test] public void CheckCreateSet()
		{
			DataSet test = new DataSet ("test");
			
			test.AddData ("a", 1,		new DataType ("name=numeric"));
			test.AddData ("b", "hello", new DataType ("name=text"));
			test.AddData ("c", 10.5,	new DataType ("name=numeric"));
			
			Assertion.AssertNotNull (test.FindRecord ("a"));
			Assertion.AssertNotNull (test.FindRecord ("b"));
			Assertion.AssertNotNull (test.FindRecord ("c"));
			
			Assertion.Assert (test.FindRecord ("a") is DataField);
			Assertion.Assert (test.FindRecord ("b") is DataField);
			Assertion.Assert (test.FindRecord ("c") is DataField);
			
			Assertion.AssertEquals (DataState.Added, test.FindRecord ("a").State);
			Assertion.AssertEquals (DataState.Added, test.FindRecord ("b").State);
			Assertion.AssertEquals (DataState.Added, test.FindRecord ("c").State);
			
			test.RemoveData ("b");
			test.RemoveData ("c");
			
			test.AddData ("b", "bye", new DataType ("name=text"));
			
			Assertion.AssertNotNull (test.FindRecord ("b"));
			Assertion.AssertNull (test.FindRecord ("c"));
			
			Assertion.AssertEquals (DataState.Added,   test.FindRecord ("b").State);
			Assertion.AssertEquals (DataState.Invalid, test.FindRecord ("c", DataVersion.ActiveOrDead).State);
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
			
			Assertion.Assert (test.FindRecord ("a") is DataField);
			Assertion.Assert (test.FindRecord ("b") is DataField);
			Assertion.Assert (test.FindRecord ("c") is DataField);
			
			Assertion.AssertEquals (DataState.Unchanged, test.FindRecord ("a").State);
			Assertion.AssertEquals (DataState.Unchanged, test.FindRecord ("b").State);
			Assertion.AssertEquals (DataState.Unchanged, test.FindRecord ("c").State);
			
			test.RemoveData ("b");
			test.RemoveData ("c");
			
			test.AddData ("b", "bye", new DataType ("name=text"));
			test.AddData ("d", true,  new DataType ("name=boolean"));
			
			Assertion.AssertNotNull (test.FindRecord ("b"));
			Assertion.AssertNull (test.FindRecord ("c"));
			Assertion.AssertNotNull (test.FindRecord ("d"));
			
			Assertion.AssertEquals (DataState.Modified, test.FindRecord ("b").State);
			Assertion.AssertEquals (DataState.Removed,  test.FindRecord ("c", DataVersion.ActiveOrDead).State);
			Assertion.AssertEquals (DataState.Added,    test.FindRecord ("d").State);
			
			test.ValidateChanges ();
			
			Assertion.AssertEquals (DataState.Unchanged, test.FindRecord ("a").State);
			Assertion.AssertEquals (DataState.Unchanged, test.FindRecord ("b").State);
			Assertion.AssertEquals (DataState.Unchanged, test.FindRecord ("d").State);
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
			
			Assertion.AssertEquals (DataState.Modified, test.FindRecord ("a").State);
			Assertion.AssertEquals (DataState.Modified, test.FindRecord ("b").State);
			Assertion.AssertEquals (2,     test.GetData ("a"));
			Assertion.AssertEquals ("bye", test.GetData ("b"));
			
			test.ResetData ("b");
			Assertion.AssertEquals (DataState.Unchanged, test.FindRecord ("b").State);
			Assertion.AssertEquals ("hello", test.GetData ("b"));
			
			test.RemoveData ("b");
			Assertion.AssertEquals (DataState.Removed, test.FindRecord ("b", DataVersion.ActiveOrDead).State);
			
			test.ResetData ("b");
			Assertion.AssertEquals (DataState.Unchanged, test.FindRecord ("b").State);
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
#endif
	}
}
