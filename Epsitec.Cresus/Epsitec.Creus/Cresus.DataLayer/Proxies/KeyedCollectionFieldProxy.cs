using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Proxies
{
	

	/// <summary>
	/// The <c>CollectionFieldProxy</c> class is used as a placeholder for a collection of
	/// <see cref="AbstractEntity"/>. They are defined by a list of <see cref="EntityKey"/> which are
	/// used to see if they are in the cache of the <see cref="DataContext"/>. If they aren't, the
	/// mechanism used to resolve them is the same as <see cref="CollectionFieldProxy"/>.
	/// </summary>
	internal class KeyedCollectionFieldProxy : CollectionFieldProxy
	{


		/// <summary>
		/// Builds a new <c>KeyedCollectionFieldProxy</c>..
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"></see> responsible of <paramref name="entity"></paramref>.</param>
		/// <param name="entity">The <see cref="AbstractEntity"/> that references the <see cref="AbstractEntity"/> of this instance.</param>
		/// <param name="fieldId">The <see cref="Druid"/> of the field.</param>
		/// <param name="targetKeys">The sequence of <see cref="EntityKey"/> defining the targets of this instance.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dataContext"/> is null.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is null.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="fieldId"/> is empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="entity"/> is not managed by <paramref name="dataContext"/>.</exception>
		/// <exception cref="System.ArgumentException">If the field given by <paramref name="fieldId"/> is not valid for the <c>KeyedCollectionFieldProxy</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="targetKeys"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="targetKeys"/> contains an empty element.</exception>
		public KeyedCollectionFieldProxy(DataContext dataContext, AbstractEntity entity, Druid fieldId, IEnumerable<EntityKey> targetKeys)
			: base (dataContext, entity, fieldId)
		{
			targetKeys.ThrowIfNull ("targetKeys");

			List<EntityKey> targetKeysAsList = targetKeys.ToList ();
			targetKeysAsList.ThrowIf (keys => keys.Any (key => key.IsEmpty), "targetKeys cannot contain empty elements");

			this.targetKeys = targetKeysAsList;
		}


		/// <summary>
		/// Promotes the proxy to its real instance.
		/// </summary>
		/// <returns>The real instance.</returns>
		public override object PromoteToRealInstance()
		{
			using (this.DataContext.LockWrite ())
			{
				List<AbstractEntity> targets = this.targetKeys
				.Select (tk => this.DataContext.GetEntity (tk))
				.ToList ();

				object result;

				if (targets.Contains (null))
				{
					result = base.PromoteToRealInstance ();
				}
				else
				{
					result = this.CreateEntityCollection (this.FieldId, targets);
				}

				return result;
			}
		}


		/// <summary>
		/// Stores the <see cref="EntityKey"/> that identifies the targets of this instance.
		/// </summary>
		private readonly IEnumerable<EntityKey> targetKeys;


	}


}
