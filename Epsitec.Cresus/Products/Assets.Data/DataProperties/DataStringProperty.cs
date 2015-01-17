//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

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
					if (reader.Name == "Value")
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
			writer.WriteStartElement ("Property.String");
			base.Serialize (writer);
			writer.WriteElementString ("Value", this.Value);
			writer.WriteEndElement ();
		}


		public readonly string Value;
	}
}
