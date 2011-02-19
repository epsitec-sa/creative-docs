//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;

using System.Collections.Generic;
using Epsitec.Common.Types;

namespace Epsitec.Common.UI
{
	[TestFixture]
	public class PanelTest
	{
		[SetUp]
		public void Initialize()
		{
			Epsitec.Common.Document.Engine.Initialize ();
			Epsitec.Common.Widgets.Widget.Initialize ();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookMetal");
		}
		
		[Test]
		public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}

		[Test]
		public void CheckDataContext()
		{
			Panel panel = new Panel ();
			DataSourceMetadata metadata = panel.DataSourceMetadata;
			DataSource source1 = new DataSource ();
			DataSource source2 = new DataSource ();

			panel.DataSource = source1;

			Widgets.Widget    root  = new Widgets.Widget ();
			Widgets.TextField field = new Widgets.TextField (panel);

			Binding binding;

			binding = DataObject.GetDataContext (panel);

			Assert.AreEqual (source1, binding.Source);
			Assert.AreEqual (null, binding.Path);

			binding = DataObject.GetDataContext (field);

			Assert.AreEqual (source1, binding.Source);
			Assert.AreEqual (null, binding.Path);
			
			DataObject.SetDataContext (root, new Binding (this, "Dummy"));

			binding = DataObject.GetDataContext (root);

			Assert.AreEqual (this, binding.Source);
			Assert.AreEqual ("Dummy", binding.Path);

			//	Verify that the edition panel inherits the data context of its
			//	owner panel, not from its parent widget.
			
			Panel edition = panel.GetPanel (PanelMode.Edition);
			edition.SetEmbedder (root);

			binding = DataObject.GetDataContext (edition);

			Assert.AreEqual (source1, binding.Source);
			Assert.AreEqual (null, binding.Path);

			//	Changing the panel data source will also change the edition panel
			//	data context accordingly.
			
			panel.DataSource = source2;

			binding = DataObject.GetDataContext (edition);

			Assert.AreEqual (source2, binding.Source);
			Assert.AreEqual (null, binding.Path);
		}

		[Test]
		public void CheckInteractive()
		{
			Widgets.Window window = new Widgets.Window ();
			
			window.Text = "PanelTest.CheckInteractive";
			window.ClientSize = new Drawing.Size (400, 300);

			PanelStack panelStack = new PanelStack ();
			
			panelStack.Dock = Widgets.DockStyle.Fill;
			panelStack.Margins = new Drawing.Margins (8, 8, 4, 4);
			panelStack.BackColor = Drawing.Color.FromRgb (1, 0.8, 0.8);

			Panel panel = new Panel ();

			Widgets.StaticText static1 = new Widgets.StaticText (panel);
			Widgets.StaticText static2 = new Widgets.StaticText (panel);

			static1.Dock = Widgets.DockStyle.Top;
			static2.Dock = Widgets.DockStyle.Top;
			static1.Text = "Arnaud";
			static2.Text = "Pierre";
			static1.Name = "LastName";
			static2.Name = "FirstName";
			
			panel.PreferredSize = new Drawing.Size (80, 40);
			panel.Anchor = Widgets.AnchorStyles.TopRight;
			panel.Margins = new Drawing.Margins (4, 4, 4, 4);
			panel.Padding = new Drawing.Margins (8, 8, 4, 4);
			panel.BackColor = Drawing.Color.FromRgb (0.8, 1, 0.8);
			panel.DrawFullFrame = true;

			panel.EditionPanel.PreferredSize = new Drawing.Size (180, 68);
			
			Widgets.StaticText text1 = new Widgets.StaticText ();
			Widgets.StaticText text2 = new Widgets.StaticText ();
			Widgets.FormTextField field1 = new Widgets.FormTextField ();
			Widgets.FormTextField field2 = new Widgets.FormTextField ();
			
			text1.Text = "Nom :";
			text1.ContentAlignment = Epsitec.Common.Drawing.ContentAlignment.MiddleRight;
			text1.Anchor = Epsitec.Common.Widgets.AnchorStyles.TopLeft;
			text1.Margins = new Epsitec.Common.Drawing.Margins (4, 0, 4, 0);
			text1.PreferredSize = new Epsitec.Common.Drawing.Size (60, 24);

			text2.Text = "Prénom :";
			text2.ContentAlignment = Epsitec.Common.Drawing.ContentAlignment.MiddleRight;
			text2.Anchor = Epsitec.Common.Widgets.AnchorStyles.TopLeft;
			text2.Margins = new Epsitec.Common.Drawing.Margins (4, 0, 28, 0);
			text2.PreferredSize = new Epsitec.Common.Drawing.Size (60, 24);

			field1.Text = "Arnaud";
			field1.TabIndex = 1;
			field1.TabNavigationMode = Widgets.TabNavigationMode.ActivateOnTab;
			field1.Anchor = Epsitec.Common.Widgets.AnchorStyles.TopLeft | Epsitec.Common.Widgets.AnchorStyles.Right;
			field1.Margins = new Epsitec.Common.Drawing.Margins (68, 4, 4+4, 0);
			field1.PreferredSize = new Epsitec.Common.Drawing.Size (100, 24);
			field1.Name = "LastName";			
			
			field2.Text = "Pierre";
			field2.TabIndex = 2;
			field2.TabNavigationMode = Widgets.TabNavigationMode.ActivateOnTab;
			field2.Anchor = Epsitec.Common.Widgets.AnchorStyles.TopLeft | Epsitec.Common.Widgets.AnchorStyles.Right;
			field2.Margins = new Epsitec.Common.Drawing.Margins (68, 4, 28+4, 0);
			field2.PreferredSize = new Epsitec.Common.Drawing.Size (100, 24);
			field2.Name = "FirstName";

			panel.EditionPanel.Padding = new Drawing.Margins (2, 2, 2, 2);
			panel.EditionPanel.Children.Add (text1);
			panel.EditionPanel.Children.Add (text2);
			panel.EditionPanel.Children.Add (field1);
			panel.EditionPanel.Children.Add (field2);

			panelStack.Children.Add (panel);

			window.Root.Children.Add (panelStack);

			window.Show ();
			
			Widgets.Window.RunInTestEnvironment (window);
		}

