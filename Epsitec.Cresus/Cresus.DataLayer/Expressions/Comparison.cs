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


		internal override DbAbstractCondition CreateDbAbstractCondition(AbstractEntity entity, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			return this.CreateDbSimpleCondition (entity, dbTableColumnResolver);
		}


		internal abstract DbSimpleCondition CreateDbSimpleCondition(AbstractEntity entity, System.Func<Druid, DbTableColumn> dbTableColumnResolver);


	}


}
