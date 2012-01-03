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
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Business.Finance.Comptabilité;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComptabilitéControllers
{
	/// <summary>
	/// Ce contrôleur gère le plan comptable de la comptabilité.
	/// </summary>
	public class PlanComptableController : AbstractController<PlanComptableColumn, ComptabilitéCompteEntity>
	{
		public PlanComptableController(TileContainer tileContainer, ComptabilitéEntity comptabilitéEntity)
			: base (tileContainer, comptabilitéEntity)
		{
			this.dataAccessor = new PlanComptableAccessor (this.comptabilitéEntity);

			this.columnMappers = new List<AbstractColumnMapper<PlanComptableColumn>> ();
			foreach (var mapper in this.ColumnMappers)
			{
				this.columnMappers.Add (mapper);
			}
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

			if (mapper.Column == PlanComptableColumn.Titre)
			{
				var compte = this.dataAccessor.SortedList[row];

				for (int i = 0; i < compte.Niveau; i++)
				{
					text = FormattedText.Concat (UIBuilder.leftIndentText, text);
				}
			}

			return text;
		}


		protected override void CreateFooter(FrameBox parent)
		{
			this.footerController = new PlanComptableFooterController (this.tileContainer, this.comptabilitéEntity, this.dataAccessor, this.columnMappers, this.arrayController);
			this.footerController.CreateUI (parent, this.UpdateArrayContent);
		}

		protected override void FinalizeFooter(FrameBox parent)
		{
			this.footerController.FinalizeUI (parent);
		}

	
		#region Column Mappers
		private IEnumerable<ColumnMapper> ColumnMappers
		{
			get
			{
				yield return new ColumnMapper (PlanComptableColumn.Numéro,         this.ValidateNuméro,         0.20, "Numéro",          "Numéro du compte");
				yield return new ColumnMapper (PlanComptableColumn.Titre,          this.ValidateTitre,          0.60, "Titre du compte", "Titre du compte");
				yield return new ColumnMapper (PlanComptableColumn.Catégorie,      this.ValidateCatégorie,      0.20, "Catégorie",       "Catégorie du compte");
				yield return new ColumnMapper (PlanComptableColumn.Type,           this.ValidateType,           0.20, "Type",            "Type du compte");
				yield return new ColumnMapper (PlanComptableColumn.Groupe,         this.ValidateGroupe,         0.20, "Groupe",          "Numéro du compte servant à regrouper celui-ci");
				yield return new ColumnMapper (PlanComptableColumn.TVA,            this.ValidateTVA,            0.15, "TVA",             "Choix pour la TVA");
				yield return new ColumnMapper (PlanComptableColumn.CompteOuvBoucl, this.ValidateCompteOuvBoucl, 0.20, "Ouv/Boucl",       "Numéro de compte utilisé lors des bouclements ou réouvertures");
				yield return new ColumnMapper (PlanComptableColumn.IndexOuvBoucl,  this.ValidateIndexOuvBoucl,  0.05, "",                "Ordre utilisé lors des bouclements ou réouvertures");
				yield return new ColumnMapper (PlanComptableColumn.Monnaie,        null,                        0.20, "Monnaie",         "Monnaie de ce compte");
			}
		}

		private class ColumnMapper : AbstractColumnMapper<PlanComptableColumn>
		{
			public ColumnMapper(PlanComptableColumn column, ValidateFunction validate, double relativeWidth, FormattedText description, FormattedText tooltip)
				: base (column, validate, relativeWidth, description, tooltip)
			{
			}
		}
		#endregion


		#region Validators
		private FormattedText ValidateNuméro(PlanComptableColumn column, ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return "Il manque le numéro du compte";
			}

			if (text.ToSimpleText ().Contains (' '))
			{
				return "Le numéro du compte ne peut pas contenir d'espace";
			}

			var t = text;
			var compte = this.comptabilitéEntity.PlanComptable.Where (x => x.Numéro == t).FirstOrDefault ();
			if (compte == null)
			{
				return FormattedText.Empty;
			}

			var himself = (this.footerController.JustCreate) ? null : this.arrayController.SelectedEntity;
			if (himself != null && himself.Numéro == text)
			{
				return FormattedText.Empty;
			}

			return "Ce numéro de compte existe déjà";
		}

		private FormattedText ValidateTitre(PlanComptableColumn column, ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return "Il manque le titre du compte";
			}
			else
			{
				return FormattedText.Empty;
			}
		}

		private FormattedText ValidateCatégorie(PlanComptableColumn column, ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return "Il manque la catégorie du compte";
			}

			CatégorieDeCompte catégorie;
			if (PlanComptableAccessor.TextToCatégorie (text, out catégorie))
			{
				return FormattedText.Empty;
			}
			else
			{
				return "La catégorie du compte n'est pas correcte";
			}
		}

		private FormattedText ValidateType(PlanComptableColumn column, ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return "Il manque le type du compte";
			}

			TypeDeCompte type;
			if (PlanComptableAccessor.TextToType (text, out type))
			{
				return FormattedText.Empty;
			}
			else
			{
				return "Le type du compte n'est pas correct";
			}
		}

		private FormattedText ValidateTVA(PlanComptableColumn column, ref FormattedText text)
		{
			return FormattedText.Empty;  //?
			if (text.IsNullOrEmpty)
			{
				return "Il manque la TVA du compte";
			}

			VatCode tva;
			if (PlanComptableAccessor.TextToTVA (text, out tva))
			{
				return FormattedText.Empty;
			}
			else
			{
				return "La TVA du compte n'est pas correcte";
			}
		}

		private FormattedText ValidateGroupe(PlanComptableColumn column, ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return FormattedText.Empty;
			}

			var n = PlanComptableAccessor.GetCompteNuméro (text);
			var compte = this.comptabilitéEntity.PlanComptable.Where (x => x.Numéro == n).FirstOrDefault ();
			if (compte == null)
			{
				return "Ce compte n'existe pas";
			}

			if (compte.Type != TypeDeCompte.Groupe)
			{
				return "Ce n'est pas un compte de groupement";
			}

			text = n;
			return FormattedText.Empty;
		}

		private FormattedText ValidateCompteOuvBoucl(PlanComptableColumn column, ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return FormattedText.Empty;
			}

			var n = PlanComptableAccessor.GetCompteNuméro (text);
			var compte = this.comptabilitéEntity.PlanComptable.Where (x => x.Numéro == n).FirstOrDefault ();
			if (compte == null)
			{
				return "Ce compte n'existe pas";
			}

			if (compte.Type != TypeDeCompte.Normal)
			{
				return "Ce compte n'a pas le type \"Normal\"";
			}

			if (compte.Catégorie != CatégorieDeCompte.Exploitation)
			{
				return "Ce n'est pas un compte d'exploitation";
			}

			text = n;
			return FormattedText.Empty;
		}

		private FormattedText ValidateIndexOuvBoucl(PlanComptableColumn column, ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return FormattedText.Empty;
			}

			int n;
			if (int.TryParse (text.ToSimpleText (), out n))
			{
				if (n >= 1 && n <= 9)
				{
					return FormattedText.Empty;
				}
			}

			return "Vous devez donner un numéro d'ordre compris entre 1 et 9";
		}
		#endregion


		private void PlanComptableImport()
		{
			this.comptabilitéEntity.Journal.Clear ();
			this.comptabilitéEntity.PlanComptable.Clear ();

			var businessSettings = this.tileContainer.Controller.BusinessContext.GetCachedBusinessSettings ();
			var financeSettings  = businessSettings.Finance;

			System.Diagnostics.Debug.Assert (financeSettings != null);
			var chart = financeSettings.GetChartOfAccountsOrDefaultToNearest (this.tileContainer.Controller.BusinessContext.GetReferenceDate ());
			var comptes = Epsitec.Cresus.Core.Business.Accounting.CresusChartOfAccountsConnector.Import (chart);

			foreach (var c in comptes)
			{
				var compte = this.tileContainer.Controller.DataContext.CreateEntity<ComptabilitéCompteEntity> ();

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

			(this.footerController as PlanComptableFooterController).PlanComptableUpdate ();
			this.dataAccessor.UpdateSortedList ();
		}


	}
}
