//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Data.ECh;
using Epsitec.Aider.Data.Eerv;
using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Library.UI;

using Epsitec.Data.Platform;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Epsitec.Aider
{
	public static partial class AiderProgram
	{
		private static void TestFullImport(AiderProgramTestImportMode importMode)
		{
			SwissPost.Initialize ();

			CoreContext.ParseOptionalSettingsFile (CoreContext.ReadCoreContextSettingsFile ());
			CoreContext.EnableEmbeddedDatabaseClient (true);
			CoreContext.StartAsInteractive ();

			Services.Initialize ();

			try
			{
				CoreData.ForceDatabaseCreationRequest = true;
				AiderProgram.TestFullImportJob (importMode);
			}
			finally
			{
				Services.ShutDown ();
			}
		}

		private static void TestFullImportJob(AiderProgramTestImportMode importMode)
		{
			using (var application = new CoreApplication ())
			{
				application.SetupApplication ();

				var coreData = application.Data;

				System.Diagnostics.Trace.WriteLine ("[" + System.DateTime.Now + "]\tSTART");

				var eervGroupDefinitionFile = new FileInfo ("..\\..\\Samples\\EERV Main\\groupdefinition.xlsx");
				var eervMainData = EervMainDataLoader.LoadEervData (eervGroupDefinitionFile);
				var parishRepository = ParishAddressRepository.Current;
				EervMainDataImporter.Import (coreData, eervMainData, parishRepository);

				System.Diagnostics.Trace.WriteLine ("[" + System.DateTime.Now + "]\tDONE EERV MAIN");

				AiderProgram.CreateUsers (coreData);

				System.Diagnostics.Trace.WriteLine ("[" + System.DateTime.Now + "]\tDONE USER CREATION");

				var eChDataFile = new FileInfo ("..\\..\\Samples\\eerv.xml");
				var eChReportedPersons = EChDataLoader.Load (eChDataFile, importMode.HasFlag (AiderProgramTestImportMode.Subset) ? 2000 : int.MaxValue);
				EChDataImporter.Import (coreData, parishRepository, eChReportedPersons);

				System.Diagnostics.Trace.WriteLine ("[" + System.DateTime.Now + "]\tDONE ECH");

				if (importMode.HasFlag (AiderProgramTestImportMode.EchOnly))
				{
					return;
				}

				AiderProgram.Test
				(
					coreData: coreData,
					person: "..\\..\\Samples\\EERV Région 9\\9010\\person.xlsx",
					activity: "..\\..\\Samples\\EERV Région 9\\9010\\activity.xlsx",
					group: "..\\..\\Samples\\EERV Région 9\\9010\\group.xlsx",
					supergroup: "..\\..\\Samples\\EERV Région 9\\9010\\supergroup.xlsx",
					id: "..\\..\\Samples\\EERV Région 9\\id.xlsx"
				);

				System.Diagnostics.Trace.WriteLine ("[" + System.DateTime.Now + "]\tDONE EERV PARISH 1");

				AiderProgram.Test
				(
					coreData: coreData,
					person: "..\\..\\Samples\\EERV Région 9\\9020\\person.xlsx",
					activity: "..\\..\\Samples\\EERV Région 9\\9020\\activity.xlsx",
					group: "..\\..\\Samples\\EERV Région 9\\9020\\group.xlsx",
					supergroup: "..\\..\\Samples\\EERV Région 9\\9020\\supergroup.xlsx",
					id: "..\\..\\Samples\\EERV Région 9\\id.xlsx"
				);

				System.Diagnostics.Trace.WriteLine ("[" + System.DateTime.Now + "]\tDONE EERV PARISH 2");

				AiderProgram.Test
				(
					coreData: coreData,
					person: "..\\..\\Samples\\EERV Région 9\\9030\\person.xlsx",
					activity: "..\\..\\Samples\\EERV Région 9\\9030\\activity.xlsx",
					group: "..\\..\\Samples\\EERV Région 9\\9030\\group.xlsx",
					supergroup: "..\\..\\Samples\\EERV Région 9\\9030\\supergroup.xlsx",
					id: "..\\..\\Samples\\EERV Région 9\\id.xlsx"
				);

				System.Diagnostics.Trace.WriteLine ("[" + System.DateTime.Now + "]\tDONE EERV PARISH 3");

				AiderProgram.Test
				(
					coreData: coreData,
					person: "..\\..\\Samples\\EERV Région 9\\9040\\person1.xlsx",
					activity: null,
					group: null,
					supergroup: null,
					id: "..\\..\\Samples\\EERV Région 9\\id.xlsx"
				);

				System.Diagnostics.Trace.WriteLine ("[" + System.DateTime.Now + "]\tDONE EERV PARISH 4A");

				AiderProgram.Test
				(
					coreData: coreData,
					person: "..\\..\\Samples\\EERV Région 9\\9040\\person2.xlsx",
					activity: null,
					group: null,
					supergroup: null,
					id: "..\\..\\Samples\\EERV Région 9\\id.xlsx"
				);

				System.Diagnostics.Trace.WriteLine ("[" + System.DateTime.Now + "]\tDONE EERV PARISH 4B");

				AiderProgram.Test
				(
					coreData: coreData,
					person: "..\\..\\Samples\\EERV Région 9\\9050\\person.xlsx",
					activity: "..\\..\\Samples\\EERV Région 9\\9050\\activity.xlsx",
					group: "..\\..\\Samples\\EERV Région 9\\9050\\group.xlsx",
					supergroup: "..\\..\\Samples\\EERV Région 9\\9050\\supergroup.xlsx",
					id: "..\\..\\Samples\\EERV Région 9\\id.xlsx"
				);

				System.Diagnostics.Trace.WriteLine ("[" + System.DateTime.Now + "]\tDONE EERV PARISH 5");

				AiderProgram.Test
				(
					coreData: coreData,
					person: "..\\..\\Samples\\EERV Ale 31\\person.xlsx",
					activity: "..\\..\\Samples\\EERV Ale 31\\activity.xlsx",
					group: "..\\..\\Samples\\EERV Ale 31\\group.xlsx",
					supergroup: "..\\..\\Samples\\EERV Ale 31\\supergroup.xlsx",
					id: "..\\..\\Samples\\EERV Ale 31\\id.xlsx"
				);

				System.Diagnostics.Trace.WriteLine ("[" + System.DateTime.Now + "]\tDONE EERV ALE");
			}
		}

		private static void Test(CoreData coreData, string person, string activity, string group, string supergroup, string id)
		{
			var eervPersonsFile = new FileInfo (person);
			var eervActivityFile = activity == null ? null : new FileInfo (activity);
			var eervGroupFile = group == null ? null : new FileInfo (group);
			var eervSuperGroupFile = supergroup == null ? null : new FileInfo (supergroup);
			var eervIdFile = new FileInfo (id);

			var eervParishData = new EervParishDataLoader ()
				.LoadEervParishData (eervPersonsFile, eervActivityFile, eervGroupFile, eervSuperGroupFile, eervIdFile)
				.ToList ();

			var parishRepository = ParishAddressRepository.Current;

			foreach (var eervParishDatum in eervParishData)
			{
				EervParishDataImporter.Import (coreData, parishRepository, eervParishDatum);
			}
		}


		private static void CreateUsers(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				AiderProgram.CreateUsers (businessContext);
			}
		}


		private static void CreateUsers(BusinessContext businessContext)
		{
			var users = new List<Tuple<string, string, bool, string, string, Tuple<bool, bool, bool>>> ()
			{
				// Users from EPSITEC
				Tuple.Create ("p.arnaud", "Pierre Arnaud", false, AiderUserRoleEntity.CountyRole, "Yverdon-Fontenay – Les Cygnes", Tuple.Create (true, true, true)),
				Tuple.Create ("m.bettex", "Marc Bettex", false, AiderUserRoleEntity.CountyRole, "Yverdon-Fontenay – Les Cygnes", Tuple.Create (true, true, true)),

				// Users from Region 9
				Tuple.Create ("b.bolay", "Bernard Bolay", false, AiderUserRoleEntity.CountyRole, "Saint-Saphorin", Tuple.Create (false, false, true)),
				Tuple.Create ("e.bornand", "Eric Bornand", false, AiderUserRoleEntity.CountyRole, "Saint-Saphorin", Tuple.Create (false, false, true)),
				Tuple.Create ("g.butticaz", "Geneviève Butticaz", false, AiderUserRoleEntity.CountyRole, "Chavornay", Tuple.Create (false, false, true)),
				Tuple.Create ("b.corbaz", "Benjamin Corbaz", false, AiderUserRoleEntity.CountyRole, "Belmont – Lutry", Tuple.Create (false, false, true)),
				Tuple.Create ("l.dewarrat", "Laurence Dewarrat", false, AiderUserRoleEntity.CountyRole, "Belmont – Lutry", Tuple.Create (false, false, true)),
				Tuple.Create ("m.genoux", "Michel Genoux", false, AiderUserRoleEntity.CountyRole, "Pully – Paudex", Tuple.Create (false, false, true)),
				Tuple.Create ("m.gonce", "Maurice Gonce", false, AiderUserRoleEntity.CountyRole, "Oron – Palézieux", Tuple.Create (false, false, true)),
				Tuple.Create ("c.jackson", "Cheryl Jackson", false, AiderUserRoleEntity.CountyRole, "Villette", Tuple.Create (false, false, true)),
				Tuple.Create ("p.jarne", "Pierrette Jarne", false, AiderUserRoleEntity.CountyRole, "Saint-Saphorin", Tuple.Create (false, false, true)),
				Tuple.Create ("g.jaton", "Gérard Jaton", true, AiderUserRoleEntity.CountyRole, "Savigny – Forel", Tuple.Create (false, false, true)),
				Tuple.Create ("y.knecht", "Yvonne Knecht", false, AiderUserRoleEntity.CountyRole, "Saint-Saphorin", Tuple.Create (false, false, true)),
				Tuple.Create ("v.mennet", "Vanina Mennet", false, AiderUserRoleEntity.CountyRole, "Savigny – Forel", Tuple.Create (false, false, true)),
				Tuple.Create ("l.pestalozzi", "Lorenzo Pestalozzi", false, AiderUserRoleEntity.CountyRole, "Saint-Saphorin", Tuple.Create (false, false, true)),
				Tuple.Create ("d.rochat", "Dorothée Rochat", false, AiderUserRoleEntity.CountyRole, "Pully – Paudex", Tuple.Create (false, false, true)),
				Tuple.Create ("p.rouge", "Pascal Rouge", false, AiderUserRoleEntity.CountyRole, "Villette", Tuple.Create (false, false, true)),
				Tuple.Create ("j.sotornik", "Jeanette Sotornik", false, AiderUserRoleEntity.CountyRole, "Villette", Tuple.Create (false, false, true)),
				Tuple.Create ("j.spothelfer", "Jean-Marc Spothelfer", false, AiderUserRoleEntity.CountyRole, "Pully – Paudex", Tuple.Create (false, true, true)),
				Tuple.Create ("s.wohlhauser", "Sylvie Wohlhauser", false, AiderUserRoleEntity.CountyRole, "Belmont – Lutry", Tuple.Create (false, false, true)),

				// Users from Ale 31
				Tuple.Create("m.aubert", "Marie-Noëlle Aubert", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("s.bagaini", "Sylviane Bagaini", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("n.besson", "Nicolas Besson", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("a.bovay", "Anouk Bovay", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create ("j.brand", "Jacques Brand", true, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("c.cherpillod", "Corinne Cherpillod", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("j.christin", "John Christin", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("m.cohen", "Michèle Cohen", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create ("c.cuendet", "Claude Cuendet", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("l.depraz", "Line Dépraz", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("m.durussel", "Michel Durussel", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("g.dutoit", "Gabriel Dutoit", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("m.eggimann", "Magda Eggimann", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("o.favrod", "Olivier Favrod", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create ("d.fankhauser", "Damaris Fankhauser", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("e.gaillard", "Esther Gaillard", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("p.gilgien", "Pascale Gilgien", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("j.habermacher", "Jean-François Habermacher", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("p.haesslein", "Patrice Haesslein", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("b.jacquiard", "Bernadette Jacquiard", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("r.jurenak", "René Jurenak", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("g.labarraque", "Guy Labarraque", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("p.mariani", "Paolo Mariani", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("a.michel", "Agnès Michel", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("m.mouthon", "Marie-Gabrielle Mouthon", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("l.nibbio", "Laurence Nibbio", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("x.paillard", "Xavier Paillard", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("a.perrenoud", "André Perrenoud", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("r.piccard", "Renaud Piccard", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("r.pingoud", "Rémy Pingoud", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("m.rapin", "Marianne Rapin", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("j.savary", "Jean-Marc Savary", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create ("j.sordet", "Jean-Michel Sordet", true, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("v.volet", "Vincent Volet", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
				Tuple.Create("l.zumstein", "Laurent Zumstein", false, AiderUserRoleEntity.AleRole, "", Tuple.Create (true, false, false)),
			};

			var parishRepository = ParishAddressRepository.Current;

			foreach (var user in users)
			{
				AiderProgram.CreateUser (businessContext, parishRepository, user.Item1, user.Item2, user.Item3, user.Item4, user.Item5, user.Item6);
			}
		}


		private static void CreateUser(BusinessContext businessContext, ParishAddressRepository parishRepository, string login, string name, bool isAdmin, string roleName, string parish, Tuple<bool, bool, bool> enableGroupEdition)
		{
			var user = businessContext.CreateAndRegisterEntity<AiderUserEntity> ();

			user.DisplayName = name;
			user.LoginName = login;
			user.SetPassword ("monsupermotdepasse");
			user.CustomUISettings = businessContext.CreateAndRegisterEntity<SoftwareUISettingsEntity> ();
			user.EnableGroupEditionCanton = enableGroupEdition.Item1;
			user.EnableGroupEditionRegion = enableGroupEdition.Item2;
			user.EnableGroupEditionParish = enableGroupEdition.Item3;

			if (isAdmin)
			{
				user.AssignGroup (businessContext, UserPowerLevel.Administrator);
			}

			user.Role = AiderUserRoleEntity.GetRole (businessContext, roleName);

			if (!string.IsNullOrEmpty (parish))
			{
				user.Parish = ParishAssigner.FindParishGroup (businessContext, parishRepository, parish);
			}
		}
	}
}