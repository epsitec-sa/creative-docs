//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Aider.Entities
{
	public struct ParticipationData
	{

		public ParticipationData(AiderPersonEntity person)
		{
			this.Person = person;
			this.LegalPerson = null;
			this.Contact = person.GetMainContact ();
		}

		public ParticipationData(AiderLegalPersonEntity legalPerson)
		{
			this.Person = null;
			this.LegalPerson = legalPerson;
			this.Contact = legalPerson.GetMainContact ();
		}

		public ParticipationData(AiderContactEntity contact)
		{
			this.Person = contact.Person;
			this.LegalPerson = contact.LegalPerson;
			this.Contact = contact;
		}

		public ParticipationData(AiderGroupParticipantEntity participation)
		{
			this.Person = participation.Person;
			this.LegalPerson = participation.LegalPerson;
			this.Contact = participation.Contact;
		}

		public readonly AiderPersonEntity		Person;
		public readonly AiderLegalPersonEntity	LegalPerson;
		public readonly AiderContactEntity		Contact;
	}
}
