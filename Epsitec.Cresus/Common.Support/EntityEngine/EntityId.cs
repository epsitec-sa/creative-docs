//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.EntityEngine
{
	public static class EntityId
	{
		public static Druid GetTypeId<T>()
			where T : AbstractEntity, new ()
		{
			return TypeIdProvider<T>.Instance.Id;
		}

		#region TypeIdProvider Class

		private class TypeIdProvider<T>
			where T : AbstractEntity, new ()
		{
			public TypeIdProvider()
			{
				var entity = EmptyEntityContext.Instance.CreateEmptyEntity<T> ();
				this.id = entity.GetEntityStructuredTypeId ();
			}

			public Druid Id
			{
				get
				{
					return this.id;
				}
			}


			public static TypeIdProvider<T> Instance = new TypeIdProvider<T> ();

			private readonly Druid id;
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
