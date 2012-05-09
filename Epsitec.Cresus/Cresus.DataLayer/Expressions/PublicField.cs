using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>PublicField</c> class represents a field of an <see cref="AbstractEntity"/> in an
	/// <see cref="Expression"/> that targets a regular field of the entity.
	/// </summary>
	public sealed class PublicField : EntityField
	{


		public PublicField(AbstractEntity entity, Druid fieldId)
			: base (entity)
		{
			fieldId.ThrowIf (id => id.IsEmpty, "fieldId is empty");

			this.FieldId = fieldId;
		}


		public Druid FieldId
		{
			get;
			private set;
		}


		internal override SqlField CreateSqlField(SqlFieldBuilder builder)
		{
			return builder.Build (this.Entity, this.FieldId);
		}


		internal override void CheckField(FieldChecker checker)
		{
			checker.Check (this.Entity, this.FieldId);
		}


	}


}
