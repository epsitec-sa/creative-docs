using NUnit.Framework;

namespace Epsitec.Cresus.Support.Tests
{
	[TestFixture]
	public class ResourceBundleTest
	{
		[Test] public void CheckCompile()
		{
			ResourceBundle bundle = new ResourceBundle ();
			string test_string = "<BUNDLE><a>A</a><b>B</b></BUNDLE>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			System.IO.MemoryStream stream = new System.IO.MemoryStream (test_data);
			bundle.Compile (stream);
			
			string[] names = bundle.FieldNames;
			
			System.Array.Sort (names);
			
			Assertion.AssertEquals (2, bundle.CountFields);
			Assertion.AssertEquals ("a", names[0]);
			Assertion.AssertEquals ("b", names[1]);
		}
		
		[Test] public void CheckCompileAndMerge()
		{
			ResourceBundle bundle = new ResourceBundle ();
			string test_string_1 = "<BUNDLE><a>A</a><b>B</b></BUNDLE>";
			string test_string_2 = "<BUNDLE><a>X</a><c>C</c></BUNDLE>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data_1 = encoding.GetBytes (test_string_1);
			byte[] test_data_2 = encoding.GetBytes (test_string_2);
			System.IO.MemoryStream stream_1 = new System.IO.MemoryStream (test_data_1);
			System.IO.MemoryStream stream_2 = new System.IO.MemoryStream (test_data_2);
			bundle.Compile (stream_1);
			bundle.Compile (stream_2);
			
			string[] names = bundle.FieldNames;
			
			System.Array.Sort (names);
			
			Assertion.AssertEquals (3, bundle.CountFields);
			Assertion.AssertEquals ("a", names[0]);
			Assertion.AssertEquals ("b", names[1]);
			Assertion.AssertEquals ("c", names[2]);
			Assertion.AssertEquals ("X", bundle["a"]);
			Assertion.AssertEquals ("B", bundle["b"]);
			Assertion.AssertEquals ("C", bundle["c"]);
			Assertion.AssertEquals (null, bundle["d"]);
		}
		
		[Test] [ExpectedException (typeof (ResourceException))] public void CheckCompileEx1()
		{
			ResourceBundle bundle = new ResourceBundle ();
			string test_string = "<a>A</a><b>B</b>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			System.IO.MemoryStream stream = new System.IO.MemoryStream (test_data);
			bundle.Compile (stream);
		}
		
		[Test] [ExpectedException (typeof (ResourceException))] public void CheckCompileEx2()
		{
			ResourceBundle bundle = new ResourceBundle ();
			string test_string = "<BUNDLE><a>A<b>B</b></BUNDLE>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			System.IO.MemoryStream stream = new System.IO.MemoryStream (test_data);
			bundle.Compile (stream);
		}
	}
}
