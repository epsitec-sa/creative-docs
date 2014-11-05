//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Helpers;
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
					if (reader.Name == "Definition")
					{
						this.Definition = new SearchDefinition (reader);
					}
					else if (reader.Name.StartsWith ("LastPattern"))
					{
						var s = reader.ReadElementContentAsString ();
						this.LastPatterns.Add (s);
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

			this.Definition.Serialize (writer, "Definition");

			for (int i=0; i<this.LastPatterns.Count; i++)
			{
				var n = "LastPattern" + i.ToStringIO ();
				writer.WriteElementString (n, this.LastPatterns[i]);
			}

			writer.WriteEndElement ();
		}


		public static SearchInfo Default = new SearchInfo (SearchDefinition.Default);

		public readonly SearchDefinition		Definition;
		public readonly List<string>			LastPatterns;
	}
}
