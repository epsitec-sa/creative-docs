//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Controllers
{
	public class PersonViewController : EntityViewController
	{
		public PersonViewController(string name)
			: base (name)
		{
			this.controllers = new List<CoreController> ();
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			foreach (CoreController controller in this.controllers)
			{
				yield return controller;
			}
		}

		public override void CreateUI(Widget container)
		{
			System.Diagnostics.Debug.Assert (this.Entity != null);
			var person = this.Entity as Entities.AbstractPersonEntity;

			if (person is Entities.NaturalPersonEntity)
			{
				//	Une première tuile pour l'identité de la personne.
				var naturalPerson = person as Entities.NaturalPersonEntity;
				this.CreateSimpleTile (container, "Data.NaturalPerson", "Personne physique", this.GetNaturalPersonSummary(naturalPerson));

				//	Une tuile distincte par adresse postale.
				foreach (Entities.AbstractContactEntity contact in naturalPerson.Contacts)
				{
					if (contact is Entities.MailContactEntity)
					{
						var mailContact = contact as Entities.MailContactEntity;
						this.CreateSimpleTile (container, "Data.Mail", this.GetMailTitle (mailContact), this.GetMailSummary (mailContact));
					}
				}

				//	Une tuile commune pour tous les numéros de téléphone.
				string telecomContent = this.GetTelecomSummary (naturalPerson.Contacts);
				if (!string.IsNullOrEmpty (telecomContent))
				{
					this.CreateSimpleTile (container, "Data.Telecom", "Téléphones", telecomContent);
				}

				//	Une tuile commune pour toutes les adresses mail.
				string uriContent = this.GetUriSummary (naturalPerson.Contacts);
				if (!string.IsNullOrEmpty (uriContent))
				{
					this.CreateSimpleTile (container, "Data.Uri", "Mails", uriContent);
				}
			}

			if (person is Entities.LegalPersonEntity)
			{
				var legalPerson = person as Entities.LegalPersonEntity;
				this.CreateSimpleTile (container, "Data.LegalPerson", "Personne morale", this.GetLegalPersonSummary (legalPerson));
			}

			this.AdjustLastTile (container);
		}



		private string GetNaturalPersonSummary(Entities.NaturalPersonEntity naturalPerson)
		{
			var builder = new StringBuilder ();

			if (naturalPerson.Title != null)
			{
				var titleEntity = naturalPerson.Title;
				builder.Append (titleEntity.Name);
				builder.Append ("<br/>");
			}

			builder.Append (Misc.SpacingAppend (naturalPerson.Firstname, naturalPerson.Lastname));
			builder.Append ("<br/>");

			return Misc.RemoveLastBreakLine (builder.ToString ());
		}

		private string GetLegalPersonSummary(Entities.LegalPersonEntity legalPerson)
		{
			var builder = new StringBuilder ();

			builder.Append (legalPerson.Name);
			builder.Append ("<br/>");

			return Misc.RemoveLastBreakLine (builder.ToString ());
		}

		private string GetMailTitle(Entities.MailContactEntity mailContact)
		{
			var builder = new StringBuilder ();

			builder.Append ("Adresse");

			if (mailContact.Roles != null && mailContact.Roles.Count != 0)
			{
				builder.Append (" ");

				foreach (Entities.ContactRoleEntity role in mailContact.Roles)
				{
					builder.Append (role.Name);
				}
			}

			return Misc.RemoveLastBreakLine (builder.ToString ());
		}

		private string GetMailSummary(Entities.MailContactEntity mailContact)
		{
			var builder = new StringBuilder ();

			if (mailContact.Address.Street != null && !string.IsNullOrEmpty (mailContact.Address.Street.StreetName))
			{
				builder.Append (mailContact.Address.Street.StreetName);
				builder.Append ("<br/>");
			}

			if (mailContact.Address.Location != null)
			{
				builder.Append (Misc.SpacingAppend (mailContact.Address.Location.PostalCode, mailContact.Address.Location.Name));
				builder.Append ("<br/>");
			}

			return Misc.RemoveLastBreakLine (builder.ToString ());
		}

		private string GetTelecomSummary(IList<Entities.AbstractContactEntity> contacts)
		{
			var builder = new StringBuilder ();

			foreach (Entities.AbstractContactEntity contact in contacts)
			{
				if (contact is Entities.TelecomContactEntity)
				{
					var telecomContact = contact as Entities.TelecomContactEntity;

					builder.Append (telecomContact.Number);
					builder.Append ("<br/>");
				}
			}

			return Misc.RemoveLastBreakLine (builder.ToString ());
		}

		private string GetUriSummary(IList<Entities.AbstractContactEntity> contacts)
		{
			var builder = new StringBuilder ();

			foreach (Entities.AbstractContactEntity contact in contacts)
			{
				if (contact is Entities.UriContactEntity)
				{
					var uriContact = contact as Entities.UriContactEntity;

					builder.Append (uriContact.Uri);
					builder.Append ("<br/>");
				}
			}
	
			return Misc.RemoveLastBreakLine(builder.ToString ());
		}


		private List<CoreController> controllers;
	}
}
