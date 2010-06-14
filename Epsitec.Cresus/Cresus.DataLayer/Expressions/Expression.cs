using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	public abstract class Expression
	{


		public Expression()
		{
		}


		internal abstract DbConditionCombiner CreateDbSelectCondition(AbstractEntity entity, System.Func<Druid, DbTableColumn> dbTableColumnResolver);
	
	
	}


}
