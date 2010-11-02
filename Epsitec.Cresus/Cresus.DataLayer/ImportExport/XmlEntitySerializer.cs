using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;

using System.Collections.Generic;

using System.Linq;

using System.Xml.Linq;


namespace Epsitec.Cresus.DataLayer.ImportExport
{

	
	internal static class XmlEntitySerializer
	{


		public static XElement Serialize(DataContext dataContext, ISet<AbstractEntity> entities)
		{
			IDictionary<AbstractEntity, int> entitiesWithIds = XmlEntitySerializer.BuildEntitiesToIds (entities);
			
			XElement xEntities = XmlEntitySerializer.CreateXElement(XmlConstants.EntitiesTag);

			foreach (AbstractEntity entity in entities)
			{
				XElement xEntity = XmlEntitySerializer.SerializeEntity (dataContext, entitiesWithIds, entity);

				xEntities.Add (xEntity);
			}

			return xEntities;
		}


		private static IDictionary<AbstractEntity, int> BuildEntitiesToIds(ISet<AbstractEntity> entities)
		{
			Dictionary<AbstractEntity, int> entitiesToIds = new Dictionary<AbstractEntity, int> ();

			int id = 0;

			foreach (AbstractEntity entity in entities)
			{
				entitiesToIds[entity] = id;

				id++;
			}

			return entitiesToIds;
		}


		private static XElement SerializeEntity(DataContext dataContext, IDictionary<AbstractEntity, int> entitiesToIds, AbstractEntity entity)
		{
			XElement xEntity = XmlEntitySerializer.CreateXElementForEntity (entitiesToIds, entity);

			foreach (XElement xField in XmlEntitySerializer.SerializeEntityFields (dataContext, entitiesToIds, entity).Where (xf => xf != null))
			{
				xEntity.Add (xField);
			}

			return xEntity;
		}


		private static XElement CreateXElementForEntity(IDictionary<AbstractEntity, int> entitiesToIds, AbstractEntity entity)
		{
			XElement xEntity = XmlEntitySerializer.CreateXElement (XmlConstants.EntityTag);

			string idAsString = InvariantConverter.ConvertToString<int> (entitiesToIds[entity]);
			XmlEntitySerializer.CreateAttribute (xEntity, XmlConstants.IdTag, idAsString);

			XmlEntitySerializer.CreateAttribute (xEntity, XmlConstants.DruidTag, entity.GetEntityStructuredTypeId ().ToResourceId ());

			return xEntity;
		}


		private static IEnumerable<XElement> SerializeEntityFields(DataContext dataContext, IDictionary<AbstractEntity, int> entitiesToIds, AbstractEntity entity)
		{
			foreach (StructuredTypeField field in dataContext.EntityContext.GetEntityFieldDefinitions (entity.GetEntityStructuredTypeId ()))
			{
				switch (field.Relation)
				{
					case FieldRelation.None:
						yield return XmlEntitySerializer.SerializeEntityValueField (entity, field);
						break;
					case FieldRelation.Reference:
						yield return XmlEntitySerializer.SerializeEntityReferenceField (dataContext, entitiesToIds, entity, field);
						break;
					case FieldRelation.Collection:
						yield return XmlEntitySerializer.SerializeEntityCollectionField (dataContext, entitiesToIds, entity, field);
						break;
					default:
						throw new System.NotImplementedException ();
				}
			}
		}

		private static XElement SerializeEntityValueField(AbstractEntity entity, StructuredTypeField field)
		{
			object value = entity.GetField<object> (field.Id);

			XElement xField;

			if (value == null)
			{
				xField = null;
			}
			else
			{
				xField = XmlEntitySerializer.CreateXElementForField (field);

				XElement xValue = XmlEntitySerializer.CreateXElement (XmlConstants.ValueTag);

				xField.Add (xValue);

				System.Type systemType = field.Type.SystemType;

				XmlEntitySerializer.CreateAttribute (xValue, XmlConstants.TypeTag, systemType.AssemblyQualifiedName);

				ISerializationConverter converter = InvariantConverter.GetSerializationConverter (systemType);

				string valueAsString = converter.ConvertToString (value, null);

				xValue.Add (valueAsString);
			}

			return xField;
		}


		private static XElement SerializeEntityReferenceField(DataContext dataContext, IDictionary<AbstractEntity, int> entitiesToIds, AbstractEntity entity, StructuredTypeField field)
		{
			AbstractEntity target = entity.GetField<AbstractEntity> (field.Id);

			XElement xField;

			if (target == null)
			{
				xField = null;
			}
			else
			{
				xField = XmlEntitySerializer.CreateXElementForField (field);

				XElement xTarget = XmlEntitySerializer.SerializeRelationTarget (dataContext, entitiesToIds, target);

				xField.Add (xTarget);
			}

			return xField;
		}


