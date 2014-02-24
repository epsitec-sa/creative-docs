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
		Number,
		Name,
		Description,
		MainValue,

		//	Définitions d'une catégorie.
		CategoryName,
		AmortizationRate,
		AmortizationType,
		Periodicity,
		Prorata,
		ResidualValue,
		Round,

		Compte1,
		Compte2,
		Compte3,
		Compte4,
		Compte5,
		Compte6,
		Compte7,
		Compte8,

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

		GroupGuidRatioFirst = 10100,
		GroupGuidRatioLast  = 10119,

		UserFieldFirst = 20000,
		UserFieldLast  = 29999,
	}
}