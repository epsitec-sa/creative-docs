using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{


	internal static class EervMainDataImporter
	{


		public static void Import(CoreData coreData, EervMainData eervData, ParishAddressRepository parishRepository)
		{
			EervMainDataImporter.ImportGroupDefinitions (coreData, eervData);
			EervMainDataImporter.ImportGlobalGroups (coreData, eervData);
			EervMainDataImporter.ImportRegionAndParishGroups (coreData, parishRepository);

			coreData.ResetIndexes ();
		}


		private static void ImportGroupDefinitions(CoreData coreData, EervMainData eervData)
		{
			using (var businessContext = new BusinessContext(coreData, false))
			{
				EervMainDataImporter.ImportGroupDefinitions (businessContext, eervData);
			}
		}


		private static void ImportGroupDefinitions(BusinessContext businessContext, EervMainData eervData)
		{
			var topLevelGroups = EervMainDataImporter.GetTopLevelGroupDefinitions (eervData);
			var mapping = new Dictionary<EervGroupDefinition, AiderGroupDefEntity> ();

			foreach (var groupDefinition in topLevelGroups)
			{
				EervMainDataImporter.ImportGroupDefinition (businessContext, groupDefinition, mapping, 0);
			}

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);
		}


		private static IEnumerable<EervGroupDefinition> GetTopLevelGroupDefinitions(EervMainData eervData)
		{
			return eervData
				.GroupDefinitions
				.Where (g => EervMainDataImporter.IsTopLevelGroupDefinition (g))
				.Where (g => !EervMainDataImporter.SkipGroupDefinition (g));
		}


		private static bool IsTopLevelGroupDefinition(EervGroupDefinition groupDefinition)
		{
			switch (groupDefinition.GroupLevel)
			{
				case 0:
					return true;

				case 1:
					return groupDefinition.GroupClassification == GroupClassification.Function
						|| groupDefinition.GroupClassification == GroupClassification.Staff
						|| groupDefinition.GroupClassification == GroupClassification.StaffAssociation;

				default:
					return false;
			}
		}


		private static bool SkipGroupDefinition(EervGroupDefinition groupDefinition)
		{
			// Pierre asked me not to import the Staff groups, after a discussion with Jean-Michel
			// Sordet from the EERV. So we skip these group definitions here.

			return groupDefinition.GroupClassification == GroupClassification.Staff;
		}


		private static AiderGroupDefEntity ImportGroupDefinition(BusinessContext businessContext, EervGroupDefinition groupDefinition, Dictionary<EervGroupDefinition, AiderGroupDefEntity> mapping, int level)
		{
			var aiderGroupDef = businessContext.CreateAndRegisterEntity<AiderGroupDefEntity> ();

			mapping[groupDefinition] = aiderGroupDef;

			aiderGroupDef.Name = groupDefinition.Name;
			aiderGroupDef.Number = groupDefinition.Id;
			// We don't use the eerv group level because the 01* groups don't have the good one.
			aiderGroupDef.Level = level;
			aiderGroupDef.SubgroupsAllowed = groupDefinition.SubgroupsAllowed;
			aiderGroupDef.MembersAllowed = groupDefinition.MembersAllowed;
			aiderGroupDef.PathTemplate = groupDefinition.GetPathTemplate ();
			aiderGroupDef.Classification = groupDefinition.GroupClassification;
			aiderGroupDef.Mutability = EervMainDataImporter.GetMutability (groupDefinition);

			if (groupDefinition.FunctionGroup != null)
			{
				aiderGroupDef.Function = mapping[groupDefinition.FunctionGroup];
			}

			var children = groupDefinition
				.Children
				.Where (c => !EervMainDataImporter.IsGroupDefinitionToDiscard (c));

			foreach (var child in children)
			{
				var importedChild = EervMainDataImporter.ImportGroupDefinition (businessContext, child, mapping, level + 1);

				aiderGroupDef.Subgroups.Add (importedChild);
			}

			return aiderGroupDef;
		}


		private static Mutability GetMutability(EervGroupDefinition groupDefinition)
		{
			var customizable = groupDefinition
				.Children
				.Any (gd => EervMainDataImporter.IsGroupDefinitionToDiscard (gd));

			return customizable
				? Mutability.Customizable
				: Mutability.SystemDefined;
		}


		private static void ImportGlobalGroups(CoreData coreData, EervMainData eervData)
		{
			using (var businessContext = new BusinessContext(coreData, false))
			{
				EervMainDataImporter.ImportGlobalGroups (businessContext, eervData);
			}
		}


		private static void ImportGlobalGroups(BusinessContext businessContext, EervMainData eervData)
		{
			var globalEervGroupDefinitions = EervMainDataImporter
				.GetTopLevelGroupDefinitions (eervData)
				.Where (g => EervMainDataImporter.IsGlobalGroup (g));

			foreach (var eervGroupDefinition in globalEervGroupDefinitions)
			{
				EervMainDataImporter.ImportGlobalGroup (businessContext, eervGroupDefinition);
			}

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);
		}


		private static void ImportGlobalGroup(BusinessContext businessContext, EervGroupDefinition eervGroupDefinition)
		{
			var aiderGroupDefinition = EervMainDataImporter.GetRootGroupDefinition (businessContext, eervGroupDefinition.GroupClassification);

			aiderGroupDefinition.Instantiate (businessContext);
		}


		private static bool IsGlobalGroup(EervGroupDefinition groupDefinition)
		{
			// We don't instantiate function group definitions because their are instantiated only
			// within special groups with special rules. We don't instantiate parish and region
			// groups because they are template group definitions that are instantiated later on.

			return groupDefinition.GroupClassification != GroupClassification.Function
				&& groupDefinition.GroupClassification != GroupClassification.Parish
				&& groupDefinition.GroupClassification != GroupClassification.Region;
		}


		private static void ImportRegionAndParishGroups(CoreData coreData, ParishAddressRepository parishRepository)
		{
			using (var businessContext = new BusinessContext(coreData, false))
			{
				EervMainDataImporter.ImportRegionAndParishGroups (businessContext, parishRepository);
			}
		}


		private static void ImportRegionAndParishGroups(BusinessContext businessContext, ParishAddressRepository parishRepository)
		{
			var regions = EervMainDataImporter.GetRegions (parishRepository);

			var regionGroups = EervMainDataImporter.CreateRegionGroups (businessContext, regions);
			var parishGroups = EervMainDataImporter.CreateParishGroups (businessContext, regions, regionGroups);

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
			var regionGroupDefinition = EervMainDataImporter.GetRootGroupDefinition (businessContext, GroupClassification.Region);

			return regions
				.Keys
				.ToDictionary (rc => rc, rc => EervMainDataImporter.CreateRegionGroup (businessContext, regionGroupDefinition, rc));
		}


		private static AiderGroupDefEntity GetRootGroupDefinition(BusinessContext businessContext, GroupClassification classification)
		{
			var dataContext = businessContext.DataContext;

			var example = new AiderGroupDefEntity ()
			{
				Level = 0,
				Classification = classification
			};

			return dataContext.GetByExample (example).Single ();
		}


		private static Dictionary<string, AiderGroupEntity> CreateParishGroups(BusinessContext businessContext, Dictionary<int, Dictionary<string, List<ParishAddressInformation>>> regions, Dictionary<int, AiderGroupEntity> regionGroups)
		{
			var parishGroupDefinition = EervMainDataImporter.GetRootGroupDefinition (businessContext, GroupClassification.Parish);
			var parishAddressInfos = regions.Values.SelectMany (p => p.Values.Select (p2 => p2.First ())).ToArray ();
			var parishIds = regions.Keys.ToDictionary (x => x, x => 0);
			var parishes = new Dictionary<string, AiderGroupEntity> ();

			foreach (var parishAddressInfo in parishAddressInfos)
			{
				var regionId = parishAddressInfo.RegionCode;
				var regionGroup = regionGroups[regionId];

				var parishId = parishIds[regionId] + 1;
				var parishName = parishAddressInfo.ParishName;
				var parish = EervMainDataImporter.CreateParishGroup (businessContext, regionGroup, parishGroupDefinition, parishAddressInfo, parishId);
				
				parishes.Add (parishName, parish);
				parishIds[regionId] = parishId;
			}

			return parishes;
		}


		private static AiderGroupEntity CreateRegionGroup(BusinessContext businessContext, AiderGroupDefEntity regionGroupDefinition, int regionCode)
		{
			var name = ParishAssigner.GetRegionGroupName (regionCode);
			var code = regionCode;

			return regionGroupDefinition.InstantiateRegion (businessContext, name, code);
		}


		private static AiderGroupEntity CreateParishGroup(BusinessContext businessContext, AiderGroupEntity regionGroup, AiderGroupDefEntity parishGroupDefinition, ParishAddressInformation parish, int parishId)
		{
			var name = ParishAssigner.GetParishGroupName (parish.ParishName);
			var code = parishId;

			return parishGroupDefinition.InstantiateParish (businessContext, regionGroup, name, code);
		}


		private static bool IsGroupDefinitionToDiscard(EervGroupDefinition groupDefinition)
		{
			var name = groupDefinition.Name;

			return name.Contains ("n1")
				|| name.Contains ("n2")
				|| name.Contains ("n3")
				|| namesToDiscard.Contains (name);
		}


		private static readonly HashSet<string> namesToDiscard = new HashSet<string> ()
		{
			"Projets 1",
			"Projet 2",
			"Mandat 1, 2, 3",
			"Groupe pilotage 1",
			"Groupe de projets 1",
			"Groupe pilotage 2",
			"Groupe de projets 2",
			"Groupe pilotage 1, 2, 3",
			"Mandataires 1, 2, 3",
			"Délégation paroisse 1, 2, 3",
			"Archives 1, Archives 2, Archives 3…",
		};


	}


}
