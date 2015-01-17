//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.PlugIns;
using Epsitec.Common.Types;

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Reporting
{
	public static class ReportBuilder
	{
		public static FormattedText GetTownAndDate(AiderAddressEntity address, Date date)
		{
			return TextFormatter.FormatText (address.Town.Name, ", le", date.ToString ("dd MMM yyyy", System.Globalization.CultureInfo.CurrentCulture));
		}

		public static string ReadTemplate(string templateName)
		{
			if (string.IsNullOrEmpty (templateName))
			{
				return null;
			}

			var path = CoreContext.GetFileDepotPath ("assets", templateName + ".txt");

			if (System.IO.File.Exists (path))
			{
				return System.IO.File.ReadAllText (path, System.Text.Encoding.UTF8);
			}

			return null;
		}

		public static FormattedText GenerateReport(string templateName, IContent content)
		{
			if (content == null)
			{
				return FormattedText.Null;
			}

			var template = ReportBuilder.ReadTemplate (templateName);

			if (template == null)
			{
				return content.GetFormattedText (null);
			}
			else
			{
				return content.GetFormattedText (template.Replace ("\r\n", ""));
			}
		}

		public static FormattedText GenerateReport(string templateName, string format, byte[] blob)
		{
			IContent content = ContentFormatterFactory.Create (format);

			if (content == null)
			{
				return FormattedText.Null;
			}

			content = content.Setup (blob) as IContent;

			if (content == null)
			{
				return FormattedText.Null;
			}

			return ReportBuilder.GenerateReport (templateName, content);
		}
	}
}
