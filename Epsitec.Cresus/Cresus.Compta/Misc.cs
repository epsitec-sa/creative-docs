//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta
{
	public static class Misc
	{
		public static DateFieldController CreateDateField(AbstractController controller, Widget parent, FormattedText initialDate, FormattedText tooltip, System.Action<EditionData> validateAction, System.Action changedAction)
		{
			var fieldController = new DateFieldController (controller, 0, new ColumnMapper (tooltip), null, changedAction);
			fieldController.CreateUI (parent);
			fieldController.Box.PreferredWidth = 80;
			fieldController.EditionData = new EditionData (controller, validateAction);
			fieldController.EditionData.Text = initialDate;
			fieldController.Validate ();

			return fieldController;
		}

		public static void ValidateDate(ComptaPériodeEntity période, EditionData data, bool emptyAccepted)
		{
			data.ClearError ();

			if (data.HasText)
			{
				Date? date;
				if (période.ParseDate (data.Text, out date) && date.HasValue)
				{
					data.Text = date.ToString ();
				}
				else
				{
					var b = période.DateDébut.ToString ();
					var e = période.DateFin.ToString ();

					data.Error = string.Format ("La date est incorrecte<br/>Elle devrait être comprise entre {0} et {1}", b, e);
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

		public static Date? ParseDate(string text)
		{
			System.DateTime d;
			if (System.DateTime.TryParse (text, out d))
			{
				return new Date (d);
			}

			return null;
		}

		public static string DateToString(Date? date)
		{
			if (date.HasValue)
			{
				return date.Value.ToString ();
			}
			else
			{
				return null;
			}
		}


		public static string CatégoriesToString(CatégorieDeCompte catégorie)
		{
			var list = new List<string> ();

			foreach (var c in Misc.Catégories)
			{
				if ((catégorie & c) != 0)
				{
					list.Add (Misc.CatégorieToString (c));
				}
			}

			return string.Join ("|", list);
		}

		public static CatégorieDeCompte StringToCatégories(string text)
		{
			var catégorie = CatégorieDeCompte.Inconnu;

			if (!string.IsNullOrEmpty (text))
			{
				var words = text.Split ('|');

				foreach (var word in words)
				{
					catégorie |= Misc.StringToCatégorie (word);
				}
			}

			return catégorie;
		}


		public static string CatégorieToString(CatégorieDeCompte catégorie)
		{
			switch (catégorie)
			{
				case CatégorieDeCompte.Actif:
					return "Actif";

				case CatégorieDeCompte.Passif:
					return "Passif";

				case CatégorieDeCompte.Charge:
					return "Charge";

				case CatégorieDeCompte.Produit:
					return "Produit";

				case CatégorieDeCompte.Exploitation:
					return "Exploitation";

				default:
					return "?";
			}
		}

		public static CatégorieDeCompte StringToCatégorie(string text)
		{
			switch (text.ToLower ())
			{
				case "actif":
				case "actifs":
					return CatégorieDeCompte.Actif;

				case "passif":
				case "passifs":
					return CatégorieDeCompte.Passif;

				case "charge":
				case "charges":
					return CatégorieDeCompte.Charge;

				case "produit":
				case "produits":
					return CatégorieDeCompte.Produit;

				case "exploitation":
				case "exploitations":
					return CatégorieDeCompte.Exploitation;

				default:
					return CatégorieDeCompte.Inconnu;
			}
		}


		private static IEnumerable<CatégorieDeCompte> Catégories
		{
			get
			{
				yield return CatégorieDeCompte.Actif;
				yield return CatégorieDeCompte.Passif;
				yield return CatégorieDeCompte.Charge;
				yield return CatégorieDeCompte.Produit;
				yield return CatégorieDeCompte.Exploitation;
			}
		}
	}
}
