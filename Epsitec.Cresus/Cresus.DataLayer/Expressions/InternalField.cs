using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Schema;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>InternalField</c> class represents a field of an <see cref="AbstractEntity"/> in an
	/// <see cref="Expression"/> that targets an field of the entity, such as its id.
	/// </summary>
	public sealed class InternalField : EntityField
	{


		/// <summary>
		/// Creates a new <c>PublicField</c>.
		/// </summary>
		/// <param name="entity">The entity that will be referenced by this instance</param>
		/// <param name="name">The name that identifies the field.</param>
		private InternalField(AbstractEntity entity, string name)
			: base (entity)
		{
			this.Name = name;
		}


		/// <summary>
		/// The name of the field.
		/// </summary>
		public string Name
		{
			get;
			private set;
		}


		internal override SqlField CreateSqlField(SqlFieldBuilder builder)
		{
			return builder.Build (this.Entity, this.Name);
		}


		internal override void CheckField(FieldChecker checker)
		{
			checker.Check (this.Entity, this.Name);
		}
		
		
		/// <summary>
		/// Creates a new InternalField instance that will target the id field of the given entity.
		/// </summary>
		/// <param name="entity">The entity for which to create an internal field.</param>
		/// <returns>The internal field.</returns>
		public static InternalField CreateId(AbstractEntity entity)
		{
			return new InternalField (entity, EntitySchemaBuilder.EntityTableColumnIdName);
		}


		/// <summary>
		/// Creates a new InternalField instance that will target the id of the entity modification
		/// log entry of the given entity.
		/// </summary>
		public static InternalField CreateModificationEntryId(AbstractEntity entity)
		{
			var name = EntitySchemaBuilder.EntityTableColumnEntityModificationEntryIdName;

			return new InternalField(entity, name);
		}


	}


}
