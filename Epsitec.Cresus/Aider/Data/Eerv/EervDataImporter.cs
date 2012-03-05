using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using System;

using System.Collections.Generic;

using System.Linq;
using Epsitec.Cresus.DataLayer.Context;


namespace Epsitec.Aider.Data.Eerv
{


	internal static class EervDataImporter
	{


		public static void ImportEervData(Func<BusinessContext> businessContextCreator, Action<BusinessContext> businessContextCleaner, IEnumerable<ParishAddressInformation> parishInformations)
		{
			var result = EervDataImporter.ImportRegionsAndParishes (businessContextCreator, businessContextCleaner, parishInformations);

			var regionNumbersToEntityKeys = result.Item1;
			var parishNamesToEntityKeys = result.Item2;
		}


		private static Tuple<Dictionary<int, EntityKey>, Dictionary<string, EntityKey>> ImportRegionsAndParishes(Func<BusinessContext> businessContextCreator, Action<BusinessContext> businessContextCleaner, IEnumerable<ParishAddressInformation> parishInformations)
		{
			Dictionary<int, EntityKey> regionNumbersToEntityKeys;
			Dictionary<string, EntityKey> parishNamesToEntityKeys;

			var regions = EervDataImporter.GetRegions (parishInformations);

			using (var businessContext = businessContextCreator ())
			{
				try
				{
					var regionGroups = EervDataImporter.CreateRegionGroups (businessContext, regions);
					var parishGroups = EervDataImporter.CreateParishGroups (businessContext, regionGroups, regions);

					businessContext.SaveChanges ();

					var dataContext = businessContext.DataContext;

					regionNumbersToEntityKeys = regionGroups.ToDictionary (r => r.Key, r => dataContext.GetNormalizedEntityKey (r.Value).Value);
					parishNamesToEntityKeys = parishGroups.ToDictionary (p=> p.Key, p => dataContext.GetNormalizedEntityKey (p.Value).Value);
				}
				finally
				{
					businessContextCleaner (businessContext);
				}
			}

			return Tuple.Create (regionNumbersToEntityKeys, parishNamesToEntityKeys);
		}


		private static Dictionary<int, Dictionary<string, List<ParishAddressInformation>>> GetRegions(IEnumerable<ParishAddressInformation> parishInformations)
		{
			return parishInformations
				.GroupBy (pi => pi.RegionCode)
				.ToDictionary (g => g.Key, g => g.GroupBy (pi => pi.ParishName).ToDictionary (g2 => g2.Key, g2 => g2.ToList ()));
		}


		private static Dictionary<int, AiderGroupEntity> CreateRegionGroups(BusinessContext businessContext, Dictionary<int, Dictionary<string, List<ParishAddressInformation>>> regions)
		{
			var regionGroupDefinition = EervDataImporter.CreateRegionGroupDefinition (businessContext);

			return regions.Keys.ToDictionary (rc => rc, rc => EervDataImporter.CreateRegionGroup (businessContext, regionGroupDefinition, rc));
		}


		private static Dictionary<string, AiderGroupEntity> CreateParishGroups(BusinessContext businessContext, Dictionary<int, AiderGroupEntity> regionGroups, Dictionary<int, Dictionary<string, List<ParishAddressInformation>>> regions)
		{
			var parishGroupDefinitinon = EervDataImporter.CreateParishGroupDefinition (businessContext);

			return regions
				.Values
				.SelectMany (p => p.Values.Select (p2 => p2.First ()))
				.ToDictionary (p => p.ParishName, p => EervDataImporter.CreateParishGroup (businessContext, regionGroups, parishGroupDefinitinon, p));
		}


		private static AiderGroupEntity CreateRegionGroup(BusinessContext businessContext, AiderGroupDefEntity regionGroupDefinition, int regionCode)
		{
			var regionGroup = businessContext.CreateEntity<AiderGroupEntity> ();

			regionGroup.Name = "R" + InvariantConverter.ToString (regionCode);
			regionGroup.GroupDef = regionGroupDefinition;

			return regionGroup;
		}


		private static AiderGroupEntity CreateParishGroup(BusinessContext businessContext, Dictionary<int, AiderGroupEntity> regionGroups, AiderGroupDefEntity parishGroupDefinition, ParishAddressInformation parish)
		{
			var parishGroup = businessContext.CreateEntity<AiderGroupEntity> ();

			parishGroup.Name = parish.ParishName;
			parishGroup.GroupDef = parishGroupDefinition;
			parishGroup.Root = regionGroups[parish.RegionCode];

			return parishGroup;
		}


		private static AiderGroupDefEntity CreateRegionGroupDefinition(BusinessContext businessContext)
		{
			var name = "Définition des groupes régionaux";
			var category = GroupCategory.SystemDefined;
			var defType = GroupDefType.Region;
			var type = GroupType.Root;

			return EervDataImporter.CreateGroupDefinition (businessContext, name, category, type, defType);
		}


		private static AiderGroupDefEntity CreateParishGroupDefinition(BusinessContext businessContext)
		{
			var name = "Définition des groupes paroissiaux";
			var category = GroupCategory.SystemDefined;
			var defType = GroupDefType.Parish;
			var type = GroupType.Node;

			return EervDataImporter.CreateGroupDefinition (businessContext, name, category, type, defType);
		}


		private static AiderGroupDefEntity CreateGroupDefinition(BusinessContext businessContext, string name, GroupCategory category, GroupType type, GroupDefType defType)
		{
			var groupDefinition = businessContext.CreateEntity<AiderGroupDefEntity> ();

			groupDefinition.Name = name;
			groupDefinition.Category = category;
			groupDefinition.DefType = defType;
			groupDefinition.Type = type;

			return groupDefinition;
		}


	}


}
