/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

namespace Epsitec.Common.Support.EntityEngine
{
    // TODO It would be nice to clean that class out of its black magic, which makes it quite
    // difficult to understand. But, I'm not sure if this is really possible.
    // In addition, it would be nice to remove all the stuff related to virtualizing entities in
    // entity collections, in this class and in EntityCollection, or to implement it completely.
    // Actually, I think that this stuff is not used in practice, as we never put proxy instances in
    // collection, but the whole collection is the proxy.
    // Marc


    /// <summary>
    /// The <c>EntityNullReferenceVirtualizer</c> provides the mechanism used to
    /// automatically generate empty entities when null references are dereferenced.
    /// </summary>
    public static class EntityNullReferenceVirtualizer
    {
        /// <summary>
        /// Patches the null references in the specified entity.
        /// </summary>
        /// <typeparam name="T">An entity type.</typeparam>
        /// <param name="entity">The entity which will be patched.</param>
        public static void PatchNullReferences<T>(T entity)
            where T : AbstractEntity
        {
            entity.ThrowIfNull("entity");

            EntityContext realEntityContext = entity.GetEntityContext();

            EntityNullReferenceVirtualizer.PatchNullReferences(
                entity,
                realEntityContext,
                newEntity: false
            );
        }

        /// <summary>
        /// Determines whether the specified entity was patched using <see cref="PatchNullReferences"/>.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>
        /// 	<c>true</c> if the specified entity was patched; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPatchedEntity(AbstractEntity entity)
        {
            if (entity == null)
            {
                return false;
            }

            if (entity.InternalGetValueStores().Any(store => store is Store))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the specified entity is null or is an empty virtualized entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// 	<c>true</c> if the specified entity is null or is an empty virtualized entity; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullEntity(AbstractEntity entity)
        {
            if (entity == null)
            {
                return true;
            }

            var context = entity.GetEntityContext();

            return EntityNullReferenceVirtualizer.IsEmptyEntityContext(context);
        }

        /// <summary>
        /// Determines whether the specified context is an empty entity context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// 	<c>true</c> if the specified context is an empty entity context; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEmptyEntityContext(EntityContext context)
        {
            return context is EmptyEntityContext;
        }

        /// <summary>
        /// Determines whether the specified entity was patched using <see cref="PatchNullReferences"/>
        /// and still is unchanged.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>
        /// 	<c>true</c> if the specified entity is a patched and still unchanged entity; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPatchedEntityStillUnchanged(AbstractEntity entity)
        {
            if (entity == null)
            {
                return false;
            }
            else
            {
                var stores = entity.InternalGetValueStores().OfType<Store>().ToArray();

                if ((stores.Length > 0) && (stores.All(store => store.IsReadOnly)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Unwraps the entity. If it maps to a null entity, then return <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns><c>null</c> if the entity maps to a null entity; otherwise, the entity.</returns>
        public static T UnwrapNullEntity<T>(this T entity)
            where T : AbstractEntity
        {
            if (EntityNullReferenceVirtualizer.IsNullEntity(entity))
            {
                return null;
            }
            else
            {
                return entity;
            }
        }

        /// <summary>
        /// Creates a null, read-only entity, which will be identified as being equal to <c>null</c>.
        /// It will not be possible to automatically promote this null entity to a real entity.
        /// </summary>
        /// <typeparam name="T">the type of the entity</typeparam>
        /// <returns>The null entity.</returns>
        public static T CreateNullEntity<T>()
            where T : AbstractEntity, new()
        {
            var context = EntityNullReferenceVirtualizer.GetEmptyEntityContext();
            var entity = context.CreateEmptyEntity<T>();

            entity.Freeze();

            return entity;
        }

        /// <summary>
        /// Creates a null entity for the specified field in the specified parent entity.
        /// </summary>
        /// <param name="parentEntity">The parent entity.</param>
        /// <param name="field">The field.</param>
        /// <returns>The null entity.</returns>
        internal static AbstractEntity CreateNullEntityForField(
            AbstractEntity parentEntity,
            StructuredTypeField field
        )
        {
            parentEntity.ThrowIfNull("entity");
            field.ThrowIfNull("field");

            if (field.Relation != FieldRelation.Reference)
            {
                throw new System.InvalidOperationException(
                    "Cannot set field to null entity for this field type"
                );
            }

            var fieldTypeId = field.TypeId;
            var entityType = EntityInfo.GetType(fieldTypeId);

            var creatorType = typeof(NullEntityCreator<>).MakeGenericType(entityType);
            var creatorObject = System.Activator.CreateInstance(creatorType) as NullEntityCreator;

            var nullEntity = creatorObject.CreateEmptyEntity(parentEntity, field);

            if (parentEntity.IsReadOnly)
            {
                nullEntity.Freeze();
            }

            return nullEntity;
        }

        /// <summary>
        /// Wraps the entity. If it is <c>null</c>, then create a virtualized empty entity.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>The wrapped entity.</returns>
        internal static T WrapNullEntity<T>(EntityContext realEntityContext, T entity)
            where T : AbstractEntity, new()
        {
            return entity
                ?? EntityNullReferenceVirtualizer.CreateEmptyEntity<T>(realEntityContext, false);
        }

        /// <summary>
        /// Creates an empty entity which will be considered equal to a null reference
        /// when calling <see cref="IsNullEntity"/>.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>The empty entity created the same way as the automatically generated entity produced by the virtualizer.</returns>
        internal static T CreateEmptyEntity<T>(EntityContext realEntityContext, bool freeze)
            where T : AbstractEntity, new()
        {
            var emptyEntityContext = EntityNullReferenceVirtualizer.GetEmptyEntityContext();
            var entity = emptyEntityContext.CreateEmptyEntity<T>();

            EntityNullReferenceVirtualizer.PatchNullReferences(
                entity,
                realEntityContext,
                newEntity: true
            );

            if (freeze)
            {
                entity.Freeze();
            }

            return entity;
        }

        private static void PatchNullReferences<T>(
            T entity,
            EntityContext realEntityContext,
            bool newEntity
        )
            where T : AbstractEntity
        {
            if (EntityNullReferenceVirtualizer.IsPatchedEntity(entity))
            {
                return;
            }

            var originalValues = entity.GetOriginalValues();
            var modifiedValues = entity.GetModifiedValues();

            var newOriginalValues = new Store(
                originalValues,
                modifiedValues,
                entity,
                realEntityContext,
                newEntity
            );
            var newModifiedValues = newEntity
                ? new StoreForwarder(modifiedValues, newOriginalValues)
                : modifiedValues;

            entity.SetOriginalValues(newOriginalValues);
            entity.SetModifiedValues(newModifiedValues);
        }

        /// <summary>
        /// Patches the null references of the specified (empty) entity, so that it can be used
        /// as a virtualized null entity. The specified entity will be attached to the specified
        /// parent entity field.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="parent">The parent entity.</param>
        /// <param name="field">The field in the parent entity.</param>
        private static void PatchNullReferences<T>(
            T entity,
            AbstractEntity parent,
            StructuredTypeField field
        )
            where T : AbstractEntity
        {
            System.Diagnostics.Debug.Assert(EntityNullReferenceVirtualizer.IsPatchedEntity(parent));
            System.Diagnostics.Debug.Assert(
                EntityNullReferenceVirtualizer.IsPatchedEntity(entity) == false
            );

            var originalValues = entity.GetOriginalValues();
            var modifiedValues = entity.GetModifiedValues();

            var parentStore = parent.GetOriginalValues() as Store;

            System.Diagnostics.Debug.Assert(parentStore != null);

            var newOriginalValues = new Store(
                originalValues,
                modifiedValues,
                entity,
                parentStore,
                field.Id
            );
            var newModifiedValues = new StoreForwarder(modifiedValues, newOriginalValues);

            entity.SetOriginalValues(newOriginalValues);
            entity.SetModifiedValues(newModifiedValues);
        }

        /// <summary>
        /// Creates an empty entity attached to a dedicated context.
        /// </summary>
        /// <param name="druid">The druid.</param>
        /// <returns>The empty entity.</returns>
        private static AbstractEntity CreateEmptyEntity(Druid druid)
        {
            var context = EntityNullReferenceVirtualizer.GetEmptyEntityContext();

            return context.CreateEmptyEntity(druid);
        }

        /// <summary>
        /// Gets the context used to create empty entities.
        /// </summary>
        /// <returns>The <see cref="EntityContext"/>.</returns>
        private static EntityContext GetEmptyEntityContext()
        {
            if (EntityNullReferenceVirtualizer.emptyEntityContext == null)
            {
                EntityNullReferenceVirtualizer.emptyEntityContext = new EmptyEntityContext();
            }

            return EntityNullReferenceVirtualizer.emptyEntityContext;
        }

        #region NullEntityCreator Class

        private abstract class NullEntityCreator
        {
            public abstract AbstractEntity CreateEmptyEntity(
                AbstractEntity parent,
                StructuredTypeField field
            );
        }

        #endregion

        #region NullEntityCreator<T> Class

        private class NullEntityCreator<T> : NullEntityCreator
            where T : AbstractEntity, new()
        {
            public override AbstractEntity CreateEmptyEntity(
                AbstractEntity parent,
                StructuredTypeField field
            )
            {
                var emptyEntityContext = EntityNullReferenceVirtualizer.GetEmptyEntityContext();
                var entity = emptyEntityContext.CreateEmptyEntity<T>();

                EntityNullReferenceVirtualizer.PatchNullReferences(entity, parent, field);

                return entity;
            }
        }

        #endregion

        #region EmptyEntityContext class

        /// <summary>
        /// The <c>EmptyEntityContext</c> class will be used when creating empty entities.
        /// This allows the virtualizer to identify an empty entity very easily by just
        /// checking if it belongs to this context, or not.
        /// </summary>
        private sealed class EmptyEntityContext : EntityContext
        {
            public EmptyEntityContext()
                : base(SafeResourceResolver.Instance, EntityLoopHandlingMode.Throw, "EmptyEntities")
            { }
        }

        #endregion

        #region Store Class

        /// <summary>
        /// The <c>Store</c> class implements the <c>GetValue</c> and <c>SetValue</c> accessors used
        /// by the entity class to access its internal data store; <c>Store.GetValue</c> handles undefined
        /// references by instantiating empty entities on the fly, whereas <c>Store.SetValue</c> on such
        /// an empty entity will transform it into a real entity. Warning: advanced magic is going on here.
        /// </summary>
        private sealed class Store : IValueStore
        {
            public Store(
                IValueStore realReadStore,
                IValueStore realWriteStore,
                AbstractEntity entity,
                EntityContext realEntityContext,
                bool isReadOnly
            )
            {
                this.realReadStore = realReadStore;
                this.realWriteStore = realWriteStore;
                this.realEntityContext = realEntityContext;

                this.values = new Dictionary<string, object>();
                this.entity = entity;

                this.isReadOnly = isReadOnly;
            }

            public Store(
                IValueStore realReadStore,
                IValueStore realWriteStore,
                AbstractEntity entity,
                Store parentStore,
                string fieldIdInParentStore
            )
                : this(realReadStore, realWriteStore, entity, parentStore.realEntityContext, true)
            {
                this.parentStore = parentStore;
                this.fieldIdInParentStore = fieldIdInParentStore;
            }

            public EntityContext RealEntityContext
            {
                get { return this.realEntityContext; }
            }

            public bool IsReadOnly
            {
                get { return this.isReadOnly; }
            }

            #region IValueStore Members

            public object GetValue(string id)
            {
                object value = this.realReadStore.GetValue(id);

                if (UndefinedValue.IsUndefinedValue(value))
                {
                    object outValue;

                    if (this.values.TryGetValue(id, out outValue))
                    {
                        return outValue;
                    }
                    else
                    {
                        bool freezeChild = this.entity.IsReadOnly;

                        return this.CreateEmptyEntityForUndefinedField(id, value, freezeChild);
                    }
                }

                if (value is AbstractEntity)
                {
                    var entity = value as AbstractEntity;

                    EntityNullReferenceVirtualizer.PatchNullReferences(
                        entity,
                        this.realEntityContext,
                        newEntity: false
                    );
                }
                else if (value is EntityCollection)
                {
                    var collection = value as EntityCollection;
                    collection.EnableEntityNullReferenceVirtualizer();
                }

                return value;
            }

            public void SetValue(string id, object value, ValueStoreSetMode mode)
            {
                if (mode == ValueStoreSetMode.InitialCollection)
                {
                    var collection = value as EntityCollection;

                    System.Diagnostics.Debug.Assert(collection != null);
                    System.Diagnostics.Debug.Assert(collection.Count == 0);

                    collection.EnableEntityNullReferenceVirtualizer();

                    this.realReadStore.SetValue(id, value, mode);
                }
                else
                {
                    this.TranformNullEntityIntoLiveEntity();
                    this.ReplaceValue(id, value, mode);
                }
            }

            #endregion


            public void TranformNullEntityIntoLiveEntity()
            {
                if (this.IsReadOnly)
                {
                    // If the entity has a parent, we make sure that it is alive. Then we make
                    // ourselves alive.

                    if (this.parentStore != null && this.fieldIdInParentStore != null)
                    {
                        this.parentStore.SetLiveEntity(this.fieldIdInParentStore, this.entity);
                    }

                    this.isReadOnly = false;

                    var entityContext = this.realEntityContext;

                    this.entity.AssignEntityContext(entityContext);
                }
            }

            private void SetLiveEntity(string fieldId, AbstractEntity entity)
            {
                //	Make sure this is a real entity, before recording the entity into the specified
                //	reference field:

                this.TranformNullEntityIntoLiveEntity();

                this.ReplaceValue(fieldId, entity, ValueStoreSetMode.Default);

                this.entity.UpdateDataGeneration();
            }

            private void ReplaceValue(string id, object value, ValueStoreSetMode mode)
            {
                if (mode == ValueStoreSetMode.ShortCircuit)
                {
                    //	The short-circuit mode is used by AbstractEntity.SetModifiedValueAsOriginalValue when it
                    //	is resetting the original values in the store. We must therefore use the 'read store' as
                    //	the target (which usually maps to the original store).

                    this.realReadStore.SetValue(id, value, mode);
                }
                else
                {
                    this.realWriteStore.SetValue(id, value, mode);
                }

                this.values.Remove(id);
            }

            private object CreateEmptyEntityForUndefinedField(
                string id,
                object defaultValue,
                bool freeze
            )
            {
                var info = this.GetStructuredTypeField(id);

                if (info == null)
                {
                    return defaultValue;
                }

                if (info.Relation == FieldRelation.Reference)
                {
                    return this.CreateEmptyEntityForField(id, info, freeze);
                }

                return defaultValue;
            }

            private AbstractEntity CreateEmptyEntityForField(
                string id,
                StructuredTypeField info,
                bool freeze
            )
            {
                var entity = EntityNullReferenceVirtualizer.CreateEmptyEntity(info.TypeId);

                if (freeze)
                {
                    entity.Freeze();
                }

                System.Diagnostics.Debug.Assert(entity != null);

                Store.PatchNullReferences(entity, parentStore: this, id: id);
                this.values.Add(id, entity);

                return entity;
            }

            private StructuredTypeField GetStructuredTypeField(string id)
            {
                var provider = this.realReadStore as IStructuredTypeProvider;

                if (provider == null)
                {
                    return null;
                }

                var type = provider.GetStructuredType() as StructuredType;

                if (type == null)
                {
                    return null;
                }
                else
                {
                    return type.GetField(id);
                }
            }

            private static void PatchNullReferences(
                AbstractEntity entity,
                Store parentStore,
                string id
            )
            {
                IValueStore realOriginalValueStore = entity.GetOriginalValues();
                IValueStore realModifiedValueStore = entity.GetModifiedValues();

                var store1 = new Store(
                    realOriginalValueStore,
                    realModifiedValueStore,
                    entity,
                    parentStore,
                    id
                );
                var store2 = new StoreForwarder(realModifiedValueStore, store1);

                entity.SetOriginalValues(store1);
                entity.SetModifiedValues(store2);
            }

            private readonly IValueStore realReadStore;
            private readonly IValueStore realWriteStore;
            private readonly Dictionary<string, object> values;
            private readonly AbstractEntity entity;
            private readonly EntityContext realEntityContext;
            private readonly Store parentStore;
            private readonly string fieldIdInParentStore;
            private bool isReadOnly;
        }

        #endregion

        #region StoreForwarder Class

        private class StoreForwarder : IValueStore
        {
            public StoreForwarder(IValueStore store1, Store store2)
            {
                this.store1 = store1;
                this.store2 = store2;
            }

            #region IValueStore Members


            public object GetValue(string id)
            {
                return this.store1.GetValue(id);
            }

            public void SetValue(string id, object value, ValueStoreSetMode mode)
            {
                this.store1.SetValue(id, value, mode);
                this.store2.TranformNullEntityIntoLiveEntity();
            }

            #endregion

            private readonly IValueStore store1;
            private readonly Store store2;
        }

        #endregion

        [System.ThreadStatic]
        private static EntityContext emptyEntityContext;
    }
}
