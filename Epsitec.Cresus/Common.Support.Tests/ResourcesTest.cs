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
				System.Console.Out.WriteLine ("  {0}: {1}", tag, bundle[tag]);
			}
			
			bundle = Resources.GetBundle ("file:button.cancel", ResourceLevel.Default);
			Assertion.AssertNotNull (bundle);
			System.Console.Out.WriteLine ("Bundle with default level only:");
			
			foreach (string tag in bundle.FieldNames)
			{
				System.Console.Out.WriteLine ("  {0}: {1}", tag, bundle[tag]);
			}
			
			bundle = Resources.GetBundle ("file:button.cancel", ResourceLevel.Localised);
			Assertion.AssertNotNull (bundle);
			System.Console.Out.WriteLine ("Bundle with localised level only:");
			
			foreach (string tag in bundle.FieldNames)
			{
				System.Console.Out.WriteLine ("  {0}: {1}", tag, bundle[tag]);
			}
		}
		
		[Test] [ExpectedException (typeof (ResourceException))] public void CheckGetBundleRecursive()
		{
			ResourceBundle bundle = Resources.GetBundle ("file:recursive");
			
			string data = bundle.GetFieldString ("loop");
		}
		
		[Test] public void CheckGetComplexBundle()
		{
			ResourceBundle bundle;
			
			bundle = Resources.GetBundle ("file:complex");
			
			Assertion.AssertNotNull (bundle);
			Assertion.AssertEquals (ResourceFieldType.Data, bundle.GetFieldType ("class"));
			Assertion.AssertEquals (ResourceFieldType.List, bundle.GetFieldType ("widgets"));
			Assertion.AssertEquals (3, bundle.GetFieldBundleListLength ("widgets"));
			Assertion.AssertEquals ("CheckButton", bundle.GetFieldBundleListItem ("widgets", 0)["class"]);
			Assertion.AssertEquals ("RadioButton", bundle.GetFieldBundleListItem ("widgets", 1)["class"]);
			Assertion.AssertEquals ("Button", bundle.GetFieldBundleListItem ("widgets", 2)["class"]);
			Assertion.AssertEquals ("file:complex", bundle.Name);
			Assertion.AssertEquals ("file:complex#widgets[0]", bundle.GetFieldBundleListItem ("widgets", 0).Name);
			Assertion.AssertEquals ("file:complex#widgets[1]", bundle.GetFieldBundleListItem ("widgets", 1).Name);
			Assertion.AssertEquals ("file:button.cancel", bundle.GetFieldBundleListItem ("widgets", 2).Name);
		}
	}
}
