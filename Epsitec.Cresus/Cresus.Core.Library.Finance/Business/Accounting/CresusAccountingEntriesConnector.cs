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
		public static int GenerateFiles(CresusChartOfAccounts chart, List<string> écritures, out FormattedText message, int maxLines = int.MaxValue)
		{
			//	Génère les fichiers ecc/ecf en fonction des réglages de l'entreprise.
			// TODO: La liste des écritures devra être remplacée par une ligne d'entités de type "écriture" !
			//	Retourne le nombre d'écriture générées, ou -1 en cas d'erreur.
			var écrituresTest = new List<CresusComptaEcritureMultiple> ();

			foreach (var écriture in écritures)
			{
				écrituresTest.Add (new CresusComptaEcritureMultiple (écriture));
			}

			// TODO: Le numéro 12345789 devra être remplacé par un numéro unique lié au mandat !
			var journal = new CresusComptaJournal (chart.Path.FullPath, 123456789);
			int nbEcritures;
			var errors = journal.GenerateFiles (écrituresTest, chart.BeginDate.ToDateTime (), chart.EndDate.ToDateTime (), out nbEcritures);

			var builder = new System.Text.StringBuilder ();

			string b = chart.BeginDate.ToDateTime ().ToString ("dd.MM.yyyy");
			string e = chart.  EndDate.ToDateTime ().ToString ("dd.MM.yyyy");
			builder.Append (string.Format ("{0} - {1} / {2} :<br/>", b, e, chart.Title));

			var messages = journal.Messages;
			for (int i = 0; i < messages.Count; i++)
			{
				if (i < maxLines)
				{
					builder.Append (string.Concat ("● ", messages[i], "<br/>"));
				}
				else
				{
					builder.Append (string.Format ("● ... <i>({0} autres erreurs)</i><br/>", (messages.Count-maxLines).ToString ()));
					break;
				}
			}

			builder.Append ("<br/>");

			message = builder.ToString ();

			return (errors.Count == 0) ? nbEcritures : -1;
		}
	}
}