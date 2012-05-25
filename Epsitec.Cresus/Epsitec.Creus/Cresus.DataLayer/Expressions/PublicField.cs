//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

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

			this.fieldId = fieldId;
		}


		public Druid							FieldId
		{
			get
			{
				return this.fieldId;
			}
		}


		public static PublicField Create<T, V>(T entity, System.Linq.Expressions.Expression<System.Func<T, V>> expression)
			where T : AbstractEntity, new ()
		{
			return new PublicField (entity, EntityInfo.GetFieldDruid (expression));
		}
		
		
		internal override SqlField CreateSqlField(SqlFieldBuilder builder)
		{
			return builder.BuildEntityField (this.Entity, this.FieldId);
		}

		internal override void CheckField(FieldChecker checker)
		{
			checker.Check (this.Entity, this.FieldId);
		}

		
		private readonly Druid					fieldId;
	}
}
