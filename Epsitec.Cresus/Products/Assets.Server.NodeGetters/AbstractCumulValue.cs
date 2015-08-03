//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public abstract class AbstractCumulValue
	{
		public AbstractCumulValue(object value)
		{
			this.Value = value;
		}

		public object Value
		{
			set
			{
				this.value = value;
			}
			get
			{
				return this.value;
			}
		}

		public virtual bool IsExist
		{
			get
			{
				return true;
			}
		}

		public abstract AbstractCumulValue Merge(AbstractCumulValue a);


		protected object								value;
	}
}
