using NUnit.Framework;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.UI
{
	[TestFixture] public class EngineTest
	{
		[Test] public void CheckDataRecordAndFields()
		{
			Data.Record      record = EngineTest.CreateRecord ();
			Types.IDataGraph graph  = record;
			
			Assertion.AssertEquals (record, graph.Root);
			Assertion.AssertEquals (2, graph.Root.Count);
			
			Types.IDataValue v1 = graph.Navigate ("FontName") as Types.IDataValue;
			Types.IDataValue v2 = graph.Navigate ("FontSize") as Types.IDataValue;
			
			Assertion.AssertNotNull (v1);
			Assertion.AssertNotNull (v2);
			
			Assertion.AssertEquals ("String",  v1.DataType.Name);
			Assertion.AssertEquals ("Decimal", v2.DataType.Name);
			
			Assertion.AssertEquals (v1, record["FontName"]);
			Assertion.AssertEquals (v2, record["FontSize"]);
			
			Assertion.AssertEquals (typeof (string),  v1.DataType.SystemType);
			Assertion.AssertEquals (typeof (decimal), v2.DataType.SystemType);
			
			Assertion.AssertEquals ("Times", v1.ReadValue ());
			Assertion.AssertEquals (12.0M,   v2.ReadValue ());
		}
		
		[Test] public void CheckBindWidget()
		{
			Data.Record record = EngineTest.CreateRecord ();
			
			TextField       text    = new TextField ();
			TextFieldUpDown up_down = new TextFieldUpDown ();
			
			Engine.BindWidget (record, text,    @"<bind path=""FontName"" />");
//			Engine.BindWidget (record, up_down, @"<bind path=""FontSize"" />");
			
			Assertion.AssertEquals ("Times", text.Text);
			
			text.Text = "Helvetica";
			
			Assertion.AssertEquals ("Helvetica", record["FontName"].ReadValue ());
		}
		
		private static Data.Record CreateRecord()
		{
			Data.Record record = new Data.Record ();
			
			record.Add (new Data.Field ("FontName", "Times"));
			record.Add (new Data.Field ("FontSize", 12.0M, new Types.DecimalType (1.0M, 299.0M, 0.1M)));
			
			return record;
		}
	}
}
