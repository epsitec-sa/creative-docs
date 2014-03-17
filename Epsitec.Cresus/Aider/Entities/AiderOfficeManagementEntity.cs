//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

			var unProcessedReports	= this.Documents.Where(l => l.ProcessDate == null).Count();
			var processedReports	= this.Documents.Where (l => l.ProcessDate != null).Count ();

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
					return TextFormatter.FormatText ("Un expéditeur");
				default:
					return TextFormatter.FormatText (this.OfficeSenders.Count, " expéditeurs");
			}
		}

		public FormattedText GetLettersTitleSummary()
		{
			return TextFormatter.FormatText ("Lettres");
		}

		public FormattedText GetLettersSummary()
		{
			switch (this.Documents.Count)
			{
				case 0:
					return TextFormatter.FormatText ("Aucune");
				case 1:
					return TextFormatter.FormatText ("un document");
				default:
					return TextFormatter.FormatText (this.OfficeSenders.Count, " documents");
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


		partial void GetOfficeSenders(ref IList<AiderOfficeSenderEntity> value)
		{
			value = this.GetOfficeSenders ().AsReadOnlyCollection ();
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


		internal void AddDocumentInternal(AiderOfficeReportEntity document)
		{
			this.GetDocuments ().Add (document);
		}

		internal void RemoveDocumentInternal(AiderOfficeReportEntity document)
		{
			this.GetDocuments ().Remove (document);
		}

		partial void GetDocuments(ref IList<AiderOfficeReportEntity> value)
		{
			value = this.GetDocuments ().AsReadOnlyCollection ();
		}

		private IList<AiderOfficeReportEntity> GetDocuments()
		{
			if (this.documents == null)
			{
				this.documents = this.ExecuteWithDataContext (d => this.FindDocuments (d), () => new List<AiderOfficeReportEntity> ());
			}

			return this.documents;
		}

		private IList<AiderOfficeReportEntity> FindDocuments(DataContext dataContext)
		{
			var example = new AiderOfficeReportEntity
			{
				Office = this
			};

			return dataContext.GetByExample (example)
							  .OrderBy (x => x.Name)
							  .ToList ();
		}
		
		
		private IList<AiderOfficeSenderEntity>			senders;
		private IList<AiderOfficeReportEntity>			documents;
	}
}
