//	Copyright � 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;

namespace Epsitec.Cresus.Core.Factories
{
	public sealed class RefIdGeneratorPoolFactory : ICoreDataComponentFactory
	{
		#region ICoreDataComponentFactory Members

		public bool CanCreate(CoreData data)
		{
			return true;
		}

		public CoreDataComponent Create(CoreData data)
		{
			return new RefIdGeneratorPool (data);
		}

		public System.Type GetComponentType()
		{
			return typeof (RefIdGeneratorPool);
		}

		#endregion
	}
}
