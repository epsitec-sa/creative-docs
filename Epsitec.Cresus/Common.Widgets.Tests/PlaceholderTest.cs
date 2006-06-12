//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

[assembly: Controller (typeof (PlaceholderTest.TestController1))]

namespace Epsitec.Common.Widgets
{
	[TestFixture] public class PlaceholderTest
	{
		[Test]
		public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}

		[Test]
		public void CheckControllerCreation()
		{
			IController c1 = Controllers.Factory.CreateController ("TestController1", "x");
			IController c2 = Controllers.Factory.CreateController ("TestController1", "y");

			Assert.IsNotNull (c1);
			Assert.IsNotNull (c2);

			Assert.AreEqual (typeof (TestController1), c1.GetType ());
			Assert.AreEqual (typeof (TestController1), c2.GetType ());

			TestController1 tc1 = c1 as TestController1;
			TestController1 tc2 = c2 as TestController1;

			Assert.AreEqual ("x", tc1.Parameter);
			Assert.AreEqual ("y", tc2.Parameter);
		}

		[Test]
		public void CheckInteractiveStringController()
		{
			Window window = new Window ();

			Layouts.GridLayoutEngine grid = new Layouts.GridLayoutEngine ();

			grid.ColumnDefinitions.Add (new Layouts.ColumnDefinition ());
			grid.ColumnDefinitions.Add (new Layouts.ColumnDefinition (new Layouts.GridLength (40)));
			grid.ColumnDefinitions.Add (new Layouts.ColumnDefinition (new Layouts.GridLength (1, Layouts.GridUnitType.Proportional)));

			grid.RowDefinitions.Add (new Layouts.RowDefinition ());
			grid.RowDefinitions.Add (new Layouts.RowDefinition ());
			grid.RowDefinitions.Add (new Layouts.RowDefinition ());
			grid.RowDefinitions.Add (new Layouts.RowDefinition ());

			UI.Panel panel = new Epsitec.Common.UI.Panel ();

			StructuredType type = new StructuredType ();
			StructuredData data = new StructuredData (type);

			type.AddField ("Name", new StringType ());
			type.AddField ("Forename", new StringType ());
			type.AddField ("Age", new IntegerType (1, 150));

			data.SetValue ("Name", "Arnaud");
			data.SetValue ("Forename", "Pierre");
			data.SetValue ("Age", System.DateTime.Now.Year - 1972);

			panel.DataSource = new UI.DataSourceCollection ();
			panel.DataSource.AddDataSource ("Person", data);
			
			Placeholder placeholder1 = new Placeholder ();
			Placeholder placeholder2 = new Placeholder ();
			Placeholder placeholder3 = new Placeholder ();

			placeholder1.Controller = "String";
			placeholder1.PreferredHeight = 20;
			placeholder1.TabIndex = 1;
			Layouts.GridLayoutEngine.SetColumn (placeholder1, 0);
			Layouts.GridLayoutEngine.SetRow (placeholder1, 1);
			Layouts.GridLayoutEngine.SetColumnSpan (placeholder1, 3);

			placeholder2.Controller = "String";
			placeholder2.PreferredHeight = 20;
			placeholder2.TabIndex = 2;
			Layouts.GridLayoutEngine.SetColumn (placeholder2, 0);
			Layouts.GridLayoutEngine.SetRow (placeholder2, 2);
			Layouts.GridLayoutEngine.SetColumnSpan (placeholder2, 3);

			placeholder3.Controller = "String";
			placeholder3.PreferredHeight = 20;
			placeholder3.TabIndex = 3;
			Layouts.GridLayoutEngine.SetColumn (placeholder3, 0);
			Layouts.GridLayoutEngine.SetRow (placeholder3, 3);

			Binding binding1 = new Binding (BindingMode.TwoWay, "Person.Name");
			Binding binding2 = new Binding (BindingMode.TwoWay, "Person.Forename");
			Binding binding3 = new Binding (BindingMode.TwoWay, "Person.Age");

			placeholder1.SetBinding (Placeholder.ValueProperty, binding1);
			placeholder2.SetBinding (Placeholder.ValueProperty, binding2);
			placeholder3.SetBinding (Placeholder.ValueProperty, binding3);

			Layouts.LayoutEngine.SetLayoutEngine (panel, grid);

			panel.Padding = new Drawing.Margins (8, 8, 5, 5);
			panel.Dock = DockStyle.Fill;
			
			panel.Children.Add (placeholder1);
			panel.Children.Add (placeholder2);
			panel.Children.Add (placeholder3);

			StaticText text;

			text = new StaticText ();
			text.Text = "Label";
			text.PreferredWidth = 40;
			text.PreferredHeight = 20;
			text.BackColor = Drawing.Color.FromBrightness (0.6);
			text.Margins = new Drawing.Margins (0, 1, 0, 1);
			text.ContentAlignment = Drawing.ContentAlignment.MiddleCenter;
			text.VerticalAlignment = VerticalAlignment.BaseLine;
			Layouts.GridLayoutEngine.SetColumn (text, 0);
			Layouts.GridLayoutEngine.SetRow (text, 0);
			panel.Children.Add (text);
			
			text = new StaticText ();
			text.Text = "Data fields";
			text.PreferredWidth = 40;
			text.PreferredHeight = 20;
			text.BackColor = Drawing.Color.FromBrightness (0.6);
			text.Margins = new Drawing.Margins (0, 0, 0, 1);
			text.ContentAlignment = Drawing.ContentAlignment.MiddleCenter;
			text.VerticalAlignment = VerticalAlignment.BaseLine;
			Layouts.GridLayoutEngine.SetColumn (text, 1);
			Layouts.GridLayoutEngine.SetRow (text, 0);
			Layouts.GridLayoutEngine.SetColumnSpan (text, 2);
			panel.Children.Add (text);

			window.Root.Children.Add (panel);
			window.Show ();
			
			Window.RunInTestEnvironment (window);
		}

