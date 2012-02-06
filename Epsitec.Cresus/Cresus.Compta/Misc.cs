//	Copyright � 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

		public static void ValidateDate(ComptaP�riodeEntity p�riode, EditionData data, bool emptyAccepted)
		{
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


		public static string Cat�goriesToString(Cat�gorieDeCompte cat�gorie)
		{
			var list = new List<string> ();

			foreach (var c in Misc.Cat�gories)
			{
				if ((cat�gorie & c) != 0)
				{
					list.Add (Misc.Cat�gorieToString (c));
				}
			}

			return string.Join ("|", list);
		}

		public static Cat�gorieDeCompte StringToCat�gories(string text)
		{
			var cat�gorie = Cat�gorieDeCompte.Inconnu;

			if (!string.IsNullOrEmpty (text))
			{
				var words = text.Split ('|');

				foreach (var word in words)
				{
					cat�gorie |= Misc.StringToCat�gorie (word);
				}
			}

			return cat�gorie;
		}


		public static string Cat�gorieToString(Cat�gorieDeCompte cat�gorie)
		{
			switch (cat�gorie)
			{
				case Cat�gorieDeCompte.Actif:
					return "Actif";

				case Cat�gorieDeCompte.Passif:
					return "Passif";

				case Cat�gorieDeCompte.Charge:
					return "Charge";

				case Cat�gorieDeCompte.Produit:
					return "Produit";

				case Cat�gorieDeCompte.Exploitation:
					return "Exploitation";

				default:
					return "?";
			}
		}

		public static Cat�gorieDeCompte StringToCat�gorie(string text)
		{
			switch (text.ToLower ())
			{
				case "actif":
				case "actifs":
					return Cat�gorieDeCompte.Actif;

				case "passif":
				case "passifs":
					return Cat�gorieDeCompte.Passif;

				case "charge":
				case "charges":
					return Cat�gorieDeCompte.Charge;

				case "produit":
				case "produits":
					return Cat�gorieDeCompte.Produit;

				case "exploitation":
				case "exploitations":
					return Cat�gorieDeCompte.Exploitation;

				default:
					return Cat�gorieDeCompte.Inconnu;
			}
		}


		private static IEnumerable<Cat�gorieDeCompte> Cat�gories
		{
			get
			{
				yield return Cat�gorieDeCompte.Actif;
				yield return Cat�gorieDeCompte.Passif;
				yield return Cat�gorieDeCompte.Charge;
				yield return Cat�gorieDeCompte.Produit;
				yield return Cat�gorieDeCompte.Exploitation;
			}
		}
	}
}
