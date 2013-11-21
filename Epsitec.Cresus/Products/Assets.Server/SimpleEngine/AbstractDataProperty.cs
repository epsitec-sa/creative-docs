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


		public static AbstractDataProperty Copy(AbstractDataProperty p)
		{
			if (p is DataComputedAmountProperty)
			{
				return new DataComputedAmountProperty (p as DataComputedAmountProperty);
			}
			else if (p is DataDateProperty)
			{
				return new DataDateProperty (p as DataDateProperty);
			}
			else if (p is DataDecimalProperty)
			{
				return new DataDecimalProperty (p as DataDecimalProperty);
			}
			else if (p is DataGuidProperty)
			{
				return new DataGuidProperty (p as DataGuidProperty);
			}
			else if (p is DataIntProperty)
			{
				return new DataIntProperty (p as DataIntProperty);
			}
			else if (p is DataStringProperty)
			{
				return new DataStringProperty (p as DataStringProperty);
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
