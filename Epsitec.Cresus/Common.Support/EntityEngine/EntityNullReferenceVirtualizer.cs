//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.EntityEngine
{
	public static class EntityNullReferenceVirtualizer
	{
		public static void Virtualize<T>(T entity) where T : AbstractEntity
		{
			if (!EntityNullReferenceVirtualizer.IsVirtualizedEntity (entity))
			{
				EntityNullReferenceVirtualizer.VirtualizeEntity (entity, false);
			}
		}

		private static void VirtualizeEntity(AbstractEntity entity, bool readOnly)
		{
			entity.SetModifiedValues (new Store (entity.GetModifiedValues ()) { ReadOnly = readOnly });
			entity.SetOriginalValues (new Store (entity.GetOriginalValues ()) { ReadOnly = readOnly });
		}

		class Store : IValueStore
		{
			public Store(IValueStore realStore)
			{
				this.realStore = realStore;
				this.values = new Dictionary<string, object> ();
			}

			public bool ReadOnly
			{
				get;
				set;
			}

			#region IValueStore Members

			public object GetValue(string id)
			{
				object value = this.realStore.GetValue (id);

				if (UndefinedValue.IsUndefinedValue (value))
				{
					if (this.values.TryGetValue (id, out value))
					{
						return value;
					}

					IStructuredTypeProvider provider = this.realStore as IStructuredTypeProvider;
					if (provider != null)
					{
						StructuredType type = provider.GetStructuredType () as StructuredType;
						if (type != null)
						{
							var info = type.GetField (id);
							
							if (info.Relation == FieldRelation.Reference)
							{
								var entity = EntityClassFactory.CreateEmptyEntity (info.TypeId);
								EntityNullReferenceVirtualizer.VirtualizeEntity (entity, true);

								this.values.Add (id, entity);

								value = entity;
							}
						}
					}
				}

				return value;
			}

			public void SetValue(string id, object value, ValueStoreSetMode mode)
			{
				if (this.ReadOnly)
				{
					throw new System.InvalidOperationException ();
				}

				this.realStore.SetValue (id, value, mode);
				this.values.Remove (id);
			}

			#endregion

			private readonly IValueStore realStore;
			private readonly Dictionary<string, object> values;
		}

		public static bool IsVirtualizedEntity(AbstractEntity entity)
		{
			if (entity.InternalGetValueStores ().Any (store => store is Store))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
