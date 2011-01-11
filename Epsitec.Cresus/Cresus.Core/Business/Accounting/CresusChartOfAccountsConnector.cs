//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			var cresus = new CresusChartOfAccounts ();

			//	Lit le fichier .crp
			var doc = new Epsitec.CresusToolkit.CresusComptaDocument (path);

			cresus.Title     = FormattedText.FromSimpleText (doc.Title);
			cresus.BeginDate = new Date (doc.BeginDate);
			cresus.EndDate   = new Date (doc.EndDate);
			cresus.Path      = new Epsitec.Common.IO.MachineFilePath (path);
			cresus.Id        = System.Guid.NewGuid ();

			foreach (var account in doc.GetAccounts ())
			{
				var def = new BookAccountDefinition (account);
				cresus.Items.Add (def);
			}

			return cresus;
		}
	}
}