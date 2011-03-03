using NUnit.Framework;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Tests.Widgets
{
	[TestFixture]
	public class MessageTest
	{
		[Test] public void CheckGetKeyName()
		{
			Assert.AreEqual ("4",				Message.GetKeyName (KeyCode.Digit4));
			Assert.AreEqual ("Pg.Prec",			Message.GetKeyName (KeyCode.PageUp));
			Assert.AreEqual ("F12",				Message.GetKeyName (KeyCode.FuncF12));
			Assert.AreEqual ("C",				Message.GetKeyName (KeyCode.AlphaC));
			Assert.AreEqual ("Ctrl+Alt+Ins",	Message.GetKeyName (KeyCode.ModifierControl | KeyCode.ModifierAlt | KeyCode.Insert));
			Assert.AreEqual ("Maj+Origine",		Message.GetKeyName (KeyCode.ModifierShift | KeyCode.Home));
			Assert.AreEqual ("F20",				Message.GetKeyName (KeyCode.FuncF20));
		}
	}
}
