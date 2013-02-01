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
			if (string.IsNullOrEmpty (this.DisplayName))
			{
				this.RefreshCache ();
			}

			return TextFormatter.FormatText (this.DisplayName, "~,", this.Address.GetStreetZipAndTownAddress ());
		}

		public override FormattedText GetSummary()
		{
			if (string.IsNullOrEmpty (this.DisplayName))
			{
				this.RefreshCache ();
			}

			return TextFormatter.FormatText (this.DisplayName, "~\n", this.Address.GetPostalAddress ());
		}

		public void RefreshCache()
		{
			if (string.IsNullOrEmpty (this.HouseholdName))
			{
				this.DisplayName = this.BuildDisplayName ();
			}
			else
			{
				this.DisplayName = this.HouseholdName;
			}
		}

		private string BuildDisplayName()
		{
			var headTitle = "";
			var headNames = new List<string> ();

			this.GetMembers ();

			var heads = this.contacts.Where (x => x.HouseholdRole == Enumerations.HouseholdRole.Head).Select (x => x.Person).ToList ();
			var men   = heads.Where (x => x.eCH_Person.PersonSex == Enumerations.PersonSex.Male);
			var women = heads.Where (x => x.eCH_Person.PersonSex == Enumerations.PersonSex.Female);

			var order = this.HouseholdMrMrs;

			switch (order)
			{
				case Enumerations.HouseholdMrMrs.None:
				case Enumerations.HouseholdMrMrs.Auto:
				case Enumerations.HouseholdMrMrs.Famille:
				case Enumerations.HouseholdMrMrs.MonsieurEtMadame:
					headNames.AddRange (men.Select (x => x.GetShortFullName ()));
					headNames.AddRange (women.Select (x => x.GetShortFullName ()));
					break;
				case Enumerations.HouseholdMrMrs.MadameEtMonsieur:
					headNames.AddRange (women.Select (x => x.GetShortFullName ()));
					headNames.AddRange (men.Select (x => x.GetShortFullName ()));
					break;
			}

			if (headNames.Count == 0)
			{
				var contact = this.contacts.Select (x => x.Person.GetShortFullName ()).FirstOrDefault ();

				if (contact != null)
				{
					headNames.Add (contact);
				}
			}

			switch (order)
			{
				case Enumerations.HouseholdMrMrs.None:
				case Enumerations.HouseholdMrMrs.Auto:
				case Enumerations.HouseholdMrMrs.Famille:
					headTitle = this.members.Count == 1 ? EnumKeyValues.GetEnumKeyValue (this.members[0].MrMrs).Values.LastOrDefault ().ToSimpleText () : "Famille";
					break;

				case Enumerations.HouseholdMrMrs.MonsieurEtMadame:
					headTitle = "M. et Mme";
					break;

				case Enumerations.HouseholdMrMrs.MadameEtMonsieur:
					headTitle = "Mme et M.";
					break;
			}

			headNames.Insert (0, headTitle);

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


		private IList<AiderPersonEntity> GetMembers()
		{
			if (this.members == null)
			{
				this.members  = new List<AiderPersonEntity> ();

				var contacts = this.GetContacts ();
				var heads    = contacts.Where (x => x.HouseholdRole == Enumerations.HouseholdRole.Head).Select (x => x.Person).OrderBy (x => x.eCH_Person.PersonDateOfBirth);
				var others   = contacts.Where (x => x.HouseholdRole != Enumerations.HouseholdRole.Head).Select (x => x.Person).OrderBy (x => x.eCH_Person.PersonDateOfBirth);

				this.members.AddRange (heads);
				this.members.AddRange (others);
			}

			return this.members;
		}


		private IList<AiderContactEntity> GetContacts()
		{
			if (this.contacts == null)
			{
				this.contacts = new List<AiderContactEntity> ();

				var businessContext = BusinessContextPool.GetCurrentContext (this);
				var dataContext = businessContext.DataContext;

				if (dataContext.IsPersistent (this))
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


		public void Add(AiderPersonEntity newMember)
		{
			this.GetMembers ().Add (newMember);

			if (this.Head1.IsNull ())
			{
				this.Head1 = newMember;
			}
		}


		public void Remove(AiderPersonEntity oldMember)
		{
			this.GetMembers ().Remove (oldMember);

			if (this.Head1 == oldMember)
			{
				this.Head1 = null;
			}
			else if (this.Head2 == oldMember)
			{
				this.Head2 = null;
			}
		}


		public IEnumerable<AiderPersonEntity> GetHeads()
		{
			var head1 = this.Head1;

			if (head1.IsNotNull ())
			{
				yield return head1;
			}

			var head2 = this.Head2;

			if (head2.IsNotNull ())
			{
				yield return head2;
			}
		}


		public string GetDefaultLastname()
		{
			if (this.Head1.IsNotNull ())
			{
				return this.Head1.eCH_Person.PersonOfficialName;
			}
			else if (this.Head2.IsNotNull ())
			{
				return this.Head2.eCH_Person.PersonOfficialName;
			}
			else
			{
				return "";
			}
		}



		// This property is only meant as an in memory cache of the members of the household. It
		// will never be saved to the database.
		private List<AiderContactEntity> contacts;
		private List<AiderPersonEntity> members;


	}


}
