﻿using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;


namespace Epsitec.Cresus.DataLayer.ImportExport
{
	

	// TODO Comment this class.
	// Marc


	// HACK The methods Export(...) and Import(...) have been designed to be used only for development
	// purposes. Therefore, they are unsuitable for production use. Problems and limitation include
	// - All the entities that are exported and their direct children are loaded in the DataContext
	//   at the same time, which might require a lot of memory and slow everything for a high number
	//   of exported entities.
	// - The xml file is completely loaded in memory, which might be problematic for large files.
	// - No checks are made so ensure that the schema of the exported data is compatible with the
	//   importing database and that the data in the xml file is compatible to the schema in the xml
	//   file.
	// - All the problems that I might not thought about.
	// Marc


	internal static class ImportExportManager
	{


		public static void Export(FileInfo file, DataContext dataContext, IEnumerable<AbstractEntity> entities, System.Func<AbstractEntity, bool> predicate)
		{
			var result = ImportExportManager.GetEntities (entities, predicate);

			ISet<AbstractEntity> exportableEntities = result.Item1;
			ISet<AbstractEntity> externalEntities = result.Item2;

			XDocument xDocument = XmlEntitySerializer.Serialize (dataContext, exportableEntities, externalEntities);

			xDocument.Save (file.FullName);
		}


		private static System.Tuple<ISet<AbstractEntity>, ISet<AbstractEntity>> GetEntities(IEnumerable<AbstractEntity> entities, System.Func<AbstractEntity, bool> predicate)
		{
			Stack<AbstractEntity> entitiesToProcess = new Stack<AbstractEntity> ();
			ISet<AbstractEntity> exportableEntities = new HashSet<AbstractEntity> ();
			ISet<AbstractEntity> externalEntities = new HashSet<AbstractEntity> ();

			entitiesToProcess.PushRange (entities);

			while (entitiesToProcess.Any ())
			{
				AbstractEntity e = entitiesToProcess.Pop ();

				if (!exportableEntities.Contains (e) && !externalEntities.Contains (e))
				{

					if (predicate (e))
					{
						exportableEntities.Add (e);

						foreach (AbstractEntity child in ImportExportManager.GetChildren (e))
						{
							entitiesToProcess.Push (child);
						}
					}
					else
					{
						externalEntities.Add (e);
					}
				}
			}

			return System.Tuple.Create (exportableEntities, externalEntities);
		}


		private static IEnumerable<AbstractEntity> GetChildren(AbstractEntity entity)
		{
			EntityContext entityContext = entity.GetEntityContext ();

			Druid entityId = entity.GetEntityStructuredTypeId ();

			var fields = from field in entityContext.GetEntityFieldDefinitions (entityId)
						 where field.Relation == FieldRelation.Reference || field.Relation == FieldRelation.Collection
						 where entityContext.IsFieldDefined (field.Id, entity)
						 select new
						 {
							 Id = field.Id,
							 Cardinality = field.Relation
						 };

			foreach (var field in fields)
			{
				switch (field.Cardinality)
				{
					case FieldRelation.Reference:
					{
						AbstractEntity target = entity.GetField<AbstractEntity> (field.Id);

						if (ImportExportManager.IsExportable (target))
						{
							yield return target;
						}

						break;
					}
					case FieldRelation.Collection:
					{
						foreach (AbstractEntity target in entity.GetFieldCollection<AbstractEntity> (field.Id))
						{
							if (ImportExportManager.IsExportable (target))
							{
								yield return target;
							}
						}

						break;
					}
					default:
						throw new System.NotImplementedException ();
				}
			}
		}


		public static bool IsExportable(AbstractEntity entity)
		{
			return EntityNullReferenceVirtualizer.IsNullEntity (entity) == false;
		}


        public static void Import(FileInfo file, DataInfrastructure dataInfrastructure)
		{
			XDocument xDocument = XDocument.Load (file.FullName);

			XmlEntitySerializer.Deserialize (dataInfrastructure, xDocument);
		}


	}


}
