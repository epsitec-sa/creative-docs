using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Expressions
{

	
	public sealed class SubQuery : Value
	{


		public SubQuery(Request subRequest)
		{
			subRequest.ThrowIfNull ("subQuery");

			this.SubRequest = subRequest;
		}


		public Request SubRequest
		{
			get;
			private set;
		}


		internal override SqlField CreateSqlField(SqlFieldBuilder builder)
		{
			return builder.BuildSqlFieldForSubQuery (this.SubRequest);
		}


		internal override void CheckField(FieldChecker checker)
		{
		}


		internal override void AddEntities(HashSet<AbstractEntity> entities)
		{
		}


	}


}