		private static XElement SerializeEntityCollectionField(DataContext dataContext, IDictionary<AbstractEntity, int> entitiesToIds, AbstractEntity entity, StructuredTypeField field)
		{
			IList<AbstractEntity> targets = entity.GetFieldCollection<AbstractEntity> (field.Id);

			XElement xField;

			if (targets.Any ())
			{
				xField = XmlEntitySerializer.CreateXElementForField (field);

				foreach (AbstractEntity target in targets)
				{
					XElement xTarget = XmlEntitySerializer.SerializeRelationTarget (dataContext, entitiesToIds, target);

					xField.Add (xTarget);
				}
			}
			else
			{
				xField = null;			
			}

			return xField;
		}


		private static XElement SerializeRelationTarget(DataContext dataContext, IDictionary<AbstractEntity, int> entitiesToIds, AbstractEntity target)
		{
			XElement xTarget = XmlEntitySerializer.CreateXElement (XmlConstants.TargetTag);

			string targetExportation;
			string targetValue;

			if (entitiesToIds.ContainsKey (target))
			{
				targetExportation = "exported";
				targetValue = InvariantConverter.ConvertToString (entitiesToIds[target]);
			}
			else
			{
				targetExportation = "unexported";

				EntityKey targetKey = dataContext.GetNormalizedEntityKey (target).Value;

				targetValue = targetKey.ToString ();
			}

			XmlEntitySerializer.CreateAttribute (xTarget, XmlConstants.TargetExporationTag, targetExportation);

			xTarget.Add (targetValue);

			return xTarget;
		}


