using NUnit.Framework;

namespace Epsitec.Common.Widgets
{
	[TestFixture] public class FeelTest
	{
		[Test] public void CheckFeelNames()
		{
			string[] names = Epsitec.Common.Widgets.Feel.Factory.FeelNames;
			
			Assert.IsNotNull (names);
			Assert.IsTrue (names.Length > 0);
			
			foreach (string name in names)
			{
				System.Console.Out.WriteLine ("Class '" + name + "' implements IFeel.");
			}
		}
		
		[Test] public void CheckFeelActivation()
		{
			string[] names = Epsitec.Common.Widgets.Feel.Factory.FeelNames;
			
			Assert.IsNotNull (names);
			Assert.IsTrue (names.Length > 0);
			
			foreach (string name in names)
			{
				Assert.IsTrue (Epsitec.Common.Widgets.Feel.Factory.SetActive (name));
				Assert.AreEqual (name, Epsitec.Common.Widgets.Feel.Factory.ActiveName);
				Assert.IsNotNull (Epsitec.Common.Widgets.Feel.Factory.Active);
			}
		}
		
		[Test] public void CheckFeelShortcuts()
		{
			Feel.Factory.SetActive ("Default");
			
			IFeel feel = Feel.Factory.Active;
			
			Assert.IsNotNull (feel);
			
			Shortcut s1 = feel.AcceptShortcut;
			Shortcut s2 = feel.CancelShortcut;
			
			Assert.AreEqual (s1, new Shortcut (KeyCode.Return));
			Assert.AreEqual (s2, new Shortcut (KeyCode.Escape));
		}
	}
}
