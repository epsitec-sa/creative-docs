using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	public class Field
	{


		public Field(Druid FieldId) : base ()
		{
			this.FieldId = FieldId;
		}


		public Druid FieldId
		{
			get;
			private set;
		}


		internal DbTableColumn CreateDbTableColumn(AbstractEntity entity, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			return dbTableColumnResolver (this.FieldId);
		}


	}


}
