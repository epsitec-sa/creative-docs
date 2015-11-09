﻿//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers;
using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Text;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Aider.Helpers;


namespace Epsitec.Aider.Entities
{
	public partial class AiderPersonEntity : IAiderWarningExampleFactoryGetter
	{
		public bool								IsGovernmentDefined
		{
			get
			{
				return (this.eCH_Person.IsNotNull ())
					&& (this.eCH_Person.DataSource == Enumerations.DataSource.Government);
			}
		}

		public bool								IsDeclared
		{
			get
			{
				return (this.eCH_Person.IsNotNull ())
					&& (this.eCH_Person.DeclarationStatus == PersonDeclarationStatus.Declared);
			}
		}

		public bool								IsAlive
		{
			get
			{
				return this.eCH_Person.IsNull () || this.eCH_Person.IsDeceased == false;
			}
		}

		public bool								IsDeceased
		{
			get
			{
				return !this.IsAlive;
			}
		}
		
		public int?								Age
		{
			get
			{
				if (this.eCH_Person.IsNull ())
				{
					return null;
				}

				var birthdate = this.eCH_Person.PersonDateOfBirth;
				var deathdate = this.eCH_Person.PersonDateOfDeath;

				if (birthdate == null)
				{
					return null;
				}
				else if (deathdate == null)
				{
					return birthdate.Value.ComputeAge ();
				}
				else
				{
					return birthdate.Value.ComputeAge (deathdate.Value);
				}
			}
		}

		public bool								HasDerogation
		{
			get
			{
				return string.IsNullOrEmpty (this.GeoParishGroupPathCache) == false;
			}
		}


		public FormattedText GetCompactSummary(AiderHouseholdEntity household)
		{
			if ((household.IsNotNull ()) &&
				(household.IsHead (this)))
			{
				var boldName = TextFormatter.FormatText (this.DisplayName).ApplyBold ();
				return TextFormatter.FormatText (boldName, "(~", this.Age, "~)");
			}
			else
			{
				return this.GetCompactSummary ();
			}
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.DisplayName, "(~", this.Age, "~)");
		}

		public override FormattedText GetSummary()
		{
			return this.GetCoordinatesSummary ();
		}

		public FormattedText GetPersonalDataSummary()
		{
			return TextFormatter.FormatText (
				this.Title, this.MrMrs, TextFormatter.Command.IfEmpty, "\n",
				this.eCH_Person.PersonFirstNames, this.eCH_Person.PersonOfficialName, "(~", this.OriginalName, "~)", "\n",
				this.eCH_Person.PersonDateOfBirth, "~ – ~", this.eCH_Person.PersonDateOfDeath, "\n",
				this.Confession, "~\n",
				this.Profession, "~\n");
		}

		public static AiderPersonEntity Create(BusinessContext businessContext, eCH_PersonEntity eChPersonEntity, PersonMrMrs mrMrs)
		{
			var aiderPersonEntity = businessContext.CreateAndRegisterEntity<AiderPersonEntity> ();

			aiderPersonEntity.eCH_Person = eChPersonEntity;
			aiderPersonEntity.MrMrs = mrMrs;		
			aiderPersonEntity.Visibility = PersonVisibilityStatus.Default;
			aiderPersonEntity.Confession = PersonConfession.Protestant;

			return aiderPersonEntity;
		}

		public string GetPersonCheckKey()
		{
			return new StringBuilder ().Append (this.BirthdayDay.ToString ())
							 .Append (this.BirthdayMonth.ToString ())
							 .Append (this.BirthdayYear.ToString ())
							 .Append (this.eCH_Person.PersonFirstNames.Split (",").First ()).ToString ();
		}

		public AiderGroupEntity GetDerogationGeoParishGroup(BusinessContext context)
		{
			if (this.HasDerogation)
			{
				return AiderGroupEntity.FindGroups (context, this.GeoParishGroupPathCache).Single ();
			}
			else
			{
				return null;
			}
		}

		public string GetFormattedBirthdayDate()
		{
			if (this.BirthdayYear == 0)
			{
				return "—";
			}
			if (this.BirthdayMonth == 0)
			{
				return string.Format ("{0:0000}", this.BirthdayYear);
			}
			if (this.BirthdayDay == 0)
			{
				return string.Format ("{0.00}.{1:0000}", this.BirthdayMonth, this.BirthdayYear);
			}

			return string.Format ("{0:00}.{1:00}.{2:0000}", this.BirthdayDay, this.BirthdayMonth, this.BirthdayYear);
		}

