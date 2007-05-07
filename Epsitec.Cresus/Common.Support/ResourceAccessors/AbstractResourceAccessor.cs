//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.ResourceAccessors
{
	class AbstractResourceAccessor : IResourceAccessor
	{
		#region IResourceAccessor Members

		public IList<CultureMap> Collection
		{
			get
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}
		}

		public IDataBroker DataBroker
		{
			get
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}
		}

		public CultureMap CreateItem()
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		public int PersistChanges()
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		#endregion



		
	}
}
