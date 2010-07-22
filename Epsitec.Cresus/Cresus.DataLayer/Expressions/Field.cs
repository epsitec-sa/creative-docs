using Epsitec.Common.Support;

using Epsitec.Cresus.Database;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>Field</c> class represents a field of an <see cref="AbstractEntity"/> in an
	/// <see cref="Expression"/>.
	/// </summary>
	public sealed class Field
	{


		/// <summary>
		/// Creates a new <c>Field</c>.
		/// </summary>
		/// <param name="FieldId">The <see cref="Druid"/> that identifies the <c>Field</c>.</param>
		public Field(Druid FieldId)
		{
			this.FieldId = FieldId;
		}


		/// <summary>
		/// Gets the <see cref="Druid"/> that identifies the <c>Field</c>.
		/// </summary>
		public Druid FieldId
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the <see cref="DbTableColumn"/> corresponding to this <c>Field</c> in the database.
		/// </summary>
		/// <param name="dbTableColumnResolver">
		/// A delegate that will return the <see cref="DbTableColumn"/> corresponding to a given
		/// <see cref="Druid"/>.
		/// </param>
		/// <returns>The <see cref="DbTableColumn"/></returns>
		internal DbTableColumn CreateDbTableColumn(System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			return dbTableColumnResolver (this.FieldId);
		}


	}


}
