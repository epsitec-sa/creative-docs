using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class ResourceBundleTest
	{
		[SetUp] public void SetUp()
		{
			Resources.SetupProviders ("test");
		}
		
		[Test] public void CheckCompile()
		{
			ResourceBundle bundle = new ResourceBundle ("test");
			string test_string = "<bundle><field name='a'>A</field><field name='b'>B</field></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
			string[] names = bundle.FieldNames;
			
			System.Array.Sort (names);
			
			Assertion.AssertEquals (2, bundle.CountFields);
			Assertion.AssertEquals ("a", names[0]);
			Assertion.AssertEquals ("b", names[1]);
		}
		
		[Test] public void CheckCompileRefBundle()
		{
			ResourceBundle bundle = new ResourceBundle ("test");
			string test_string = "<bundle><field name='a'>A</field><field name='b'><ref target='file:button.cancel'/></field></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
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
			ResourceBundle bundle = new ResourceBundle ("test");
			string test_string = "<bundle><field name='a'>A0</field><field name='b'><bundle><field name='a'>A1</field><field name='b'>B1</field></bundle></field></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
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
			Assertion.AssertEquals ("test", bundle.Name);
			Assertion.AssertEquals ("test#b", bundle.GetFieldBundle ("b").Name);
		}
		
		[Test] public void CheckCompileCDATA()
		{
			ResourceBundle bundle = new ResourceBundle ("test");
			string test_string = "<bundle><field name='text'><![CDATA[Small <b>text</b> to <i>check</i> if CDATA&lt;..&gt; works.]]></field></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
			string[] names = bundle.FieldNames;
			
			Assertion.AssertEquals (1, bundle.CountFields);
			Assertion.AssertEquals ("text", names[0]);
			Assertion.AssertEquals ("Small <b>text</b> to <i>check</i> if CDATA&lt;..&gt; works.", bundle["text"]);
		}
		
		[Test] public void CheckCompileEscapes()
		{
			ResourceBundle bundle = new ResourceBundle ("test");
			string test_string = "<bundle><field name='text'>&lt;&amp;&gt;</field></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
			string[] names = bundle.FieldNames;
			
			Assertion.AssertEquals (1, bundle.CountFields);
			Assertion.AssertEquals ("text", names[0]);
			Assertion.AssertEquals ("<&>", bundle["text"]);
		}
		
		[Test] public void CheckCompileAndMerge()
		{
			ResourceBundle bundle = new ResourceBundle ("test");
			string test_string_1 = "<bundle><field name='a'>A</field><field name='b'>B</field></bundle>";
			string test_string_2 = "<bundle><field name='a'>X</field><field name='c'>C</field></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data_1 = encoding.GetBytes (test_string_1);
			byte[] test_data_2 = encoding.GetBytes (test_string_2);
			bundle.Compile (test_data_1);
			bundle.Compile (test_data_2);
			
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
			ResourceBundle bundle = new ResourceBundle ("test");
			string test_string = "<field name='a'>A</field><field name='b'>B</field>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
		}
		
		[Test] [ExpectedException (typeof (ResourceException))] public void CheckCompileEx2()
		{
			ResourceBundle bundle = new ResourceBundle ("test");
			string test_string = "<bundle><field name='a'>A<field name='b'>B</field></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
		}
		
		[Test] public void CheckCompileRefLevel1()
		{
			ResourceBundle bundle = new ResourceBundle ("test");
			string test_string = "<bundle><field name='a'>A</field><ref target='file:button.cancel'/></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
			string[] names = bundle.FieldNames;
			
			System.Array.Sort (names);
			
			Assertion.AssertEquals (6, bundle.CountFields);
			Assertion.AssertEquals ("a",      names[0]);
			Assertion.AssertEquals ("anchor", names[1]);
			Assertion.AssertEquals ("class",  names[2]);
			Assertion.AssertEquals ("name",   names[3]);
			Assertion.AssertEquals ("size",   names[4]);
			Assertion.AssertEquals ("text",   names[5]);
		}
		
		[Test] public void CheckCompileRefLevel2()
		{
			ResourceBundle bundle = new ResourceBundle ("test");
			string test_string = "<bundle><field name='a'>A</field><field name='b'><ref target='file:strings#label.Hello'/></field><field name='c'><ref target='file:strings#title.MainWindow'/></field></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
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
			ResourceBundle bundle = new ResourceBundle ("test");
			string test_string = "<bundle><field name='a'>A</field><field name='b'><ref target='button.cancel#text'/></field></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data, "file", ResourceLevel.Default);
			
			string[] names = bundle.FieldNames;
			
			System.Array.Sort (names);
			
			Assertion.AssertEquals (2, bundle.CountFields);
			Assertion.AssertEquals ("a", names[0]);
			Assertion.AssertEquals ("b", names[1]);
			Assertion.AssertEquals ("Cancel", bundle["b"]);
		}
		
		[Test] [ExpectedException (typeof (ResourceException))] public void CheckCompileRefEx1()
		{
			ResourceBundle bundle = new ResourceBundle ("test");
			string test_string = "<bundle><field name='a'>A</field><field name='b'><ref target='button.cancel#text'/></field></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
		}
		
		[Test] [ExpectedException (typeof (ResourceException))] public void CheckCompileRefEx2()
		{
			ResourceBundle bundle = new ResourceBundle ("test");
			string test_string = "<bundle><field name='a'>A</field><field name='b'><ref target='file:does-not-exist#text'/></field></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
		}
		
		[Test] [ExpectedException (typeof (ResourceException))] public void CheckCompileRefEx3()
		{
			ResourceBundle bundle = new ResourceBundle ("test");
			string test_string = "<bundle><field name='a'>A</field><field name='b'><ref target='file:button.cancel#does-not-exist'/></field></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
		}
		
		private void CompileWithExceptionHandling(string test_string)
		{
			ResourceBundle bundle = new ResourceBundle ("test");
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			
			try
			{
				bundle.Compile (test_data);
			}
			catch (System.Exception ex)
			{
				System.Console.Out.WriteLine ("Message: " + ex.Message);
				return;
			}
			
			throw new System.Exception ("Expected exception not thrown");
		}
		
		[Test] public void CheckCompileSyntax1()
		{
			string test_string = "<bundle><field name='a'>A</field><fold name='b'>B</field></bundle>";
			this.CompileWithExceptionHandling (test_string);
		}
		
		[Test] public void CheckCompileSyntax2()
		{
			string test_string = "<bundle><field name='a'>A</field><field name='b'>B</fold></bundle>";
			this.CompileWithExceptionHandling (test_string);
		}
		
		[Test] public void CheckCompileSyntax3()
		{
			string test_string = "<bundle><field name='a'>A</field><field nome='b'>B</field></bundle>";
			this.CompileWithExceptionHandling (test_string);
		}
		
		[Test] public void CheckCompileSyntax4()
		{
			string test_string = "<bundle><field name='a'>A</field>\n<ref target='x'/></bundle>";
			this.CompileWithExceptionHandling (test_string);
		}
		
		[Test] public void CheckCompileSyntax5()
		{
			string test_string = "<bundle><field name='a'>A</field>\n<ref target='file:button.cancel' type='x'/></bundle>";
			this.CompileWithExceptionHandling (test_string);
		}
	}
}
