//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Accounting
{
	public static class CresusChartOfAccountsConnector
	{
		public static CresusChartOfAccounts Load(string path)
		{
			var dir = System.IO.Path.GetDirectoryName (path);
			var name = System.IO.Path.GetFileNameWithoutExtension (path) + ".crp";

			path = System.IO.Path.Combine (dir, name);

			//	Try to read the CRP file, which stores the chart of accounts of the CRE
			//	file :

			var doc = new Epsitec.CresusToolkit.CresusComptaDocument (path);

			if (doc.CheckMetadata ())
			{
				var cresus = new CresusChartOfAccounts ()
				{
					Title     = FormattedText.FromSimpleText (doc.Title),
					BeginDate = new Date (doc.BeginDate),
					EndDate   = new Date (doc.EndDate),
					Path      = new Epsitec.Common.IO.MachineFilePath (path),
					Id        = System.Guid.NewGuid (),
				};

				cresus.Items.AddRange (doc.GetAccounts ().Select (x => new BookAccountDefinition (x)));

				return cresus;
			}

			return null;
		}

		public static List<ComptabilitéCompteEntity> Import(CresusChartOfAccounts chart)
		{
			var comptes = new List<ComptabilitéCompteEntity> ();

			var doc = new Epsitec.CresusToolkit.CresusComptaDocument (chart.Path.FullPath);

			if (doc.CheckMetadata ())
			{
				foreach (var account in doc.GetAccountsTitlesAndGroups ())
				{
					if (string.IsNullOrEmpty (account.Caption))
					{
						continue;
					}

					var compte = new ComptabilitéCompteEntity ();

					compte.Numéro        = account.Number;
					compte.Titre         = account.Caption;
					compte.IndexOuvBoucl = account.OpenCloseIndex;

					switch (account.Category)
					{
						case CresusToolkit.CresusComptaCategory.Actif:
							compte.Catégorie = Finance.Comptabilité.CatégorieDeCompte.Actif;
							break;

						case CresusToolkit.CresusComptaCategory.Passif:
							compte.Catégorie = Finance.Comptabilité.CatégorieDeCompte.Passif;
							break;

						case CresusToolkit.CresusComptaCategory.Charge:
							compte.Catégorie = Finance.Comptabilité.CatégorieDeCompte.Charge;
							break;

						case CresusToolkit.CresusComptaCategory.Produit:
							compte.Catégorie = Finance.Comptabilité.CatégorieDeCompte.Produit;
							break;

						case CresusToolkit.CresusComptaCategory.Exploitation:
							compte.Catégorie = Finance.Comptabilité.CatégorieDeCompte.Exploitation;
							break;

						default:
							compte.Catégorie = Finance.Comptabilité.CatégorieDeCompte.Inconnu;
							break;
					}

					switch (account.Status)
					{
						case CresusToolkit.CresusComptaStatus.Account:
							compte.Type = Finance.Comptabilité.TypeDeCompte.Normal;
							break;

						case CresusToolkit.CresusComptaStatus.Group:
							compte.Type = Finance.Comptabilité.TypeDeCompte.Groupe;
							break;

						case CresusToolkit.CresusComptaStatus.Title:
							compte.Type = Finance.Comptabilité.TypeDeCompte.Titre;
							break;

						default:
							compte.Type = Finance.Comptabilité.TypeDeCompte.Normal;
							break;
					}

					comptes.Add (compte);
				}

				foreach (var account in doc.GetAccountsTitlesAndGroups ())
				{
					var compte = comptes.Where (x => x.Numéro == account.Number).FirstOrDefault ();

					if (!string.IsNullOrEmpty (account.Group))
					{
						var g = comptes.Where (x => x.Numéro == account.Group).FirstOrDefault ();
						if (g != null)
						{
							compte.Groupe = g;
						}
					}

					if (!string.IsNullOrEmpty (account.OpenClose))
					{
						var o = comptes.Where (x => x.Numéro == account.OpenClose).FirstOrDefault ();
						if (o != null)
						{
							compte.CompteOuvBoucl = o;
						}
					}
				}
			}

			return comptes;
		}
	}
}