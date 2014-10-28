//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

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


		public override void Serialize(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement ("Property.ComputedAmount");
			this.Value.Serialize (writer);
			writer.WriteEndElement ();
		}


		public readonly ComputedAmount Value;
	}
}
