using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Types.Serialization
{
	public static class SimpleSerialization
	{
		public static string SerializeToString(DependencyObject root)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			System.IO.StringWriter stringWriter = new System.IO.StringWriter (buffer);
			System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter (stringWriter);

			xmlWriter.Formatting = System.Xml.Formatting.None;
			xmlWriter.WriteStartElement (SimpleSerialization.RootElementName);

			Serialization.Context context = new Serialization.SerializerContext (new Serialization.IO.XmlWriter (xmlWriter));
			context.ActiveWriter.WriteAttributeStrings ();

			Storage.Serialize (root, context);
			
			xmlWriter.WriteEndElement ();
			xmlWriter.Flush ();
			xmlWriter.Close ();

			return buffer.ToString ();
		}

		public static DependencyObject DeserializeFromString(string xml)
		{
			System.IO.StringReader stringReader = new System.IO.StringReader (xml);
			System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader (stringReader);

			Serialization.Context context = new Serialization.DeserializerContext (new Serialization.IO.XmlReader (xmlReader));

			while (xmlReader.Read ())
			{
				if ((xmlReader.NodeType == System.Xml.XmlNodeType.Element) &&
					(xmlReader.LocalName == SimpleSerialization.RootElementName))
				{
					break;
				}
			}

			DependencyObject root = Storage.Deserialize (context);

			xmlReader.Close ();
			stringReader.Close ();
			
			return root;
		}

		public const string RootElementName = "objects";
	}
}
