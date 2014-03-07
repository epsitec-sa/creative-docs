//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Data.Platform;

using System;

using System.Linq;
using System.Collections.Generic;

namespace Epsitec.Aider.Entities
{
	public partial class AiderOfficeManagementEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.OfficeName);
		}

		public FormattedText GetSettingsTitleSummary()
		{
			return TextFormatter.FormatText ("Réglage du secrétariat");
		}

		public FormattedText GetSettingsSummary()
		{
			if(this.Settings.Count () > 0)
			{

				var text =	"(" + this.Settings.Count +") disponibles";

				return TextFormatter.FormatText (text);
			}
			else
			{
				var text =		"Aucun réglages";

				return TextFormatter.FormatText (text);
			}
			
		}

		public static AiderOfficeManagementEntity Find(BusinessContext businessContext, AiderGroupEntity group)
		{
			var officeExample = new AiderOfficeManagementEntity
			{
				ParishGroup = group
			};

			return businessContext.DataContext.GetByExample (officeExample).First ();
		}

		public static AiderOfficeManagementEntity Create(BusinessContext businessContext,string name,AiderGroupEntity parishGroup)
		{
			var office = businessContext.CreateAndRegisterEntity<AiderOfficeManagementEntity> ();

			office.OfficeName = name;
			office.ParishGroup = parishGroup;
			office.ParishGroupPathCache = parishGroup.Path;

			return office;
		}

		public static AiderOfficeSettingsEntity CreateSettings(BusinessContext businessContext, AiderOfficeManagementEntity office, AiderContactEntity officialContact, AiderContactEntity officialAddress, AiderTownEntity ppFrankingTown)
		{
			var settings = businessContext.CreateAndRegisterEntity<AiderOfficeSettingsEntity> ();

			settings.Name = "Réglage " + (office.Settings.Count () + 1);
			settings.Office = office;
			 
			if(officialContact.IsNotNull ())
			{
				settings.OfficialContact = officialContact;
			}

			if (officialAddress.IsNotNull ())
			{
				settings.OfficeAddress = officialAddress;
			}

			if (ppFrankingTown.IsNotNull ())
			{
				settings.PPFrankingTown = ppFrankingTown;
			}

			office.Settings.Add (settings);
			return settings;
		}

	}
}
