//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public struct EntryAccounts
	{
		public EntryAccounts(
			string accountPurchaseDebit, string accountPurchaseCredit,
			string accountSaleDebit, string accountSaleCredit,
			string accountAmortizationAutoDebit, string accountAmortizationAutoCredit,
			string accountAmortizationExtraDebit, string accountAmortizationExtraCredit,
			string accountIncreaseDebit, string accountIncreaseCredit,
			string accountDecreaseDebit, string accountDecreaseCredit)
		{
			this.AccountPurchaseDebit           = accountPurchaseDebit;
			this.AccountPurchaseCredit          = accountPurchaseCredit;
			this.AccountSaleDebit               = accountSaleDebit;
			this.AccountSaleCredit              = accountSaleCredit;
			this.AccountAmortizationAutoDebit   = accountAmortizationAutoDebit;
			this.AccountAmortizationAutoCredit  = accountAmortizationAutoCredit;
			this.AccountAmortizationExtraDebit  = accountAmortizationExtraDebit;
			this.AccountAmortizationExtraCredit = accountAmortizationExtraCredit;
			this.AccountIncreaseDebit           = accountIncreaseDebit;
			this.AccountIncreaseCredit          = accountIncreaseCredit;
			this.AccountDecreaseDebit           = accountDecreaseDebit;
			this.AccountDecreaseCredit          = accountDecreaseCredit;
		}


		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty (this.AccountPurchaseDebit)
					&& string.IsNullOrEmpty (this.AccountPurchaseCredit)
					&& string.IsNullOrEmpty (this.AccountSaleDebit)
					&& string.IsNullOrEmpty (this.AccountSaleCredit)
					&& string.IsNullOrEmpty (this.AccountAmortizationAutoDebit)
					&& string.IsNullOrEmpty (this.AccountAmortizationAutoCredit)
					&& string.IsNullOrEmpty (this.AccountAmortizationExtraDebit)
					&& string.IsNullOrEmpty (this.AccountAmortizationExtraCredit)
					&& string.IsNullOrEmpty (this.AccountIncreaseDebit)
					&& string.IsNullOrEmpty (this.AccountIncreaseCredit)
					&& string.IsNullOrEmpty (this.AccountDecreaseDebit)
					&& string.IsNullOrEmpty (this.AccountDecreaseCredit);
			}
		}


		public static EntryAccounts Empty = new EntryAccounts (null, null, null, null, null, null, null, null, null, null, null, null);

		public readonly string					AccountPurchaseDebit;
		public readonly string					AccountPurchaseCredit;
		public readonly string					AccountSaleDebit;
		public readonly string					AccountSaleCredit;
		public readonly string					AccountAmortizationAutoDebit;
		public readonly string					AccountAmortizationAutoCredit;
		public readonly string					AccountAmortizationExtraDebit;
		public readonly string					AccountAmortizationExtraCredit;
		public readonly string					AccountIncreaseDebit;
		public readonly string					AccountIncreaseCredit;
		public readonly string					AccountDecreaseDebit;
		public readonly string					AccountDecreaseCredit;
	}
}
