using NUnit.Framework;

namespace Epsitec.Cresus.DataLayer.Tests
{
	[TestFixture]
	public class GeneralTest
	{
		[Test] public void CheckDataField()
		{
			DataRecord test = new DataField ();
			
			Assertion.AssertEquals (false, test.IsSet);
			Assertion.AssertEquals (true,  test.IsField);
			Assertion.AssertEquals (false, test.IsTable);
			
			Assertion.AssertNull (test.DataType);
		}
		
		[Test] public void CheckDataSet()
		{
			DataRecord test = new DataSet ();
			
			Assertion.AssertEquals (true,  test.IsSet);
			Assertion.AssertEquals (false, test.IsField);
			Assertion.AssertEquals (false, test.IsTable);
			
			Assertion.AssertNotNull (test.DataType);
			Assertion.AssertEquals (DataClass.Complex, test.DataType.DataClass);
		}
		
		[Test] public void CheckDataTable()
		{
			DataRecord test = new DataTable ();
			
			Assertion.AssertEquals (false, test.IsSet);
			Assertion.AssertEquals (false, test.IsField);
			Assertion.AssertEquals (true,  test.IsTable);
			
			Assertion.AssertNotNull (test.DataType);
			Assertion.AssertEquals (DataClass.Complex, test.DataType.DataClass);
		}
	}
}
