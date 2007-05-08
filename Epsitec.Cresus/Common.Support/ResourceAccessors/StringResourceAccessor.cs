//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.ResourceAccessors
{
	class StringResourceAccessor : AbstractResourceAccessor
	{
		public StringResourceAccessor()
			: base (new ResourceBrokers.StringDataBroker ())
		{
		}

		public override void Load()
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		protected override Druid CreateId()
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		protected override void PersistItem(CultureMap item)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}
	}
}
