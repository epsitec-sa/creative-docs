using System.Collections.Generic;


namespace Epsitec.Cresus.Database
{


	public abstract class DbAbstractCondition
	{


		internal abstract IEnumerable<DbTableColumn> Columns
		{
			get;
		}


		internal abstract void ReplaceTableColumns(System.Func<DbTableColumn, DbTableColumn> replaceOperation);


		internal abstract SqlField CreateSqlField();


	}


}