		public string GetParishLocationName(BusinessContext context, ParishOrigin parishOrigin)
		{
			AiderGroupEntity group;

			switch (parishOrigin)
			{
				case ParishOrigin.Active:
					group = this.ParishGroup;
					break;

				case ParishOrigin.Geographic:
					group = this.GetDerogationGeoParishGroup (context);
					break;

				default:
					throw new System.NotSupportedException (parishOrigin.GetQualifiedName ());
			}

			if ((group.IsNull ()) ||
				(group.IsNoParish ()))
			{
				return null;
			}

			int prefixLength = "Paroisse d?".Length;

			return group.Name.Substring (prefixLength).Trim ();
		}

		public string GetCallName()
		{
			if (string.IsNullOrWhiteSpace (this.CallName))
			{
				return eCH_PersonEntity.GetDefaultFirstName (this.eCH_Person);
			}

			return this.CallName;
		}

		public string GetShortCallName()
		{
			return NameProcessor.GetAbbreviatedFirstname (this.GetCallName ());
		}


		public string GetFullName()
		{
			return this.GetFullName (this.GetCallName ());
		}

		public string GetShortFullName()
		{
			return this.GetFullName (this.GetShortCallName ());
		}

		public string GetFullName(string firstname)
		{
			return StringUtils.Join (" ", firstname, this.eCH_Person.PersonOfficialName);
		}

		public string GetDisplayName()
		{
			var lastname = this.eCH_Person.PersonOfficialName;
			var firstname = this.GetCallName ();

			var name = TextFormatter.FormatText (lastname, ",", firstname).ToSimpleText ();

			if (this.eCH_Person.IsDeceased)
			{
				name += AiderPersonEntity.DeceasedSuffix;
			}

			return name;
		}

		public string GetIconName(string prefix)
		{
			string suffix;

			if (this.eCH_Person.PersonSex == PersonSex.Female)
			{
				suffix = ".AiderPerson.Female-";
			}
			else
			{
				suffix = ".AiderPerson.Male-";
			}

			if (this.Language == Enumerations.Language.German)
			{
				return prefix + suffix + "German";
			}
			else
			{
				return prefix + suffix + "French";
			}
		}


		public void ToggleHouseholdRole(AiderHouseholdEntity household)
		{
			if (household.IsNull ())
			{
				return;
			}

			var contact = this.Contacts.FirstOrDefault (x => x.Household == household);

			if (contact.IsNotNull ())
			{
				if (contact.HouseholdRole == Enumerations.HouseholdRole.Head)
				{
					contact.HouseholdRole = Enumerations.HouseholdRole.None;
				}
				else
				{
					contact.HouseholdRole = Enumerations.HouseholdRole.Head;
				}
			}
		}

		public void ClearDerogation()
		{
			this.GeoParishGroupPathCache = "";
		}

		public void AssignNewHousehold(BusinessContext context, bool move)
		{
			var newHousehold = context.CreateAndRegisterEntity<AiderHouseholdEntity> ();
			var mainContact  = this.MainContact;

			if ((mainContact.IsNotNull ()) &&
				(mainContact.Address.IsNotNull ()))
			{
				var oldAddress = mainContact.Address;
				var newAddress = newHousehold.Address;

				newAddress.Town = oldAddress.Town;
				newAddress.StreetHouseNumberAndComplement = oldAddress.StreetHouseNumberAndComplement;
			}

			if (move)
			{
				AiderContactEntity.ChangeHousehold (context, this.MainContact, newHousehold, isHead: true);
			}
		}

		public void RemoveFromThisHousehold(BusinessContext context, AiderHouseholdEntity household)
		{
			if (household.IsNull ())
			{
				return;
			}

			var contacts = this.Contacts;
			var contact  = contacts.FirstOrDefault (x => x.Household == household);

			if (this.Households.Count == 1)
			{
				//	The person is attached to a single household. We have to create a new household
				//	and move it there; this will remove the person from its previous household.

				this.AssignNewHousehold (context, move: true);
			}
			else
			{
				//	The person is attached to several households. Simply delete the contact; this
				//	will remove the association between the person and the selected household.

				context.Register (household);

				AiderContactEntity.Delete (context, contact);
			}
		}
		
		public void RemoveFromHouseholds(BusinessContext context)
		{
			var example = new AiderContactEntity ()
			{
				Person      = this,
				ContactType = Enumerations.ContactType.PersonHousehold,
			};

			var results = context.GetByExample (example);
			var households = results.Select (x => x.Household).Where (x => x.IsNotNull ()).Distinct ().ToList ();

			results.ForEach (x => AiderContactEntity.Delete (context, x));

			AiderHouseholdEntity.DeleteEmptyHouseholds (context, households, keepChildrenOnly: true);
		}

		
		public void HidePerson(BusinessContext businessContext)
		{
			this.Visibility = PersonVisibilityStatus.Hidden;
			this.eCH_Person.RemovalReason = RemovalReason.Unknown;

			if (this.MainContact.IsNotNull ())
			{
				var household = this.MainContact.Household;

				if (household.IsNotNull ())
				{
					this.RemoveFromHouseholds (businessContext);
					AiderHouseholdEntity.DeleteEmptyHouseholds (businessContext, household.ToEnumerable (), true);
					household.RefreshCache ();
				}		
			}
		}

