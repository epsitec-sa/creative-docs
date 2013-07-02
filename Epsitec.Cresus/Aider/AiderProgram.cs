//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Data.ECh;
using Epsitec.Aider.Data.Eerv;
using Epsitec.Aider.Data.Job;
using Epsitec.Aider.Data.Subscription;

using Epsitec.Common.Debug;
using Epsitec.Common.IO;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Library.UI;

using Epsitec.Data.Platform;

using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		[Conditional ("RELEASE")]
		private static void SetupExceptionCatcher()
		{
			GeneralExceptionCatcher.Setup ();
			GeneralExceptionCatcher.AbortOnException = true;
			GeneralExceptionCatcher.AddExceptionHandler (e => ErrorLogger.LogException (e));
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

				if (args.Contains ("-echdownload"))						//	-echdownload -echdir:S:\eerv -echdelete:true
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.DownloadEchData (args));
					return;
				}

				if (args.Contains ("-echimportation"))
				{
					AiderProgram.RunEchImportation (args);
					return;
				}

				if (args.Contains ("-echupdate"))						//  -echupdate -newechfile:s:\eerv-new.xml -oldechfile:s:\eerv-old.xml -output:s:\analyse.md
				{
					AiderProgram.RunEchUpdate (args);
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

				if (args.Contains ("-importsubscriptions"))
				{
					AiderProgram.RunSubscriptionImportation (args);
					return;
				}

				if (args.Contains ("-exportsubscriptions"))				//	-exportsubscriptions -output:Q:\output.txt -error:Q:\error.log
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.RunSubscriptionExportation (args));
					return;
				}

				if (args.Contains ("-testmatchsort"))					//	-testmatchsort -input:Q:\error.log
				{
					//	Analyzes the log file produced by an address export based on MAT[CH]sort
					//	for the "Bonne Nouvelle" journal.

					ConsoleCreator.RunWithConsole (() => Tests.TestMatchSort.AnalyzeLogs (AiderProgram.GetFile (args, "-input:", true)));
					return;
				}

				if (args.Contains ("-uploadsubscriptions"))				//	-uploadsubscriptions -input:Q:\output.txt -publicationdate:2013-06-27 -response:Q:\tamedia.log
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.UploadSubscriptionExportation (args));
					return;
				}

				if (args.Contains ("-fixparticipations"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FixParticipations (args));
					return;
				}

				if (args.Contains ("-fixambiguousaddresses"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FixAmbiguousAddresses (args));
					return;
				}

				if (args.Contains ("-fixparishassignations"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FixParishAssignation (args));
					return;
				}

				if (args.Contains ("-fixcontactnames"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FixContactNames (args));
					return;
				}

				if (args.Contains ("-fixchardonnesubscriptions"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FixChardonneSubscriptions (args));
					return;
				}
			}

			AiderProgram.RunNormalMode (args);
		}

		private static void UploadSubscriptionExportation(string[] args)
		{
			var outputFile = AiderProgram.GetFile (args, "-input:", true);
			var responseFile = AiderProgram.GetFile (args, "-response:", false);
			var date = AiderProgram.GetDate (args, "-publicationdate:");

			SubscriptionUploader.FtpUploadFile (outputFile, responseFile, date);
		}

		private static void DownloadEchData(string[] args)
		{
			var eChDataDirPath  = AiderProgram.GetString (args, "-echdir:", true);
			var eChDataFileName = AiderProgram.GetString (args, "-echfile:", false);

			var download = EchDataDownloader.Download (eChDataDirPath, eChDataFileName);

			if (AiderProgram.GetBool (args, "-echdelete:", false).GetValueOrDefault ())
			{
				download.DeleteDownloadedFilesButLast ();
			}
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

		private static void RunEchUpdate(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				var newEChDataFile = AiderProgram.GetFile (args, "-newechfile:", true);
				var oldEChDataFile = AiderProgram.GetFile (args, "-oldechfile:", true);
				var reportFile = AiderProgram.GetFile(args, "-output:", true);
				UpdateEChData.StartJob (oldEChDataFile.FullName, newEChDataFile.FullName,reportFile.FullName, coreData);

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
			var subscribeHouseholds = AiderProgram.GetBool (args, "-households:", false, false);
			var subscribeLegalPersons = AiderProgram.GetBool (args, "-legalpersons:", false, false);

			AiderProgram.RunWithCoreData (coreData =>
			{
				var parishRepository = ParishAddressRepository.Current;

				SubscriptionGenerator.Create
				(
					coreData, parishRepository, subscribeHouseholds, subscribeLegalPersons
				);
			});
		}

		private static void RunSubscriptionImportation(string[] args)
		{
			var fileWeb = AiderProgram.GetFile (args, "-web:", true);
			var fileDoctor = AiderProgram.GetFile (args, "-doctor:", true);
			var filePro = AiderProgram.GetFile (args, "-pro:", true);

			var subscriptions = SubscriptionDataLoader.LoadSubscriptions
			(
				fileWeb, fileDoctor, filePro
			);

			var parishRepository = ParishAddressRepository.Current;

			AiderProgram.RunWithCoreData (coreData =>
			{
				SubscriptionDataImporter.Import (coreData, parishRepository, subscriptions);
			});
		}

		private static void RunSubscriptionExportation(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				var outputFile = AiderProgram.GetFile (args, "-output:", true);
				var errorFile = AiderProgram.GetFile (args, "-error:", false);

				var writer = new SubscriptionFileWriter (coreData, outputFile, errorFile, true);

				writer.Write ();
			});
		}


		private static void FixParticipations(string[] args)
		{
			AiderProgram.RunWithCoreData (ParticipationFixer.FixParticipations);
		}

		private static void FixAmbiguousAddresses(string[] args)
		{
			var streetRepository = SwissPostStreetRepository.Current;

			var eChDataFile = AiderProgram.GetFile (args, "-echfile:", true);
			var echReportedPersons = EChDataLoader.Load (eChDataFile);

			AiderProgram.RunWithCoreData (c => AmbiguousAddressFixer.FixAmbiguousAddresses (streetRepository, echReportedPersons, c));
		}

		private static void FixParishAssignation(string[] args)
		{
			var parishRepository = ParishAddressRepository.Current;

			AiderProgram.RunWithCoreData
			(
				coreData => ParishAssignationFixer.FixParishAssignations
				(
					parishRepository, coreData
				)
			);
		}

		private static void FixContactNames(string[] args)
		{
			AiderProgram.RunWithCoreData
			(
				coreData => ContactNameFixer.FixContactNames (coreData)
			);
		}

		private static void FixChardonneSubscriptions(string[] args)
		{
			AiderProgram.RunWithCoreData
			(
				coreData => ChardonneSubscriptionFixer.FixChardonneSubscriptions (coreData)
			);
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

		private static bool GetBool(string[] args, string key, bool mandatory, bool defaultValue)
		{
			return AiderProgram.GetBool (args, key, mandatory) ?? defaultValue;
		}

		private static bool? GetBool(string[] args, string key, bool mandatory)
		{
			var value = AiderProgram.GetString (args, key, mandatory);

			return value != null
				? (bool?) bool.Parse (value)
				: (bool?) null;
		}

		private static Date GetDate(string[] args, string key)
		{
			var value = AiderProgram.GetString (args, key, true);

			return new Date (DateTime.Parse (value));
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