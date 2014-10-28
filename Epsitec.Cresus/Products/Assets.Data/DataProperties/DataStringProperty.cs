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


		public override void Serialize(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement ("Property.String");
			writer.WriteElementString ("Value", this.Value);
			writer.WriteEndElement ();
		}


		public readonly string Value;
	}
}
