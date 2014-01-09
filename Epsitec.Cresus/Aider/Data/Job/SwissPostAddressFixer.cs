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
				.Select (x => new MatchResponse
				{
					Pstat = x[11],
					Ustat = x[18],
					ContactId = "db:" + x[0].Substring (x[0].Length - 18).Replace ("/", ":"),
					CorrectedZip =  x[14],
					CorrectedZipAddOn = x[15],
					CorrectedTown = x[16],
					CorrectedStreet = x[12],
					CorrectedNumber = x[13],
					NewZip = x[35],
					NewZipAddOn = x[36],
					NewTown = x[37],
					NewStreet = x[30],
					NewNumber = x[31],
					NewPostBox = x[34]
				});

			SwissPostAddressFixer.FixAddresses (coreData, adresses.ToList ());
		}

		private static void FixAddresses(CoreData coreData, List<MatchResponse> addressData)
		{
			var modifiedRchAddressesData		= addressData.Where (x => x.Pstat == "3");
			var completedRchAddressesData		= addressData.Where (x => x.Pstat == "4");
			var archivedRchAddressesData		= addressData.Where (x => x.Pstat == "5");
			var invalidRchAddressesData			= addressData.Where (x => x.Pstat == "6");
			
			var officialMoveAddressesData		= addressData.Where (x => x.Ustat == "1" || x.Ustat == "2" || x.Ustat == "3");
			var deceadedAddressesData			= addressData.Where (x => x.Ustat == "4");
			var officialChangeAddressesData		= addressData.Where (x => x.Ustat == "5");
			
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
			
			System.Console.WriteLine ("Fixing official addresses changes...");
			SwissPostAddressFixer.ApplyFix (coreData, officialChangeAddressesData, SwissPostAddressFixer.ApplyOfficialChange);

			System.Console.WriteLine ("Fixing deceaded contact addresses...");
			SwissPostAddressFixer.ExecuteWithBusinessContext (coreData,
				businessContext =>
				{
					foreach (var decease in deceadedAddressesData)
					{
						var contact = decease.GetContact (businessContext);

						if (contact.IsNotNull ())
						{
							contact.Person.KillPerson (businessContext, new Date(2013,12,20), true);
						}
						else
						{
							System.Console.WriteLine ("Contact not found: {0}", decease.ContactId);
						}
					}
				});
			
			System.Console.WriteLine ("Fixing official move addresses...");
			SwissPostAddressFixer.ExecuteWithBusinessContext (coreData,
				businessContext =>
				{
					var townRepo					= new AiderTownRepository (businessContext);

					var approvedMoves				= new List<MatchResponse> ();
					var alreadyProcessedContactsIds	= new HashSet<string> ();
					var officialyMovedContactsIds	= officialMoveAddressesData.Where(m => m.Ustat == "1").ToDictionary (k => k.ContactId, v => v);
					var unknowMovedContactsIds		= officialMoveAddressesData.Where (m => m.Ustat == "2" || m.Ustat == "3").ToDictionary (k => k.ContactId, v => v);

					foreach (var move in officialMoveAddressesData)
					{
						//check that the move is not already made by other household member
						if (!alreadyProcessedContactsIds.Contains (move.ContactId))
						{
							var contact = move.GetContact (businessContext);

							if (contact.IsNotNull ())
							{
								//We check household members before applying moves
								if (contact.Household.IsNotNull ())
								{
									if (contact.Household.Members.Count == 1)
									{
										if (move.Ustat == "1")
										{
											approvedMoves.Add (move);
											alreadyProcessedContactsIds.Add (move.ContactId);
										}
										else
										{
											SwissPostAddressFixer.AddMatchUnknowComment (move, contact.Person);

											var subscription = AiderSubscriptionEntity.FindSubscription (businessContext, contact)
											?? AiderSubscriptionEntity.FindSubscription (businessContext, contact.Household);

											if (subscription.IsNotNull ())
											{
												System.Diagnostics.Debug.WriteLine (subscription.DisplayAddress);

												subscription.SusbscriptionFlag = Enumerations.SubscriptionFlag.VerificationRequired;
											}
										}
										
									}
									else if (contact.Household.Members.Count > 1)
									{
										//We find other head members for checking move approval
										foreach (var headMember in contact.Household.Members.Where (c => contact.Household.IsHead (c) && c != contact.Person))
										{
											var otherContactId = businessContext.DataContext.GetPersistedId (headMember.HouseholdContact);
											MatchResponse otherMove = null;

											if (move.Ustat == "1")
											{
												//is in official move list?
												if (officialyMovedContactsIds.TryGetValue (otherContactId, out otherMove))
												{
													if (otherMove.IsSameNewAddress (move))//moved at same place, approved
													{
														approvedMoves.Add (move);
														alreadyProcessedContactsIds.Add (move.ContactId);
														alreadyProcessedContactsIds.Add (otherMove.ContactId);
														break;
													}
													else //moved but not at same place, comment only
													{
														SwissPostAddressFixer.AddMatchMoveComment (move, contact.Person);
													}
												}
												else //not in list, comment only
												{
													SwissPostAddressFixer.AddMatchMoveComment (move, contact.Person);
												}
											}
											else
											{
												//is in unknow move list?
												if (unknowMovedContactsIds.TryGetValue (otherContactId, out otherMove))
												{
													SwissPostAddressFixer.AddMatchUnknowComment (move, contact.Person);
													SwissPostAddressFixer.AddMatchUnknowComment (move, headMember);

													alreadyProcessedContactsIds.Add (move.ContactId);
													alreadyProcessedContactsIds.Add (otherMove.ContactId);

													var subscription = AiderSubscriptionEntity.FindSubscription (businessContext, contact)
													?? AiderSubscriptionEntity.FindSubscription (businessContext, contact.Household);

													if (subscription.IsNotNull ())
													{
														System.Diagnostics.Debug.WriteLine (subscription.DisplayAddress);

														subscription.SusbscriptionFlag = Enumerations.SubscriptionFlag.VerificationRequired;
													}
													break;
												}
												else //not in list, comment only
												{
													SwissPostAddressFixer.AddMatchUnknowComment (move, contact.Person);
												}
											}
										}
									}
								}
								else //no household, approved
								{
									if (move.Ustat == "1")
									{
										approvedMoves.Add (move);
										alreadyProcessedContactsIds.Add (move.ContactId);
									}
									else
									{
										SwissPostAddressFixer.AddMatchUnknowComment (move, contact.Person);
									}
								}
							}
							else
							{
								System.Console.WriteLine ("Contact not found: {0}", move.ContactId);
							}
						}
					}

					//apply fix with approved moves only
					SwissPostAddressFixer.FixModifiedAddresses (approvedMoves, businessContext, townRepo, SwissPostAddressFixer.ApplyOfficialChange);
				});



		}

		private static void AddMatchMoveComment(MatchResponse move,AiderPersonEntity person)
		{
			string comment =	"MATCH " + 
								System.DateTime.Now.ToString ("y") +
								"/ CODE " + move.Ustat +"\nNouvelle adresse:\n" +
								move.NewStreet + " " + move.NewNumber +"\n" +
								move.NewPostBox +
								move.NewZip + " " + move.NewZipAddOn + " " + move.NewTown;

			AiderCommentEntity.CombineSystemComments (person, comment);
		}

		private static void AddMatchUnknowComment(MatchResponse move, AiderPersonEntity person)
		{
			if (move.Ustat == "2")
			{
				string comment =	"MATCH " + 
								System.DateTime.Now.ToString ("y") +
								"/ CODE 2\ndéménagé à un lieu inconnu\n";

				AiderCommentEntity.CombineSystemComments (person, comment);
			}
			if (move.Ustat == "3")
			{
				string comment =	"MATCH " + 
								System.DateTime.Now.ToString ("y") +
								"/ CODE 3\nParti à l'étranger\n";

				AiderCommentEntity.CombineSystemComments (person, comment);
			}
			
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

		private static void ApplyCorrection(AiderTownRepository townRepo, MatchResponse modif, AiderAddressEntity address)
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

		private static void ApplyOfficialChange(AiderTownRepository townRepo, MatchResponse modif, AiderAddressEntity address)
		{
			//	First update the town, then the street, otherwise the mapping algorithm based
			//	on the user friendly name might not select the correct street name:

			address.Town = townRepo.GetTown (modif.NewZip, modif.NewTown, "CH");

			address.StreetUserFriendly       = modif.NewStreet;
			address.HouseNumberAndComplement = modif.NewNumber;
			address.PostBox					 = modif.NewPostBox;
			
		}
	}
}