//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	/// <summary>
	/// Cette structure décrit comment doit être importé un compte (ajouté ou fusionné avec un autre).
	/// </summary>
	public struct AccountMergeTodo
	{
		public static AccountMergeTodo NewAdd(DataObject addAccount)
		{
			return new AccountMergeTodo (addAccount, null);
		}

		public static AccountMergeTodo NewMerge(DataObject importedAccount, DataObject mergeWithAccount)
		{
			return new AccountMergeTodo (importedAccount, mergeWithAccount);
		}

		private AccountMergeTodo(DataObject importedAccount, DataObject mergeWith)
		{
			this.ImportedAccount = importedAccount;
			this.MergeWithAccount       = mergeWith;
		}

		public bool IsAdd
		{
			get
			{
				return this.MergeWithAccount == null;
			}
		}

		public bool IsMerge
		{
			get
			{
				return this.MergeWithAccount != null;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.ImportedAccount  == null
					&& this.MergeWithAccount == null;
			}
		}

		public static AccountMergeTodo Empty = new AccountMergeTodo (null, null);

		public readonly DataObject				ImportedAccount;
		public readonly DataObject				MergeWithAccount;
	}
}
