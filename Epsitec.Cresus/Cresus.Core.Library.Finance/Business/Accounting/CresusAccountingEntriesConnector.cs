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
		public static string GenerateFiles(CresusChartOfAccounts chart)
		{
			//	La version final recevra en entrée la liste des écritures.
			//	On génère ici quelques écritures de test.
			//	TODO: à supprimer et terminer
			var écritures = new List<CresusComptaEcritureMultiple> ();

			{
				var multiple = new CresusComptaEcritureMultiple ()
				{
					Date  = new System.DateTime (2010, 9, 1),
					Pièce = "10",
				};

				var simple = new CresusComptaEcritureSimple ()
				{
					Crédit  = "1000",
					Débit   = "2000",
					Libellé = "Virement 1",
					Montant = 123.45M,
				};

				multiple.Ecritures.Add (simple);

				écritures.Add (multiple);
			}

			{
				var multiple = new CresusComptaEcritureMultiple ()
				{
					Date  = new System.DateTime (2011, 3, 31),
					Pièce = "11",
				};

				var simple = new CresusComptaEcritureSimple ()
				{
					Crédit  = "1000",
					Débit   = "2000",
					Libellé = "Virement 2",
					Montant = 1000.00M,
				};

				multiple.Ecritures.Add (simple);

				écritures.Add (multiple);
			}

			{
				var multiple = new CresusComptaEcritureMultiple ()
				{
					Date  = new System.DateTime (2011, 7, 26),
					Pièce = "13",
				};

				var simple = new CresusComptaEcritureSimple ()
				{
					Crédit  = "1000",
					Débit   = "2000",
					Libellé = "Virement 3",
					Montant = 20.50M,
				};

				multiple.Ecritures.Add (simple);

				écritures.Add (multiple);
			}

			{
				var multiple = new CresusComptaEcritureMultiple ()
				{
					Date  = new System.DateTime (2011, 4, 1),
					Pièce = "12",
				};

				{
					var simple = new CresusComptaEcritureSimple ()
					{
						Crédit  = "1000",
						Libellé = "Virement 4.1",
						Montant = 11.00M,
					};

					multiple.Ecritures.Add (simple);
				}

				{
					var simple = new CresusComptaEcritureSimple ()
					{
						Crédit  = "1010",
						Libellé = "Virement 4.2",
						Montant = 22.00M,
					};

					multiple.Ecritures.Add (simple);
				}

				{
					var simple = new CresusComptaEcritureSimple ()
					{
						Débit   = "2000",
						Libellé = "Virement 4.3",
						Montant = 33.00M,
					};

					multiple.Ecritures.Add (simple);
				}

				écritures.Add (multiple);
			}

			var journal = new CresusComptaJournal (chart.Path.FullPath, 123456789);
			var result = journal.GenerateFiles (écritures, chart.BeginDate.ToDateTime (), chart.EndDate.ToDateTime ());

			if (result == CresusToolkit.CresusComptaJournalError.Ok)
			{
				return null;  // ok
			}
			else
			{
				return result.ToString ();
			}
		}
	}
}