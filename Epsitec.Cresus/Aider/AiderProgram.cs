﻿//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Data.ECh;
using Epsitec.Aider.Data.Eerv;
using Epsitec.Aider.Data.Job;
using Epsitec.Aider.Data.Groups;
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
					ConsoleCreator.RunWithConsole (() => AiderProgram.RunEchImportation (args));
					return;
				}
																		
				if (args.Contains ("-echupdate"))						//  -echupdate -newechfile:s:\eerv\last.xml -oldechfile:s:\eerv\initial.xml -output:s:\eerv\analyse.md
				{														//  use -v for verbose logging
					ConsoleCreator.RunWithConsole (() => AiderProgram.RunEchUpdate (args));
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

				if (args.Contains ("-initofficemanagement"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.InitOfficeManagement (args) );
					return;
				}

				if (args.Contains ("-initdefaultemployee"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.InitDefaultEmployee (args));
					return;
				}

				if (args.Contains ("-initevents"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.InitEvents (args));
					return;
				}

				if (args.Contains ("-populateusergroups"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.PopulateUserGroups (args));
					return;
				}

				if (args.Contains ("-initfunctions"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.InitFunctions (args));
					return;
				}			

				if (args.Contains ("-generatesubscriptions"))
				{
					AiderProgram.RunSubscriptionGeneration (args);
					return;
				}

				if (args.Contains ("-importsubscriptions"))				//	-importsubscriptions -web:xxx -doctor:xxx -pro:xxx -generic:xxx
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.RunSubscriptionImportation (args));
					return;
				}

				if (args.Contains ("-importsubscriptions2"))				//	-importsubscriptions2 -file:xxx
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.RunSubscriptionImportation2 (args));
					return;
				}

				if (args.Contains ("-exportcontacts"))					//	-exportcontacts -output-rch:xxx -output-custom:xxx
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.RunContactExportation (args));
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

				if (args.Contains ("-clearwarnings"))				//	-clearwarnings -beforedate:2014-01-01 -killpersons:true -createsubscriptions:true
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.ClearWarnings (args));
					return;
				}

				if (args.Contains ("-fixparticipations"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FixParticipations (args));
					return;
				}

				if (args.Contains ("-hotfixderogations"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.HotfixDerogations (args));
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

				if (args.Contains ("-fixnoparish"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FixNoParish (args));
					return;
				}

				if (args.Contains ("-fixcontactnames"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FixContactNames (args));
					return;
				}

				if (args.Contains ("-calculateage")) // must be run 1 times/year
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.CalculateAge (args));
					return;
				}

				if (args.Contains ("-fixpersonswithoutcontact"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FixPersonsWithoutContact (args));
					return;
				}

				if (args.Contains ("-fixchardonnesubscriptions"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FixChardonneSubscriptions (args));
					return;
				}

				if (args.Contains ("-fixprillysubscriptions"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FixPrillySubscriptions (args));
					return;
				}

				if (args.Contains ("-fixsubscriptionsparishgroup"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FixSubscriptionsParishGroup (args));
					return;
				}

				if (args.Contains ("-fixduplicatesubscriptions"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FixDuplicateSubscriptions (args));
					return;
				}

				if (args.Contains("-fixwarningparishgroup"))
				{
					ConsoleCreator.RunWithConsole(() => AiderProgram.FixWarningParishGroup(args));
					return;
				}

				if (args.Contains ("-fixzombies"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FixZombies(args));
					return;
				}

				if (args.Contains ("-fixzombiesechstatus"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FixZombiesEChStatus (args));
					return;
				}

				if (args.Contains ("-createmissingsubscriptions"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.CreateMissingSubscriptionsOrWarnHousehold (args));
					return;
				}

				if (args.Contains ("-warnhouseholdwithnosubscription"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.WarnHouseholdWithNoSubscription (args));
					return;
				}

				if (args.Contains ("-fixechpersons")) // -fixechpersons -echfile:s:\eerv\last.xml 
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FixEChPersons (args));
					return;
				}

				if (args.Contains ("-fixoldechwarnings"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FixOldEChWarnings (args));
					return;
				}

				if (args.Contains ("-fixdeparturewarnings"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FixDepartureWarnings (args));
					return;
				}

				if (args.Contains ("-fixmissingsubscriptions"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FixMissingSubscriptions (args));
					return;
				}

				if (args.Contains ("-fixuselessmissingsubscriptions"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FixUselessMissingSubscriptions (args));
					return;
				}

				if (args.Contains ("-findpotentialduplicatedpersons"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FindPotentialDuplicatedPersons (args));
					return;
				}

				if (args.Contains ("-loadelasticsearch"))
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.LoadElasticSearch (args));
					return;
				}

				if (args.Contains ("-fixaddresses")) //-fixaddresses -file:s:\DATA\rchmatched.csv
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FixAddresses (args));
					return;
				}

				if (args.Contains ("-moveaddresses")) //-moveaddresses -file:s:\DATA\rchmatched.csv
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.MoveAddresses (args));
					return;
				}

				if (args.Contains ("-cleardqflags")) //-cleardqflags
				{
					//More info about this command: https://git.epsitec.ch/aider/dataquality/issues/3
					ConsoleCreator.RunWithConsole (
						() => AiderProgram.RunWithCoreData (
							coreData => ClearDataQualityFlags.Run (coreData)
						)
					);
					return;
				}
				
				if (args.Contains ("-flagduplicatedpersons")) //-flagduplicatedpersons
				{
					//More info about this command: https://git.epsitec.ch/aider/dataquality/issues/3
					ConsoleCreator.RunWithConsole (() => AiderProgram.AutoMergeDuplicatedPersons (args));
					return;
				}

				if (args.Contains ("-flagmissinghousehold")) //-flagmissinghousehold
				{
					//More info about this command: https://git.epsitec.ch/aider/dataquality/issues/3
					ConsoleCreator.RunWithConsole (
						() => AiderProgram.RunWithCoreData (
							coreData => MXFlagger.FlagContacts (coreData)
						)
					);
					return;
				}

				if (args.Contains ("-fixdqflags")) //-fixdqflags
				{
					//More info about this command: https://git.epsitec.ch/aider/dataquality/issues/3
					ConsoleCreator.RunWithConsole (
						() => AiderProgram.RunWithCoreData (
							coreData => DataQualityFlagsFixer.Run (coreData)
						)
					);
					return;
				}

				if (args.Contains ("-fixrolecacheparticipations")) //-fixrolecacheparticipations
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FixRoleCacheParticipations (args));
					return;
				}

				if (args.Contains ("-initrolecache")) //-initrolecache
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.InitRoleCache (args));
					return;
				}

				if (args.Contains ("-buildrolecache")) //-buildrolecache
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.BuildRoleCache (args));
					return;
				}

				if (args.Contains ("-purgerolecache")) //-purgerolecache
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.PurgeRoleCache (args));
					return;
				}

				if (args.Contains ("-cleanhouseholds")) //-cleanhouseholds
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.CleanHouseholds (args));
					return;
				}

				if (args.Contains ("-cleanmailingcategories")) //-cleanmailingcategories
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.CleanMailingCategories (args));
					return;
				}

				if (args.Contains ("-setpersonmrmrs")) //-setpersonmrmrs
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.SetPersonMrMrs (args));
					return;
				}

				if (args.Contains ("-townfusionhack")) //-townfusionhack
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.FusionArzier (args));
					return;
				}

				if (args.Contains ("-admin2muni")) //-admin2muni
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.Admin2Muni (args));
					return;
				}

				if (args.Contains ("-updateparishname")) //-updateparishname -current:"Morge" -new:"Morges – Echichens" -pla:false
				{
					ConsoleCreator.RunWithConsole (() => AiderProgram.UpdateParish (args));
					return;
				}

				
			}

			AiderProgram.RunNormalMode (args);
		}

		private static void UploadSubscriptionExportation(string[] args)
		{
			var outputFile = AiderProgram.GetFile (args, "-input:", true);
			var responseFile = AiderProgram.GetFile (args, "-response:");
			var date = AiderProgram.GetDate (args, "-publicationdate:");

			SubscriptionUploader.FtpUploadFile (outputFile, responseFile, date);
		}

		private static void ClearWarnings(string[] args)
		{
			var date		= AiderProgram.GetDate (args, "-beforedate:");
			var killer		= AiderProgram.GetBool (args, "-killpersons:", true ,false);
			var subscriber	= AiderProgram.GetBool (args, "-createsubscriptions:", true, false);
			AiderProgram.RunWithCoreData (coreData =>
			{
				WarningCleaner.ClearWarningsBeforeDate (coreData, date, killer, subscriber);
			});			
		}

		private static void DownloadEchData(string[] args)
		{
			var eChDataDirPath  = AiderProgram.GetString (args, "-echdir:", true);
			var eChDataFileName = AiderProgram.GetString (args, "-echfile:");

			var download = EchDataDownloader.Download (eChDataDirPath, eChDataFileName);

			if (AiderProgram.GetBool (args, "-echdelete:", false).GetValueOrDefault ())
			{
				download.DeleteDownloadedFilesButLast ();
			}
		}

		private static void LoadElasticSearch(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				var job = new ElasticSearchLoader (coreData);

				job.ProcessJob ();

			});
		}

		private static void RunEchImportation(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				var eChDataFile = AiderProgram.GetFile (args, "-echfile:", true);
				var mode = AiderProgram.GetString (args, "-mode:");

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
				var newEChDataFile     = AiderProgram.GetFile (args, "-newechfile:", true);
				var oldEChDataFile     = AiderProgram.GetFile (args, "-oldechfile:", true);
				var reportFile         = AiderProgram.GetFile (args, "-output:", true);
				
				var verboseLogging	   = false;
				if (args.Contains ("-v"))
				{
					verboseLogging = true;
				}
				
				//System.Console.WriteLine ("Running DataQualityJobs before updating...");
				//EChPersonFixer.TryFixAll (coreData);
				//HouseholdsFix.EchHouseholdsQuality (coreData, oldEChDataFile.FullName);

				System.Console.WriteLine ("Running ECh Warning Fixer before updating...");
				EChWarningsFixer.TryFixAll (coreData);

				var parishRepository = ParishAddressRepository.Current;
				var updater = new EChDataUpdater (oldEChDataFile.FullName, newEChDataFile.FullName, reportFile.FullName, coreData, parishRepository,verboseLogging);
				updater.ProcessJob ();
				
				System.Console.WriteLine ("Running ECh Warning Fixer after updating...");
				EChWarningsFixer.TryFixAll (coreData);

				System.Console.WriteLine ("Fixing 'no parish' after updating...");
				ParishAssignationFixer.FixNoParish (coreData);

				//System.Console.WriteLine ("Running DataQualityJobs after updating...");
				//HouseholdsFix.EchHouseholdsQuality (coreData, newEChDataFile.FullName);
				//EChPersonFixer.TryFixAll (coreData);

				System.Console.WriteLine ("Fixing subscriptions after updating...");
				SubscriptionAndRefusalFixer.FixDuplicateSubscriptions (coreData);

				System.Console.WriteLine ("Press RETURN to quit");
				System.Console.ReadLine ();
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

		private static void InitOfficeManagement(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				OfficeManagementEntities.CreateIfNeeded (coreData);
				DerogationGroups.CreateIfNeeded (coreData);
				AiderUsersGroups.InitParishUserGroups (coreData);
				AiderUsersGroups.InitRegionalUserGroups (coreData);
			});
		}

		private static void InitEvents(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				AiderEvents.InitGroupPathCacheIfNeeded (coreData);
				AiderEvents.RebuildMainActorsOnValidatedEvents (coreData);
			});
		}

		private static void InitDefaultEmployee(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				AiderCollaborators.Init (coreData);
			});
		}

		private static void PopulateUserGroups(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				AiderUsersGroups.PopulateUserGroupsWithUsers (coreData);
			});
		}

		private static void InitFunctions(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				AiderGeneralFunctions.ApplyFemininForm (coreData);
			});
		}

		private static void FusionArzier(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				TownFusionHack.FusionArzier (coreData);
			});
		}

		private static void Admin2Muni(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				UpdateLegalPerson.AdminToMunicipality (coreData);
				System.Console.WriteLine ("Press RETURN to quit");
				System.Console.ReadLine ();
			});
		}

		private static void UpdateParish(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				var currentName = AiderProgram.GetString (args, "-current:", mandatory: true);
				var newName     = AiderProgram.GetString (args, "-new:", mandatory: true);
				var isPLA       = AiderProgram.GetBool (args, "-pla:", mandatory: true) ?? false;
				UpdateParishName.Update (coreData, currentName, newName, isPLA);
				System.Console.WriteLine ("Press RETURN to quit");
				System.Console.ReadLine ();
			});
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
				var eervActivityFile = AiderProgram.GetFile (args, "-activityfile:");
				var eervGroupFile = AiderProgram.GetFile (args, "-groupfile:");
				var eervSuperGroupFile = AiderProgram.GetFile (args, "-supergroupfile:");
				var eervIdFile = AiderProgram.GetFile (args, "-idfile:", true);
				var loadOnly = AiderProgram.GetBool (args, "-loadOnly:", false);
				var forcedParishId = AiderProgram.GetString (args, "-forcedparishid:");
				var considerDateOfBirth = AiderProgram.GetBool (args, "-considerdateofbirth", false) ?? true;
				var considerSex = AiderProgram.GetBool (args, "-considersex", false) ?? true;

				var eervParishData = new EervParishDataLoader ()
					.LoadEervParishData (eervPersonsFile, eervActivityFile, eervGroupFile, eervSuperGroupFile, eervIdFile, forcedParishId)
					.ToList ();

				if (!loadOnly.GetValueOrDefault ())
				{
					var parishRepository = ParishAddressRepository.Current;

					foreach (var eervParishDatum in eervParishData)
					{
						EervParishDataImporter.Import (coreData, parishRepository, eervParishDatum, considerDateOfBirth, considerSex);
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

				if (subscribeHouseholds)
				{
					SubscriptionGenerator.SubscribeHouseholds(coreData, parishRepository);
				}

				if (subscribeLegalPersons)
				{
					SubscriptionGenerator.SubscribeLegalPersons (coreData, parishRepository);
				}
			});
		}

		private static void RunSubscriptionImportation(string[] args)
		{
			var fileWeb     = AiderProgram.GetFile (args, "-web:");
			var fileDoctor  = AiderProgram.GetFile (args, "-doctor:");
			var filePro     = AiderProgram.GetFile (args, "-pro:");
			var fileGeneric = AiderProgram.GetFile (args, "-generic:");

			var subscriptions = SubscriptionDataLoader.LoadSubscriptions (fileWeb, fileDoctor, filePro, fileGeneric);

			var parishRepository = ParishAddressRepository.Current;

			AiderProgram.RunWithCoreData (coreData =>
			{
				SubscriptionDataImporter.Import (coreData, parishRepository, subscriptions);
			});
		}

		private static void RunSubscriptionImportation2(string[] args)
		{
			var filePath = AiderProgram.GetFile (args, "-file:");

			var subscriptions = SubscriptionDataLoader.LoadSubscriptions (filePath);

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
				var errorFile = AiderProgram.GetFile (args, "-error:");
				var excludeDistrictError = AiderProgram.GetBool (args, "-exclude-district-error:", false, false);

				var writer = new SubscriptionFileWriter (coreData, outputFile, errorFile, excludeDistrictError);

				writer.Write ();
			});
		}

		private static void RunContactExportation(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				var outputRchFile = AiderProgram.GetFile (args, "-output-rch:", true);
				var outputCustomFile = AiderProgram.GetFile (args, "-output-custom:", true);

				var writer = new ContactFileWriter (coreData, outputRchFile, outputCustomFile);

				writer.Write ();
			});
		}


		private static void FixParticipations(string[] args)
		{
			AiderProgram.RunWithCoreData (ParticipationFixer.FixParticipations);
		}

		private static void HotfixDerogations(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData => DerogationHotfix.Hotfix (coreData));
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

			AiderProgram.RunWithCoreData (coreData => ParishAssignationFixer.FixParishAssignations (parishRepository, coreData));
		}

		private static void FixNoParish(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData => ParishAssignationFixer.FixNoParish (coreData));
		}

		private static void FixContactNames(string[] args)
		{
			AiderProgram.RunWithCoreData
			(
				coreData => ContactNameFixer.FixContactNames (coreData)
			);
		}

		private static void CalculateAge(string[] args)
		{
			AiderProgram.RunWithCoreData
			(
				coreData => AgeCalculator.Start (coreData)
			);
		}

		private static void FixChardonneSubscriptions(string[] args)
		{
			AiderProgram.RunWithCoreData
			(
				coreData => ChardonneSubscriptionFixer.FixChardonneSubscriptions (coreData)
			);
		}

		private static void FixPrillySubscriptions(string[] args)
		{
			AiderProgram.RunWithCoreData
			(
				coreData => PrillySubscriptionFixer.FixPrillySubscriptions (coreData)
			);
		}

		private static void FixDuplicateSubscriptions(string[] args)
		{
			AiderProgram.RunWithCoreData
			(
				coreData => SubscriptionAndRefusalFixer.FixDuplicateSubscriptions (coreData)
			);
		}

		private static void FixSubscriptionsParishGroup(string[] args)
		{
			AiderProgram.RunWithCoreData
			(
				coreData => SubscriptionAndRefusalFixer.FixParishGroupPath (coreData)
			);
		}

		private static void FixWarningParishGroup(string[] args)
		{
			AiderProgram.RunWithCoreData
			(
				coreData => WarningParishGroupPathFixer.StartJob(coreData)
			);
		}

		private static void FixEChPersons(string[] args)
		{
			AiderProgram.RunWithCoreData
			(
				coreData =>
				{
					var echFilePath = AiderProgram.GetString (args, "-echfile:", true);
					EChPersonFixer.FixHiddenPersons (coreData, echFilePath);
					EChPersonFixer.TryFixAll (coreData);
				}
			);
		}

		private static void FixPersonsWithoutContact(string[] args)
		{
			AiderProgram.RunWithCoreData
			(
				coreData => PersonsWithoutContactFixer.TryFixAll (coreData)
			);
		}

		private static void WarnHouseholdWithNoSubscription(string[] args)
		{
			AiderProgram.RunWithCoreData
			(
				coreData => SubscriptionAndRefusalFixer.WarnHouseholdWithNoSubscription (coreData)
			);
		}

		private static void CreateMissingSubscriptionsOrWarnHousehold(string[] args)
		{
			AiderProgram.RunWithCoreData
			(
				coreData => SubscriptionAndRefusalFixer.CreateMissingSubscriptionsOrWarnHousehold (coreData)
			);
		}

		private static void FixOldEChWarnings(string[] args)
		{
			AiderProgram.RunWithCoreData ( coreData => {
						EChWarningsFixer.TryFixAll (coreData);
						System.Console.WriteLine ("Press RETURN to quit");
						System.Console.ReadLine ();
					});
		}

		private static void FixDepartureWarnings(string[] args)
		{
			AiderProgram.RunWithCoreData ( coreData => {
						EChWarningsFixer.TryFixDepartureWarnings (coreData);
						System.Console.WriteLine ("Press RETURN to quit");
						System.Console.ReadLine ();
					});
		}

		private static void FixUselessMissingSubscriptions(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				EChWarningsFixer.TryFixUselessMissingSubscriptions (coreData);
				System.Console.WriteLine ("Press RETURN to quit");
				System.Console.ReadLine ();
			});
		}


		private static void FixMissingSubscriptions(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				EChWarningsFixer.TryFixMissingSubscriptions (coreData);
				System.Console.WriteLine ("Press RETURN to quit");
				System.Console.ReadLine ();
			});
		}

		private static void FindPotentialDuplicatedPersons(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				PotentialDuplicatedPersonFinder.FindAll (coreData);
				System.Console.WriteLine ("Press RETURN to quit");
				System.Console.ReadLine ();
			});
		}
		
		private static void FixZombies(string[] args)
		{
			AiderProgram.RunWithCoreData
			(
				coreData => PersonDeathFixer.FixAll (coreData)
			);
		}

		private static void FixZombiesEChStatus(string[] args)
		{
			AiderProgram.RunWithCoreData
			(
				coreData =>
				{
					PersonDeathFixer.FixEChStatus (coreData);
					System.Console.WriteLine ("Press RETURN to quit");
					System.Console.ReadLine ();
				}
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

		private static void FixAddresses(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				var correctedAddressesFile = AiderProgram.GetFile (args, "-file:", true);

				SwissPostAddressFixer.ApplyFixes (coreData, correctedAddressesFile);

				System.Console.WriteLine ("Press RETURN to quit");
				System.Console.ReadLine ();
			});
		}

		private static void MoveAddresses(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				var correctedAddressesFile = AiderProgram.GetFile (args, "-file:", true);

				SwissPostAddressFixer.ApplyMoves (coreData, correctedAddressesFile);

				System.Console.WriteLine ("Press RETURN to quit");
				System.Console.ReadLine ();
			});
		}

		private static void AutoMergeDuplicatedPersons(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				FlagDuplicatedPersons.Run (coreData);

				System.Console.WriteLine ("Press RETURN to quit");
				System.Console.ReadLine ();
			});
		}

		private static void InitRoleCache(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				RoleCache.InitBaseSet (coreData);

				System.Console.WriteLine ("Press RETURN to quit");
				System.Console.ReadLine ();
			});
		}

		private static void FixRoleCacheParticipations(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				RoleCache.FixParticipations (coreData);

				System.Console.WriteLine ("Press RETURN to quit");
				System.Console.ReadLine ();
			});
		}

		private static void BuildRoleCache(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				RoleCache.Build (coreData);

				System.Console.WriteLine ("Press RETURN to quit");
				System.Console.ReadLine ();
			});
		}

		private static void PurgeRoleCache(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				RoleCache.Purge (coreData);

				System.Console.WriteLine ("Press RETURN to quit");
				System.Console.ReadLine ();
			});
		}

		private static void CleanHouseholds(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				//var echFilePath = AiderProgram.GetString (args, "-echfile:", true);
				//HouseholdsFix.EchHouseholdsQuality (coreData, echFilePath);

				//HouseholdsFix.PerformBatchFix (coreData);
				HouseholdCleaner.FixHouseholds(coreData);

				System.Console.WriteLine ("Press RETURN to quit");
				System.Console.ReadLine ();
			});
		}

		private static void CleanMailingCategories(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				MailingCategoriesCleaner.Clean (coreData);

				System.Console.WriteLine ("Press RETURN to quit");
				System.Console.ReadLine ();
			});
		}

		private static void SetPersonMrMrs(string[] args)
		{
			AiderProgram.RunWithCoreData (coreData =>
			{
				PersonMrMrsSetter.Set (coreData);

				System.Console.WriteLine ("Press RETURN to quit");
				System.Console.ReadLine ();
			});
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

		private static FileInfo GetFile(string[] args, string key, bool mandatory = false)
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

		private static string GetString(string[] args, string key, bool mandatory = false)
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