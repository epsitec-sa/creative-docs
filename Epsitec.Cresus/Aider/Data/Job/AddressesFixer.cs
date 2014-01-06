using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;

using Epsitec.Common.IO;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;


namespace Epsitec.Aider.Data.Job
{
	internal static class AddressesFixer
	{
		public static void FixPstat3To6(CoreData coreData,FileInfo rch,FileInfo nonRch)
		{
			//Rch
			var rchAddressesData = File.ReadAllLines (rch.FullName, System.Text.Encoding.Default).Skip(1).Select (x => x.Split (';')).Select (
				x => new MatchResponse
				{
					Pstat = x[11],
					ContactId = "db:" + x[0].Substring (x[0].Length - 18).Replace("/",":"),
					Zip =  x[14],
					ZipAddOn = x[15],
					Town = x[16],
					Street = x[12],
					Number = x[13]
				});

			var nonRchAddressesData = File.ReadAllLines (nonRch.FullName, System.Text.Encoding.Default).Skip (1).Select (x => x.Split (';')).Select (
				x => new MatchResponse
				{
					Pstat = x[11],
					ContactId = "db:" + x[0].Substring (x[0].Length - 18).Replace ("/", ":"),
					Zip =  x[14],
					ZipAddOn = x[15],
					Town = x[16],
					Street = x[12],
					Number = x[13]
				});

			//Unified dataset
			var addressData = rchAddressesData.Union (nonRchAddressesData).ToList();

			//PSTAT 3
			var modifiedRchAddressesData = addressData.Where (x => x.Pstat == "3");

			//PSTAT 4
			var completedRchAddressesData = addressData.Where (x => x.Pstat == "4");

			//PSTAT 5
			var archivedRchAddressesData = addressData.Where (x => x.Pstat == "5");

			//PSTAT 6
			var invalidRchAddressesData = addressData.Where (x => x.Pstat == "6");

			using (var businessContext = new BusinessContext (coreData, false))
			{
				var townRepo = new AiderTownRepository (businessContext);

				//MODIFIED
				Console.WriteLine ("Update modified addresses...");
				foreach (var modif in modifiedRchAddressesData)
				{
					var contact = (AiderContactEntity) businessContext.DataContext.GetPersistedEntity (modif.ContactId);
					businessContext.Register (contact);

					if (contact.IsNotNull ())
					{

						var address = contact.Address;

						//Street update
						address.StreetUserFriendly = modif.Street;

						//skip this change if null
						if (!string.IsNullOrEmpty (modif.Number))
						{
							address.HouseNumberAndComplement = modif.Number;
						}


						//Town update
						address.Town = townRepo.GetTown (modif.Zip, modif.Town, "CH");

						//Apply changes
						contact.RefreshCache ();
					}
					else
					{
						Console.WriteLine ("Contact not found: {0}", modif.ContactId);
					}
				}

				//COMPLETED
				Console.WriteLine ("Update completed addresses...");
				foreach (var modif in completedRchAddressesData)
				{
					var contact = (AiderContactEntity) businessContext.DataContext.GetPersistedEntity (modif.ContactId);
					businessContext.Register (contact);
					
					if (contact.IsNotNull ())
					{
						var address = contact.Address;
						address.HouseNumberAndComplement = modif.Number;

						//Apply changes
						contact.RefreshCache ();
					}
					else
					{
						Console.WriteLine ("Contact not found: {0}", modif.ContactId);
					}
				}

				//ARCHIVED
				Console.WriteLine ("Update archived addresses...");
				foreach (var modif in archivedRchAddressesData)
				{
					var contact = (AiderContactEntity) businessContext.DataContext.GetPersistedEntity (modif.ContactId);
					businessContext.Register (contact);


					if (contact.IsNotNull ())
					{

						var address = contact.Address;

						//Street update
						address.StreetUserFriendly = modif.Street;

						//Town update
						address.Town = townRepo.GetTown (modif.Zip, modif.Town, "CH");
					
						//Apply changes
						contact.RefreshCache ();
					}
					else
					{
						Console.WriteLine ("Contact not found: {0}", modif.ContactId);
					}
				}

				//INVALID
				Console.WriteLine ("Update subscription of invalid addresses...");
				foreach (var modif in invalidRchAddressesData)
				{
					var contact = (AiderContactEntity) businessContext.DataContext.GetPersistedEntity (modif.ContactId);
					businessContext.Register (contact);

					if (contact.IsNotNull ())
					{
						var subscription = AiderSubscriptionEntity.FindSubscription (businessContext, contact);

						if (subscription.IsNotNull ())
						{
							subscription.SusbscriptionFlag = Enumerations.SubscriptionFlag.VerificationRequired;
						}
					}
					else
					{
						Console.WriteLine ("Contact not found: {0}", modif.ContactId);
					}
				}


				//Apply changes
				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
				
			}
		}
	}

	public class MatchResponse
	{
		public string Pstat
		{
			get;
			set;
		}

		public string ContactId
		{
			get;
			set;
		}

		public string Zip
		{
			get;
			set;
		}

		public string ZipAddOn
		{
			get;
			set;
		}

		public string Town
		{
			get;
			set;
		}

		public string Street
		{
			get;
			set;
		}

		public string Number
		{
			get;
			set;
		}
	}
}
