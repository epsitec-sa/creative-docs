//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace Epsitec.Aider.Data.Job
{
	internal static class SwissPostAddressFixer
	{
		public static void ApplyFixes(CoreData coreData, FileInfo file)
		{
			var adresses = File.ReadAllLines (file.FullName, System.Text.Encoding.Default)
				.Skip (1)
				.Select (x => x.Split (';'))
				.Select (x => new MatchResponse
				{
					Pstat = x[11],
					ContactId = "db:" + x[0].Substring (x[0].Length - 18).Replace ("/", ":"),
					Zip =  x[14],
					ZipAddOn = x[15],
					Town = x[16],
					Street = x[12],
					Number = x[13]
				});

			SwissPostAddressFixer.FixAddresses (coreData, adresses.ToList ());
		}

		private static void FixAddresses(CoreData coreData, List<MatchResponse> addressData)
		{
			var modifiedRchAddressesData  = addressData.Where (x => x.Pstat == "3");
			var completedRchAddressesData = addressData.Where (x => x.Pstat == "4");
			var archivedRchAddressesData  = addressData.Where (x => x.Pstat == "5");
			var invalidRchAddressesData   = addressData.Where (x => x.Pstat == "6");

			System.Console.WriteLine ("Fixing modified addresses...");
			SwissPostAddressFixer.ApplyFix (coreData, modifiedRchAddressesData, SwissPostAddressFixer.ApplyModification);
			
			System.Console.WriteLine ("Fixing completed addresses...");
			SwissPostAddressFixer.ApplyFix (coreData, completedRchAddressesData, SwissPostAddressFixer.ApplyModification);
			
			System.Console.WriteLine ("Fixing archived addresses...");
			SwissPostAddressFixer.ApplyFix (coreData, archivedRchAddressesData, SwissPostAddressFixer.ApplyModification);

			System.Console.WriteLine ("Fixing subscriptions of invalid addresses...");

			SwissPostAddressFixer.ExecuteWithBusinessContext (coreData,
				businessContext =>
				{
					foreach (var modif in invalidRchAddressesData)
					{
						var contact = modif.GetContact (businessContext);

						if (contact.IsNotNull ())
						{
							var subscription = AiderSubscriptionEntity.FindSubscription (businessContext, contact)
											?? AiderSubscriptionEntity.FindSubscription (businessContext, contact.Household);

							if (subscription.IsNotNull ())
							{
								System.Diagnostics.Debug.WriteLine (subscription.DisplayAddress);

								subscription.SusbscriptionFlag = Enumerations.SubscriptionFlag.VerificationRequired;
							}
						}
						else
						{
							System.Console.WriteLine ("Contact not found: {0}", modif.ContactId);
						}
					}
				});
		}

		private static void ApplyFix(CoreData coreData, IEnumerable<MatchResponse> addresses, System.Action<AiderTownRepository, MatchResponse, AiderAddressEntity> fix)
		{
			SwissPostAddressFixer.ExecuteWithBusinessContext (coreData,
				businessContext =>
				{
					var townRepo = new AiderTownRepository (businessContext);
					SwissPostAddressFixer.FixModifiedAddresses (addresses, businessContext, townRepo, fix);
				});
		}

		private static void ExecuteWithBusinessContext(CoreData coreData, System.Action<BusinessContext> action)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				action (businessContext);

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}

		private static void FixModifiedAddresses(IEnumerable<MatchResponse> addresses, BusinessContext businessContext, AiderTownRepository townRepo,
			System.Action<AiderTownRepository, MatchResponse, AiderAddressEntity> action)
		{
			foreach (var modif in addresses)
			{
				var contact = modif.GetContact (businessContext);

				if (contact.IsNotNull ())
				{
					businessContext.Register (contact);

					var address = contact.Address;
					var oldAddress = address.GetDisplayAddress ();

					action (townRepo, modif, address);

					businessContext.ApplyRulesToRegisteredEntities (RuleType.Validate);
					businessContext.ApplyRulesToRegisteredEntities (RuleType.Update);

					businessContext.ClearRegisteredEntities ();
					
					var newAddress = address.GetDisplayAddress ();

					if (oldAddress != newAddress)
					{
						System.Diagnostics.Debug.WriteLine (oldAddress.ToSimpleText () + " > " + newAddress.ToSimpleText ());
					}
				}
				else
				{
					System.Console.WriteLine ("Contact not found: {0}", modif.ContactId);
				}
			}
		}

		private static void ApplyModification(AiderTownRepository townRepo, MatchResponse modif, AiderAddressEntity address)
		{
			//	First update the town, then the street, otherwise the mapping algorithm based
			//	on the user friendly name might not select the correct street name:

			address.Town = townRepo.GetTown (modif.Zip, modif.Town, "CH");

			if (string.IsNullOrEmpty (modif.Number))
			{
				//	We never remove an existing house number on an existing address

				address.StreetUserFriendly = modif.Street;
			}
			else
			{
				address.StreetUserFriendly       = modif.Street;
				address.HouseNumberAndComplement = modif.Number;
			}
		}
	}
}