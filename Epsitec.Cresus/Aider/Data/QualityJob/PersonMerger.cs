using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Common.IO;
using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Epsitec.Cresus.Database;

namespace Epsitec.Aider.Data.Job
{
	/// <summary>
	/// Merge des personnes depuis un fichier de correction
	/// </summary>
	internal static class PersonMerger
	{
		public static void Run(CoreData coreData, FileInfo fixupDataFile)
		{
			var duplicateSets = new List<HashSet<long>>();
			using (StreamReader r = new StreamReader(fixupDataFile.FullName))
			{
				var json = r.ReadToEnd();
				duplicateSets = JsonConvert.DeserializeObject<List<HashSet<long>>>(json);
			}
			using (var businessContext = new BusinessContext(coreData, false))
            {
				foreach (var set in duplicateSets)
				{
					var personToMerge = new List<AiderPersonEntity>();
					var toCleanup = new List<eCH_PersonEntity>();
					foreach (var id in set)
					{
						var person = businessContext.DataContext.ResolveEntity<eCH_PersonEntity>(new DbKey(id));
						if(person.DataSource == DataSource.Undefined)
                        {
							toCleanup.Add(person);

						}
						if(person.IsDeceased)
                        {
							break;
                        }
                        try
                        {
							var aiderPerson = businessContext.DataContext.GetSingleByExample<AiderPersonEntity>(new AiderPersonEntity
							{
								eCH_Person = person
							});

							personToMerge.Add(aiderPerson);
						}
						catch
                        {
							continue;
                        }
					}
					if(personToMerge.Count>1)
                    {
						var govAiderPerson = personToMerge.Where(p => p.IsGovernmentDefined).First();
						var nonGovAiderPersons = personToMerge.Where(p => p.IsGovernmentDefined == false).ToList();
						foreach (var nonGov in nonGovAiderPersons)
						{
							var merged = AiderPersonEntity.MergePersons(businessContext, govAiderPerson, nonGov);
							if(merged)
							{
								foreach(var person in toCleanup)
                                {
									eCH_PersonEntity.Delete(businessContext, person);
                                }
							}
						}
					}
				}
			}
				
		}
	}


}
