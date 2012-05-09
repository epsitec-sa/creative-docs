using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>EntityField</c> class represents a field of an <see cref="AbstractEntity"/> in an
	/// <see cref="Expression"/>.
	/// </summary>
	public abstract class EntityField : Value
	{


		/// <summary>
		/// Creates a new <c>EntityField</c>.
		/// </summary>
		/// <param name="entity">The entity that will be referenced by this instance</param>
		protected EntityField(AbstractEntity entity)
		{
			entity.ThrowIfNull ("entity");

			this.Entity = entity;
		}


		/// <summary>
		/// The entity referenced by this instance.
		/// </summary>
		public AbstractEntity Entity
		{
			get;
			private set;
		}
	
	
	}


}