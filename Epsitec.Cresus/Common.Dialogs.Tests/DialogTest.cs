using NUnit.Framework;

namespace Epsitec.Common.Dialogs
{
	[TestFixture] public class DialogTest
	{
		[SetUp] public void SetUp()
		{
			Epsitec.Common.Widgets.Widget.Initialise ();
		}
		
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
		
		[Test] public void CheckLoad1()
		{
			Dialog dialog = new Dialog ();
			dialog.Load ("file:unknown_dialog");
		}
	}
}
