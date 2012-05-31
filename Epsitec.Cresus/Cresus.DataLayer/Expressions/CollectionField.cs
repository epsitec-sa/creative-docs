using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>CollectionField</c> class represents a field of an <see cref="AbstractEntity"/> in an
	/// <see cref="Expression"/> that targets an column in the collection table of field of the
	/// entity.
	/// </summary>
	public sealed class CollectionField : EntityDruidField
	{


		/// <summary>
		/// Creates a new <c>CollectionField</c>.
		/// </summary>
		/// <param name="source">The entity that will be referenced by this instance</param>
		/// <param name="fieldId">The id of the field that will be referenced by this instance</param>
		/// <param name="target">The entity that is the target of the field referenced by this intance</param>
		/// <param name="columnName">The name that identifies the column referenced by this instace</param>
		private CollectionField(AbstractEntity source, Druid fieldId, AbstractEntity target, string columnName)
			: base (source, fieldId)
		{
			target.ThrowIfNull ("target");
			columnName.ThrowIfNullOrEmpty ("columnName");

			this.target = target;
			this.columnName = columnName;
		}


		/// <summary>
		/// The entity that is the target of the field.
		/// </summary>
		public AbstractEntity Target
		{
			get
			{
				return this.target;
			}
		}


		/// <summary>
		/// The name of the field.
		/// </summary>
		public string ColumnName
		{
			get
			{
				return this.columnName;
			}
		}


		internal override SqlField CreateSqlField(SqlFieldBuilder builder)
		{
			var source = this.Entity;
			var fieldId = this.FieldId;
			var target = this.Target;
			var name = this.ColumnName;

			return builder.BuildRelationColumn (source, fieldId, target, name);
		}


		internal override void CheckField(FieldChecker checker)
		{
			checker.CheckCollectionField (this.Entity, this.FieldId, this.ColumnName);
		}


		internal override void AddEntities(HashSet<AbstractEntity> entities)
		{
			base.AddEntities (entities);

			entities.Add (this.Target);
		}


		public static CollectionField CreateRank(AbstractEntity entity, Druid fieldId, AbstractEntity target)
		{
			var columnName = EntitySchemaBuilder.EntityFieldTableColumnRankName;

			return new CollectionField (entity, fieldId, target, columnName);
		}


		private readonly AbstractEntity target;


		private readonly string columnName;


	}


}
