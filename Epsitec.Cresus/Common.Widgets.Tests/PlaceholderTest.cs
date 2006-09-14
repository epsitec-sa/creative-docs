//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

[assembly: Controller (typeof (PlaceholderTest.Test1Controller))]

namespace Epsitec.Common.Widgets
{
	[TestFixture] public class PlaceholderTest
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
		public void CheckControllerCreation()
		{
			IController c1 = Controllers.Factory.CreateController ("Test1", "x");
			IController c2 = Controllers.Factory.CreateController ("Test1", "y");

			Assert.IsNotNull (c1);
			Assert.IsNotNull (c2);

			Assert.AreEqual (typeof (Test1Controller), c1.GetType ());
			Assert.AreEqual (typeof (Test1Controller), c2.GetType ());

			Test1Controller tc1 = c1 as Test1Controller;
			Test1Controller tc2 = c2 as Test1Controller;

			Assert.AreEqual ("x", tc1.Parameter);
			Assert.AreEqual ("y", tc2.Parameter);
		}

		[Test]
		public void CheckInteractiveControllers()
		{
			Window window = new Window ();

			Layouts.GridLayoutEngine grid = new Layouts.GridLayoutEngine ();

			grid.ColumnDefinitions.Add (new Layouts.ColumnDefinition ());
			grid.ColumnDefinitions.Add (new Layouts.ColumnDefinition (new Layouts.GridLength (40)));
			grid.ColumnDefinitions.Add (new Layouts.ColumnDefinition ());
			grid.ColumnDefinitions.Add (new Layouts.ColumnDefinition (new Layouts.GridLength (1, Layouts.GridUnitType.Proportional), 60, double.PositiveInfinity));
			grid.ColumnDefinitions.Add (new Layouts.ColumnDefinition ()); // en trop

			grid.RowDefinitions.Add (new Layouts.RowDefinition ());
			grid.RowDefinitions.Add (new Layouts.RowDefinition ());
			grid.RowDefinitions.Add (new Layouts.RowDefinition ());
			grid.RowDefinitions.Add (new Layouts.RowDefinition ());
			grid.RowDefinitions.Add (new Layouts.RowDefinition (new Layouts.GridLength (1, Layouts.GridUnitType.Proportional)));
//			grid.RowDefinitions.Add (new Layouts.RowDefinition ()); // en pas assez

			grid.ColumnDefinitions[0].RightBorder = 1;
			
			grid.RowDefinitions[0].BottomBorder = 1;
			grid.RowDefinitions[2].TopBorder = -1;
			grid.RowDefinitions[3].TopBorder = -1;

			UI.Panel panel = new Epsitec.Common.UI.Panel ();

			StructuredType type = new StructuredType ();
			StructuredData data = new StructuredData (type);

			IntegerType ageType = new IntegerType (16, 80);
			ageType.DefinePreferredRange (new DecimalRange (20, 65, 10));
			
			type.AddField ("Name", new StringType (1));
			type.AddField ("Forename", new StringType (1));
			type.AddField ("Age", ageType);
			type.AddField ("Sex", new EnumType (typeof (Sex)));

			data.SetValue ("Name", "Arnaud");
			data.SetValue ("Forename", "Pierre");
			data.SetValue ("Age", System.DateTime.Now.Year - 1972);
			data.SetValue ("Sex", Sex.Male);

			panel.DataSource = new UI.DataSourceCollection ();
			panel.DataSource.AddDataSource ("Person", data);
			
			Placeholder placeholder1 = new Placeholder ();
			Placeholder placeholder2 = new Placeholder ();
			Placeholder placeholder3 = new Placeholder ();
			Placeholder placeholder4 = new Placeholder ();

			placeholder1.Controller = "*";
			placeholder1.PreferredHeight = 20;
			placeholder1.TabIndex = 1;
			Layouts.GridLayoutEngine.SetColumn (placeholder1, 0);
			Layouts.GridLayoutEngine.SetRow (placeholder1, 1);
			Layouts.GridLayoutEngine.SetColumnSpan (placeholder1, 4);

			placeholder2.Controller = "*";
			placeholder2.PreferredHeight = 20;
			placeholder2.TabIndex = 2;
			Layouts.GridLayoutEngine.SetColumn (placeholder2, 0);
			Layouts.GridLayoutEngine.SetRow (placeholder2, 2);
			Layouts.GridLayoutEngine.SetColumnSpan (placeholder2, 4);

			placeholder3.Controller = "*";
			placeholder3.PreferredHeight = 20;
			placeholder3.TabIndex = 3;
			Layouts.GridLayoutEngine.SetColumn (placeholder3, 0);
			Layouts.GridLayoutEngine.SetRow (placeholder3, 3);

			placeholder4.Controller = "*";
			placeholder4.PreferredHeight = 20;
			placeholder4.TabIndex = 4;
			Layouts.GridLayoutEngine.SetColumn (placeholder4, 2);
			Layouts.GridLayoutEngine.SetRow (placeholder4, 3);
			Layouts.GridLayoutEngine.SetColumnSpan (placeholder4, 2);

			Binding binding1 = new Binding (BindingMode.TwoWay, "Person.Name");
			Binding binding2 = new Binding (BindingMode.TwoWay, "Person.Forename");
			Binding binding3 = new Binding (BindingMode.TwoWay, "Person.Age");
			Binding binding4 = new Binding (BindingMode.TwoWay, "Person.Sex");

			placeholder1.SetBinding (Placeholder.ValueProperty, binding1);
			placeholder2.SetBinding (Placeholder.ValueProperty, binding2);
			placeholder3.SetBinding (Placeholder.ValueProperty, binding3);
			placeholder4.SetBinding (Placeholder.ValueProperty, binding4);

			Layouts.LayoutEngine.SetLayoutEngine (panel, grid);

			panel.Padding = new Drawing.Margins (8, 8, 5, 5);
			panel.Dock = DockStyle.Fill;
			
			panel.Children.Add (placeholder1);
			panel.Children.Add (placeholder2);
			panel.Children.Add (placeholder3);
			panel.Children.Add (placeholder4);

			StaticText text;

			text = new StaticText ();
			text.Text = "Label";
			text.PreferredWidth = 40;
			text.PreferredHeight = 20;
			text.BackColor = Drawing.Color.FromBrightness (0.6);
			text.Margins = new Drawing.Margins (0, 0, 0, 0);
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
			text.Margins = new Drawing.Margins (0, 0, 0, 0);
			text.ContentAlignment = Drawing.ContentAlignment.MiddleCenter;
			text.VerticalAlignment = VerticalAlignment.BaseLine;
			Layouts.GridLayoutEngine.SetColumn (text, 1);
			Layouts.GridLayoutEngine.SetRow (text, 0);
			Layouts.GridLayoutEngine.SetColumnSpan (text, 3);
			panel.Children.Add (text);

			text = new StaticText ();
			text.PreferredHeight = 20;
			Layouts.GridLayoutEngine.SetColumn (text, 0);
			Layouts.GridLayoutEngine.SetRow (text, 5);
			Layouts.GridLayoutEngine.SetColumnSpan (text, 4);
			panel.Children.Add (text);

			StructureChangeListener listener = new StructureChangeListener (text);

			data.ValueChanged += listener.HandleValueChanged;

			window.Root.Children.Add (panel);
			window.Show ();
			
			Window.RunInTestEnvironment (window);
		}

