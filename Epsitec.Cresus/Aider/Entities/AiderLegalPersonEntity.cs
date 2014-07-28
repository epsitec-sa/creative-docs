//	Copyright © 2013-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;

namespace Epsitec.Aider.Entities
{
	public partial class AiderLegalPersonEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.GetNameMultiLine (), "\n", this.Type);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public FormattedText GetAddressLabelText(PostalAddressType type = PostalAddressType.Default)
		{
			return this.GetAddressLabelText (this.GetAddressRecipientText (), type);
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Name.GetEntityStatus ());
				a.Accumulate (this.Comment);
				a.Accumulate ((this.Type == LegalPersonType.None ? EntityStatus.Empty : EntityStatus.Valid).TreatAsOptional ());

				return a.EntityStatus;
			}
		}

		public string GetNameMultiLine()
		{
			return string.Join ("\n", this.GetNameLines ());
		}

		public IEnumerable<string> GetNameLines()
		{
			var separators = new char[]
			{
				'|', ';', ':'
			};

			return this.Name.Split (separators, System.StringSplitOptions.RemoveEmptyEntries)
				.Select (x => x.Trim ());
		}

		public bool IsMemberOf(AiderGroupEntity group)
		{
			return this.GetMemberships (group).Any ();
		}

		public IEnumerable<AiderGroupParticipantEntity> GetMemberships(AiderGroupEntity group)
		{
			return this.GetParticipations ().Where (g => g.Group == group);
		}

		
		public void RefreshCache()
		{
			this.DisplayZipCode = this.GetDisplayZipCode ();
			this.DisplayAddress = this.GetDisplayAddress ();
		}


		public string GetDisplayZipCode()
		{
			return this.Address.GetDisplayZipCode ().ToSimpleText ();
		}

		public string GetDisplayAddress()
		{
			return this.Address.GetDisplayAddress ().ToSimpleText ();
		}

		public AiderContactEntity GetMainContact()
		{
			return this.Contacts.FirstOrDefault ();
		}

		private FormattedText GetAddressLabelText(string recipient, PostalAddressType type)
		{
			return TextFormatter.FormatText (recipient, "\n", this.Address.GetPostalAddress (type));
		}

		private string GetAddressRecipientText()
		{
			var contact = this.Contacts.FirstOrDefault ();
			if(contact != null)
			{
				List<string> lines = new List<string> ();

				lines.Add (contact.LegalPersonContactMrMrs.GetLongText ());
				lines.Add (contact.LegalPersonContactFullName);
				lines.AddRange (contact.LegalPerson.GetNameLines ());

				return StringUtils.Join ("\n", lines);
			}

			return "";
		}

		private IList<AiderGroupParticipantEntity> GetParticipations()
		{
			if (this.participations == null)
			{
				this.participations = this.ExecuteWithDataContext
				(
					d => this.FindParticipations (d),
					() => new List<AiderGroupParticipantEntity> ()
				);
			}

			return this.participations;
		}


		internal void AddParticipationInternal(AiderGroupParticipantEntity participation)
		{
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
			this.GetParticipations ().Remove (participation);
		}


		private IList<AiderGroupParticipantEntity> FindParticipations(DataContext dataContext)
		{
			var request = AiderGroupParticipantEntity.CreateParticipantRequest (dataContext, this, true);

			return dataContext.GetByRequest<AiderGroupParticipantEntity> (request);
		}


		public void AddContactInternal(AiderContactEntity contact)
		{
			this.GetContacts ().Add (contact);
		}

		public void RemoveContactInternal(AiderContactEntity contact)
		{
			this.GetContacts ().Remove (contact);
		}


		public static void Delete(BusinessContext businessContext, AiderLegalPersonEntity legalPerson)
		{
			foreach (var contact in legalPerson.Contacts.ToArray ())
			{
				AiderContactEntity.Delete (businessContext, contact);
			}

			if (legalPerson.Comment.IsNotNull ())
			{
				businessContext.DeleteEntity (legalPerson.Comment);
			}

			if (legalPerson.Address.IsNotNull ())
			{
				businessContext.DeleteEntity (legalPerson.Address);
			}

			foreach (var subscription in AiderSubscriptionEntity.FindSubscriptions (businessContext, legalPerson))
			{
				businessContext.DeleteEntity (subscription);
			}

			foreach (var refusal in AiderSubscriptionRefusalEntity.FindRefusals (businessContext, legalPerson))
			{
				businessContext.DeleteEntity (refusal);
			}

			businessContext.DeleteEntity (legalPerson);
		}

		
		partial void OnParishGroupChanging(AiderGroupEntity oldValue, AiderGroupEntity newValue)
		{
			this.ParishGroupPathCache = AiderGroupEntity.GetPath (newValue);
		}

		partial void GetContacts(ref IList<AiderContactEntity> value)
		{
			value = this.GetContacts ().AsReadOnlyCollection ();
		}


		partial void GetFullAddressTextSingleLine(ref string value)
		{
			this.GetFullAddressTextMultiLine (ref value);

			value = value.Replace ("\n", ", ");
		}


		partial void SetFullAddressTextSingleLine(string value)
		{
			throw new System.NotImplementedException ("Do not use this method");
		}


		partial void GetFullAddressTextMultiLine(ref string value)
		{
			var contact = this.GetMainContact ();

			if (contact != null)
			{
				value = contact.GetAddressLabelText ().ToSimpleText ();
			}
			else
			{
				value = this.Name + "\n" + this.Address.GetPostalAddress ().ToSimpleText ();
			}
		}


		partial void SetFullAddressTextMultiLine(string value)
		{
			throw new System.NotImplementedException ("Do not use this method");
		}

		
		private IList<AiderContactEntity> GetContacts()
		{
			if (this.contacts == null)
			{
				this.contacts = this.ExecuteWithDataContext
				(
					d => this.GetContacts (d),
					() => new List<AiderContactEntity> ()
				);
			}

			return this.contacts;
		}

		private IList<AiderContactEntity> GetContacts(DataContext dataContext)
		{
			var example = new AiderContactEntity ()
			{
				LegalPerson = this,
			};

			return dataContext
				.GetByExample (example)
				.Where (x => x.Person.IsAlive)
				.ToList ();
		}


		private IList<AiderGroupParticipantEntity>	participations;
		private IList<AiderContactEntity>			contacts;
	}
}
