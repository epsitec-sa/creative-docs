using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Context;


namespace Epsitec.Cresus.DataLayer.Proxies
{


	/// <summary>
	/// The <c>ValueFieldProxy</c> class is a placeholder for the value of a value field of an
	/// <see cref="AbstractEntity"/>.
	/// </summary>
	internal class ValueFieldProxy : AbstractFieldProxy, IValueProxy
	{


		/// <summary>
		/// Builds a new <c>ValueFieldProxy</c>, which represents the value of the field with the id
		/// <paramref name="fieldId"/> of the <see cref="AbstractEntity"/> given by
		/// <paramref name="entity"/>.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"/> responsible of <paramref name="entity"/>.</param>
		/// <param name="entity">The entity.</param>
		/// <param name="fieldId">The id of the field.</param>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="dataContext"/> is null.
		/// If <paramref name="entity"/> is null.
		/// </exception>
		/// <exception cref="System.ArgumentException">If <paramref name="fieldId"/> is empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="entity"/> is not managed by <paramref name="dataContext"/>.</exception>
		/// <exception cref="System.ArgumentException">If the field given by <paramref name="fieldId"/> is not valid for the <c>ValueFieldProxy</c>.</exception>
		public ValueFieldProxy(DataContext dataContext, AbstractEntity entity, Druid fieldId)
			: base (dataContext, entity, fieldId)
		{
		}


		/// <summary>
		/// Gets the kind of <see cref="FieldRelation"/> of the field used by this instance.
		/// </summary>
		protected override FieldRelation FieldRelation
		{
			get
			{
				return FieldRelation.None;
			}
		}


		#region IEntityProxy Members


		/// <summary>
		/// Gets the real value represented by the current instance.
		/// </summary>
		/// <returns></returns>
		public object GetValue()
		{
			using (this.DataContext.LockWrite ())
			{
				object result = this.DataContext.DataLoader.ResolveValueField (Entity, FieldId);

				return result ?? UndefinedValue.Value;
			}
		}


		#endregion

	
	}


}
