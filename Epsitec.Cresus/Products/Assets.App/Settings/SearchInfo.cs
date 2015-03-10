//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.App.Settings
{
	public struct SearchInfo
	{
		public SearchInfo(SearchDefinition definition)
		{
			this.Definition = definition;
			this.LastPatterns = new List<string> ();
		}

		public SearchInfo(System.Xml.XmlReader reader)
		{
			this.Definition = SearchDefinition.Default;
			this.LastPatterns = new List<string> ();

			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case X.Definition:
							this.Definition = new SearchDefinition (reader);
							break;

						case X.LastPattern:
							var s = reader.ReadElementContentAsString ();
							this.LastPatterns.Add (s);
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}


		public SearchInfo FromDefinition(SearchDefinition definition)
		{
			var info = new SearchInfo (definition);
			info.LastPatterns.AddRange (this.LastPatterns);

			return info;
		}


		public void Serialize(System.Xml.XmlWriter writer, string name)
		{
			writer.WriteStartElement (name);

			this.Definition.Serialize (writer, X.Definition);

			foreach (var pattern in this.LastPatterns)
			{
				writer.WriteElementString (X.LastPattern, pattern);
			}

			writer.WriteEndElement ();
		}


		public static SearchInfo Default = new SearchInfo (SearchDefinition.Default);

		public readonly SearchDefinition		Definition;
		public readonly List<string>			LastPatterns;
	}
}
