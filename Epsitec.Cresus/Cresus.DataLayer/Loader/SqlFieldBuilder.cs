using Epsitec.Common.Support;

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using System;


namespace Epsitec.Cresus.DataLayer.Loader
{


	internal sealed class SqlFieldBuilder
	{



		public SqlFieldBuilder(AbstractEntity rootEntity, AliasNode rootAlias, Func<DbRawType, DbSimpleType, DbNumDef, object, SqlField> constantResolver, Func<AbstractEntity, AliasNode, AbstractEntity, Druid, SqlField> publicFieldResolver, Func<AbstractEntity, AliasNode, AbstractEntity, string, SqlField> privateFieldResolver)
		{
			this.rootEntity = rootEntity;
			this.rootAlias = rootAlias;

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
			return this.publicFieldResolver (this.rootEntity, this.rootAlias, entity, fieldId);
		}


		public SqlField Build(AbstractEntity entity, string name)
		{
			return this.privateFieldResolver (this.rootEntity, this.rootAlias, entity, name);
		}


		private readonly AbstractEntity rootEntity;


		private readonly AliasNode rootAlias;


		private readonly Func<DbRawType, DbSimpleType, DbNumDef, object, SqlField> constantResolver;


		private readonly Func<AbstractEntity, AliasNode, AbstractEntity, Druid, SqlField> publicFieldResolver;


		private readonly Func<AbstractEntity, AliasNode, AbstractEntity, string, SqlField> privateFieldResolver;


	}


}
