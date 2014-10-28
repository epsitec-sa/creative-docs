//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data.DataProperties
{
	public class DataDateProperty : AbstractDataProperty
	{
		public DataDateProperty(ObjectField field, System.DateTime value)
			: base (field)
		{
			this.Value = value;
		}

		public DataDateProperty(DataDateProperty model)
			: base (model)
		{
			this.Value = model.Value;
		}

		public DataDateProperty(System.Xml.XmlReader reader)
			: base (reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (reader.Name == "Value")
					{
						var s = reader.ReadElementContentAsString ();
						this.Value = System.DateTime.Parse (s, System.Globalization.CultureInfo.InvariantCulture);
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
			writer.WriteStartElement ("Property.Date");
			base.Serialize (writer);
			writer.WriteElementString ("Value", this.Value.ToString (System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteEndElement ();
		}


		public readonly System.DateTime Value;
	}
}
