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
	public static class AiderProgram
	{
		public static void Main(string[] args)
		{
			if (args.Length >= 1)
			{
				if (args.Contains ("-echimportation"))
				{
					AiderProgram.RunEchImportation (args);
					return;
				}

				if (args.Contains ("-eervmainimportation"))
				{
					AiderProgram.RunEervMainImportation (args);
					return;
				}

				if (args.Contains ("-eervparishimportation"))
				{
					AiderProgram.RunEervParishImportation (args);
					return;
				}
			}

			AiderProgram.RunNormalMode (args);
		}

		private static void RunEchImportation(string[] args)
		{
			AiderProgram.RunWithCoreDataManager (coreDataManager =>
			{
				var eChDataFile = AiderProgram.GetFile (args, "-echfile:");
				
				var eChReportedPersons = EChDataLoader.Load (eChDataFile);
				
				EChDataImporter.Import (coreDataManager, eChReportedPersons);
			});
		}

		private static void RunEervMainImportation(string[] args)
		{
			AiderProgram.RunWithCoreDataManager (coreDataManager =>
			{
				var eervGroupDefinitionFile = AiderProgram.GetFile (args, "-groupdefinitionfile:");
				
				var eervMainData = EervMainDataLoader.LoadEervData (eervGroupDefinitionFile);
				var parishRepository = ParishAddressRepository.Current;

				EervMainDataImporter.Import (coreDataManager, eervMainData, parishRepository);
			});
		}

		private static void RunEervParishImportation(string[] args)
		{
			AiderProgram.RunWithCoreDataManager (coreDataManager =>
			{
				var eervGroupDefinitionFile = AiderProgram.GetFile (args, "-groupdefinitionfile:");
				var eervPersonsFile = AiderProgram.GetFile (args, "-personfile:");
				var eervActivityFile = AiderProgram.GetFile (args, "-activityfile:");
				var eervGroupFile = AiderProgram.GetFile (args, "-groupfile:");
				var eervSuperGroupFile = AiderProgram.GetFile (args, "-supergroupfile:");
				var eervIdFile = AiderProgram.GetFile (args, "-idfile:");

				var eervMainData = EervMainDataLoader.LoadEervData (eervGroupDefinitionFile);
				var eervParishData = EervParishDataLoader.LoadEervParishData (eervPersonsFile, eervActivityFile, eervGroupFile, eervSuperGroupFile, eervIdFile).ToList ();
				var parishRepository = ParishAddressRepository.Current;

				foreach (var eervParishDatum in eervParishData)
				{
					EervParishDataImporter.Import (coreDataManager, parishRepository, eervMainData, eervParishDatum);
				}
			});
		}

		private static void RunWithCoreDataManager(Action<CoreDataManager> action)
		{
			SwissPost.Initialize ();
			CoreContext.ParseOptionalSettingsFile (CoreContext.ReadCoreContextSettingsFile ());
			CoreContext.StartAsInteractive ();
			Services.Initialize ();

			using (var application = new CoreApplication ())
			{
				application.SetupApplication ();

				var coreDataManager = new CoreDataManager (application.Data);

				action (coreDataManager);

				Services.ShutDown ();
			}
		}

		private static FileInfo GetFile(string[] args, string key)
		{
			foreach (var arg in args)
			{
				if (arg.StartsWith (key))
				{
					var path = arg.Substring (key.Length);

					return new FileInfo (path);
				}
			}

			throw new Exception ("Argument " + key + " is missing!");
		}

		private static void RunNormalMode(string[] args)
		{
			SwissPost.Initialize ();
			CoreProgram.Main (args);
		}
	}
}