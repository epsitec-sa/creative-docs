using NUnit.Framework;

namespace Epsitec.Common.Dialogs
{
	[TestFixture] public class DialogTest
	{
		[Test] public void CheckDialogLoadDesignerFactory()
		{
			Assert.IsTrue (Dialog.LoadDesignerFactory ());
		}
	}
}