		[Test]
		public void CheckValueTypeObjectAndValueName()
		{
			UI.Panel panel = new Epsitec.Common.UI.Panel ();

			StructuredType type = new StructuredType ();
			StructuredData data = new StructuredData (type);

			type.AddField ("Name", new StringType ());
			type.AddField ("Forename", new StringType ());
			type.AddField ("Age", new IntegerType (1, 150));

			data.SetValue ("Name", "Arnaud");
			data.SetValue ("Forename", "Pierre");
			data.SetValue ("Age", System.DateTime.Now.Year - 1972);

			panel.DataSource = new UI.DataSourceCollection ();
			panel.DataSource.AddDataSource ("Person", data);

			Placeholder placeholder1 = new Placeholder ();
			Placeholder placeholder2 = new Placeholder ();
			Placeholder placeholder3 = new Placeholder ();

			placeholder1.Controller = "String";
			placeholder2.Controller = "String";
			placeholder3.Controller = "String";

			Binding binding1 = new Binding (BindingMode.TwoWay, "Person.Name");
			Binding binding2 = new Binding (BindingMode.TwoWay, "Person.Forename");
			Binding binding3 = new Binding (BindingMode.TwoWay, "Person.Age");

			placeholder1.SetBinding (Placeholder.ValueProperty, binding1);
			placeholder2.SetBinding (Placeholder.ValueProperty, binding2);
			placeholder3.SetBinding (Placeholder.ValueProperty, binding3);

			panel.Children.Add (placeholder1);
			panel.Children.Add (placeholder2);
			panel.Children.Add (placeholder3);

			Assert.AreEqual ("Name",		placeholder1.ValueName);
			Assert.AreEqual ("Forename",	placeholder2.ValueName);
			Assert.AreEqual ("Age",			placeholder3.ValueName);

			Assert.AreEqual (typeof (StringType),	placeholder1.ValueTypeObject.GetType ());
			Assert.AreEqual (typeof (StringType),	placeholder2.ValueTypeObject.GetType ());
			Assert.AreEqual (typeof (IntegerType),	placeholder3.ValueTypeObject.GetType ());
		}

		#region TestController1 Class

		internal class TestController1 : Controllers.AbstractController
		{
			public TestController1(string parameter)
			{
				this.parameter = parameter;
			}

			public string Parameter
			{
				get
				{
					return this.parameter;
				}
			}
			
			protected override void CreateUserInterface(object valueTypeObject, string valueName)
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}

			protected override void RefreshUserInterface(object oldValue, object newValue)
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}

			private string parameter;
		}
		
		#endregion
	}
}
