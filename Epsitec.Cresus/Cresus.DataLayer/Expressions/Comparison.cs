using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	public abstract class Comparison : Expression
	{


		public Comparison() : base ()
		{
		}


		internal override DbConditionCombiner CreateDbSelectCondition(AbstractEntity entity, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			//DbSelectCondition dbSelectCondition = new DbSelectCondition ();
			//DbCondition dbCondition = this.CreateDbCondition (entity, dbTableColumnResolver);

			//dbSelectCondition.Conditions.AddCondition (dbCondition);

			//return dbSelectCondition;

			throw new System.NotImplementedException ();
		}


		internal abstract DbSimpleCondition CreateDbCondition(AbstractEntity entity, System.Func<Druid, DbTableColumn> dbTableColumnResolver);


	}


}
