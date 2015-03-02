//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Serialization;

namespace Epsitec.Cresus.Assets.Data.DataProperties
{
	public class DataGuidRatioProperty : AbstractDataProperty
	{
		public DataGuidRatioProperty(ObjectField field, GuidRatio value)
			: base (field)
		{
			this.Value = value;
		}

		public DataGuidRatioProperty(DataGuidRatioProperty model)
			: base (model)
		{
			this.Value = model.Value;
		}

		public DataGuidRatioProperty(System.Xml.XmlReader reader)
			: base (reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (reader.Name == X.GuidRatio)
					{
						this.Value = new GuidRatio (reader);
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
			writer.WriteStartElement (X.Property_GuidRatio);
			base.Serialize (writer);
			this.Value.Serialize (writer, X.GuidRatio);
			writer.WriteEndElement ();
		}


		public readonly GuidRatio Value;
	}
}