		public void DeleteNonParishGroupParticipations(BusinessContext businessContext)
		{		
			foreach (var participation in this.GetParticipations ())
			{
				var parishGroup = participation.Group.IsParish ();
				var regionGroup = participation.Group.IsRegion ();
				
				if (!parishGroup && !regionGroup)
				{
					this.MainContact.RemoveParticipationInternal (participation);
					businessContext.DeleteEntity (participation);
				}
			}
		}

		public FormattedText GetParishGroupParticipationsNumberedSummary(BusinessContext businessContext)
		{
			var lines  = new List<string> ();
			lines.Add ("Participations locales : ");

			var parishParticipations = this.GetLocalParticipationsOrderedByName ();
			if (!parishParticipations.Any ())
			{
				lines.Add ("Aucune");
				return TextFormatter.FormatText (lines.Join ("\n"));
			}

			var index=1;
			parishParticipations.ForEach (p =>
			{
				var key = index.ToString () + "# ";
				lines.Add (key + p.GetRolePathOrHierarchicalName ());
				index++;
			});

			return TextFormatter.FormatText (lines.Join ("\n"));
		}

		public FormattedText GetParticipationsNumberedSummary(BusinessContext businessContext, IEnumerable<AiderGroupParticipantEntity> participations)
		{
			var lines = new List<string> ();
			lines.Add ("Participations : ");
			if (!participations.Any ())
			{
				lines.Add ("Aucune");
				return TextFormatter.FormatText (lines.Join ("\n"));
			}
			var index=1;
			participations.ForEach (p =>
			{
				var key = index.ToString () + "# ";
				lines.Add (key + p.GetRolePathOrHierarchicalName ());
				index++;
			});

			return TextFormatter.FormatText (lines.Join ("\n"));
		}

		public FormattedText GetNonParishGroupParticipationsNumberedSummary(BusinessContext businessContext)
		{
			var lines = new List<string> ();
			lines.Add ("Participations globales : ");
			var nonParishParticipations = this.GetGlobalParticipationsOrderedByName ();
			if (!nonParishParticipations.Any ())
			{
				lines.Add ("Aucune");
				return TextFormatter.FormatText (lines.Join ("\n"));
			}
			var index=1;
			nonParishParticipations.ForEach (p => 
			{
				var key = index.ToString () + "# ";
				lines.Add (key + p.GetRolePathOrHierarchicalName ());
				index++;
			});

			return TextFormatter.FormatText (lines.Join ("\n"));
		}

		public IEnumerable<AiderGroupParticipantEntity> GetLocalParticipationsOrderedByName()
		{
			return this.GetParticipations ()
									   .Where (p => p.Group.IsInTheSameParish (this.ParishGroup))
									   .OrderBy (p => p.Group.Name);
		}

		public IEnumerable<AiderGroupParticipantEntity> GetOtherParishLevelParticipations()
		{
			return this.GetParticipations ()
									   .Where (p => p.Group.IsInTheSameParish (this.ParishGroup) && p.Group.Name != this.ParishGroup.Name);
		}

		public IEnumerable<AiderGroupParticipantEntity> GetGlobalParticipationsOrderedByName()
		{
			return this.GetParticipations ()
										  .Where (p => !p.Group.IsInTheSameParish (this.ParishGroup))
										  .OrderBy (p => p.Group.Name);
		}

		public void DeleteNumberedParticipationsNotInKeys(BusinessContext businessContext, IEnumerable<AiderGroupParticipantEntity> participations, string keyString, string userLogin)
		{
			var keys = keyString.Split (',');
			var index=1;
			participations.ForEach (p =>
			{
				if (!keys.Contains (InvariantConverter.ToString (index)))
				{
					this.MainContact.RemoveParticipationInternal (p);
					businessContext.DeleteEntity (p);
				}
				else
				{
					p.Comment.SystemText = "warning:ok participation conservée par " + userLogin;
				}
				index++;
			});
		}

		public void DeleteParishGroupParticipation(BusinessContext businessContext)
		{
			foreach (var participation in this.GetParticipations ().Where (p => p.Group == this.ParishGroup))
			{
				this.MainContact.RemoveParticipationInternal (participation);
				businessContext.DeleteEntity (participation);
			}
		}
		
