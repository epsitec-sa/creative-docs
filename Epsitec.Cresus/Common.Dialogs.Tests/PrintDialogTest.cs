using NUnit.Framework;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// Summary description for PrintDialogTest.
	/// </summary>
	[TestFixture] public class PrintDialogTest
	{
		[Test] public void CheckShow()
		{
			Print dialog = new Print ();
			dialog.Show ();
		}
	}
}
