using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class ResourcesTest
	{
		[SetUp] public void SetUp()
		{
			Resources.SetupProviders ("test");
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
		
		[Test] public void CheckGetBundle()
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
			Assert.AreEqual ("complex#widgets[0]", bundle["widgets"].AsList[0].AsBundle.Name);
			Assert.AreEqual ("complex#widgets[1]", bundle["widgets"].AsList[1].AsBundle.Name);
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
			Assert.AreEqual ("Réglages", text_merged);
		}
		[Test] public void CheckGetBundleIds()
		{
			string[] names_1 = Resources.GetBundleIds ("file:*");
			string[] names_2 = Resources.GetBundleIds ("file:*", ResourceLevel.Localised);
			
			for (int i = 0; i < names_1.Length; i++)
			{
				System.Console.Out.WriteLine ("{0} : {1}", i, names_1[i]);
			}
			
			for (int i = 0; i < names_2.Length; i++)
			{
				System.Console.Out.WriteLine ("{0} : {1}", i, names_2[i]);
			}
		}
	}
}
