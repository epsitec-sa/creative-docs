using NUnit.Framework;

namespace Epsitec.Common.Dialogs
{
	[TestFixture] public class DialogTest
	{
		[Test] public void CheckLoadDesignerFactory()
		{
			Assert.IsTrue (Dialog.LoadDesignerFactory ());
		}
		
		[Test] public void CheckCreateDesigner()
		{
			IDialogDesigner designer = Dialog.CreateDesigner ();
			
			Assert.IsNotNull (designer);
		}
		
		[Test] public void CheckCreateEmptyDialog()
		{
			IDialogDesigner designer = Dialog.CreateDesigner ();
			Widgets.Window  window   = new Widgets.Window ();
			
			Assert.IsNotNull (designer);
			
			designer.DialogWindow = window;
			designer.StartDesign ();
		}
	}
}