		/// <summary>
		/// Kills the person... which will mark it as deceased, removing any live contacts and
		/// associated households; groups will be remapped to the deceased contact.
		/// </summary>
		/// <param name="businessContext">The business context.</param>
		/// <param name="date">The date of the death.</param>
		/// <param name="uncertain">If set to <c>true</c>, the date is uncertain.</param>
		public void KillPerson(BusinessContext businessContext, Date date, bool uncertain)
		{
			var contacts       = this.Contacts.ToList ();
			var households     = this.Households.ToList ();
			var participations = this.Groups.ToList ();
			var warnings       = this.Warnings.ToList ();

			this.Visibility = PersonVisibilityStatus.Deceased;
			this.eCH_Person.PersonDateOfDeath = date;
			this.eCH_Person.PersonDateOfDeathIsUncertain = uncertain;
			this.eCH_Person.RemovalReason = RemovalReason.Deceased;

			var deadContact = AiderContactEntity.CreateDeceased (businessContext, this);

			//	Update all group participations and replace the contact with the deceased one,
			//	so that we can keep an history:

			foreach (var participation in participations)
			{
				participation.Contact = deadContact;
				participation.EndDate = date;
			}

			foreach (var warning in warnings)
			{
				AiderPersonWarningEntity.Delete (businessContext, warning);
			}

			foreach (var contact in contacts)
			{
				AiderContactEntity.Delete (businessContext, contact);
			}

			AiderHouseholdEntity.DeleteEmptyHouseholds (businessContext, households);
		}

		public static void MergeGroupParticipations(BusinessContext businessContext, AiderPersonEntity officialPerson, AiderPersonEntity otherPerson)
		{
			var officialContact = officialPerson.MainContact;
			var officialGroups  = new HashSet<AiderGroupEntity> (officialPerson.Groups.Select (x => x.Group));

			var participationsToReassign = otherPerson.Groups.Where (g => !officialGroups.Contains (g.Group)).ToList ();
			var duplicateParticipations  = otherPerson.Groups.Where (g => officialGroups.Contains (g.Group)).ToList ();

			//	Migrate groups to the 'official' person by re-affecting the participations from the
			//	other person:

			foreach (var group in participationsToReassign)
			{
				officialPerson.AddParticipationInternal (group);
				otherPerson.RemoveParticipationInternal (group);

				group.Contact = officialContact;
				group.Person  = officialPerson;

				//				var participationData = new ParticipationData (officialPerson);
				//				AiderGroupParticipantEntity.StartParticipation (businessContext, group.Group, participationData, group.StartDate, FormattedText.FromSimpleText ("Fusion"));
			}

			foreach (var group in duplicateParticipations)
			{
				otherPerson.RemoveParticipationInternal (group);

				businessContext.DeleteEntity (group);
			}
		}

		private static void Swap(ref AiderPersonEntity a, ref AiderPersonEntity b)
		{
			var swap = b;

			b = a;
			a = swap;
		}

		private static void AddMergeSystemComment(AiderPersonEntity officialPerson)
		{
			var user = Epsitec.Cresus.Core.Business.UserManagement.UserManager.Current.AuthenticatedUser;
			var name = user.IsNull () ? "le système" : user.DisplayName;
			
			AiderCommentEntity.CombineSystemComments (officialPerson, string.Format ("Fusion par {0} le {1}", name, System.DateTime.Now.ToShortDateString ()));
		}

		private static void MergeContacts(BusinessContext businessContext, AiderPersonEntity officialPerson, AiderPersonEntity otherPerson)
		{
			//	Migrate contacts...

			var household     = officialPerson.Households.FirstOrDefault ();
			var otherContacts = otherPerson.Contacts.ToList ();
			
			foreach (var contact in otherContacts)
			{
				if (contact.Household.IsNull ())
				{
					officialPerson.AddContactInternal (contact);
					otherPerson.RemoveContactInternal (contact);
					contact.Person = officialPerson;
				}
				else
				{
					//	Group participations have been taken care of, but the changes have not yet
					//	been persisted; AiderContactEntity.Delete may not delete the participations
					//	or else, the reassigned participations would be deleted...
					
					AiderContactEntity.Delete (businessContext, contact, deleteParticipations: false);
				}
			}
		}

