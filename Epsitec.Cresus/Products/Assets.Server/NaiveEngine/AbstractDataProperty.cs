//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.NaiveEngine
{
	public abstract class AbstractDataProperty
	{
		public AbstractDataProperty(int id)
		{
			this.Id = id;
		}

		public readonly int Id;

		public PropertyState State;
	}
}
