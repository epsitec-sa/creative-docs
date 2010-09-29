//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Cresus.Core.Entities
{
	public static class Common
	{
		/// <summary>
		/// Retourne l'adresse de l'entreprise définie dans les réglages BusinessSettings.
		/// </summary>
		/// <returns></returns>
		public static FormattedText GetFirmAdress()
		{
#if false
			BusinessSettingsEntity settings = CoreProgram.Application.BusinessSettings;

			if (settings == null)
			{
				return null;
			}

			var mailContact = settings.LegalPerson.Contacts.Where (x => x is MailContactEntity).First () as MailContactEntity;

			if (mailContact == null)
			{
				return null;
			}

			return Entities.Common.GetMailContactSummary (mailContact);
#endif
			return null;  // TODO:  supprimer
		}

		public static FormattedText GetMailContactSummary(MailContactEntity x)
		{
			return TextFormatter.FormatText (x.LegalPerson.Name, "\n",
											 x.LegalPerson.Complement, "\n",
											 x.Complement, "\n",
											 x.Address.Street.StreetName, "\n",
											 x.Address.Street.Complement, "\n",
											 x.Address.PostBox.Number, "\n",
											 TextFormatter.FormatText (x.Address.Location.Country.Code, "~-", x.Address.Location.PostalCode),
											 x.Address.Location.Name);
		}

		public static FormattedText GetCompactMailContactSummary(MailContactEntity x)
		{
			return TextFormatter.FormatText (x.Address.Street.StreetName, "~,", x.Address.Location.PostalCode, x.Address.Location.Name);
		}

	}
}
