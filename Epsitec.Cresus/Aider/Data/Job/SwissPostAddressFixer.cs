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
using Epsitec.Common.Types;


namespace Epsitec.Aider.Data.Job
{
	internal static class SwissPostAddressFixer
	{
		public static void ApplyFixes(CoreData coreData, FileInfo file)
		{
			var adresses = File.ReadAllLines (file.FullName, System.Text.Encoding.Default)
				.Skip (1)
				.Select (x => x.Split (';'))
				.Select (x => new MatchResponseFix
				{
					Pstat = x[11],
					ContactId = "db:" + x[0].Substring (x[0].Length - 18).Replace ("/", ":"),
					CorrectedZip =  x[14],
					CorrectedZipAddOn = x[15],
					CorrectedTown = x[16],
					CorrectedStreet = x[12],
					CorrectedNumber = x[13],
				});

			SwissPostAddressFixer.FixAddresses (coreData, adresses.ToList ());
		}
		
		public static void ApplyMoves(CoreData coreData, FileInfo file)
		{
			var adresses = File.ReadAllLines (file.FullName, System.Text.Encoding.Default)
				.Skip (1)
				.Select (x => x.Split (';'))
				.Select (x => new MatchResponseMove
				{
					Ustat = x[18],
					ContactId = "db:" + x[0].Substring (x[0].Length - 18).Replace ("/", ":"),
					NewZip = x[35],
					NewZipAddOn = x[36],
					NewTown = x[37],
					NewStreet = x[30],
					NewNumber = x[31],
					NewPostBox = x[34]
				});

			SwissPostAddressFixer.MoveAddresses (coreData, adresses.ToList ());
		}

		private static void FixAddresses(CoreData coreData, List<MatchResponseFix> addressData)
		{
			var normalizedRchAddressesData		= addressData.Where (x => x.Pstat == "2");
			var modifiedRchAddressesData		= addressData.Where (x => x.Pstat == "3");
			var completedRchAddressesData		= addressData.Where (x => x.Pstat == "4");
			var archivedRchAddressesData		= addressData.Where (x => x.Pstat == "5");
			var invalidRchAddressesData			= addressData.Where (x => x.Pstat == "6");

			System.Console.WriteLine ("Fixing normalized addresses...");
			SwissPostAddressFixer.ApplyFix (coreData, normalizedRchAddressesData, SwissPostAddressFixer.ApplyCorrection);

			System.Console.WriteLine ("Fixing modified addresses...");
			SwissPostAddressFixer.ApplyFix (coreData, modifiedRchAddressesData, SwissPostAddressFixer.ApplyCorrection);
			
			System.Console.WriteLine ("Fixing completed addresses...");
			SwissPostAddressFixer.ApplyFix (coreData, completedRchAddressesData, SwissPostAddressFixer.ApplyCorrection);
			
			System.Console.WriteLine ("Fixing archived addresses...");
			SwissPostAddressFixer.ApplyFix (coreData, archivedRchAddressesData, SwissPostAddressFixer.ApplyCorrection);

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

		private static void MoveAddresses(CoreData coreData, List<MatchResponseMove> addressData)
		{
			var officialMoveAddressesData	= addressData.Where (x => x.Ustat == "1" || x.Ustat == "2" || x.Ustat == "3").ToList ();
			var deceasedAddressesData		= addressData.Where (x => x.Ustat == "4").ToList ();
			var officialChangeAddressesData	= addressData.Where (x => x.Ustat == "5").ToList ();

			System.Console.WriteLine ("Fixing official addresses changes...");
			SwissPostAddressFixer.ApplyFix (coreData, officialChangeAddressesData, SwissPostAddressFixer.ApplyMove);

			System.Console.WriteLine ("Fixing deceased contact addresses...");
			SwissPostAddressFixer.ExecuteWithBusinessContext (coreData,	context => SwissPostAddressFixer.ApplyPersonDecesead (context, deceasedAddressesData));
			
			System.Console.WriteLine ("Fixing official move addresses...");
			SwissPostAddressFixer.ExecuteWithBusinessContext (coreData, context => SwissPostAddressFixer.ApplyOfficialMoves (context, officialMoveAddressesData));
		}

		private static string GetMatchMoveDate()
		{
			return string.Format ("{0:00}.{1:00}.{2:0000}", SwissPostAddressFixer.MatchMoveDate.Day, SwissPostAddressFixer.MatchMoveDate.Month, SwissPostAddressFixer.MatchMoveDate.Year);
		}

		private static void AddSystemComment(MatchResponseMove move, AiderPersonEntity person)
		{
			string comment;

			switch (move.Ustat)
			{
				case "1":
					comment =
						"MAT[CH] " + SwissPostAddressFixer.GetMatchMoveDate () + " / Statut " + move.Ustat +"\n" +
						"Nouvelle adresse:\n" +
						move.NewStreet + " " + move.NewNumber + "\n" +
						move.NewPostBox + "\n" +
						move.NewZip + " " + move.NewZipAddOn + " " + move.NewTown;
					break;
				
				case "2":
					comment =
						"MAT[CH] " + SwissPostAddressFixer.GetMatchMoveDate () + " / Statut " + move.Ustat +"\n" +
						"Nouvelle adresse inconnue";
					break;

				case "3":
					comment =
						"MAT[CH] " + SwissPostAddressFixer.GetMatchMoveDate () + " / Statut " + move.Ustat +"\n" +
						"Parti à l'étranger";
					break;

				default:
					return;
			}
			
			comment = comment.Replace ("\n\n", "\n");

			System.Diagnostics.Debug.WriteLine ("Comment: " + comment.Replace ("\n", ", "));

			AiderCommentEntity.CombineSystemComments (person, comment);
		}

		private static void ApplyFix<T>(CoreData coreData, IEnumerable<T> addresses, System.Action<AiderTownRepository, T, AiderAddressEntity, AiderContactEntity> fix)
			where T : MatchResponse
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

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
			}
		}

