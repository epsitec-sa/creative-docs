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
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string = "<bundle type='String'><data name='a'>A</data><data name='b'>B</data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
			string[] names = bundle.FieldNames;
			
			System.Array.Sort (names);
			
			Assertion.AssertEquals (2, bundle.CountFields);
			Assertion.AssertEquals ("a", names[0]);
			Assertion.AssertEquals ("b", names[1]);
			Assertion.AssertEquals ("test", bundle.Name);
			Assertion.AssertEquals ("String", bundle.Type);
		}
		
		[Test] public void CheckCompileRefBundle()
		{
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string = "<bundle><data name='a'>A</data><bundle name='b'><ref target='file:button.cancel'/></bundle></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
			string[] names = bundle.FieldNames;
			
			System.Array.Sort (names);
			
			Assertion.AssertEquals (2, bundle.CountFields);
			Assertion.AssertEquals ("a", names[0]);
			Assertion.AssertEquals ("b", names[1]);
			Assertion.AssertEquals ("A", bundle["a"].AsString);
			Assertion.Assert (bundle["b"].Data is ResourceBundle);
		}
		
		[Test] public void CheckCompileSubBundle()
		{
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string = "<bundle><data name='a'>A0</data><bundle name='b'><data name='a'>A1</data><data name='b'>B1</data></bundle></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
			string[] names = bundle.FieldNames;
			
			System.Array.Sort (names);
			
			Assertion.AssertEquals (2, bundle.CountFields);
			Assertion.AssertEquals ("a", names[0]);
			Assertion.AssertEquals ("b", names[1]);
			Assertion.AssertEquals ("A0", bundle["a"].AsString);
			
			Assertion.Assert (bundle["b"].Data is ResourceBundle);
			
			Assertion.AssertEquals (2, bundle["b"].AsBundle.CountFields);
			Assertion.AssertEquals ("A1", bundle["b"].AsBundle["a"].AsString);
			Assertion.AssertEquals ("B1", bundle["b"].AsBundle["b"].AsString);
			Assertion.AssertEquals ("test", bundle.Name);
			Assertion.AssertEquals ("test#b", bundle["b"].AsBundle.Name);
		}
		
		[Test] public void CheckCompileCDATA()
		{
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string = "<bundle><data name='text'><![CDATA[Small <b>text</b> to <i>check</i> if CDATA&lt;..&gt; works.]]></data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
			string[] names = bundle.FieldNames;
			
			Assertion.AssertEquals (1, bundle.CountFields);
			Assertion.AssertEquals ("text", names[0]);
			Assertion.AssertEquals ("Small <b>text</b> to <i>check</i> if CDATA&lt;..&gt; works.", bundle["text"].AsString);
		}
		
		[Test] public void CheckCompileEmbeddedXml()
		{
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string = "<bundle><data name='text'><xml>Small <b>text</b> to <i>check</i> if embedded &lt;xml&gt; works.</xml></data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
			string[] names = bundle.FieldNames;
			
			Assertion.AssertEquals (1, bundle.CountFields);
			Assertion.AssertEquals ("text", names[0]);
			Assertion.AssertEquals ("Small <b>text</b> to <i>check</i> if embedded &lt;xml&gt; works.", bundle["text"].AsString);
		}
		
		[Test] public void CheckCompileEscapes()
		{
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string = "<bundle><data name='text'>&lt;&amp;&gt;</data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
			string[] names = bundle.FieldNames;
			
			Assertion.AssertEquals (1, bundle.CountFields);
			Assertion.AssertEquals ("text", names[0]);
			Assertion.AssertEquals ("<&>", bundle["text"].AsString);
		}
		
		[Test] public void CheckCompileAndMerge()
		{
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string_1 = "<bundle><data name='a'>A</data><data name='b'>B</data></bundle>";
			string test_string_2 = "<bundle><data name='a'>X</data><data name='c'>C</data></bundle>";
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
			Assertion.AssertEquals ("X", bundle["a"].AsString);
			Assertion.AssertEquals ("B", bundle["b"].AsString);
			Assertion.AssertEquals ("C", bundle["c"].AsString);
			Assertion.Assert (bundle["d"].IsEmpty);
		}
		
		[Test] public void CheckCompileAndNoMerge()
		{
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string_1 = "<bundle><data name='a'>A</data><data name='b'>B</data></bundle>";
			string test_string_2 = "<bundle><data name='a'>X</data><data name='c'>C</data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data_1 = encoding.GetBytes (test_string_1);
			byte[] test_data_2 = encoding.GetBytes (test_string_2);
			
			bundle.AutoMergeEnabled = false;
			
			bundle.Compile (test_data_1);
			bundle.Compile (test_data_2);
			
			string[] names = bundle.FieldNames;
			
			System.Array.Sort (names);
			
			Assertion.AssertEquals (4, bundle.CountFields);
			Assertion.AssertEquals ("a", names[0]);
			Assertion.AssertEquals ("a", names[1]);
			Assertion.AssertEquals ("b", names[2]);
			Assertion.AssertEquals ("c", names[3]);
		}
		
		[Test] [ExpectedException (typeof (System.Xml.XmlException))] public void CheckCompileEx1()
		{
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string = "<data name='a'>A</data><data name='b'>B</data>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
		}
		
		[Test] [ExpectedException (typeof (System.Xml.XmlException))] public void CheckCompileEx2()
		{
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string = "<bundle><data name='a'>A<data name='b'>B</data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
		}
		
		[Test] [ExpectedException (typeof (Support.ResourceException))] public void CheckCompileEx3()
		{
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string = "<invalid_root><data name='a'>A</data><data name='b'>B</data></invalid_root>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
		}
		
		[Test] public void CheckCompileRefLevel1()
		{
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string = "<bundle><data name='a'>A</data><ref target='file:button.cancel'/></bundle>";
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
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string = "<bundle><data name='a'>A</data><data name='b'><ref target='file:strings#label.Hello'/></data><data name='c'><ref target='file:strings#title.MainWindow'/></data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
			string[] names = bundle.FieldNames;
			
			System.Array.Sort (names);
			
			Assertion.AssertEquals (3, bundle.CountFields);
			Assertion.AssertEquals ("a", names[0]);
			Assertion.AssertEquals ("b", names[1]);
			Assertion.AssertEquals ("c", names[2]);
			
			System.Console.Out.WriteLine ("Referenced field 'b' contains '{0}'.", bundle["b"].AsString);
			System.Console.Out.WriteLine ("Referenced field 'c' contains '{0}'.", bundle["c"].AsString);
		}
		
		[Test] public void CheckCompileRefLevel2AutoPrefix()
		{
			ResourceBundle bundle = ResourceBundle.Create ("test", "file", ResourceLevel.Default, 0);
			string test_string = "<bundle><data name='a'>A</data><data name='b'><ref target='button.cancel#text'/></data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
			string[] names = bundle.FieldNames;
			
			System.Array.Sort (names);
			
			Assertion.AssertEquals (2, bundle.CountFields);
			Assertion.AssertEquals ("a", names[0]);
			Assertion.AssertEquals ("b", names[1]);
			Assertion.AssertEquals ("Cancel", bundle["b"].AsString);
		}
		
		[Test] [ExpectedException (typeof (ResourceException))] public void CheckCompileRefEx1()
		{
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string = "<bundle><data name='a'>A</data><data name='b'><ref target='button.cancel#text'/></data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
			string value_a = bundle["a"].AsString;
			string value_b = bundle["b"].AsString;
		}
		
		[Test] [ExpectedException (typeof (ResourceException))] public void CheckCompileRefEx2()
		{
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string = "<bundle><data name='a'>A</data><data name='b'><ref target='file:does-not-exist#text'/></data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
			string value_a = bundle["a"].AsString;
			string value_b = bundle["b"].AsString;
		}
		
		[Test] [ExpectedException (typeof (ResourceException))] public void CheckCompileRefEx3()
		{
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string = "<bundle><data name='a'>A</data><data name='b'><ref target='file:button.cancel#does-not-exist'/></data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
			string value_a = bundle["a"].AsString;
			string value_b = bundle["b"].AsString;
		}
		
		private void CompileWithExceptionHandling(string test_string)
		{
			ResourceBundle bundle = ResourceBundle.Create ("test");
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
			string test_string = "<bundle><data name='a'>A</data><d*ta name='b'>B</data></bundle>";
			this.CompileWithExceptionHandling (test_string);
		}
		
		[Test] public void CheckCompileSyntax2()
		{
			string test_string = "<bundle><data name='a'>A</data><data name='b'>B</d*ta></bundle>";
			this.CompileWithExceptionHandling (test_string);
		}
		
		[Test] public void CheckCompileSyntax3()
		{
			string test_string = "<bundle><data name='a'>A</data><data nome='b'>B</data></bundle>";
			this.CompileWithExceptionHandling (test_string);
		}
		
		[Test] public void CheckCompileSyntax4()
		{
			string test_string = "<bundle><data name='a'>A</data>\n<ref target='x'/></bundle>";
			this.CompileWithExceptionHandling (test_string);
		}
		
		[Test] public void CheckCompileSyntax5()
		{
			string test_string = "<bundle><data name='a'>A</data>\n<ref target='file:button.cancel' type='x'/></bundle>";
			this.CompileWithExceptionHandling (test_string);
		}
		
		[Test] public void CheckCreateXmlNode1()
		{
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string =
				"<bundle name='test'>\r\n" +
				"  <data name='a'>Aà</data>\r\n" +
				"  <bundle name='b'>\r\n" +
				"    <data name='a'>B:A</data>\r\n" +
				"    <data name='b'>B:B</data>\r\n" +
				"  </bundle>\r\n" +
				"</bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			
			bundle.Compile (test_data);
			
			byte[] live_data = bundle.CreateXmlAsData ();
			
			ResourceBundleTest.XmlDumpIfDifferent (test_data, live_data, bundle);
			Assertion.Assert ("Serialised data not equal to source data", ResourceBundleTest.XmlTestEqual (test_data, live_data));
		}
		
		[Test] public void CheckCreateXmlNode2()
		{
			ResourceBundle bundle = ResourceBundle.Create ();
			string test_string = 
				"<bundle name=\"&quot;test&quot;\" type='String'>\r\n" +
				"  <data name='a'>\r\n" +
				"    <ref target='strings#label.OK' />\r\n" +
				"  </data>\r\n" +
				"</bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			
			bundle.Compile (test_data);
			
			Assertion.AssertEquals ("\"test\"", bundle.Name);
			Assertion.AssertEquals ("String", bundle.Type);
			
			byte[] live_data = bundle.CreateXmlAsData ();
			
			ResourceBundleTest.XmlDumpIfDifferent (test_data, live_data, bundle);
			Assertion.Assert ("Serialised data not equal to source data", ResourceBundleTest.XmlTestEqual (test_data, live_data));
			
			System.Console.Out.WriteLine (bundle[0].Xml.InnerXml);
		}
		
		[Test] public void CheckCreateXmlNode3()
		{
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string = 
				"<bundle name='test'>\r\n" +
				"  <ref target='file:button.cancel' />\r\n" +
				"</bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			
			bundle.RefInclusionEnabled = false;
			
			bundle.Compile (test_data);
			
			byte[] live_data = bundle.CreateXmlAsData ();
			
			ResourceBundleTest.XmlDumpIfDifferent (test_data, live_data, bundle);
			
			Assertion.Assert ("Serialised data not equal to source data", ResourceBundleTest.XmlTestEqual (test_data, live_data));
			Assertion.Assert ("Field not recognized as <ref> tag.", bundle[0].IsRef);
			
			System.Console.Out.WriteLine (bundle[0].Xml.OuterXml);
		}
		
		[Test] public void CheckCreateXmlNode4()
		{
			ResourceBundle bundle = ResourceBundle.Create ();
			
			string test_string = 
				"<bundle name='test' type='String'>\r\n" +
				"  <data name='a'>A</data>\r\n" +
				"  <data name='b'>\r\n" +
				"    <xml>\r\n" + 
				"      <b>B  B</b>\r\n" +
				"    </xml>\r\n" +
				"  </data>\r\n" +
				"</bundle>";
			
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			
			ResourceBundle.Field field_1 = bundle.CreateEmptyField (ResourceFieldType.Data);
			ResourceBundle.Field field_2 = bundle.CreateEmptyField (ResourceFieldType.Data);
			
			field_1.SetName ("a");
			field_1.SetStringValue ("A");
			
			field_2.SetName ("b");
			field_2.SetStringValue ("<b>B  B</b>");
			
			bundle.Add (field_1);
			bundle.Add (field_2);
			
			bundle.DefineName ("test");
			bundle.DefineType ("String");
			
			byte[] live_data = bundle.CreateXmlAsData ();
			
			ResourceBundleTest.XmlDumpIfDifferent (test_data, live_data, bundle);
			Assertion.Assert ("Serialised data not equal to source data", ResourceBundleTest.XmlTestEqual (test_data, live_data));
			
			bundle = ResourceBundle.Create ();
			bundle.Compile (test_data);
			
			Assertion.AssertEquals ("A", bundle[0].AsString);
			Assertion.AssertEquals ("<b>B  B</b>", bundle[1].AsString);
		}
		
		
		
		static void XmlDumpIfDifferent(byte[] a, byte[] b, ResourceBundle bundle)
		{
			if (ResourceBundleTest.XmlTestEqual (a, b) == false)
			{
				bundle.CreateXmlDocument (false).Save (System.Console.Out);
				System.Console.Out.WriteLine ();
				System.Console.Out.WriteLine ("Bundle original XML source uses {0} bytes:", a.Length);
				ResourceBundleTest.XmlDumpData (a);
				System.Console.Out.WriteLine ("Bundle produced XML source uses {0} bytes:", b.Length);
				ResourceBundleTest.XmlDumpData (b);
			}
		}
		
		static bool XmlTestEqual(byte[] a, byte[] b)
		{
			if (a.Length != b.Length)
			{
				return false;
			}
			
			for (int i = 0; i < a.Length; i++)
			{
				byte x = a[i];
				byte y = b[i];
				
				if (x == '\'') x = (byte) '\"';
				if (y == '\'') y = (byte) '\"';
				
				if (x != y)
				{
					return false;
				}
			}
			
			return true;
		}
		
		static void XmlDumpData(byte[] data)
		{
			for (int i = 0; i < data.Length; i++)
			{
				if ((i > 0) && ((i % 20) == 0)) System.Console.Out.WriteLine ();
				System.Console.Out.Write ("{0,3:x2}", data[i]);
			}
			
			System.Console.Out.WriteLine ();
		}
	}
}