		/// <summary>
		/// Merges two persons. The official person will be the one which will be kept;
		/// the other person will get deleted. Move as much information from the 'other'
		/// to the official one. If the other person has a governmental eCH record, then
		/// it will be used instead of the official person (swap).
		/// </summary>
		/// <param name="businessContext">The business context.</param>
		/// <param name="officialPerson">The official person.</param>
		/// <param name="otherPerson">The other person.</param>
		/// <returns><c>true</c> if the persons were merged, <c>false</c> otherwise</returns>
		public static bool MergePersons(BusinessContext businessContext, AiderPersonEntity officialPerson, AiderPersonEntity otherPerson)
		{
			if (officialPerson == otherPerson)
			{
				return false;
			}

			if ((officialPerson.eCH_Person.DataSource == Enumerations.DataSource.Government) &&
				(otherPerson.eCH_Person.DataSource == Enumerations.DataSource.Government))
			{
				Logic.BusinessRuleException (officialPerson, "Impossible de fusionner deux personnes provenant du RCH.");
				
				return false;
			}

			if (otherPerson.eCH_Person.DataSource == Enumerations.DataSource.Government)
			{
				AiderPersonEntity.Swap (ref officialPerson, ref otherPerson);
			}
			AiderPersonEntity.CopyAdditionalAddressInfos (officialPerson, otherPerson);
			AiderPersonEntity.MergeSubscriptions (businessContext, officialPerson, otherPerson);
			AiderPersonEntity.MergeGroupParticipations (businessContext, officialPerson, otherPerson);
			AiderPersonEntity.MergeContacts (businessContext, officialPerson, otherPerson);
			AiderCommentEntity.CombineComments (officialPerson, otherPerson.Comment.Text.ToSimpleText ());
			AiderCommentEntity.CombineSystemComments (officialPerson, otherPerson.Comment.SystemText);
			AiderPersonEntity.AddMergeSystemComment (officialPerson);
			AiderContactEntity.DeleteDuplicateContacts (businessContext, officialPerson.Contacts.ToList ());
			AiderPersonEntity.Delete (businessContext, otherPerson);

			return true;
		}

		public static string GetIconName(string prefix, PersonMrMrs? personMrMrs, Language? language = null)
		{
			string suffix;

			switch (personMrMrs)
			{
				case PersonMrMrs.Madame:
				case PersonMrMrs.Mademoiselle:
					suffix = ".AiderPerson.Female-";
					break;
				default:
					suffix = ".AiderPerson.Male-";
					break;
			}

			if (language == Enumerations.Language.German)
			{
				return prefix + suffix + "German";
			}
			else
			{
				return prefix + suffix + "French";
			}
		}

		private static void CopyAdditionalAddressInfos(AiderPersonEntity officialPerson, AiderPersonEntity otherPerson)
		{
			var baseInfos  = officialPerson.Address;
			var infos  = otherPerson.Address;

			if (baseInfos.Email.IsNullOrWhiteSpace () && !infos.Email.IsNullOrWhiteSpace ())
			{
				baseInfos.Email = infos.Email;
			}

			if (baseInfos.Web.IsNullOrWhiteSpace () && !infos.Web.IsNullOrWhiteSpace ())
			{
				baseInfos.Web = infos.Web;
			}

			if (baseInfos.Mobile.IsNullOrWhiteSpace () && !infos.Mobile.IsNullOrWhiteSpace ())
			{
				baseInfos.Mobile = infos.Mobile;
			}

			if (baseInfos.Phone1.IsNullOrWhiteSpace () && !infos.Phone1.IsNullOrWhiteSpace ())
			{
				baseInfos.Phone1 = infos.Phone1;
			}

			if (baseInfos.Phone2.IsNullOrWhiteSpace () && !infos.Phone2.IsNullOrWhiteSpace ())
			{
				baseInfos.Phone2 = infos.Phone2;
			}

			if (baseInfos.Fax.IsNullOrWhiteSpace () && !infos.Fax.IsNullOrWhiteSpace ())
			{
				baseInfos.Fax = infos.Fax;
			}

			AiderCommentEntity.CombineComments (baseInfos, infos.Comment.Text.ToSimpleText ());
		}

		private static void MergeSubscriptions(BusinessContext businessContext, AiderPersonEntity officialPerson, AiderPersonEntity otherPerson)
		{
			var official = AiderSubscriptionEntity.FindSubscriptions (businessContext, officialPerson).ToList ();
			var other    = AiderSubscriptionEntity.FindSubscriptions (businessContext, otherPerson).ToList ();

			foreach (var subscription in other)
			{
				AiderSubscriptionEntity.Delete (businessContext, subscription);
			}
		}
		
		public static void Delete(BusinessContext businessContext, AiderPersonEntity person)
		{
			if (person.IsNull ())
			{
				return;
			}

			var contacts = person.Contacts.ToList ();
			var groups   = person.Groups.ToList ();

			foreach (var contact in contacts)
			{
				AiderContactEntity.Delete (businessContext, contact);
			}

			foreach (var group in groups)
			{
				group.Delete (businessContext);
			}
			
			businessContext.DeleteEntity (person);
		}

