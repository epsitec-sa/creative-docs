using NUnit.Framework;

namespace Epsitec.Cresus.Support.Tests
{
	[TestFixture]
	public class ResourcesTest
	{
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
	}
}
