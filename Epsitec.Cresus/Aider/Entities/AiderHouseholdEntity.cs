//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Text;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Entities
{
	public partial class AiderHouseholdEntity
	{
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.DisplayName, "~,", this.Address.GetStreetZipAndTownAddress ());
		}

		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.DisplayName, "~\n", this.Address.GetPostalAddress ());
		}

		public void RefreshCache()
		{
			this.DisplayName          = this.GetDisplayName ();
			this.DisplayZipCode       = this.GetDisplayZipCode ();
			this.DisplayAddress       = this.GetDisplayAddress ();
			this.ParishGroupPathCache = AiderGroupEntity.GetPath (this.GetParishGroup ());
		}

		public bool IsHead(AiderPersonEntity person)
		{
			return this.Contacts.Any (c => c.Person == person && c.HouseholdRole == HouseholdRole.Head);
		}

		public FormattedText GetAddressLabelText()
		{
			return TextFormatter.FormatText
			(
				this.GetAddressRecipientText (false, 35),
				"\n",
				this.Address.GetPostalAddress ()
			);
		}


		private FormattedText GetAddressRecipientText(bool abbreviatedTitle, int lineLength)
		{
			if (this.Contacts.Count == 0)
			{
				// This may happen for corrupted households.

				return FormattedText.Empty;
			}

			return TextFormatter.FormatText
			(
				this.GetHonorific (abbreviatedTitle),
				"\n",
				this.GetAddressName ().BreakInLines (lineLength)
			);
		}

		public FormattedText GetMembersTitle()
		{
			var nbMembers = this.Members.Count;

			return TextFormatter.FormatText ("Membres (", nbMembers, ")");
		}

		public FormattedText GetMembersSummary()
		{
			var members = this.Members
				.Select (m => m.GetCompactSummary ())
				.CreateSummarySequence (10, "...");

			return FormattedText.Join (FormattedText.FromSimpleText ("\n"), members);
		}


		public string GetHonorific(bool abbreviated)
		{
			var members = this.GetMembers ();

			// If we have a single member, we return its title instead of the title of the
			// household.
			if (members.Count == 1)
			{
				return members[0].MrMrs.GetText (abbreviated);
			}

			var honorific = this.HouseholdMrMrs;
			switch (honorific)
			{
				case HouseholdMrMrs.MonsieurEtMadame:
				case HouseholdMrMrs.MadameEtMonsieur:
					// If we have a single head and some children, we use the "Family" title.
					if (this.GetHeads ().Count == 1)
					{
						goto case HouseholdMrMrs.Famille;
					}

					// Only if we have 2 heads, do we use the real title.
					return honorific.GetText (abbreviated);

				case HouseholdMrMrs.Famille:
				case HouseholdMrMrs.None:
				case HouseholdMrMrs.Auto:
					return HouseholdMrMrs.Famille.GetText (abbreviated);

				default:
					throw new NotImplementedException ();
			}
		}


		private string GetAddressName()
		{
			if (this.GetMembers ().Count == 0)
			{
				return "";
			}

			var names = this.GetHeadNames ();
			var firstnames = names.Item1;
			var lastnames = names.Item2;

			if (firstnames.Count == 1)
			{
				return firstnames[0] + " " + lastnames[0];
			}
			else if (firstnames.Count == 2 && lastnames.Count == 1)
			{
				return firstnames[0] + " et " + firstnames[1] + " " + lastnames[0];
			}
			else if (firstnames.Count == 2 && lastnames.Count == 2)
			{
				return firstnames[0] + " " + lastnames[0] + " et "
					+ firstnames[1] + " " + lastnames[1];
			}
			else
			{
				throw new NotImplementedException ();
			}
		}


		public Tuple<List<string>, List<string>> GetHeadNames()
		{
			var heads = this.GetHeadForNames ();

			var firstnames = heads
				.Select (p => p.GetCallName ())
				.ToList ();

			var lastnames = !string.IsNullOrEmpty (this.HouseholdName)
				? new List<string> () { this.HouseholdName }
				: heads.Select (p => p.eCH_Person.PersonOfficialName).ToList ();

			lastnames = NameProcessor.FilterLastnamePseudoDuplicates (lastnames);

			return Tuple.Create (firstnames, lastnames);
		}


		private List<AiderPersonEntity> GetHeadForNames()
		{
			var heads = this.GetHeads ();

			var man = heads
				.Where (x => x.eCH_Person.PersonSex == PersonSex.Male)
				.FirstOrDefault ();

			var woman = heads
				.Where (x => x.eCH_Person.PersonSex == PersonSex.Female)
				.FirstOrDefault ();

			var result = new List<AiderPersonEntity> ();

			switch (this.HouseholdMrMrs)
			{
				case HouseholdMrMrs.None:
				case HouseholdMrMrs.Auto:
				case HouseholdMrMrs.Famille:
				case HouseholdMrMrs.MonsieurEtMadame:
					if (man != null)
					{
						result.Add (man);
					}
					if (woman != null)
					{
						result.Add (woman);
					}
					break;

				case HouseholdMrMrs.MadameEtMonsieur:
					if (woman != null)
					{
						result.Add (woman);
					}
					if (man != null)
					{
						result.Add (man);
					}
					break;

				default:
					throw new NotImplementedException ();
			}

			if (result.Count == 0)
			{
				var noSex = heads
					.Where (x => x.eCH_Person.PersonSex == PersonSex.Unknown)
					.FirstOrDefault ();

				if (noSex != null)
				{
					result.Add (noSex);
				}
			}

			if (result.Count == 0)
			{
				var child = this.GetChildren ().FirstOrDefault ();

				if (child != null)
				{
					result.Add (child);
				}
			}

			return result;
		}


		public static AiderHouseholdEntity Create(BusinessContext context, AiderAddressEntity templateAddress = null)
		{
			var household = context.CreateAndRegisterEntity<AiderHouseholdEntity> ();
			
			household.Address = AiderAddressEntity.Create (context, templateAddress);

			return household;
		}

		public static void Delete(BusinessContext businessContext, AiderHouseholdEntity household)
		{
			foreach (var contact in household.Contacts.ToArray ())
			{
				AiderContactEntity.Delete (businessContext, contact);
			}

			if (household.Comment.IsNotNull ())
			{
				businessContext.DeleteEntity (household.Comment);
			}

			if (household.Address.IsNotNull ())
			{
				businessContext.DeleteEntity (household.Address);
			}

			businessContext.DeleteEntity (household);
		}


		internal void AddContactInternal(AiderContactEntity contact)
		{
			this.GetContacts ().Add (contact);
			this.ClearMemberCache ();
		}

		internal void RemoveContactInternal(AiderContactEntity contact)
		{
			this.GetContacts ().Remove (contact);
			this.ClearMemberCache ();
		}

		
		partial void GetMembers(ref IList<AiderPersonEntity> value)
		{
			value = this.GetMembers ().AsReadOnlyCollection ();
		}

		partial void GetContacts(ref IList<AiderContactEntity> value)
		{
			value = this.GetContacts ().AsReadOnlyCollection ();
		}
		

		private IList<AiderPersonEntity> GetMembers()
		{
			if (this.membersCache == null)
			{
				var heads = this.GetHeads ().OrderBy (x => x.eCH_Person.PersonDateOfBirth);
				var children = this.GetChildren ().OrderBy (x => x.eCH_Person.PersonDateOfBirth);

				this.membersCache = heads.Concat (children).ToList ();
			}

			return this.membersCache;
		}

		private IList<AiderContactEntity> GetContacts()
		{
			if (this.contactsCache == null)
			{
				this.contactsCache = this.ExecuteWithDataContext
				(
					d => this.GetContacts (d),
					() => new List<AiderContactEntity> ()
				);
			}

			return this.contactsCache;
		}

		private IList<AiderContactEntity> GetContacts(DataContext dataContext)
		{
			var example = new AiderContactEntity ()
			{
				Household = this,
			};

			return dataContext.GetByExample (example)
				.Where (x => x.Person.IsAlive)
				.ToList ();
		}

		
		private void ClearMemberCache()
		{
			this.membersCache = null;
		}


		private string GetDisplayZipCode()
		{
			return this.Address.GetDisplayZipCode ().ToSimpleText ();
		}

		private string GetDisplayAddress()
		{
			return this.Address.GetDisplayAddress ().ToSimpleText ();
		}


		public string GetDisplayName()
		{
			if (this.GetMembers ().Count == 0)
			{
				return "";
			}

			var names = this.GetHeadNames ();
			var firstnames = names.Item1;
			var lastnames = names.Item2;

			if (firstnames.Count == 1)
			{
				return lastnames[0] + ", " + firstnames[0];
			}
			else if (firstnames.Count == 2 && lastnames.Count == 1)
			{
				return lastnames[0] + ", " + firstnames[0] + " et " + firstnames[1];

			}
			else if (firstnames.Count == 2 && lastnames.Count == 2)
			{
				return lastnames[0] + ", " + firstnames[0] + " et "
					+ lastnames[1] + ", " + firstnames[1];
			}
			else
			{
				throw new NotImplementedException ();
			}
		}

		
		private AiderGroupEntity GetParishGroup()
		{
			//	With the AIDER data model, we cannot represent households where persons
			//	belong to different parishes. Just pick the parish of the first one.

			return this.Members
				.Select (m => m.ParishGroup)
				.FirstOrDefault (p => p.IsNotNull ());
		}


		private IList<AiderPersonEntity> GetHeads()
		{
			return this.GetMembers (HouseholdRole.Head);
		}


		private IList<AiderPersonEntity> GetChildren()
		{
			return this.GetMembers (HouseholdRole.None);
		}


		private IList<AiderPersonEntity> GetMembers(HouseholdRole role)
		{
			return this.GetContacts ()
				.Where (x => x.HouseholdRole == role)
				.Select (x => x.Person)
				.ToList ();
		}


		//	These properties are only meant as an in memory cache of the members of the household.
		//	They will never be saved to the database:
		private IList<AiderContactEntity>		contactsCache;
		private IList<AiderPersonEntity>		membersCache;
	}
}
