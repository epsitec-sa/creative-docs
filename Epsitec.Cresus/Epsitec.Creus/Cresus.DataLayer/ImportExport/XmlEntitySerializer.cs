using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;

using System.Collections.Generic;

using System.Linq;

using System.Xml.Linq;


namespace Epsitec.Cresus.DataLayer.ImportExport
{

	
	// TODO Comment this class.
	// Marc


	internal static class XmlEntitySerializer
	{


		public static XDocument Serialize(DataContext dataContext, ISet<AbstractEntity> exportableEntities, ISet<AbstractEntity> externalEntities, ISet<AbstractEntity> discardedEntities)
		{
			var entitiesToIds = XmlEntitySerializer.AssignIdToEntities (exportableEntities, externalEntities);
			var entityDefinitionsToIds = XmlEntitySerializer.AssignIdToEntityDefinitions (exportableEntities);
			var fieldDefinitionsToIds = XmlEntitySerializer.AssignIdToFieldDefinitions (dataContext, exportableEntities);

			XElement xData = new XElement (XName.Get (XmlConstants.DataTag));

			xData.Add (XmlEntitySerializer.SerializeEntityDefinitions (dataContext, entityDefinitionsToIds, fieldDefinitionsToIds));
			xData.Add (XmlEntitySerializer.SerializeExportableEntities (dataContext, entitiesToIds, entityDefinitionsToIds, fieldDefinitionsToIds, exportableEntities, discardedEntities));
			xData.Add (XmlEntitySerializer.SerializeExternalEntities (dataContext, entitiesToIds, externalEntities));

			XDocument xDocument = new XDocument ();

			xDocument.Add (xData);

			return xDocument;
		}


		private static IDictionary<AbstractEntity, int> AssignIdToEntities(ISet<AbstractEntity> entitiesToExport, ISet<AbstractEntity> entitiesNotToExport)
		{
			var entities = entitiesToExport.Concat (entitiesNotToExport);

			return XmlEntitySerializer.AssignId (entities);
		}


		private static IDictionary<Druid, int> AssignIdToEntityDefinitions(ISet<AbstractEntity> exportableEntities)
		{
			var entityDefinitions = exportableEntities
				.Select (e => e.GetEntityStructuredTypeId ());

			return XmlEntitySerializer.AssignId (entityDefinitions);
		}


		private static IDictionary<Druid, int> AssignIdToFieldDefinitions(DataContext dataContext, ISet<AbstractEntity> exportableEntities)
		{
			var fields = exportableEntities
				.Select (e => e.GetEntityStructuredTypeId ())
				.Distinct ()
				.SelectMany (d => dataContext.DataInfrastructure.EntityEngine.EntityTypeEngine.GetFields (d))
				.Select (d => d.CaptionId)
				.Distinct ();

			return XmlEntitySerializer.AssignId (fields);
		}


		private static IDictionary<T, int> AssignId<T>(IEnumerable<T> items)
		{
			IDictionary<T, int> itemsToIds = new Dictionary<T, int> ();
			
			int id = 0;

			foreach (T item in items)
			{
				itemsToIds[item] = id;

				id++;
			}

			return itemsToIds;
		}


		private static XElement SerializeEntityDefinitions(DataContext dataContext, IDictionary<Druid, int> entityDefinitionsToIds, IDictionary<Druid, int> fieldDefinitionsToIds)
		{
			XElement xDefinition = new XElement (XName.Get (XmlConstants.EntityDefinitionsTag));

			foreach (Druid entityDruid in entityDefinitionsToIds.Keys)
			{
				xDefinition.Add (XmlEntitySerializer.SerializeEntityDefinition (dataContext, entityDefinitionsToIds, fieldDefinitionsToIds, entityDruid));
			}

			return xDefinition;
		}


