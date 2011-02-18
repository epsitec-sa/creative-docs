using NUnit.Framework;

namespace Epsitec.Common.Script
{
	[TestFixture] public class DeveloperTest
	{
		[SetUp] public void Initialize()
		{
			Common.Widgets.Widget.Initialize ();
			Common.Document.Engine.Initialize ();
			Common.Widgets.Adorners.Factory.SetActive ("LookMetal");
		}
		
		[Test] public void CheckEditorEngine()
		{
			Assert.IsNotNull (Editor.Engine);
			
			Common.UI.Data.ObsoleteRecord record = new Epsitec.Common.UI.Data.ObsoleteRecord ();
			Types.IDataValue[]    values = SourceTest.CreateValues (out record);
			Source                source = SourceTest.CreateSource (values);
			ScriptWrapper         script = new ScriptWrapper ();
			
			script.Source = source;
			
			object document = Editor.Engine.CreateDocument (script);
			
			Editor.Engine.ShowMethod (document, "Mysterious");
		}
		
		[Test] public void CheckDeveloperUI()
		{
			Widgets.Window window = new Widgets.Window ();
			
			Common.UI.Data.ObsoleteRecord record = new Epsitec.Common.UI.Data.ObsoleteRecord ();
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
