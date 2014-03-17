//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Data.Platform;

using System;

using System.Linq;
using System.Collections.Generic;
using System.Text;



namespace Epsitec.Aider.Entities
{
	public partial class AiderOfficeReportEntity
	{
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);		
		}

		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText ("<a href='" + this.ProcessorUrl +"'>" + this.Name + "</a>");	
		}

		public string BuildProcessorUrlForSender(BusinessContext context, string processorName, AiderOfficeSenderEntity sender)
		{
			var senderEntityId		= context.DataContext
												.GetPersistedId (sender)
												.Substring (3)
												.Replace (':', '-');

			var entityId			= context.DataContext
												.GetPersistedId (this)
												.Substring (3)
												.Replace (':', '-');
			return new StringBuilder ()
							.Append ("/proxy/reporting/")
							.Append (processorName)
							.Append ("/")
							.Append (senderEntityId)
							.Append ("/")
							.Append (entityId)
							.ToString ();
		}
	}
}
