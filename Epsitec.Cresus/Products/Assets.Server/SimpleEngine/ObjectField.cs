//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public enum ObjectField
	{
		Unknown,

		OneShotNumber,
		OneShotDateOperation,
		OneShotComment,
		OneShotDocuments,

		GroupParent,
		Name,
		Number,
		Description,
		MainValue,
		Entry,

		//	Définitions d'une catégorie.
		CategoryName,
		AmortizationRate,
		AmortizationType,
		Periodicity,
		Prorata,
		ResidualValue,
		Round,

		Account1,
		Account2,
		Account3,
		Account4,
		Account5,
		Account6,
		Account7,
		Account8,

		//	Définitions d'un compte.
		AccountCategory,
		AccountType,

		//	Définitins d'une écriture.
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

		UserFieldType,
		UserFieldColumnWidth,
		UserFieldLineWidth,
		UserFieldLineCount,
		UserFieldSummaryOrder,
		UserFieldTopMargin,
		UserFieldField,
		UserFieldGuid,

		GroupGuidRatioFirst = 10000,
		GroupGuidRatioLast  = 10099,

		UserFieldFirst = 20000,
		UserFieldLast  = 29999,
	}
}