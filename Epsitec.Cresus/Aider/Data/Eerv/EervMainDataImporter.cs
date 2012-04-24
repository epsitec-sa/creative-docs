using Epsitec.Aider.Entities;
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


		public static void Import(BusinessContextManager businessContextManager, EervMainData eervData, ParishAddressRepository parishRepository)
		{
			EervMainDataImporter.ImportGroupDefinitions (businessContextManager, eervData);

			var result = EervMainDataImporter.ImportRegionsAndParishes (businessContextManager, parishRepository);

			var parishNamesToEntityKeys = result.Item2;

			EervMainDataImporter.AssignPersonsToParishes (businessContextManager, parishRepository, parishNamesToEntityKeys);
		}


		private static void ImportGroupDefinitions(BusinessContextManager businessContextManager, EervMainData eervData)
		{
			businessContextManager.Execute (b => EervMainDataImporter.ImportGroupDefinitions (b, eervData));
		}


		private static void ImportGroupDefinitions(BusinessContext businessContext, EervMainData eervData)
		{
			var topLevelGroups = eervData.GroupDefinitions.Where (g => g.Parent == null);

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

			var children = groupDefinition
				.Children
				.Select (c => EervMainDataImporter.ImportGroupDefinition (businessContext, c))
				.ToList ();

			aiderGroupDef.Subgroups.AddRange (children);

			// TODO Add a lot more stuff here to set up properly the group definition level, type,
			// category, etc.

			return aiderGroupDef;
		}


		private static Tuple<Dictionary<int, EntityKey>, Dictionary<string, EntityKey>> ImportRegionsAndParishes(BusinessContextManager businessContextManager, ParishAddressRepository parishRepository)
		{
			return businessContextManager.Execute(b =>
			{
				return EervMainDataImporter.ImportRegionsAndParishes(b, parishRepository);
			});
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
			var name = "Région " + InvariantConverter.ToString (regionCode);

			return regionGroupDefinition.Instantiate (businessContext, name);
		}


		private static AiderGroupEntity CreateParishGroup(BusinessContext businessContext, Dictionary<int, AiderGroupEntity> regionGroups, AiderGroupDefEntity parishGroupDefinition, ParishAddressInformation parish)
		{
			var name = "Paroisse de " + parish.ParishName;

			var parishGroup = parishGroupDefinition.Instantiate (businessContext, name);
			
			parishGroup.Root = regionGroups[parish.RegionCode];

			return parishGroup;
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