		public FormattedText GetGroupTitle()
		{
			int nbGroups = this.Groups.Count;

			return TextFormatter.FormatText ("Groupes (", nbGroups, ")");
		}

		public FormattedText GetGroupText()
		{
			var groups = this.Groups
				.Select (g => g.GetSummaryWithHierarchicalGroupName ())
				.CreateSummarySequence (10, "...");

			var text = TextFormatter.Join ("\n", groups);

			return TextFormatter.FormatText (text);
		}

		public FormattedText GetWarningsTitle()
		{
			return TextFormatter.FormatText ("Averstissements (" + this.Warnings.Count + ")");
		}

		public FormattedText GetWarningsDescription()
		{
			var warnings = this.Warnings
				.Select (w => w.Title)
				.CreateSummarySequence (10, "...");

			var text = TextFormatter.Join ("\n", warnings);

			return TextFormatter.FormatText (text);
		}

		public bool IsMemberOf(AiderGroupEntity group)
		{
			return this.GetMemberships (group).Any ();
		}

		public IEnumerable<AiderGroupParticipantEntity> GetMemberships(AiderGroupEntity group)
		{
			return this.GetParticipations ().Where (g => g.Group == group);
		}

		public IEnumerable<AiderPersonEntity> GetAllHouseholdMembers()
		{
			return this.Households.SelectMany (x => x.Members);
		}

		public AiderContactEntity GetMainContact()
		{
			var mainAddress = this.GetAddress ();
			if (mainAddress.IsNotNull ())
			{
				return this.Contacts.FirstOrDefault (c => c.GetAddress () == mainAddress);
			}
			else
			{
				return this.Contacts.FirstOrDefault ();
			}
		}
		
		public AiderContactEntity GetHouseholdContact()
		{
			return this.Contacts.Where (x => x.Household.IsNotNull ()).FirstOrDefault ();
		}

		public IList<AiderGroupParticipantEntity> GetParticipations(bool reload = false)
		{
			if (this.participations == null || reload)
			{
				this.participations = this.ExecuteWithDataContext (d => this.FindParticipations (d), () => new List<AiderGroupParticipantEntity> ());
			}

			return this.participations;
		}


		internal void RefreshCache()
		{
			//	This is called by AiderPersonBusinessRules.ApplyUpdateRule in order to refresh
			//	the cached data whenever the person data gets edited.

			this.RefreshDisplayName ();
			this.RefreshBirthdayDate ();
		}

		internal void AddParticipationInternal(AiderGroupParticipantEntity participation)
		{
			System.Diagnostics.Debug.Assert (participation.IsNotNull ());

			//	If we assign the person to a group, check to see if the group is a parish
			//	(or the special "no parish" group), in which case we have to update
			//	the ParishGroup property instead:

			if ((participation.Group.IsParish ()) ||
				(participation.Group.IsNoParish ()))
			{
				this.ParishGroup = participation.Group;
			}

			this.GetParticipations ().Add (participation);
		}

		internal void RestoreParticipationInternal(AiderGroupParticipantEntity participation)
		{
			System.Diagnostics.Debug.Assert (participation.IsNotNull ());

			var participations = this.GetParticipations ();
			if (!participations.Contains (participation))
			{
				this.GetParticipations ().Add (participation);
			}
		}

		internal void RemoveParticipationInternal(AiderGroupParticipantEntity participation)
		{
			System.Diagnostics.Debug.Assert (participation.IsNotNull ());

			if (participation.Group == this.ParishGroup)
			{
				this.ParishGroup = null;
			}

			this.GetParticipations ().Remove (participation);
		}

		internal void AddWarningInternal(AiderPersonWarningEntity warning)
		{
			this.GetWarnings ().Add (warning);
		}

		internal void RemoveWarningInternal(AiderPersonWarningEntity warning)
		{
			this.GetWarnings ().Remove (warning);
		}


		internal void ProcessPersonDeath()
		{
			//	TODO: process the death of the person (remove from all associated groups)
		}

		internal void ProcessPersonRevival()
		{
			//	TODO: process the death of the person (remove from all associated groups)
		}


		partial void OnParishGroupChanging(AiderGroupEntity oldValue, AiderGroupEntity newValue)
		{
			this.ParishGroupPathCache = AiderGroupEntity.GetPath (newValue);
		}

		partial void OnVisibilityChanging(PersonVisibilityStatus oldValue, PersonVisibilityStatus newValue)
		{
			switch (oldValue)
			{
				case PersonVisibilityStatus.Deceased:
					this.ProcessPersonRevival ();
					break;
			}

			switch (newValue)
			{
				case PersonVisibilityStatus.Deceased:
					this.ProcessPersonDeath ();
					break;
			}
		}



