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
			string[] good_names = new string[] { "123", "ABC-123", "ABC.A", "ABC D", "AB+. .-", "_", "__", "_1" };
			string[] bad_names  = new string[] { "", "@123", "ABC.", "ABC..A", "ABC ", "A(B)", "A[X]", "A/B", "A\\B", "A#B", "A*B", "A\"B", "A'B" };
			
			foreach (string name in good_names)
			{
				Assertion.Assert (string.Format ("{0} should be accepted", name), Resources.ValidateId ("file:"+name));
			}
			
			foreach (string name in bad_names)
			{
				Assertion.Assert (string.Format ("{0} should be rejected", name), !Resources.ValidateId ("file:"+name));
			}
		}
		
		[Test] public void CheckGetBundle()
		{
			ResourceBundle bundle;
			
			bundle = Resources.GetBundle ("file:button.cancel");
			Assertion.AssertNotNull (bundle);
			System.Console.Out.WriteLine ("Bundle with merged levels:");
			
			foreach (string tag in bundle.FieldNames)
			{
				System.Console.Out.WriteLine ("  {0}: {1}", tag, bundle[tag].Data);
			}
			
			bundle = Resources.GetBundle ("file:button.cancel", ResourceLevel.Default);
			Assertion.AssertNotNull (bundle);
			System.Console.Out.WriteLine ("Bundle with default level only:");
			
			foreach (string tag in bundle.FieldNames)
			{
				System.Console.Out.WriteLine ("  {0}: {1}", tag, bundle[tag].AsString);
			}
			
			bundle = Resources.GetBundle ("file:button.cancel", ResourceLevel.Localised);
			Assertion.AssertNotNull (bundle);
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
			
			Assertion.AssertNotNull (bundle);
			Assertion.AssertEquals (ResourceFieldType.Data, bundle["class"].Type);
			Assertion.AssertEquals (ResourceFieldType.List, bundle["widgets"].Type);
			Assertion.AssertEquals (3, bundle["widgets"].AsList.Count);
			Assertion.AssertEquals ("CheckButton", bundle["widgets"].AsList[0].AsBundle["class"].AsString);
			Assertion.AssertEquals ("RadioButton", bundle["widgets"].AsList[1].AsBundle["class"].AsString);
			Assertion.AssertEquals ("Button",      bundle["widgets"].AsList[2].AsBundle["class"].AsString);
			Assertion.AssertEquals ("file:complex", bundle.Name);
			Assertion.AssertEquals ("file:complex#widgets[0]", bundle["widgets"].AsList[0].AsBundle.Name);
			Assertion.AssertEquals ("file:complex#widgets[1]", bundle["widgets"].AsList[1].AsBundle.Name);
			Assertion.AssertEquals ("file:button.cancel",      bundle["widgets"].AsList[2].AsBundle.Name);
		}
		
		[Test] public void CheckGetText()
		{
			string text_default = Resources.GetText ("file:strings#title.SettingsWindow", ResourceLevel.Default);
			string text_merged  = Resources.GetText ("file:strings#title.SettingsWindow", ResourceLevel.Merged);
			string text_miss_1  = Resources.GetText ("file:strings#DoesNotExist");
			string text_miss_2  = Resources.GetText ("file:does_not_exist#DoesNotExist");
			
			Assertion.AssertNotNull (text_default);
			Assertion.AssertNotNull (text_merged);
			Assertion.AssertNull (text_miss_1);
			Assertion.AssertNull (text_miss_2);
			
			Assertion.AssertEquals ("Settings", text_default);
			Assertion.AssertEquals ("Réglages", text_merged);
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
