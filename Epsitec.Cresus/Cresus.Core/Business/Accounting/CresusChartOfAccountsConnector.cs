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

			IEnumerable<BookAccountDefinition> bookAccountDefs = null;	// TODO: charger les comptes
			string title = "?"; // TODO: reprendre l'élément TITLE=... du fichier CRP
			var fullPath = new Epsitec.Common.IO.MachineFilePath (path);

			cresus.Title = FormattedText.FromSimpleText (title);
			cresus.Path  = fullPath;
			cresus.Id    = System.Guid.NewGuid ();
			cresus.Items.AddRange (bookAccountDefs);
			//	TODO: reprendre les dates de début/fin de la période comptable

			return cresus;
		}
	}
}