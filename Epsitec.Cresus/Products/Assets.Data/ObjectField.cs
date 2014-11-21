//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public enum ObjectField
	{
		Unknown,

		OneShotNumber,
		OneShotUser,
		OneShotDateEvent,
		OneShotDateOperation,
		OneShotComment,
		OneShotDocuments,

		GroupParent,
		Name,
		Number,
		Description,
		MainValue,

		//	Définitions d'une catégorie.
		CategoryName,
		AmortizationMethod,
		AmortizationYearCount,
		AmortizationRate,
		AmortizationType,
		Periodicity,
		Prorata,
		Round,
		ResidualValue,
		Expression,

		//	Définition d'un groupe.
		GroupSuggestedDuringCreation,

		AccountPurchaseDebit,
		AccountPurchaseCredit,
		AccountSaleDebit,
		AccountSaleCredit,
		AccountAmortizationAutoDebit,
		AccountAmortizationAutoCredit,
		AccountAmortizationExtraDebit,
		AccountAmortizationExtraCredit,
		AccountIncreaseDebit,
		AccountIncreaseCredit,
		AccountDecreaseDebit,
		AccountDecreaseCredit,
		AccountAdjustDebit,
		AccountAdjustCredit,

		AssetEntryForcedDate,
		AssetEntryForcedDebit,
		AssetEntryForcedCredit,
		AssetEntryForcedStamp,
		AssetEntryForcedTitle,
		AssetEntryForcedAmount,

		HistoryDate,
		HistoryGlyph,
		HistoryValue,

		//	Définitions d'un compte.
		AccountCategory,
		AccountType,

		//	Définitins d'une écriture.
		EntryAssetGuid,
		EntryEventGuid,
		EntryDate,
		EntryDebitAccount,
		EntryCreditAccount,
		EntryStamp,
		EntryTitle,
		EntryAmount,

		EventDate,
		EventGlyph,
		EventType,

		LastViewsPin,
		LastViewsType,
		LastViewsMode,
		LastViewsEvent,
		LastViewsPage,
		LastViewsDate,
		LastViewsDescription,

		MergeImport,
		MergeCurrent,

		UserFieldOrder,
		UserFieldType,
		UserFieldRequired,
		UserFieldColumnWidth,
		UserFieldLineWidth,
		UserFieldLineCount,
		UserFieldSummaryOrder,
		UserFieldTopMargin,
		UserFieldField,

		WarningViewGlyph,
		WarningObject,
		WarningDate,
		WarningEventGlyph,
		WarningField,
		WarningDescription,

		GroupGuidRatioFirst = 10000,
		GroupGuidRatioLast  = 10099,

		UserFieldFirst = 20000,
		UserFieldLast  = 29999,

		MCH2Report = 30000,
	}
}