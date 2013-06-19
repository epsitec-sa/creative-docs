﻿//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System;

using System.Collections.Generic;

using System.Linq;
using Epsitec.Common.Types.Converters;


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
			var honorific = this.HouseholdMrMrs;

			var contacts = this.GetContacts ();
			var heads = AiderHouseholdEntity.GetHeads (contacts);
			var children = AiderHouseholdEntity.GetChildren (contacts);

			return AiderHouseholdEntity.GetHeadTitle (honorific, heads, children, abbreviated);
		}


		public List<string> GetFirstnames()
		{
			var honorific = this.HouseholdMrMrs;

			var contacts = this.GetContacts ();
			var heads = AiderHouseholdEntity.GetHeads (contacts);
			var children = AiderHouseholdEntity.GetChildren (contacts);

			return AiderHouseholdEntity.GetHeadFirstnames (honorific, heads, children);
		}


		public List<string> GetLastnames()
		{
			var honorific = this.HouseholdMrMrs;

			var contacts = this.GetContacts ();
			var heads = AiderHouseholdEntity.GetHeads (contacts);
			var children = AiderHouseholdEntity.GetChildren (contacts);

			return AiderHouseholdEntity.GetHeadLastnames (honorific, heads, children, true);
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
				this.membersCache = this.GetMembers (this.GetContacts ());
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

		private List<AiderPersonEntity> GetMembers(IList<AiderContactEntity> contacts)
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
			if (string.IsNullOrEmpty (this.HouseholdName))
			{
				return AiderHouseholdEntity.BuildDisplayName (this.GetContacts (), this.HouseholdMrMrs);
			}
			
			return this.HouseholdName;
		}

		
		private AiderGroupEntity GetParishGroup()
		{
			//	With the AIDER data model, we cannot represent households where persons
			//	belong to different parishes. Just pick the parish of the first one.

			return this.Members
				.Select (m => m.ParishGroup)
				.FirstOrDefault (p => p.IsNotNull ());
		}


		private static string BuildDisplayName(IList<AiderContactEntity> contacts, HouseholdMrMrs order)
		{
			if (contacts == null)
			{
				return null;
			}

			var heads = AiderHouseholdEntity.GetHeads (contacts);
			var children = AiderHouseholdEntity.GetChildren (contacts);

			return AiderHouseholdEntity.BuildDisplayName (heads, children, order);
		}

		private static string BuildDisplayName(IList<AiderPersonEntity> heads, IList<AiderPersonEntity> children, HouseholdMrMrs order)
		{
			var headTitle = AiderHouseholdEntity.GetHeadTitle (order, heads, children, true);

			var headLastnames = AiderHouseholdEntity.GetHeadLastnames (order, heads, children, removePseudoDuplicates: true);
			var headLastname = StringUtils.Join (" ", headLastnames);

			return StringUtils.Join (" ", headTitle, headLastname);
		}


		private static IList<AiderPersonEntity> GetHeads(IEnumerable<AiderContactEntity> contacts)
		{
			return AiderHouseholdEntity.GetMembers (contacts, HouseholdRole.Head);
		}


		private static IList<AiderPersonEntity> GetChildren(IEnumerable<AiderContactEntity> contacts)
		{
			return AiderHouseholdEntity.GetMembers (contacts, HouseholdRole.None);
		}


		private static IList<AiderPersonEntity> GetMembers(IEnumerable<AiderContactEntity> contacts, HouseholdRole role)
		{
			return contacts
				.Where (x => x.HouseholdRole == role)
				.Select (x => x.Person)
				.ToList ();
		}


		private static List<string> GetHeadLastnames(HouseholdMrMrs order, IEnumerable<AiderPersonEntity> heads, IEnumerable<AiderPersonEntity> children, bool removePseudoDuplicates)
		{
			var headNames = AiderHouseholdEntity.GetHeadForNames (order, heads, children)
				.Select (p => p.eCH_Person.PersonOfficialName)
				.Distinct ()
				.ToList ();

			//	If we are asked to, we remove the pseudo duplicates here. Pseudo duplicates are
			//	names that contains another name. Like when we have a family where the wife has
			//	kept its maiden name and appended the name of her husband to it.
			//	For instance the husband is called "Albert Dupond" and the wife is called "Ginette
			//	Dupond-Dupuis" or "Ginette Dupuis-Dupond".

			if (removePseudoDuplicates)
			{
				//	Order the names by size, so that we know that a name can only be included in
				//	names that are after it in the list.

				var copy = headNames
					.OrderBy (n => n.Length)
					.Select (x => new { Original = x, LowerCase = TextConverter.ConvertToLowerAndStripAccents (x).Replace ('-', ' ') })
					.ToList ();

				for (int i = 0; i < copy.Count; i++)
				{
					var shorter = copy[i].LowerCase;
					var length  = shorter.Length;

					for (int j = i + 1; j < copy.Count; j++)
					{
						var longer = copy[j].LowerCase;

						if (longer == shorter)
						{
							//	Exact duplicate based on lower-case accent-stripped name ("André" = "Andre",
							//	"von Siebenthal" = "Von Siebenthal").
						}
						else
						{
							int pos = longer.IndexOf (shorter);

							if (pos < 0)					//	nothing in common
							{
								continue;
							}

							int end = pos + shorter.Length;

							if (((end < longer.Length) && (longer[end] != ' ')) ||
								((pos > 0) && longer[pos-1] != ' '))
							{
								//	The longer word does not start or end with the short name, nor is it
								//	part of the longer name...

								continue;
							}
						}

						headNames.Remove (copy[j].Original);
						copy.RemoveAt (j);
					}
				}
			}

			return headNames;
		}


		private static List<string> GetHeadFirstnames(HouseholdMrMrs order, IEnumerable<AiderPersonEntity> heads, IEnumerable<AiderPersonEntity> children)
		{
			return AiderHouseholdEntity.GetHeadForNames (order, heads, children)
				.Select (p => p.GetCallName ())
				.ToList ();
		}


		private static IEnumerable<AiderPersonEntity> GetHeadForNames(HouseholdMrMrs order, IEnumerable<AiderPersonEntity> heads, IEnumerable<AiderPersonEntity> children)
		{
			var man = heads
				.Where (x => x.eCH_Person.PersonSex == PersonSex.Male)
				.FirstOrDefault ();

			var woman = heads
				.Where (x => x.eCH_Person.PersonSex == PersonSex.Female)
				.FirstOrDefault ();

			var result = new List<AiderPersonEntity> ();

			switch (order)
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
				var child = children.FirstOrDefault ();

				if (child != null)
				{
					result.Add (child);
				}
			}

			return result;
		}


		private static string GetHeadTitle(HouseholdMrMrs honorific, IList<AiderPersonEntity> heads, IList<AiderPersonEntity> children, bool abbreviated)
		{
			// If we have a single member, we return its title instead of the title of the
			// household.
			if (heads.Count + children.Count == 1)
			{
				var member = heads.Count == 1
					? heads[0]
					: children[0];

				return member.MrMrs.GetText (abbreviated);
			}

			switch (honorific)
			{
				case HouseholdMrMrs.MonsieurEtMadame:
				case HouseholdMrMrs.MadameEtMonsieur:
					// If we have a single head and some children, we use the "Family" title.
					if (heads.Count == 1)
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


		//	These properties are only meant as an in memory cache of the members of the household.
		//	They will never be saved to the database:
		private IList<AiderContactEntity>		contactsCache;
		private IList<AiderPersonEntity>		membersCache;
	}
}
