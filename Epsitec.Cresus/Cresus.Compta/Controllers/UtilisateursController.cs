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
	/// Ce contrôleur gère les utilisateurs de la comptabilité.
	/// </summary>
	public class UtilisateursController : AbstractController
	{
		public UtilisateursController(ComptaApplication app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new UtilisateursDataAccessor (this);
		}


		public override ControllerType ControllerType
		{
			get
			{
				return Controllers.ControllerType.Utilisateurs;
			}
		}

		protected override void UpdateTitle()
		{
			this.SetTitle ();
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
				return 40;
			}
		}

		protected override FormattedText GetArrayText(int row, ColumnType columnType)
		{
			//	Retourne le texte contenu dans une cellule.
			return this.dataAccessor.GetText (row, columnType);
		}


		protected override void CreateEditor(FrameBox parent)
		{
			this.editorController = new UtilisateursEditorController (this);
			this.editorController.CreateUI (parent, this.UpdateArrayContent);
			this.editorController.ShowInfoPanel = this.mainWindowController.ShowInfoPanel;
		}


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Icône,           0.30, ContentAlignment.MiddleCenter, "MdP", edition: false);
				yield return new ColumnMapper (ColumnType.Utilisateur,     1.00, ContentAlignment.MiddleLeft,   "Identité de l'utilisateur",            "Nom de l'utilisateur utilisé pour l'identification");
				yield return new ColumnMapper (ColumnType.NomComplet,      1.00, ContentAlignment.MiddleLeft,   "Nom complet",                          "Nom complet de l'utilisateur (prénom et nom usuels)");
				yield return new ColumnMapper (ColumnType.DateDébut,       0.00, ContentAlignment.MiddleLeft,   "Dates de début et de fin de validité", "Date de début de validité (facultatif)");
				yield return new ColumnMapper (ColumnType.DateFin,         0.00, ContentAlignment.MiddleLeft,   "",                                     "Date de fin de validité (facultatif)");
				yield return new ColumnMapper (ColumnType.IdentitéWindows, 0.00, ContentAlignment.MiddleLeft,   "Utilise l'identité Windows",           "Une coche indique que l'identification est automatique, si elle<br/>correspond à l'identité de l'utilisateur de la session Windows");
				yield return new ColumnMapper (ColumnType.Désactivé,       0.00, ContentAlignment.MiddleLeft,   "Utilisateur désactivé",                "Une coche indique que cet utilisateur ne peut plus s'identifier");
				yield return new ColumnMapper (ColumnType.MotDePasse,      0.00, ContentAlignment.MiddleLeft,   "Mot de passe",                         "Mot de passe de l'utilisateur (facultatif)");
				yield return new ColumnMapper (ColumnType.Pièce,           0.00, ContentAlignment.MiddleLeft,   "Générateur de pièces",                 "Générateur pour les numéros de pièces (facultatif)");
				yield return new ColumnMapper (ColumnType.Résumé,          1.00, ContentAlignment.MiddleLeft,   "Résumé", edition: false);
			}
		}


	}
}
