//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.Accounting;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Library.Business.EccEcfExport
{
	public static class EccEcfExport
	{
		public static void Export(BusinessContext businessContext)
		{
			var businessSettings = businessContext.GetCached<BusinessSettingsEntity> ();
			var businessFinance = businessSettings.Finance;

			//	De l'année la plus récente à la plus ancienne.
			var charts = businessFinance.GetAllChartsOfAccounts ().OrderBy (x => x.BeginDate).Reverse ();

			if (!charts.Any ())  // aucun plan comptable défini ?
			{
				FormattedText message = "La comptabilisation est impossible, car aucun plan comptable n'a été défini dans les réglages de l'entreprise.";
				var dialog = MessageDialog.CreateOk ("Comptabilisation", DialogIcon.Question, message);
				dialog.OpenDialog ();
			}
			else
			{
				//	Confirmation initiale.
				FormattedText message = "";
				message += "Cette opération va préparer les écritures en vue de leur comptabilisation<br/>";
				message += "avec le logiciel Crésus Comptabilité, sous la forme de fichiers ecc/ecf.<br/>";
				message += "<br/>";
				message += "Voulez-vous exporter les écritures ?";

				var dialog = MessageDialog.CreateYesNo ("Comptabilisation", DialogIcon.Question, message);
				dialog.OpenDialog ();
				if (dialog.Result != DialogResult.Yes)
				{
					return;
				}

				//	On simule quelques écritures.
				//	TODO: à remplacer par le vrai code dès que cela sera possible !
				var écritures = new List<string> ();
				écritures.Add ("12.01.2010/10/1000;2000;Virement 1;123.45");
				écritures.Add ("04.02.2010/11/1100;3000;Virement 2;0.05");
				écritures.Add ("01.09.2010/12/1020;2100;Virement 3;10000.00");

				écritures.Add ("31.03.2011/21/1000;2000;Virement 10;100.00");
				écritures.Add ("01.04.2011/20/1000;...;Virement 11.1;11.00/1030;...;Virement 11.2;22.00/...;2000;Virement 11.3;33.00");
				écritures.Add ("26.07.2011/22/1100;2100;Virement 12;20.75");
				écritures.Add ("27.07.2011/23/1010;2010;Virement 12;2.50");

				écritures.Add ("03.01.2012/30/1000;2000;Virement 20;100.00");
				écritures.Add ("14.01.2012/31/1000;2000;Virement 21;100.00");

				message = "Les écritures générées sont bidons et toujours identiques, juste pour tester les mécanismes (3 en 2010, 4 en 2011 et 2 en 2012).<br/><br/>Voulez-vous simuler des écritures erronées, pour tester la gestion des erreurs ?";
				dialog = MessageDialog.CreateYesNo ("Comptabilisation", DialogIcon.Question, message);
				dialog.OpenDialog ();
				if (dialog.Result == DialogResult.Yes)
				{
					//	Génère volontairement des écritures fausses en 2010.
#if false
					écritures.Add ("02.09.2010/13/...;...;Erreur;1.00");
					écritures.Add ("03.09.2010/14");
					écritures.Add ("04.09.2010/15");
#endif

					//	Génère volontairement une écriture multiple non balancée.
					écritures.Add ("10.11.2011/25/1000;...;Virement 13.1;11.00/1030;...;Virement 13.2;22.00/...;2000;Virement 13.3;33.01");

					//	Génère volontairement 10 écritures fausses (sans libellé).
					for (int i = 0; i < 10; i++)
					{
						écritures.Add (string.Format ("13.11.2011/{0}/1000;2000;;1.00", (i+100).ToString ()));
					}
				}

				//	Effectue l'exportation pour chaque plan comptable.
				var builder = new System.Text.StringBuilder ();
				bool jobOk = true;
				bool first = true;
				int total = 0;
				int maxLines = charts.Count () > 3 ? 3 : 6;
				int linesCount = 0;

				foreach (CresusChartOfAccounts chart in charts)
				{
					if (!first)
					{
						builder.Append ("<br/>");
					}

					if (linesCount > 12)
					{
						maxLines = 1;
					}

					FormattedText result;
					int nbEcritures, nbErrors;
					if (CresusAccountingEntriesConnector.GenerateFiles (chart, écritures, out result, maxLines, out nbEcritures, out nbErrors))
					{
						total += nbEcritures;
						linesCount += 2;
					}
					else  // erreur ?
					{
						jobOk = false;
						linesCount += 1 + System.Math.Min (nbErrors, maxLines);
					}

					builder.Append (result);

					first = false;
				}

				//	Affiche le résumé final.
				FormattedText summary;
				if (jobOk)
				{
					summary = TextFormatter.FormatText ("Les fichiers sont prêts pour la combtabilisation.").ApplyBold ().ApplyFontSize (14.0);

					if (total < écritures.Count)
					{
						int n = écritures.Count - total;
						summary += string.Format ("<br/><br/><i><b>Remarque:</b> {0} écritures n'ont pas été exportées, car elles ne correspondaient pas<br/>aux périodes des plans comptables définis dans les réglages de l'entreprise.</i>", n.ToString ());
					}
				}
				else
				{
					summary = TextFormatter.FormatText ("La comptabilisation n'est pas possible, car des erreurs sont survenues.").ApplyBold ().ApplyFontSize (14.0);
				}

				message = FormattedText.Concat (summary, "<br/><br/>", builder.ToString (), " ");  // un bug dans TextLayout oblige de terminer par un espace !

				dialog = MessageDialog.CreateOk ("Comptabilisation", jobOk ? DialogIcon.Question : DialogIcon.Warning, message);
				dialog.OpenDialog ();
			}
		}
	}
}
