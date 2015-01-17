//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Linq;
using System.Collections.Generic;

namespace Epsitec.Aider.Entities
{
	public partial class AiderOfficeSenderEntity
	{
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Name.Replace ("(", "\n("));
		}

		public static AiderOfficeSenderEntity Find(BusinessContext businessContext, AiderContactEntity contact)
		{
			var example = new AiderOfficeSenderEntity ()
			{
				OfficialContact = contact
			};

			return businessContext.DataContext.GetByExample (example).First ();
		}

		public static AiderOfficeSenderEntity Create(BusinessContext businessContext,
			/**/									   AiderOfficeManagementEntity office, AiderContactEntity officialContact)
		{
			var settings = businessContext.CreateAndRegisterEntity<AiderOfficeSenderEntity> ();

			settings.Name   = "";
			settings.Office = office;
			
			settings.ParishGroupPathCache = office.ParishGroupPathCache;

			if (officialContact.IsNotNull ())
			{
				if (string.IsNullOrEmpty (settings.Name))
				{
					settings.Name = officialContact.DisplayName;
				}
				else
				{
					settings.Name = settings.Name + " (" + officialContact.DisplayName + ")";
				}
				settings.OfficialContact = officialContact;
			}

			if (string.IsNullOrEmpty (settings.Name))
			{
				settings.Name = string.Format ("Expéditeur {0:00}", office.OfficeSenders.Count);
			}

			office.AddSenderInternal (settings);

			return settings;
		}

		public static void Delete(BusinessContext context, AiderOfficeSenderEntity settings)
		{
			var office = settings.Office;

			office.RemoveSenderInternal (settings);

			context.DeleteEntity (settings);
		}
	}
}
