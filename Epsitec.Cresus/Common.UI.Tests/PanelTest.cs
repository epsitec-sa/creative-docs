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
		}
		
		[Test]
		public void CheckSerialization1()
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

			string xml;
			Panel copy;
			
			xml = this.SerializePanel (panel, manager);
			System.Console.Out.WriteLine ("{0}", xml);
			copy = this.DeserializePanel (xml, manager);
			
			panel.PanelMode = PanelMode.Edition;
			panel.Children.Add (c);
			panel.PanelMode = PanelMode.Default;

			xml = this.SerializePanel (panel, manager);
			System.Console.Out.WriteLine ("{0}", xml);
			copy = this.DeserializePanel (xml, manager);

			Assert.AreEqual (2, Collection.Count<Widgets.Visual> (copy.DefaultVisuals));
			Assert.AreEqual (1, Collection.Count<Widgets.Visual> (copy.EditVisuals));
		}
		
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
	}
}
