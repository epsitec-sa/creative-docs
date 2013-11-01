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

		public readonly ObjectField Field;

		public PropertyState State;
	}
}
