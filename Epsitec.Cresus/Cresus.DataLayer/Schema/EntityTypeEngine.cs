using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Schema
{


	internal sealed class EntityTypeEngine
	{


		/*
		 * All the method of this class are thread safe, but the StructuredType and
		 * StructuredTypeField that it returns are not. There is no formal guarantee whatsoever that
		 * they are thread safe. However, given how these objects are used within the DataLayer
		 * project (they are accessed only for read operations) and that they are not modified
		 * by the Cresus.Core project (they are supposed to be accessed only for read operation)
		 * and that this class calls the appropriate methods so that their internal state is supposed
		 * to be stable at the end of the constructor execution, they can be used in a thread safe
		 * way by the DataLayer project.
		 * However, I repeat, there are no formal guarantees on that. These objects are not
		 * synchronized and are mutable. This is some kind of "we know that it will work, so finger
		 * crossed" situation. And of course, if they are modified in any way, all those assumptions
		 * might turn out to be false and then we'll be screwed up.
		 * Marc
		 */


		public EntityTypeEngine(IEnumerable<Druid> entityTypeIds)
		{
			entityTypeIds.ThrowIfNull ("entityTypeIds");

			this.entityTypesCache = this.ComputeEntityTypesCache (entityTypeIds);
			this.entityTypeCache = this.ComputeEntityTypeCache ();
			this.baseTypesCache = this.ComputeBaseTypesCache ();
			this.rootTypeCache = this.ComputeRootTypeCache ();
			this.fieldsCache = this.ComputeFieldsCache ();
			this.fieldCache = this.ComputeFieldCache ();
			this.localTypeCache = this.ComputeLocalTypeCache ();
			this.valueFieldsCache = this.ComputeValueFieldsCache ();
			this.referenceFieldsCache = this.ComputeReferenceFieldsCache ();
			this.collectionFieldsCache = this.ComputeCollectionFieldsCache ();
			this.localFieldsCache = this.ComputeLocalFieldsCache ();
			this.localValueFieldsCache = this.ComputeLocalValueFieldsCache ();
			this.localReferenceFieldsCache = this.ComputeLocalReferenceFieldsCache ();
			this.localCollectionFieldsCache = this.ComputeLocalCollectionFieldsCache ();
			this.referencingFieldsCache = this.ComputeReferencingFieldsCache ();

			this.EnsureReferencedObjectsAreDeserialized ();
		}


		public ReadOnlyCollection<StructuredType> GetEntityTypes()
		{
			return this.entityTypesCache;
		}


		private ReadOnlyCollection<StructuredType> ComputeEntityTypesCache(IEnumerable<Druid> entityTypeIds)
		{
			return EntityTypeEngine.ComputeEntityTypes (entityTypeIds);
		}


		public StructuredType GetEntityType(Druid entityTypeId)
		{
			return EntityTypeEngine.GetFromCache (this.entityTypeCache, entityTypeId);
		}


		private ReadOnlyDictionary<Druid, StructuredType> ComputeEntityTypeCache()
		{
			return this.entityTypesCache
				.ToDictionary
				(
					t => t.CaptionId,
					t => t
				)
				.AsReadOnlyDictionary ();
		}


		public ReadOnlyCollection<StructuredType> GetBaseTypes(Druid entityTypeId)
		{
			return EntityTypeEngine.GetFromCache (this.baseTypesCache, entityTypeId);
		}


		private ReadOnlyDictionary<Druid, ReadOnlyCollection<StructuredType>> ComputeBaseTypesCache()
		{
			return this.entityTypesCache
				.ToDictionary
				(
					t => t.CaptionId,
					t => this.ComputeBaseTypes (t)
				)
				.AsReadOnlyDictionary ();
		}


		private ReadOnlyCollection<StructuredType> ComputeBaseTypes(StructuredType type)
		{
			List<StructuredType> baseTypes = new List<StructuredType> (){};

			while (type != null)
			{
				baseTypes.Add (type);

				type = type.BaseType;
			}

			return baseTypes.AsReadOnly ();
		}


		public StructuredType GetRootType(Druid entityTypeId)
		{
			return EntityTypeEngine.GetFromCache (this.rootTypeCache, entityTypeId);
		}


		private ReadOnlyDictionary<Druid, StructuredType> ComputeRootTypeCache()
		{
			return this.entityTypesCache
				.ToDictionary
				(
					t => t.CaptionId,
					t => this.ComputeRootType (t.CaptionId)
				)
				.AsReadOnlyDictionary ();
		}


		private StructuredType ComputeRootType(Druid entityTypeId)
		{
			var baseTypes = this.GetBaseTypes (entityTypeId);

			return baseTypes[baseTypes.Count - 1];
		}


		public ReadOnlyCollection<StructuredTypeField> GetFields(Druid entityTypeId)
		{
			return EntityTypeEngine.GetFromCache (this.fieldsCache, entityTypeId);
		}


		private ReadOnlyDictionary<Druid, ReadOnlyCollection<StructuredTypeField>> ComputeFieldsCache()
		{
			return this.entityTypesCache
				.ToDictionary
				(
					f => f.CaptionId,
					f => this.ComputeFields (f)
				)
				.AsReadOnlyDictionary ();
		}


		private ReadOnlyCollection<StructuredTypeField> ComputeFields(StructuredType entityType)
		{
			return entityType.Fields.Values
				.Where (f => f.Source == FieldSource.Value)
				.AsReadOnlyCollection ();
		}


		public StructuredTypeField GetField(Druid entityTypeId, Druid fieldId)
		{
			var key = Tuple.Create (entityTypeId, fieldId);

			return EntityTypeEngine.GetFromCache (this.fieldCache, key);
		}


		private ReadOnlyDictionary<Tuple<Druid, Druid>, StructuredTypeField> ComputeFieldCache()
		{
			return this.fieldsCache
				.SelectMany (item => item.Value.Select (f => new { TypeId = item.Key, Field = f }))
				.ToDictionary
				(
					item => Tuple.Create (item.TypeId, item.Field.CaptionId),
					item => item.Field
				)
				.AsReadOnlyDictionary ();
		}


		public StructuredType GetLocalType(Druid entityTypeId, Druid fieldId)
		{
			var key = Tuple.Create (entityTypeId, fieldId);

			return EntityTypeEngine.GetFromCache (this.localTypeCache, key);
		}


		private ReadOnlyDictionary<Tuple<Druid, Druid>, StructuredType> ComputeLocalTypeCache()
		{
			return this.fieldCache
				.ToDictionary
				(
					item => item.Key,
					item => this.ComputeLocalType (item.Key.Item1, item.Key.Item2)
				)
				.AsReadOnlyDictionary ();
		}


		private StructuredType ComputeLocalType(Druid entityTypeId, Druid fieldId)
		{
			return this.baseTypesCache[entityTypeId]
				.First (t => t.GetField (fieldId.ToResourceId ()).Membership != FieldMembership.Inherited);
		}


		public ReadOnlyCollection<StructuredTypeField> GetValueFields(Druid entityTypeId)
		{
			return EntityTypeEngine.GetFromCache (this.valueFieldsCache, entityTypeId);
		}


		private ReadOnlyDictionary<Druid, ReadOnlyCollection<StructuredTypeField>> ComputeValueFieldsCache()
		{
			return this.fieldsCache
				.ToDictionary
				(
					item => item.Key,
					item => item.Value.Where (f => f.Relation == FieldRelation.None).AsReadOnlyCollection ()
				)
				.AsReadOnlyDictionary ();
		}


		public ReadOnlyCollection<StructuredTypeField> GetReferenceFields(Druid entityTypeId)
		{
			return EntityTypeEngine.GetFromCache (this.referenceFieldsCache, entityTypeId);
		}


		private ReadOnlyDictionary<Druid, ReadOnlyCollection<StructuredTypeField>> ComputeReferenceFieldsCache()
		{
			return this.fieldsCache
				.ToDictionary
				(
					item => item.Key,
					item => item.Value.Where (f => f.Relation == FieldRelation.Reference).AsReadOnlyCollection ()
				)
				.AsReadOnlyDictionary ();
		}


		public ReadOnlyCollection<StructuredTypeField> GetCollectionFields(Druid entityTypeId)
		{
			return EntityTypeEngine.GetFromCache (this.collectionFieldsCache, entityTypeId);
		}


		private ReadOnlyDictionary<Druid, ReadOnlyCollection<StructuredTypeField>> ComputeCollectionFieldsCache()
		{
			return this.fieldsCache
				.ToDictionary
				(
					item => item.Key,
					item => item.Value.Where (f => f.Relation == FieldRelation.Collection).AsReadOnlyCollection ()
				)
				.AsReadOnlyDictionary ();
		}


		public ReadOnlyCollection<StructuredTypeField> GetLocalFields(Druid entityTypeId)
		{
			return EntityTypeEngine.GetFromCache (this.localFieldsCache, entityTypeId);
		}


		private ReadOnlyDictionary<Druid, ReadOnlyCollection<StructuredTypeField>> ComputeLocalFieldsCache()
		{
			return this.fieldsCache
				   .ToDictionary
				   (
					   item => item.Key,
					   item => item.Value.Where (f => f.Membership != FieldMembership.Inherited).AsReadOnlyCollection ()
				   )
				   .AsReadOnlyDictionary ();
		}


		public ReadOnlyCollection<StructuredTypeField> GetLocalValueFields(Druid entityTypeId)
		{
			return EntityTypeEngine.GetFromCache (this.localValueFieldsCache, entityTypeId);
		}


		private ReadOnlyDictionary<Druid, ReadOnlyCollection<StructuredTypeField>> ComputeLocalValueFieldsCache()
		{
			return this.localFieldsCache
				.ToDictionary
				(
					item => item.Key,
					item => item.Value.Where (f => f.Relation == FieldRelation.None).AsReadOnlyCollection ()
				)
				.AsReadOnlyDictionary ();
		}


		public ReadOnlyCollection<StructuredTypeField> GetLocalReferenceFields(Druid entityTypeId)
		{
			return EntityTypeEngine.GetFromCache (this.localReferenceFieldsCache, entityTypeId);
		}


		private ReadOnlyDictionary<Druid, ReadOnlyCollection<StructuredTypeField>> ComputeLocalReferenceFieldsCache()
		{
			return this.localFieldsCache
				.ToDictionary
				(
					item => item.Key,
					item => item.Value.Where (f => f.Relation == FieldRelation.Reference).AsReadOnlyCollection ()
				)
				.AsReadOnlyDictionary ();
		}


		public ReadOnlyCollection<StructuredTypeField> GetLocalCollectionFields(Druid entityTypeId)
		{
			return EntityTypeEngine.GetFromCache (this.localCollectionFieldsCache, entityTypeId);
		}


		private ReadOnlyDictionary<Druid, ReadOnlyCollection<StructuredTypeField>> ComputeLocalCollectionFieldsCache()
		{
			return this.localFieldsCache
				.ToDictionary
				(
					item => item.Key,
					item => item.Value.Where (f => f.Relation == FieldRelation.Collection).AsReadOnlyCollection ()
				)
				.AsReadOnlyDictionary ();
		}


		public ReadOnlyDictionary<StructuredType, ReadOnlyCollection<StructuredTypeField>> GetReferencingFields(Druid entityTypeId)
		{
			return EntityTypeEngine.GetFromCache (this.referencingFieldsCache, entityTypeId);
		}


		private ReadOnlyDictionary<Druid, ReadOnlyDictionary<StructuredType, ReadOnlyCollection<StructuredTypeField>>> ComputeReferencingFieldsCache()
		{
			return this.entityTypesCache
				.ToDictionary
				(
					t => t.CaptionId,
					t => this.ComputeReferencingFields (t)
				)
				.AsReadOnlyDictionary ();
		}


		private ReadOnlyDictionary<StructuredType, ReadOnlyCollection<StructuredTypeField>> ComputeReferencingFields(StructuredType type)
		{
			var baseTypeIds = this.baseTypesCache[type.CaptionId]
				.Select (t => t.CaptionId)
				.ToList ();

			return this.localReferenceFieldsCache.Concat (this.localCollectionFieldsCache)
				.SelectMany (item => item.Value.Select (f => new { TId = item.Key, F = f }))
				.Where (item => baseTypeIds.Contains (item.F.TypeId))
				.GroupBy (item => item.TId, item => item.F)
				.ToDictionary (g => this.entityTypeCache[g.Key], g => g.AsReadOnlyCollection ())
				.AsReadOnlyDictionary ();
		}


		private void EnsureReferencedObjectsAreDeserialized()
		{
			foreach (var type in this.GetEntityTypes ())
			{
				var typeCaption = type.Caption;
				
				// The call to type.Fields ensures that the cache for the fields are built.
				// Marc

				foreach (var field in type.Fields.Values)
				{
					AbstractType fieldType = field.Type as AbstractType;

					if (fieldType != null)
					{
						var fieldTypeCaption = fieldType.Caption;
					}
				}
			}
		}


		public static IEnumerable<Druid> GetRelatedEntityTypeIds(IEnumerable<Druid> entityTypeIds)
		{
			entityTypeIds.ThrowIfNull ("entityTypeIds");

			var entityTypes = EntityTypeEngine.ComputeEntityTypes (entityTypeIds);
			var relatedEntityTypes = new HashSet<StructuredType> ();

			foreach (StructuredType entityType in entityTypes)
			{
				EntityTypeEngine.AddRelatedEntityTypes (relatedEntityTypes, entityType);
			}

			return relatedEntityTypes.Select (t => t.CaptionId);
		}


		private static void AddRelatedEntityTypes(ISet<StructuredType> types, StructuredType type)
		{
			if (types.Add (type))
			{
				var baseType = type.BaseType;

				if (baseType != null)
				{
					EntityTypeEngine.AddRelatedEntityTypes (types, type.BaseType);
				}

				var localRelationFields = type.Fields.Values
					.Where (f => f.Source == FieldSource.Value)
					.Where (f => f.Membership != FieldMembership.Inherited)
					.Where (f => f.Relation != FieldRelation.None)
					.ToList ();

				foreach (StructuredTypeField field in localRelationFields)
				{
					EntityTypeEngine.AddRelatedEntityTypes (types, (StructuredType) field.Type);
				}
			}
		}


		private static ReadOnlyCollection<StructuredType> ComputeEntityTypes(IEnumerable<Druid> entityTypeIds)
		{
			return entityTypeIds
				.Distinct ()
				.Select (id => EntityTypeEngine.ComputeEntityType (id))
				.AsReadOnlyCollection ();
		}


		private static StructuredType ComputeEntityType(Druid entityTypeId)
		{
			ResourceManager resourceManager = Epsitec.Common.Support.Resources.DefaultManager;

			StructuredType entityType = resourceManager.GetStructuredType (entityTypeId);

			if (entityType == null)
			{
				throw new System.ArgumentException ("The structured type " + entityTypeId + " does not exists.");
			}

			return entityType;
		}


		private static TValue GetFromCache<TKey, TValue>(IDictionary<TKey, TValue> cache, TKey key)
		{
			TValue value;

			if (!cache.TryGetValue (key, out value))
			{
				throw new System.ArgumentException ("Element not found!");
			}

			return value;
		}


		private readonly ReadOnlyCollection<StructuredType> entityTypesCache;


		private readonly ReadOnlyDictionary<Druid, StructuredType> entityTypeCache;


		private readonly ReadOnlyDictionary<Druid, ReadOnlyCollection<StructuredType>> baseTypesCache;


		private readonly ReadOnlyDictionary<Druid, StructuredType> rootTypeCache;


		private readonly ReadOnlyDictionary<Druid, ReadOnlyCollection<StructuredTypeField>> fieldsCache;


		private readonly ReadOnlyDictionary<Tuple<Druid, Druid>, StructuredTypeField> fieldCache;


		private readonly ReadOnlyDictionary<Tuple<Druid, Druid>, StructuredType> localTypeCache;


		private readonly ReadOnlyDictionary<Druid, ReadOnlyCollection<StructuredTypeField>> valueFieldsCache;


		private readonly ReadOnlyDictionary<Druid, ReadOnlyCollection<StructuredTypeField>> referenceFieldsCache;


		private readonly ReadOnlyDictionary<Druid, ReadOnlyCollection<StructuredTypeField>> collectionFieldsCache;


		private readonly ReadOnlyDictionary<Druid, ReadOnlyCollection<StructuredTypeField>> localFieldsCache;


		private readonly ReadOnlyDictionary<Druid, ReadOnlyCollection<StructuredTypeField>> localValueFieldsCache;


		private readonly ReadOnlyDictionary<Druid, ReadOnlyCollection<StructuredTypeField>> localReferenceFieldsCache;


		private readonly ReadOnlyDictionary<Druid, ReadOnlyCollection<StructuredTypeField>> localCollectionFieldsCache;


		private readonly ReadOnlyDictionary<Druid, ReadOnlyDictionary<StructuredType, ReadOnlyCollection<StructuredTypeField>>> referencingFieldsCache;
		

	}


}
