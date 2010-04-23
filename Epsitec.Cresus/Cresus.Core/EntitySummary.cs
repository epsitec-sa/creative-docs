//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Core
{
	/// <summary>
	/// Cette classe statique se charge de produire le texte résumé pour tous les types d'entités.
	/// </summary>
	public static class EntitySummary
	{
		public static string GetNaturalPersonSummary(Entities.NaturalPersonEntity naturalPerson)
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

		public static string GetLegalPersonSummary(Entities.LegalPersonEntity legalPerson)
		{
			var builder = new StringBuilder ();

			builder.Append (legalPerson.Name);
			builder.Append ("<br/>");

			return Misc.RemoveLastBreakLine (builder.ToString ());
		}


		public static string GetMailTitle(Entities.MailContactEntity mailContact)
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

		public static string GetMailSummary(Entities.MailContactEntity mailContact)
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


		public static string GetTelecomsSummary(IList<Entities.AbstractContactEntity> contacts)
		{
			var builder = new StringBuilder ();

			foreach (Entities.AbstractContactEntity contact in contacts)
			{
				if (contact is Entities.TelecomContactEntity)
				{
					var telecomContact = contact as Entities.TelecomContactEntity;

					builder.Append (telecomContact.Number);
					EntitySummary.AppendRoles (builder, telecomContact.Roles);
					builder.Append ("<br/>");
				}
			}

			return Misc.RemoveLastBreakLine (builder.ToString ());
		}

		public static string GetUrisSummary(IList<Entities.AbstractContactEntity> contacts)
		{
			var builder = new StringBuilder ();

			foreach (Entities.AbstractContactEntity contact in contacts)
			{
				if (contact is Entities.UriContactEntity)
				{
					var uriContact = contact as Entities.UriContactEntity;

					builder.Append (uriContact.Uri);
					EntitySummary.AppendRoles (builder, uriContact.Roles);
					builder.Append ("<br/>");
				}
			}

			return Misc.RemoveLastBreakLine (builder.ToString ());
		}


		private static void AppendRoles(StringBuilder builder, IList<Entities.ContactRoleEntity> roles)
		{
			if (roles != null && roles.Count != 0)
			{
				builder.Append (" (");

				bool first = true;
				foreach (Entities.ContactRoleEntity role in roles)
				{
					if (!first)
					{
						builder.Append (", ");
					}

					builder.Append (role.Name);
					first = false;
				}

				builder.Append (")");

			}
		}
	}
}
