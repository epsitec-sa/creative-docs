using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class ResourcesTest
	{
		[SetUp] public void SetUp()
		{
			try
			{
				Implementation.BaseProvider.CreateResourceDatabase ("test");
			}
			catch
			{
			}
			
			Resources.SetupApplication ("test");
		}
		
		[Test] public void CheckDebugDumpProviders()
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("Found {0} providers.", Resources.ProviderCount));
			Resources.DebugDumpProviders ();
		}
		
		[Test] public void CheckValidateAndDumpResults()
		{
			string[] prefixes = Resources.ProviderPrefixes;
			string[] names = new string[] { "", "@123", "123", "ABC-123", "ABC.", "ABC.A", "ABC..A", "ABC D", "ABC ", "A(B)", "A[X]", "A/B", "A\\B" };
			
			foreach (string prefix in prefixes)
			{
				System.Console.Out.WriteLine ("Prefix '{0}' checking:", prefix);
				
				foreach (string name in names)
				{
					System.Console.Out.WriteLine (" '{0}' is {1}", name, Resources.ValidateId (prefix+":"+name) ? "valid" : "not valid");
				}
			}
		}
		
		[Test] public void CheckValidateFile()
		{
			string[] good_names = new string[] { "123", "@123", "ABC-123", "A(B)", "ABC.A", "ABC D", "AB+. .-", "_", "__", "_1", "A\"B", "A'B" };
			string[] bad_names  = new string[] { "", "ABC.", "ABC..A", "ABC ", "A[X]", "A/B", "A\\B", "A#B", "A*B" };
			
			foreach (string name in good_names)
			{
				Assert.IsTrue (Resources.ValidateId ("file:"+name), string.Format ("{0} should be accepted", name));
			}
			
			foreach (string name in bad_names)
			{
				Assert.IsTrue (!Resources.ValidateId ("file:"+name), string.Format ("{0} should be rejected", name));
			}
		}
		
		[Test] public void CheckFileContains()
		{
			Assert.IsTrue (Resources.ContainsId ("file:button.cancel"));
		}
		
		[Test] public void CheckFileGetBundle()
		{
			ResourceBundle bundle;
			
			bundle = Resources.GetBundle ("file:button.cancel");
			Assert.IsNotNull (bundle);
			System.Console.Out.WriteLine ("Bundle with merged levels:");
			
			foreach (string tag in bundle.FieldNames)
			{
				System.Console.Out.WriteLine ("  {0}: {1}", tag, bundle[tag].Data);
			}
			
			bundle = Resources.GetBundle ("file:button.cancel", ResourceLevel.Default);
			Assert.IsNotNull (bundle);
			System.Console.Out.WriteLine ("Bundle with default level only:");
			
			foreach (string tag in bundle.FieldNames)
			{
				System.Console.Out.WriteLine ("  {0}: {1}", tag, bundle[tag].AsString);
			}
			
			bundle = Resources.GetBundle ("file:button.cancel", ResourceLevel.Localised);
			Assert.IsNotNull (bundle);
			System.Console.Out.WriteLine ("Bundle with localised level only:");
			
			foreach (string tag in bundle.FieldNames)
			{
				System.Console.Out.WriteLine ("  {0}: {1}", tag, bundle[tag].Data);
			}
		}
		
		[Test] public void CheckBase00ContainsNo()
		{
			Assert.IsTrue (! Resources.ContainsId ("base:raw_data_test"));
		}
		
		[Test] public void CheckBase01SetBinaryData()
		{
			byte[] data = new byte[] { 1, 2, 3, 4, 5 };
			
			Resources.SetBinaryData ("base:raw_data_test", ResourceLevel.Default, null, data, ResourceSetMode.Write);
		}
		
		[Test] public void CheckBase02ContainsYes()
		{
			Assert.IsTrue (Resources.ContainsId ("base:raw_data_test"));
		}
		
		[Test] public void CheckBase03GetBinaryData()
		{
			System.Diagnostics.Debug.WriteLine ("Check: Base / GetBinaryData");
			System.Diagnostics.Debug.WriteLine (">> begin");
			byte[] data = Resources.GetBinaryData ("base:raw_data_test", ResourceLevel.Default, null);
			System.Diagnostics.Debug.WriteLine ("<< end");
			
			Assert.IsNotNull (data);
			Assert.AreEqual (5, data.Length);
			Assert.AreEqual (1, data[0]);
			Assert.AreEqual (2, data[1]);
			Assert.AreEqual (3, data[2]);
			Assert.AreEqual (4, data[3]);
			Assert.AreEqual (5, data[4]);
		}
		
		[Test] public void CheckBase04RemoveResource()
		{
			Assert.IsTrue (Resources.ContainsId ("base:raw_data_test"));
			
			byte[] data_xx = new byte[] { (byte)'X' };
			byte[] data_de = new byte[] { (byte)'D', (byte)'E' };
			
			Resources.SetBinaryData ("base:raw_data_test", ResourceLevel.Customised, null, data_xx, ResourceSetMode.CreateOnly);
			Resources.SetBinaryData ("base:raw_data_test", ResourceLevel.Localised, Resources.FindCultureInfo ("de"), data_de, ResourceSetMode.CreateOnly);
			
			byte[] find_xx = Resources.GetBinaryData ("base:raw_data_test", ResourceLevel.Customised, null);
			byte[] find_de = Resources.GetBinaryData ("base:raw_data_test", ResourceLevel.Localised, Resources.FindCultureInfo ("de"));
			
			Assert.IsNotNull (find_xx);
			Assert.IsNotNull (find_de);
			Assert.AreEqual (data_xx[0], find_xx[0]);
			Assert.AreEqual (data_de[0], find_de[0]);
			Assert.AreEqual (data_de[1], find_de[1]);
			
			Resources.RemoveResource ("base:raw_data_test", ResourceLevel.Customised, null);
			Resources.RemoveResource ("base:raw_data_test", ResourceLevel.Localised, Resources.FindCultureInfo ("de"));
			
			Assert.IsNull (Resources.GetBinaryData ("base:raw_data_test", ResourceLevel.Customised, null));
			Assert.IsNull (Resources.GetBinaryData ("base:raw_data_test", ResourceLevel.Localised, Resources.FindCultureInfo ("de")));
			Assert.IsNotNull (Resources.GetBinaryData ("base:raw_data_test", ResourceLevel.Default, null));
			Assert.IsTrue (Resources.ContainsId ("base:raw_data_test"));
			
			Resources.SetBinaryData ("base:raw_data_test", ResourceLevel.Customised, null, data_xx, ResourceSetMode.CreateOnly);
			Resources.SetBinaryData ("base:raw_data_test", ResourceLevel.Localised, Resources.FindCultureInfo ("de"), data_de, ResourceSetMode.CreateOnly);
			
			Resources.RemoveResource ("base:raw_data_test", ResourceLevel.All, null);
			
			Assert.IsNull (Resources.GetBinaryData ("base:raw_data_test", ResourceLevel.Customised, null));
			Assert.IsNull (Resources.GetBinaryData ("base:raw_data_test", ResourceLevel.Localised, Resources.FindCultureInfo ("de")));
			Assert.IsNull (Resources.GetBinaryData ("base:raw_data_test", ResourceLevel.Default, null));
			Assert.IsTrue (! Resources.ContainsId ("base:raw_data_test"));
		}
		

