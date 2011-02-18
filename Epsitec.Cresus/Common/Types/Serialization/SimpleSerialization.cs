//	Copyright © 2006-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Serialization
{
	public static class SimpleSerialization
	{
		public static string SerializeToString(DependencyObject root)
		{
			return SimpleSerialization.SerializeToString (root, SimpleSerialization.RootElementName, System.Xml.Formatting.None);
		}
		
		public static string SerializeToString(DependencyObject root, string rootElementName)
		{
			return SimpleSerialization.SerializeToString (root, rootElementName, System.Xml.Formatting.None);
		}

		public static string SerializeToString(DependencyObject root, string rootElementName, System.Xml.Formatting formatting)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			System.IO.StringWriter stringWriter = new System.IO.StringWriter (buffer);
			System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter (stringWriter);

			xmlWriter.Formatting = System.Xml.Formatting.None;
			xmlWriter.WriteStartElement (rootElementName);

			using (Serialization.Context context = new Serialization.SerializerContext (new Serialization.IO.XmlWriter (xmlWriter)))
			{
				xmlWriter.Formatting = formatting;
				context.ActiveWriter.WriteAttributeStrings ();

				Storage.Serialize (root, context);

				xmlWriter.WriteEndElement ();
				xmlWriter.Flush ();
				xmlWriter.Close ();

				return buffer.ToString ();
			}
		}

		public static DependencyObject DeserializeFromString(string xml)
		{
			return SimpleSerialization.DeserializeFromString (xml, SimpleSerialization.RootElementName, Support.Resources.DefaultManager);
		}

		public static DependencyObject DeserializeFromString(string xml, string rootElementName)
		{
			return SimpleSerialization.DeserializeFromString (xml, rootElementName, Support.Resources.DefaultManager);
		}

		public static DependencyObject DeserializeFromString(string xml, Support.ResourceManager manager)
		{
			return SimpleSerialization.DeserializeFromString (xml, SimpleSerialization.RootElementName, manager);
		}

		public static DependencyObject DeserializeFromString(string xml, string rootElementName, Support.ResourceManager manager)
		{
			System.IO.StringReader stringReader = new System.IO.StringReader (xml);
			System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader (stringReader);

			using (Serialization.Context context = new Serialization.DeserializerContext (new Serialization.IO.XmlReader (xmlReader)))
			{
				context.ExternalMap.Record (Serialization.Context.WellKnownTagResourceManager, manager);

				while (xmlReader.Read ())
				{
					if ((xmlReader.NodeType == System.Xml.XmlNodeType.Element) &&
						(xmlReader.LocalName == rootElementName))
					{
						break;
					}
				}

				DependencyObject root = Storage.Deserialize (context);

				xmlReader.Close ();
				stringReader.Close ();

				return root;
			}
		}

		public const string RootElementName = "objects";
	}
}
