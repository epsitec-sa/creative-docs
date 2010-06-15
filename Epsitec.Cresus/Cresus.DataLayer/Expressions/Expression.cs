using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	public abstract class Expression
	{


		public Expression()
		{
		}


		internal abstract DbAbstractCondition CreateDbAbstractCondition(AbstractEntity entity, System.Func<Druid, DbTableColumn> dbTableColumnResolver);


		internal abstract IEnumerable<Druid> GetFields();

	}


}