		private static XElement SerializeEntityDefinition(DataContext dataContext, IDictionary<Druid, int> entityDefinitionsToIds, IDictionary<Druid, int> fieldDefinitionsToIds, Druid entityDruid)
		{
			XElement xEntity = new XElement (XName.Get (XmlConstants.EntityTag));

			string eId = InvariantConverter.ConvertToString (entityDefinitionsToIds[entityDruid]);
			string eName = DbContext.Current.ResourceManager.GetCaption (entityDruid).Name;
			string eDruid = entityDruid.ToResourceId ();

			xEntity.SetAttributeValue (XName.Get (XmlConstants.NameTag), eName);
			xEntity.SetAttributeValue (XName.Get (XmlConstants.DefinitionIdTag), eId);
			xEntity.SetAttributeValue (XName.Get (XmlConstants.DruidTag), eDruid);

			foreach (var field in dataContext.DataInfrastructure.EntityEngine.EntityTypeEngine.GetFields (entityDruid))
			{
				Druid fieldDruid = field.CaptionId;
				int fieldId = fieldDefinitionsToIds[fieldDruid];

				XElement xField = new XElement (XName.Get (XmlConstants.FieldTag));

				string fId = InvariantConverter.ConvertToString (fieldId);
				string fName = DbContext.Current.ResourceManager.GetCaption (fieldDruid).Name;
				string fDruid = fieldDruid.ToResourceId ();

				xField.SetAttributeValue (XName.Get (XmlConstants.NameTag), fName);
				xField.SetAttributeValue (XName.Get (XmlConstants.DefinitionIdTag), fId);
				xField.SetAttributeValue (XName.Get (XmlConstants.DruidTag), fDruid);	

				xEntity.Add (xField);
			}

			return xEntity;
		}


		private static XElement SerializeExportableEntities(DataContext dataContext, IDictionary<AbstractEntity, int> entitiesToIds, IDictionary<Druid, int> entityDefinitionsToIds, IDictionary<Druid, int> fieldDefinitionsToIds, ISet<AbstractEntity> entitiesToExport, ISet<AbstractEntity> discardedEntities)
		{
			XElement xEntities = new XElement (XName.Get (XmlConstants.ExportedEntitiesTag));

			foreach (AbstractEntity entity in entitiesToExport)
			{
				XElement xEntity = XmlEntitySerializer.SerializeEntity (dataContext, discardedEntities, entitiesToIds, entityDefinitionsToIds, fieldDefinitionsToIds, entity);

				xEntities.Add (xEntity);
			}

			return xEntities;
		}


		private static XElement SerializeEntity(DataContext dataContext, ISet<AbstractEntity> discardedEntities, IDictionary<AbstractEntity, int> entitiesToIds, IDictionary<Druid, int> entityDefinitionsToIds, IDictionary<Druid, int> fieldDefinitionsToIds, AbstractEntity entity)
		{
			XElement xEntity = XmlEntitySerializer.CreateXElementForEntity (entitiesToIds, entityDefinitionsToIds, entity);

			foreach (StructuredTypeField field in dataContext.DataInfrastructure.EntityEngine.EntityTypeEngine.GetFields (entity.GetEntityStructuredTypeId ()))
			{
				XElement xField = XmlEntitySerializer.SerializeEntityField(discardedEntities, entitiesToIds, fieldDefinitionsToIds, entity, field);

				if (xField != null)
				{
					xEntity.Add (xField);
				}
			}

			return xEntity;
		}


		private static XElement CreateXElementForEntity(IDictionary<AbstractEntity, int> entitiesToIds, IDictionary<Druid, int> entityDefinitionsToIds, AbstractEntity entity)
		{
			Druid entityDruid = entity.GetEntityStructuredTypeId ();
			
			XElement xEntity = new XElement (XName.Get (XmlConstants.EntityTag));

			string id = InvariantConverter.ConvertToString<int> (entitiesToIds[entity]);
			string definitionId = InvariantConverter.ConvertToString (entityDefinitionsToIds[entity.GetEntityStructuredTypeId ()]);
			string name = DbContext.Current.ResourceManager.GetCaption (entityDruid).Name;

			xEntity.SetAttributeValue (XName.Get (XmlConstants.NameTag), name);
			xEntity.SetAttributeValue (XName.Get (XmlConstants.EntityIdTag), id);
			xEntity.SetAttributeValue (XName.Get (XmlConstants.DefinitionIdTag), definitionId);

			return xEntity;
		}


