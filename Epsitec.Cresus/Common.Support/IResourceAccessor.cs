//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	public interface IResourceAccessor
	{
		IList<CultureMap> Collection
		{
			get;
		}
		
		IDataBroker DataBroker
		{
			get;
		}
		
		CultureMap CreateItem();
		
		int PersistChanges();
	}
}