		[Test]
		public void CheckSampleDataSource()
		{
			StringType strType = new StringType ();
			IntegerType intType = new IntegerType (1, 999);

			strType.DefineDefaultValue ("");
			strType.DefineSampleValue ("Abc");

			intType.DefineDefaultValue (1);
			intType.DefineSampleValue (123);

			Panel panel = new Panel ();
			DataSourceMetadata metadata = panel.DataSourceMetadata;

			StructuredType recType = new StructuredType ();

			recType.Fields.Add ("A", strType);
			recType.Fields.Add ("B", intType);
			recType.Fields.Add ("R", recType);

			metadata.Fields.Add (new StructuredTypeField ("Record", recType));

			string xml = Panel.SerializePanel (panel);

			System.Console.Out.WriteLine (xml);

			panel.SetupSampleDataSource ();

			Assert.AreEqual ("Abc", StructuredTree.GetValue (panel.DataSource, "Record.A"));
			Assert.AreEqual (123, StructuredTree.GetValue (panel.DataSource, "Record.B"));
			Assert.AreEqual ("Abc", StructuredTree.GetValue (panel.DataSource, "Record.R.R.A"));
		}

		[Test]
		public void CheckSampleDataSourceWithBinding()
		{
			StringType strType = new StringType ();
			IntegerType intType = new IntegerType (1, 999);

			strType.DefineDefaultValue ("");
			strType.DefineSampleValue ("Abc");

			intType.DefineDefaultValue (1);
			intType.DefineSampleValue (123);

			Panel panel = new Panel ();
			DataSourceMetadata metadata = panel.DataSourceMetadata;

			StructuredType recType = new StructuredType ();

			recType.Fields.Add ("A", strType);
			recType.Fields.Add ("B", intType);
			recType.Fields.Add ("R", recType);

			metadata.Fields.Add (new StructuredTypeField ("Record", recType));

			Placeholder placeholder1 = new Placeholder ();
			Placeholder placeholder2 = new Placeholder ();
			Placeholder placeholder3 = new Placeholder ();

			panel.Children.Add (placeholder1);
			panel.Children.Add (placeholder2);
			panel.Children.Add (placeholder3);

			Assert.IsNull (panel.DataSource);
			Assert.AreEqual (UnknownValue.Value, StructuredTree.GetValue (panel.DataSource, "Record.A"));
			Assert.AreEqual (UnknownValue.Value, StructuredTree.GetValue (panel.DataSource, "Record.B"));
			Assert.AreEqual (UnknownValue.Value, StructuredTree.GetValue (panel.DataSource, "Record.R"));

			placeholder1.SetBinding (Placeholder.ValueProperty, new Binding (BindingMode.OneWay, "Record.A"));
			placeholder2.SetBinding (Placeholder.ValueProperty, new Binding (BindingMode.OneWay, "Record.B"));
			placeholder3.SetBinding (Placeholder.ValueProperty, new Binding (BindingMode.OneWay, "Record.R.R.A"));

			string xml = Panel.SerializePanel (panel);

			System.Console.Out.WriteLine (xml);

			panel.SetupSampleDataSource ();

			Assert.AreEqual ("Abc", StructuredTree.GetValue (panel.DataSource, "Record.A"));
			Assert.AreEqual (123, StructuredTree.GetValue (panel.DataSource, "Record.B"));
			Assert.AreEqual ("Abc", StructuredTree.GetValue (panel.DataSource, "Record.R.R.A"));

			Assert.AreEqual ("Abc", placeholder1.Value);
			Assert.AreEqual (123, placeholder2.Value);
			Assert.AreEqual ("Abc", placeholder3.Value);
		}

