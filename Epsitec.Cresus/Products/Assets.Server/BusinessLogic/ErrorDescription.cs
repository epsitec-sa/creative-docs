//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class ErrorDescription
	{
		public static string GetErrorObject(DataAccessor accessor, Error error)
		{
			var obj = accessor.GetObject (BaseType.Assets, error.ObjectGuid);
			if (obj == null)
			{
				return null;
			}
			else
			{
				return ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Name);
			}
		}


		public static string GetErrorDescription(Error error)
		{
			if (error.Type == ErrorType.AmortizationGenerate)
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
			else if (error.Type == ErrorType.AmortizationRemove)
			{
				if (error.Counter == 0)
				{
					return "Aucun amortissement n'a été supprimé";
				}
				else if (error.Counter == 1)
				{
					return "Un amortissement a été supprimé";
				}
				else
				{
					return string.Format ("{0} amortissements ont été supprimés", error.Counter);
				}
			}
			else
			{
				return ErrorDescription.GetErrorDescription (error.Type);
			}
		}

		private static string GetErrorDescription(ErrorType errorType)
		{
			switch (errorType)
			{
				case ErrorType.Unknown:
					return "Erreur inconnue";

				case ErrorType.AmortizationAlreadyDone:
					return "L'objet a déja été amorti durant cette période";

				case ErrorType.AmortizationUndefined:
					return "L'amortissement n'est pas défini";

				case ErrorType.AmortizationInvalidRate:
					return "Le taux d'amortissement est incorrect";

				case ErrorType.AmortizationInvalidType:
					return "Le type d'amortissement est incorrect";

				case ErrorType.AmortizationInvalidPeriod:
					return "La périodicité de l'amortissement est incorrecte";

				case ErrorType.AmortizationEmptyAmount:
					return "L'objet n'a pas de valeur comptable";

				case ErrorType.AmortizationOutObject:
					return "L'objet est sorti";

				case ErrorType.AmortizationResidualReached:
					return "L'amortissement a atteint la valeur résiduelle";

				default:
					return "?";
			}
		}
	}
}
