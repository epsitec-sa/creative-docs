using Epsitec.Common.Support;

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using System;


namespace Epsitec.Cresus.DataLayer.Loader
{


	internal sealed class SqlFieldBuilder
	{



		public SqlFieldBuilder(AliasManager aliasManager, Func<DbRawType, DbSimpleType, DbNumDef, object, SqlField> constantResolver, Func<AliasManager, AbstractEntity, Druid, SqlField> publicFieldResolver, Func<AliasManager, AbstractEntity, string, SqlField> privateFieldResolver)
		{
			this.aliasManager = aliasManager;

			this.constantResolver = constantResolver;
			this.publicFieldResolver = publicFieldResolver;
			this.privateFieldResolver = privateFieldResolver;
		}


		public SqlField Build(DbRawType dbRawType, DbSimpleType dbSimpleType, DbNumDef dbNumDef, object value)
		{
			return this.constantResolver (dbRawType, dbSimpleType, dbNumDef, value);
		}


		public SqlField Build(AbstractEntity entity, Druid fieldId)
		{
			return this.publicFieldResolver (this.aliasManager, entity, fieldId);
		}


		public SqlField Build(AbstractEntity entity, string name)
		{
			return this.privateFieldResolver (this.aliasManager, entity, name);
		}


		private readonly AliasManager aliasManager;


		private readonly Func<DbRawType, DbSimpleType, DbNumDef, object, SqlField> constantResolver;


		private readonly Func<AliasManager, AbstractEntity, Druid, SqlField> publicFieldResolver;


		private readonly Func<AliasManager, AbstractEntity, string, SqlField> privateFieldResolver;


	}


}
