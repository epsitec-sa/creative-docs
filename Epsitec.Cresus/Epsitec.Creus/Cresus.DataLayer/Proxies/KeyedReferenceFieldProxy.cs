using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.DataLayer.Context;


namespace Epsitec.Cresus.DataLayer.Proxies
{


	/// <summary>
	/// The <c>KyedReferenceFieldProxy</c> class is used as a placeholder for an
	/// <see cref="AbstractEntity"/>. The <see cref="AbstractEntity"/> that it represents is defined
	/// by an <see cref="EntityKey"/>. This <see cref="EntityKey"/> is used to look for the target
	/// in the <see cref="DataContext"/>. If the target is not cached in the <see cref="DataContext"/>,
	/// then we look in the database for the as in <see cref="ReferenceFieldProxy"/>.
	/// </summary>
	internal class KeyedReferenceFieldProxy : ReferenceFieldProxy
	{


		/// <summary>
		/// Builds a new <c>KeyedReferenceFieldProxy</c>.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"></see> responsible of <paramref name="entity"></paramref>.</param>
		/// <param name="entity">The <see cref="AbstractEntity"/> that references the <see cref="AbstractEntity"/> of this instance.</param>
		/// <param name="fieldId">The <see cref="Druid"/> of the field.</param>
		/// <param name="targetKey">The <see cref="EntityKey"/> that references the target of this instance.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dataContext"/> is null.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is null.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="fieldId"/> is empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="entity"/> is not managed by <paramref name="dataContext"/>.</exception>
		/// <exception cref="System.ArgumentException">If the field given by <paramref name="fieldId"/> is not valid for the <c>KeyedReferenceFieldProxy</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="targetKey"/> is empty.</exception>
		public KeyedReferenceFieldProxy(DataContext dataContext, AbstractEntity entity, Druid fieldId, EntityKey targetKey)
			: base (dataContext, entity, fieldId)
		{
			targetKey.ThrowIf (key => key.IsEmpty, "targetKey cannot be empty");
			
			this.targetKey = targetKey;
		}


		/// <summary>
		/// Promotes the proxy to its real instance.
		/// </summary>
		/// <returns>The real instance.</returns>
		public override object PromoteToRealInstance()
		{
			using (this.DataContext.LockWrite ())
			{
				object target = this.DataContext.GetEntity (this.targetKey);

				if (target == null)
				{
					target = base.PromoteToRealInstance ();
				}

				return target;
			}
		}
		

		/// <summary>
		/// The <see cref="EntityKey"/> that defines the target of this instance.
		/// </summary>
		private readonly EntityKey targetKey;


	}


}
