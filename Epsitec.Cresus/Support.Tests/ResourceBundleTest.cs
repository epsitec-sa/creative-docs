using NUnit.Framework;

namespace Epsitec.Cresus.Support.Tests
{
	[TestFixture]
	public class ResourceBundleTest
	{
		[Test] public void CheckCompile()
		{
			ResourceBundle bundle = new ResourceBundle ();
			string test_string = "<bundle><a>A</a><b>B</b></bundle>";
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
		
		[Test] public void CheckCompileRefBundle()
		{
			ResourceBundle bundle = new ResourceBundle ();
			string test_string = "<bundle><a>A</a><b><ref t='file:button.cancel'/></b></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			System.IO.MemoryStream stream = new System.IO.MemoryStream (test_data);
			bundle.Compile (stream);
			
			string[] names = bundle.FieldNames;
			
			System.Array.Sort (names);
			
			Assertion.AssertEquals (2, bundle.CountFields);
			Assertion.AssertEquals ("a", names[0]);
			Assertion.AssertEquals ("b", names[1]);
			Assertion.AssertEquals ("A", bundle["a"]);
			Assertion.Assert (bundle["b"] is ResourceBundle);
		}
		
		[Test] public void CheckCompileSubBundle()
		{
			ResourceBundle bundle = new ResourceBundle ();
			string test_string = "<bundle><a>A0</a><b><bundle><a>A1</a><b>B1</b></bundle></b></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			System.IO.MemoryStream stream = new System.IO.MemoryStream (test_data);
			bundle.Compile (stream);
			
			string[] names = bundle.FieldNames;
			
			System.Array.Sort (names);
			
			Assertion.AssertEquals (2, bundle.CountFields);
			Assertion.AssertEquals ("a", names[0]);
			Assertion.AssertEquals ("b", names[1]);
			Assertion.AssertEquals ("A0", bundle["a"]);
			Assertion.Assert (bundle["b"] is ResourceBundle);
			Assertion.AssertEquals (2, bundle.GetFieldBundle ("b").CountFields);
			Assertion.AssertEquals ("A1", bundle.GetFieldBundle ("b")["a"]);
			Assertion.AssertEquals ("B1", bundle.GetFieldBundle ("b")["b"]);
		}
		
		[Test] public void CheckCompileCDATA()
		{
			ResourceBundle bundle = new ResourceBundle ();
			string test_string = "<bundle><txt><![CDATA[Small <b>text</b> to <i>check</i> if CDATA&lt;..&gt; works.]]></txt></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			System.IO.MemoryStream stream = new System.IO.MemoryStream (test_data);
			bundle.Compile (stream);
			
			string[] names = bundle.FieldNames;
			
			Assertion.AssertEquals (1, bundle.CountFields);
			Assertion.AssertEquals ("txt", names[0]);
			Assertion.AssertEquals ("Small <b>text</b> to <i>check</i> if CDATA&lt;..&gt; works.", bundle["txt"]);
		}
		
		[Test] public void CheckCompileEscapes()
		{
			ResourceBundle bundle = new ResourceBundle ();
			string test_string = "<bundle><txt>&lt;&amp;&gt;</txt></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			System.IO.MemoryStream stream = new System.IO.MemoryStream (test_data);
			bundle.Compile (stream);
			
			string[] names = bundle.FieldNames;
			
			Assertion.AssertEquals (1, bundle.CountFields);
			Assertion.AssertEquals ("txt", names[0]);
			Assertion.AssertEquals ("<&>", bundle["txt"]);
		}
		
		[Test] public void CheckCompileAndMerge()
		{
			ResourceBundle bundle = new ResourceBundle ();
			string test_string_1 = "<bundle><a>A</a><b>B</b></bundle>";
			string test_string_2 = "<bundle><a>X</a><c>C</c></bundle>";
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
			string test_string = "<bundle><a>A<b>B</b></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			System.IO.MemoryStream stream = new System.IO.MemoryStream (test_data);
			bundle.Compile (stream);
		}
		
		[Test] public void CheckCompileRefLevel1()
		{
			ResourceBundle bundle = new ResourceBundle ();
			string test_string = "<bundle><a>A</a><ref t='file:button.cancel'/></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			System.IO.MemoryStream stream = new System.IO.MemoryStream (test_data);
			bundle.Compile (stream);
			
			string[] names = bundle.FieldNames;
			
			System.Array.Sort (names);
			
			Assertion.AssertEquals (5, bundle.CountFields);
			Assertion.AssertEquals ("a", names[0]);
			Assertion.AssertEquals ("cls", names[1]);
			Assertion.AssertEquals ("nam", names[2]);
			Assertion.AssertEquals ("siz", names[3]);
			Assertion.AssertEquals ("txt", names[4]);
		}
		
		[Test] public void CheckCompileRefLevel2()
		{
			ResourceBundle bundle = new ResourceBundle ();
			string test_string = "<bundle><a>A</a><b><ref t='file:strings#label.Hello'/></b><c><ref t='file:strings#title.MainWindow'/></c></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			System.IO.MemoryStream stream = new System.IO.MemoryStream (test_data);
			bundle.Compile (stream);
			
			string[] names = bundle.FieldNames;
			
			System.Array.Sort (names);
			
			Assertion.AssertEquals (3, bundle.CountFields);
			Assertion.AssertEquals ("a", names[0]);
			Assertion.AssertEquals ("b", names[1]);
			Assertion.AssertEquals ("c", names[2]);
			
			System.Console.Out.WriteLine ("Referenced field 'b' contains '{0}'.", bundle["b"]);
			System.Console.Out.WriteLine ("Referenced field 'c' contains '{0}'.", bundle["c"]);
		}
		
		[Test] public void CheckCompileRefLevel2AutoPrefix()
		{
			ResourceBundle bundle = new ResourceBundle ();
			string test_string = "<bundle><a>A</a><b><ref t='button.cancel#txt'/></b></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			System.IO.MemoryStream stream = new System.IO.MemoryStream (test_data);
			bundle.Compile (stream, "file", ResourceLevel.Default);
			
			string[] names = bundle.FieldNames;
			
			System.Array.Sort (names);
			
			Assertion.AssertEquals (2, bundle.CountFields);
			Assertion.AssertEquals ("a", names[0]);
			Assertion.AssertEquals ("b", names[1]);
			Assertion.AssertEquals ("Cancel", bundle["b"]);
		}
		
		[Test] [ExpectedException (typeof (ResourceException))] public void CheckCompileRefEx1()
		{
			ResourceBundle bundle = new ResourceBundle ();
			string test_string = "<bundle><a>A</a><b><ref t='button.cancel#txt'/></b></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			System.IO.MemoryStream stream = new System.IO.MemoryStream (test_data);
			bundle.Compile (stream);
		}
		
		[Test] [ExpectedException (typeof (ResourceException))] public void CheckCompileRefEx2()
		{
			ResourceBundle bundle = new ResourceBundle ();
			string test_string = "<bundle><a>A</a><b><ref t='file:does-not-exist#txt'/></b></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			System.IO.MemoryStream stream = new System.IO.MemoryStream (test_data);
			bundle.Compile (stream);
		}
		
		[Test] [ExpectedException (typeof (ResourceException))] public void CheckCompileRefEx3()
		{
			ResourceBundle bundle = new ResourceBundle ();
			string test_string = "<bundle><a>A</a><b><ref t='file:button.cancel#does-not-exist'/></b></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			System.IO.MemoryStream stream = new System.IO.MemoryStream (test_data);
			bundle.Compile (stream);
		}
	}
}
