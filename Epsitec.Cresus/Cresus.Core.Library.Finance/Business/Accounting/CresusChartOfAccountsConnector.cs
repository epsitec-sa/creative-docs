//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Accounting
{
	public static class CresusChartOfAccountsConnector
	{
		public static CresusChartOfAccounts Load(string path)
		{
			var dir = System.IO.Path.GetDirectoryName (path);
			var name = System.IO.Path.GetFileNameWithoutExtension (path) + ".crp";

			path = System.IO.Path.Combine (dir, name);

			//	Try to read the CRP file, which stores the chart of accounts of the CRE
			//	file :

			var doc = new Epsitec.CresusToolkit.CresusComptaDocument (path);

			if (doc.CheckMetadata ())
			{
				var cresus = new CresusChartOfAccounts ()
				{
					Title     = FormattedText.FromSimpleText (doc.Title),
					BeginDate = new Date (doc.BeginDate),
					EndDate   = new Date (doc.EndDate),
					Path      = new Epsitec.Common.IO.MachineFilePath (path),
					Id        = System.Guid.NewGuid (),
				};

				cresus.Items.AddRange (doc.GetAccounts ().Select (x => new BookAccountDefinition (x)));

				return cresus;
			}

			return null;
		}
	}
}