		private static XElement SerializeEntityField(ISet<AbstractEntity> discardedEntities, IDictionary<AbstractEntity, int> entitiesToIds, IDictionary<Druid, int> fieldDefinitionsToIds, AbstractEntity entity, StructuredTypeField field)
		{
			XElement xField;
			
			switch (field.Relation)
			{
				case FieldRelation.None:
					xField = XmlEntitySerializer.SerializeEntityValueField (fieldDefinitionsToIds, entity, field);
					break;

				case FieldRelation.Reference:
					xField = XmlEntitySerializer.SerializeEntityReferenceField(discardedEntities, entitiesToIds, fieldDefinitionsToIds, entity, field);
					break;

				case FieldRelation.Collection:
					xField = XmlEntitySerializer.SerializeEntityCollectionField(discardedEntities, entitiesToIds, fieldDefinitionsToIds, entity, field);
					break;

				default:
					throw new System.NotImplementedException ();
			}

			return xField;
		}

		private static XElement SerializeEntityValueField(IDictionary<Druid, int> fieldDefinitionsToIds, AbstractEntity entity, StructuredTypeField field)
		{
			object value = entity.GetField<object> (field.Id);

			XElement xField;

			if (value == null)
			{
				xField = null;
			}
			else
			{
				System.Type systemType = field.Type.SystemType;

				ISerializationConverter converter = InvariantConverter.GetSerializationConverter (systemType);

				string fValue = converter.ConvertToString (value, null);

				xField = XmlEntitySerializer.CreateXElementForField (fieldDefinitionsToIds, field, fValue);
			}

			return xField;
		}


		private static XElement SerializeEntityReferenceField(ISet<AbstractEntity> discardedEntities, IDictionary<AbstractEntity, int> entitiesToIds, IDictionary<Druid, int> fieldDefinitionsToIds, AbstractEntity entity, StructuredTypeField field)
		{
			AbstractEntity target = entity.GetField<AbstractEntity> (field.Id);

			XElement xField;

			if (target == null || discardedEntities.Contains (target))
			{
				xField = null;
			}
			else
			{
				string fValue = InvariantConverter.ConvertToString (entitiesToIds[target]);

				xField = XmlEntitySerializer.CreateXElementForField (fieldDefinitionsToIds, field, fValue);
			}

			return xField;
		}


		private static XElement SerializeEntityCollectionField(ISet<AbstractEntity> discardedEntities, IDictionary<AbstractEntity, int> entitiesToIds, IDictionary<Druid, int> fieldDefinitionsToIds, AbstractEntity entity, StructuredTypeField field)
		{
			IList<AbstractEntity> targets = entity.GetFieldCollection<AbstractEntity> (field.Id)
				.Where (t => t != null && !discardedEntities.Contains (t))
				.ToList ();

			XElement xField;

			if (targets.Any ())
			{
				IEnumerable<string> targetIds = targets
					.Select (t => entitiesToIds[t])
					.Select (t => InvariantConverter.ConvertToString (t));

				string fValue = string.Join (",", targetIds);

				xField = XmlEntitySerializer.CreateXElementForField (fieldDefinitionsToIds, field, fValue);
			}
			else
			{
				xField = null;
			}

			return xField;
		}


		private static XElement CreateXElementForField(IDictionary<Druid, int> fieldDefinitionsToIds, StructuredTypeField field, string value)
		{
			XElement xField = new XElement (XName.Get (XmlConstants.FieldTag));

			string definitionId = InvariantConverter.ConvertToString (fieldDefinitionsToIds[field.CaptionId]);
			string name = DbContext.Current.ResourceManager.GetCaption (field.CaptionId).Name;

			xField.SetAttributeValue (XName.Get (XmlConstants.NameTag), name);
			xField.SetAttributeValue (XmlConstants.DefinitionIdTag, definitionId);

			//	Detect the special case of an XML BLOB, encoded as a "U"-prefixed string by the
			//	automatic type converter; since we cannot be sure that the data is indeed valid
			//	XML, we have no other choice than to try to parse it:
			
			if ((value != null) &&
				(value.StartsWith ("U<")) &&
				(value.EndsWith (">")))
			{
				try
				{
					var xml = XElement.Parse (value.Substring (1));

					//	Don't store the 'value' attribute, but instead store the prefix in an 'xml'
					//	attribute and the data itself as a sub-tree:

					xField.SetAttributeValue (XmlConstants.XmlTag, "U");
					xField.Add (xml);

					return xField;
				}
				catch
				{
					//	If the value is not valid XML, never mind, simply store the data as a
					//	text in the 'value' attribute.
				}
			}
			
				
			xField.SetAttributeValue (XmlConstants.ValueTag, value);

			return xField;
		}


