using NUnit.Framework;

namespace Epsitec.Common.Widgets
{
	[TestFixture]
	public class AdornerFactoryTest
	{
		[Test] public void CheckAdornerNames()
		{
			string[] names = Epsitec.Common.Widgets.Adorner.Factory.AdornerNames;
			
			Assert.IsNotNull (names);
			Assert.IsTrue (names.Length > 0);
			
			foreach (string name in names)
			{
				System.Console.Out.WriteLine ("Class '" + name + "' implements IAdorner.");
			}
		}
		
		[Test] public void CheckAdornerActivation()
		{
			string[] names = Epsitec.Common.Widgets.Adorner.Factory.AdornerNames;
			
			Assert.IsNotNull (names);
			Assert.IsTrue (names.Length > 0);
			
			foreach (string name in names)
			{
				Assert.IsTrue (Epsitec.Common.Widgets.Adorner.Factory.SetActive (name));
				Assert.AreEqual (name, Epsitec.Common.Widgets.Adorner.Factory.ActiveName);
				Assert.IsNotNull (Epsitec.Common.Widgets.Adorner.Factory.Active);
			}
		}
	}
}
