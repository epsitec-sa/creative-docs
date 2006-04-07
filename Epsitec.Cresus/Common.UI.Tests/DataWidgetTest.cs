using NUnit.Framework;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.UI
{
	[TestFixture] public class DataWidgetTest
	{
		[SetUp] public void SetUp()
		{
			Common.Widgets.Widget.Initialise ();
			Common.Widgets.Adorners.Factory.SetActive ("LookMetal");
			
			Support.Resources.SetupApplication ("Test");
		}
		
		[Test] public void CheckUnboundDataWidget()
		{
			Window window = new Window ();
			window.Text = "CheckUnboundDataWidget";
			window.Root.Padding = new Drawing.Margins (8, 8, 16, 16);
			
			Widgets.DataWidget data_widget = new Widgets.DataWidget ();
			
			data_widget.Dock   = DockStyle.Fill;
			data_widget.SetParent (window.Root);
			
			window.Show ();
		}
		
		[Test] public void CheckSimpleDataWidgets()
		{
			Data.Field field_1 = new Data.Field ("Field1", "Hello, world...", new Types.StringType ());
			Data.Field field_2 = new Data.Field ("Field2", 10.0M, new Types.DecimalType (-100, 100, 0.05M));
			Data.Field field_3 = new Data.Field ("Field3", Data.Representation.None);
			
			Window window = new Window ();
			window.Text = "CheckSimpleDataWidgets";
			window.Root.Padding = new Drawing.Margins (8, 8, 16, 16);
			
			Widgets.DataWidget data_widget_1 = new Widgets.DataWidget ();
			Widgets.DataWidget data_widget_2 = new Widgets.DataWidget ();
			Widgets.DataWidget data_widget_3 = new Widgets.DataWidget ();
			Widgets.DataWidget data_widget_4 = new Widgets.DataWidget ();
			Widgets.DataWidget data_widget_5 = new Widgets.DataWidget ();
			
			field_1.DefineCaption ("Text");
			field_2.DefineCaption ("Numeric value");
			field_3.DefineCaption ("Representation");
			
			data_widget_1.Dock           = DockStyle.Top;
			data_widget_1.SetParent (window.Root);
			data_widget_1.DataSource     = field_1;
			data_widget_1.Representation = Data.Representation.Automatic;
			data_widget_1.Margins        = new Drawing.Margins (0, 0, 0, 2);
			
			data_widget_2.Dock           = DockStyle.Top;
			data_widget_2.SetParent (window.Root);
			data_widget_2.DataSource     = field_2;
			data_widget_2.Representation = Data.Representation.Automatic;
			data_widget_2.Margins        = new Drawing.Margins (0, 0, 0, 2);
			
			data_widget_3.Dock           = DockStyle.Fill;
			data_widget_3.SetParent (window.Root);
			data_widget_3.DataSource     = field_3;
			data_widget_3.Representation = Data.Representation.RadioColumns;
			data_widget_3.Margins        = new Drawing.Margins (0, 0, 0, 2);
			
			data_widget_4.Dock           = DockStyle.Fill;
			data_widget_4.SetParent (window.Root);
			data_widget_4.DataSource     = field_3;
			data_widget_4.Representation = Data.Representation.RadioRows;
			data_widget_4.Margins        = new Drawing.Margins (0, 0, 0, 2);
			
			data_widget_5.Dock           = DockStyle.Fill;
			data_widget_5.SetParent (window.Root);
			data_widget_5.DataSource     = field_3;
			data_widget_5.Representation = Data.Representation.RadioList;
			data_widget_5.Margins        = new Drawing.Margins (0, 0, 0, 0);
			
			window.Show ();
		}
	}
}
