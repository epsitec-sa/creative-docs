//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Aider.Entities
{
	public struct ParticipationData
	{
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

		public AiderPersonEntity		Person;
		public AiderLegalPersonEntity	LegalPerson;
		public AiderContactEntity		Contact;
	}
}
