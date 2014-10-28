//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

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
			this.Value = new GuidRatio (reader);
		}


		public override void Serialize(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement ("Property.GuidRatio");
			base.Serialize (writer);
			this.Value.Serialize (writer);
			writer.WriteEndElement ();
		}


		public readonly GuidRatio Value;
	}
}
