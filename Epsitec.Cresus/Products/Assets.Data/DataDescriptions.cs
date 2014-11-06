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
				case ObjectField.Account1:
					return Res.Strings.Enum.ObjectField.Account1.ToString ();

				case ObjectField.Account2:
					return Res.Strings.Enum.ObjectField.Account2.ToString ();

				case ObjectField.Account3:
					return Res.Strings.Enum.ObjectField.Account3.ToString ();

				case ObjectField.Account4:
					return Res.Strings.Enum.ObjectField.Account4.ToString ();

				case ObjectField.Account5:
					return Res.Strings.Enum.ObjectField.Account5.ToString ();

				case ObjectField.Account6:
					return Res.Strings.Enum.ObjectField.Account6.ToString ();

				case ObjectField.Account7:
					return Res.Strings.Enum.ObjectField.Account7.ToString ();

				case ObjectField.Account8:
					return Res.Strings.Enum.ObjectField.Account8.ToString ();

				case ObjectField.AccountCategory:
					return Res.Strings.Enum.ObjectField.AccountCategory.ToString ();

				case ObjectField.AccountType:
					return Res.Strings.Enum.ObjectField.AccountType.ToString ();

				case ObjectField.AmortizationRate:
					return Res.Strings.Enum.ObjectField.AmortizationRate.ToString ();

				case ObjectField.AmortizationType:
					return Res.Strings.Enum.ObjectField.AmortizationType.ToString ();

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

				case ObjectField.Prorata:
					return Res.Strings.Enum.ObjectField.Prorata.ToString ();

				case ObjectField.ResidualValue:
					return Res.Strings.Enum.ObjectField.ResidualValue.ToString ();

				case ObjectField.Round:
					return Res.Strings.Enum.ObjectField.Round.ToString ();

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

				case EventType.MainValue:
					return Res.Strings.Enum.EventType.MainValue.ToString ();

				case EventType.AmortizationAuto:
					return Res.Strings.Enum.EventType.AmortizationAuto.ToString ();

				case EventType.AmortizationPreview:
					return Res.Strings.Enum.EventType.AmortizationPreview.ToString ();

				case EventType.AmortizationExtra:
					return Res.Strings.Enum.EventType.AmortizationExtra.ToString ();

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
