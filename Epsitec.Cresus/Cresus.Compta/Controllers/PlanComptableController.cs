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

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère le plan comptable de la comptabilité.
	/// </summary>
	public class PlanComptableController : AbstractController
	{
		public PlanComptableController(Application app, BusinessContext businessContext, ComptaEntity comptaEntity, MainWindowController mainWindowController)
			: base (app, businessContext, comptaEntity, mainWindowController)
		{
			this.dataAccessor = new PlanComptableDataAccessor (this);
		}


		public override bool HasShowSearchPanel
		{
			get
			{
				return true;
			}
		}

		public override bool HasShowFilterPanel
		{
			get
			{
				return true;
			}
		}

		public override bool HasShowOptionsPanel
		{
			get
			{
				return false;
			}
		}

		public override bool HasShowInfoPanel
		{
			get
			{
				return true;
			}
		}


		protected override void UpdateTitle()
		{
			this.SetTitle ("Plan comptable");
		}


		protected override FormattedText GetArrayText(int row, ColumnType columnType)
		{
			//	Retourne le texte contenu dans une cellule.
			var text = this.dataAccessor.GetText (row, columnType);

			if (columnType == ColumnType.Titre)
			{
				var compte = this.dataAccessor.GetEditionData (row) as ComptaCompteEntity;

				for (int i = 0; i < compte.Niveau; i++)
				{
					text = FormattedText.Concat (UIBuilder.leftIndentText, text);
				}
			}

			return text;
		}


		protected override void CreateFooter(FrameBox parent)
		{
			this.footerController = new PlanComptableFooterController (this);
			this.footerController.CreateUI (parent, this.UpdateArrayContent);
			this.footerController.ShowInfoPanel = this.ShowInfoPanel;
		}


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
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

				yield return new ColumnMapper (ColumnType.Profondeur, 0.20, ContentAlignment.MiddleLeft, "Profondeur", show: false);
			}
		}
	}
}
