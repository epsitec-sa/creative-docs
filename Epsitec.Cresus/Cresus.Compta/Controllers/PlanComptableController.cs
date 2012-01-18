//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta
{
	/// <summary>
	/// Ce contrôleur gère le plan comptable de la comptabilité.
	/// </summary>
	public class PlanComptableController : AbstractController
	{
		public PlanComptableController(CoreApp app, BusinessContext businessContext, ComptabilitéEntity comptabilitéEntity, List<AbstractController> controllers)
			: base (app, businessContext, comptabilitéEntity, controllers)
		{
			this.dataAccessor = new PlanComptableDataAccessor (this.businessContext, this.comptabilitéEntity);
			this.InitializeColumnMapper ();
		}


		protected override void FinalUpdate()
		{
			base.FinalUpdate ();
			this.topToolbarController.ImportEnable = true;
		}

		protected override void ImportAction()
		{
			this.PlanComptableImport ();
			this.UpdateArrayContent ();
			this.footerController.UpdateFooterContent ();
		}


		protected override FormattedText GetArrayText(int row, int column)
		{
			//	Retourne le texte contenu dans une cellule.
			var mapper = this.columnMappers[column];
			var text = this.dataAccessor.GetText (row, mapper.Column);

			if (mapper.Column == ColumnType.Titre)
			{
				var compte = this.dataAccessor.GetEditionData (row) as ComptabilitéCompteEntity;

				for (int i = 0; i < compte.Niveau; i++)
				{
					text = FormattedText.Concat (UIBuilder.leftIndentText, text);
				}
			}

			return text;
		}


		protected override void CreateFooter(FrameBox parent)
		{
			this.footerController = new PlanComptableFooterController (this.app, this.businessContext, this.comptabilitéEntity, this.dataAccessor, this.columnMappers, this, this.arrayController);
			this.footerController.CreateUI (parent, this.UpdateArrayContent);
		}

		protected override void FinalizeFooter(FrameBox parent)
		{
			this.footerController.FinalizeUI (parent);
		}

		public override void UpdateData()
		{
		}


		protected override IEnumerable<ColumnMapper> ColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Numéro,         0.20, "Numéro",          "Numéro du compte");
				yield return new ColumnMapper (ColumnType.Titre,          0.60, "Titre du compte", "Titre du compte");
				yield return new ColumnMapper (ColumnType.Catégorie,      0.20, "Catégorie",       "Catégorie du compte");
				yield return new ColumnMapper (ColumnType.Type,           0.20, "Type",            "Type du compte");
				yield return new ColumnMapper (ColumnType.Groupe,         0.20, "Groupe",          "Numéro du compte servant à regrouper celui-ci");
				yield return new ColumnMapper (ColumnType.TVA,            0.15, "TVA",             "Choix pour la TVA");
				yield return new ColumnMapper (ColumnType.CompteOuvBoucl, 0.20, "Ouv/Boucl",       "Numéro de compte utilisé lors des bouclements ou réouvertures");
				yield return new ColumnMapper (ColumnType.IndexOuvBoucl,  0.05, "",                "Ordre utilisé lors des bouclements ou réouvertures");
				yield return new ColumnMapper (ColumnType.Monnaie,        0.20, "Monnaie",         "Monnaie de ce compte");
			}
		}


		private void PlanComptableImport()
		{
#if false
			this.comptabilitéEntity.Journal.Clear ();
			this.comptabilitéEntity.PlanComptable.Clear ();

			var businessSettings = this.businessContext.GetCachedBusinessSettings ();
			var financeSettings  = businessSettings.Finance;

			System.Diagnostics.Debug.Assert (financeSettings != null);
			var chart = financeSettings.GetChartOfAccountsOrDefaultToNearest (this.businessContext.GetReferenceDate ());
			var comptes = Epsitec.Cresus.Core.Business.Accounting.CresusChartOfAccountsConnector.Import (chart);

			foreach (var c in comptes)
			{
				var compte = this.businessContext.DataContext.CreateEntity<ComptabilitéCompteEntity> ();

				compte.Numéro         = c.Numéro;
				compte.Titre          = c.Titre;
				compte.Catégorie      = c.Catégorie;
				compte.Type           = c.Type;
//				compte.Groupe         = c.Groupe;
//				compte.CompteOuvBoucl = c.CompteOuvBoucl;
				compte.IndexOuvBoucl  = c.IndexOuvBoucl;

				this.comptabilitéEntity.PlanComptable.Add (compte);
			}

			foreach (var c in comptes)
			{
				if (c.Groupe != null && !c.Groupe.Numéro.IsNullOrEmpty)
				{
					var compte    = this.comptabilitéEntity.PlanComptable.Where (x => x.Numéro == c.Numéro).FirstOrDefault ();
					compte.Groupe = this.comptabilitéEntity.PlanComptable.Where (x => x.Numéro == c.Groupe.Numéro).FirstOrDefault ();
				}
			}

			foreach (var c in comptes)
			{
				if (c.CompteOuvBoucl != null && !c.CompteOuvBoucl.Numéro.IsNullOrEmpty)
				{
					var compte            = this.comptabilitéEntity.PlanComptable.Where (x => x.Numéro == c.Numéro).FirstOrDefault ();
					compte.CompteOuvBoucl = this.comptabilitéEntity.PlanComptable.Where (x => x.Numéro == c.CompteOuvBoucl.Numéro).FirstOrDefault ();
				}
			}

			this.comptabilitéEntity.UpdateNiveauCompte ();
#endif
		}
	}
}
