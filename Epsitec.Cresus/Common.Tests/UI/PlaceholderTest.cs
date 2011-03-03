//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;

using Epsitec.Common.UI;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using System.Collections.Generic;
using Epsitec.Common.UI.Controllers;
using Epsitec.Common.Drawing;
using Epsitec.Common.Tests.UI;
using Epsitec.Common.Support;

[assembly: Controller (typeof (PlaceholderTest.Test1Controller))]

namespace Epsitec.Common.Tests.UI
{
	[TestFixture]
	public class PlaceholderTest
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
			IController c1 = ControllerFactory.CreateController ("Test1", new ControllerParameters ("x"));
			IController c2 = ControllerFactory.CreateController ("Test1", new ControllerParameters ("y"));

			Assert.IsNotNull (c1);
			Assert.IsNotNull (c2);

			Assert.AreEqual (typeof (Test1Controller), c1.GetType ());
			Assert.AreEqual (typeof (Test1Controller), c2.GetType ());

			Test1Controller tc1 = c1 as Test1Controller;
			Test1Controller tc2 = c2 as Test1Controller;

			Assert.AreEqual ("x", tc1.Parameters);
			Assert.AreEqual ("y", tc2.Parameters);
		}

		[Test]
		public void CheckControllerParameters()
		{
			ControllerParameters parameters1 = new ControllerParameters (null);
			ControllerParameters parameters2 = new ControllerParameters ("");
			ControllerParameters parameters3 = new ControllerParameters ("a");
			ControllerParameters parameters4 = new ControllerParameters ("a=1 b=2 c=x=y");
			ControllerParameters parameters5 = new ControllerParameters ("a=1 b=2 a=3");
			ControllerParameters parameters6 = new ControllerParameters ("a=0  c=x=y b=2 a=1 ");

			Assert.AreEqual (parameters1, parameters2);
			Assert.AreEqual (parameters4, parameters6);

			Assert.AreEqual (null, parameters1.GetParameterValue ("a"));
			Assert.AreEqual (null, parameters2.GetParameterValue ("a"));
			Assert.AreEqual ("", parameters3.GetParameterValue ("a"));
			Assert.AreEqual ("1", parameters4.GetParameterValue ("a"));
			Assert.AreEqual ("3", parameters5.GetParameterValue ("a"));

			Assert.AreEqual ("a=1 b=2 c=x=y", parameters6.ToString ());

			parameters6.SetParameterValue ("a", null);
			parameters6.SetParameterValue ("foo", "");
			parameters6.SetParameterValue ("bar", "123");
			
			Assert.AreEqual ("b=2 bar=123 c=x=y foo", parameters6.ToString ());
		}

		[Test]
		public void CheckGetMinSpan()
		{
			Widget root = new Widget ();
			Widget empty = new Widget ();

			StructuredType type = new StructuredType ();
			StructuredData data = new StructuredData (type);

			type.Fields.Add ("x", StringType.NativeDefault);
			data.SetValue ("x", "abc");

			DataObject.SetDataContext (root, new Binding (data));
			
			Placeholder placeholder = new Placeholder ();
			placeholder.SetBinding (Placeholder.ValueProperty, new Binding (BindingMode.TwoWay, "x"));

			int minColumnSpan;
			int minRowSpan;

			Assert.AreEqual ("*", placeholder.Controller);
			Assert.IsFalse (placeholder.GetMinSpan (null, 0, 0, out minColumnSpan, out minRowSpan));
			Assert.AreEqual (UndefinedValue.Value, placeholder.Value);
			Assert.IsFalse (placeholder.GetMinSpan (empty, 0, 0, out minColumnSpan, out minRowSpan));
			Assert.IsTrue (placeholder.GetMinSpan (root, 0, 0, out minColumnSpan, out minRowSpan));
			Assert.AreEqual (2, minColumnSpan);
			Assert.AreEqual (1, minRowSpan);
			Assert.AreEqual (UndefinedValue.Value, placeholder.Value);
		}

		[Test]
		public void CheckInteractiveControllers()
		{
			Window window = new Window ();

			Widgets.Layouts.GridLayoutEngine grid = new Widgets.Layouts.GridLayoutEngine ();

			grid.ColumnDefinitions.Add (new Widgets.Layouts.ColumnDefinition ());
			grid.ColumnDefinitions.Add (new Widgets.Layouts.ColumnDefinition (new Widgets.Layouts.GridLength (40)));
			grid.ColumnDefinitions.Add (new Widgets.Layouts.ColumnDefinition ());
			grid.ColumnDefinitions.Add (new Widgets.Layouts.ColumnDefinition (new Widgets.Layouts.GridLength (1, Widgets.Layouts.GridUnitType.Proportional), 60, double.PositiveInfinity));
			grid.ColumnDefinitions.Add (new Widgets.Layouts.ColumnDefinition ()); // en trop

			grid.RowDefinitions.Add (new Widgets.Layouts.RowDefinition ());
			grid.RowDefinitions.Add (new Widgets.Layouts.RowDefinition ());
			grid.RowDefinitions.Add (new Widgets.Layouts.RowDefinition ());
			grid.RowDefinitions.Add (new Widgets.Layouts.RowDefinition ());
			grid.RowDefinitions.Add (new Widgets.Layouts.RowDefinition (new Widgets.Layouts.GridLength (1, Widgets.Layouts.GridUnitType.Proportional)));
//			grid.RowDefinitions.Add (new Widgets.Layouts.RowDefinition ()); // en pas assez

			grid.ColumnDefinitions[0].RightBorder = 1;

			grid.RowDefinitions[0].BottomBorder = 1;
			grid.RowDefinitions[2].TopBorder = -1;
			grid.RowDefinitions[3].TopBorder = -1;

			Panel panel = new Epsitec.Common.UI.Panel ();

			StructuredType type = new StructuredType ();
			StructuredData data = new StructuredData (type);

			IntegerType ageType = new IntegerType (16, 80);
			ageType.DefinePreferredRange (new DecimalRange (20, 65, 10));

			type.Fields.Add ("Name", new StringType (1));
			type.Fields.Add ("Forename", new StringType (1));
			type.Fields.Add ("Age", ageType);
			type.Fields.Add ("Sex", new EnumType (typeof (Sex)));

			data.SetValue ("Name", "Arnaud");
			data.SetValue ("Forename", "Pierre");
			data.SetValue ("Age", System.DateTime.Now.Year - 1972);
			data.SetValue ("Sex", Sex.Male);

			panel.DataSource = new DataSource ();
			panel.DataSource.AddDataSource ("Person", data);

			Placeholder placeholder1 = new Placeholder ();
			Placeholder placeholder2 = new Placeholder ();
			Placeholder placeholder3 = new Placeholder ();
			Placeholder placeholder4 = new Placeholder ();

			placeholder1.Controller = "String";
			placeholder1.ControllerParameters = "Mode=Multiline";
			placeholder1.PreferredHeight = 40;
			placeholder1.TabIndex = 1;
			Widgets.Layouts.GridLayoutEngine.SetColumn (placeholder1, 0);
			Widgets.Layouts.GridLayoutEngine.SetRow (placeholder1, 1);
			Widgets.Layouts.GridLayoutEngine.SetColumnSpan (placeholder1, 4);

			placeholder2.Controller = "*";
			placeholder2.PreferredHeight = 20;
			placeholder2.TabIndex = 2;
			Widgets.Layouts.GridLayoutEngine.SetColumn (placeholder2, 0);
			Widgets.Layouts.GridLayoutEngine.SetRow (placeholder2, 2);
			Widgets.Layouts.GridLayoutEngine.SetColumnSpan (placeholder2, 4);

			placeholder3.Controller = "*";
			placeholder3.PreferredHeight = 20;
			placeholder3.TabIndex = 3;
			Widgets.Layouts.GridLayoutEngine.SetColumn (placeholder3, 0);
			Widgets.Layouts.GridLayoutEngine.SetRow (placeholder3, 3);

			placeholder4.Controller = "Enum";
			placeholder4.ControllerParameters = "Mode=Combo";
			placeholder4.PreferredHeight = 20;
			placeholder4.TabIndex = 4;
			Widgets.Layouts.GridLayoutEngine.SetColumn (placeholder4, 2);
			Widgets.Layouts.GridLayoutEngine.SetRow (placeholder4, 3);
			Widgets.Layouts.GridLayoutEngine.SetColumnSpan (placeholder4, 2);

			Binding binding1 = new Binding (BindingMode.TwoWay, "Person.Name");
			Binding binding2 = new Binding (BindingMode.TwoWay, "Person.Forename");
			Binding binding3 = new Binding (BindingMode.TwoWay, "Person.Age");
			Binding binding4 = new Binding (BindingMode.TwoWay, "Person.Sex");

			placeholder1.SetBinding (Placeholder.ValueProperty, binding1);
			placeholder2.SetBinding (Placeholder.ValueProperty, binding2);
			placeholder3.SetBinding (Placeholder.ValueProperty, binding3);
			placeholder4.SetBinding (Placeholder.ValueProperty, binding4);

			Widgets.Layouts.LayoutEngine.SetLayoutEngine (panel, grid);

			panel.Padding = new Margins (8, 8, 5, 5);
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
			text.BackColor = Color.FromBrightness (0.6);
			text.Margins = new Margins (0, 0, 0, 0);
			text.ContentAlignment = ContentAlignment.MiddleCenter;
			text.VerticalAlignment = VerticalAlignment.BaseLine;
			Widgets.Layouts.GridLayoutEngine.SetColumn (text, 0);
			Widgets.Layouts.GridLayoutEngine.SetRow (text, 0);
			panel.Children.Add (text);

			text = new StaticText ();
			text.Text = "Data fields";
			text.PreferredWidth = 40;
			text.PreferredHeight = 20;
			text.BackColor = Color.FromBrightness (0.6);
			text.Margins = new Margins (0, 0, 0, 0);
			text.ContentAlignment = ContentAlignment.MiddleCenter;
			text.VerticalAlignment = VerticalAlignment.BaseLine;
			Widgets.Layouts.GridLayoutEngine.SetColumn (text, 1);
			Widgets.Layouts.GridLayoutEngine.SetRow (text, 0);
			Widgets.Layouts.GridLayoutEngine.SetColumnSpan (text, 3);
			panel.Children.Add (text);

			text = new StaticText ();
			text.PreferredHeight = 20;
			Widgets.Layouts.GridLayoutEngine.SetColumn (text, 0);
			Widgets.Layouts.GridLayoutEngine.SetRow (text, 5);
			Widgets.Layouts.GridLayoutEngine.SetColumnSpan (text, 4);
			panel.Children.Add (text);

			StructureChangeListener listener = new StructureChangeListener (text);

			data.ValueChanged += listener.HandleValueChanged;

			window.Root.Children.Add (panel);
			window.Show ();

			Window.RunInTestEnvironment (window);
		}

		[Test]
		public void CheckInteractiveControllersWithAsyncBinding()
		{
			Window window = new Window ();

			Widgets.Layouts.GridLayoutEngine grid = new Widgets.Layouts.GridLayoutEngine ();

			grid.ColumnDefinitions.Add (new Widgets.Layouts.ColumnDefinition ());
			grid.ColumnDefinitions.Add (new Widgets.Layouts.ColumnDefinition (new Widgets.Layouts.GridLength (40)));
			grid.ColumnDefinitions.Add (new Widgets.Layouts.ColumnDefinition ());
			grid.ColumnDefinitions.Add (new Widgets.Layouts.ColumnDefinition (new Widgets.Layouts.GridLength (1, Widgets.Layouts.GridUnitType.Proportional), 60, double.PositiveInfinity));
			grid.ColumnDefinitions.Add (new Widgets.Layouts.ColumnDefinition ()); // en trop

			grid.RowDefinitions.Add (new Widgets.Layouts.RowDefinition ());
			grid.RowDefinitions.Add (new Widgets.Layouts.RowDefinition ());
			grid.RowDefinitions.Add (new Widgets.Layouts.RowDefinition ());
			grid.RowDefinitions.Add (new Widgets.Layouts.RowDefinition ());
			grid.RowDefinitions.Add (new Widgets.Layouts.RowDefinition ());
			grid.RowDefinitions.Add (new Widgets.Layouts.RowDefinition (new Widgets.Layouts.GridLength (1, Widgets.Layouts.GridUnitType.Proportional)));
			//			grid.RowDefinitions.Add (new Widgets.Layouts.RowDefinition ()); // en pas assez

			grid.ColumnDefinitions[0].RightBorder = 1;

			grid.RowDefinitions[0].BottomBorder = 1;
			grid.RowDefinitions[2].TopBorder = -1;
			grid.RowDefinitions[3].TopBorder = -1;

			Panel panel = new Epsitec.Common.UI.Panel ();

			StructuredType type = new StructuredType ();
			StructuredData data = new StructuredData (type);

			IntegerType ageType = new IntegerType (16, 80);
			ageType.DefinePreferredRange (new DecimalRange (20, 65, 10));

			type.Fields.Add ("Name", new StringType (1));
			type.Fields.Add ("Forename", new StringType (1));
			type.Fields.Add ("Slow", TypeRosetta.GetNamedTypeFromTypeObject (typeof (SlowIntValue)));
			type.Fields.Add ("Sex", new EnumType (typeof (Sex)));

			data.SetValue ("Name", "Arnaud");
			data.SetValue ("Forename", "Pierre");
			data.SetValue ("Slow", new SlowIntValue ());
			data.SetValue ("Sex", Sex.Male);

			panel.DataSource = new DataSource ();
			panel.DataSource.AddDataSource ("Person", data);

			Placeholder placeholder1 = new Placeholder ();
			Placeholder placeholder2 = new Placeholder ();
			Placeholder placeholder3 = new Placeholder ();
			Placeholder placeholder4 = new Placeholder ();
			Placeholder placeholder5 = new Placeholder ();
			Placeholder placeholder6 = new Placeholder ();

			placeholder1.Controller = "*";
			placeholder1.PreferredHeight = 20;
			placeholder1.TabIndex = 1;
			Widgets.Layouts.GridLayoutEngine.SetColumn (placeholder1, 0);
			Widgets.Layouts.GridLayoutEngine.SetRow (placeholder1, 1);
			Widgets.Layouts.GridLayoutEngine.SetColumnSpan (placeholder1, 4);

			placeholder2.Controller = "*";
			placeholder2.PreferredHeight = 20;
			placeholder2.TabIndex = 2;
			Widgets.Layouts.GridLayoutEngine.SetColumn (placeholder2, 0);
			Widgets.Layouts.GridLayoutEngine.SetRow (placeholder2, 2);
			Widgets.Layouts.GridLayoutEngine.SetColumnSpan (placeholder2, 4);

			placeholder3.Controller = "Numeric";
			placeholder3.PreferredHeight = 20;
			placeholder3.TabIndex = 3;
			Widgets.Layouts.GridLayoutEngine.SetColumn (placeholder3, 0);
			Widgets.Layouts.GridLayoutEngine.SetRow (placeholder3, 3);

			placeholder4.Controller = "Enum";
			placeholder4.ControllerParameters = "Mode=Combo";
			placeholder4.PreferredHeight = 20;
			placeholder4.TabIndex = 4;
			Widgets.Layouts.GridLayoutEngine.SetColumn (placeholder4, 2);
			Widgets.Layouts.GridLayoutEngine.SetRow (placeholder4, 3);
			Widgets.Layouts.GridLayoutEngine.SetColumnSpan (placeholder4, 2);

			placeholder5.Controller = "Numeric";
			placeholder5.PreferredHeight = 20;
			placeholder5.TabIndex = 5;
			Widgets.Layouts.GridLayoutEngine.SetColumn (placeholder5, 0);
			Widgets.Layouts.GridLayoutEngine.SetRow (placeholder5, 4);

			placeholder6.Controller = "Numeric";
			placeholder6.PreferredHeight = 20;
			placeholder6.TabIndex = 6;
			Widgets.Layouts.GridLayoutEngine.SetColumn (placeholder6, 2);
			Widgets.Layouts.GridLayoutEngine.SetRow (placeholder6, 4);


			Binding binding1 = new Binding (BindingMode.TwoWay, "Person.Name");
			Binding binding2 = new Binding (BindingMode.TwoWay, "Person.Forename");
			Binding binding3 = new Binding (BindingMode.OneWay, "Person.Slow.A");
			Binding binding4 = new Binding (BindingMode.TwoWay, "Person.Sex");
			Binding binding5 = new Binding (BindingMode.OneWay, "Person.Slow.B");
			Binding binding6 = new Binding (BindingMode.OneWay, "Person.Slow.C");

			binding3.IsAsync = true;
			binding5.IsAsync = true;
			binding6.IsAsync = true;

			placeholder1.SetBinding (Placeholder.ValueProperty, binding1);
			placeholder2.SetBinding (Placeholder.ValueProperty, binding2);
			placeholder3.SetBinding (Placeholder.ValueProperty, binding3);
			placeholder4.SetBinding (Placeholder.ValueProperty, binding4);
			placeholder5.SetBinding (Placeholder.ValueProperty, binding5);
			placeholder6.SetBinding (Placeholder.ValueProperty, binding6);

			Widgets.Layouts.LayoutEngine.SetLayoutEngine (panel, grid);

			panel.Padding = new Margins (8, 8, 5, 5);
			panel.Dock = DockStyle.Fill;

			panel.Children.Add (placeholder1);
			panel.Children.Add (placeholder2);
			panel.Children.Add (placeholder3);
			panel.Children.Add (placeholder4);
			panel.Children.Add (placeholder5);
			panel.Children.Add (placeholder6);

			StaticText text;

			text = new StaticText ();
			text.Text = "Label";
			text.PreferredWidth = 40;
			text.PreferredHeight = 20;
			text.BackColor = Color.FromBrightness (0.6);
			text.Margins = new Margins (0, 0, 0, 0);
			text.ContentAlignment = ContentAlignment.MiddleCenter;
			text.VerticalAlignment = VerticalAlignment.BaseLine;
			Widgets.Layouts.GridLayoutEngine.SetColumn (text, 0);
			Widgets.Layouts.GridLayoutEngine.SetRow (text, 0);
			panel.Children.Add (text);

			text = new StaticText ();
			text.Text = "Data fields";
			text.PreferredWidth = 40;
			text.PreferredHeight = 20;
			text.BackColor = Color.FromBrightness (0.6);
			text.Margins = new Margins (0, 0, 0, 0);
			text.ContentAlignment = ContentAlignment.MiddleCenter;
			text.VerticalAlignment = VerticalAlignment.BaseLine;
			Widgets.Layouts.GridLayoutEngine.SetColumn (text, 1);
			Widgets.Layouts.GridLayoutEngine.SetRow (text, 0);
			Widgets.Layouts.GridLayoutEngine.SetColumnSpan (text, 3);
			panel.Children.Add (text);

			text = new StaticText ();
			text.PreferredHeight = 20;
			Widgets.Layouts.GridLayoutEngine.SetColumn (text, 0);
			Widgets.Layouts.GridLayoutEngine.SetRow (text, 5);
			Widgets.Layouts.GridLayoutEngine.SetColumnSpan (text, 4);
			panel.Children.Add (text);

			StructureChangeListener listener = new StructureChangeListener (text);

			data.ValueChanged += listener.HandleValueChanged;

			window.Root.Children.Add (panel);
			window.Show ();

			Window.RunInTestEnvironment (window);
		}

		[Test]
		public void CheckBinding1()
		{
			Placeholder placeholder = new Placeholder ();

			StructuredType type = Res.Types.Record.Address;
			StructuredData data = new StructuredData (type);

			data.SetValue ("FirstName", "Pierre");

			Binding binding = new Binding (BindingMode.TwoWay, data, "FirstName");

			placeholder.SetBinding (AbstractPlaceholder.ValueProperty, binding);

			Application.ExecuteAsyncCallbacks ();

			Assert.AreEqual ("Pierre", placeholder.Value);
			Assert.AreEqual (2, placeholder.Children.Count);
			Assert.AreEqual ("Prénom", ((Widget) placeholder.Children[0]).Text);
			Assert.AreEqual ("Pierre", ((Widget) placeholder.Children[1]).Text);
		}

		[Test]
		public void CheckBinding2()
		{
			Widget      container   = new Widget ();
			Placeholder placeholder = new Placeholder ();

			container.Children.Add (placeholder);

			StructuredType type = Res.Types.Record.Address;
			StructuredData data1 = new StructuredData (type);
			StructuredData data2 = new StructuredData (type);

			data1.SetValue ("FirstName", "Cathi");
			data1.SetValue ("LastName", "Nicoud");

			data2.SetValue ("FirstName", "Pierre");
			data2.SetValue ("LastName", "Arnaud");

			StructuredType staffType = new StructuredType ();
			StructuredData staff = new StructuredData (staffType);

			staffType.Fields.Add (new StructuredTypeField ("Boss", type, Druid.Empty, 0, FieldRelation.Reference));
			staffType.Fields.Add (new StructuredTypeField ("Employees", type, Druid.Empty, 0, FieldRelation.Collection));

			List<StructuredData> list = new List<StructuredData> ();
			list.Add (data1);
			list.Add (data2);

			staff.SetValue ("Boss", data1);
			staff.SetValue ("Employees", list);

			DataObject.SetDataContext (container, new Binding (staff));
			
			Binding binding;
			
			binding = new Binding (BindingMode.TwoWay, "Boss.FirstName");
			placeholder.SetBinding (AbstractPlaceholder.ValueProperty, binding);

			Application.ExecuteAsyncCallbacks ();

			Assert.AreEqual ("Cathi", placeholder.Value);
			Assert.AreEqual (2, placeholder.Children.Count);
			Assert.AreEqual ("Prénom", ((Widget) placeholder.Children[0]).Text);
			Assert.AreEqual ("Cathi", ((Widget) placeholder.Children[1]).Text);

			//	And now, bind to the collection... which will need to magically instanciate
			//	a collection view !

			binding = new Binding (BindingMode.TwoWay, "Employees.LastName");
			placeholder.SetBinding (AbstractPlaceholder.ValueProperty, binding);

			Application.ExecuteAsyncCallbacks ();

			Assert.AreEqual ("Nicoud", placeholder.Value);
			Assert.AreEqual (2, placeholder.Children.Count);
			Assert.AreEqual ("Nom", ((Widget) placeholder.Children[0]).Text);
			Assert.AreEqual ("Nicoud", ((Widget) placeholder.Children[1]).Text);

			TablePlaceholder table = new TablePlaceholder ();

			container.Children.Add (table);

			binding = new Binding (BindingMode.TwoWay, "Employees");
			table.SetBinding (TablePlaceholder.ItemsProperty, binding);

			Assert.AreEqual (list, table.CollectionView.SourceCollection);

			table.CollectionView.MoveCurrentToNext ();
			Assert.AreEqual ("Arnaud", placeholder.Value);
		}

		[Test]
		public void CheckTablePlaceholderSerialization()
		{
			TablePlaceholder placeholder = new TablePlaceholder ();

			placeholder.SourceTypeId = Res.Types.Record.Address.CaptionId;
			
			placeholder.Columns.Add ("FirstName");
			placeholder.Columns.Add ("LastName");
			placeholder.Columns.Add ("Company");

			Panel panel = new Panel ();

			panel.ResourceManager = Epsitec.Common.Support.Resources.DefaultManager;
			panel.Children.Add (placeholder);

			string xml = Panel.SerializePanel (panel);

			panel = Panel.DeserializePanel (xml, null, Epsitec.Common.Support.Resources.DefaultManager);
			placeholder = panel.Children[0] as TablePlaceholder;

			Assert.AreEqual (3, placeholder.Columns.Count);
			Assert.AreEqual ("FirstName", placeholder.Columns[0].FieldId);
			Assert.AreEqual ("LastName", placeholder.Columns[1].FieldId);
			Assert.AreEqual ("Company", placeholder.Columns[2].FieldId);
			Assert.AreEqual (Res.Types.Record.Address.CaptionId, placeholder.SourceTypeId);
		}


		private class SlowIntValue : DependencyObject
		{
			public int A
			{
				get
				{
					return (int) this.GetValue (SlowIntValue.AProperty);
				}
			}

			public int B
			{
				get
				{
					return (int) this.GetValue (SlowIntValue.BProperty);
				}
			}

			public int C
			{
				get
				{
					return (int) this.GetValue (SlowIntValue.CProperty);
				}
			}

			private static object GetAValue(DependencyObject o)
			{
				System.Threading.Thread.Sleep (3*1000);
				return 35;
			}

			private static object GetBValue(DependencyObject o)
			{
				System.Threading.Thread.Sleep (5*1000);
				return 36;
			}

			private static object GetCValue(DependencyObject o)
			{
				System.Threading.Thread.Sleep (10*1000);
				return 37;
			}

			public static DependencyProperty AProperty	= DependencyProperty.RegisterReadOnly ("A", typeof (int), typeof (SlowIntValue), new DependencyPropertyMetadata (SlowIntValue.GetAValue));
			public static DependencyProperty BProperty	= DependencyProperty.RegisterReadOnly ("B", typeof (int), typeof (SlowIntValue), new DependencyPropertyMetadata (SlowIntValue.GetBValue));
			public static DependencyProperty CProperty	= DependencyProperty.RegisterReadOnly ("C", typeof (int), typeof (SlowIntValue), new DependencyPropertyMetadata (SlowIntValue.GetCValue));
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
					//					buffer.Append (", ");
					//					buffer.Append (data.GetValue ("Slow.Age"));
					//					buffer.Append (" years old");
					buffer.Append ("; ");
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
			Unknown,
			Male,
			Female
		}

		[Test]
		public void CheckValueTypeObjectAndValueName()
		{
			Panel panel = new Epsitec.Common.UI.Panel ();

			StructuredType type = new StructuredType ();
			StructuredData data = new StructuredData (type);

			type.Fields.Add ("Name", new StringType ());
			type.Fields.Add ("Forename", new StringType ());
			type.Fields.Add ("Age", new IntegerType (1, 150));

			data.SetValue ("Name", "Arnaud");
			data.SetValue ("Forename", "Pierre");
			data.SetValue ("Age", System.DateTime.Now.Year - 1972);

			panel.DataSource = new DataSource ();
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

			Assert.AreEqual ("Name", placeholder1.ValueName);
			Assert.AreEqual ("Forename", placeholder2.ValueName);
			Assert.AreEqual ("Age", placeholder3.ValueName);

			Assert.AreEqual (typeof (StringType), placeholder1.ValueType.GetType ());
			Assert.AreEqual (typeof (StringType), placeholder2.ValueType.GetType ());
			Assert.AreEqual (typeof (IntegerType), placeholder3.ValueType.GetType ());
		}

		#region TestController1 Class

		internal class Test1Controller : AbstractController
		{
			public Test1Controller(ControllerParameters parameters)
				: base (parameters)
			{
			}

			public string Parameters
			{
				get
				{
					return this.ControllerParameters.ToString ();
				}
			}

			protected override void CreateUserInterface(INamedType valueType, Caption caption)
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}

			protected override void RefreshUserInterface(object oldValue, object newValue)
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}
		}

		#endregion
	}
}