		private class StructureChangeListener
		{
			public StructureChangeListener(Widget widget)
			{
				this.widget = widget;
			}

			public void HandleValueChanged(object sender, DependencyPropertyChangedEventArgs e)
			{
				StructuredData data = sender as StructuredData;
				
				if (data != null)
				{
					System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
					buffer.Append (data.GetValue ("Forename"));
					buffer.Append (" ");
					buffer.Append (data.GetValue ("Name"));
					buffer.Append (", ");
					buffer.Append (data.GetValue ("Age"));
					buffer.Append (" years old; ");
					buffer.Append (data.GetValue ("Sex"));
					buffer.Append (", ");
					buffer.Append (@"<font size=""80%"">(");
					buffer.Append (this.counter++);
					buffer.Append (@" changes)</font>");
					
					this.widget.Text = buffer.ToString ();
				}
			}
			
			Widget widget;
			int counter = 1;
		}

		enum Sex
		{
			Unknown, Male, Female
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

			Assert.AreEqual (typeof (StringType),	placeholder1.ValueType.GetType ());
			Assert.AreEqual (typeof (StringType),	placeholder2.ValueType.GetType ());
			Assert.AreEqual (typeof (IntegerType),	placeholder3.ValueType.GetType ());
		}

		#region TestController1 Class

		internal class Test1Controller : Controllers.AbstractController
		{
			public Test1Controller(string parameter)
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
			
			protected override void CreateUserInterface(INamedType valueType, string valueName, Caption caption)
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
