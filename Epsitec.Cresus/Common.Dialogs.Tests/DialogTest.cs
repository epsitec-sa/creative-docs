using NUnit.Framework;

namespace Epsitec.Common.Dialogs
{
	[TestFixture] public class DialogTest
	{
		[Test] public void CheckDialogLoadDesigner()
		{
			Assert.IsTrue (Dialog.LoadDesigner ());
		}
	}
}