		private static XElement SerializeExternalEntities(DataContext dataContext, IDictionary<AbstractEntity, int> entitiesToIds, ISet<AbstractEntity> externalEntities)
		{
			XElement xExternal = new XElement (XName.Get (XmlConstants.ExternalEntitiesTag));

			foreach (AbstractEntity externalEntity in externalEntities)
			{
				Druid entityDruid = externalEntity.GetEntityStructuredTypeId ();

				string id = InvariantConverter.ConvertToString (entitiesToIds[externalEntity]);
				string key = dataContext.GetNormalizedEntityKey (externalEntity).Value.ToString ();
				string name = DbContext.Current.ResourceManager.GetCaption (entityDruid).Name;
				
				XElement xEntity = new XElement (XName.Get (XmlConstants.EntityTag));

				xEntity.SetAttributeValue (XName.Get (XmlConstants.NameTag), name);
				xEntity.SetAttributeValue (XName.Get (XmlConstants.EntityIdTag), id);
				xEntity.SetAttributeValue (XName.Get (XmlConstants.KeyTag), key);

				xExternal.Add (xEntity);
			}

			return xExternal;
		}


		public static void Deserialize(DataInfrastructure dataInfrastructure, XDocument xDocument)
		{
			DataContext dataContext = null;

			try
			{
				dataContext = dataInfrastructure.CreateDataContext ();

				XElement xData = xDocument.Element (XName.Get (XmlConstants.DataTag));
				XElement xEntityDefinitions = xData.Element (XName.Get (XmlConstants.EntityDefinitionsTag));
				XElement xExportedEntities = xData.Element (XName.Get (XmlConstants.ExportedEntitiesTag));
				XElement xExternalEntities = xData.Element (XName.Get (XmlConstants.ExternalEntitiesTag));

				var idsToEntityDefinitions = XmlEntitySerializer.DeserializeIdsToEntityDefinitions (dataContext, xEntityDefinitions);
				var idsToFieldDefinitions = XmlEntitySerializer.DeserializeIdsToFieldDefinitions (dataContext, xEntityDefinitions);
				var idsToEntities =  new Dictionary<int, AbstractEntity> ();

				foreach (var item in XmlEntitySerializer.DeserializeIdsToExportedEntities (dataContext, idsToEntityDefinitions, xExportedEntities))
				{
					idsToEntities[item.Key] = item.Value;
				}

				foreach (var item in XmlEntitySerializer.DeserializeIdToExternalEntities (dataContext, xExternalEntities))
				{
					idsToEntities[item.Key] = item.Value;
				}

				XmlEntitySerializer.DeserializeExportedEntitiesFields (idsToEntities, idsToFieldDefinitions, xExportedEntities);

				dataContext.SaveChanges ();
			}
			finally
			{
				if (dataContext != null)
				{
					dataInfrastructure.DeleteDataContext (dataContext);
				}
			}
		}


		private static IDictionary<int, StructuredType> DeserializeIdsToEntityDefinitions(DataContext dataContext, XElement xEntityDefinitions)
		{
			IDictionary<int, StructuredType> idsToEntityDefinitions = new Dictionary<int, StructuredType> ();

			foreach (XElement xEntity in xEntityDefinitions.Descendants (XName.Get (XmlConstants.EntityTag)))
			{
				string eId = xEntity.Attribute (XName.Get (XmlConstants.DefinitionIdTag)).Value;
				string eDruid = xEntity.Attribute (XName.Get (XmlConstants.DruidTag)).Value;

				int entityId = InvariantConverter.ConvertFromString<int> (eId);
				Druid entityDruid = Druid.Parse (eDruid);

				StructuredType entityDefinition = dataContext.DataInfrastructure.EntityEngine.EntityTypeEngine.GetEntityType (entityDruid);

				idsToEntityDefinitions[entityId] = entityDefinition;
			}

			return idsToEntityDefinitions;
		}


