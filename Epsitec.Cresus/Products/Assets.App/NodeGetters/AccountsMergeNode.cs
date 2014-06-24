//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.NodeGetters
{
	/// <summary>
	///	Noeud correspondant à une ligne de la liste des comptes à fusionner.
	/// </summary>
	public struct AccountsMergeNode
	{
		public AccountsMergeNode(DataObject importedAccount, DataObject currentAccount)
		{
			this.ImportedAccount = importedAccount;
			this.CurrentAccount  = currentAccount;
		}

		public bool IsEmpty
		{
			get
			{
				return this.ImportedAccount == null
					&& this.CurrentAccount  == null;
			}
		}

		public static AccountsMergeNode Empty = new AccountsMergeNode (null, null);

		public readonly DataObject				ImportedAccount;
		public readonly DataObject				CurrentAccount;
	}
}
