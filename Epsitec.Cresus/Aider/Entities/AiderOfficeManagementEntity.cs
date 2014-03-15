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
		public AiderOfficeManagementEntity()
		{

		}

		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.OfficeMainContact.GetCompactSummary ());
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.OfficeName);
		}


		public FormattedText GetDocumentTitleSummary()
		{
			return TextFormatter.FormatText ("Documents");
		}

		public FormattedText GetDocumentsSummary()
		{
			var summary = TextFormatter.FormatText ("Résumé :\n");

			var unProcessedReports	= this.Letters.Where(l => l.ProcessDate == null).Count();
			var processedReports	= this.Letters.Where (l => l.ProcessDate != null).Count ();

			if (unProcessedReports > 0)
			{
				summary.AppendLine (TextFormatter.FormatText (unProcessedReports, " documents en attentes"));
			}

			if (processedReports > 0)
			{
				summary.AppendLine (TextFormatter.FormatText (unProcessedReports, " documents archivés"));
			}

			return summary;
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
					return TextFormatter.FormatText (this.OfficeSenders.Count, " expéditeurs actifs");
			}
		}

		public FormattedText GetLettersTitleSummary()
		{
			return TextFormatter.FormatText ("Lettres");
		}

		public FormattedText GetLettersSummary()
		{
			switch (this.Letters.Count)
			{
				case 0:
					return TextFormatter.FormatText ("Aucune");
				case 1:
					return TextFormatter.FormatText ("une lettre");
				default:
					return TextFormatter.FormatText (this.OfficeSenders.Count, " lettres");
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

		internal void AddSenderInternal(AiderOfficeSenderEntity settings)
		{
			if (!this.GetOfficeSenders ().Any (s => s == settings))
			{
				this.GetOfficeSenders ().Add (settings);
			}
		}

		internal void RemoveSenderInternal(AiderOfficeSenderEntity settings)
		{
			this.GetOfficeSenders ().Remove (settings);
		}

		partial void GetOfficeSenders(ref IList<AiderOfficeSenderEntity> value)
		{
			value = this.GetOfficeSenders ().AsReadOnlyCollection ();
		}

		private IList<AiderOfficeSenderEntity> GetOfficeSenders()
		{
			if (this.senders == null)
			{
				this.senders = this.ExecuteWithDataContext (d => this.FindOfficeSenders (d), () => new List<AiderOfficeSenderEntity> ());
			}

			return this.senders;
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


		internal void AddLetterInternal(AiderOfficeLetterReportEntity letter)
		{
			this.GetLetters ().Add (letter);
		}

		internal void RemoveLetterInternal(AiderOfficeLetterReportEntity letter)
		{
			this.GetLetters ().Remove (letter);
		}

		partial void GetLetters(ref IList<AiderOfficeLetterReportEntity> value)
		{
			value = this.GetLetters ().AsReadOnlyCollection ();
		}

		private IList<AiderOfficeLetterReportEntity> GetLetters()
		{
			if (this.letters == null)
			{
				this.letters = this.ExecuteWithDataContext (d => this.FindLetters (d), () => new List<AiderOfficeLetterReportEntity> ());
			}

			return this.letters;
		}

		private IList<AiderOfficeLetterReportEntity> FindLetters(DataContext dataContext)
		{
			var example = new AiderOfficeLetterReportEntity
			{
				Office = this
			};

			return dataContext.GetByExample (example)
							  .OrderBy (x => x.Name)
							  .ToList ();
		}
		
		private IList<AiderOfficeSenderEntity>			senders;
		private IList<AiderOfficeLetterReportEntity>	letters;
	}
}
