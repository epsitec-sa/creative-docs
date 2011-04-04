using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Schema;


namespace Epsitec.Cresus.DataLayer.Proxies
{


	/// <summary>
	/// The <c>AbstractFieldProxy</c> class is the base class to used for the placeholder of what is
	/// targeted by the field of an <see cref="AbstractEntity"/>.
	/// </summary>
	internal abstract class AbstractFieldProxy
	{


		/// <summary>
		/// Builds a new <c>AbstractFieldProxy</c> which represents the <see cref="AbstractEntity"/>
		/// a proxy for what is referenced by the field with the <see cref="Druid"/> <paramref name="fieldId"/>
		/// of <paramref name="entity"/>.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"></see> responsible of <paramref name="entity"></paramref>.</param>
		/// <param name="entity">The <see cref="AbstractEntity"/> that references the <see cref="AbstractEntity"/> of this instance.</param>
		/// <param name="fieldId">The <see cref="Druid"/> of the field.</param>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="dataContext"/> is null.
		/// If <paramref name="entity"/> is null.
		/// </exception>
		/// <exception cref="System.ArgumentException">If <paramref name="fieldId"/> is empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="entity"/> is not managed by <paramref name="dataContext"/>.</exception>
		/// <exception cref="System.ArgumentException">If the field given by <paramref name="fieldId"/> is not valid for the <c>AbstractFieldProxy</c>.</exception>
		public AbstractFieldProxy(DataContext dataContext, AbstractEntity entity, Druid fieldId)
		{
			dataContext.ThrowIfNull ("dataContext");
			entity.ThrowIfNull ("entity");
			entity.ThrowIf (e => !dataContext.Contains (e), "dataContext does not own entity");
			fieldId.ThrowIf (id => id.IsEmpty, "fieldId cannot be empty");
			fieldId.ThrowIf (id => !this.CheckFieldId (dataContext, entity, id), "fieldId is not valid for entity.");

			this.DataContext = dataContext;
			this.Entity = entity;
			this.FieldId = fieldId;
		}


		/// <summary>
		/// Checks that the field given by a <see cref="Druid"/> exists in the given
		/// <see cref="AbstractEntity"/> and is of the appropriate relation type.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"></see> responsible of <paramref name="entity"></paramref>.</param>
		/// <param name="entity">The <see cref="AbstractEntity"/> to check.</param>
		/// <param name="fieldId">The <see cref="Druid"/> defining the field to check.</param>
		/// <returns><c>true</c> if the <see cref="Druid"/> defines a valid field for the <see cref="AbstractEntity"/>.</returns>
		private bool CheckFieldId(DataContext dataContext, AbstractEntity entity, Druid fieldId)
		{
			Druid entityTypeId = entity.GetEntityStructuredTypeId ();

			EntityTypeEngine entityTypeEngine = dataContext.DataInfrastructure.EntityEngine.EntityTypeEngine;

			StructuredTypeField field;

			try
			{
				// If the field does not exists, entityTypeEngine throws an exception because it
				// cannot find it. Therefore we catch and return false in that case. If this
				// statement succeed, then we know that the field exists.
				// Marc

				field = entityTypeEngine.GetField (entityTypeId, fieldId);
			}
			catch (System.ArgumentException)
			{
				return false;
			}

			return field.Relation == this.FieldRelation;
		}


		/// <summary>
		/// Gets the kind of <see cref="FieldRelation"/> of the field used by this instance.
		/// </summary>
		protected abstract FieldRelation FieldRelation
		{
			get;
		}


		/// <summary>
		/// The <see cref="DataContext"/> responsible of the <see cref="AbstractEntity"/> of this
		/// instance.
		/// </summary>
		protected DataContext DataContext
		{
			get;
			private set;
		}


		/// <summary>
		/// The <see cref="AbstractEntity"/> whose one of the field is represented by this instance.
		/// </summary>
		protected AbstractEntity Entity
		{
			get;
			private set;
		}


		/// <summary>
		/// The id of the field represented by this instance.
		/// </summary>
		protected Druid FieldId
		{
			get;
			private set;
		}


	}


}
