using NUnit.Framework;

namespace Epsitec.Common.Script
{
	[TestFixture] public class DeveloperTest
	{
		[SetUp] public void Initialise()
		{
			Common.Widgets.Widget.Initialise ();
			Common.Pictogram.Engine.Initialise ();
			Common.Widgets.Adorner.Factory.SetActive ("LookMetal");
		}
		
		[Test] public void CheckDeveloperUI()
		{
			Widgets.Window window = new Widgets.Window ();
			
			Common.UI.Data.Record record  = new Epsitec.Common.UI.Data.Record ();
			Types.IDataValue[]    values  = SourceTest.CreateValues (out record);
			
			Developer.EditionController controller = new Developer.EditionController ();
			
			window.Text       = "CheckDeveloperUI";
			window.ClientSize = new Drawing.Size (600, 400);
			
			Source source = SourceTest.CreateSource (values);
			
			controller.Source = source;
			controller.CreateWidgets (window.Root);
			
			window.Show ();
		}
	}
}
