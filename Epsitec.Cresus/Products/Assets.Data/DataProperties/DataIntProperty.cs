//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Helpers;

namespace Epsitec.Cresus.Assets.Data.DataProperties
{
	public class DataIntProperty : AbstractDataProperty
	{
		public DataIntProperty(ObjectField field, int value)
			: base (field)
		{
			this.Value = value;
		}

		public DataIntProperty(DataIntProperty model)
			: base (model)
		{
			this.Value = model.Value;
		}

		public DataIntProperty(System.Xml.XmlReader reader)
			: base (reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (reader.Name == "Value")
					{
						var s = reader.ReadElementContentAsString ();
						this.Value = s.ParseInt ();
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
			writer.WriteStartElement ("Property.Int");
			base.Serialize (writer);
			writer.WriteElementString ("Value", this.Value.ToStringIO ());
			writer.WriteEndElement ();
		}


		public readonly int Value;
	}
}
