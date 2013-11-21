//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
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

		public readonly string Value;
	}
}
