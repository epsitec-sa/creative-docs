//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Aider.Reporting;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System.Linq;
using System.Collections.Generic;

namespace Epsitec.Aider.Entities
{
	public partial class AiderOfficeGroupParticipantReportEntity
	{
		partial void GetParticipants(ref IList<AiderGroupParticipantEntity> value)
		{
			value = this.GetParticipants ().AsReadOnlyCollection ();
		}

		private IList<AiderGroupParticipantEntity> GetParticipants()
		{
			if (this.participants == null)
			{
				this.participants = this.ExecuteWithDataContext (d => this.GetParticipants (d), () => new List<AiderGroupParticipantEntity> ());
			}

			return this.participants;
		}

		private IList<AiderGroupParticipantEntity> GetParticipants(DataContext d)
		{
			return this.Group.FindParticipants (d, this.Group.FindParticipantCount (d));
		}

		public static AiderOfficeGroupParticipantReportEntity Create(BusinessContext context, AiderGroupEntity group, AiderOfficeSenderEntity sender, string documentName, string title, string templateName, IContent content)
		{
			var report = context.CreateAndRegisterEntity<AiderOfficeGroupParticipantReportEntity> ();

			report.Name			= documentName;
			report.DataTemplate = templateName;
			report.CreationDate	= Date.Today;
			report.Title		= title;
			report.Group		= group;
			report.Office		= sender.Office;

			report.SetContent (content);
			
			sender.Office.AddDocumentInternal (report);
			return report;
		}

		private IList<AiderGroupParticipantEntity>	participants;
	}
}
