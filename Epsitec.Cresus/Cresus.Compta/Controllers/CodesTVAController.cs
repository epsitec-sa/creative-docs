//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les codes TVA de la comptabilité.
	/// </summary>
	public class CodesTVAController : AbstractController
	{
		public CodesTVAController(Application app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new CodesTVADataAccessor (this);

			this.viewSettingsList = this.mainWindowController.GetViewSettingsList ("Présentation.CodesTVA.ViewSettings");
		}


		protected override void UpdateTitle()
		{
			this.SetTitle ("Codes TVA");
		}


		public override bool AcceptPériodeChanged
		{
			get
			{
				return false;
			}
		}


		public override bool HasRightEditor
		{
			get
			{
				return true;
			}
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
				return false;
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


		protected override int ArrayLineHeight
		{
			get
			{
				return 20;
			}
		}

		protected override FormattedText GetArrayText(int row, ColumnType columnType)
		{
			//	Retourne le texte contenu dans une cellule.
			return this.dataAccessor.GetText (row, columnType);
		}


		protected override void CreateEditor(FrameBox parent)
		{
			this.editorController = new CodesTVAEditorController (this);
			this.editorController.CreateUI (parent, this.UpdateArrayContent);
			this.editorController.ShowInfoPanel = this.mainWindowController.ShowInfoPanel;
		}


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Code,           1.00, ContentAlignment.MiddleLeft,  "Code",            "Code unique");
				yield return new ColumnMapper (ColumnType.Titre,          5.00, ContentAlignment.MiddleLeft,  "Description",     "Description du code TVA");
				yield return new ColumnMapper (ColumnType.Taux1,          0.50, ContentAlignment.MiddleRight, "Taux 1",          "Premier taux en pourcents");
				yield return new ColumnMapper (ColumnType.Taux2,          0.50, ContentAlignment.MiddleRight, "Taux 2",          "Deuxième taux en pourcents");
				yield return new ColumnMapper (ColumnType.Compte,         1.00, ContentAlignment.MiddleLeft,  "Compte TVA",      "Compte TVA");

				yield return new ColumnMapper (ColumnType.Chiffre,        0.00, ContentAlignment.MiddleLeft,  "Chiffre",         "Chiffre sur le formulaire de décompte TVA");
				yield return new ColumnMapper (ColumnType.MontantFictif,  0.00, ContentAlignment.MiddleRight, "Montant fictif",  "Montant fictif pour le contrôle de la disposition");

				yield return new ColumnMapper (ColumnType.ParDéfaut,      0.00, ContentAlignment.MiddleRight, "Code par défaut", "Ce code est le code par défaut");
				yield return new ColumnMapper (ColumnType.Désactivé,      0.00, ContentAlignment.MiddleRight, "Code désactivé",  "Ce code n'est provisoirement plus disponible");
			}
		}
	}
}
