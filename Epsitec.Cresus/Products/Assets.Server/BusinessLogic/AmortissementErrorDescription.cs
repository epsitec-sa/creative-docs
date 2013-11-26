//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class AmortissementErrorDescription
	{
		public static string GetErrorDescription(AmortissementError error)
		{
			if (error.ErrorType == AmortissementErrorType.Generate)
			{
				if (error.Counter == 0)
				{
					return "Aucun amortissement n'a été généré";
				}
				else if (error.Counter == 1)
				{
					return "Un amortissement a été généré";
				}
				else
				{
					return string.Format ("{0} amortissements ont été générés", error.Counter);
				}
			}
			else
			{
				return AmortissementErrorDescription.GetErrorDescription (error.ErrorType);
			}
		}

		public static string GetErrorDescription(AmortissementErrorType errorType)
		{
			switch (errorType)
			{
				case AmortissementErrorType.Unknown:
					return "Erreur inconnue";

				case AmortissementErrorType.AlreadyAmorti:
					return "L'objet a déja été amorti durant cette période";

				case AmortissementErrorType.InvalidRate:
					return "Le taux d'amortissement est incorrect";

				case AmortissementErrorType.InvalidType:
					return "Le type d'amortissement est incorrect";

				case AmortissementErrorType.InvalidPeriod:
					return "La périodicité de l'amortissement est incorrecte";

				case AmortissementErrorType.EmptyAmount:
					return "L'objet n'a pas de valeur comptable";

				case AmortissementErrorType.OutObject:
					return "L'objet est sorti";

				default:
					return null;
			}
		}
	}
}
