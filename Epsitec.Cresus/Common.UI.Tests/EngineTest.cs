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
			Assertion.AssertEquals (12.0, (double)(decimal) v2.ReadValue ());
		}
		
		[Test] public void CheckBindWidget()
		{
			Data.Record record = EngineTest.CreateRecord ();
			
			TextField       text    = new TextField ();
			TextFieldUpDown up_down = new TextFieldUpDown ();
			
			Engine.BindWidget (record, text,    @"<bind path=""FontName"" />");
			Engine.BindWidget (record, up_down, @"<bind path=""FontSize"" />");
			
			Assertion.AssertEquals ("Times", text.Text);
			
			Assertion.AssertEquals ( 12.0, (double) up_down.Value);
			Assertion.AssertEquals (  1.0, (double) up_down.MinValue);
			Assertion.AssertEquals (299.0, (double) up_down.MaxValue);
			Assertion.AssertEquals (  0.1, (double) up_down.Resolution);
			
			text.Text     = "Helvetica";
			up_down.Value = 14M;
			
			Assertion.AssertEquals ("Helvetica", record["FontName"].ReadValue ());
			Assertion.AssertEquals (14.0, (double) up_down.Value);
			
			record["FontName"].WriteValue ("Courier");
			record["FontSize"].WriteValue (10M);
			
			Assertion.AssertEquals ("Courier", text.Text);
			Assertion.AssertEquals (10.0, (double) up_down.Value);
		}
		
		[Test] public void CheckConstraint()
		{
			Data.Record record = EngineTest.CreateRecord ();
			
			TextField       text    = new TextField ();
			TextFieldUpDown up_down = new TextFieldUpDown ();
			
			Engine.BindWidget (record, text,    @"<bind path=""FontName"" />");
			Engine.BindWidget (record, up_down, @"<bind path=""FontSize"" />");
			
			Assertion.AssertEquals ("Times", text.Text);
			Assertion.AssertEquals ("Times", record["FontName"].ReadValue ());
			Assertion.AssertEquals (12.0, (double) up_down.Value);
			Assertion.AssertEquals (12.0, (double) (decimal) record["FontSize"].ReadValue ());
			
			text.Text = "XYZ";		//	pas accepté par la contrainte XStringConstraint
			
			Assertion.AssertEquals ("Times", record["FontName"].ReadValue ());
			
			up_down.Text = "-5.5";	//	pas accepté
			
			Assertion.AssertEquals (12.0, (double) (decimal) record["FontSize"].ReadValue ());
			
			up_down.Value = -5.5M;	//	accepté, parce que Value contraint à [1..299]
			
			Assertion.AssertEquals (1.0, (double) (decimal) record["FontSize"].ReadValue ());
			
			text.Text = "A";		//	pas accepté par la contrainte XStringConstraint
			
			Assertion.AssertEquals ("Times", record["FontName"].ReadValue ());
			
			text.Text = "ABC";		//	modification OK
			
			Assertion.AssertEquals ("ABC", record["FontName"].ReadValue ());
		}
		
		
		private static Data.Record CreateRecord()
		{
			Data.Record record = new Data.Record ();
			
			record.Add (new Data.Field ("FontName", "Times", null, new XStringConstraint ()));
			record.Add (new Data.Field ("FontSize", 12M, new Types.DecimalType (1.0M, 299.0M, 0.1M)));
			
			return record;
		}
		
		
		private class XStringConstraint : Types.IDataConstraint
		{
			public XStringConstraint()
			{
			}
			
			
			#region IDataConstraint Members
			public bool CheckConstraint(object value)
			{
				string text = value as string;
				
				if ((text != null) &&
					(text.Length > 1) &&
					(text[0] != 'X'))
				{
					return true;
				}
				
				return false;
			}
			#endregion

		}

	}
}
