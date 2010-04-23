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
		public PersonViewController(string name, AbstractEntity entity, ViewControllerMode mode)
			: base (name, entity, mode)
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
			System.Diagnostics.Debug.Assert (this.entity != null);

			Entities.AbstractPersonEntity person = this.entity as Entities.AbstractPersonEntity;
			System.Diagnostics.Debug.Assert (person != null);

			if (person is Entities.NaturalPersonEntity)
			{
				//	Une première tuile pour l'identité de la personne.
				Entities.NaturalPersonEntity naturalPerson = person as Entities.NaturalPersonEntity;
				this.CreateTile (container, "Data.Person", "Personne", this.GetDescription(naturalPerson));

				//	Une tuile par adresse postale.
				foreach (Entities.AbstractContactEntity contact in naturalPerson.Contacts)
				{
					if (contact is Entities.MailContactEntity)
					{
						Entities.MailContactEntity mailContact = contact as Entities.MailContactEntity;
						this.CreateTile (container, "Data.Mail", "Adresse", this.GetDescription (mailContact));
					}
				}

				//	Une tuile pour tous les numéros de téléphone.
				string telecomContent = this.GetTelecomDescription (naturalPerson.Contacts);
				if (!string.IsNullOrEmpty (telecomContent))
				{
					this.CreateTile (container, "Data.Telecom", "Téléphones", telecomContent);
				}

				//	Une tuile pour toutes les adresses mail.
				string uriContent = this.GetUriDescription (naturalPerson.Contacts);
				if (!string.IsNullOrEmpty (uriContent))
				{
					this.CreateTile (container, "Data.Uri", "Mails", uriContent);
				}
			}

			if (person is Entities.LegalPersonEntity)
			{
				Entities.LegalPersonEntity legalPerson = person as Entities.LegalPersonEntity;

				//	TODO:
			}
		}


		private void CreateTile(Widget container, string iconUri, string title, string content)
		{
			Widgets.SimpleTile tile = new Widgets.SimpleTile
			{
				Parent = container,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 1),
				ArrowLocation = Direction.Right,
				IconUri = iconUri,
				Title = title,
				Content = content,
			};
		}


		private string GetDescription(Entities.NaturalPersonEntity naturalPerson)
		{
			return Misc.SpacingAppend (naturalPerson.Firstname, naturalPerson.Lastname);
		}

		private string GetDescription(Entities.MailContactEntity mailContact)
		{
			StringBuilder builder = new StringBuilder ();

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

			return builder.ToString ();
		}

		private string GetTelecomDescription(IList<Entities.AbstractContactEntity> contacts)
		{
			StringBuilder builder = new StringBuilder ();

			foreach (Entities.AbstractContactEntity contact in contacts)
			{
				if (contact is Entities.TelecomContactEntity)
				{
					Entities.TelecomContactEntity telecomContact = contact as Entities.TelecomContactEntity;

					builder.Append (telecomContact.Number);
					builder.Append ("<br/>");
				}
			}

			return builder.ToString ();
		}

		private string GetUriDescription(IList<Entities.AbstractContactEntity> contacts)
		{
			StringBuilder builder = new StringBuilder ();

			foreach (Entities.AbstractContactEntity contact in contacts)
			{
				if (contact is Entities.UriContactEntity)
				{
					Entities.UriContactEntity uriContact = contact as Entities.UriContactEntity;

					builder.Append (uriContact.Uri);
					builder.Append ("<br/>");
				}
			}
	
			return builder.ToString ();
		}


		private List<CoreController> controllers;
	}
}
