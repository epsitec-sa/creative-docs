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
		Maintenance,
		Color,
		SerialNumber,

		Person1,
		Person2,
		Person3,
		Person4,
		Person5,

		//	Définitions d'une catégorie.
		CategoryName,
		AmortizationRate,
		AmortizationType,
		Periodicity,
		Prorata,
		ResidualValue,
		Round,

		//	Personne.
		Title,
		FirstName,
		Company,
		Address,
		Zip,
		City,
		Country,
		Phone1,
		Phone2,
		Phone3,
		Mail,

		LastViewsPin,
		LastViewsType,
		LastViewsPage,
		LastViewsDate,
		LastViewsDescription,

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

		MainValue = 10000,
		Value1,
		Value2,
		Value3,
		Value4,
		Value5,
		Value6,
		Value7,
		Value8,
		Value9,
		Value10,

		GroupGuidRatioFirst = 10100,
		GroupGuidRatioLast  = 10119,

		UserFieldFirst = 20000,
		UserFieldLast  = 29999,
	}
}