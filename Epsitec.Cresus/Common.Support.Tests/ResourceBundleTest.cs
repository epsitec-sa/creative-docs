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
			
			Assert.AreEqual (2, bundle.CountFields);
			Assert.AreEqual ("a", names[0]);
			Assert.AreEqual ("b", names[1]);
			Assert.AreEqual ("test", bundle.Name);
			Assert.AreEqual ("String", bundle.Type);
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
			
			Assert.AreEqual (2, bundle.CountFields);
			Assert.AreEqual ("a", names[0]);
			Assert.AreEqual ("b", names[1]);
			Assert.AreEqual ("A", bundle["a"].AsString);
			Assert.IsTrue (bundle["b"].Data is ResourceBundle);
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
			
			Assert.AreEqual (2, bundle.CountFields);
			Assert.AreEqual ("a", names[0]);
			Assert.AreEqual ("b", names[1]);
			Assert.AreEqual ("A0", bundle["a"].AsString);
			
			Assert.IsTrue (bundle["b"].Data is ResourceBundle);
			
			Assert.AreEqual (2, bundle["b"].AsBundle.CountFields);
			Assert.AreEqual ("A1", bundle["b"].AsBundle["a"].AsString);
			Assert.AreEqual ("B1", bundle["b"].AsBundle["b"].AsString);
			Assert.AreEqual ("test", bundle.Name);
			Assert.AreEqual ("test#b", bundle["b"].AsBundle.Name);
		}
		
		[Test] public void CheckCompileCDATA()
		{
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string = "<bundle><data name='text'><![CDATA[Small <b>text</b> to <i>check</i> if CDATA&lt;..&gt; works.]]></data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
			string[] names = bundle.FieldNames;
			
			Assert.AreEqual (1, bundle.CountFields);
			Assert.AreEqual ("text", names[0]);
			Assert.AreEqual ("Small <b>text</b> to <i>check</i> if CDATA&lt;..&gt; works.", bundle["text"].AsString);
		}
		
		[Test] public void CheckCompileEmbeddedXml()
		{
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string = "<bundle><data name='text'><xml>Small <b>text</b> to <i>check</i> if embedded &lt;xml&gt; works.</xml></data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
			string[] names = bundle.FieldNames;
			
			Assert.AreEqual (1, bundle.CountFields);
			Assert.AreEqual ("text", names[0]);
			Assert.AreEqual ("Small <b>text</b> to <i>check</i> if embedded &lt;xml&gt; works.", bundle["text"].AsString);
		}
		
		[Test] public void CheckCompileEscapes()
		{
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string = "<bundle><data name='text'>&lt;&amp;&gt;</data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
			string[] names = bundle.FieldNames;
			
			Assert.AreEqual (1, bundle.CountFields);
			Assert.AreEqual ("text", names[0]);
			Assert.AreEqual ("<&>", bundle["text"].AsString);
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
			
			Assert.AreEqual (3, bundle.CountFields);
			Assert.AreEqual ("a", names[0]);
			Assert.AreEqual ("b", names[1]);
			Assert.AreEqual ("c", names[2]);
			Assert.AreEqual ("X", bundle["a"].AsString);
			Assert.AreEqual ("B", bundle["b"].AsString);
			Assert.AreEqual ("C", bundle["c"].AsString);
			Assert.IsTrue (bundle["d"].IsEmpty);
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
			
			Assert.AreEqual (4, bundle.CountFields);
			Assert.AreEqual ("a", names[0]);
			Assert.AreEqual ("a", names[1]);
			Assert.AreEqual ("b", names[2]);
			Assert.AreEqual ("c", names[3]);
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
			
			Assert.AreEqual (5, bundle.CountFields);
			Assert.AreEqual ("a",      names[0]);
			Assert.AreEqual ("class",  names[1]);
			Assert.AreEqual ("Name",   names[2]);
			Assert.AreEqual ("Size",   names[3]);
			Assert.AreEqual ("Text",   names[4]);
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
			
			Assert.AreEqual (3, bundle.CountFields);
			Assert.AreEqual ("a", names[0]);
			Assert.AreEqual ("b", names[1]);
			Assert.AreEqual ("c", names[2]);
			
			System.Console.Out.WriteLine ("Referenced field 'b' contains '{0}'.", bundle["b"].AsString);
			System.Console.Out.WriteLine ("Referenced field 'c' contains '{0}'.", bundle["c"].AsString);
		}
		
		[Test] public void CheckCompileRefLevel2AutoPrefix()
		{
			ResourceBundle bundle = ResourceBundle.Create ("file", "test", ResourceLevel.Default, 0);
			string test_string = "<bundle><data name='a'>A</data><data name='b'><ref target='button.cancel#Text'/></data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
			string[] names = bundle.FieldNames;
			
			System.Array.Sort (names);
			
			Assert.AreEqual (2, bundle.CountFields);
			Assert.AreEqual ("a", names[0]);
			Assert.AreEqual ("b", names[1]);
			Assert.AreEqual ("Cancel", bundle["b"].AsString);
		}
		
		[Test] [ExpectedException (typeof (ResourceException))] public void CheckCompileRefEx1()
		{
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string = "<bundle><data name='a'>A</data><data name='b'><ref target='button.cancel#Text'/></data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
			
			string value_a = bundle["a"].AsString;
			string value_b = bundle["b"].AsString;
		}
		
		[Test] [ExpectedException (typeof (ResourceException))] public void CheckCompileRefEx2()
		{
			ResourceBundle bundle = ResourceBundle.Create ("test");
			string test_string = "<bundle><data name='a'>A</data><data name='b'><ref target='file:does-not-exist#Text'/></data></bundle>";
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
			Assert.IsTrue (ResourceBundleTest.XmlTestEqual (test_data, live_data), "Serialized data not equal to source data");
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
			
			Assert.AreEqual ("\"test\"", bundle.Name);
			Assert.AreEqual ("String", bundle.Type);
			
			byte[] live_data = bundle.CreateXmlAsData ();
			
			ResourceBundleTest.XmlDumpIfDifferent (test_data, live_data, bundle);
			Assert.IsTrue (ResourceBundleTest.XmlTestEqual (test_data, live_data), "Serialized data not equal to source data");
			
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
			
			Assert.IsTrue (ResourceBundleTest.XmlTestEqual (test_data, live_data), "Serialized data not equal to source data");
			Assert.IsTrue (bundle[0].IsRef, "Field not recognized as <ref> tag.");
			
			System.Console.Out.WriteLine (bundle[0].Xml.OuterXml);
		}
		
		[Test] public void CheckCreateXmlNode4()
		{
			ResourceBundle bundle = ResourceBundle.Create ();
			
			string test_string = 
				"<bundle name='test' type='String' about='Simple description...'>\r\n" +
				"  <data name='a'>A</data>\r\n" +
				"  <data name='b'>\r\n" +
				"    <xml>\r\n" + 
				"      <b>B  B</b>\r\n" +
				"    </xml>\r\n" +
				"  </data>\r\n" +
				"</bundle>";
			
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			
			ResourceBundle.Field field_1 = bundle.CreateField (ResourceFieldType.Data);
			ResourceBundle.Field field_2 = bundle.CreateField (ResourceFieldType.Data);
			
			field_1.SetName ("a");
			field_1.SetStringValue ("A");
			
			field_2.SetName ("b");
			field_2.SetStringValue ("<b>B  B</b>");
			
			bundle.Add (field_1);
			bundle.Add (field_2);
			
			bundle.DefineName ("test");
			bundle.DefineType ("String");
			bundle.DefineAbout ("Simple description...");
			
			byte[] live_data = bundle.CreateXmlAsData ();
			
			ResourceBundleTest.XmlDumpIfDifferent (test_data, live_data, bundle);
			Assert.IsTrue (ResourceBundleTest.XmlTestEqual (test_data, live_data), "Serialized data not equal to source data");
			
			bundle = ResourceBundle.Create ();
			bundle.Compile (test_data);
			
			Assert.AreEqual ("A", bundle[0].AsString);
			Assert.AreEqual ("<b>B  B</b>", bundle[1].AsString);
			Assert.AreEqual ("Simple description...", bundle.About);
		}
		
		[Test] public void CheckSortName()
		{
			string x1 = ResourceBundle.CreateSortName ("abc",  0, 1);
			string x2 = ResourceBundle.CreateSortName ("abc",  5, 2);
			string x3 = ResourceBundle.CreateSortName ("abc", 54, 2);
			
			Assert.AreEqual ("0/abc",  x1);
			Assert.AreEqual ("05/abc", x2);
			Assert.AreEqual ("54/abc", x3);
			
			Assert.AreEqual ("abc", ResourceBundle.ExtractSortName (x1));
			Assert.AreEqual ("abc", ResourceBundle.ExtractSortName (x2));
			Assert.AreEqual ("abc", ResourceBundle.ExtractSortName (x3));
		}
		
		[Test] [ExpectedException (typeof (ResourceException))] public void CheckSortNameEx1()
		{
			string x1 = ResourceBundle.CreateSortName ("abc",  10, 1);
		}
		
		[Test] [ExpectedException (typeof (ResourceException))] public void CheckSortNameEx2()
		{
			ResourceBundle.ExtractSortName ("abc");
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
