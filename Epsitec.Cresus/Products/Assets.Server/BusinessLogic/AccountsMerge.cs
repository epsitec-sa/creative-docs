//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public class AccountsMerge : System.IDisposable
	{
		public void Dispose()
		{
		}


		public void Merge(GuidList<DataObject> current, GuidList<DataObject> import, AccountsMergeMode mode)
		{
			this.currentData = current;
			this.importData  = import;
			this.mode        = mode;

			if (this.mode == AccountsMergeMode.XferAll)
			{
				this.XferAll ();
			}
			else
			{
				this.Merge ();
			}
		}

		private void XferAll()
		{
			this.currentData.Clear ();

			foreach (var account in this.importData)
			{
				this.currentData.Add (account);
			}
		}

		private void Merge()
		{
		}


		private GuidList<DataObject>			currentData;
		private GuidList<DataObject>			importData;
		private AccountsMergeMode				mode;
	}
}
