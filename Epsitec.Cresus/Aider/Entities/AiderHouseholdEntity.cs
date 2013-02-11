//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

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
			this.DisplayName = this.GetDisplayName ();
			this.DisplayZipCode = this.GetDisplayZipCode ();
			this.DisplayAddress = this.GetDisplayAddress ();
			this.ParishGroupPathCache = this.GetParishGroupPathCache ();
		}


		private string GetDisplayZipCode()
		{
			return this.Address.GetDisplayZipCode ().ToSimpleText ();
		}


		private string GetDisplayAddress()
		{
			return this.Address.GetDisplayAddress ().ToSimpleText ();
		}


		private string GetDisplayName()
		{
			if (string.IsNullOrEmpty (this.HouseholdName))
			{
				return AiderHouseholdEntity.BuildDisplayName (this.GetContacts (), this.HouseholdMrMrs);
			}
			
			return this.HouseholdName;
		}


		private string GetParishGroupPathCache()
		{
			// The logic here is very simple. Maybe we need something more complex.

			return this.Members
				.Select (m => m.GetParishGroupPathCache ())
				.Where (p => !string.IsNullOrEmpty (p))
				.FirstOrDefault ();
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


		public void Delete(BusinessContext businessContext)
		{
			foreach (var contact in this.Contacts.ToArray ())
			{
				AiderContactEntity.Delete (businessContext, contact);
			}

			if (this.Comment.IsNotNull ())
			{
				businessContext.DeleteEntity (this.Comment);
			}

			if (this.Address.IsNotNull ())
			{
				businessContext.DeleteEntity (this.Address);
			}

			businessContext.DeleteEntity (this);
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

		private static string BuildDisplayName(IList<AiderContactEntity> contacts, HouseholdMrMrs order)
		{
			if (contacts == null)
			{
				return null;
			}
			
			var heads    = contacts.Where (x => x.HouseholdRole == HouseholdRole.Head).Select (x => x.Person.eCH_Person).ToList ();
			var children = contacts.Where (x => x.HouseholdRole != HouseholdRole.Head).Select (x => x.Person.eCH_Person).ToList ();

			return AiderHouseholdEntity.BuildDisplayName (heads, children, order);
		}
			
		private static string BuildDisplayName(IEnumerable<eCH_PersonEntity> heads, IEnumerable<eCH_PersonEntity> children, HouseholdMrMrs order)
		{
			var men   = heads.Where (x => x.PersonSex == PersonSex.Male);
			var women = heads.Where (x => x.PersonSex == PersonSex.Female);

			var headNames = new List<string> ();

			switch (order)
			{
				case HouseholdMrMrs.None:
				case HouseholdMrMrs.Auto:
				case HouseholdMrMrs.Famille:
				case HouseholdMrMrs.MonsieurEtMadame:
					headNames.AddRange (men.Select (x => x.PersonOfficialName));
					headNames.AddRange (women.Select (x => x.PersonOfficialName));
					break;
				case HouseholdMrMrs.MadameEtMonsieur:
					headNames.AddRange (women.Select (x => x.PersonOfficialName));
					headNames.AddRange (men.Select (x => x.PersonOfficialName));
					break;
			}

			if (headNames.Count == 0)
			{
				var child = children.Select (x => x.PersonOfficialName).FirstOrDefault ();

				if (child != null)
				{
					headNames.Add (child);
				}
			}

			var    members   = heads.Concat (children).ToList ();
			string headTitle = null;
			
			switch (order)
			{
				case HouseholdMrMrs.None:
				case HouseholdMrMrs.Auto:
				case HouseholdMrMrs.Famille:
					if (members.Count == 1)
					{
						switch (members[0].PersonSex)
						{
							case PersonSex.Female:
								headTitle = "Mme";
								break;
							case PersonSex.Male:
								headTitle = "M.";
								break;
						}
					}
					break;

				case HouseholdMrMrs.MonsieurEtMadame:
					headTitle = "M. et Mme";
					break;

				case HouseholdMrMrs.MadameEtMonsieur:
					headTitle = "Mme et M.";
					break;
			}


			headNames.Insert (0, headTitle ?? "Famille");

			return StringUtils.Join (" ", headNames.Distinct ().ToArray ());
		}


		private IList<AiderPersonEntity> GetMembers()
		{
			if (this.membersCache == null)
			{
				this.membersCache = AiderHouseholdEntity.GetMembers (this.GetContacts ());
			}

			return this.membersCache;
		}

		private IList<AiderContactEntity> GetContacts()
		{
			if (this.contactsCache == null)
			{
				this.contactsCache = new List<AiderContactEntity> ();

				var dataContext = DataContextPool.GetDataContext (this);

				if ((dataContext != null) &&
					(dataContext.IsPersistent (this)))
				{
					var example = new AiderContactEntity ()
					{
						Household = this,
					};

					var contacts = dataContext.GetByExample (example);
					var alive    = contacts.Where (x => x.Person.IsAlive);

					this.contactsCache.AddRange (alive);
				}
			}

			return this.contactsCache;
		}

		private static List<AiderPersonEntity> GetMembers(IList<AiderContactEntity> contacts)
		{
			var members = new List<AiderPersonEntity> ();

			var heads   = contacts.Where (x => x.HouseholdRole == HouseholdRole.Head).Select (x => x.Person).OrderBy (x => x.eCH_Person.PersonDateOfBirth);
			var others  = contacts.Where (x => x.HouseholdRole != HouseholdRole.Head).Select (x => x.Person).OrderBy (x => x.eCH_Person.PersonDateOfBirth);

			members.AddRange (heads);
			members.AddRange (others);

			return members;
		}

		
		private void ClearMemberCache()
		{
			this.membersCache = null;
		}



		//	These properties are only meant as an in memory cache of the members of the household.
		//	They will never be saved to the database:
		private List<AiderContactEntity>		contactsCache;
		private List<AiderPersonEntity>			membersCache;
	}
}
