//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class CustomerEntity
	{
		/// <summary>
		/// Gets the mail contact of the specified type.
		/// </summary>
		/// <param name="type">The type of mail contact.</param>
		/// <returns>The mail contact if a match can be found; otherwise, <c>null</c>.</returns>
		public MailContactEntity GetMailContact(ContactGroupType type)
		{
			var mailContacts = this.Relation.Person.Contacts.OfType<MailContactEntity> ();

			return mailContacts.FirstOrDefault (x => x.ContactGroups.Any (c => c.ContactGroupType == type));
		}

		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
				(
					"N°", this.IdA, "\n",
					this.Relation.GetSummary (), "\n",
					"Représentant: ~", this.SalesRepresentative.GetCompactSummary ()
				);
		}

		public override FormattedText GetCompactSummary()
		{
			return this.Relation.GetCompactSummary ();
		}

		public override string[] GetEntityKeywords()
		{
			return this.Relation.GetEntityKeywords ();
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.IdA.GetEntityStatus ());
				a.Accumulate (this.IdB.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.IdC.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.Affairs.Select (x => x.GetEntityStatus ()));
				a.Accumulate (this.Relation.GetEntityStatus ());
				a.Accumulate (this.SalesRepresentative.GetEntityStatus ().TreatAsOptional ());

				return a.EntityStatus;
			}
		}
	}
}
