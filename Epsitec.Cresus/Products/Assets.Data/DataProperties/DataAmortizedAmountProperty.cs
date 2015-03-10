//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Serialization;

namespace Epsitec.Cresus.Assets.Data.DataProperties
{
	public class DataAmortizedAmountProperty : AbstractDataProperty
	{
		public DataAmortizedAmountProperty(ObjectField field, AmortizedAmount value)
			: base (field)
		{
			this.Value = value;
		}

		public DataAmortizedAmountProperty(DataAmortizedAmountProperty model)
			: base (model)
		{
			this.Value = model.Value;
		}

		public DataAmortizedAmountProperty(System.Xml.XmlReader reader)
			: base (reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (reader.Name == X.AmortizedAmount)
					{
						this.Value = new AmortizedAmount (reader);
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
			writer.WriteStartElement (X.Property_AmortizedAmount);
			base.Serialize (writer);
			this.Value.Serialize (writer, X.AmortizedAmount);
			writer.WriteEndElement ();
		}


		public readonly AmortizedAmount Value;
	}
}
