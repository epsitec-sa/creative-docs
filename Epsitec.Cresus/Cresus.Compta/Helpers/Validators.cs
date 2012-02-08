//	Copyright � 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Helpers
{
	public static class Validators
	{
		public static void ValidateText(EditionData data, FormattedText error)
		{
			//	Valide un texte libre.
			data.ClearError ();

			if (!data.HasText)
			{
				data.Error = error;
			}
		}

		public static void ValidateMontant(EditionData data, bool emptyAccepted)
		{
			//	Valide un montant.
			data.ClearError ();

			if (data.HasText)
			{
				decimal? montant = Converters.ParseMontant (data.Text);
				if (montant.HasValue)
				{
					data.Text = Converters.MontantToString (montant);
				}
				else
				{
					data.Error = "Le montant n'est pas correct";
				}
			}
			else
			{
				if (!emptyAccepted)
				{
					data.Error = "Il manque le montant";
				}
			}
		}

		public static void ValidateDate(EditionData data, bool emptyAccepted)
		{
			//	Valide une date situ�e dans n'importe quelle p�riode.
			data.ClearError ();

			if (data.HasText)
			{
				Date date;
				if (!P�riodesDataAccessor.ParseDate (data.Text, out date))
				{
					data.Error = "La date est incorrecte";
				}
			}
			else
			{
				if (!emptyAccepted)
				{
					data.Error = "Il manque la date";
				}
			}
		}

		public static void ValidateDate(ComptaP�riodeEntity p�riode, EditionData data, bool emptyAccepted)
		{
			//	Valide une date situ�e dans une p�riode donn�e.
			data.ClearError ();

			if (data.HasText)
			{
				Date? date;
				if (p�riode.ParseDate (data.Text, out date) && date.HasValue)
				{
					data.Text = date.ToString ();
				}
				else
				{
					var b = p�riode.DateD�but.ToString ();
					var e = p�riode.DateFin.ToString ();

					data.Error = string.Format ("La date est incorrecte<br/>Elle devrait �tre comprise entre {0} et {1}", b, e);
				}
			}
			else
			{
				if (!emptyAccepted)
				{
					data.Error = "Il manque la date";
				}
			}
		}
	}
}
