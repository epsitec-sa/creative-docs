//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

using Epsitec.CresusToolkit;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Accounting
{
	public static class CresusAccountingEntriesConnector
	{
		public static bool GenerateFiles(CresusChartOfAccounts chart, List<string> écritures, out FormattedText message, int maxLines, out int nbEcritures, out int nbErrors)
		{
			//	Génère les fichiers ecc/ecf en fonction des réglages de l'entreprise.
			//	TODO: La liste des écritures devra être remplacée par une ligne d'entités de type "écriture" !
			//	Retourne false en cas d'erreur.
			var écrituresTest = new List<CresusComptaEcritureMultiple> ();

			foreach (var écriture in écritures)
			{
				écrituresTest.Add (new CresusComptaEcritureMultiple (écriture));
			}

			// TODO: Le numéro 12345789 devra être remplacé par un numéro unique lié au mandat !
			var journal = new CresusComptaJournal (chart.Path.FullPath, 123456789);
			bool ok = journal.GenerateFiles (écrituresTest, chart.BeginDate.ToDateTime (), chart.EndDate.ToDateTime (), out nbEcritures);

			var builder = new System.Text.StringBuilder ();

			if (!ok)  // erreur ?
			{
				builder.Append ("<font color=\"#c90000\">");  // rouge foncé
			}

			if (chart.BeginDate.Month == 1 &&
				chart.EndDate.Month == 12 &&
				chart.BeginDate.Year == chart.EndDate.Year)  // de janvier à décembre de la même année ?
			{
				builder.Append (string.Format ("<b>{0} / {1} :</b><br/>", chart.BeginDate.Year.ToString (), chart.Title));
			}
			else  // période différente d'une année entière ?
			{
				string b = chart.BeginDate.ToDateTime ().ToString ("dd.MM.yyyy");
				string e = chart.EndDate.ToDateTime ().ToString ("dd.MM.yyyy");
				builder.Append (string.Format ("<b>{0} — {1} / {2} :</b><br/>", b, e, chart.Title));
			}

			var messages = journal.Messages;
			for (int i = 0; i < messages.Count; i++)
			{
				if (i < maxLines || messages.Count-maxLines == 1)
				{
					builder.Append (string.Concat ("● ", messages[i], "<br/>"));
				}
				else
				{
					builder.Append (string.Format ("● ... <i>({0} autres erreurs)</i><br/>", (messages.Count-maxLines).ToString ()));
					break;
				}
			}

			if (!ok)  // erreur ?
			{
				builder.Append ("</font>");
			}

			message = builder.ToString ();
			nbErrors = ok ? 0 : journal.Errors.Count;
			return ok;
		}
	}
}