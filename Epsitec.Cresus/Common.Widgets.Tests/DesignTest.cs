using NUnit.Framework;

namespace Epsitec.Common.Widgets
{
	[TestFixture] public class DesignTest
	{
		public DesignTest()
		{
			Pictogram.Engine.Initialise ();
			Widgets.Adorner.Factory.SetActive ("LookMetal");
		}
		
		[Test] public void CheckController()
		{
			Design.Controller controller = new Design.Controller ();
			
			controller.Initialise ();
			controller.CreationWindow.Show ();
			controller.AttributeWindow.Show ();
		}
	}
}
