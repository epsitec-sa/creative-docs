﻿//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public partial class AiderOfficeReportEntity : IContentTextProducer
	{
		public AiderOfficeReportEntity()
		{
		}
		
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);		
		}

		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Name, new FormattedText (" (<a href='" + this.ProcessorUrl +"' target='_blank'>PDF</a>)"));
		}

		public void SetContent(IContent data)
		{
			if (data == null)
			{
				this.DataFormat = null;
				this.DataBlob   = null;
			}
			else
			{
				this.DataFormat = data.Format;
				this.DataBlob   = data.GetContentBlob ();
			}
		}

		public FormattedText GetFormattedText()
		{
			IContentTextProducer producer = this;

			return producer.GetFormattedText (this.DataTemplate);
		}

		public string GetProcessorUrlForSender(BusinessContext context, string processorName, AiderOfficeSenderEntity sender)
		{
			var data = context.DataContext;

			var senderId = AiderOfficeReportEntity.GetUrlEntityId (data, sender);
			var reportId = AiderOfficeReportEntity.GetUrlEntityId (data, this);
			
			return string.Format ("/proxy/reporting/{0}/{1}/{2}", processorName, senderId, reportId);
		}

		#region IContentTextProducer Members

		FormattedText IContentTextProducer.GetFormattedText(string template)
		{
			return ReportBuilder.GenerateReport (template, this.DataFormat, this.DataBlob);
		}

		#endregion

		private static string GetUrlEntityId(DataContext data, AbstractEntity entity)
		{
			return data.GetPersistedId (entity).Substring (3).Replace (':', '-');
		}
	}
}