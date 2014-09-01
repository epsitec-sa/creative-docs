//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Assets.Data
{
	[DesignerVisible]
	public enum ObjectField
	{
		Unknown,

		OneShotNumber,
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
		AmortizationRate,
		AmortizationType,
		Periodicity,
		Prorata,
		Round,
		ResidualValue,

		Account1,
		Account2,
		Account3,
		Account4,
		Account5,
		Account6,
		Account7,
		Account8,

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
		LastViewsPage,
		LastViewsDate,
		LastViewsDescription,

		MergeImport,
		MergeCurrent,

		UserFieldType,
		UserFieldRequired,
		UserFieldColumnWidth,
		UserFieldLineWidth,
		UserFieldLineCount,
		UserFieldSummaryOrder,
		UserFieldTopMargin,
		UserFieldField,
		UserFieldGuid,

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