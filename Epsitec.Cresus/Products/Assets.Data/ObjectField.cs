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
		MethodGuid,
		Periodicity,

		//	Définition d'un groupe.
		GroupSuggestedDuringCreation,

		//	Définition d'une méthode.
		Expression,

		//	Définition d'un argument.
		ArgumentField,
		ArgumentType,
		ArgumentNullable,
		ArgumentVariable,
		ArgumentDefault,

		AccountPurchaseDebit,
		AccountPurchaseCredit,
		AccountPurchaseVatCode,
		AccountSaleDebit,
		AccountSaleCredit,
		AccountSaleVatCode,
		AccountAmortizationAutoDebit,
		AccountAmortizationAutoCredit,
		AccountAmortizationAutoVatCode,
		AccountAmortizationExtraDebit,
		AccountAmortizationExtraCredit,
		AccountAmortizationExtraVatCode,
		AccountIncreaseDebit,
		AccountIncreaseCredit,
		AccountIncreaseVatCode,
		AccountDecreaseDebit,
		AccountDecreaseCredit,
		AccountDecreaseVatCode,
		AccountAdjustDebit,
		AccountAdjustCredit,
		AccountAdjustVatCode,

		AssetEntryForcedDate,
		AssetEntryForcedDebit,
		AssetEntryForcedCredit,
		AssetEntryForcedStamp,
		AssetEntryForcedTitle,
		AssetEntryForcedAmount,
		AssetEntryForcedVatCode,

		HistoryDate,
		HistoryGlyph,
		HistoryValue,

		//	Définitions d'un compte.
		AccountCategory,
		AccountType,

		//	Définition d'un code TVA.
		VatRate,

		//	Définitins d'une écriture.
		EntryAssetGuid,
		EntryEventGuid,
		EntryDate,
		EntryDebitAccount,
		EntryCreditAccount,
		EntryStamp,
		EntryTitle,
		EntryAmount,
		EntryVatCode,
		EntryLocked,

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

		ExpressionSimulationRank,
		ExpressionSimulationDate,
		ExpressionSimulationType,
		ExpressionSimulationInitial,
		ExpressionSimulationAmortization,
		ExpressionSimulationFinal,
		ExpressionSimulationDebug,
		ExpressionSimulationTrace = ExpressionSimulationDebug+10,
		ExpressionSimulationError,

		GroupGuidRatioFirst = 10000,
		GroupGuidRatioLast  = 10099,

		ArgumentFirst = 10100,
		ArgumentLast  = 10199,

		UserFieldFirst = 20000,
		UserFieldLast  = 29999,

		MCH2Report = 30000,
	}
}