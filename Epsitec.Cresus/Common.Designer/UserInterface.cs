using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Designer
{
	public class UserInterface
	{
		public static UI.Panel CreateEmptyPanel()
		{
			//	Crée un panneau "UI" vide pour expérimenter.

			UI.Panel panel = new Epsitec.Common.UI.Panel ();
			UI.DataSourceCollection sources = new Epsitec.Common.UI.DataSourceCollection ();

			CustomerRecord customer = new CustomerRecord ();

			customer.SetValue (CustomerRecord.NameProperty, "Arnaud");
			customer.SetValue (CustomerRecord.SurameProperty, "Pierre");
			customer.SetValue (CustomerRecord.AddressProperty, "Ch. du Fontenay 6");
			customer.SetValue (CustomerRecord.PostalNumberProperty, 1400);
			customer.SetValue (CustomerRecord.CityProperty, "Yverdon-les-Bains");

			sources.AddDataSource ("Customer", customer);
			
			panel.DataSource = sources;

			return panel;
		}

		public static string SerializePanel(UI.Panel panel)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			System.IO.StringWriter stringWriter = new System.IO.StringWriter (buffer);
			System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter (stringWriter);

			Types.Serialization.Context context = new Types.Serialization.SerializerContext (new Types.Serialization.IO.XmlWriter (xmlWriter));
			
			xmlWriter.Formatting = System.Xml.Formatting.None;
			xmlWriter.WriteStartElement ("panel");

			context.ActiveWriter.WriteAttributeStrings ();
			// TODO: utiliser Panel.FillSerializationContext
			
			Types.Storage.Serialize (panel, context);

			xmlWriter.WriteEndElement ();
			xmlWriter.Flush ();
			xmlWriter.Close ();

			return buffer.ToString ();
		}

		public static UI.Panel DeserializePanel(string xml, Support.ResourceManager manager)
		{
			System.IO.StringReader stringReader = new System.IO.StringReader (xml);
			System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader (stringReader);

			xmlReader.Read ();
			
			System.Diagnostics.Debug.Assert (xmlReader.NodeType == System.Xml.XmlNodeType.Element);
			System.Diagnostics.Debug.Assert (xmlReader.LocalName == "panel");

			Types.Serialization.Context context = new Types.Serialization.DeserializerContext (new Types.Serialization.IO.XmlReader (xmlReader));

			context.ExternalMap.Record (Types.Serialization.Context.WellKnownTagResourceManager, manager);
			// TODO: utiliser Panel.FillSerializationContext

			UI.Panel panel = Types.Storage.Deserialize (context) as UI.Panel;
			
			return panel;

		}
		
		private class CustomerRecord : Types.DependencyObject
		{
			//	Class "bidon" pour avoir au moins un source de données disponible.
			
			public CustomerRecord()
			{
			}
			
			public static readonly Types.DependencyProperty NameProperty = Types.DependencyProperty.Register ("Name", typeof (string), typeof (CustomerRecord));
			public static readonly Types.DependencyProperty SurameProperty = Types.DependencyProperty.Register ("Surname", typeof (string), typeof (CustomerRecord));
			public static readonly Types.DependencyProperty AddressProperty = Types.DependencyProperty.Register ("Address", typeof (string), typeof (CustomerRecord));
			public static readonly Types.DependencyProperty PostalNumberProperty = Types.DependencyProperty.Register ("PostalNumber", typeof (int), typeof (CustomerRecord));
			public static readonly Types.DependencyProperty CityProperty = Types.DependencyProperty.Register ("City", typeof (string), typeof (CustomerRecord));

		}
	}
}
