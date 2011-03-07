//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Repositories
{
	public class BusinessSettingsRepository : Repository<BusinessSettingsEntity>
	{
		public BusinessSettingsRepository(CoreData data, DataContext context = null)
			: base (data, context, Common.Types.DataLifetimeExpectancy.Stable)
		{
		}
	}
}
