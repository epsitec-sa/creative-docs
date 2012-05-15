//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Permanents;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère le plan comptable de la comptabilité.
	/// </summary>
	public class PlanComptableController : AbstractController
	{
		public PlanComptableController(ComptaApplication app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new PlanComptableDataAccessor (this);

			this.viewSettingsList = this.mainWindowController.GetViewSettingsList ("Présentation.Réglages.ViewSettings");
		}


		public override bool AcceptPériodeChanged
		{
			get
			{
				return false;
			}
		}


		public override bool HasSearchPanel
		{
			get
			{
				return true;
			}
		}

		public override bool HasFilterPanel
		{
			get
			{
				return true;
			}
		}

		public override bool HasOptionsPanel
		{
			get
			{
				return false;
			}
		}

		public override bool HasInfoPanel
		{
			get
			{
				return true;
			}
		}


		protected override void UpdateTitle()
		{
			this.SetGroupTitle ("Réglages");
			this.SetTitle ("Plan comptable");
			this.SetSubtitle ("Toutes les périodes");
		}


		protected override FormattedText GetArrayText(int row, ColumnType columnType)
		{
			//	Retourne le texte contenu dans une cellule.
			var text = this.dataAccessor.GetText (row, columnType);

			if (columnType == ColumnType.Titre)
			{
				var compte = this.dataAccessor.GetEditionEntity (row) as ComptaCompteEntity;

				for (int i = 0; i < compte.Niveau; i++)
				{
					text = FormattedText.Concat (UIBuilder.leftIndentText, text);
				}
			}

			return text;
		}


		protected override void CreateEditor(FrameBox parent)
		{
			this.editorController = new PlanComptableEditorController (this);
			this.editorController.CreateUI (parent, this.UpdateArrayContent);
			this.editorController.ShowInfoPanel = this.mainWindowController.ShowInfoPanel;
		}


		#region Context menu
		protected override VMenu ContextMenu
		{
			//	Retourne le menu contextuel à utiliser.
			get
			{
				var menu = new VMenu ();

				//?this.PutContextMenuCommand (menu, Res.Commands.Edit.Duplicate);
				//?this.PutContextMenuCommand (menu, Res.Commands.Edit.Delete);
				//?this.PutContextMenuSeparator (menu);
				this.PutContextMenuExtrait (menu);
				this.PutContextMenuBudget (menu);

				return menu;
			}
		}

		private void PutContextMenuExtrait(VMenu menu)
		{
			var compte = this.dataAccessor.GetEditionEntity (this.arrayController.SelectedRow) as ComptaCompteEntity;

			var item = this.PutContextMenuItem (menu, "Présentation.Extrait", string.Format ("Extrait du compte {0}", compte.Numéro));

			item.Clicked += delegate
			{
				var présentation = this.mainWindowController.ShowPrésentation (ControllerType.Extrait);

				var permanent = présentation.DataAccessor.Permanents as ExtraitDeComptePermanents;
				permanent.NuméroCompte = compte.Numéro;

				présentation.UpdateAfterChanged ();
			};
		}

		private void PutContextMenuBudget(VMenu menu)
		{
			var compte = this.dataAccessor.GetEditionEntity (this.arrayController.SelectedRow) as ComptaCompteEntity;

			var item = this.PutContextMenuItem (menu, "Présentation.Budgets", string.Format ("Budgets du compte {0}", compte.Numéro));

			item.Clicked += delegate
			{
				var présentation = this.mainWindowController.ShowPrésentation (ControllerType.Budgets);

				int row = (présentation.DataAccessor as BudgetsDataAccessor).GetIndexOf (compte);
				if (row != -1)
				{
					présentation.SelectedArrayLine = row;
				}
			};
		}
		#endregion


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Numéro,         0.20, "Numéro",          "Numéro du compte");
				yield return new ColumnMapper (ColumnType.Titre,          0.60, "Titre du compte", "Titre du compte");
				yield return new ColumnMapper (ColumnType.Catégorie,      0.20, "Catégorie",       "Catégorie du compte");
				yield return new ColumnMapper (ColumnType.Type,           0.20, "Type",            "Type du compte");
				yield return new ColumnMapper (ColumnType.Groupe,         0.20, "Groupe",          "Numéro du compte servant à regrouper celui-ci");
				yield return new ColumnMapper (ColumnType.CodeTVA,        0.20, "Code TVA",        "Code TVA par défaut");
				yield return new ColumnMapper (ColumnType.CodesTVA,       0.30, "Codes TVA",       "Codes TVA possibles");
				yield return new ColumnMapper (ColumnType.CompteOuvBoucl, 0.20, "Ouv/Boucl",       "Numéro de compte utilisé lors des bouclements ou réouvertures");
				yield return new ColumnMapper (ColumnType.IndexOuvBoucl,  0.05, "",                "Ordre utilisé lors des bouclements ou réouvertures");
				yield return new ColumnMapper (ColumnType.Monnaie,        0.20, "Monnaie",         "Monnaie de ce compte");

				yield return new ColumnMapper (ColumnType.Profondeur, 0.20, ContentAlignment.MiddleLeft, "Profondeur", show: false);
			}
		}

		protected override void UpdateColumnMappers()
		{
			bool hasTVA = this.settingsList.GetBool (SettingsType.EcritureTVA);

			this.ShowHideColumn (ColumnType.CodeTVA, hasTVA);
		}
	}
}
