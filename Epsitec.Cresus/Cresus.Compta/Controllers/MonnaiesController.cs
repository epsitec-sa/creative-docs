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
	/// Ce contrôleur gère les monnaies de la comptabilité.
	/// </summary>
	public class MonnaiesController : AbstractController
	{
		public MonnaiesController(ComptaApplication app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new MonnaiesDataAccessor (this);
		}


		protected override void UpdateTitle()
		{
			this.SetGroupTitle (Présentations.GetGroupName (ControllerType.Monnaies));
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
				return false;
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
			this.editorController = new MonnaiesEditorController (this);
			this.editorController.CreateUI (parent, this.UpdateArrayContent);
			this.editorController.ShowInfoPanel = this.mainWindowController.ShowInfoPanel;
		}


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Code,        0.40, ContentAlignment.MiddleLeft, "Monnaie",          "Code ISO à 3 lettres de la monnaie");
				yield return new ColumnMapper (ColumnType.Description, 1.00, ContentAlignment.MiddleLeft, "Description",      "Description de la monnaie");
				yield return new ColumnMapper (ColumnType.Décimales,   0.40, ContentAlignment.MiddleLeft, "Décimales",        "Nombre de décimales de la monnaie");
				yield return new ColumnMapper (ColumnType.Arrondi,     0.40, ContentAlignment.MiddleLeft, "Arrondi",          "Plus petite valeur possible");
				yield return new ColumnMapper (ColumnType.Cours,       0.50, ContentAlignment.MiddleLeft, "Cours",            "Cours de la monnaie");
				yield return new ColumnMapper (ColumnType.Unité,       0.40, ContentAlignment.MiddleLeft, "Unité",            "Facteur multiplicafif du cours");
				yield return new ColumnMapper (ColumnType.CompteGain,  0.60, ContentAlignment.MiddleLeft, "Gains de change",  "Compte pour les gains de change");
				yield return new ColumnMapper (ColumnType.ComptePerte, 0.60, ContentAlignment.MiddleLeft, "Pertes de change", "Compte pour les pertes de change");
			}
		}
	}
}
