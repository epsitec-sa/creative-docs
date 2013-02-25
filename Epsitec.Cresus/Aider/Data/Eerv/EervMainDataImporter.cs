﻿using Epsitec.Aider.Entities;
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
			var topLevelGroups = eervData
				.GroupDefinitions
				.Where (g => EervMainDataImporter.IsTopLevelGroupDefinition (g));

			foreach (var groupDefinition in topLevelGroups)
			{
				EervMainDataImporter.ImportGroupDefinition (businessContext, groupDefinition);
			}

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);
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


		private static AiderGroupDefEntity ImportGroupDefinition(BusinessContext businessContext, EervGroupDefinition groupDefinition)
		{
			var aiderGroupDef = businessContext.CreateAndRegisterEntity<AiderGroupDefEntity> ();

			aiderGroupDef.Name = groupDefinition.Name;
			aiderGroupDef.Number = groupDefinition.Id;
			aiderGroupDef.SubgroupsAllowed = groupDefinition.SubgroupsAllowed;
			aiderGroupDef.MembersAllowed = groupDefinition.MembersAllowed;
			aiderGroupDef.PathTemplate = groupDefinition.GetPathTemplate ();
			aiderGroupDef.Classification = groupDefinition.GroupClassification;
			aiderGroupDef.Mutability = EervMainDataImporter.GetMutability (groupDefinition);

			var children = new List<AiderGroupDefEntity> ();

			if (groupDefinition.FunctionGroup != null)
			{
				var funcGroup = groupDefinition.FunctionGroup;
				var funcGroupEntity = funcGroup.EntityCache;
				children.Add (funcGroupEntity);
			}

			children.AddRange (groupDefinition
				.Children
				.Where (c => !EervMainDataImporter.IsGroupDefinitionToDiscard (c))
				.Select (c => EervMainDataImporter.ImportGroupDefinition (businessContext, c)));

			aiderGroupDef.Subgroups.AddRange (children);
			groupDefinition.EntityCache = aiderGroupDef;

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
			var rootEervGroupDefinitions = eervData
				.GroupDefinitions
				.Where (g => EervMainDataImporter.IsGlobalGroup (g));

			foreach (var eervGroupDefinition in rootEervGroupDefinitions)
			{
				EervMainDataImporter.ImportGlobalGroup (businessContext, eervGroupDefinition);
			}

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);
		}


		private static void ImportGlobalGroup(BusinessContext businessContext, EervGroupDefinition eervGroupDefinition)
		{
			var example = new AiderGroupDefEntity ()
			{
				PathTemplate = eervGroupDefinition.GetPathTemplate ()
			};
				
			var aiderGroupDefinition = businessContext.DataContext.GetByExample (example).Single ();
				
			aiderGroupDefinition.Instantiate (businessContext, new GroupPathInfo
			(
				name: aiderGroupDefinition.Name,
				template: aiderGroupDefinition.PathTemplate,
				output: aiderGroupDefinition.PathTemplate,
				level: 0
			));
		}


		private static bool IsGlobalGroup(EervGroupDefinition groupDefinition)
		{
			// We don't instantiate function group definitions because their are instantiated only
			// within special groups with special rules. We don't instantiate parish and region
			// groups because they are template group definitions that are instantiated later on.

			return EervMainDataImporter.IsTopLevelGroupDefinition (groupDefinition)
				&& groupDefinition.GroupClassification != GroupClassification.Function
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
				PathTemplate = EervMainDataImporter.GetPathTemplate (classification)
			};

			return dataContext.GetByExample (example).Single ();
		}


		private static string GetPathTemplate(GroupClassification classification)
		{
			switch (classification)
			{
				case GroupClassification.Region:
					return AiderGroupIds.Region;

				case GroupClassification.Parish:
					return AiderGroupIds.Parish;

				default:
					throw new NotImplementedException ();
			}
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
			var name     = ParishAssigner.GetRegionGroupName (regionCode);
			var template = regionGroupDefinition.PathTemplate.SubstringStart (AiderGroupIds.SubgroupLength);
			var path     = AiderGroupIds.GetRegionId (regionCode);
			var info     = new GroupPathInfo (name, template, path);

			return regionGroupDefinition.Instantiate (businessContext, info);
		}

		private static AiderGroupEntity CreateParishGroup(BusinessContext businessContext, AiderGroupEntity regionGroup, AiderGroupDefEntity parishGroupDefinition, ParishAddressInformation parish, int parishId)
		{
			var name     = ParishAssigner.GetParishGroupName (parish.ParishName);
			var path     = AiderGroupIds.GetRegionId (parish.RegionCode) + AiderGroupIds.GetParishId (parishId);
			var template = parishGroupDefinition.PathTemplate.SubstringStart (AiderGroupIds.SubgroupLength);
			var info     = new GroupPathInfo (name, template, path, 1);

			var parishGroup = parishGroupDefinition.Instantiate (businessContext, regionGroup, info);

			return parishGroup;
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
