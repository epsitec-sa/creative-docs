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

		OneShotNuméro,
		OneShotDateOpération,
		OneShotCommentaire,
		OneShotDocuments,

		Parent,
		Numéro,
		Nom,
		Description,
		Maintenance,
		Couleur,
		NuméroSérie,

		NomCatégorie,
		TauxAmortissement,
		TypeAmortissement,
		Périodicité,
		ValeurRésiduelle,

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

		ValeurComptable = 10000,
		Valeur1,
		Valeur2,
		Valeur3,
		Valeur4,
		Valeur5,
		Valeur6,
		Valeur7,
		Valeur8,
		Valeur9,
		Valeur10,

		GroupGuid  = 10100,
		GroupRatio = 10200,

		MaxField = 10300,
	}
}