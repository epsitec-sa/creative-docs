//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data;
using Epsitec.Aider.Data.ECh;
using Epsitec.Aider.Data.Eerv;
using Epsitec.Aider.Tools;

using Epsitec.Cresus.Core;
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

				var coreDataManager = new CoreDataManager (application.Data);

				var guid = Guid.NewGuid ().ToString ();
				Directory.CreateDirectory ("C:\\ProgramData\\Epsitec\\Firebird Databases\\" + guid);

				System.Diagnostics.Debug.WriteLine ("[" + System.DateTime.Now + "]\tSTART");
				File.Copy ("C:\\ProgramData\\Epsitec\\Firebird Databases\\AIDER.FIREBIRD", "C:\\ProgramData\\Epsitec\\Firebird Databases\\" + guid + "\\AIDER-IMPORT-1.FIREBIRD");

				var eervGroupDefinitionFile = new FileInfo ("S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Main\\Groupe definition.xlsx");
				var eervMainData = EervMainDataLoader.LoadEervData (eervGroupDefinitionFile);
				var parishRepository = ParishAddressRepository.Current;
				EervMainDataImporter.Import (coreDataManager, eervMainData, parishRepository);

				System.Diagnostics.Debug.WriteLine ("[" + System.DateTime.Now + "]\tDONE EERV MAIN");
				File.Copy ("C:\\ProgramData\\Epsitec\\Firebird Databases\\AIDER.FIREBIRD", "C:\\ProgramData\\Epsitec\\Firebird Databases\\" + guid + "\\AIDER-IMPORT-2.FIREBIRD");

				var eChDataFile = new FileInfo ("S:\\Epsitec.Cresus\\App.Aider\\Samples\\eerv.xml");
				var eChReportedPersons = EChDataLoader.Load (eChDataFile, importMode.HasFlag (AiderProgramTestImportMode.Subset) ? 2000 : int.MaxValue);
				EChDataImporter.Import (coreDataManager, parishRepository, eChReportedPersons);

				System.Diagnostics.Debug.WriteLine ("[" + System.DateTime.Now + "]\tDONE ECH");
				File.Copy ("C:\\ProgramData\\Epsitec\\Firebird Databases\\AIDER.FIREBIRD", "C:\\ProgramData\\Epsitec\\Firebird Databases\\" + guid + "\\AIDER-IMPORT-3.FIREBIRD");

				if (importMode.HasFlag (AiderProgramTestImportMode.EchOnly))
				{
					return;
				}

				AiderProgram.Test
				(
					coreDataManager: coreDataManager,
					groupDef: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Main\\Groupe definition.xlsx",
					person: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9010\\person.xlsx",
					activity: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9010\\activity.xlsx",
					group: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9010\\group.xlsx",
					supergroup: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9010\\supergroup.xlsx",
					id: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9010\\id.xlsx"
				);

				System.Diagnostics.Debug.WriteLine ("[" + System.DateTime.Now + "]\tDONE EERV PARISH 1");
				File.Copy ("C:\\ProgramData\\Epsitec\\Firebird Databases\\AIDER.FIREBIRD", "C:\\ProgramData\\Epsitec\\Firebird Databases\\" + guid + "\\AIDER-IMPORT-4.FIREBIRD");

				AiderProgram.Test
				(
					coreDataManager: coreDataManager,
					groupDef: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Main\\Groupe definition.xlsx",
					person: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9020\\person.xlsx",
					activity: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9020\\activity.xlsx",
					group: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9020\\group.xlsx",
					supergroup: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9020\\supergroup.xlsx",
					id: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9020\\id.xlsx"
				);

				System.Diagnostics.Debug.WriteLine ("[" + System.DateTime.Now + "]\tDONE EERV PARISH 2");
				File.Copy ("C:\\ProgramData\\Epsitec\\Firebird Databases\\AIDER.FIREBIRD", "C:\\ProgramData\\Epsitec\\Firebird Databases\\" + guid + "\\AIDER-IMPORT-5.FIREBIRD");

				AiderProgram.Test
				(
					coreDataManager: coreDataManager,
					groupDef: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Main\\Groupe definition.xlsx",
					person: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9030\\person.xlsx",
					activity: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9030\\activity.xlsx",
					group: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9030\\group.xlsx",
					supergroup: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9030\\supergroup.xlsx",
					id: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9030\\id.xlsx"
				);

				System.Diagnostics.Debug.WriteLine ("[" + System.DateTime.Now + "]\tDONE EERV PARISH 3");
				File.Copy ("C:\\ProgramData\\Epsitec\\Firebird Databases\\AIDER.FIREBIRD", "C:\\ProgramData\\Epsitec\\Firebird Databases\\" + guid + "\\AIDER-IMPORT-6.FIREBIRD");

				AiderProgram.Test
				(
					coreDataManager: coreDataManager,
					groupDef: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Main\\Groupe definition.xlsx",
					person: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9040\\person1.xlsx",
					activity: null,
					group: null,
					supergroup: null,
					id: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9040\\id.xlsx"
				);

				System.Diagnostics.Debug.WriteLine ("[" + System.DateTime.Now + "]\tDONE EERV PARISH 4A");
				File.Copy ("C:\\ProgramData\\Epsitec\\Firebird Databases\\AIDER.FIREBIRD", "C:\\ProgramData\\Epsitec\\Firebird Databases\\" + guid + "\\AIDER-IMPORT-7.FIREBIRD");

				AiderProgram.Test
				(
					coreDataManager: coreDataManager,
					groupDef: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Main\\Groupe definition.xlsx",
					person: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9040\\person2.xlsx",
					activity: null,
					group: null,
					supergroup: null,
					id: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9040\\id.xlsx"
				);

				System.Diagnostics.Debug.WriteLine ("[" + System.DateTime.Now + "]\tDONE EERV PARISH 4B");
				File.Copy ("C:\\ProgramData\\Epsitec\\Firebird Databases\\AIDER.FIREBIRD", "C:\\ProgramData\\Epsitec\\Firebird Databases\\" + guid + "\\AIDER-IMPORT-8.FIREBIRD");

				AiderProgram.Test
				(
					coreDataManager: coreDataManager,
					groupDef: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Main\\Groupe definition.xlsx",
					person: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9050\\person.xlsx",
					activity: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9050\\activity.xlsx",
					group: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9050\\group.xlsx",
					supergroup: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9050\\supergroup.xlsx",
					id: "S:\\Epsitec.Cresus\\App.Aider\\Samples\\EERV Région 9\\9050\\id.xlsx"
				);

				System.Diagnostics.Debug.WriteLine ("[" + System.DateTime.Now + "]\tDONE EERV PARISH 5");
				File.Copy ("C:\\ProgramData\\Epsitec\\Firebird Databases\\AIDER.FIREBIRD", "C:\\ProgramData\\Epsitec\\Firebird Databases\\" + guid + "\\AIDER-IMPORT-9.FIREBIRD");
			}
		}

		private static void Test(CoreDataManager coreDataManager, string groupDef, string person, string activity, string group, string supergroup, string id)
		{
			var eervGroupDefinitionFile = new FileInfo (groupDef);
			var eervPersonsFile = new FileInfo (person);
			var eervActivityFile = activity == null ? null : new FileInfo (activity);
			var eervGroupFile = group == null ? null : new FileInfo (group);
			var eervSuperGroupFile = supergroup == null ? null : new FileInfo (supergroup);
			var eervIdFile = new FileInfo (id);

			var eervMainData = EervMainDataLoader.LoadEervData (eervGroupDefinitionFile);

			var eervParishData = new EervParishDataLoader (true)
				.LoadEervParishData (eervPersonsFile, eervActivityFile, eervGroupFile, eervSuperGroupFile, eervIdFile)
				.ToList ();

			var parishRepository = ParishAddressRepository.Current;

			foreach (var eervParishDatum in eervParishData)
			{
				EervParishDataImporter.Import (coreDataManager, parishRepository, eervMainData, eervParishDatum);
			}
		}
	}
}