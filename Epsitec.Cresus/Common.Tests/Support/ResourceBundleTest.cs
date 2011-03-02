//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;
using System.Collections.Generic;
using Epsitec.Common.Support;

namespace Epsitec.Common.Tests.Support
{
	[TestFixture]
	public class ResourceBundleTest
	{
		[SetUp]
		public void SetUp()
		{
			Resources.DefaultManager.DefineDefaultModuleName ("LowLevelTest");
		}

		[Test]
		public void CheckCompile()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "test");
			string test_string = "<bundle type='String'><data name='a'>A</data><data name='b'>B</data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);

			string[] names = bundle.FieldNames;

			System.Array.Sort (names);

			Assert.AreEqual (2, bundle.FieldCount);
			Assert.AreEqual ("a", names[0]);
			Assert.AreEqual ("b", names[1]);
			Assert.AreEqual ("test", bundle.Name);
			Assert.AreEqual ("String", bundle.Type);
		}

		[Test]
		public void CheckCompileRefBundle()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "test");
			string test_string = "<bundle><data name='a'>A</data><bundle name='b'><ref target='file:button.cancel'/></bundle></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);

			string[] names = bundle.FieldNames;

			System.Array.Sort (names);

			Assert.AreEqual (2, bundle.FieldCount);
			Assert.AreEqual ("a", names[0]);
			Assert.AreEqual ("b", names[1]);
			Assert.AreEqual ("A", bundle["a"].AsString);
			Assert.IsTrue (bundle["b"].Data is ResourceBundle);
		}

		[Test]
		public void CheckCompileSubBundle()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "test");
			string test_string = "<bundle><data name='a'>A0</data><bundle name='b'><data name='a'>A1</data><data name='b'>B1</data></bundle></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);

			string[] names = bundle.FieldNames;

			System.Array.Sort (names);

			Assert.AreEqual (2, bundle.FieldCount);
			Assert.AreEqual ("a", names[0]);
			Assert.AreEqual ("b", names[1]);
			Assert.AreEqual ("A0", bundle["a"].AsString);

			Assert.IsTrue (bundle["b"].Data is ResourceBundle);

			Assert.AreEqual (2, bundle["b"].AsBundle.FieldCount);
			Assert.AreEqual ("A1", bundle["b"].AsBundle["a"].AsString);
			Assert.AreEqual ("B1", bundle["b"].AsBundle["b"].AsString);
			Assert.AreEqual ("test", bundle.Name);
			Assert.AreEqual ("b", bundle["b"].AsBundle.Name);
		}

		[Test]
		public void CheckCompileCDATA()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "test");
			string test_string = "<bundle><data name='text'><![CDATA[Small <b>text</b> to <i>check</i> if CDATA&lt;..&gt; works.]]></data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);

			string[] names = bundle.FieldNames;

			Assert.AreEqual (1, bundle.FieldCount);
			Assert.AreEqual ("text", names[0]);
			Assert.AreEqual ("Small <b>text</b> to <i>check</i> if CDATA&lt;..&gt; works.", bundle["text"].AsString);
		}

		[Test]
		public void CheckCompileEmbeddedXml()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "test");
			string test_string = "<bundle><data name='text'><xml>Small <b>text</b> to <i>check</i> if embedded &lt;xml&gt; works.</xml></data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);

			string[] names = bundle.FieldNames;

			Assert.AreEqual (1, bundle.FieldCount);
			Assert.AreEqual ("text", names[0]);
			Assert.AreEqual ("Small <b>text</b> to <i>check</i> if embedded &lt;xml&gt; works.", bundle["text"].AsString);
		}

		[Test]
		public void CheckCompileEscapes()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "test");
			string test_string = "<bundle><data name='text'>&lt;&amp;&gt;</data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);

			string[] names = bundle.FieldNames;

			Assert.AreEqual (1, bundle.FieldCount);
			Assert.AreEqual ("text", names[0]);
			Assert.AreEqual ("<&>", bundle["text"].AsString);
		}

		[Test]
		public void CheckCompileAndMerge()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "test");
			string test_string_1 = "<bundle><data name='a'>A</data><data name='b'>B</data></bundle>";
			string test_string_2 = "<bundle><data name='a'>X</data><data name='c'>C</data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data_1 = encoding.GetBytes (test_string_1);
			byte[] test_data_2 = encoding.GetBytes (test_string_2);
			bundle.Compile (test_data_1);
			bundle.Compile (test_data_2);

			string[] names = bundle.FieldNames;

			System.Array.Sort (names);

			Assert.AreEqual (3, bundle.FieldCount);
			Assert.AreEqual ("a", names[0]);
			Assert.AreEqual ("b", names[1]);
			Assert.AreEqual ("c", names[2]);
			Assert.AreEqual ("X", bundle["a"].AsString);
			Assert.AreEqual ("B", bundle["b"].AsString);
			Assert.AreEqual ("C", bundle["c"].AsString);
			Assert.IsTrue (bundle["d"].IsEmpty);
		}

		[Test]
		public void CheckCompileAndNoMerge()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "test");
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

			Assert.AreEqual (4, bundle.FieldCount);
			Assert.AreEqual ("a", names[0]);
			Assert.AreEqual ("a", names[1]);
			Assert.AreEqual ("b", names[2]);
			Assert.AreEqual ("c", names[3]);
		}

		[Test]
		[ExpectedException (typeof (ResourceException))]
		public void CheckCompileEx1()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "test");
			string test_string = "<data name='a'>A</data><data name='b'>B</data>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
		}

		[Test]
		[ExpectedException (typeof (System.Xml.XmlException))]
		public void CheckCompileEx2()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "test");
			string test_string = "<bundle><data name='a'>A<data name='b'>B</data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
		}

		[Test]
		[ExpectedException (typeof (ResourceException))]
		public void CheckCompileEx3()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "test");
			string test_string = "<invalid_root><data name='a'>A</data><data name='b'>B</data></invalid_root>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);
		}

		[Test]
		public void CheckCompileRefLevel1()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "test");
			string test_string = @"<bundle><data name=""aaa"">A</data><ref target=""file:button.cancel""/></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);

			string[] names = bundle.FieldNames;

			System.Array.Sort (names);

			Assert.AreEqual (6, bundle.FieldCount);
			Assert.AreEqual ("aaa", names[0]);
			Assert.AreEqual ("Anchor", names[1]);
			Assert.AreEqual ("class", names[2]);
			Assert.AreEqual ("Name", names[3]);
			Assert.AreEqual ("Size", names[4]);
			Assert.AreEqual ("Text", names[5]);
		}

		[Test]
		public void CheckCompileRefLevel2()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "test");
			string test_string = "<bundle><data name='a'>A</data><data name='b'><ref target='file:strings#label.Hello'/></data><data name='c'><ref target='file:strings#title.MainWindow'/></data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);

			string[] names = bundle.FieldNames;

			System.Array.Sort (names);

			Assert.AreEqual (3, bundle.FieldCount);
			Assert.AreEqual ("a", names[0]);
			Assert.AreEqual ("b", names[1]);
			Assert.AreEqual ("c", names[2]);

			System.Console.Out.WriteLine ("Referenced field 'b' contains '{0}'.", bundle["b"].AsString);
			System.Console.Out.WriteLine ("Referenced field 'c' contains '{0}'.", bundle["c"].AsString);
		}

		[Test]
		public void CheckCompileRefLevel2AutoPrefix()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "file", "test", ResourceLevel.Default, 0);
			string test_string = "<bundle><data name='a'>A</data><data name='b'><ref target='button.cancel#Text'/></data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);

			string[] names = bundle.FieldNames;

			System.Array.Sort (names);

			Assert.AreEqual (2, bundle.FieldCount);
			Assert.AreEqual ("a", names[0]);
			Assert.AreEqual ("b", names[1]);
			Assert.AreEqual ("Cancel", bundle["b"].AsString);
		}

		[Test]
		[ExpectedException (typeof (ResourceException))]
		public void CheckCompileRefEx1()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "test");
			string test_string = "<bundle><data name='a'>A</data><data name='b'><ref target='button.cancel#Text'/></data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);

			string value_a = bundle["a"].AsString;
			string value_b = bundle["b"].AsString;
		}

		[Test]
		[ExpectedException (typeof (ResourceException))]
		public void CheckCompileRefEx2()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "test");
			string test_string = "<bundle><data name='a'>A</data><data name='b'><ref target='file:does-not-exist#Text'/></data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);

			string value_a = bundle["a"].AsString;
			string value_b = bundle["b"].AsString;
		}

		[Test]
		[ExpectedException (typeof (ResourceException))]
		public void CheckCompileRefEx3()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "test");
			string test_string = "<bundle><data name='a'>A</data><data name='b'><ref target='file:button.cancel#does-not-exist'/></data></bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);
			bundle.Compile (test_data);

			string value_a = bundle["a"].AsString;
			string value_b = bundle["b"].AsString;
		}

		private void CompileWithExceptionHandling(string test_string)
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "test");
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

		[Test]
		public void CheckCompileSyntax1()
		{
			string test_string = "<bundle><data name='a'>A</data><d*ta name='b'>B</data></bundle>";
			this.CompileWithExceptionHandling (test_string);
		}

		[Test]
		public void CheckCompileSyntax2()
		{
			string test_string = "<bundle><data name='a'>A</data><data name='b'>B</d*ta></bundle>";
			this.CompileWithExceptionHandling (test_string);
		}

		[Test]
		[Ignore ("Broken syntax checking")]
		public void CheckCompileSyntax3()
		{
			string test_string = "<bundle><data name='a'>A</data><data nome='b'>B</data></bundle>";
			this.CompileWithExceptionHandling (test_string);
		}

		[Test]
		public void CheckCompileSyntax4()
		{
			string test_string = "<bundle><data name='a'>A</data>\n<ref target='x'/></bundle>";
			this.CompileWithExceptionHandling (test_string);
		}

		[Test]
		public void CheckCompileSyntax5()
		{
			string test_string = "<bundle><data name='a'>A</data>\n<ref target='file:button.cancel' type='x'/></bundle>";
			this.CompileWithExceptionHandling (test_string);
		}

		[Test]
		public void CheckCreateXmlNode1()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "test");
			string test_string =
				"<?xml version='1.0' encoding='utf-8'?>\r\n" +
				"<bundle name='test' culture='fr'>\r\n" +
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

		[Test]
		public void CheckCreateXmlNode2()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager);
			string test_string = 
				"<?xml version='1.0' encoding='utf-8'?>\r\n" +
				"<bundle name=\"test\" type='String' culture='fr'>\r\n" +
				"  <data name='a'>\r\n" +
				"    <ref target='strings#label.OK' />\r\n" +
				"  </data>\r\n" +
				"</bundle>";
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);

			bundle.Compile (test_data);

			Assert.AreEqual ("test", bundle.Name);
			Assert.AreEqual ("String", bundle.Type);

			byte[] live_data = bundle.CreateXmlAsData ();

			ResourceBundleTest.XmlDumpIfDifferent (test_data, live_data, bundle);
			Assert.IsTrue (ResourceBundleTest.XmlTestEqual (test_data, live_data), "Serialized data not equal to source data");

			System.Console.Out.WriteLine (bundle[0].Xml.InnerXml);
		}

		[Test]
		public void CheckCreateXmlNode3()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "test");
			string test_string = 
				"<?xml version='1.0' encoding='utf-8'?>\r\n" +
				"<bundle name='test' culture='fr'>\r\n" +
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

		[Test]
		public void CheckCreateXmlNode4()
		{
			ResourceBundle bundle = ResourceBundle.Create (Resources.DefaultManager, "file", "test", ResourceLevel.Default, Resources.FindCultureInfo ("fr"));

			string test_string = 
				"<?xml version='1.0' encoding='utf-8'?>\r\n" +
				"<bundle name='test' type='String' about='Simple description...' culture='fr'>\r\n" +
				"  <data name='a'>A</data>\r\n" +
				"  <data name='b'>&lt;b&gt;B  B&lt;/b&gt;</data>\r\n" +
				"  <data name='c'>\r\n" +
				"    <xml>\r\n" +
				"      <c id='1'>\r\n" + 
				"        <d />   x   \n   a</c>\r\n" +
				"    </xml>\r\n" +
				"  </data>\r\n" +
				"</bundle>";

			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			byte[] test_data = encoding.GetBytes (test_string);

			ResourceBundle.Field field_1 = bundle.CreateField (ResourceFieldType.Data);
			ResourceBundle.Field field_2 = bundle.CreateField (ResourceFieldType.Data);
			ResourceBundle.Field field_3 = bundle.CreateField (ResourceFieldType.Data);

			field_1.SetName ("a");
			field_1.SetStringValue ("A");

			field_2.SetName ("b");
			field_2.SetStringValue ("<b>B  B</b>");

			field_3.SetName ("c");
			field_3.SetXmlValue ("<c id=\"1\"><d />   x   \n   a</c>");

			bundle.Add (field_1);
			bundle.Add (field_2);
			bundle.Add (field_3);

			bundle.DefineName ("test");
			bundle.DefineType ("String");
			bundle.DefineAbout ("Simple description...");

			byte[] live_data = bundle.CreateXmlAsData ();

			ResourceBundleTest.XmlDumpIfDifferent (test_data, live_data, bundle);
			Assert.IsTrue (ResourceBundleTest.XmlTestEqual (test_data, live_data), "Serialized data not equal to source data");

			bundle = ResourceBundle.Create (Resources.DefaultManager);
			bundle.Compile (test_data);

			Assert.AreEqual ("A", bundle[0].AsString);
			Assert.AreEqual ("<b>B  B</b>", bundle[1].AsString);
			Assert.AreEqual ("<c id=\"1\"><d />   x   \n   a</c>", bundle[2].AsString);
			Assert.AreEqual ("Simple description...", bundle.About);
		}


		static void XmlDumpIfDifferent(byte[] a, byte[] b, ResourceBundle bundle)
		{
			a = ResourceBundleTest.RemoveUtf8Header (a);
			b = ResourceBundleTest.RemoveUtf8Header (b);

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

		static byte[] RemoveUtf8Header(byte[] a)
		{
			if ((a[0] == 0xef) &&
				(a[1] == 0xbb) &&
				(a[2] == 0xbf))
			{
				byte[] copy = new byte[a.Length-3];
				System.Array.Copy (a, 3, copy, 0, a.Length-3);
				a = copy;
			}
			return a;
		}

		static bool XmlTestEqual(byte[] a, byte[] b)
		{
			a = ResourceBundleTest.RemoveUtf8Header (a);
			b = ResourceBundleTest.RemoveUtf8Header (b);

			if (a.Length != b.Length)
			{
				return false;
			}

			for (int i = 0; i < a.Length; i++)
			{
				byte x = a[i];
				byte y = b[i];

				if (x == '\'')
					x = (byte) '\"';
				if (y == '\'')
					y = (byte) '\"';

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
				if ((i > 0) && ((i % 20) == 0))
					System.Console.Out.WriteLine ();
				System.Console.Out.Write ("{0,3:x2}", data[i]);
			}

			System.Console.Out.WriteLine ();
		}
	}
}
