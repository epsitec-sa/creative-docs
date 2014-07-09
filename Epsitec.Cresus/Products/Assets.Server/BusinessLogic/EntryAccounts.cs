//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public struct EntryAccounts
	{
		public EntryAccounts(string account1, string account2, string account3, string account4, string account5, string account6, string account7, string account8)
		{
			this.Account1 = account1;
			this.Account2 = account2;
			this.Account3 = account3;
			this.Account4 = account4;
			this.Account5 = account5;
			this.Account6 = account6;
			this.Account7 = account7;
			this.Account8 = account8;
		}


		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty (this.Account1)
					&& string.IsNullOrEmpty (this.Account2)
					&& string.IsNullOrEmpty (this.Account3)
					&& string.IsNullOrEmpty (this.Account4)
					&& string.IsNullOrEmpty (this.Account5)
					&& string.IsNullOrEmpty (this.Account6)
					&& string.IsNullOrEmpty (this.Account7)
					&& string.IsNullOrEmpty (this.Account8);
			}
		}


		public static EntryAccounts Empty = new EntryAccounts (null, null, null, null, null, null, null, null);

		public readonly string					Account1;
		public readonly string					Account2;
		public readonly string					Account3;
		public readonly string					Account4;
		public readonly string					Account5;
		public readonly string					Account6;
		public readonly string					Account7;
		public readonly string					Account8;
	}
}
