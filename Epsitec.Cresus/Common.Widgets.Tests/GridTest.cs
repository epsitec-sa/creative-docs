//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	[TestFixture]
	public class GridTest
	{
		[SetUp]
		public void Initialize()
		{
			Epsitec.Common.Widgets.Widget.Initialize ();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookMetal");
		}

		[Test]
		public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}

		[Test]
		public void CheckGridLength()
		{
			Layouts.GridLength length1 = new Layouts.GridLength ();
			Layouts.GridLength length2 = Layouts.GridLength.Auto;
			Layouts.GridLength length3 = new Layouts.GridLength (10);
			Layouts.GridLength length4 = new Layouts.GridLength (1, Layouts.GridUnitType.Proportional);
			Layouts.GridLength length5 = new Layouts.GridLength (2.5, Layouts.GridUnitType.Proportional);

			Types.ISerializationConverter conv = Common.Types.InvariantConverter.GetSerializationConverter (typeof (Layouts.GridLength));

			Assert.AreEqual ("Auto", conv.ConvertToString (length1, null));
			Assert.AreEqual ("Auto", conv.ConvertToString (length2, null));
			Assert.AreEqual ("10", conv.ConvertToString (length3, null));
			Assert.AreEqual ("*", conv.ConvertToString (length4, null));
			Assert.AreEqual ("*2.5", conv.ConvertToString (length5, null));

			Assert.AreEqual (length2, conv.ConvertFromString ("Auto", null));
			Assert.AreEqual (length3, conv.ConvertFromString ("10", null));
			Assert.AreEqual (length4, conv.ConvertFromString ("*", null));
			Assert.AreEqual (length5, conv.ConvertFromString ("*2.5", null));
		}
		
		[Test]
		public void CheckInteractiveGridLayout()
		{
			Window window = new Window ();

			window.ClientSize = new Size (400, 300);
			window.Text = "CheckInteractiveGrid";
			window.Root.Padding = new Margins (8, 8, 5, 5);

			Layouts.GridLayoutEngine grid = new Layouts.GridLayoutEngine ();

			grid.ColumnDefinitions.Add (new Layouts.ColumnDefinition (new Layouts.GridLength (1, Layouts.GridUnitType.Proportional)));
			grid.ColumnDefinitions.Add (new Layouts.ColumnDefinition ());
			grid.ColumnDefinitions.Add (new Layouts.ColumnDefinition ());
			grid.ColumnDefinitions.Add (new Layouts.ColumnDefinition ());
			grid.ColumnDefinitions.Add (new Layouts.ColumnDefinition ());

			grid.RowDefinitions.Add (new Layouts.RowDefinition (new Layouts.GridLength (30)));
			grid.RowDefinitions.Add (new Layouts.RowDefinition ());
			grid.RowDefinitions.Add (new Layouts.RowDefinition ());
			grid.RowDefinitions.Add (new Layouts.RowDefinition (50, double.PositiveInfinity));
			grid.RowDefinitions.Add (new Layouts.RowDefinition (new Layouts.GridLength (1, Layouts.GridUnitType.Proportional)));
			grid.RowDefinitions.Add (new Layouts.RowDefinition (new Layouts.GridLength (2, Layouts.GridUnitType.Proportional)));

			Button button;
			StaticText text;
			TextField field;

			text = new StaticText ();
			text.BackColor = Drawing.Color.FromRgb (0.7, 1, 0.7);
			text.Margins = new Margins (1, 1, 0, 0);
			Layouts.GridLayoutEngine.SetColumn (text, 3);
			Layouts.GridLayoutEngine.SetRow (text, 0);
			Layouts.GridLayoutEngine.SetRowSpan (text, 3);
			window.Root.Children.Add (text);

			button = new Button ();
			button.Text = "A";
			button.PreferredWidth = 40;
			Layouts.GridLayoutEngine.SetColumn (button, 0);
			Layouts.GridLayoutEngine.SetRow (button, 0);
			window.Root.Children.Add (button);

			button = new Button ();
			button.Text = "B";
			button.PreferredWidth = 60;
			Layouts.GridLayoutEngine.SetColumn (button, 1);
			Layouts.GridLayoutEngine.SetRow (button, 0);
			window.Root.Children.Add (button);

			button = new Button ();
			button.Text = "C";
			button.PreferredWidth = 20;
			Layouts.GridLayoutEngine.SetColumn (button, 2);
			Layouts.GridLayoutEngine.SetRow (button, 0);
			window.Root.Children.Add (button);

			text = new StaticText ();
			text.Text = "Xyz";
			text.PreferredWidth = 20;
			text.Margins = new Drawing.Margins (4, 4, 0, 0);
			text.ContentAlignment = ContentAlignment.BottomLeft;
			Layouts.GridLayoutEngine.SetColumn (text, 3);
			Layouts.GridLayoutEngine.SetRow (text, 0);
			window.Root.Children.Add (text);

			button = new Button ();
			button.Text = "E";
			button.PreferredWidth = 20;
			Layouts.GridLayoutEngine.SetColumn (button, 4);
			Layouts.GridLayoutEngine.SetRow (button, 0);
			window.Root.Children.Add (button);

			button = new Button ();
			button.Text = "A";
			button.PreferredWidth = 20;
			button.PreferredHeight = 60;
			Layouts.GridLayoutEngine.SetColumn (button, 0);
			Layouts.GridLayoutEngine.SetRow (button, 1);
			Layouts.GridLayoutEngine.SetRowSpan (button, 2);
			window.Root.Children.Add (button);

			button = new Button ();
			button.Text = "B";
			button.PreferredWidth = 20;
			Layouts.GridLayoutEngine.SetColumn (button, 1);
			Layouts.GridLayoutEngine.SetRow (button, 1);
			Layouts.GridLayoutEngine.SetColumnSpan (button, 2);
			window.Root.Children.Add (button);

			button = new Button ();
			button.Text = "C";
			button.Margins = new Margins (4, 4, 0, 0);
			button.PreferredWidth = 20;
			button.VerticalAlignment = VerticalAlignment.Center;
			Layouts.GridLayoutEngine.SetColumn (button, 3);
			Layouts.GridLayoutEngine.SetRow (button, 1);
			window.Root.Children.Add (button);

			field = new TextField ();
			field.Text = "B";
			field.PreferredWidth = 60;
			field.Margins = new Margins (4, 4, 0, 0);
			field.VerticalAlignment = VerticalAlignment.BaseLine;
			Layouts.GridLayoutEngine.SetColumn (field, 1);
			Layouts.GridLayoutEngine.SetRow (field, 2);
			window.Root.Children.Add (field);

			button = new Button ();
			button.Text = "C";
			button.PreferredWidth = 20;
			button.VerticalAlignment = VerticalAlignment.BaseLine;
			Layouts.GridLayoutEngine.SetColumn (button, 2);
			Layouts.GridLayoutEngine.SetRow (button, 2);
			window.Root.Children.Add (button);

			text = new StaticText ();
			text.Text = "Xyz";
			text.PreferredWidth = 20;
			text.Margins = new Drawing.Margins (4, 4, 0, 0);
			text.ContentAlignment = ContentAlignment.BottomLeft;
			text.VerticalAlignment = VerticalAlignment.BaseLine;
			Layouts.GridLayoutEngine.SetColumn (text, 3);
			Layouts.GridLayoutEngine.SetRow (text, 2);
			window.Root.Children.Add (text);

			button = new Button ();
			button.Text = "E";
			button.PreferredWidth = 20;
			button.VerticalAlignment = VerticalAlignment.BaseLine;
			Layouts.GridLayoutEngine.SetColumn (button, 4);
			Layouts.GridLayoutEngine.SetRow (button, 2);
			window.Root.Children.Add (button);


			button = new Button ();
			button.Text = "C-E";
			button.PreferredWidth = 100;
			button.VerticalAlignment = VerticalAlignment.Stretch;
			Layouts.GridLayoutEngine.SetColumn (button, 2);
			Layouts.GridLayoutEngine.SetRow (button, 3);
			Layouts.GridLayoutEngine.SetColumnSpan (button, 3);
			window.Root.Children.Add (button);

			button = new Button ();
			button.Text = "X (x1)";
			button.VerticalAlignment = VerticalAlignment.Stretch;
			Layouts.GridLayoutEngine.SetColumn (button, 0);
			Layouts.GridLayoutEngine.SetRow (button, 4);
			Layouts.GridLayoutEngine.SetColumnSpan (button, 5);
			window.Root.Children.Add (button);

			button = new Button ();
			button.Text = "Y (x2)";
			button.VerticalAlignment = VerticalAlignment.Stretch;
			Layouts.GridLayoutEngine.SetColumn (button, 0);
			Layouts.GridLayoutEngine.SetRow (button, 5);
			Layouts.GridLayoutEngine.SetColumnSpan (button, 5);
			window.Root.Children.Add (button);

			Layouts.LayoutEngine.SetLayoutEngine (window.Root, grid);

			window.Show ();
			Window.RunInTestEnvironment (window);
		}
	}
}
