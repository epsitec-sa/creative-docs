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
			Assertion.AssertEquals (4, graph.Root.Count);
			
			Types.IDataValue v1 = graph.Navigate ("FontName") as Types.IDataValue;
			Types.IDataValue v2 = graph.Navigate ("FontSize") as Types.IDataValue;
			Types.IDataValue v3 = graph.Navigate ("UseHyphen") as Types.IDataValue;
			Types.IDataValue v4 = graph.Navigate ("FontStyle") as Types.IDataValue;
			
			Assertion.AssertNotNull (v1);
			Assertion.AssertNotNull (v2);
			Assertion.AssertNotNull (v3);
			Assertion.AssertNotNull (v4);
			
			Assertion.AssertEquals ("String",  v1.DataType.Name);
			Assertion.AssertEquals ("Decimal", v2.DataType.Name);
			Assertion.AssertEquals ("Boolean", v3.DataType.Name);
			Assertion.AssertEquals ("Integer", v4.DataType.Name);
			
			Assertion.AssertEquals (v1, record["FontName"]);
			Assertion.AssertEquals (v2, record["FontSize"]);
			Assertion.AssertEquals (v3, record["UseHyphen"]);
			Assertion.AssertEquals (v4, record["FontStyle"]);
			
			Assertion.AssertEquals (typeof (string),  v1.DataType.SystemType);
			Assertion.AssertEquals (typeof (decimal), v2.DataType.SystemType);
			Assertion.AssertEquals (typeof (bool),    v3.DataType.SystemType);
			Assertion.AssertEquals (typeof (int),     v4.DataType.SystemType);
			
			Assertion.AssertEquals ("Times", v1.ReadValue ());
			Assertion.AssertEquals (12.0, (double)(decimal) v2.ReadValue ());
			Assertion.AssertEquals (false, v3.ReadValue ());
			Assertion.AssertEquals (1, (int) v4.ReadValue ());
		}
		
		[Test] public void CheckBindWidget()
		{
			Data.Record record = EngineTest.CreateRecord ();
			
			Widget          parent  = new Widget ();
			TextField       text    = new TextField (parent);
			TextFieldUpDown up_down = new TextFieldUpDown (parent);
			CheckButton     check_b = new CheckButton (parent);
			RadioButton     radio_1 = new RadioButton (parent, "Group A", 1);
			RadioButton     radio_2 = new RadioButton (parent, "Group A", 2);
			RadioButton     radio_3 = new RadioButton (parent, "Group A", 3);
			
			Engine.BindWidget (record, text,    @"<bind path=""FontName"" />");
			Engine.BindWidget (record, up_down, @"<bind path=""FontSize"" />");
			Engine.BindWidget (record, check_b, @"<bind path=""UseHyphen"" />");
			Engine.BindWidget (record, radio_1, @"<bind path=""FontStyle"" />");
			
			Assertion.AssertEquals ("Times", text.Text);
			
			Assertion.AssertEquals ( 12.0, (double) up_down.Value);
			Assertion.AssertEquals (  1.0, (double) up_down.MinValue);
			Assertion.AssertEquals (299.0, (double) up_down.MaxValue);
			Assertion.AssertEquals (  0.1, (double) up_down.Resolution);
			
			Assertion.AssertEquals (false, check_b.IsActive);
			
			Assertion.AssertEquals (true,  radio_1.IsActive);
			Assertion.AssertEquals (false, radio_2.IsActive);
			Assertion.AssertEquals (false, radio_3.IsActive);
			
			text.Text           = "Helvetica";
			up_down.Value       = 14M;
			check_b.ActiveState = WidgetState.ActiveYes;
			radio_2.ActiveState = WidgetState.ActiveYes;
			
			Assertion.AssertEquals (false, radio_1.IsActive);
			Assertion.AssertEquals (true,  radio_2.IsActive);
			Assertion.AssertEquals (false, radio_3.IsActive);
			
			Assertion.AssertEquals ("Helvetica",             record["FontName"].ReadValue ());
			Assertion.AssertEquals (14.0, (double) (decimal) record["FontSize"].ReadValue ());
			Assertion.AssertEquals (true,                    record["UseHyphen"].ReadValue ());
			Assertion.AssertEquals (2,                 (int) record["FontStyle"].ReadValue ());
			
			record["FontName"].WriteValue ("Courier");
			record["FontSize"].WriteValue (10M);
			record["UseHyphen"].WriteValue (false);
			record["FontStyle"].WriteValue (3);
			
			Assertion.AssertEquals ("Courier", text.Text);
			Assertion.AssertEquals (10.0, (double) up_down.Value);
			Assertion.AssertEquals (false, check_b.IsActive);
			
			Assertion.AssertEquals (false, radio_1.IsActive);
			Assertion.AssertEquals (false, radio_2.IsActive);
			Assertion.AssertEquals (true,  radio_3.IsActive);
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
			record.Add (new Data.Field ("UseHyphen", false));
			record.Add (new Data.Field ("FontStyle", 1));
			
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
