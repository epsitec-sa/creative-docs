//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.Export
{
	public struct AccountsImportInstructions
	{
		public AccountsImportInstructions(AccountsMergeMode mode, string filename)
		{
			this.Mode     = mode;
			this.Filename = filename;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Mode == AccountsMergeMode.Unknown
					&& string.IsNullOrEmpty (this.Filename);
			}
		}

		public static AccountsImportInstructions Empty   = new AccountsImportInstructions (AccountsMergeMode.Unknown, null);
		public static AccountsImportInstructions Default = new AccountsImportInstructions (AccountsMergeMode.Merge, null);

		public readonly AccountsMergeMode		Mode;
		public readonly string					Filename;
	}
}