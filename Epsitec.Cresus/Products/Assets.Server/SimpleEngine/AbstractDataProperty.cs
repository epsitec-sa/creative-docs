//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public abstract class AbstractDataProperty
	{
		public AbstractDataProperty(int fieldId)
		{
			this.FieldId = fieldId;
		}

		public readonly int FieldId;

		public PropertyState State;
	}
}
