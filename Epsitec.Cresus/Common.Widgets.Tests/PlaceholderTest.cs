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
		public void CheckStringController()
		{
			StructuredType type = new StructuredType ();
			StructuredData data = new StructuredData (type);

			type.AddField ("Name", new StringType ());
			type.AddField ("Forename", new StringType ());
			type.AddField ("Age", new IntegerType (1, 150));

			data.SetValue ("Name", "Arnaud");
			data.SetValue ("Forename", "Pierre");
			data.SetValue ("Age", System.DateTime.Now.Year - 1972);

			Window window = new Window ();
			UI.Panel panel = new Epsitec.Common.UI.Panel ();

			panel.DataSource = new UI.DataSourceCollection ();
			panel.DataSource.AddDataSource ("Person", data);
			
			Placeholder placeholder1 = new Placeholder ();
			Placeholder placeholder2 = new Placeholder ();
			Placeholder placeholder3 = new Placeholder ();

			placeholder1.Dock = DockStyle.Top;
			placeholder1.Controller = "StringController";
			placeholder1.PreferredHeight = 20;
			placeholder1.TabIndex = 1;

			placeholder2.Dock = DockStyle.Top;
			placeholder2.Controller = "StringController";
			placeholder2.PreferredHeight = 20;
			placeholder2.TabIndex = 2;

			placeholder3.Dock = DockStyle.Top;
			placeholder3.Controller = "StringController";
			placeholder3.PreferredHeight = 20;
			placeholder3.TabIndex = 3;

			Binding binding1 = new Binding (BindingMode.TwoWay, null, "Person.Name");
			Binding binding2 = new Binding (BindingMode.TwoWay, null, "Person.Forename");
			Binding binding3 = new Binding (BindingMode.TwoWay, null, "Person.Age");

			placeholder1.SetBinding (Placeholder.ValueProperty, binding1);
			placeholder2.SetBinding (Placeholder.ValueProperty, binding2);
			placeholder3.SetBinding (Placeholder.ValueProperty, binding3);

			panel.Dock = DockStyle.Fill;
			
			panel.Children.Add (placeholder1);
			panel.Children.Add (placeholder2);
			panel.Children.Add (placeholder3);
			
			window.Root.Children.Add (panel);
			window.Show ();
			
			Window.RunInTestEnvironment (window);
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