#if false
		[Test] public void CheckBaseStressTest()
		{
			Assert.IsTrue (! Resources.ContainsId ("base:StressResourceLtl.0"));
			Assert.IsTrue (! Resources.ContainsId ("base:StressResourceBig.0"));
			
			System.Diagnostics.Debug.WriteLine ("Creating 1000 resources (10 bytes).");
			for (int i = 0; i < 1000; i++)
			{
				string id_name = string.Format ("base:StressResourceLtl.{0}", i);
				byte[] id_data = new byte[10];
				
				Resources.SetBinaryData (id_name, ResourceLevel.Default, null, id_data, ResourceSetMode.CreateOnly);
			}
			System.Diagnostics.Debug.WriteLine ("Done.");
			
			System.Diagnostics.Debug.WriteLine ("Creating 1000 resources (10'000 bytes).");
			for (int i = 0; i < 1000; i++)
			{
				string id_name = string.Format ("base:StressResourceBig.{0}", i);
				byte[] id_data = new byte[10000];
				
				Resources.SetBinaryData (id_name, ResourceLevel.Default, null, id_data, ResourceSetMode.CreateOnly);
			}
			System.Diagnostics.Debug.WriteLine ("Done.");
			
			System.Diagnostics.Debug.WriteLine ("Loading 1000 resources (10 bytes).");
			for (int i = 0; i < 1000; i++)
			{
				string id_name = string.Format ("base:StressResourceLtl.{0}", i);
				byte[] id_data = Resources.GetBinaryData (id_name, ResourceLevel.Default, null);
			}
			System.Diagnostics.Debug.WriteLine ("Done.");
			
			System.Diagnostics.Debug.WriteLine ("Loading 1000 resources (10'000 bytes).");
			for (int i = 0; i < 1000; i++)
			{
				string id_name = string.Format ("base:StressResourceBig.{0}", i);
				byte[] id_data = Resources.GetBinaryData (id_name, ResourceLevel.Default, null);
			}
			System.Diagnostics.Debug.WriteLine ("Done.");
			
			System.Diagnostics.Debug.WriteLine ("Deleting 1000 resources (10 bytes).");
			for (int i = 0; i < 1000; i++)
			{
				string id_name = string.Format ("base:StressResourceLtl.{0}", i);
				Resources.RemoveResource (id_name, ResourceLevel.Default, null);
			}
			System.Diagnostics.Debug.WriteLine ("Done.");
			
			System.Diagnostics.Debug.WriteLine ("Deleting 1000 resources (10'000 bytes).");
			for (int i = 0; i < 1000; i++)
			{
				string id_name = string.Format ("base:StressResourceBig.{0}", i);
				Resources.RemoveResource (id_name, ResourceLevel.Default, null);
			}
			System.Diagnostics.Debug.WriteLine ("Done.");
		}
