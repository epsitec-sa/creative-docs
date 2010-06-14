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


		internal override DbSelectCondition CreateDbSelectCondition(AbstractEntity entity, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			DbSelectCondition dbSelectCondition = new DbSelectCondition ();
			DbCondition dbCondition = this.CreateDbCondition (entity, dbTableColumnResolver);

			dbSelectCondition.Conditions.AddCondition (dbCondition);

			return dbSelectCondition;
		}


		internal abstract DbCondition CreateDbCondition(AbstractEntity entity, System.Func<Druid, DbTableColumn> dbTableColumnResolver);


	}


}
