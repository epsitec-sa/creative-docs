//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data.DataProperties
{
	public class DataGuidProperty : AbstractDataProperty
	{
		public DataGuidProperty(ObjectField field, Guid value)
			: base (field)
		{
			this.Value = value;
		}

		public DataGuidProperty(DataGuidProperty model)
			: base (model)
		{
			this.Value = model.Value;
		}

		public DataGuidProperty(System.Xml.XmlReader reader)
			: base (reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (reader.Name == "Guid")
					{
						this.Value = new Guid (reader);
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
			writer.WriteStartElement ("Property.Guid");
			base.Serialize (writer);
			this.Value.Serialize (writer, "Guid");
			writer.WriteEndElement ();
		}


		public readonly Guid Value;
	}
}
