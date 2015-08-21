//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Entities;
using Epsitec.Cresus.WebCore.Server.Core.IO;
using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Common.Support.EntityEngine;

namespace Epsitec.Aider.Processors.Reports
{
	public abstract class AbstractProcessor<T> : AbstractProcessor
		where T : Epsitec.Aider.Entities.AiderOfficeReportEntity
	{
		protected AbstractProcessor(CoreServer coreServer)
			: base (coreServer)
		{

		}
		public override string CreateReport(System.IO.Stream stream, BusinessContext businessContext, dynamic parameters)
		{
			string settingsId	= parameters.settings;
			string reportId		= parameters.report;

			var settings = EntityIO.ResolveEntity (businessContext, settingsId) as AiderOfficeSenderEntity;
			var letter  = EntityIO.ResolveEntity (businessContext, reportId) as T;

			return this.GenerateDocument (stream, businessContext, settings, letter);
		}

		public override string CreateReports(System.IO.Stream stream, BusinessContext businessContext, IEnumerable<AbstractEntity> entities, dynamic parameters)
		{
			string settingsId	= parameters.settings;
			var settings = EntityIO.ResolveEntity (businessContext, settingsId) as AiderOfficeSenderEntity;
			if (entities.First ().GetType () == typeof (AiderEventParticipantEntity))
			{
				var participants  = entities.Cast<AiderEventParticipantEntity> ();
				var reports  = participants.Select (p => p.Event.Report).Distinct ().Cast<T> ();
				return this.GenerateDocuments (stream, businessContext, settings, reports);
			} 
			else if (entities.First ().GetType () == typeof (AiderEventEntity))
			{
				var events  = entities.Cast<AiderEventEntity> ();
				var reports  = events.Select (e => e.Report).Distinct ().Cast<T> ();
				return this.GenerateDocuments (stream, businessContext, settings, reports);
			}
			else
			{
				var reports  = entities.Cast<T> ();
				return this.GenerateDocuments (stream, businessContext, settings, reports);
			}

		}

		protected abstract string GenerateDocument(System.IO.Stream stream, BusinessContext businessContext, AiderOfficeSenderEntity settings, T letter);
		protected abstract string GenerateDocuments(System.IO.Stream stream, BusinessContext businessContext, AiderOfficeSenderEntity settings, IEnumerable<T> reports);
	}
}