		private static void FixModifiedAddresses<T>(IEnumerable<T> addresses, BusinessContext businessContext, AiderTownRepository townRepo,
			System.Action<AiderTownRepository, T, AiderAddressEntity, AiderContactEntity> action)
			where T : MatchResponse
		{
			foreach (var modif in addresses)
			{
				var contact = modif.GetContact (businessContext);

				if (contact.IsNotNull ())
				{
					businessContext.Register (contact);

					var address = contact.Address;
					var oldAddress = address.GetDisplayAddress ();

					action (townRepo, modif, address, contact);

					businessContext.ApplyRulesToRegisteredEntities (RuleType.Validate, EntitySaveMode.IgnoreValidationErrors);
					businessContext.ApplyRulesToRegisteredEntities (RuleType.Update, EntitySaveMode.IgnoreValidationErrors);

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

		private static void ApplyCorrection(AiderTownRepository townRepo, MatchResponseFix modif, AiderAddressEntity address, AiderContactEntity contact)
		{
			//	First update the town, then the street, otherwise the mapping algorithm based
			//	on the user friendly name might not select the correct street name:

			address.Town = townRepo.GetTown (modif.CorrectedZip, modif.CorrectedTown, "CH");

			if (string.IsNullOrEmpty (modif.CorrectedNumber))
			{
				//	We never remove an existing house number on an existing address

				address.StreetUserFriendly = modif.CorrectedStreet;
			}
			else
			{
				address.StreetUserFriendly       = modif.CorrectedStreet;
				address.HouseNumberAndComplement = modif.CorrectedNumber;
			}
		}

		private static void ApplyMove(AiderTownRepository townRepo, MatchResponseMove move, AiderAddressEntity address, AiderContactEntity contact)
		{
			//	First update the town, then the street, otherwise the mapping algorithm based
			//	on the user friendly name might not select the correct street name:

			address.Town = townRepo.GetTown (move.NewZip, move.NewTown, "CH");

			address.StreetUserFriendly       = move.NewStreet;
			address.HouseNumberAndComplement = move.NewNumber;
			address.PostBox					 = move.NewPostBox;
		}

		private static void ApplyPersonDecesead(BusinessContext businessContext, IEnumerable<MatchResponseMove> deceasedAddressesData)
		{
			{
				foreach (var deceased in deceasedAddressesData)
				{
					var contact = deceased.GetContact (businessContext);

					if (contact.IsNotNull ())
					{
						System.Diagnostics.Debug.WriteLine ("Killing " + contact.DisplayName + ", " + contact.Person.eCH_Person.DeclarationStatus.ToString ());
						contact.Person.KillPerson (businessContext, SwissPostAddressFixer.MatchMoveDate, uncertain: true);
					}
					else
					{
						System.Console.WriteLine ("Contact not found: {0}", deceased.ContactId);
					}
				}
			}
		}

		private class OfficialMover
		{
			public OfficialMover(BusinessContext businessContext, IEnumerable<MatchResponseMove> moves)
			{
				this.businessContext = businessContext;
				this.moves           = moves;
				this.townRepo        = new AiderTownRepository (this.businessContext);
				
				this.approvedMoves				 = new List<MatchResponseMove> ();
				this.alreadyProcessedContactsIds = new HashSet<string> ();
				this.officiallyMovedContactsIds  = this.moves.Where (m => m.Ustat == "1").ToDictionary (k => k.ContactId, v => v);
				this.unknownMovedContactsIds     = this.moves.Where (m => m.Ustat == "2" || m.Ustat == "3").ToDictionary (k => k.ContactId, v => v);
			}

			public void FixAddresses()
			{
				this.ProcessMoves ();
				this.FixApprovedMoves ();
			}

			private void FixApprovedMoves()
			{
				SwissPostAddressFixer.FixModifiedAddresses (this.approvedMoves, this.businessContext, this.townRepo, this.ApplyOfficialChange);
			}

			private void ApplyOfficialChange(AiderTownRepository townRepo, MatchResponseMove move, AiderAddressEntity address, AiderContactEntity contact)
			{
				SwissPostAddressFixer.ApplyMove (townRepo, move, address, contact);

				if (address.Town.SwissCantonCode != "VD")
				{
					this.ProcessMoveSubscription (contact);
				}
			}


			private void ProcessMoves()
			{
				foreach (var move in moves)
				{
					//	Check that the move was not already made by another household member

					if (!this.alreadyProcessedContactsIds.Contains (move.ContactId))
					{
						var contact = move.GetContact (this.businessContext);

						if (contact.IsNotNull ())
						{
							this.ProcessMove (move, contact);
						}
						else
						{
							System.Console.WriteLine ("Contact not found: {0}", move.ContactId);
						}
					}
				}
			}

			private void ProcessMove(MatchResponseMove move, AiderContactEntity contact)
			{
				if (contact.Household.IsNotNull ())
				{
					this.ProcessMoveWithHousehold (move, contact);
				}
				else
				{
					this.ProcessMoveSingle (move, contact);
				}
			}

			private void ProcessMoveWithHousehold(MatchResponseMove move, AiderContactEntity contact)
			{
				switch (contact.Household.Members.Count)
				{
					case 1:
						this.ProcessMoveSingle (move, contact);
						break;

					case 0:
						System.Console.WriteLine ("Contact has an empty household");
						break;

					default:
						this.ProcessMoveMultiple (move, contact);
						break;
				}
			}
			
			private void ProcessMoveSingle(MatchResponseMove move, AiderContactEntity contact)
			{
				//	Known move to known address ?

				if (move.Ustat == "1")
				{
					System.Diagnostics.Debug.WriteLine ("Move single: " + contact.DisplayName);
					this.approvedMoves.Add (move);
					this.alreadyProcessedContactsIds.Add (move.ContactId);
				}
				else
				{
					SwissPostAddressFixer.AddSystemComment (move, contact.Person);

					this.ProcessMoveSubscription (contact);
				}
			}
			
			private void ProcessMoveSubscription(AiderContactEntity contact)
			{
				var subscription = AiderSubscriptionEntity.FindSubscription (this.businessContext, contact)
								?? AiderSubscriptionEntity.FindSubscription (this.businessContext, contact.Household);

				if (subscription.IsNotNull ())
				{
					System.Diagnostics.Debug.WriteLine ("Subscription: " + subscription.DisplayAddress);
					subscription.SusbscriptionFlag = Enumerations.SubscriptionFlag.VerificationRequired;
				}
			}
			
			private void ProcessMoveMultiple(MatchResponseMove move, AiderContactEntity contact)
			{
				// First find other adult members before checking move approval...

				var members = contact.Household.Members.Where (p => contact.Household.IsHead (p) || p.Age.GetValueOrDefault () >= 18).ToList ();

				if (!members.Any (p => p == contact.Person))
				{
					//	The person is probably a child moving to a new address !?

					SwissPostAddressFixer.AddSystemComment (move, contact.Person);
					return;
				}

				switch (members.Count)
				{
					case 1:
						this.ProcessMoveSingle (move, contact);
						return;
					
					case 2:
						break;

					default:
						System.Diagnostics.Debug.WriteLine ("Special case: " + members.Count.ToString () + " members");
						break;
				}

				foreach (var member in members.Where (c => c != contact.Person))
				{
					var otherContactId = this.businessContext.DataContext.GetPersistedId (member.HouseholdContact);

					MatchResponseMove otherMove = null;

					if (move.Ustat == "1")
					{
						//	Is this an official move ?
						
						if (this.officiallyMovedContactsIds.TryGetValue (otherContactId, out otherMove))
						{
							if (otherMove.HasSameNewAddress (move))	//	Moved to same place, approved
							{
								if (alreadyProcessedContactsIds.Add (move.ContactId))
								{
									approvedMoves.Add (move);
								}

								alreadyProcessedContactsIds.Add (otherMove.ContactId);

								System.Diagnostics.Debug.WriteLine ("Move mulitple: " + contact.DisplayName + " + " + member.DisplayName);
							}
							else
							{
								//	Moved but not at same place, comment only
								if (alreadyProcessedContactsIds.Add (move.ContactId))
								{
									SwissPostAddressFixer.AddSystemComment (move, contact.Person);
								}
								break;
							}
						}
						else //not in list, comment only
						{
							if (alreadyProcessedContactsIds.Add (move.ContactId))
							{
								SwissPostAddressFixer.AddSystemComment (move, contact.Person);
							}
							break;
						}
					}
					else
					{
						//	This is an unknown move (or a move to a foreign country)

						if (this.unknownMovedContactsIds.TryGetValue (otherContactId, out otherMove))
						{
							if (alreadyProcessedContactsIds.Add (move.ContactId))
							{
								SwissPostAddressFixer.AddSystemComment (move, contact.Person);
								this.ProcessMoveSubscription (contact);
							}

							if (alreadyProcessedContactsIds.Add (otherMove.ContactId))
							{
								SwissPostAddressFixer.AddSystemComment (move, member);
							}
						}
						else //not in list, comment only
						{
							if (alreadyProcessedContactsIds.Add (move.ContactId))
							{
								SwissPostAddressFixer.AddSystemComment (move, contact.Person);
							}
						}
					}
				}
			}

			private readonly BusinessContext businessContext;
			private readonly IEnumerable<MatchResponseMove> moves;
			private readonly List<MatchResponseMove> approvedMoves;
			private readonly HashSet<string> alreadyProcessedContactsIds;
			private readonly AiderTownRepository townRepo;
			private readonly Dictionary<string, MatchResponseMove> officiallyMovedContactsIds;
			private readonly Dictionary<string, MatchResponseMove> unknownMovedContactsIds;
		}

		private static void ApplyOfficialMoves(BusinessContext businessContext, IEnumerable<MatchResponseMove> officialMoveAddressesData)
		{
			var mover = new OfficialMover (businessContext, officialMoveAddressesData);

			mover.FixAddresses ();
		}


		public static Date MatchMoveDate = new Date (2013, 12, 20);
	}
}