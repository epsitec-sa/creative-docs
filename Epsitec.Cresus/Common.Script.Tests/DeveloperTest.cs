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
		
		[Test] public void CheckEditorEngine()
		{
			Assert.IsNotNull (Editor.Engine);
			
			Common.UI.Data.Record record = new Epsitec.Common.UI.Data.Record ();
			Types.IDataValue[]    values = SourceTest.CreateValues (out record);
			Source                source = SourceTest.CreateSource (values);
			
			object document = Editor.Engine.CreateDocument (source);
			
			Editor.Engine.ShowMethod (document, "Mysterious");
		}
		
		[Test] public void CheckDeveloperUI()
		{
			Widgets.Window window = new Widgets.Window ();
			
			Common.UI.Data.Record record = new Epsitec.Common.UI.Data.Record ();
			Types.IDataValue[]    values = SourceTest.CreateValues (out record);
			Source                source = SourceTest.CreateSource (values);
			
			Developer.EditionController controller = new Developer.EditionController ();
			
			window.Text       = "CheckDeveloperUI";
			window.ClientSize = new Drawing.Size (600, 400);
			
			
			controller.Source = source;
			controller.CreateWidgets (window.Root);
			
			window.Show ();
		}
	}
}
