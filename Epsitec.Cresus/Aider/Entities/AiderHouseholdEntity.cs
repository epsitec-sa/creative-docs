//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;

using System.Linq;
using Epsitec.Cresus.DataLayer.Context;


namespace Epsitec.Aider.Entities
{


	public partial class AiderHouseholdEntity
	{
		public IList<AiderContactEntity> Contacts
		{
			get
			{
				return this.GetContacts ().AsReadOnlyCollection ();
			}
		}

		public override FormattedText GetCompactSummary()
		{
			this.RefreshCacheIfNeeded ();

			return TextFormatter.FormatText (this.DisplayName, "~,", this.Address.GetStreetZipAndTownAddress ());
		}

		public override FormattedText GetSummary()
		{
			this.RefreshCacheIfNeeded ();

			return TextFormatter.FormatText (this.DisplayName, "~\n", this.Address.GetPostalAddress ());
		}

		public void RefreshCacheIfNeeded()
		{
			if (string.IsNullOrEmpty (this.DisplayName))
			{
				this.RefreshCache ();
			}
		}

		public void RefreshCache()
		{
			if (string.IsNullOrEmpty (this.HouseholdName))
			{
				this.DisplayName = this.BuildDisplayName () ?? this.DisplayName;
			}
			else
			{
				this.DisplayName = this.HouseholdName;
			}
		}

		private string BuildDisplayName()
		{
			return AiderHouseholdEntity.BuildDisplayName (this.GetContacts (), this.HouseholdMrMrs);
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

			return FormattedText.Join (FormattedText.FromSimpleText("\n"), members);
		}


		partial void GetMembers(ref IList<AiderPersonEntity> value)
		{
			value = this.GetMembers ().AsReadOnlyCollection ();
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

		private IList<AiderPersonEntity> GetMembers()
		{
			if (this.members == null)
			{
				this.members = AiderHouseholdEntity.GetMembers (this.GetContacts ());
			}

			return this.members;
		}


		private IList<AiderContactEntity> GetContacts()
		{
			if (this.contacts == null)
			{
				this.contacts = new List<AiderContactEntity> ();

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

					this.contacts.AddRange (alive);
				}
			}

			return this.contacts;
		}


		public void AddContactInternal(AiderContactEntity contact)
		{
			this.GetContacts ().Add (contact);
			this.ClearMemberCache ();			
		}


		public void RemoveContactInternal(AiderContactEntity contact)
		{
			this.GetContacts ().Remove (contact);
			this.ClearMemberCache ();
		}


		private void ClearMemberCache()
		{
			this.members = null;
		}


		// This property is only meant as an in memory cache of the members of the household. It
		// will never be saved to the database.
		private List<AiderContactEntity> contacts;
		private List<AiderPersonEntity> members;


	}


}
