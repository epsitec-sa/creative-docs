//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;

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

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.OfficeName);
		}

		public FormattedText GetSettingsTitleSummary()
		{
			return TextFormatter.FormatText ("Expéditeurs");
		}

		public FormattedText GetSettingsSummary()
		{
			switch (this.OfficeSenders.Count)
			{
				case 0:
					return TextFormatter.FormatText ("Aucun");
				case 1:
					return TextFormatter.FormatText ("Expéditeur actif");
				default:
					return TextFormatter.FormatText (this.OfficeSenders.Count, "expéditeurs actifs");
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

		internal void AddSettingsInternal(AiderOfficeSenderEntity settings)
		{
			this.GetOfficeSenders ().Add (settings);
		}

		internal void RemoveSettingsInternal(AiderOfficeSenderEntity settings)
		{
			this.GetOfficeSenders ().Remove (settings);
		}

		partial void GetOfficeSenders(ref IList<AiderOfficeSenderEntity> value)
		{
			value = this.GetOfficeSenders ().AsReadOnlyCollection ();
		}

		private IList<AiderOfficeSenderEntity> GetOfficeSenders()
		{
			if (this.settings == null)
			{
				this.settings = this.ExecuteWithDataContext (d => this.FindOfficeSenders (d), () => new List<AiderOfficeSenderEntity> ());
			}

			return this.settings;
		}

		private IList<AiderOfficeSenderEntity> FindOfficeSenders(DataContext dataContext)
		{
			var example = new AiderOfficeSenderEntity
			{
				Office = this
			};

			return dataContext.GetByExample (example)
							  .OrderBy (x => x.Name)
							  .ToList ();
		}
		
		private IList<AiderOfficeSenderEntity> settings;
	}
}
