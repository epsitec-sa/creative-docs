//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public static class DataDescriptions
	{
		public static string GetObjectFieldDescription(ObjectField field)
		{
			if (field >= ObjectField.GroupGuidRatioFirst &&
				field <= ObjectField.GroupGuidRatioLast)
			{
				return Res.Strings.ObjectField.InGroupGuidRation.ToString ();
			}

			switch (field)
			{
				case ObjectField.AccountPurchaseDebit:
					return Res.Strings.Enum.ObjectField.AccountPurchaseDebit.ToString ();

				case ObjectField.AccountPurchaseCredit:
					return Res.Strings.Enum.ObjectField.AccountPurchaseCredit.ToString ();

				case ObjectField.AccountSaleDebit:
					return Res.Strings.Enum.ObjectField.AccountSaleDebit.ToString ();

				case ObjectField.AccountSaleCredit:
					return Res.Strings.Enum.ObjectField.AccountSaleCredit.ToString ();

				case ObjectField.AccountAmortizationAutoDebit:
					return Res.Strings.Enum.ObjectField.AccountAmortizationAutoDebit.ToString ();

				case ObjectField.AccountAmortizationAutoCredit:
					return Res.Strings.Enum.ObjectField.AccountAmortizationAutoCredit.ToString ();

				case ObjectField.AccountAmortizationExtraDebit:
					return Res.Strings.Enum.ObjectField.AccountAmortizationExtraDebit.ToString ();

				case ObjectField.AccountAmortizationExtraCredit:
					return Res.Strings.Enum.ObjectField.AccountAmortizationExtraCredit.ToString ();

				case ObjectField.AccountIncreaseDebit:
					return Res.Strings.Enum.ObjectField.AccountIncreaseDebit.ToString ();

				case ObjectField.AccountIncreaseCredit:
					return Res.Strings.Enum.ObjectField.AccountIncreaseCredit.ToString ();

				case ObjectField.AccountDecreaseDebit:
					return Res.Strings.Enum.ObjectField.AccountDecreaseDebit.ToString ();

				case ObjectField.AccountDecreaseCredit:
					return Res.Strings.Enum.ObjectField.AccountDecreaseCredit.ToString ();

				case ObjectField.AccountAdjustDebit:
					return Res.Strings.Enum.ObjectField.AccountAdjustDebit.ToString ();

				case ObjectField.AccountAdjustCredit:
					return Res.Strings.Enum.ObjectField.AccountAdjustCredit.ToString ();

				case ObjectField.AccountCategory:
					return Res.Strings.Enum.ObjectField.AccountCategory.ToString ();

				case ObjectField.AccountType:
					return Res.Strings.Enum.ObjectField.AccountType.ToString ();

				case ObjectField.ArgumentType:
					return Res.Strings.Enum.ObjectField.ArgumentType.ToString ();

				case ObjectField.ArgumentNullable:
					return Res.Strings.Enum.ObjectField.ArgumentNullable.ToString ();

				case ObjectField.ArgumentVariable:
					return Res.Strings.Enum.ObjectField.ArgumentVariable.ToString ();

				case ObjectField.ArgumentDefault:
					return Res.Strings.Enum.ObjectField.ArgumentDefault.ToString ();

				case ObjectField.CategoryName:
					return Res.Strings.Enum.ObjectField.CategoryName.ToString ();

				case ObjectField.Description:
					return Res.Strings.Enum.ObjectField.Description.ToString ();

				case ObjectField.EntryAmount:
					return Res.Strings.Enum.ObjectField.EntryAmount.ToString ();

				case ObjectField.EntryCreditAccount:
					return Res.Strings.Enum.ObjectField.EntryCreditAccount.ToString ();

				case ObjectField.EntryDebitAccount:
					return Res.Strings.Enum.ObjectField.EntryDebitAccount.ToString ();

				case ObjectField.EntryStamp:
					return Res.Strings.Enum.ObjectField.EntryStamp.ToString ();

				case ObjectField.EntryTitle:
					return Res.Strings.Enum.ObjectField.EntryTitle.ToString ();

				case ObjectField.GroupParent:
					return Res.Strings.Enum.ObjectField.GroupParent.ToString ();

				case ObjectField.GroupSuggestedDuringCreation:
					return Res.Strings.Enum.ObjectField.GroupSuggestedDuringCreation.ToString ();

				case ObjectField.MainValue:
					return Res.Strings.Enum.ObjectField.MainValue.ToString ();

				case ObjectField.Name:
					return Res.Strings.Enum.ObjectField.Name.ToString ();

				case ObjectField.Number:
					return Res.Strings.Enum.ObjectField.Number.ToString ();

				case ObjectField.OneShotComment:
					return Res.Strings.Enum.ObjectField.OneShotComment.ToString ();

				case ObjectField.OneShotDateEvent:
					return Res.Strings.Enum.ObjectField.OneShotDateEvent.ToString ();

				case ObjectField.OneShotDateOperation:
					return Res.Strings.Enum.ObjectField.OneShotDateOperation.ToString ();

				case ObjectField.OneShotDocuments:
					return Res.Strings.Enum.ObjectField.OneShotDocuments.ToString ();

				case ObjectField.OneShotNumber:
					return Res.Strings.Enum.ObjectField.OneShotNumber.ToString ();

				case ObjectField.OneShotUser:
					return Res.Strings.Enum.ObjectField.OneShotUser.ToString ();

				case ObjectField.Periodicity:
					return Res.Strings.Enum.ObjectField.Periodicity.ToString ();

				case ObjectField.MethodGuid:
					return Res.Strings.Enum.ObjectField.Method.ToString ();

				case ObjectField.Expression:
					return Res.Strings.Enum.ObjectField.Expression.ToString ();

				case ObjectField.UserFieldColumnWidth:
					return Res.Strings.Enum.ObjectField.UserFieldColumnWidth.ToString ();

				case ObjectField.UserFieldLineWidth:
					return Res.Strings.Enum.ObjectField.UserFieldLineWidth.ToString ();

				case ObjectField.UserFieldLineCount:
					return Res.Strings.Enum.ObjectField.UserFieldLineCount.ToString ();

				case ObjectField.UserFieldRequired:
					return Res.Strings.Enum.ObjectField.UserFieldRequired.ToString ();

				case ObjectField.UserFieldSummaryOrder:
					return Res.Strings.Enum.ObjectField.UserFieldSummaryOrder.ToString ();

				case ObjectField.UserFieldTopMargin:
					return Res.Strings.Enum.ObjectField.UserFieldTopMargin.ToString ();

				case ObjectField.UserFieldType:
					return Res.Strings.Enum.ObjectField.UserFieldType.ToString ();

				default:
					return null;
			}
		}

		public static string GetEventDescription(EventType type)
		{
			switch (type)
			{
				case EventType.Input:
					return Res.Strings.Enum.EventType.Input.ToString ();

				case EventType.Modification:
					return Res.Strings.Enum.EventType.Modification.ToString ();

				case EventType.Increase:
					return Res.Strings.Enum.EventType.Increase.ToString ();

				case EventType.Decrease:
					return Res.Strings.Enum.EventType.Decrease.ToString ();

				case EventType.Adjust:
					return Res.Strings.Enum.EventType.Adjust.ToString ();

				case EventType.AmortizationAuto:
					return Res.Strings.Enum.EventType.AmortizationAuto.ToString ();

				case EventType.AmortizationPreview:
					return Res.Strings.Enum.EventType.AmortizationPreview.ToString ();

				case EventType.AmortizationExtra:
					return Res.Strings.Enum.EventType.AmortizationExtra.ToString ();

				case EventType.AmortizationSuppl:
					return Res.Strings.Enum.EventType.AmortizationSuppl.ToString ();

				case EventType.Locked:
					return Res.Strings.Enum.EventType.Locked.ToString ();

				case EventType.Output:
					return Res.Strings.Enum.EventType.Output.ToString ();

				default:
					return null;
			}
		}
	}
}
