//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Export
{
	public struct AccountsImportReport
	{
		public AccountsImportReport(AccountsImportMode mode, string message)
		{
			this.Mode    = mode;
			this.Message = message;
		}

		public readonly AccountsImportMode		Mode;
		public readonly string					Message;
	}
}