		[Test]
		public void CheckSampleDataSourceWithBindingRealTypes()
		{
			Support.ResourceManager manager = new Support.ResourceManager ();

			Panel panel = new Panel ();
			panel.ResourceManager = manager;
			
			DataSourceMetadata metadata = panel.DataSourceMetadata;

			metadata.Fields.Add (new StructuredTypeField ("Person", Res.Types.Record.Address));

			Placeholder placeholder1 = new Placeholder ();
			Placeholder placeholder2 = new Placeholder ();
			Placeholder placeholder3 = new Placeholder ();
			Placeholder placeholder4 = new Placeholder ();

			panel.Children.Add (placeholder1);
			panel.Children.Add (placeholder2);
			panel.Children.Add (placeholder3);
			panel.Children.Add (placeholder4);

			Assert.IsNull (panel.DataSource);
			
			Assert.AreEqual (UnknownValue.Value, StructuredTree.GetValue (panel.DataSource, "Person.FirstName"));
			Assert.AreEqual (UnknownValue.Value, StructuredTree.GetValue (panel.DataSource, "Person.LastName"));
			Assert.AreEqual (UnknownValue.Value, StructuredTree.GetValue (panel.DataSource, "Person.Zip"));
			Assert.AreEqual (UnknownValue.Value, StructuredTree.GetValue (panel.DataSource, "Person.City"));

			placeholder1.SetBinding (Placeholder.ValueProperty, new Binding (BindingMode.TwoWay, "Person.FirstName"));
			placeholder2.SetBinding (Placeholder.ValueProperty, new Binding (BindingMode.TwoWay, "Person.LastName"));
			placeholder3.SetBinding (Placeholder.ValueProperty, new Binding (BindingMode.TwoWay, "Person.Zip"));
			placeholder4.SetBinding (Placeholder.ValueProperty, new Binding (BindingMode.TwoWay, "Person.City"));

			string xml = Panel.SerializePanel (panel);

			System.Console.Out.WriteLine (xml);

			Panel clone = Panel.DeserializePanel (xml, null, manager);
			panel.SetupSampleDataSource ();

			placeholder1.Value = "Pierre";
			placeholder2.Value = "Arnaud";
			placeholder3.Value = 1400;
			placeholder4.Value = "Yverdon-les-Bains";

			Assert.AreEqual ("Pierre", StructuredTree.GetValue (panel.DataSource, "Person.FirstName"));
			Assert.AreEqual ("Arnaud", StructuredTree.GetValue (panel.DataSource, "Person.LastName"));
			Assert.AreEqual (1400, StructuredTree.GetValue (panel.DataSource, "Person.Zip"));
			Assert.AreEqual ("Yverdon-les-Bains", StructuredTree.GetValue (panel.DataSource, "Person.City"));

			panel = clone;
			panel.SetupSampleDataSource ();

			placeholder1 = panel.Children[0] as Placeholder;
			placeholder2 = panel.Children[1] as Placeholder;
			placeholder3 = panel.Children[2] as Placeholder;
			placeholder4 = panel.Children[3] as Placeholder;

			Assert.AreEqual ("Abc", StructuredTree.GetValue (panel.DataSource, "Person.FirstName"));
			Assert.AreEqual ("Abc", StructuredTree.GetValue (panel.DataSource, "Person.LastName"));
			Assert.AreEqual (1000, StructuredTree.GetValue (panel.DataSource, "Person.Zip"));
			Assert.AreEqual ("Abc", StructuredTree.GetValue (panel.DataSource, "Person.City"));

			Assert.AreEqual ("Abc", placeholder1.Value);
			Assert.AreEqual ("Abc", placeholder2.Value);
			Assert.AreEqual (1000, placeholder3.Value);
			Assert.AreEqual ("Abc", placeholder4.Value);
		}

		[Test]
		public void CheckSerialization()
		{
			Support.ResourceManager manager = new Support.ResourceManager ();

			Panel panel = new Panel ();
			panel.ResourceManager = manager;

			Widgets.Widget a = new Widgets.Widget ();
			Widgets.Widget b = new Widgets.Widget ();
			Widgets.Widget c = new Widgets.Widget ();

			a.Name = "a";
			b.Name = "b";
			c.Name = "c";

			panel.Children.Add (a);
			panel.Children.Add (b);

			panel.PreferredSize = new Drawing.Size (100, 80);

			string xml;
			Panel copy;

			xml = Panel.SerializePanel (panel);
			System.Console.Out.WriteLine ("{0}", xml);
			copy = Panel.DeserializePanel (xml, null, manager);

			panel.EditionPanel.Children.Add (c);
			panel.EditionPanel.PreferredSize = new Drawing.Size (200, 96);

			xml = Panel.SerializePanel (panel);
			System.Console.Out.WriteLine ("{0}", xml);
			copy = Panel.DeserializePanel (xml, null, manager);

			Assert.AreEqual (PanelMode.Default, copy.PanelMode);
			Assert.AreEqual (2, Collection.Count<Widgets.Visual> (copy.Children));
			Assert.AreEqual (1, Collection.Count<Widgets.Visual> (copy.EditionPanel.Children));
			Assert.AreEqual (new Drawing.Size (100, 80), copy.PreferredSize);
			Assert.AreEqual (new Drawing.Size (200, 96), copy.EditionPanel.PreferredSize);
		}
	}
}
