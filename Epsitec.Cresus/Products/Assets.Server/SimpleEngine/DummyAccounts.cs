//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public static class DummyAccounts
	{
		public static void AddAccounts(DataMandat mandat, string name)
		{
			//	On lit un plan comptable placé dans "S:\Epsitec.Cresus\App.CresusAssets\External\Data".
			//	C'est n'importe quoi, mais ça marche.
			//	TODO: Hack à supprimer dès que possible !

			var exeRootPath = Globals.Directories.ExecutableRoot;
			var filename = System.IO.Path.Combine (exeRootPath, "External", "Data", name+".crp");

			using (var importEngine = new AccountsImport ())
			{
				try
				{
					var accounts = new GuidDictionary<DataObject> (mandat.UndoManager);
					var range = importEngine.Import (accounts, null, filename);

					mandat.AddAccounts (range, accounts);
					//?mandat.CurrentAccountsDateRange = range;
				}
				catch
				{
				}
			}
		}
	}

}