		partial void GetAllEmails(ref string value)
		{
			var addresses = this.Contacts.Select (x => x.Address).Concat (this.Households.Select (x => x.Address)).Where (x => x.IsNotNull ());
			var emails = addresses.Select (x => x.Email).Where (x => string.IsNullOrEmpty (x) == false).Distinct ();

			value = string.Join ("; ", emails);
		}

		partial void SetAllEmails(string value)
		{
			throw new System.NotImplementedException ();
		}

		partial void GetAllPhoneNumbers(ref string value)
		{
			var addresses = this.Contacts.Select (x => x.Address).Concat (this.Households.Select (x => x.Address)).Where (x => x.IsNotNull ());
			var phones = addresses.Select (x => x.Phone1).Concat (addresses.Select (x => x.Phone2)).Concat (addresses.Select (x => x.Mobile)).Where (x => string.IsNullOrEmpty (x) == false).Distinct ();

			value = string.Join ("; ", phones);
		}

		partial void SetAllPhoneNumbers(string value)
		{
			throw new System.NotImplementedException ();
		}

		partial void GetAddress(ref AiderAddressEntity value)
		{
			value = this.GetAddress ();
		}

		partial void SetAddress(AiderAddressEntity value)
		{
			throw new System.NotImplementedException ();
		}

		partial void GetMainContact(ref AiderContactEntity value)
		{
			value = this.GetMainContact ();
		}

		public string GetMainPhone()
		{
			return AiderContactsHelpers.GetMainPhone (this.Contacts);
		}

		public string GetMainEmail()
		{
			return AiderContactsHelpers.GetMainEmail (this.Contacts);
		}


		public string GetSecondaryPhone()
		{
			return AiderContactsHelpers.GetSecondaryPhone (this.Contacts);
		}

		public string GetSecondaryEmail()
		{
			return AiderContactsHelpers.GetSecondaryEmail (this.Contacts);
		}

		partial void GetMainEmail(ref string value)
		{
			value = this.GetMainEmail ();
		}

		partial void GetMainPhone(ref string value)
		{
			value = this.GetMainPhone ();
		}

		partial void GetSecondaryEmail(ref string value)
		{
			value = this.GetSecondaryEmail ();
		}

		partial void GetSecondaryPhone(ref string value)
		{
			value = this.GetSecondaryPhone ();
		}

		partial void SetMainContact(AiderContactEntity value)
		{
			throw new System.NotImplementedException ();
		}

		partial void GetHouseholdContact(ref AiderContactEntity value)
		{
			value = this.GetHouseholdContact ();
		}

		partial void SetHouseholdContact(AiderContactEntity value)
		{
			throw new System.NotImplementedException ();
		}

		partial void GetGroups(ref IList<AiderGroupParticipantEntity> value)
		{
			value = this.GetParticipations ().AsReadOnlyCollection ();
		}

		partial void GetHouseholds(ref IList<AiderHouseholdEntity> value)
		{
			value = this.GetHouseholds ().OrderBy (x => x.DisplayName).AsReadOnlyCollection ();
		}

		partial void GetContacts(ref IList<AiderContactEntity> value)
		{
			value = this.GetContacts ().OrderBy (x => x.DisplayAddress).AsReadOnlyCollection ();
		}

		partial void GetAdditionalAddresses(ref IList<AiderContactEntity> value)
		{
			value = this.GetContacts ()
				.Where (x => x.ContactType == ContactType.PersonAddress)
				.Where (x => x.AddressType != AddressType.Confidential)
				.OrderBy (x => x.DisplayAddress)
				.AsReadOnlyCollection ();
		}

		partial void GetConfidentialAddresses(ref IList<AiderContactEntity> value)
		{
			value = this.GetContacts ()
				.Where (x => x.ContactType == ContactType.PersonAddress)
				.Where (x => x.AddressType == AddressType.Confidential)
				.OrderBy (x => x.DisplayAddress)
				.AsReadOnlyCollection ();
		}

		partial void GetWarnings(ref IList<AiderPersonWarningEntity> value)
		{
			value = this.GetWarnings ().AsReadOnlyCollection ();
		}

		partial void GetCallNameDisplay(ref string value)
		{
			value = this.GetCallName ();
		}

		partial void SetCallNameDisplay(string value)
		{
			throw new System.NotSupportedException ("Do not call this method.");
		}

		partial void GetEmployee(ref AiderEmployeeEntity value)
		{
			value = this.GetVirtualCollection (ref this.employees, x => x.Person = this).FirstOrDefault ();
		}

		partial void SetEmployee(AiderEmployeeEntity value)
		{
			throw new System.NotImplementedException ();
		}