		private static XElement CreateXElementForField(StructuredTypeField field)
		{
			XElement xField = XmlEntitySerializer.CreateXElement (XmlConstants.FieldTag);

			XmlEntitySerializer.CreateAttribute (xField, XmlConstants.DruidTag, field.CaptionId.ToResourceId ());

			XmlEntitySerializer.CreateAttribute (xField, XmlConstants.CardinalityTag, field.Relation.ToString ());

			return xField;
		}
					
		
		public static void Deserialize(DataInfrastructure dataInfrastructure, XElement xEntities)
		{
			DataContext dataContext = null;

			try
			{
				dataContext = dataInfrastructure.CreateDataContext ();

				IDictionary<int, AbstractEntity> idsToEntities = XmlEntitySerializer.BuildEmptyEntities (dataContext, xEntities);

				XmlEntitySerializer.PopulateEntities (dataContext, xEntities, idsToEntities);

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


		private static IDictionary<int, AbstractEntity> BuildEmptyEntities(DataContext dataContext, XElement xEntities)
		{
			Dictionary<int, AbstractEntity> idsToEntities = new Dictionary<int, AbstractEntity> ();

			foreach (XElement xEntity in xEntities.Elements (XmlEntitySerializer.CreateXName (XmlConstants.EntityTag)))
			{
				string idAsString = xEntity.Attribute (XmlEntitySerializer.CreateXName (XmlConstants.IdTag)).Value;
				string typeIdAsString = xEntity.Attribute (XmlEntitySerializer.CreateXName (XmlConstants.DruidTag)).Value;

				int id = InvariantConverter.ConvertFromString<int> (idAsString);
				Druid typeId = Druid.Parse (typeIdAsString);

				AbstractEntity entity = dataContext.CreateEntity (typeId);

				idsToEntities[id] = entity;
			}

			return idsToEntities;
		}


		public static void PopulateEntities(DataContext dataContext, XElement xEntities, IDictionary<int, AbstractEntity> idsToEntities)
		{
			foreach (XElement xEntity in xEntities.Elements (XmlEntitySerializer.CreateXName (XmlConstants.EntityTag)))
			{
				XmlEntitySerializer.PopulateEntity (dataContext, xEntity, idsToEntities);
			}
		}


		private static void PopulateEntity(DataContext dataContext, XElement xEntity, IDictionary<int, AbstractEntity> idsToEntities)
		{
			string idAsString = xEntity.Attribute (XmlEntitySerializer.CreateXName (XmlConstants.IdTag)).Value;
			int id = InvariantConverter.ConvertFromString<int> (idAsString);

			AbstractEntity entity = idsToEntities[id];

			foreach (XElement xField in xEntity.Elements (XmlEntitySerializer.CreateXName (XmlConstants.FieldTag)))
			{
				XmlEntitySerializer.PopulateField (dataContext, xField, idsToEntities, entity);
			}
		}


		private static void PopulateField(DataContext dataContext, XElement xField, IDictionary<int, AbstractEntity> idsToEntities, AbstractEntity entity)
		{
			string cardinalityAsString = xField.Attribute (XmlEntitySerializer.CreateXName (XmlConstants.CardinalityTag)).Value;
			FieldRelation cardinality = (FieldRelation) System.Enum.Parse (typeof (FieldRelation), cardinalityAsString);

			switch (cardinality)
			{
				case FieldRelation.None:
					XmlEntitySerializer.PopulateValueField (xField, entity);
					break;
				case FieldRelation.Reference:
					XmlEntitySerializer.PopulateReferenceField (dataContext, xField, idsToEntities, entity);
					break;
				case FieldRelation.Collection:
					XmlEntitySerializer.PopulateCollectionField (dataContext, xField, idsToEntities, entity);
					break;
				default:
					throw new System.NotImplementedException ();
			}
		}


		private static void PopulateValueField(XElement xField, AbstractEntity entity)
		{
			string fieldIdAsString = xField.Attribute (XmlEntitySerializer.CreateXName (XmlConstants.DruidTag)).Value;
			Druid fieldId = Druid.Parse (fieldIdAsString);

			XElement xValue = xField.Element (XmlEntitySerializer.CreateXName (XmlConstants.ValueTag));

			string systemTypeAsString = xValue.Attribute (XmlEntitySerializer.CreateXName (XmlConstants.TypeTag)).Value;
			string valueAsString = xValue.Value;

			System.Type systemType = System.Type.GetType (systemTypeAsString);

			ISerializationConverter converter = InvariantConverter.GetSerializationConverter (systemType);

			object value = converter.ConvertFromString (valueAsString, null);

			entity.SetField<object> (fieldId.ToResourceId (), value);
		}


		private static void PopulateReferenceField(DataContext dataContext, XElement xField, IDictionary<int, AbstractEntity> idsToEntities, AbstractEntity entity)
		{
			string fieldIdAsString = xField.Attribute (XmlEntitySerializer.CreateXName (XmlConstants.DruidTag)).Value;
			Druid fieldId = Druid.Parse (fieldIdAsString);

			XElement xTarget = xField.Element (XmlEntitySerializer.CreateXName (XmlConstants.TargetTag));

			AbstractEntity target = XmlEntitySerializer.GetTarget (dataContext, xTarget, idsToEntities);

			if (target != null)
			{
				entity.SetField<AbstractEntity> (fieldId.ToResourceId (), target);
			}
		}


		private static void PopulateCollectionField(DataContext dataContext, XElement xField, IDictionary<int, AbstractEntity> idsToEntities, AbstractEntity entity)
		{
			string fieldIdAsString = xField.Attribute (XmlEntitySerializer.CreateXName (XmlConstants.DruidTag)).Value;
			Druid fieldId = Druid.Parse (fieldIdAsString);

			IList<AbstractEntity> targets = entity.GetFieldCollection<AbstractEntity> (fieldId.ToResourceId ());

			foreach (XElement xTarget in xField.Elements (XmlEntitySerializer.CreateXName (XmlConstants.TargetTag)))
			{
				AbstractEntity target = XmlEntitySerializer.GetTarget (dataContext, xTarget, idsToEntities);

				if (target != null)
				{
					targets.Add (target);
				}
			}
		}


		private static AbstractEntity GetTarget(DataContext dataContext, XElement xTarget, IDictionary<int, AbstractEntity> idsToEntities)
		{
			string targetExportation = xTarget.Attribute (XmlEntitySerializer.CreateXName (XmlConstants.TargetExporationTag)).Value;
			string targetValue = xTarget.Value;

			AbstractEntity target;

			switch (targetExportation)
			{
				case "exported":

					int targetId = InvariantConverter.ConvertFromString<int> (targetValue);

					target = idsToEntities[targetId];

					break;

				case "unexported":

					EntityKey targetKey = EntityKey.Parse (targetValue).Value;

					target = dataContext.ResolveEntity (targetKey.EntityId, targetKey.RowKey);

					break;

				default:
					throw new System.NotImplementedException ();
			}

			return target;
		}


		private static XElement CreateXElement(string name)
		{
			return new XElement (XmlEntitySerializer.CreateXName (name));
		}


		private static void CreateAttribute(XElement xElement, string name, string value)
		{
			xElement.SetAttributeValue (XmlEntitySerializer.CreateXName (name), value);
		}


		private static XName CreateXName(string name)
		{
			return XName.Get (name, XmlConstants.Namespace);
		}


		private static class XmlConstants
		{
			public static readonly string Namespace = "cresus";

			public static readonly string EntitiesTag = "entities";

			public static readonly string EntityTag = "entity";

			public static readonly string IdTag = "id";

			public static readonly string FieldTag = "field";

			public static readonly string DruidTag = "druid";

			public static readonly string CardinalityTag = "cardinality";

			public static readonly string ValueTag = "value";

			public static readonly string TypeTag = "type";

			public static readonly string TargetTag = "target";

			public static readonly string TargetExporationTag = "exportation";

		}

	}


}
