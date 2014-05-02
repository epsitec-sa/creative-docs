//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

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

		public readonly AmortizedAmount Value;
	}
}
