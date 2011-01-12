//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Cresus.Core.Repositories
{
	public class FinanceSettingsRepository : Repository<FinanceSettingsEntity>
	{
		public FinanceSettingsRepository(CoreData data, DataContext context = null)
			: base (data, context)
		{
		}
	}
}
