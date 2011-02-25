//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business.UserManagement;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Factories
{
	public sealed class UserManagerFactory : ICoreDataComponentFactory
	{
		#region ICoreDataComponentFactory Members

		public bool CanCreate(CoreData data)
		{
			return data.DataInfrastructure != null;
		}

		public CoreDataComponent Create(CoreData data)
		{
			return new UserManager (data);
		}

		public System.Type GetComponentType()
		{
			return typeof (UserManager);
		}

		#endregion
	}
}