#endif
		
		[Test] public void CheckBase50GetBundle()
		{
			ResourceBundle bundle;
			
			bundle = Resources.GetBundle ("base:button.cancel");
			Assert.IsNotNull (bundle);
			System.Console.Out.WriteLine ("Bundle with merged levels:");
			
			foreach (string tag in bundle.FieldNames)
			{
				System.Console.Out.WriteLine ("  {0}: {1}", tag, bundle[tag].Data);
			}
			
			bundle = Resources.GetBundle ("base:button.cancel", ResourceLevel.Default);
			Assert.IsNotNull (bundle);
			System.Console.Out.WriteLine ("Bundle with default level only:");
			
			foreach (string tag in bundle.FieldNames)
			{
				System.Console.Out.WriteLine ("  {0}: {1}", tag, bundle[tag].AsString);
			}
			
			bundle = Resources.GetBundle ("base:button.cancel", ResourceLevel.Localised);
			Assert.IsNotNull (bundle);
			System.Console.Out.WriteLine ("Bundle with localised level only:");
			
			foreach (string tag in bundle.FieldNames)
			{
				System.Console.Out.WriteLine ("  {0}: {1}", tag, bundle[tag].Data);
			}
		}
		
		[Test] [ExpectedException (typeof (ResourceException))] public void CheckGetBundleRecursive()
		{
			ResourceBundle bundle = Resources.GetBundle ("file:recursive");
			
			string data = bundle["loop"].AsString;
		}
		
		[Test] public void CheckGetComplexBundle()
		{
			ResourceBundle bundle;
			
			bundle = Resources.GetBundle ("file:complex");
			
			Assert.IsNotNull (bundle);
			Assert.AreEqual (ResourceFieldType.Data, bundle["class"].Type);
			Assert.AreEqual (ResourceFieldType.List, bundle["widgets"].Type);
			Assert.AreEqual (3, bundle["widgets"].AsList.Count);
			Assert.AreEqual ("CheckButton", bundle["widgets"].AsList[0].AsBundle["class"].AsString);
			Assert.AreEqual ("RadioButton", bundle["widgets"].AsList[1].AsBundle["class"].AsString);
			Assert.AreEqual ("Button",      bundle["widgets"].AsList[2].AsBundle["class"].AsString);
			Assert.AreEqual ("complex", bundle.Name);
//-			Assert.AreEqual ("complex#widgets[0]", bundle["widgets"].AsList[0].AsBundle.Name);
//-			Assert.AreEqual ("complex#widgets[1]", bundle["widgets"].AsList[1].AsBundle.Name);
			Assert.AreEqual ("button.cancel",      bundle["widgets"].AsList[2].AsBundle.Name);
		}
		
		[Test] public void CheckGetText()
		{
			string text_default = Resources.GetText ("file:strings#title.SettingsWindow", ResourceLevel.Default);
			string text_merged  = Resources.GetText ("file:strings#title.SettingsWindow", ResourceLevel.Merged);
			string text_miss_1  = Resources.GetText ("file:strings#DoesNotExist");
			string text_miss_2  = Resources.GetText ("file:does_not_exist#DoesNotExist");
			
			Assert.IsNotNull (text_default);
			Assert.IsNotNull (text_merged);
			Assert.IsNull (text_miss_1);
			Assert.IsNull (text_miss_2);
			
			Assert.AreEqual ("Settings", text_default);
			Assert.AreEqual ("R�glages", text_merged);
		}
		
		[Test] public void CheckGetBundleIds()
		{
			string[] names_1 = Resources.GetBundleIds ("file:*");
			string[] names_2 = Resources.GetBundleIds ("file:*", ResourceLevel.Localised);
			string[] names_3 = Resources.GetBundleIds ("file:*", "String", ResourceLevel.Localised);
			string[] names_4 = Resources.GetBundleIds ("file:strings", ResourceLevel.All);
			
			System.Console.Out.WriteLine ("file:*");
			
			for (int i = 0; i < names_1.Length; i++)
			{
				System.Console.Out.WriteLine (" {0} : {1}", i, names_1[i]);
			}
			
			System.Console.Out.WriteLine ("file:*, level=localised");
			
			for (int i = 0; i < names_2.Length; i++)
			{
				System.Console.Out.WriteLine (" {0} : {1}", i, names_2[i]);
			}
			
			System.Console.Out.WriteLine ("file:*, type=String, level=localised");
			
			for (int i = 0; i < names_3.Length; i++)
			{
				System.Console.Out.WriteLine (" {0} : {1}", i, names_3[i]);
			}
			
			System.Console.Out.WriteLine ("file:strings, level=all");
			
			for (int i = 0; i < names_4.Length; i++)
			{
				string clean  = Resources.StripSuffix (names_4[i]);
				string suffix = Resources.ExtractSuffix (names_4[i]);
				
				ResourceLevel level;
				System.Globalization.CultureInfo culture;
				
				Resources.MapFromSuffix (suffix, out level, out culture);
				
				string about = "default";
				
				switch (level)
				{
					case ResourceLevel.Localised:
						about = "localised to " + culture.EnglishName;
						break;
					case ResourceLevel.Customised:
						about = "customised for " + culture.EnglishName;
						break;
				}
				
				System.Console.Out.WriteLine (" {0} : {1} -> {2}, {3}, {4}", i, names_4[i], clean, suffix, about);
			}
		}
	}
}
