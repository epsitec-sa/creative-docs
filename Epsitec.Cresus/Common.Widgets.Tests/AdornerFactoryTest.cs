using NUnit.Framework;

namespace Epsitec.Common.Widgets
{
	[TestFixture]
	public class AdornerFactoryTest
	{
		[Test] public void CheckAdornerNames()
		{
			string[] names = Epsitec.Common.Widgets.Adorner.Factory.AdornerNames;
			
			Assertion.AssertNotNull (names);
			Assertion.Assert (names.Length > 0);
			
			foreach (string name in names)
			{
				System.Console.Out.WriteLine ("Class '" + name + "' implements IAdorner.");
			}
		}
		
		[Test] public void CheckAdornerActivation()
		{
			string[] names = Epsitec.Common.Widgets.Adorner.Factory.AdornerNames;
			
			Assertion.AssertNotNull (names);
			Assertion.Assert (names.Length > 0);
			
			foreach (string name in names)
			{
				Assertion.Assert (Epsitec.Common.Widgets.Adorner.Factory.SetActive (name));
				Assertion.AssertEquals (name, Epsitec.Common.Widgets.Adorner.Factory.ActiveName);
				Assertion.AssertNotNull (Epsitec.Common.Widgets.Adorner.Factory.Active);
			}
		}
	}
}
