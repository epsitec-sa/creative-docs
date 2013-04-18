﻿//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Data.ECh;
using Epsitec.Aider.Data.Eerv;
using Epsitec.Aider.Data.Subscription;

using Epsitec.Common.Debug;

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
		public static void Main(string[] args)
		{
			AiderProgram.SetupExceptionCatcher ();
			AiderProgram.Run (args);
		}

		private static void SetupExceptionCatcher()
		{
#if !DEBUG
			GeneralExceptionCatcher.Setup ();
			GeneralExceptionCatcher.AbortOnException = true;
			GeneralExceptionCatcher.AddExceptionHandler (e => ErrorLogger.LogException (e));
#endif
		}

		private static void Run(string[] args)
		{
			if (args.Length >= 1)
			{
				if (args.Contains ("-testfullimport"))
				{
					AiderProgramTestImportMode mode = AiderProgramTestImportMode.Default;

					if (args.Contains ("-echonly"))
					{
						mode |= AiderProgramTestImportMode.EchOnly;
					}
					if (args.Contains ("-subset"))
					{
						mode |= AiderProgramTestImportMode.Subset;
					}

					AiderProgram.TestFullImport (mode);
					return;
				}

				if (args.Contains ("-testpostmatch"))
				{
					Tests.TestPostMatch.TestMatchStreet ();
					return;
				}

				if (args.Contains ("-analyzeparishfile"))
				{
					AiderProgram.AnalyzeParishFile (args);
					return;
				}

				if (args.Contains ("-analyzeechfile"))
				{
					AiderProgram.AnalyzeEchFile (args);
					return;
				}

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

				if (args.Contains ("-generatesubscriptions"))
				{
					AiderProgram.RunSubscriptionGeneration (args);
					return;
				}
			}

			AiderProgram.RunNormalMode (args);
		}

		private static void RunEchImportation(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				var eChDataFile = AiderProgram.GetFile (args, "-echfile:", true);
				var mode = AiderProgram.GetString (args, "-mode:", false);

				var maxCount = AiderProgram.GetMaxCount (mode);
				var eChReportedPersons = EChDataLoader.Load (eChDataFile, maxCount);

				var parishRepository = ParishAddressRepository.Current;

				EChDataImporter.Import (coreData, parishRepository, eChReportedPersons);
			});
		}

		private static int GetMaxCount(string mode)
		{
			if (mode == "full" || mode == null)
			{
				return int.MaxValue;
			}
			
			if (mode == "partial")
			{
				return 1000;
			}
			
			throw new Exception ("Invalid mode");
		}

		private static void RunEervMainImportation(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				var eervGroupDefinitionFile = AiderProgram.GetFile (args, "-groupdefinitionfile:", true);
				
				var eervMainData = EervMainDataLoader.LoadEervData (eervGroupDefinitionFile);
				var parishRepository = ParishAddressRepository.Current;

				EervMainDataImporter.Import (coreData, eervMainData, parishRepository);
			});
		}

		private static void RunEervParishImportation(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				var eervPersonsFile = AiderProgram.GetFile (args, "-personfile:", true);
				var eervActivityFile = AiderProgram.GetFile (args, "-activityfile:", false);
				var eervGroupFile = AiderProgram.GetFile (args, "-groupfile:", false);
				var eervSuperGroupFile = AiderProgram.GetFile (args, "-supergroupfile:", false);
				var eervIdFile = AiderProgram.GetFile (args, "-idfile:", true);
				var loadOnly = AiderProgram.GetBool (args, "-loadOnly:", false);

				var eervParishData = new EervParishDataLoader ()
					.LoadEervParishData (eervPersonsFile, eervActivityFile, eervGroupFile, eervSuperGroupFile, eervIdFile)
					.ToList ();

				if (!loadOnly.GetValueOrDefault ())
				{
					var parishRepository = ParishAddressRepository.Current;

					foreach (var eervParishDatum in eervParishData)
					{
						EervParishDataImporter.Import (coreData, parishRepository, eervParishDatum);
					}
				}
			});
		}

		private static void RunSubscriptionGeneration(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				var parishRepository = ParishAddressRepository.Current;

				SubscriptionGenerator.Create (coreData, parishRepository);
			});
		}

		private static void AnalyzeParishFile(string[] args)
		{
			var input = AiderProgram.GetFile (args, "-input:", true);
			var output = AiderProgram.GetFile (args, "-output:", true);

			Tests.ParishFileAnalyzer.Analyze (input, output);
		}

		private static void AnalyzeEchFile(string[] args)
		{
			var input = AiderProgram.GetFile (args, "-input:", true);

			Tests.EChFileAnalyzer.Analyze (input);
		}

		private static void RunWithCoreData(Action<CoreData> action)
		{
			SwissPost.Initialize ();
			CoreContext.ParseOptionalSettingsFile (CoreContext.ReadCoreContextSettingsFile ());
			CoreContext.StartAsInteractive ();
			Services.Initialize ();

			using (var application = new CoreApplication ())
			{
				application.SetupApplication ();

				action (application.Data);

				Services.ShutDown ();
			}
		}

		private static FileInfo GetFile(string[] args, string key, bool mandatory)
		{
			var path = AiderProgram.GetString (args, key, mandatory);

			return path != null
				? new FileInfo (path)
				: null;
		}

		private static bool? GetBool(string[] args, string key, bool mandatory)
		{
			var value = AiderProgram.GetString (args, key, mandatory);

			return value != null
				? (bool?) bool.Parse (value)
				: (bool?) null;
		}

		private static string GetString(string[] args, string key, bool mandatory)
		{
			foreach (var arg in args)
			{
				if (arg.StartsWith (key))
				{
					return arg.Substring (key.Length);
				}
			}

			if (mandatory)
			{
				throw new Exception ("Argument " + key + " is missing!");
			}

			return null;
		}

		private static void RunNormalMode(string[] args)
		{
			SwissPost.Initialize ();
			CoreProgram.Main (args);
		}
	}
}