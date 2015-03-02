//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Serialization;

namespace Epsitec.Cresus.Assets.Data.DataProperties
{
	public class DataStringProperty : AbstractDataProperty
	{
		public DataStringProperty(ObjectField field, string value)
			: base (field)
		{
			this.Value = value;
		}

		public DataStringProperty(DataStringProperty model)
			: base (model)
		{
			this.Value = model.Value;
		}

		public DataStringProperty(System.Xml.XmlReader reader)
			: base (reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (reader.Name == X.Value)
					{
						this.Value = reader.ReadElementContentAsString ();
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}


		public override void Serialize(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement (X.Property_String);
			base.Serialize (writer);
			writer.WriteElementString (X.Value, this.Value);
			writer.WriteEndElement ();
		}


		public static string WithoutVat = "-";


		public readonly string Value;
	}
}
