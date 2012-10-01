using Epsitec.Aider.Entities;
using Epsitec.Aider.Tools;

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

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

			EervMainDataImporter.ImportRegionsAndParishes (coreDataManager, parishRepository);

			EervMainDataImporter.AssignPersonsToParishes (coreDataManager, parishRepository);

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

			businessContext.SaveChanges ();
		}


		private static AiderGroupDefEntity ImportGroupDefinition(BusinessContext businessContext, EervGroupDefinition groupDefinition)
		{
			var aiderGroupDef = businessContext.CreateEntity<AiderGroupDefEntity> ();

			aiderGroupDef.Name = groupDefinition.Name;
			aiderGroupDef.PathTemplate = groupDefinition.GetPathTemplate ();
			aiderGroupDef.NodeType = groupDefinition.GroupNodeType;
			aiderGroupDef.Classification = groupDefinition.GroupClassification;
			aiderGroupDef.Category = Enumerations.GroupCategory.SystemDefined;

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


		private static void ImportRegionsAndParishes(CoreDataManager coreDataManager, ParishAddressRepository parishRepository)
		{
			coreDataManager.Execute(b =>
			{
				EervMainDataImporter.ImportRegionsAndParishes(b, parishRepository);
			});
		}


		private static void ImportRegionsAndParishes(BusinessContext businessContext, ParishAddressRepository parishRepository)
		{
			var regions = EervMainDataImporter.GetRegions (parishRepository);

			var regionGroups = EervMainDataImporter.CreateRegionGroups (businessContext, regions);
			var parishGroups = EervMainDataImporter.CreateParishGroups (businessContext, regionGroups, regions);

			businessContext.SaveChanges ();
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

			return regions
				.Values
				.SelectMany (p => p.Values.Select (p2 => p2.First ()))
				.ToDictionary (p => p.ParishName, p => EervMainDataImporter.CreateParishGroup (businessContext, regionGroups, parishGroupDefinition, p));
		}


		private static AiderGroupEntity CreateRegionGroup(BusinessContext businessContext, AiderGroupDefEntity regionGroupDefinition, int regionCode)
		{
			var name = AiderGroupEntity.GetRegionGroupName (regionCode);

			return regionGroupDefinition.Instantiate (businessContext, name);
		}


		private static AiderGroupEntity CreateParishGroup(BusinessContext businessContext, Dictionary<int, AiderGroupEntity> regionGroups, AiderGroupDefEntity parishGroupDefinition, ParishAddressInformation parish)
		{
			var name = AiderGroupEntity.GetParishGroupName (parish.ParishName);

			var parishGroup = parishGroupDefinition.Instantiate (businessContext, name);

			//	TODO: init. group path
//-			parishGroup.Root = regionGroups[parish.RegionCode];

			return parishGroup;
		}


		private static void AssignPersonsToParishes(CoreDataManager coreDataManager, ParishAddressRepository parishRepository)
		{
			AiderEnumerator.Execute
			(
				coreDataManager,
				(b, p) => ParishAssigner.AssignToParishes (parishRepository, b, p)
			);
		}


	}


}
