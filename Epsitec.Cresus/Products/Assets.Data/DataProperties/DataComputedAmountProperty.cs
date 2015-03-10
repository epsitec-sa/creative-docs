//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Serialization;

namespace Epsitec.Cresus.Assets.Data.DataProperties
{
	public class DataComputedAmountProperty : AbstractDataProperty
	{
		public DataComputedAmountProperty(ObjectField field, ComputedAmount value)
			: base (field)
		{
			this.Value = value;
		}

		public DataComputedAmountProperty(DataComputedAmountProperty model)
			: base (model)
		{
			this.Value = model.Value;
		}

		public DataComputedAmountProperty(System.Xml.XmlReader reader)
			: base (reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (reader.Name == X.ComputedAmount)
					{
						this.Value = new ComputedAmount (reader);
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
			writer.WriteStartElement (X.Property_ComputedAmount);
			base.Serialize (writer);
			this.Value.Serialize (writer, X.ComputedAmount);
			writer.WriteEndElement ();
		}


		public readonly ComputedAmount Value;
	}
}
