using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.DataLayer.Context;


namespace Epsitec.Cresus.DataLayer.Proxies
{


	/// <summary>
	/// The <c>ValueFieldProxy</c> class is a placeholder for the value of a value field of an
	/// <see cref="AbstractEntity"/>.
	/// </summary>
	internal class ValueFieldProxy : IValueProxy
	{


		/// <summary>
		/// Builds a new <c>ValueFieldProxy</c>, which represents the value of the field with the id
		/// <paramref name="fieldId"/> of the <see cref="AbstractEntity"/> given by
		/// <paramref name="entity"/>.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"/> responsible of <paramref name="entity"/>.</param>
		/// <param name="entity">The entity.</param>
		/// <param name="fieldId">The id of the field.</param>
		public ValueFieldProxy(DataContext dataContext, AbstractEntity entity, Druid fieldId)
		{
			if (dataContext == null)
			{
				throw new System.ArgumentNullException ("dataContext");
			}

			if (entity == null)
			{
				throw new System.ArgumentNullException ("entity");
			}

			if (fieldId.IsEmpty)
			{
				throw new System.ArgumentException ("fieldId is not valid.");
			}

			// TODO Add more test on the input arguments, such as to detect if entity is not managed
			// by dataContext, or if fieldId is not a field of entity ?
			// Marc
			
			this.dataContext = dataContext;
			this.entity = entity;
			this.fieldId = fieldId;
		}


		#region IEntityProxy Members


		/// <summary>
		/// Gets the real value represented by the current instance.
		/// </summary>
		/// <returns></returns>
		public object GetValue()
		{
			return this.dataContext.DataLoader.GetFieldValue (entity, fieldId);
		}


		#endregion


		/// <summary>
		/// The <see cref="DataContext"/> responsible of the <see cref="AbstractEntity"/> of this
		/// instance.
		/// </summary>
		private readonly DataContext dataContext;


		/// <summary>
		/// The <see cref="AbstractEntity"/> whose one of the field is represented by this instance.
		/// </summary>
		private readonly AbstractEntity entity;


		/// <summary>
		/// The id of the field represented by this instance.
		/// </summary>
		private readonly Druid fieldId;

	
	}


}
