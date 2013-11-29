//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;

using System.IO;
using System.Linq;


namespace Epsitec.Aider.Data.Subscription
{
	internal sealed class ContactFileWriter
	{
		public ContactFileWriter(CoreData coreData, FileInfo outputRchFile, FileInfo outputCustomFile)
		{
			this.coreData         = coreData;
			this.outputRchFile    = outputRchFile;
			this.outputCustomFile = outputCustomFile;
			this.startTime        = System.DateTime.UtcNow;
			this.errors           = new List<string> ();
		}


		public void Write()
		{
			var lines = this.GetLines ();

			ContactFileLine.Write (lines.Where (x => x.Source == ContactFileLineSource.Rch), this.outputRchFile);
			ContactFileLine.Write (lines.Where (x => x.Source == ContactFileLineSource.Custom), this.outputCustomFile);

			foreach (var error in this.errors)
			{
				System.Console.WriteLine ("{0}", error);
			}

			System.Console.ReadLine ();
		}

		private IEnumerable<ContactFileLine> GetLines()
		{
			var lines = new List<ContactFileLine> ();

			AiderEnumerator.Execute (this.coreData, (businessContext, contacts) => lines.AddRange (this.GetLines (businessContext.DataContext, contacts)));

			return lines;
		}

		private IEnumerable<ContactFileLine> GetLines(DataContext context, IEnumerable<AiderContactEntity> contacts)
		{
			return contacts.Select (x => this.GetLine (context, x)).Where (x => x != null);
		}


		private ContactFileLine GetLine(DataContext context, AiderContactEntity contact)
		{
			if ((contact.Address.IsNull ()) ||
				(contact.Address.Town.IsNull ()) ||
				(string.IsNullOrEmpty (contact.Address.Town.SwissCantonCode)))
			{
				this.errors.Add ("Contact has no address: " + context.GetNormalizedEntityKey (contact).ToString ());
				
				return null;
			}

			switch (contact.ContactType)
			{
				case ContactType.PersonAddress:
					return this.GetNaturalPersonLine ("A", context, contact);

				case ContactType.PersonHousehold:
					return this.GetNaturalPersonLine ("H", context, contact);

				case ContactType.Legal:
					return this.GetLegalPersonLine (context, contact);

				default:
					this.errors.Add ("ContactType not handled: " + context.GetNormalizedEntityKey (contact).ToString ());
					return null;
			}
		}

		private ContactFileLine GetNaturalPersonLine(string prefix, DataContext context, AiderContactEntity contact)
		{
			var person = contact.Person;
			
			if ((person.IsNull ()) ||
				(person.Age.HasValue && (person.Age < 16)))
			{
				return null;
			}

			var id      = prefix + context.GetNormalizedEntityKey (contact).ToString ();
			var source  = person.eCH_Person.DataSource == Enumerations.DataSource.Government ? ContactFileLineSource.Rch : ContactFileLineSource.Custom;
			var address = contact.Address;

			return new ContactFileLine (id, person.MrMrs, person.eCH_Person.PersonOfficialName, person.GetCallName (), null, address.PostBox, address.AddressLine1, address.Street, address.HouseNumberAndComplement, address.Town.SwissZipCode.ToString (), address.Town.Name, source);
		}

		private ContactFileLine GetLegalPersonLine(DataContext context, AiderContactEntity contact)
		{
			var legalPerson   = contact.LegalPerson;
			var corporateName = legalPerson.Name;
			var address       = legalPerson.Address;
			
			var id = "L/" + context.GetNormalizedEntityKey (contact).ToString ();

			return new ContactFileLine (id, null, null, null, corporateName, address.PostBox, address.AddressLine1, address.Street, address.HouseNumberAndComplement, address.Town.SwissZipCode.ToString (), address.Town.Name, ContactFileLineSource.Custom);
		}


		private readonly CoreData				coreData;
		private readonly FileInfo				outputRchFile;
		private readonly FileInfo				outputCustomFile;

		private readonly List<string>			errors;
		
		private readonly System.DateTime		startTime;
	}
}