		private static IDictionary<int, StructuredTypeField> DeserializeIdsToFieldDefinitions(DataContext dataContext, XElement xEntityDefinitions)
		{
			var idsToFieldDefinitions = new Dictionary<int, StructuredTypeField> ();

			foreach (XElement xEntity in xEntityDefinitions.Elements (XName.Get (XmlConstants.EntityTag)))
			{
				string eDruid = xEntity.Attribute (XName.Get (XmlConstants.DruidTag)).Value;
				Druid entityDruid = Druid.Parse (eDruid);

				foreach (XElement xField in xEntity.Elements (XName.Get (XmlConstants.FieldTag)))
				{
					string fId = xField.Attribute (XName.Get (XmlConstants.DefinitionIdTag)).Value;
					string fDruid = xField.Attribute (XName.Get (XmlConstants.DruidTag)).Value;

					int fieldId = InvariantConverter.ConvertFromString<int> (fId);
					Druid fieldDruid = Druid.Parse (fDruid);

					StructuredTypeField fieldDefinition = dataContext.DataInfrastructure.EntityEngine.EntityTypeEngine.GetField (entityDruid, fieldDruid);

					idsToFieldDefinitions[fieldId] = fieldDefinition;
				}
			}

			return idsToFieldDefinitions;
		}


		private static IDictionary<int, AbstractEntity> DeserializeIdsToExportedEntities(DataContext dataContext, IDictionary<int, StructuredType> idsToEntityDefinitions, XElement xEntities)
		{
			Dictionary<int, AbstractEntity> idsToEntities = new Dictionary<int, AbstractEntity> ();

			foreach (XElement xEntity in xEntities.Elements (XName.Get (XmlConstants.EntityTag)))
			{
				string eId = xEntity.Attribute (XName.Get (XmlConstants.EntityIdTag)).Value;
				string eDefinitionId = xEntity.Attribute (XName.Get (XmlConstants.DefinitionIdTag)).Value;

				int entityId = InvariantConverter.ConvertFromString<int> (eId);
				int entityDefinitionId = InvariantConverter.ConvertFromString<int> (eDefinitionId);

				Druid entityDruid = idsToEntityDefinitions[entityDefinitionId].CaptionId;

				AbstractEntity entity = dataContext.CreateEntity (entityDruid);

				idsToEntities[entityId] = entity;
			}

			return idsToEntities;
		}


		private static IDictionary<int, AbstractEntity> DeserializeIdToExternalEntities(DataContext dataContext, XElement xExternalEntities)
		{
			Dictionary<int, AbstractEntity> idsToEntities = new Dictionary<int, AbstractEntity> ();

			foreach (XElement xExternalEntity in xExternalEntities.Elements (XName.Get (XmlConstants.EntityTag)))
			{
				string eId = xExternalEntity.Attribute (XName.Get (XmlConstants.EntityIdTag)).Value;
				string eKey = xExternalEntity.Attribute (XName.Get (XmlConstants.KeyTag)).Value;

				int entityId = InvariantConverter.ConvertFromString<int> (eId);
				EntityKey entityKey = EntityKey.Parse (eKey).Value;

				AbstractEntity entity = dataContext.ResolveEntity (entityKey.EntityId, entityKey.RowKey);

				idsToEntities[entityId] = entity;
			}

			return idsToEntities;
		}


		private static void DeserializeExportedEntitiesFields(IDictionary<int, AbstractEntity> idsToEntities, IDictionary<int, StructuredTypeField> idsToFieldDefinitions, XElement xEntities)
		{
			foreach (XElement xEntity in xEntities.Elements (XName.Get (XmlConstants.EntityTag)))
			{
				XmlEntitySerializer.DeserializeExportedEntityFields (idsToEntities, idsToFieldDefinitions, xEntity);
			}
		}


		private static void DeserializeExportedEntityFields(IDictionary<int, AbstractEntity> idsToEntities, IDictionary<int, StructuredTypeField> idsToFieldDefinitions, XElement xEntity)
		{
			string eId = xEntity.Attribute (XName.Get (XmlConstants.EntityIdTag)).Value;
			int entityId = InvariantConverter.ConvertFromString<int> (eId);

			AbstractEntity entity = idsToEntities[entityId];

			foreach (XElement xField in xEntity.Elements (XName.Get (XmlConstants.FieldTag)))
			{
				XmlEntitySerializer.DeserializeExportedEntityField (idsToEntities, idsToFieldDefinitions, entity, xField);
			}
		}


