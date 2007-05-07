//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.ResourceBrokers
{
	class StringDataBroker : IDataBroker
	{
		#region IDataBroker Members

		public Epsitec.Common.Types.StructuredData CreateData()
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		#endregion
	}
}
