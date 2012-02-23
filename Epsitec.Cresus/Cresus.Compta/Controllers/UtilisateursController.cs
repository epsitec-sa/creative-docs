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
	/// Ce contrôleur gère les utilisateurs de la comptabilité.
	/// </summary>
	public class UtilisateursController : AbstractController
	{
		public UtilisateursController(Application app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new UtilisateursDataAccessor (this);

			this.memoryList = this.mainWindowController.GetMemoryList ("Présentation.Utilisateurs.Memory");
		}


		protected override void UpdateTitle()
		{
			this.SetTitle ("Utilisateurs");
		}


		public override bool AcceptPériodeChanged
		{
			get
			{
				return false;
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


		protected override void CreateFooter(FrameBox parent)
		{
			this.footerController = new UtilisateursFooterController (this);
			this.footerController.CreateUI (parent, this.UpdateArrayContent);
			this.footerController.ShowInfoPanel = this.mainWindowController.ShowInfoPanel;
		}


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Utilisateur, 1.00, ContentAlignment.MiddleLeft, "Utilisateur",          "Nom de l'utilisateur utilisé pour l'identification");
				yield return new ColumnMapper (ColumnType.NomComplet,  1.00, ContentAlignment.MiddleLeft, "Nom complet",          "Nom complet (facultatif)");
				yield return new ColumnMapper (ColumnType.DateDébut,   0.50, ContentAlignment.MiddleLeft, "Date début",           "Date de début de validité (facultatif)");
				yield return new ColumnMapper (ColumnType.DateFin,     0.50, ContentAlignment.MiddleLeft, "Date Fin",             "Date de fin de validité (facultatif)");
				yield return new ColumnMapper (ColumnType.MotDePasse,  0.60, ContentAlignment.MiddleLeft, "Mot de passe",         "Mot de passe de l'utilisateur (facultatif)");
				yield return new ColumnMapper (ColumnType.Pièce,       0.80, ContentAlignment.MiddleLeft, "Générateur de pièces", "Générateur pour les numéros de pièces (facultatif)");
				yield return new ColumnMapper (ColumnType.Résumé,      1.00, ContentAlignment.MiddleLeft, "Résumé");
			}
		}
	}
}
