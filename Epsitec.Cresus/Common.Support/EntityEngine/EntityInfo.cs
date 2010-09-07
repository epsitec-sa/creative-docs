//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityInfo</c> class is used to retrieve runtime information
	/// about an entity, such as its DRUID.
	/// </summary>
	public static class EntityInfo
	{
		/// <summary>
		/// Gets the structured type id for the specified entity.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <returns>The entity id.</returns>
		public static Druid GetTypeId<T>()
			where T : AbstractEntity, new ()
		{
			return TypeIdProvider<T>.Instance.Id;
		}

		/// <summary>
		/// Gets the structured type key for the specified entity.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <returns>The entity key.</returns>
		public static string GetTypeKey<T>()
			where T : AbstractEntity, new ()
		{
			return TypeIdProvider<T>.Instance.Key;
		}

		#region TypeIdProvider Class

		private class TypeIdProvider<T>
			where T : AbstractEntity, new ()
		{
			public TypeIdProvider()
			{
				var entity = EmptyEntityContext.Instance.CreateEmptyEntity<T> ();
				this.id  = entity.GetEntityStructuredTypeId ();
				this.key = entity.GetEntityStructuredTypeKey ();
			}

			public Druid Id
			{
				get
				{
					return this.id;
				}
			}

			public string Key
			{
				get
				{
					return this.key;
				}
			}


			public static TypeIdProvider<T> Instance = new TypeIdProvider<T> ();

			private readonly Druid id;
			private readonly string key;
		}

		#endregion

		#region EmptyEntityContext class

		/// <summary>
		/// The <c>EmptyEntityContext</c> class will be used when creating empty entities
		/// for analysis purposes only.
		/// </summary>
		private class EmptyEntityContext : EntityContext
		{
			public EmptyEntityContext()
				: base (Resources.DefaultManager, EntityLoopHandlingMode.Throw, "Analysis/EmptyEntities")
			{
			}

			public static EmptyEntityContext Instance = new EmptyEntityContext ();
		}

		#endregion
	}
}
