//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public abstract class AbstractDataProperty
	{
		public AbstractDataProperty(ObjectField field)
		{
			this.Field = field;
		}

		public AbstractDataProperty(AbstractDataProperty model)
		{
			this.Field = model.Field;
			this.State = model.State;
		}


		public static AbstractDataProperty Copy(AbstractDataProperty model)
		{
			if (model is DataComputedAmountProperty)
			{
				return new DataComputedAmountProperty (model as DataComputedAmountProperty);
			}
			else if (model is DataAmortizedAmountProperty)
			{
				return new DataAmortizedAmountProperty (model as DataAmortizedAmountProperty);
			}
			else if (model is DataDateProperty)
			{
				return new DataDateProperty (model as DataDateProperty);
			}
			else if (model is DataDecimalProperty)
			{
				return new DataDecimalProperty (model as DataDecimalProperty);
			}
			else if (model is DataGuidProperty)
			{
				return new DataGuidProperty (model as DataGuidProperty);
			}
			else if (model is DataGuidRatioProperty)
			{
				return new DataGuidRatioProperty (model as DataGuidRatioProperty);
			}
			else if (model is DataIntProperty)
			{
				return new DataIntProperty (model as DataIntProperty);
			}
			else if (model is DataStringProperty)
			{
				return new DataStringProperty (model as DataStringProperty);
			}
			else
			{
				return null;
			}
		}


		public readonly ObjectField Field;
		public PropertyState State;  // TODO: beurk !
	}
}
