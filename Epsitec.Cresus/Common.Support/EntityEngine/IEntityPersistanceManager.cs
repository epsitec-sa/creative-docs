//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	public interface IEntityPersistanceManager
	{
		string GetPersistedId(AbstractEntity entity);
		AbstractEntity GetPeristedEntity(string id, Druid entityId);
	}
}
