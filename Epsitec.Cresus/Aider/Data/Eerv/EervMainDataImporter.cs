using Epsitec.Aider.Entities;
using Epsitec.Aider.Tools;

using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{


	internal static class EervMainDataImporter
	{


		public static void Import(CoreDataManager coreDataManager, EervMainData eervData, ParishAddressRepository parishRepository)
		{
			EervMainDataImporter.ImportGroupDefinitions (coreDataManager, eervData);

			EervMainDataImporter.ImportGlobalGroups (coreDataManager, eervData);
			EervMainDataImporter.ImportRegionAndParishGroups (coreDataManager, parishRepository);

			coreDataManager.CoreData.ResetIndexes ();
		}


		private static void ImportGroupDefinitions(CoreDataManager coreDataManager, EervMainData eervData)
		{
			coreDataManager.Execute (b => EervMainDataImporter.ImportGroupDefinitions (b, eervData));
		}


		private static void ImportGroupDefinitions(BusinessContext businessContext, EervMainData eervData)
		{
			var functionRoot = eervData.GroupDefinitions.First (g => g.GroupClassification == Enumerations.GroupClassification.Function && g.GroupLevel == 1);
			EervMainDataImporter.ImportGroupDefinition (businessContext, functionRoot);

			var topLevelGroups = eervData.GroupDefinitions.Where (g => g.GroupLevel == 0);

			foreach (var groupDefinition in topLevelGroups)
			{
				EervMainDataImporter.ImportGroupDefinition (businessContext, groupDefinition);
			}

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);
		}


		private static AiderGroupDefEntity ImportGroupDefinition(BusinessContext businessContext, EervGroupDefinition groupDefinition)
		{
			var aiderGroupDef = businessContext.CreateAndRegisterEntity<AiderGroupDefEntity> ();

			aiderGroupDef.Name = groupDefinition.Name;
			aiderGroupDef.PathTemplate = groupDefinition.GetPathTemplate ();
			aiderGroupDef.NodeType = groupDefinition.GroupNodeType;
			aiderGroupDef.Classification = groupDefinition.GroupClassification;
			aiderGroupDef.Mutability = Enumerations.Mutability.SystemDefined;

			var children = new List<AiderGroupDefEntity> ();

			if (groupDefinition.FunctionGroup != null)
			{
				var funcGroup = groupDefinition.FunctionGroup;
				var funcGroupEntity = funcGroup.EntityCache;
				children.Add (funcGroupEntity);
			}

			children.AddRange (groupDefinition
				.Children
				.Select (c => EervMainDataImporter.ImportGroupDefinition (businessContext, c)));

			aiderGroupDef.Subgroups.AddRange (children);
			groupDefinition.EntityCache = aiderGroupDef;

			// TODO Add a lot more stuff here to set up properly the group definition level, type,
			// category, etc.

			return aiderGroupDef;
		}


		private static void ImportGlobalGroups(CoreDataManager coreDataManager, EervMainData eervData)
		{
			coreDataManager.Execute (b => EervMainDataImporter.ImportGlobalGroups (b, eervData));
		}


		private static void ImportGlobalGroups(BusinessContext businessContext, EervMainData eervData)
		{
			var rootAiderGroupDefs =
				from eervGroupDef in eervData.GroupDefinitions
				where eervGroupDef.GroupLevel == 0
				let path = eervGroupDef.GetPathTemplate ()
				where !path.StartsWith (AiderGroupIds.Region)
					&& !path.StartsWith (AiderGroupIds.Parish)
					&& !path.StartsWith (AiderGroupIds.Function)
				let example = new AiderGroupDefEntity ()
				{
					PathTemplate = path
				}
				select businessContext.DataContext.GetByExample (example).Single ();

			foreach (var aiderGroupDef in rootAiderGroupDefs)
			{
				aiderGroupDef.Instantiate (businessContext, new GroupPathInfo
				(
					name: aiderGroupDef.Name,
					template: aiderGroupDef.PathTemplate,
					output: aiderGroupDef.PathTemplate,
					level: 0
				));
			}

			EervMainDataImporter.CreateNoParishGroup (businessContext);

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);
		}

		private static AiderGroupEntity CreateNoParishGroup(BusinessContext businessContext)
		{
			var name = "Personnes sans paroisse";
			var level = 0;
			var path = AiderGroupIds.NoParish;

			return AiderGroupEntity.Create (businessContext, null, name, level, path);
		}


		private static void ImportRegionAndParishGroups(CoreDataManager coreDataManager, ParishAddressRepository parishRepository)
		{
			coreDataManager.Execute (b =>
			{
				EervMainDataImporter.ImportRegionAndParishGroups (b, parishRepository);
			});
		}


		private static void ImportRegionAndParishGroups(BusinessContext businessContext, ParishAddressRepository parishRepository)
		{
			var regions = EervMainDataImporter.GetRegions (parishRepository);

			var regionGroups = EervMainDataImporter.CreateRegionGroups (businessContext, regions);
			var parishGroups = EervMainDataImporter.CreateParishGroups (businessContext, regionGroups, regions);

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);
		}


		private static Dictionary<int, Dictionary<string, List<ParishAddressInformation>>> GetRegions(ParishAddressRepository parishRepository)
		{
			return parishRepository.FindAllAddressInformations ()
				.GroupBy (pi => pi.RegionCode)
				.ToDictionary (g => g.Key, g => g.GroupBy (pi => pi.ParishName).ToDictionary (g2 => g2.Key, g2 => g2.ToList ()));
		}


		private static Dictionary<int, AiderGroupEntity> CreateRegionGroups(BusinessContext businessContext, Dictionary<int, Dictionary<string, List<ParishAddressInformation>>> regions)
		{
			var regionGroupDefinition = AiderGroupDefEntity.Find (businessContext, "Régions"); 

			return regions
				.Keys
				.ToDictionary (rc => rc, rc => EervMainDataImporter.CreateRegionGroup (businessContext, regionGroupDefinition, rc));
		}


		private static Dictionary<string, AiderGroupEntity> CreateParishGroups(BusinessContext businessContext, Dictionary<int, AiderGroupEntity> regionGroups, Dictionary<int, Dictionary<string, List<ParishAddressInformation>>> regions)
		{
			var parishGroupDefinition = AiderGroupDefEntity.Find (businessContext, "Paroisses");
			var parishAddressInfos = regions.Values.SelectMany (p => p.Values.Select (p2 => p2.First ())).ToArray ();
			var parishIds = regions.Keys.ToDictionary (x => x, x => 0);
			var parishes = new Dictionary<string, AiderGroupEntity> ();

			foreach (var parishAddressInfo in parishAddressInfos)
			{
				var regionId = parishAddressInfo.RegionCode;
				var parishId = parishIds[regionId] + 1;

				parishes.Add (parishAddressInfo.ParishName,
					/**/      EervMainDataImporter.CreateParishGroup (businessContext, parishGroupDefinition, parishAddressInfo, parishId));

				parishIds[regionId] = parishId;
			}

			return parishes;
		}


		private static AiderGroupEntity CreateRegionGroup(BusinessContext businessContext, AiderGroupDefEntity regionGroupDefinition, int regionCode)
		{
			var name     = AiderGroupEntity.GetRegionGroupName (regionCode);
			var template = regionGroupDefinition.PathTemplate.SubstringStart (AiderGroupIds.SubgroupLength);
			var path     = AiderGroupIds.GetRegionId (regionCode);
			var info     = new GroupPathInfo (name, template, path);

			return regionGroupDefinition.Instantiate (businessContext, info);
		}

		private static AiderGroupEntity CreateParishGroup(BusinessContext businessContext, AiderGroupDefEntity parishGroupDefinition, ParishAddressInformation parish, int parishId)
		{
			var name     = AiderGroupEntity.GetParishGroupName (parish.ParishName);
			var path     = AiderGroupIds.GetRegionId (parish.RegionCode) + AiderGroupIds.GetParishId (parishId);
			var template = parishGroupDefinition.PathTemplate.SubstringStart (AiderGroupIds.SubgroupLength);
			var info     = new GroupPathInfo (name, template, path, 1);

			var parishGroup = parishGroupDefinition.Instantiate (businessContext, info);

			return parishGroup;
		}


	}


}
