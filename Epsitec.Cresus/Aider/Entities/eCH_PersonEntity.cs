//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.Entities;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Data.ECh;

namespace Epsitec.Aider.Entities
{
	public partial class eCH_PersonEntity
	{
		/// <summary>
		/// Gets a value indicating whether this person is deceased.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this person is deceased; otherwise, <c>false</c>.
		/// </value>
		public bool IsDeceased
		{
			get
			{
				return this.PersonDateOfDeath.HasValue;
			}
		}

		/// <summary>
		/// Gets the default first name for the person, which is the 1st first name in
		/// the list.
		/// </summary>
		/// <param name="person">The person.</param>
		/// <returns>The default first name.</returns>
		internal static string GetDefaultFirstName(eCH_PersonEntity person)
		{
			if ((person.IsNull ()) ||
				(string.IsNullOrWhiteSpace (person.PersonFirstNames)))
			{
				return "";
			}
			else
			{
				string[] names = person.PersonFirstNames.Split (' ');
				return names[0];
			}
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.GetDisplayName (), "(~", this.PersonDateOfBirth.Value.ComputeAge (), "~)");
		}

		public override FormattedText GetSummary()
		{
			var person = this.GetDataContext ().GetByExample<AiderPersonEntity> (new AiderPersonEntity ()
			{
				eCH_Person = this
			}).SingleOrDefault ();
			var personInfo = "Donnée eCh de la personne manquante";
			if (person.IsNotNull ())
			{
				personInfo = person.GetFullName ();
			}

            if (this.RemovalReason == RemovalReason.None)
            {
                return TextFormatter.FormatText (personInfo, "\n", "eCH-ID", this.PersonId, " (", this.DeclarationStatus, ")");
            }
            else
            {
                return TextFormatter.FormatText (personInfo, "\n", "eCH-ID", this.PersonId, " (", this.DeclarationStatus, ")\n", "Raison:", this.RemovalReason);
            }
        }

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.ReportedPerson1.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.ReportedPerson2.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.PersonOfficialName.GetEntityStatus ());
				a.Accumulate (this.PersonFirstNames.GetEntityStatus ());

				return a.EntityStatus;
			}
		}
		
		
		public string GetDisplayName()
		{
			var lastname = this.PersonOfficialName;
			var firstname = this.PersonFirstNames;

			var name = TextFormatter.FormatText (lastname, ",", firstname).ToSimpleText ();

			if (this.IsDeceased)
			{
				name += " †";
			}

			return name;
		}

		public eCH_AddressEntity GetAddress ()
		{
			if (this.Address1.IsNotNull ())
			{
				return this.Address1;
			}

			if (this.Address2.IsNotNull ())
			{
				return this.Address2;
			}

			if (this.ReportedPerson1.IsNotNull ())
			{
				return this.ReportedPerson1.Address;
			}

			if (this.ReportedPerson2.IsNotNull ())
			{
				return this.ReportedPerson2.Address;
			}

			return null;
		}

		public IEnumerable<eCH_ReportedPersonEntity> ReportedPersons
		{
			get
			{
				if (this.ReportedPerson1.IsNotNull ())
				{
					yield return this.ReportedPerson1;
				}

				if (this.ReportedPerson2.IsNotNull ())
				{
					yield return this.ReportedPerson2;
				}
			}
		}

		partial void GetNationalityCountryName(ref string value)
		{
			var nationality = this.Nationality;

			// Here we check explicitely for null and not IsNotNull(), because the this.Nationality
			// property won't create "null" entities, because it is a virtual property. This is
			// another flaw in the EntityNullReferenceVirtualizer design.

			value = nationality != null
				? nationality.Name
				: "";
		}

		partial void SetNationalityCountryName(string value)
		{
			throw new System.NotSupportedException ("Do not call this method.");
		}

		partial void GetNationality(ref AiderCountryEntity value)
		{
			if (string.IsNullOrWhiteSpace (this.NationalityCountryCode))
			{
				return;
			}

			var context    = BusinessContextPool.GetCurrentContext (this);
			var repository = context.GetRepository<AiderCountryEntity> ();
			
			var example = new AiderCountryEntity ()
			{
				IsoCode = this.NationalityCountryCode,
			};

			value = repository.GetByExample (example).FirstOrDefault ();
		}

		partial void SetNationality(AiderCountryEntity value)
		{
			if (value.IsNull ())
			{
				this.NationalityCountryCode = "";
			}
			else
			{
				this.NationalityCountryCode = value.IsoCode;
			}
		}
	}
}
