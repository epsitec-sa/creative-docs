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
			Assertion.AssertEquals (DataClass.Complex, test.DataType.DataClass);
		}
		
		[Test] public void CheckDataTable()
		{
			DataRecord test = new DataTable ();
			
			Assertion.AssertEquals (false, test.IsField);
			Assertion.AssertEquals (false, test.IsSet);
			Assertion.AssertEquals (true,  test.IsTable);
			
			Assertion.AssertNotNull (test.DataType);
			Assertion.AssertEquals (DataClass.Complex, test.DataType.DataClass);
		}
		
		[Test] public void CheckCreateSet()
		{
			DataSet test = new DataSet ();
			
			test.AddData ("a", 1);
			test.AddData ("b", "hello");
			test.AddData ("c", 10.5);
			
			Assertion.AssertNotNull (test.FindRecord ("a"));
			Assertion.AssertNotNull (test.FindRecord ("b"));
			Assertion.AssertNotNull (test.FindRecord ("c"));
			
			Assertion.Assert (test.FindRecord ("a").IsField);
			Assertion.Assert (test.FindRecord ("b").IsField);
			Assertion.Assert (test.FindRecord ("c").IsField);
			
			Assertion.AssertEquals (DataState.Added, test.FindRecord ("a").DataState);
			Assertion.AssertEquals (DataState.Added, test.FindRecord ("b").DataState);
			Assertion.AssertEquals (DataState.Added, test.FindRecord ("c").DataState);
		}
	}
}