		private static void DeserializeExportedEntityField(IDictionary<int, AbstractEntity> idsToEntities, IDictionary<int, StructuredTypeField> idsToFieldDefinitions, AbstractEntity entity, XElement xField)
		{
			string fDefintionId = xField.Attribute (XName.Get (XmlConstants.DefinitionIdTag)).Value;
			int fieldDefinitionId = InvariantConverter.ConvertFromString<int> (fDefintionId);
			StructuredTypeField fieldDefinition = idsToFieldDefinitions[fieldDefinitionId];

			//	The value is either stored as an attribute (value="x") or as an XML sub-tree:

			string fValue = (string) xField.Attribute (XName.Get (XmlConstants.ValueTag));
			string fXml   = fValue == null ? (string) xField.Attribute (XName.Get (XmlConstants.XmlTag)) : null;

			if (fXml != null)
			{
				fValue = fXml + xField.FirstNode.ToString ();
			}

			switch (fieldDefinition.Relation)
			{
				case FieldRelation.None:
					XmlEntitySerializer.DeserializeExportedEntityValueField (entity, fieldDefinition, fValue);
					break;

				case FieldRelation.Reference:
					XmlEntitySerializer.DeserializeExportedEntityReferenceField (idsToEntities, entity, fieldDefinition, fValue);
					break;

				case FieldRelation.Collection:
					XmlEntitySerializer.DeserializeExportedEntityCollectionField (idsToEntities, entity, fieldDefinition, fValue);
					break;

				default:
					throw new System.NotImplementedException ();
			}
		}


		private static void DeserializeExportedEntityValueField(AbstractEntity entity, StructuredTypeField fieldDefinition, string fValue)
		{
			System.Type systemType = fieldDefinition.Type.SystemType;

			ISerializationConverter converter = InvariantConverter.GetSerializationConverter (systemType);

			object fieldValue = converter.ConvertFromString (fValue, null);

			entity.SetField<object> (fieldDefinition.CaptionId.ToResourceId (), fieldValue);
		}


		private static void DeserializeExportedEntityReferenceField(IDictionary<int, AbstractEntity> idsToEntities, AbstractEntity entity, StructuredTypeField fieldDefinition, string fValue)
		{
			int fieldValue = InvariantConverter.ConvertFromString<int> (fValue);

			AbstractEntity target = idsToEntities[fieldValue];

			if (target != null)
			{
				entity.SetField<AbstractEntity> (fieldDefinition.CaptionId.ToResourceId (), target);
			}
		}


		private static void DeserializeExportedEntityCollectionField(IDictionary<int, AbstractEntity> idsToEntities, AbstractEntity entity, StructuredTypeField fieldDefinition, string fValue)
		{
			List<int> fieldValue = fValue.Split (',').Select (v => InvariantConverter.ConvertFromString<int> (v)).ToList ();

			IList<AbstractEntity> targets = entity.GetFieldCollection<AbstractEntity> (fieldDefinition.CaptionId.ToResourceId ());

			foreach (AbstractEntity target in fieldValue.Select (t => idsToEntities[t]).Where (t => t != null))
			{
				targets.Add (target);
			}
		}


		private static class XmlConstants
		{

			public static readonly string DataTag = "data";

			public static readonly string EntityDefinitionsTag = "entityDefinitions";

			public static readonly string ExportedEntitiesTag = "exportedEntities";

			public static readonly string ExternalEntitiesTag = "externalEntities";

			public static readonly string EntityTag = "entity";

			public static readonly string FieldTag = "field";

			public static readonly string NameTag = "name";

			public static readonly string EntityIdTag = "entityId";

			public static readonly string DefinitionIdTag = "definitionId";

			public static readonly string DruidTag = "druid";

			public static readonly string ValueTag = "value";

			public static readonly string XmlTag = "xml";

			public static readonly string KeyTag = "key";

		}

	}


}
