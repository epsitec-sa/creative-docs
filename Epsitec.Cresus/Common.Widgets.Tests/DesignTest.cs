using NUnit.Framework;

namespace Epsitec.Common.Widgets
{
	[TestFixture] public class DesignTest
	{
		[Test] public void CheckController()
		{
			Design.Controller controller = new Design.Controller ();
			
			controller.Initialise ();
			controller.WidgetPaletteWindow.Show ();
		}
	}
}