		private AiderAddressEntity GetAddress()
		{
			//	A person's address is the one which was explicitely defined to be the default
			//	(AddressType = Default), or the first household address, or any fully defined
			//	available address for the person if everything else failed:

			var defaultAddress = 
				this.AdditionalAddresses.Where (x => x.AddressType == AddressType.Default).Select (x => x.Address).FirstOrDefault () ??
				this.Households.Select (x => x.Address).FirstOrDefault () ??
				this.AdditionalAddresses.Where (x => x.HasFullAddress ()).Select (x => x.Address).FirstOrDefault ();

			return defaultAddress;
		}

		private ISet<AiderHouseholdEntity> GetHouseholds()
		{
			if (this.households == null)
			{
				this.households = this.Contacts
					.Where (x => x.Household.IsNotNull ())
					.Select (x => x.Household)
					.ToSet ();
			}

			return this.households;
		}

		private ISet<AiderContactEntity> GetContacts()
		{
			if (this.contacts == null)
			{
				this.contacts = this.ExecuteWithDataContext (d => this.FindContacts (d).ToSet (), () => new HashSet<AiderContactEntity> ());
			}

			return this.contacts;
		}

		private IList<AiderPersonWarningEntity> GetWarnings()
		{
			if (this.warnings == null)
			{
				this.warnings = this.ExecuteWithDataContext (d => this.FindWarnings (d), () => new List<AiderPersonWarningEntity> ());
			}

			return this.warnings;
		}

		private IList<AiderGroupParticipantEntity> FindParticipations(DataContext dataContext)
		{
			var request = AiderGroupParticipantEntity.CreateParticipantRequest (dataContext, this, true);

			return dataContext
				.GetByRequest<AiderGroupParticipantEntity> (request)
				.OrderBy (g => g.GetSummaryWithHierarchicalGroupName ().ToString ())
				.ToList ();
		}

		private IList<AiderContactEntity> FindContacts(DataContext dataContext)
		{
			var example = new AiderContactEntity ()
			{
				Person = this
			};

			return dataContext.GetByExample (example);
		}

		private IList<AiderPersonWarningEntity> FindWarnings(DataContext dataContext)
		{
			var controller = AiderWarningController.Current;

			return controller
				.GetWarnings<AiderPersonWarningEntity> (this)
				.ToList ();
		}

		private void RefreshDisplayName()
		{
			this.DisplayName = this.GetDisplayName ();
		}

		private void RefreshBirthdayDate()
		{
			Date? date = null;

			if (this.eCH_Person.IsNotNull ())
			{
				date = this.eCH_Person.PersonDateOfBirth;
			}

			if (date.HasValue == false)
			{
				this.BirthdayDay   = 0;
				this.BirthdayMonth = 0;
				this.BirthdayYear  = 0;
			}
			else
			{
				this.BirthdayDay   = date.Value.Day;
				this.BirthdayMonth = date.Value.Month;
				this.BirthdayYear  = date.Value.Year;
			}
		}


		private FormattedText GetCoordinatesSummary()
		{
			var lines = this.GetCoordinatesSummaryLines ();

			return TextFormatter.FormatText (lines.Select (x => x.AppendLineIfNotNull ()));
		}

		private IEnumerable<FormattedText> GetCoordinatesSummaryLines()
		{
			// Gets the full name
			var fullNameText = this.GetFullName ();

			if (!fullNameText.IsNullOrWhiteSpace ())
			{
				yield return fullNameText;
			}

			//	Gets the default mail address
			//	TODO: ...
		}



		public void AddContactInternal(AiderContactEntity contact)
		{
			this.GetContacts ().Add (contact);
			this.ClearHouseholdCache ();
		}

		public void RemoveContactInternal(AiderContactEntity contact)
		{
			this.GetContacts ().Remove (contact);
			this.ClearHouseholdCache ();
		}


		private void ClearHouseholdCache()
		{
			this.households = null;
		}




		#region IAiderWarningExampleFactoryGetter Members

		AiderWarningExampleFactory IAiderWarningExampleFactoryGetter.GetWarningExampleFactory()
		{
			return AiderPersonEntity.warningExampleFactory;
		}

		#endregion


		public static readonly string			DeceasedSuffix = " †";

		private static readonly AiderWarningExampleFactory warningExampleFactory = new AiderWarningExampleFactory<AiderPersonEntity, AiderPersonWarningEntity> ((example, source) => example.Person = source);

		private IList<AiderGroupParticipantEntity>	participations;
		private IList<AiderPersonWarningEntity>		warnings;
		private ISet<AiderEmployeeEntity>			employees;
		private ISet<AiderHouseholdEntity>			households;
		private ISet<AiderContactEntity>			contacts;
	}
}
