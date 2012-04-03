using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Tools;

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using System;

using System.Collections.Generic;

using System.Diagnostics;

using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{


	internal static class EervMainDataImporter
	{


		public static void Import(BusinessContextManager businessContextManager, ParishAddressRepository parishRepository)
		{
			var result = EervMainDataImporter.ImportRegionsAndParishes (businessContextManager, parishRepository);

			var regionNumbersToEntityKeys = result.Item1;
			var parishNamesToEntityKeys = result.Item2;

			EervMainDataImporter.AssignPersonsToParishes (businessContextManager, parishRepository, parishNamesToEntityKeys);
		}


		private static Tuple<Dictionary<int, EntityKey>, Dictionary<string, EntityKey>> ImportRegionsAndParishes(BusinessContextManager businessContextManager, ParishAddressRepository parishRepository)
		{
			Func<BusinessContext, Tuple<Dictionary<int, EntityKey>, Dictionary<string, EntityKey>>> function = b =>
			{
				return ImportRegionsAndParishes (b, parishRepository);
			};

			return businessContextManager.Execute (function);
		}


		private static Tuple<Dictionary<int, EntityKey>, Dictionary<string, EntityKey>> ImportRegionsAndParishes(BusinessContext businessContext, ParishAddressRepository parishRepository)
		{
			Dictionary<int, EntityKey> regionNumbersToEntityKeys;
			Dictionary<string, EntityKey> parishNamesToEntityKeys;

			var regions = EervMainDataImporter.GetRegions (parishRepository);

			var regionGroups = EervMainDataImporter.CreateRegionGroups (businessContext, regions);
			var parishGroups = EervMainDataImporter.CreateParishGroups (businessContext, regionGroups, regions);

			businessContext.SaveChanges ();

			var dataContext = businessContext.DataContext;

			regionNumbersToEntityKeys = regionGroups.ToDictionary (r => r.Key, r => dataContext.GetNormalizedEntityKey (r.Value).Value);
			parishNamesToEntityKeys = parishGroups.ToDictionary (p => p.Key, p => dataContext.GetNormalizedEntityKey (p.Value).Value);

			return Tuple.Create (regionNumbersToEntityKeys, parishNamesToEntityKeys);
		}


		private static Dictionary<int, Dictionary<string, List<ParishAddressInformation>>> GetRegions(ParishAddressRepository parishRepository)
		{
			return parishRepository.FindAllAddressInformations ()
				.GroupBy (pi => pi.RegionCode)
				.ToDictionary (g => g.Key, g => g.GroupBy (pi => pi.ParishName).ToDictionary (g2 => g2.Key, g2 => g2.ToList ()));
		}


		private static Dictionary<int, AiderGroupEntity> CreateRegionGroups(BusinessContext businessContext, Dictionary<int, Dictionary<string, List<ParishAddressInformation>>> regions)
		{
			var regionGroupDefinition = EervMainDataImporter.CreateRegionGroupDefinition (businessContext);

			return regions.Keys.ToDictionary (rc => rc, rc => EervMainDataImporter.CreateRegionGroup (businessContext, regionGroupDefinition, rc));
		}


		private static Dictionary<string, AiderGroupEntity> CreateParishGroups(BusinessContext businessContext, Dictionary<int, AiderGroupEntity> regionGroups, Dictionary<int, Dictionary<string, List<ParishAddressInformation>>> regions)
		{
			var parishGroupDefinitinon = EervMainDataImporter.CreateParishGroupDefinition (businessContext);

			return regions
				.Values
				.SelectMany (p => p.Values.Select (p2 => p2.First ()))
				.ToDictionary (p => p.ParishName, p => EervMainDataImporter.CreateParishGroup (businessContext, regionGroups, parishGroupDefinitinon, p));
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

			return EervMainDataImporter.CreateGroupDefinition (businessContext, name, category, type, defType);
		}


		private static AiderGroupDefEntity CreateParishGroupDefinition(BusinessContext businessContext)
		{
			var name = "Définition des groupes paroissiaux";
			var category = GroupCategory.SystemDefined;
			var defType = GroupDefType.Parish;
			var type = GroupType.Node;

			return EervMainDataImporter.CreateGroupDefinition (businessContext, name, category, type, defType);
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


		private static void AssignPersonsToParishes(BusinessContextManager businessContextManager, ParishAddressRepository parishRepository, Dictionary<string, EntityKey> parishNamesToEntityKeys)
		{
			// TODO This method could be significantly improved performance wise if we could have a
			// way to return only a subset of entities from a request to the DataContext. Because
			// now we have to rely on the set of persons that have already processed and we have to
			// load all persons in memory at each batch, which is useless and horribly slow.

			var processedPersons = new HashSet<EntityKey> ();

			var batchSize = 5000;
			var finished = false;

			var batchIndex = 0;

			do
			{
				var processed = EervMainDataImporter.AssignPersonsToParishes (businessContextManager, parishRepository, parishNamesToEntityKeys, processedPersons, batchSize);

				 if (processed.Any ())
				 {
					 processedPersons.AddRange (processed);
				 }
				 else
				 {
					 finished = true;
				 }

				var t = DateTime.Now;
				var lowerBound = batchIndex * batchSize;
				var upperBound = (batchIndex + 1) * batchSize;

				Debug.WriteLine (string.Format ("[{0}]\tAssigned person batch ({1}-{2}) to parish", t, lowerBound, upperBound));

				batchIndex++;
			}
			while (!finished);
		}


		private static HashSet<EntityKey> AssignPersonsToParishes(BusinessContextManager businessContextManager, ParishAddressRepository parishRepository, Dictionary<string, EntityKey> parishNamesToEntityKeys, HashSet<EntityKey> processedPersons, int batchSize)
		{
			Func<BusinessContext, HashSet<EntityKey>> function = b =>
			{
				return EervMainDataImporter.AssignPersonsToParishes (b, parishRepository, parishNamesToEntityKeys, processedPersons, batchSize);
			};

			return businessContextManager.Execute (function);
		}


		private static HashSet<EntityKey> AssignPersonsToParishes(BusinessContext businessContext, ParishAddressRepository parishRepository, Dictionary<string, EntityKey> parishNamesToEntityKeys, HashSet<EntityKey> processedPersons, int batchSize)
		{
			var processed = new HashSet<EntityKey> ();
			var persons = businessContext.GetAllEntities<AiderPersonEntity> ().ToList ();

			businessContext.GetAllEntities<AiderHouseholdEntity> ();
			businessContext.GetAllEntities<AiderAddressEntity> ();
			businessContext.GetAllEntities<AiderTownEntity> ();

			for (int i = 0; i < persons.Count && processed.Count < batchSize; i++)
			{
				var person = persons[i];
				var personEntityKey = businessContext.DataContext.GetNormalizedEntityKey (person).Value;

				var alreadyProcessed = processedPersons.Contains (personEntityKey);

				if (!alreadyProcessed)
				{
					EervMainDataImporter.AssignPersonToParish (businessContext, parishRepository, parishNamesToEntityKeys, person);

					processed.Add (personEntityKey);
				}
			}

			businessContext.SaveChanges ();

			return processed;
		}


		private static void AssignPersonToParish(BusinessContext businessContext, ParishAddressRepository parishRepository, Dictionary<string, EntityKey> parishNamesToEntityKeys, AiderPersonEntity person)
		{
			var address = person.GetHouseholds ().First ().Address;
			var parishGroup = EervMainDataImporter.FindParishGroup (businessContext, parishRepository, parishNamesToEntityKeys, address);

			if (parishGroup == null)
			{
				var nameText = person.DisplayName;
				var addressText = address.GetSummary ().ToSimpleText ().Replace ("\n", "; ");

				Debug.WriteLine ("WARNING: parish not found for " + nameText + " at address " + addressText);
			}
			else
			{
				EervMainDataImporter.AssignPersonToParish (businessContext, person, parishGroup);
			}
		}


		private static AiderGroupEntity FindParishGroup(BusinessContext businessContext, ParishAddressRepository parishRepository, Dictionary<string, EntityKey> parishNamesToEntityKeys, AiderAddressEntity address)
		{
			var parishName = ParishLocator.FindParishName (parishRepository, address);

			AiderGroupEntity parishGroup;

			if (parishName == null)
			{
				parishGroup = null;
			}
			else
			{
				var parishEntityKey = parishNamesToEntityKeys[parishName];
				parishGroup = (AiderGroupEntity) businessContext.DataContext.ResolveEntity (parishEntityKey);
			}

			return parishGroup;
		}


		private static AiderGroupParticipantEntity AssignPersonToParish(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity parishGroup)
		{
			var participant = businessContext.CreateEntity<AiderGroupParticipantEntity> ();

			participant.Person = person;
			participant.StartDate = Date.Today;
			participant.Group = parishGroup;

			return participant;
		}


	}


}
