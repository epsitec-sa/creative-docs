//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			Epsitec.Common.Widgets.Widget.Initialize ();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookMetal");
		}
		
		[Test]
		public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
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
			
			xml = this.SerializePanel (panel, manager);
			System.Console.Out.WriteLine ("{0}", xml);
			copy = this.DeserializePanel (xml, manager);
			
			panel.EditionPanel.Children.Add (c);
			panel.EditionPanel.PreferredSize = new Drawing.Size (200, 96);
			
			xml = this.SerializePanel (panel, manager);
			System.Console.Out.WriteLine ("{0}", xml);
			copy = this.DeserializePanel (xml, manager);

			Assert.AreEqual (PanelMode.Default, copy.PanelMode);
			Assert.AreEqual (2, Collection.Count<Widgets.Visual> (copy.Children));
			Assert.AreEqual (1, Collection.Count<Widgets.Visual> (copy.EditionPanel.Children));
			Assert.AreEqual (new Drawing.Size (100, 80), copy.PreferredSize);
			Assert.AreEqual (new Drawing.Size (200, 96), copy.EditionPanel.PreferredSize);
		}

		[Test]
		public void CheckInteractive()
		{
			Widgets.Window window = new Widgets.Window ();
			
			window.Text = "PanelTest.CheckInteractive";
			window.ClientSize = new Drawing.Size (400, 300);

			PanelStack panelStack = new PanelStack ();
			
			panelStack.Dock = Widgets.DockStyle.Fill;
			panelStack.Margins = new Drawing.Margins (4, 4, 8, 8);
			panelStack.BackColor = Drawing.Color.FromRgb (1, 0.8, 0.8);

			Panel panel = new Panel ();

			Widgets.Button buttonEdit = new Widgets.Button (panel);

			buttonEdit.TabIndex = 1;
			buttonEdit.TabNavigation = Widgets.Widget.TabNavigationMode.ActivateOnTab;
			buttonEdit.Dock = Widgets.DockStyle.Fill;
			buttonEdit.Text = "Edit";
			buttonEdit.Clicked += delegate (object sender, Widgets.MessageEventArgs e)
			{
				Panel editPanel = panel.EditionPanel;
				panelStack.StartEdition (editPanel);
			};

			panel.PreferredSize = new Drawing.Size (100, 100);
			panel.Anchor = Widgets.AnchorStyles.TopRight;
			panel.Margins = new Drawing.Margins (4, 4, 4, 4);
			panel.BackColor = Drawing.Color.FromRgb (0.8, 1, 0.8);

			panel.EditionPanel.PreferredSize = new Drawing.Size (180, 120);
			
			Widgets.Button buttonEnd = new Widgets.Button ();
			
			buttonEnd.TabIndex = 1;
			buttonEnd.TabNavigation = Widgets.Widget.TabNavigationMode.ActivateOnTab;
			buttonEnd.Dock = Widgets.DockStyle.Bottom;
			buttonEnd.Text = "End Edit";
			buttonEnd.Clicked += delegate (object sender, Widgets.MessageEventArgs e)
			{
				panelStack.EndEdition ();
			};

			panel.EditionPanel.Children.Add (buttonEnd);

			panelStack.Children.Add (panel);

			window.Root.Children.Add (panelStack);

			window.Show ();
			
			Widgets.Window.RunInTestEnvironment (window);
		}

		#region Support Code

		string SerializePanel(Panel panel, Support.ResourceManager manager)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			System.IO.StringWriter stringWriter = new System.IO.StringWriter (buffer);
			System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter (stringWriter);

			using (Types.Serialization.Context context = new Types.Serialization.SerializerContext (new Types.Serialization.IO.XmlWriter (xmlWriter)))
			{
				Panel.FillSerializationContext (context, null, manager);

				xmlWriter.Formatting = System.Xml.Formatting.Indented;
				xmlWriter.WriteStartElement ("panel");

				context.ActiveWriter.WriteAttributeStrings ();

				Types.Storage.Serialize (panel, context);

				xmlWriter.WriteEndElement ();
				xmlWriter.Flush ();
				xmlWriter.Close ();

				return buffer.ToString ();
			}
		}
		
		Panel DeserializePanel(string xml, Support.ResourceManager manager)
		{
			System.IO.StringReader stringReader = new System.IO.StringReader (xml);
			System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader (stringReader);

			xmlReader.Read ();

			System.Diagnostics.Debug.Assert (xmlReader.NodeType == System.Xml.XmlNodeType.Element);
			System.Diagnostics.Debug.Assert (xmlReader.LocalName == "panel");

			Types.Serialization.Context context = new Types.Serialization.DeserializerContext (new Types.Serialization.IO.XmlReader (xmlReader));

			Panel.FillSerializationContext (context, null, manager);

			return Types.Storage.Deserialize (context) as Panel;
		}

		#endregion
	}
}
