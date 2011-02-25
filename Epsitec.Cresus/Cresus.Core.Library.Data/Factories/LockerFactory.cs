//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Data;

namespace Epsitec.Cresus.Core.Factories
{
	public sealed class LockerFactory : ICoreDataComponentFactory
	{
		#region ICoreDataComponentFactory Members

		public bool CanCreate(CoreData data)
		{
			return data.DataInfrastructure != null;
		}

		public CoreDataComponent Create(CoreData data)
		{
			return new Locker (data);
		}

		public System.Type GetComponentType()
		{
			return typeof (Locker);
		}

		#endregion
	}
}
