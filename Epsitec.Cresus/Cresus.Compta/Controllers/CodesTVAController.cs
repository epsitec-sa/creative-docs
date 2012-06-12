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

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les codes TVA de la comptabilité.
	/// </summary>
	public class CodesTVAController : AbstractController
	{
		public CodesTVAController(ComptaApplication app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new CodesTVADataAccessor (this);
		}


		public override ControllerType ControllerType
		{
			get
			{
				return Controllers.ControllerType.CodesTVA;
			}
		}


		public override bool AcceptPériodeChanged
		{
			get
			{
				return false;
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
				yield return new ColumnMapper (ColumnType.Désactivé,     0.30, ContentAlignment.MiddleCenter, "Code actif",      "Ce code est actif");
				yield return new ColumnMapper (ColumnType.Code,          1.00, ContentAlignment.MiddleLeft,   "Code",            "Code unique");
				yield return new ColumnMapper (ColumnType.Titre,         5.00, ContentAlignment.MiddleLeft,   "Description",     "Description du code TVA");
				yield return new ColumnMapper (ColumnType.Taux,          1.00, ContentAlignment.MiddleLeft,   "Liste de taux",   "Liste de taux de TVA");
				yield return new ColumnMapper (ColumnType.Compte,        0.60, ContentAlignment.MiddleLeft,   "Compte",          "Compte de TVA");
																											  
				yield return new ColumnMapper (ColumnType.Chiffre,       0.00, ContentAlignment.MiddleLeft,   "Chiffre",         "Chiffre sur le formulaire de décompte TVA");
				yield return new ColumnMapper (ColumnType.MontantFictif, 0.00, ContentAlignment.MiddleRight,  "Montant fictif",  "Montant fictif pour le contrôle de la disposition");
			}
		}
	}
}
