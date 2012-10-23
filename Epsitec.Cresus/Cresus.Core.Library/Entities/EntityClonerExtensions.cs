//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	/// <summary>
	/// The <c>EntityClonerExtensions</c> class provides extensions methods used to clone
	/// entities.
	/// </summary>
	public static class EntityClonerExtensions
	{
		/// <summary>
		/// Clones the specified entity.
		/// </summary>
		/// <typeparam name="T">The type of the entity (or the type of one of its base classes).</typeparam>
		/// <param name="entity">The entity to clone.</param>
		/// <param name="businessContext">The business context.</param>
		/// <returns>The cloned entity.</returns>
		/// <exception cref="System.InvalidOperationException">Throws an invalid operation exception if
		/// the entity does not implement <see cref="ICopyableEntity&lt;T&gt;"/>.</exception>
		public static T CloneEntity<T>(this T entity, BusinessContext businessContext)
			where T : AbstractEntity, new ()
		{
			if (entity.IsNull ())
			{
				return null;
			}

			var type = entity.GetType ();

			if (type == typeof (T))
			{
				//	The caller already operates on the most derived type, we can therefore
				//	simply create a new instance of T and copy it:

				return EntityCopier<T>.Clone (businessContext, entity);
			}
			else
			{
				//	The caller did not specify the most derived type (he may be operating
				//	on a base class, for instance). We must find the adequate copier:

				var copier = EntityClonerExtensions.GetEntityCopier (type);

				return copier.GenericClone (businessContext, entity) as T;
			}
		}

		private static EntityCopier GetEntityCopier(System.Type type)
		{
			if (EntityClonerExtensions.copiers == null)
			{
				EntityClonerExtensions.copiers = new Dictionary<System.Type, EntityCopier> ();
			}

			EntityCopier copier;

			if (EntityClonerExtensions.copiers.TryGetValue (type, out copier) == false)
			{
				copier = System.Activator.CreateInstance (typeof (EntityCopier<>).MakeGenericType (type)) as EntityCopier;
				EntityClonerExtensions.copiers[type] = copier;
			}
			
			return copier;
		}

		#region EntityCopier Class

		private abstract class EntityCopier
		{
			public abstract AbstractEntity GenericClone(BusinessContext businessContext, AbstractEntity entity);
		}

		#endregion

		#region EntityCopier<T> Class

		private sealed class EntityCopier<T> : EntityCopier
			where T : AbstractEntity, new ()
		{
			public EntityCopier()
			{
			}

			public static T Clone(BusinessContext businessContext, AbstractEntity entity)
			{
				var cloneable = entity as ICopyableEntity<T>;

				if (cloneable == null)
				{
					throw new System.InvalidOperationException ("Cannot clone entity");
				}

				var copy = businessContext.CreateEntity<T> ();

				cloneable.CopyTo (businessContext, copy);

				return copy;
			}

			public override AbstractEntity GenericClone(BusinessContext businessContext, AbstractEntity entity)
			{
				return EntityCopier<T>.Clone (businessContext, entity);
			}
		}

		#endregion

		[System.ThreadStatic]
		private static Dictionary<System.Type, EntityCopier> copiers;
	}
}
